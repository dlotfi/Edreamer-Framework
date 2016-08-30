// Based on this thread in StackOverflow forum: http://stackoverflow.com/questions/4464983/how-do-i-implement-an-http-response-filter-to-operate-on-the-entire-content-at-o
// Processing chunk of response produces some unwanted bytes in the response which might be rendered as unknown characters on the client

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.WebPages;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Mvc.DeferredRender
{
    public class DeferredRendersReplacementStream : MemoryStream
    {
        private readonly Stream _outputStream;
        private readonly IDictionary<string, Func<HelperResult>> _deferredRenders;
        private MemoryStream _cachedStream;
        private bool _isClosing;
        private bool _isClosed;
        
        public DeferredRendersReplacementStream(Stream outputStream, IDictionary<string, Func<HelperResult>> deferredRenders)
        {
            Throw.IfArgumentNull(outputStream, "outputStream");
            Throw.IfArgumentNullOrEmpty(deferredRenders, "deferredRenders");
            _outputStream = outputStream;
            _deferredRenders = deferredRenders;
            _cachedStream = new MemoryStream(1024);
        }

        public override void Flush()
        {
            if (_isClosing && !_isClosed)
            {
                var encoding = HttpContext.Current.Response.ContentEncoding;
                var cachedContent = encoding.GetString(_cachedStream.ToArray());
                cachedContent = DeferredRenderHelper.ProcessDefferedRenders(cachedContent, _deferredRenders);
                var buffer = encoding.GetBytes(cachedContent);
                _outputStream.Write(buffer, 0, buffer.Length);
                _cachedStream.SetLength(0);
                _outputStream.Flush();
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _cachedStream.Write(buffer, 0, count);
        }

        public override void Close()
        {
            _isClosing = true;
            Flush();
            _isClosed = true;
            _isClosing = false;
            _outputStream.Close();
        }
    }
}