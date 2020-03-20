using Newtonsoft.Json;
using RemotiatR.Shared;
using System;
using System.IO;
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

        public Task<object> Deserialize(Stream stream)
        {
            try
            {
                if (stream == null) throw new ArgumentNullException(nameof(stream));

                RestartStream(stream);

                using var streamReader = new StreamReader(stream, Encoding.Default);
                using var jsonTextReader = new JsonTextReader(streamReader);

                var payload = (MessagePayload)_jsonSerializer.Deserialize(jsonTextReader, typeof(MessagePayload))!;

                return Task.FromResult(payload.Data!);
            }
            catch(Exception exception)
            {
                throw new Exception("Deserializing failed. See inner exception for details.", exception);
            }
        }

        public Task<Stream> Serialize(object value)
        {
            try
            {
                var stream = new MemoryStream();
                using var streamWriter = new StreamWriter(stream, Encoding.Default, 1024, true);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);

                jsonTextWriter.CloseOutput = false;
                _jsonSerializer.Serialize(jsonTextWriter, new MessagePayload { Data = value }, typeof(MessagePayload));
                jsonTextWriter.Flush();

                RestartStream(stream);

                return Task.FromResult((Stream)stream);
            }
            catch (Exception exception)
            {
                throw new Exception("Derializing failed. See inner exception for details.", exception);
            }
        }

        private static void RestartStream(Stream stream)
        {
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
        }

        private class MessagePayload
        {
            public object? Data { get; set; }

            public MessagePayload() { }
        }
    }
}
