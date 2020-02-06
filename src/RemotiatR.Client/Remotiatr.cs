using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.ObjectModel;

namespace RemotiatR.Client
{
    public interface IRemotiatr : IMediator
    {
    }

    public class Remotiatr : IRemotiatr
    {
        private readonly IMessageSender _messageSender;

        private readonly IReadOnlyDictionary<Type, Uri> _uriLookup;

        internal Remotiatr(IMessageSender messageSender, IEnumerable<Assembly> assembliesToScan, Func<Type,Uri> uriBuilder)
        {
            _messageSender = messageSender;

            var typesToMatch = assembliesToScan
                .SelectMany(x => x.GetTypes())
                .Where(x => 
                    x.IsClass 
                    && x.IsVisible
                    && x.GetConstructors().Any(x => !x.IsStatic && x.IsPublic)
                    && x.GetInterfaces().Any(x => x == typeof(IBaseRequest) || x == typeof(INotification))
                )
                .ToArray();

            _uriLookup = new ReadOnlyDictionary<Type,Uri>(typesToMatch.ToDictionary(x => x, x => uriBuilder(x)));
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default) => 
            SendRequest(notification, notification.GetType(), typeof(Unit), cancellationToken);

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification =>
            SendRequest(notification, notification.GetType(), typeof(Unit), cancellationToken);

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) =>
            SendRequest(request, request.GetType(), typeof(TResponse), cancellationToken).ContinueWith(x => (TResponse)x.Result);

        public Task<object> Send(object request, CancellationToken cancellationToken = default) =>
            SendRequest(request, request.GetType(), GetResponseType(request.GetType()) ?? typeof(Unit), cancellationToken);

        private async Task<object> SendRequest(object request, Type requestType, Type responseType, CancellationToken cancellationToken)
        {
            if (!_uriLookup.TryGetValue(requestType, out var uri))
                throw new InvalidOperationException($"Cannot locate a URI for type {requestType.FullName}");

            var response = await _messageSender.SendRequest(uri, request, requestType, responseType, cancellationToken);

            return response;
        }

        private static Type GetResponseType(Type requestType) =>
            requestType.GetInterfaces()
                .First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()
                .First();
    }
}
