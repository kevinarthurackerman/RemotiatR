using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public DefaultJsonMessageSerializer(IEnumerable<Type> allowedMessageTypes)
        {
            _jsonSerializer = new JsonSerializer
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Ignore
            };

            _jsonSerializer.Converters.Add(new MessageConverter(allowedMessageTypes));
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

        private class MessageConverter : JsonConverter<Message>
        {
            private readonly HashSet<Type> _allowedMessageTypes;

            public MessageConverter(IEnumerable<Type> allowedMessageTypes)
            {
                _allowedMessageTypes = allowedMessageTypes as HashSet<Type> ?? allowedMessageTypes.ToHashSet();
            }

            public override bool CanRead => true;

            public override bool CanWrite => true;

            public override Message ReadJson(JsonReader reader, Type objectType, Message? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var jObject = serializer.Deserialize<JObject>(reader);
                
                jObject!.TryGetValue(nameof(Message.Data), StringComparison.OrdinalIgnoreCase, out var dataToken);

                if (dataToken == null) throw new InvalidOperationException($"{nameof(Message)} payload missing {nameof(Message.Data)} property");

                var dataObject = dataToken as JObject;

                if (dataObject == null) throw new InvalidOperationException($"{nameof(Message)} {nameof(Message.Data)} property was not an object type");

                dataObject.TryGetValue("$type", out var typeToken);

                if (typeToken == null) throw new InvalidOperationException($"{nameof(Message)} {nameof(Message.Data)} property was missing $type property");

                var typeString = typeToken.Value<string>();

                var type = Type.GetType(typeString, throwOnError: false, ignoreCase: true);

                if (type == null || !_allowedMessageTypes.Contains(type))
                    throw new InvalidOperationException($"{nameof(Message)}.{nameof(Message.Data)} is not an allowed type");

                var thisIndex = serializer.Converters.IndexOf(this);
                serializer.Converters.RemoveAt(thisIndex);

                var result = jObject.ToObject(objectType, serializer) as Message;

                serializer.Converters.Insert(thisIndex, this);

                return result;
            }

            public override void WriteJson(JsonWriter writer, Message? value, JsonSerializer serializer)
            {
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }

                if (value!.Data != null && !_allowedMessageTypes.Contains(value.Data.GetType()))
                    throw new InvalidOperationException($"{nameof(Message)}.{nameof(Message.Data)} is not an allowed type");

                var thisIndex = serializer.Converters.IndexOf(this);
                serializer.Converters.RemoveAt(thisIndex);

                serializer.Serialize(writer, value);
                
                serializer.Converters.Insert(thisIndex, this);
            }
        }
    }
}
