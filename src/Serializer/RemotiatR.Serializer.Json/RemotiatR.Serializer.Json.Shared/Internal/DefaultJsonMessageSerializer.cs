using Newtonsoft.Json;
using RemotiatR.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemotiatR.Serializer.Json.Shared
{
    internal class DefaultJsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializer _jsonSerializer;

        public DefaultJsonMessageSerializer()
        {
            _jsonSerializer = new JsonSerializer
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public Task<MessageSerializerResult> Deserialize(Stream stream)
        {
            try
            {
                if (stream == null) throw new ArgumentNullException(nameof(stream));

                RestartStream(stream);

                using var streamReader = new StreamReader(stream, Encoding.Default);
                using var jsonTextReader = new JsonTextReader(streamReader);

                var result = _jsonSerializer.Deserialize(jsonTextReader, typeof(Message));

                if (!(result is Message messageSerializerResult))
                    throw new InvalidOperationException($"Message payload was not of expected type {typeof(Message).FullName}");

                return Task.FromResult(new MessageSerializerResult(
                    messageSerializerResult.Data, 
                    messageSerializerResult.Meta ?? new Dictionary<string, string>()
                ));
            }
            catch(Exception exception)
            {
                throw new Exception("Deserializing failed. See inner exception for details.", exception);
            }
        }

        public Task<Stream> Serialize(object? message, IDictionary<string,string> messageMetadata)
        {
            return Task.FromResult((Stream)new LazyStream(() =>
            {
                try
                {
                    var payload = new Message
                    {
                        Data = message,
                        Meta = messageMetadata.Any()
                            ? messageMetadata is Dictionary<string, string> metaDict
                                ? metaDict
                                : messageMetadata.ToDictionary(x => x.Key, x => x.Value)
                            : null
                    };

                    var stream = new MemoryStream();
                    using var streamWriter = new StreamWriter(stream, Encoding.Default, 1024, true);
                    using var jsonTextWriter = new JsonTextWriter(streamWriter);

                    jsonTextWriter.CloseOutput = false;
                    _jsonSerializer.Serialize(jsonTextWriter, payload, typeof(Message));
                    jsonTextWriter.Flush();

                    RestartStream(stream);

                    return stream;
                }
                catch (Exception exception)
                {
                    throw new Exception("Serializing failed. See inner exception for details.", exception);
                }
            }));
        }

        private static void RestartStream(Stream stream)
        {
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
        }

        private class Message
        {
            public object? Data { get; set; }
            public Dictionary<string,string>? Meta { get; set; }
        }
    }
}
