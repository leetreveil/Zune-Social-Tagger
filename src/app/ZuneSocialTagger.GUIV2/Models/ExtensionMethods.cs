using System.Collections.Generic;

namespace ZuneSocialTagger.GUIV2.Models
{
    public static class ExtensionMethods
    {
        public static AsyncObservableCollection<T> ToAsyncObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            var collection = new AsyncObservableCollection<T>();

            foreach (var @object in enumerable)
                collection.Add(@object);

            return collection;
        }
    }
}