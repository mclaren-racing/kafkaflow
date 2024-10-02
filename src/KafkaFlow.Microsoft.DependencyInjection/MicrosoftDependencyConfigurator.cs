using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaFlow;

internal class MicrosoftDependencyConfigurator : IDependencyConfigurator
{
    private readonly IServiceCollection _services;

    public MicrosoftDependencyConfigurator(IServiceCollection services)
    {
        _services = services;
        _services.AddSingleton<IDependencyResolver>(provider => new MicrosoftDependencyResolver(provider));
    }

    public IDependencyConfigurator Add(
        Type serviceType,
        Type implementationType,
        InstanceLifetime lifetime)
    {
        _services.Add(
            ServiceDescriptor.Describe(
                serviceType,
                implementationType,
                ParseLifetime(lifetime)));

        return this;
    }

    public IDependencyConfigurator Add<TService, TImplementation>(InstanceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService
    {
        _services.Add(
            ServiceDescriptor.Describe(
                typeof(TService),
                typeof(TImplementation),
                ParseLifetime(lifetime)));

        return this;
    }

    public IDependencyConfigurator Add<TService>(InstanceLifetime lifetime)
        where TService : class
    {
        _services.Add(
            ServiceDescriptor.Describe(
                typeof(TService),
                typeof(TService),
                ParseLifetime(lifetime)));

        return this;
    }

    public IDependencyConfigurator Add<TImplementation>(TImplementation service)
        where TImplementation : class
    {
        _services.AddSingleton(service);
        return this;
    }

    public IDependencyConfigurator Add<TImplementation>(
        Type serviceType,
        Func<IDependencyResolver, TImplementation> factory,
        InstanceLifetime lifetime)
    {
        _services.Add(
            ServiceDescriptor.Describe(
                serviceType,
                provider => factory(new MicrosoftDependencyResolver(provider)),
                ParseLifetime(lifetime)));

        return this;
    }

    public bool IsRegistered(Type serviceType)
    {
        return _services.Any(service => service.ServiceType == serviceType);
    }

    private static ServiceLifetime ParseLifetime(InstanceLifetime lifetime)
    {
        return lifetime switch
        {
            InstanceLifetime.Singleton => ServiceLifetime.Singleton,
            InstanceLifetime.Scoped => ServiceLifetime.Scoped,
            InstanceLifetime.Transient => ServiceLifetime.Transient,
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
        };
    }
}
