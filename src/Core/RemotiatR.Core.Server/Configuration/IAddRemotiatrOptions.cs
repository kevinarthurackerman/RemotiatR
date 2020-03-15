﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace RemotiatR.Server
{
    public interface IAddRemotiatrOptions
    {
        IServiceCollection Services { get; }

        IAddRemotiatrOptions SetMessageKeyGenerator(Func<Type, string> keyGenerator);

        IAddRemotiatrOptions AddAssemblies(params Assembly[] assembliesToScan);

        IAddRemotiatrOptions WithMediatorImplementationType(Type implementationType);

        IAddRemotiatrOptions WithMediatorLifetime(ServiceLifetime serviceLifetime);
    }
}
