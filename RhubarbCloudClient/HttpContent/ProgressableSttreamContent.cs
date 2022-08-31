using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RhubarbCloudClient
{
    public class ProgressableSttreamContent : HttpContent
    {
        private const int DEFAULT_BUFFER_SIZE = 4096;
        private Stream content;
        private int bufferSize;
        private bool contentConsumed;
        public ProgressTracker progressTracker { get; private set; }

        public ProgressableSttreamContent(Stream content, ProgressTracker progressTracke = null) : this(content, DEFAULT_BUFFER_SIZE, progressTracke) { }

        public ProgressableSttreamContent(Stream content, int buffersize, ProgressTracker progressTracke = null)
        {
            if (content is null)
            {
                throw new ArgumentException(nameof(content));
            }
            if (buffersize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(buffersize));
            }

            this.content = content;
            bufferSize = buffersize;
            progressTracker = progressTracke??new ProgressTracker();
        }

        private void PrepareContent()
        {
            if (contentConsumed)
            {
                if (content.CanSeek)
                {
                    content.Position = 0;
                }
                else
                {
                    throw new InvalidOperationException("net_http_content_stream_already_read");
                }
                contentConsumed = true;
            }
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Contract.Assert(stream != null);
            PrepareContent();
            return Task.Run(() =>
            {
                var buffer = new byte[bufferSize];
                var size = content.Length;
                var uploaded = 0;
                progressTracker.ChangeState(ProgressState.PendingUpload);
                using (content) while (true)
                    {
                        var length = content.Read(buffer, 0, buffer.Length);
                        if (length <= 0) break;
                        progressTracker.Bytes = uploaded += length;
                        stream.Write(buffer, 0, length);
                        progressTracker.ChangeState(ProgressState.Uploading);
                    }
                progressTracker.ChangeState(ProgressState.PendingResponse);
            });
        }

        protected override bool TryComputeLength(out long length)
        {
            length = content.Length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
