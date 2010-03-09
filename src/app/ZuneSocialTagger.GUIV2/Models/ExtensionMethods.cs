using System.Collections.Generic;
using Caliburn.PresentationFramework;

namespace ZuneSocialTagger.GUIV2.Models
{
    public static class ExtensionMethods
    {
        public static BindableCollection<T> ToBindableCollection<T>(this IEnumerable<T> enumerable )
        {
            var collection = new BindableCollection<T>();

            foreach (var @object in enumerable)
                collection.Add(@object);

            return collection;
        }
    }
}