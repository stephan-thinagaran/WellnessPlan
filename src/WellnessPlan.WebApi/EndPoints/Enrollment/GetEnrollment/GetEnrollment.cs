using FluentValidation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using WellnessPlan.Shared.Messaging;

namespace WellnessPlan.WebApi.EndPoints.Enrollment.GetEnrollment;

internal static class GetEnrollment
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Methods should not have too many parameters", Justification = "Reason for ignoring this rule")]
    internal class GetEnrollmentQueryHandler : IQueryHandler<GetEnrollmentQuery, GetEnrollmentResponse>
    {
        private readonly IMemoryCache cache;

        public GetEnrollmentQueryHandler(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public async Task<GetEnrollmentResponse?> Handle(GetEnrollmentQuery request, CancellationToken cancellationToken)
        {
            var enrollment = await cache.GetOrCreateAsync($"{Constant.GetEnrollmentCacheKey}{request.EnrollmentId}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

                await Task.Run(() => Console.Write("Test"), CancellationToken.None);
                return new GetEnrollmentResponse { EnrollmentId = request.EnrollmentId, PlanName = "Test Plan" };
            });

            return enrollment;
        }
    }

    public class EndPoint : IEndPoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/enrollments/{enrollmentId:guid}", async ([FromRoute] Guid enrollmentId, 
                                                                      [FromServices] IQueryDispatcher queryDispatcher, 
                                                                      [FromServices] IValidator<GetEnrollmentQuery> validator, 
                                                                      CancellationToken cancellationToken) =>
            {
                var query = new GetEnrollmentQuery { EnrollmentId = enrollmentId };
                var validationResult = await validator.ValidateAsync(query, cancellationToken);
                if(!validationResult.IsValid)
                    return Results.BadRequest(validationResult.Errors);

                var response = await queryDispatcher.Dispatch<GetEnrollmentQuery, GetEnrollmentResponse>(query, cancellationToken);
                return response is null ? Results.NotFound() : Results.Ok(response);
            });
        }
    }

    internal class GetEnrollmentQuery : IQuery
    {
        public Guid EnrollmentId { get; set; }
    }

    internal sealed class GetEnrollmentResponse : IQueryResult
    {
        public Guid EnrollmentId { get; set; }

        public string PlanName { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<GetEnrollmentQuery>
    {
        public Validator()
        {
            RuleFor(x => x.EnrollmentId).NotEmpty().WithMessage("EnrollmentId Cannot be Empty");
        }
    }

    internal static IServiceCollection RegisterGetEnrollmentDependencies(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetEnrollmentQuery, GetEnrollmentResponse>, GetEnrollmentQueryHandler>();
        services.AddScoped<IValidator<GetEnrollmentQuery>, Validator>();
        return services;
    }

    public static class Constant
    {
        public const string GetEnrollmentCacheKey = "GetEnrollment_";
    }

}
