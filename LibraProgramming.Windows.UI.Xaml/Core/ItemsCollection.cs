using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Foundation.Collections;

namespace LibraProgramming.Windows.UI.Xaml.Core
{
    public class ItemsCollection : IObservableVector<object>
    {
        private int lockCount;
        private readonly IList<object> items;

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return items.IsReadOnly;
            }
        }

        public event VectorChangedEventHandler<object> VectorChanged;

        public object this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
                DoVectorChangedEvent(new ItemsCollectionChangedEventArgs(CollectionChange.ItemChanged, (uint)index));
            }
        }

        public ItemsCollection()
        {
            items = new List<object>();
        }

        public IEnumerator<object> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDisposable BeginUpdate()
        {
            lockCount++;
            return new UpdateLocker(this);
        }

        public void Add(object item)
        {
            var index = Count;

            items.Add(item);

            DoVectorChangedEvent(new ItemsCollectionChangedEventArgs(CollectionChange.ItemInserted, (uint)index));
        }

        public void Clear()
        {
            items.Clear();
            DoVectorChangedEvent(new ItemsCollectionChangedEventArgs(CollectionChange.Reset));
        }

        public bool Contains(object item)
        {
            return items.Contains(item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(object item)
        {
            var index = items.IndexOf(item);

            if (0 > index)
            {
                return false;
            }

            items.RemoveAt(index);

            DoVectorChangedEvent(new ItemsCollectionChangedEventArgs(CollectionChange.ItemRemoved, (uint)index));

            return true;
        }

        public int IndexOf(object item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, object item)
        {
            items.Insert(index, item);

            DoVectorChangedEvent(new ItemsCollectionChangedEventArgs(CollectionChange.ItemInserted, (uint)index));
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
            DoVectorChangedEvent(new ItemsCollectionChangedEventArgs(CollectionChange.ItemRemoved, (uint)index));
        }

        private void DoVectorChangedEvent(IVectorChangedEventArgs args)
        {
            if (0 < lockCount)
            {
                return;
            }

            var handler = VectorChanged;

            if (null != handler)
            {
                handler.Invoke(this, args);
            }
        }

        private class UpdateLocker : IDisposable
        {
            private readonly ItemsCollection collection;

            public UpdateLocker(ItemsCollection collection)
            {
                this.collection = collection;
            }

            public void Dispose()
            {
                if (collection.lockCount > 0)
                {
                    if (0 == --collection.lockCount)
                    {
                        collection.DoVectorChangedEvent(new ItemsCollectionChangedEventArgs(CollectionChange.Reset));
                    }
                }
            }
        }

        private class ItemsCollectionChangedEventArgs : IVectorChangedEventArgs
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

            public ItemsCollectionChangedEventArgs(CollectionChange change)
            {
                CollectionChange = change;
            }

            public ItemsCollectionChangedEventArgs(CollectionChange change, uint index)
                : this(change)
            {
                Index = index;
            }
        }
    }
}