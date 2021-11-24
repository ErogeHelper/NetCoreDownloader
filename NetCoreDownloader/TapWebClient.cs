using System;
using System.Net;
using System.Net.Cache;
using System.Threading;

namespace NetCoreDownloader
{
    internal class TapWebClient
    {
        public string BaseAddress { get; set; }

        public RequestCachePolicy CachePolicy { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public ICredentials Credentials { get; set; }

        public WebHeaderCollection Headers { get; set; }

        public IWebProxy Proxy { get; set; }

        public TapWebClient()
        {
            WebClient webClient = new WebClient();
            this.BaseAddress = webClient.BaseAddress;
            this.Headers = webClient.Headers;
            this.Proxy = webClient.Proxy;
        }

        public void DownloadStringAsync(string uri, CancellationToken cancelToken = default(CancellationToken), IProgress<DownloadProgressChangedEventArgs> progress = null)
        {
            var client = GetWebClient(cancelToken, progress);
            client.DownloadStringAsync(new Uri(uri));
        }

        public void DownloadDataAsync(string uri, CancellationToken cancelToken = default(CancellationToken), IProgress<DownloadProgressChangedEventArgs> progress = null)
        {
            var client = GetWebClient(cancelToken, progress);
            client.DownloadDataAsync(new Uri(uri));
        }

        public void DownloadFileAsync(string uri, string fileName, CancellationToken cancelToken = default(CancellationToken), IProgress<DownloadProgressChangedEventArgs> progress = null)
        {
            var client = GetWebClient(cancelToken, progress);
            client.DownloadFileAsync(new Uri(uri), fileName);
        }

        private WebClient GetWebClient(CancellationToken cancelToken, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            WebClient wc = new WebClient
            {
                BaseAddress = this.BaseAddress,
                CachePolicy = this.CachePolicy,
                UseDefaultCredentials = this.UseDefaultCredentials,
                Credentials = this.Credentials,
                Headers = this.Headers,
                Proxy = this.Proxy
            };
            if (cancelToken != CancellationToken.None)
            {
                cancelToken.Register(delegate ()
                {
                    wc.CancelAsync();
                });
            }
            if (progress != null)
            {
                wc.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs args)
                {
                    progress.Report(args);
                };
            }
            return wc;
        }
    }
}
