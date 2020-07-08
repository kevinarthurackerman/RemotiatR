using RemotiatR.Server;
using RemotiatR.Shared;
using RemotiatR.Serializer.Json.Shared;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using MediatR;

namespace RemotiatR.Serializer.Json.Server
{
    public static class IAddRemotiatrOptionsExtensions
    {
        public static IAddRemotiatrOptions AddJsonSerializer(this IAddRemotiatrOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.Services.TryAddScoped<IMessageSerializer>(x =>
            {
                var messageHostInfoLookup = x.GetRequiredService<IMessageHostInfoLookup>();

                var requestTypes = messageHostInfoLookup
                    .SelectMany(x => x.Value.MessageInfos)
                    .Select(x => x.Key)
                    .ToArray();

                var responseTypes = requestTypes.Select(x => 
                    x.GetInterfaces().FirstOrDefault(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IRequest<>)))
                    .Where(x => x != null)
                    .Select(x => x.GetGenericArguments().First())
                    .ToArray();

                return new DefaultJsonMessageSerializer(requestTypes.Concat(responseTypes).ToArray());
            });

            return options;
        }
    }
}
