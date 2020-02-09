using MediatR;
using System;
using System.Linq;

namespace RemotiatR.Shared.Internal
{
    internal static class TypeExtensions
    {
        internal static bool IsNotificationType(this Type type) =>
            type.IsClass
            && type.IsVisible
            && type.GetConstructors().Any(x => !x.IsStatic && x.IsPublic)
            && type.GetInterfaces().Any(x => x == typeof(INotification));

        internal static bool IsRequestType(this Type type) =>
            type.IsClass
            && type.IsVisible
            && type.GetConstructors().Any(x => !x.IsStatic && x.IsPublic)
            && type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));

        internal static Type GetResponseType(this Type type)
        {
            var requestTypeInterface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>));
            
            if (requestTypeInterface == null) throw new InvalidOperationException($"Cannot get response type for non-request type {type.FullName}");

            return requestTypeInterface.GetGenericArguments().First();
        }
    }
}
