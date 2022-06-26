using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Statik.Hosting.Impl
{
    public class AppBaseAppendMessageHandler : HttpMessageHandler
    {
        private readonly HttpClient _innerHttpClient;
        private readonly PathString _appBase;

        public AppBaseAppendMessageHandler(HttpClient innerHttpClient, PathString appBase)
        {
            _innerHttpClient = innerHttpClient;
            _appBase = appBase;
        }
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var copy = new HttpRequestMessage(request.Method, request.RequestUri);
            
            copy.Content = request.Content;
            
            foreach (var v in request.Headers)
            {
                copy.Headers.TryAddWithoutValidation(v.Key, v.Value);
            }

            foreach (var v in request.Options)
            {
                copy.Options.TryAdd(v.Key, v.Value);
            }

            copy.Version = request.Version;

            if (!_appBase.HasValue) return _innerHttpClient.SendAsync(copy, cancellationToken);
            
            var pathString = _appBase;
            pathString = pathString.Add(copy.RequestUri.PathAndQuery);

            copy.RequestUri = new Uri(copy.RequestUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped) + pathString);
                
            
            return _innerHttpClient.SendAsync(copy, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
                
            if (disposing)
            {
                _innerHttpClient.Dispose();
            }
        }
    }
}