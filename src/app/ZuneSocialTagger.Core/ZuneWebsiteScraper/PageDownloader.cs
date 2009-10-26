using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

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
                    throw new PageDownloaderException("redirected to another webpage");


                Stream stream = response.GetResponseStream();

                //TODO: Move the HtmlDecode to somewhere later down the pipeline as we dont really need to be decoding the entire page
                using (var reader = new StreamReader(stream, Encoding.Default))
                    return HttpUtility.HtmlDecode(reader.ReadToEnd());
            }
            catch(UriFormatException ex)
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