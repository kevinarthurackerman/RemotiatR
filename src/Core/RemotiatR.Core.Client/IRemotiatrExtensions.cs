using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Client
{
    public static class IRemotiatrExtensions
    {
        public static Task Publish<TMarker>(this IRemotiatr<TMarker> remotiatr, object notification) =>
            remotiatr.Publish(notification, default, default);

        public static Task Publish<TMarker>(this IRemotiatr<TMarker> remotiatr, object notification, Action<IServiceProvider> configure) =>
            remotiatr.Publish(notification, configure, default);

        public static Task Publish<TMarker>(this IRemotiatr<TMarker> remotiatr, object notification, CancellationToken cancellationToken) =>
            remotiatr.Publish(notification, default, cancellationToken);

        public static Task Publish<TMarker, TNotification>(this IRemotiatr<TMarker> remotiatr, TNotification notification) where TNotification : INotification =>
            remotiatr.Publish(notification, default, default);

        public static Task Publish<TMarker, TNotification>(this IRemotiatr<TMarker> remotiatr, TNotification notification, Action<IServiceProvider> configure) where TNotification : INotification =>
            remotiatr.Publish(notification, configure, default);

        public static Task Publish<TMarker, TNotification>(this IRemotiatr<TMarker> remotiatr, TNotification notification, CancellationToken cancellationToken) where TNotification : INotification =>
            remotiatr.Publish(notification, default, cancellationToken);

        public static Task Publish<TMarker, TNotification>(this IRemotiatr<TMarker> remotiatr, TNotification notification, Action<IServiceProvider> configure, CancellationToken cancellationToken) where TNotification : INotification =>
            remotiatr.Publish(notification, configure, cancellationToken);

        public static Task<object> Send<TMarker>(this IRemotiatr<TMarker> remotiatr, object request) =>
            remotiatr.Send(request, default, default);

        public static Task<object> Send<TMarker>(this IRemotiatr<TMarker> remotiatr, object request, Action<IServiceProvider> configure) =>
            remotiatr.Send(request, configure, default);

        public static Task<object> Send<TMarker>(this IRemotiatr<TMarker> remotiatr, object request, CancellationToken cancellationToken) =>
            remotiatr.Send(request, default, cancellationToken);

        public static async Task<TResponse> Send<TMarker, TResponse>(this IRemotiatr<TMarker> remotiatr, IRequest<TResponse> request) =>
            (TResponse) await remotiatr.Send(request, default, default);

        public static async Task<TResponse> Send<TMarker, TResponse>(this IRemotiatr<TMarker> remotiatr, IRequest<TResponse> request, Action<IServiceProvider> configure) =>
            (TResponse) await remotiatr.Send(request, configure, default);

        public static async Task<TResponse> Send<TMarker, TResponse>(this IRemotiatr<TMarker> remotiatr, IRequest<TResponse> request, CancellationToken cancellationToken) =>
            (TResponse) await remotiatr.Send(request, default, cancellationToken);

        public static async Task<TResponse> Send<TMarker, TResponse>(this IRemotiatr<TMarker> remotiatr, IRequest<TResponse> request, Action<IServiceProvider> configure, CancellationToken cancellationToken) =>
            (TResponse)await remotiatr.Send(request, configure, cancellationToken);
    }
}
