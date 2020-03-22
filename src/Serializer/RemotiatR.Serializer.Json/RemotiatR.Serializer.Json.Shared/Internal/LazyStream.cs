using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RemotiatR.Serializer.Json.Shared
{
    internal class LazyStream : Stream
    {
        private readonly Lazy<Stream> _stream;

        internal LazyStream(Func<Stream> streamFactory) => _stream = new Lazy<Stream>(streamFactory);

        public override long Position 
        {
            get => _stream.Value.Position;
            set => _stream.Value.Position = value;
        }

        public override long Length => _stream.Value.Length;

        public override bool CanWrite => _stream.Value.CanWrite;

        public override bool CanTimeout => _stream.Value.CanTimeout;

        public override bool CanSeek => _stream.Value.CanSeek;

        public override bool CanRead => _stream.Value.CanRead;

        public override int ReadTimeout 
        {
            get => _stream.Value.ReadTimeout;
            set => _stream.Value.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            get => _stream.Value.WriteTimeout;
            set => _stream.Value.WriteTimeout = value;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object? state) =>
            _stream.Value.BeginRead(buffer, offset, count, callback, state);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object? state) =>
            _stream.Value.BeginWrite(buffer, offset, count, callback, state);

        public override void Close() => _stream.Value.Close();

        public override void CopyTo(Stream destination, int bufferSize) => _stream.Value.CopyTo(destination, bufferSize);

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
            _stream.Value.CopyToAsync(destination, bufferSize, cancellationToken);

        public override ValueTask DisposeAsync() => _stream.Value.DisposeAsync();

        public override int EndRead(IAsyncResult asyncResult) => _stream.Value.EndRead(asyncResult);

        public override void EndWrite(IAsyncResult asyncResult) => _stream.Value.EndWrite(asyncResult);

        public override void Flush() => _stream.Value.Flush();

        public override Task FlushAsync(CancellationToken cancellationToken) => _stream.Value.FlushAsync(cancellationToken);

        public override int Read(byte[] buffer, int offset, int count) => _stream.Value.Read(buffer, offset, count);

        public override int Read(Span<byte> buffer) => _stream.Value.Read(buffer);

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            _stream.Value.ReadAsync(buffer, cancellationToken);

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            _stream.Value.ReadAsync(buffer, offset, count, cancellationToken);

        public override int ReadByte() =>
            _stream.Value.ReadByte();

        public override long Seek(long offset, SeekOrigin origin) =>
            _stream.Value.Seek(offset, origin);

        public override void SetLength(long value) =>
            _stream.Value.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) =>
            _stream.Value.Write(buffer, offset, count);

        public override void Write(ReadOnlySpan<byte> buffer) =>
            _stream.Value.Write(buffer);

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            _stream.Value.WriteAsync(buffer, offset, count, cancellationToken);

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
            _stream.Value.WriteAsync(buffer, cancellationToken);

        public override void WriteByte(byte value) => _stream.Value.WriteByte(value);

        protected override void Dispose(bool disposing) => _stream.Value.Dispose();
    }
}
