using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    public class PageDownloader
    {
        public static void DownloadAsync(string url, Action<string> callback)
        {
            ThreadPool.QueueUserWorkItem(state => callback(Download(url)));
        }

        public static string Download(string url)
        {
            WebRequest request = WebRequest.Create(url);

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();

            using (var reader = new StreamReader(stream, Encoding.UTF8))
                return reader.ReadToEnd();
        }
    }
}