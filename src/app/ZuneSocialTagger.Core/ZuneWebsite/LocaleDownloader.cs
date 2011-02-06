using System;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public class LocaleDownloader
    {
        public static void IsMarketPlaceEnabledForLocaleAsync(string locale, Action<bool> callback)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(
                String.Format("http://tuners.zune.net/{0}/ZunePCClient/v4.7/configuration.xml", locale));

            httpWebRequest.BeginGetResponse(ReqCallback, new AsyncResult<bool>(httpWebRequest, callback));
        }

        private static void ReqCallback(IAsyncResult asyncResult)
        {
            AsyncResult<bool> result = asyncResult.AsyncState as AsyncResult<bool>;
            HttpWebRequest httpWebRequest = result.HttpWebRequest;

            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.EndGetResponse(asyncResult))
            {
                XDocument document = XDocument.Load(XmlReader.Create(httpWebResponse.GetResponseStream()));

                var isMarketPlaceEnabled = document
                    .Descendants().Where(x => x.Name.LocalName == "featureEnablement")
                    .Descendants().Where(x => x.Name.LocalName == "marketplace")
                    .Descendants().Where(x => x.Name.LocalName == "status")
                    .First().Value;

                if (isMarketPlaceEnabled == "enabled")
                    result.Callback(true);

                if (isMarketPlaceEnabled == "disabled")
                    result.Callback(false);
            }
        }

        public class AsyncResult<T>
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
