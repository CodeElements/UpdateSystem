#if NETSTANDARD
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace CodeElements.UpdateSystem.Utilities
{
	public class IncrementalHashStream : Stream
	{
		private readonly Stream _stream;

		public IncrementalHashStream(Stream stream, IncrementalHash incrementalHash)
		{
			IncrementalHash = incrementalHash;
			_stream = stream;
		}

		public IncrementalHash IncrementalHash { get; }

		public override bool CanRead { get; } = false;
		public override bool CanSeek { get; } = false;
		public override bool CanWrite { get; } = true;
		public override long Length => _stream.Length;

		public override long Position
		{
			get => _stream.Position;
			set => _stream.Position = value;
		}

		public override void Flush()
		{
			_stream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			_stream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			IncrementalHash.AppendData(buffer, offset, count);
			_stream.Write(buffer, offset, count);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			IncrementalHash.AppendData(buffer, offset, count);
			return _stream.WriteAsync(buffer, offset, count, cancellationToken);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			IncrementalHash.Dispose();
		}
	}
}
#endif