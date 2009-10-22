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
            try
            {
                WebRequest request = WebRequest.Create(url);

                WebResponse response = request.GetResponse();

                //we have to do contains here because to different url's can be the same
                //for example www.google.com is the same as www.google.com/
                if (response.ResponseUri.ToString() != url && !response.ResponseUri.ToString().Contains(url))
                    throw new PageDownloaderException("could not retrieve the webpage");


                Stream stream = response.GetResponseStream();

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return reader.ReadToEnd();
            }
            catch (NotSupportedException ex)
            {
                throw new PageDownloaderException("invalid url", ex);
            }
            catch (WebException ex)
            {
                throw new PageDownloaderException("could retrieve the webpage", ex);
            }
        }
    }
}