namespace SimpleEncryptor.Core
{
    internal class BoundedStream : Stream
    {
        private readonly Stream _inner;
        private readonly long _limit;
        private long _read;

        public BoundedStream(Stream inner, long limit) { _inner = inner; _limit = limit; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long remaining = _limit - _read;
            if (remaining <= 0) return 0;
            int bytesRead = _inner.Read(buffer, offset, (int)Math.Min(count, remaining));
            _read += bytesRead;
            return bytesRead;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _limit;
        public override long Position { get => _read; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
