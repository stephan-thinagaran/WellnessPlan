using MediatR;
using Carter;

namespace WellnessPlan.WebApi.EndPoints.Enrollment.GetEnrollment;

internal static class GetEnrollment
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Methods should not have too many parameters", Justification = "Reason for ignoring this rule")]
    internal class GetEnrollmentQueryHandler : IRequestHandler<GetEnrollmentQuery, GetEnrollmentResponse>
    {
        public GetEnrollmentQueryHandler()
        {
        }

        public async Task<GetEnrollmentResponse> Handle(GetEnrollmentQuery request, CancellationToken cancellationToken)
        {
            var enrollment = new GetEnrollmentResponse { EnrollmentId = request.EnrollmentId, PlanName = "Test Plan" };
            await Task.Run(()=> Console.Write("Test"), CancellationToken.None );
            return await Task.FromResult(enrollment);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Methods should not have too many parameters", Justification = "Reason for ignoring this rule")]
    public class EndPoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/enrollments/{enrollmentId:guid}", async (Guid enrollmentId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var query = new GetEnrollmentQuery { EnrollmentId = enrollmentId };
                var response = await mediator.Send(query, cancellationToken).ConfigureAwait(false);
                return Results.Ok(response);
            });
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Methods should not have too many parameters", Justification = "Reason for ignoring this rule")]
    public class NewEndPoint : ICarterModule 
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/test", () => Results.Ok("Hello Test"));
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

}
