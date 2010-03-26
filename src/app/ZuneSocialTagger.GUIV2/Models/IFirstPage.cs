using System;
namespace ZuneSocialTagger.GUIV2.Models
{
    /// <summary>
    /// Blank interface for identifying the first 'view' that the application can load
    /// </summary>
    public interface IFirstPage
    {
        event Action FinishedLoading;
        void ViewHasFinishedLoading();
    }
}