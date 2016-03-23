using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Collections;

namespace LibraProgramming.Windows.UI.Xaml.Core
{
    /// <summary>
    /// 
    /// </summary>
    class VectorChangedEventArgs : IVectorChangedEventArgs
    {
        public CollectionChange CollectionChange
        {
            get;
            private set;
        }

        public uint Index
        {
            get;
            private set;
        }

        public VectorChangedEventArgs(CollectionChange collectionChange, uint index)
        {
            CollectionChange = collectionChange;
            Index = index;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ObservableVector<T> : IObservableVector<T>
    {
        private readonly IList<T> inner;

        public event VectorChangedEventHandler<T> VectorChanged;

        /// <summary>
        /// 
        /// </summary>
        public int Count => inner.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {
                inner[index] = value;
                RaiseVectorChanged(CollectionChange.ItemChanged, index);
            }
        }

        public ObservableVector()
            : this(Enumerable.Empty<T>())
        {
        }

        public ObservableVector(IEnumerable<T> source)
        {
            inner = new List<T>(source);
        }

        private void RaiseVectorChanged(CollectionChange collectionChange, int index)
        {
            var handler = VectorChanged;
            handler?.Invoke(this, new VectorChangedEventArgs(collectionChange, (uint) index));
        }

        public int IndexOf(T item)
        {
            return inner.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            inner.Insert(index, item);
            RaiseVectorChanged(CollectionChange.ItemInserted, index);
        }

        public void RemoveAt(int index)
        {
            inner.RemoveAt(index);
            RaiseVectorChanged(CollectionChange.ItemRemoved, index);
        }

        public void Add(T item)
        {
            inner.Add(item);
            RaiseVectorChanged(CollectionChange.ItemInserted, inner.Count - 1);
        }

        public void Clear()
        {
            inner.Clear();
            RaiseVectorChanged(CollectionChange.Reset, 0);
        }

        public bool Contains(T item)
        {
            return inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var index = inner.IndexOf(item);

            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }
    }
}