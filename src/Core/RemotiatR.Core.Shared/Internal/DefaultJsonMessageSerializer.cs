using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RemotiatR.Shared.Internal
{
    internal class DefaultJsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializer _jsonSerializer;
        private readonly IKeyMessageTypeMappings _keyMessageTypeMappings;

        public DefaultJsonMessageSerializer(IKeyMessageTypeMappings keyMessageTypeMappings)
        {
            _keyMessageTypeMappings = keyMessageTypeMappings ?? throw new ArgumentNullException(nameof(keyMessageTypeMappings));

            _jsonSerializer = new JsonSerializer();
            _jsonSerializer.Converters.Add(new MessagePayloadJsonConverter(keyMessageTypeMappings));
        }

        public Task<object> Deserialize(Stream stream)
        {
            try
            {
                if (stream == null) throw new ArgumentNullException(nameof(stream));

                RestartStream(stream);

                using var streamReader = new StreamReader(stream, Encoding.Default);
                using var jsonTextReader = new JsonTextReader(streamReader);

                var data = _jsonSerializer.Deserialize(jsonTextReader, typeof(MessagePayload))!;

                return Task.FromResult(data);
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
                if (!_keyMessageTypeMappings.MessageTypeToKeyLookup.TryGetValue(value.GetType(), out var key))
                    throw new InvalidOperationException($"Unable to locate key for message type {value.GetType().FullName} - cannot serialize object.");

                var stream = new MemoryStream();
                using var streamWriter = new StreamWriter(stream, Encoding.Default, 1024, true);
                using var jsonTextWriter = new JsonTextWriter(streamWriter);

                jsonTextWriter.CloseOutput = false;
                _jsonSerializer.Serialize(jsonTextWriter, new MessagePayload(key, value), typeof(MessagePayload));
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
            public string Key { get; set; }
            public object Data { get; set; }

            internal MessagePayload(string key, object? data)
            {
                Key = key ?? throw new ArgumentNullException(nameof(key));
                Data = data ?? throw new ArgumentNullException(nameof(data));
            }
        }

        private class MessagePayloadJsonConverter : JsonConverter
        {
            private readonly IKeyMessageTypeMappings _keyMessageTypeMappings;

            internal MessagePayloadJsonConverter(IKeyMessageTypeMappings keyMessageTypeMappings) =>
                _keyMessageTypeMappings = keyMessageTypeMappings;

            public override bool CanConvert(Type objectType) => objectType == typeof(MessagePayload);

            public override bool CanRead => true;

            public override bool CanWrite => false;

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null) return null;

                var jObj = JObject.Load(reader);

                var key = jObj.GetValue(nameof(MessagePayload.Key), StringComparison.OrdinalIgnoreCase)?.ToString();

                if (key == null) throw new InvalidOperationException("Key was not set - cannot deserialize object.");

                if (!_keyMessageTypeMappings.KeyToMessageTypeLookup.TryGetValue(key, out var type))
                    throw new InvalidOperationException($"Unable to locate message type match for key {key}");

                var data = jObj.GetValue(nameof(MessagePayload.Data), StringComparison.OrdinalIgnoreCase);

                if (data == null) throw new InvalidOperationException("Data was not set - cannot deserialize object.");

                var resultData = serializer.Deserialize(data.CreateReader(), type);

                return resultData;
            }

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
        }
    }
}
