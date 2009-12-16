using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASFTag.Net
{
    public class TagContainer : ICollection<Attribute>
    {
        private readonly List<Attribute> _frames;

        public TagContainer()
        {
            _frames = new List<Attribute>();
        }



        #region ICollection

        public IEnumerator<Attribute> GetEnumerator()
        {
            return _frames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Attribute item)
        {
            _frames.Add(item);
        }

        public void AddRange(IEnumerable<Attribute> items)
        {
            _frames.AddRange(items);
        }

        public void Clear()
        {
            _frames.Clear();
        }

        public bool Contains(Attribute item)
        {
            return _frames.Contains(item);
        }

        public void CopyTo(Attribute[] array, int arrayIndex)
        {
            _frames.CopyTo(array,arrayIndex);
        }

        public bool Remove(Attribute item)
        {
            return _frames.Remove(item);
        }

        public int Count
        {
            get { return _frames.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

    }
}
