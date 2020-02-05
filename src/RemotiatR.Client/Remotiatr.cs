using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace RemotiatR.Client
{
    public interface IRemotiatr : IMediator
    {
    }

    public class Remotiatr : IRemotiatr
    {
        private readonly IMessageSender _messageSender;
        private readonly UrlBuilder _urlBuilder;

        internal Remotiatr(IMessageSender messageSender, UrlBuilder urlBuilder)
        {
            _messageSender = messageSender;
            _urlBuilder = urlBuilder;
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default) => 
            SendRequest(notification, notification.GetType(), typeof(Unit), cancellationToken);

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification =>
            SendRequest(notification, notification.GetType(), typeof(Unit), cancellationToken);

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) =>
            SendRequest(request, request.GetType(), typeof(TResponse), cancellationToken).ContinueWith(x => (TResponse)x.Result);

        public Task<object> Send(object request, CancellationToken cancellationToken = default) =>
            SendRequest(request, request.GetType(), request.GetType().GenericTypeArguments.FirstOrDefault() ?? typeof(Unit), cancellationToken);

        private async Task<object> SendRequest(object request, Type requestType, Type responseType, CancellationToken cancellationToken)
        {
            var url = _urlBuilder(request.GetType());

            var response = await _messageSender.SendRequest(url, request, requestType, responseType, cancellationToken);

            return response;
        }
    }
}
