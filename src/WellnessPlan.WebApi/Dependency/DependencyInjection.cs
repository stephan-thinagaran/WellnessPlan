using Carter;

using WellnessPlan.Shared.Messaging;
using WellnessPlan.WebApi.EndPoints.Enrollment.GetEnrollment;
using WellnessPlan.WebApi.Infrastructure.Messaging;

namespace WellnessPlan.WebApi.Dependency;

public static class DependencyInjection
{

    // App Builder Dependencies
    internal static IHostApplicationBuilder CoreBuilder(this IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.AddServiceDefaults();
        hostApplicationBuilder.Services.RegisterDependencies();
        return hostApplicationBuilder;
    }

    internal static WebApplication MapServices(this WebApplication webApplication)
    {
        webApplication.MapCarter();
        return webApplication;
    }

    // All Dependecies
    internal static IServiceCollection RegisterDependencies(this IServiceCollection services)
    {
        services.RegisterCoreDependencies();
        services.RegisterUsecaseDependencies();
        return services;
    }

    // Core Dependencies
    internal static IServiceCollection RegisterCoreDependencies(this IServiceCollection services)
    {
        // var assembly = typeof(Program).Assembly;
        services.AddEndpointsApiExplorer();
        services.AddCarter();

        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        return services;

    }

    // Use Case Dependencies
    internal static IServiceCollection RegisterUsecaseDependencies(this IServiceCollection services)
    {
        services.RegisterGetEnrollmentDependencies();
        return services;
    }
}
