using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using LibraProgramming.Windows.UI.Xaml.Primitives.Commands;

namespace LibraProgramming.Windows.UI.Xaml.Primitives.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public class DialogCommandCollection : ICollection<IDialogCommand>, ICollection, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly DialogCommandDispatcher dispatcher;
        private int lockCount;
        private readonly IList<IDialogCommand> items;

        public int Count => items.Count;

        public bool IsSynchronized => ((ICollection) items).IsSynchronized;

        public object SyncRoot => ((ICollection) items).SyncRoot;

        public bool IsReadOnly => items.IsReadOnly;

        public IDialogCommand this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                var existing = items[index];

                items[index] = value;

                DoCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, existing));
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public DialogCommandCollection(DialogCommandDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            items = new List<IDialogCommand>();
        }

        public IEnumerator<IDialogCommand> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection) items).CopyTo(array, index);
        }

        public IDisposable BeginUpdate()
        {
            return new UpdateLocker(this);
        }

        public void Add(IDialogCommand item)
        {
            var index = Count;

            items.Add(item);

            item.Dispatcher = dispatcher;

            DoCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            DoCountChanged();
        }

        public void Clear()
        {
            items.Clear();
            DoCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            DoCountChanged();
        }

        public bool Contains(IDialogCommand item)
        {
            return items.Contains(item);
        }

        public void CopyTo(IDialogCommand[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(IDialogCommand item)
        {
            var index = items.IndexOf(item);

            if (0 > index)
            {
                return false;
            }

            items.RemoveAt(index);

            item.Dispatcher = null;

            DoCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            DoCountChanged();

            return true;
        }

        public int IndexOf(IDialogCommand item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, IDialogCommand item)
        {
            items.Insert(index, item);

            item.Dispatcher = dispatcher;

            DoCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            DoCountChanged();
        }

        public void RemoveAt(int index)
        {
            var item = items[index];

            items.RemoveAt(index);

            item.Dispatcher = null;

            DoCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, index));
            DoCountChanged();
        }

        private void DoCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (0 < lockCount)
            {
                return;
            }

            CollectionChanged?.Invoke(this, args);
        }

        private void DoCountChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        }

        private class UpdateLocker : IDisposable
        {
            private readonly DialogCommandCollection collection;

            public UpdateLocker(DialogCommandCollection collection)
            {
                this.collection = collection;
                collection.lockCount++;
            }

            public void Dispose()
            {
                if (collection.lockCount > 0)
                {
                    if (0 == --collection.lockCount)
                    {
                        collection.DoCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace));
                    }
                }
            }
        }
    }
}