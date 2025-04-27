
using FluentValidation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json;

using WellnessPlan.Infrastructure.Database;
using WellnessPlan.Shared.Messaging;
using Entity = WellnessPlan.Infrastructure.Database.AdventureWorks.Entities;

namespace WellnessPlan.WebApi.EndPoints.HumanResources.Employee;

public static class GetEmployee
{

    public class GetEmployeeQueryHandler : IQueryHandler<GetEmployeeQuery, GetEmployeeResponse>
    {
        private readonly IMemoryCache cache;
        private readonly SqlRepository<Entity.Employee> employeeRepo;

        public GetEmployeeQueryHandler(IMemoryCache cache, SqlRepository<Entity.Employee> employeeRepo)
        {
            this.cache = cache;
            this.employeeRepo = employeeRepo;
        }

        public async Task<GetEmployeeResponse?> Handle(GetEmployeeQuery query, CancellationToken cancellationToken)
        {
            var cacheKey = $"{Constant.EmployeeIdCachePrefix}{query.NationalIDNumber}";
            if(cache.TryGetValue<string?>(cacheKey, out string? cachedResponseJson) 
                    && !string.IsNullOrWhiteSpace(cachedResponseJson))
            {   
                try
                {
                    var cachedResponse = JsonConvert.DeserializeObject<GetEmployeeResponse>(cachedResponseJson);
                    return cachedResponse;
                }
                catch(JsonSerializationException ex)
                {
                    // Log deserialization error, cache might be corrupt
                    // Consider removing the invalid cache entry: cache.Remove(cacheKey);
                    Console.WriteLine($"Error deserializing cached employee data for key {cacheKey}: {ex.Message}");
                }
            }

            var employees = await employeeRepo.FindAsync(x=>x.NationalIdnumber == query.NationalIDNumber).ConfigureAwait(false);
            var employee = employees.FirstOrDefault();

            if(employee == null)
                return null;

            var response = MapExtension.ToGetEmployeeResponse(employee);
            var responseJson = JsonConvert.SerializeObject(response);

            cache.Set<string>(cacheKey, responseJson);

            return response;
        }
    }

    public class EndPoint : IEndPoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/employees/{employeeNo}", async ([FromRoute] string employeeNo, 
                                                                      [FromServices] IQueryDispatcher queryDispatcher, 
                                                                      [FromServices] IValidator<GetEmployeeQuery> validator, 
                                                                      CancellationToken cancellationToken) =>
            {
                var query = new GetEmployeeQuery { NationalIDNumber = employeeNo };
                var validationResult = await validator.ValidateAsync(query, cancellationToken);
                if(!validationResult.IsValid)
                    return Results.BadRequest(validationResult.Errors);

                var response = await queryDispatcher.Dispatch<GetEmployeeQuery, GetEmployeeResponse>(query, cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            });
        }
    }

    public class Validator : AbstractValidator<GetEmployeeQuery>
    {
        public Validator()
        {
            RuleFor(x=> x.NationalIDNumber).NotNull().NotEmpty().WithMessage("EmployeeId Cannot be null/empty");
        }
    }

    public static IServiceCollection RegisterGetEmployeeDependencies(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetEmployeeQuery, GetEmployeeResponse>, GetEmployeeQueryHandler>();
        services.AddScoped<IValidator<GetEmployeeQuery>, Validator>();
        return services;
    }

    public class GetEmployeeQuery : IQuery
    {
        public string NationalIDNumber { get; set; } = string.Empty;
    }

    public class GetEmployeeResponse : IQueryResult
    {
        public int BusinessEntityId { get; set; }

        public string NationalIdnumber { get; set; } = null!;

        public string LoginId { get; set; } = null!;

        public short? OrganizationLevel { get; set; }

        public string JobTitle { get; set; } = null!;

        public DateOnly BirthDate { get; set; }

        public string MaritalStatus { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public DateOnly HireDate { get; set; }

        public bool SalariedFlag { get; set; }

        public short VacationHours { get; set; }

        public short SickLeaveHours { get; set; }

        public bool CurrentFlag { get; set; }

        public Guid Rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }
    }

    public static class Constant
    {
        public const string EmployeeIdCachePrefix = "NationalIDNumber";
    }

    public static class MapExtension
    {
        public static GetEmployeeResponse ToGetEmployeeResponse(Entity.Employee emp) => new GetEmployeeResponse
        {
            BusinessEntityId = emp.BusinessEntityId,
            NationalIdnumber = emp.NationalIdnumber,
            LoginId = emp.LoginId,
            OrganizationLevel = emp.OrganizationLevel,
            JobTitle = emp.JobTitle,
            BirthDate = emp.BirthDate,
            MaritalStatus = emp.MaritalStatus,
            Gender = emp.Gender,
            HireDate = emp.HireDate,
            SalariedFlag = emp.SalariedFlag,
            VacationHours = emp.VacationHours,
            SickLeaveHours = emp.SickLeaveHours,
            CurrentFlag = emp.CurrentFlag,
            Rowguid = emp.Rowguid,
            ModifiedDate = emp.ModifiedDate
        };
    }
}
