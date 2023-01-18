using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TestJob.Application;

public static class ServiceCollection
{
    public static void AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollection).GetTypeInfo().Assembly;
        services.AddMediatR(assembly);
    }
}