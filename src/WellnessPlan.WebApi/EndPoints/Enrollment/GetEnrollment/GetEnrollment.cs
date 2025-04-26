using MediatR;
using WellnessPlan.Shared.Messaging;

namespace WellnessPlan.WebApi.EndPoints.Enrollment.GetEnrollment;

internal static class GetEnrollment
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Methods should not have too many parameters", Justification = "Reason for ignoring this rule")]
    internal class GetEnrollmentQueryHandler : IQueryHandler<GetEnrollmentQuery, GetEnrollmentResponse>
    {
        public async Task<GetEnrollmentResponse> Handle(GetEnrollmentQuery request, CancellationToken cancellationToken)
        {
            var enrollment = new GetEnrollmentResponse { EnrollmentId = request.EnrollmentId, PlanName = "Test Plan" };
            await Task.Run(()=> Console.Write("Test"), CancellationToken.None );
            return await Task.FromResult(enrollment);
        }
    }

    public class EndPoint : IEndPoint
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/enrollments/{enrollmentId:guid}", async (Guid enrollmentId, IQueryDispatcher queryDispatcher, CancellationToken cancellationToken) =>
            {
                var query = new GetEnrollmentQuery { EnrollmentId = enrollmentId };
                var response = await queryDispatcher.Dispatch<GetEnrollmentQuery, GetEnrollmentResponse>(query, cancellationToken);
                return Results.Ok(response);
            });
        }
    }

    internal sealed class GetEnrollmentQuery : IRequest<GetEnrollmentResponse>
    {
        public Guid EnrollmentId { get; set; }
    }

    internal sealed class GetEnrollmentResponse
    {
        public Guid EnrollmentId { get; set; }

        public string PlanName { get; set; } = string.Empty;
    }

    internal static IServiceCollection RegisterGetEnrollmentDependencies(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetEnrollmentQuery, GetEnrollmentResponse>, GetEnrollmentQueryHandler>();
        return services;
    }

}
