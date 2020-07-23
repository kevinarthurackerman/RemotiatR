using Microsoft.Extensions.DependencyInjection.Extensions;
using RemotiatR.Client;
using RemotiatR.Shared;
using RemotiatR.Serializer.Json.Shared;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using MediatR;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace RemotiatR.Serializer.Json.Client
{
    public static class IRemotiatrServiceCollectionExtensions
    {
        private static ConditionalWeakTable<IMessageHostInfoLookup, HashSet<Type>> _allowedMessageTypesCache = 
            new ConditionalWeakTable<IMessageHostInfoLookup, HashSet<Type>>();

        public static IAddRemotiatrOptions AddJsonSerializer(this IAddRemotiatrOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.Services.TryAddScoped<IMessageSerializer>(x =>
            {
                var messageHostInfoLookup = x.GetRequiredService<IMessageHostInfoLookup>();

                var allowedMessageTypes = _allowedMessageTypesCache.GetValue(messageHostInfoLookup, x =>
                {
                    var requestTypes = x
                        .SelectMany(x => x.Value.MessageInfos)
                        .Select(x => x.Key)
                        .ToArray();

                    var responseTypes = requestTypes.Select(x =>
                        x.GetInterfaces().FirstOrDefault(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IRequest<>)))
                        .Where(x => x != null)
                        .Select(x => x.GetGenericArguments().First())
                        .ToArray();

                    return requestTypes.Concat(responseTypes).ToHashSet();
                });

                return new DefaultJsonMessageSerializer(allowedMessageTypes);
            });

            return options;
        }
    }
}
