using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RemotiatR.Shared
{
    public interface IMessageSerializer
    {
        Task<MessageSerializerResult> Deserialize(Stream stream);

        Task<Stream> Serialize(object? message, IDictionary<string, string> messageMetadata);
    }

    public class MessageSerializerResult
    {
        public object? Message { get; }

        public IDictionary<string, string> MessageMetadata { get; }

        public MessageSerializerResult(object? message, IDictionary<string, string> messageMetadata)
        {
            Message = message;
            MessageMetadata = messageMetadata;
        }
    }
}
