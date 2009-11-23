using System;
using HtmlAgilityPack;

namespace ZuneSocialTagger.Core.ZuneWebsite
{
    public static class HtmlDocumentExtensionMethods
    {
        /// <summary>
        /// Gets a HtmlNode from the xpath for the chosen element id i.e. <html id=5><a>something</a></html>
        /// GetNodeByIdAndXpath("5",a/ul/li); would get the element with id of 5 and walk the xpath to the li element
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public static HtmlNode GetNodeByIdAndXpath(this HtmlDocument document, string elementId, string xPath)
        {
            try
            {
                HtmlNode node = document.GetElementbyId(elementId);
                return node.SelectSingleNode(xPath);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}