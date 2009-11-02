using System;
namespace ZuneSocialTagger.GUIV2
{
    public class SearchEventArgs : EventArgs
    {
        public string SearchText { get; set; }

        public SearchEventArgs(string searchText): base()
        {
            this.SearchText = searchText;
        }
    }
}