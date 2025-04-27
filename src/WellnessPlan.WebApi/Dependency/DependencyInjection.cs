using Carter;
using FluentValidation;
using WellnessPlan.Shared.Messaging;
using WellnessPlan.WebApi.EndPoints.Enrollment.GetEnrollment;
using WellnessPlan.WebApi.Extensions;
using WellnessPlan.WebApi.Infrastructure.Messaging;
using WellnessPlan.WebApi.Middleware;

namespace WellnessPlan.WebApi.Dependency;

public static class DependencyInjection
{

    // App Builder Dependencies
    internal static WebApplicationBuilder CoreBuilder(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.AddServiceDefaults();
        webApplicationBuilder.Services.RegisterDependencies();
        webApplicationBuilder.AddFluentValidationEndpointFilter();
        return webApplicationBuilder;
    }

    internal static WebApplication MapServices(this WebApplication webApplication)
    {
        webApplication.MapCarter();
        webApplication.UseMiddleware<RequestContextLoggingMiddleware>();
        webApplication.UseRequestContextLogging();
        webApplication.UseExceptionHandler();
        return webApplication;
    }

    // All Dependecies
    internal static IServiceCollection RegisterDependencies(this IServiceCollection services)
    {
        services.RegisterCoreDependencies();
        services.RegisterInfraDependencies();
        services.RegisterUsecaseDependencies();
        return services;
    }

    // Core Dependencies
    internal static IServiceCollection RegisterCoreDependencies(this IServiceCollection services)
    {
        var assembly = typeof(Program).Assembly;
        services.AddEndpointsApiExplorer();
        services.AddCarter();
        services.AddValidatorsFromAssembly(assembly);

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        return services;
    }

    internal static IServiceCollection RegisterInfraDependencies(this IServiceCollection services)
    {
        services.AddMemoryCache();
        return services;
    }

    // Use Case Dependencies
    internal static IServiceCollection RegisterUsecaseDependencies(this IServiceCollection services)
    {
        services.RegisterGetEnrollmentDependencies();
        return services;
    }
}
