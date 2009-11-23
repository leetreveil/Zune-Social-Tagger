using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Diagnostics;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class PageDownloader
    {
        public static void DownloadAsync(string url, Action<string> callback)
        {
            ThreadPool.QueueUserWorkItem(state => callback(Download(url)));
        }

        public static string Download(string url)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                WebRequest request = WebRequest.Create(url);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                using (var reader = new StreamReader(stream, Encoding.Default))
                {
                    string data = HttpUtility.HtmlDecode(reader.ReadToEnd());

                    sw.Stop();

                    Console.WriteLine("time taken to download webpage: {0}",sw.ElapsedMilliseconds);

                    return data;
                }
                //TODO: Move the HtmlDecode to somewhere later down the pipeline as we dont really need to be decoding the entire page
            }
            catch (UriFormatException ex)
            {
                throw new PageDownloaderException("invalid url", ex);
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