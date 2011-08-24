using System;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public enum MarketplaceStatus
    {
        Available,
        NotAvailable,
        Error
    }

    public class MarketplaceDetails
    {
        public MarketplaceStatus MarketplaceStatus { get; set; }
        public string MarketplaceLocale { get; set; }
    }

    public class LocaleDownloader
    {
        public static void IsMarketPlaceEnabledForLocaleAsync(string locale, Action<MarketplaceDetails> callback)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(
                String.Format("http://tuners.zune.net/{0}/ZunePCClient/v4.7/configuration.xml", locale));

            httpWebRequest.BeginGetResponse(ReqCallback, new AsyncResult<MarketplaceDetails>(httpWebRequest, callback));
        }

        private static void ReqCallback(IAsyncResult asyncResult)
        {
            var result = asyncResult.AsyncState as AsyncResult<MarketplaceDetails>;
            try
            {
                HttpWebRequest httpWebRequest = result.HttpWebRequest;

                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.EndGetResponse(asyncResult))
                {
                    XDocument document = XDocument.Load(XmlReader.Create(httpWebResponse.GetResponseStream()));

                    var features = document
                        .Descendants()
                        .Where(x => x.Name.LocalName == "featureEnablement");

                    var music = features.Descendants()
                        .Where(x => x.Name.LocalName == "music");

                    var isMarketPlaceEnabled = music
                        .Descendants().Where(x => x.Name.LocalName == "status")
                        .First().Value;

                    var details = new MarketplaceDetails();

                    if (isMarketPlaceEnabled == "enabled")
                        details.MarketplaceStatus = MarketplaceStatus.Available;

                    if (isMarketPlaceEnabled == "disabled")
                        details.MarketplaceStatus = MarketplaceStatus.NotAvailable;

                    var locale = features
                        .Descendants().Where(x => x.Name.LocalName == "marketplace")
                        .Descendants().Where(x => x.Name.LocalName == "culture")
                        .First().Value;

                    details.MarketplaceLocale = locale;

                    result.Callback(details);
                }
            }
            catch (Exception)
            {
                result.Callback(null);
            }
        }

        internal class AsyncResult<T>
        {
            public AsyncResult(HttpWebRequest httpWebRequest, Action<T> callback)
            {
                HttpWebRequest = httpWebRequest;
                Callback = callback;
            }

            public HttpWebRequest HttpWebRequest { get; set; }
            public Action<T> Callback { get; set; }
        }
    }

}
