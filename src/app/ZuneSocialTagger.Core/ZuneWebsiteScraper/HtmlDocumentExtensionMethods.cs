using HtmlAgilityPack;

namespace ZuneSocialTagger.Core.ZuneWebsiteScraper
{
    public static class HtmlDocumentExtensionMethods
    {
        /// <summary>
        /// Gets a HtmlNode from the xpath for the chosen element id i.e. <html id=5><a>something</a></html>
        /// GetNode("5",a); would get the element with id of 5 and walk the xpath
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static HtmlNode GetNode(this HtmlDocument document, string elementId, string xPath)
        {
            HtmlNode node = document.GetElementbyId(elementId);
            return node.SelectSingleNode(xPath);
        }
    }
}