using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.UI.Xaml.Core
{
    public partial class ObservableCollectionView : DependencyObject, IEnumerable, INotifyCollectionChanged, ICollectionViewFactory, ICollectionUpdatable, IObservableCollectionView
    {
        public static readonly DependencyProperty CanFilterProperty;
        public static readonly DependencyProperty FilterProperty;
        public static readonly DependencyProperty SourceProperty;

        private ICollectionView collectionView;
        private int updateCounter;

        public bool CanFilter
        {
            get
            {
                return (bool) GetValue(CanFilterProperty);
            }
            set
            {
                SetValue(CanFilterProperty, value);
            }
        }

        public ICollectionFilter Filter
        {
            get
            {
                return (ICollectionFilter) GetValue(FilterProperty);
            }
            set
            {
                SetValue(FilterProperty, value);
            }
        }

        public IEnumerable Source
        {
            get
            {
                return (IEnumerable) GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        private IList<object> List
        {
            get;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Предоставляет поведение инициализации базового класса для производных классов DependencyObject.
        /// </summary>
        public ObservableCollectionView()
        {
            List = new List<object>();
        }

        static ObservableCollectionView()
        {
            CanFilterProperty = DependencyProperty
                .Register(
                    "CanFilter",
                    typeof (bool),
                    typeof (ObservableCollectionView),
                    new PropertyMetadata(false, OnCanFilterPropertyChanged)
                );
            FilterProperty = DependencyProperty
                .Register(
                    "Filter",
                    typeof (ICollectionFilter),
                    typeof (ObservableCollectionView),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnFilterPropertyChanged)
                );
            SourceProperty = DependencyProperty
                .Register(
                    "Source",
                    typeof (IEnumerable),
                    typeof (ObservableCollectionView),
                    new PropertyMetadata(DependencyProperty.UnsetValue, OnSourcePropertyChanged)
                );
        }

        /// <summary>
        /// Возвращает перечислитель, который осуществляет итерацию по коллекции.
        /// </summary>
        /// <returns>
        /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора коллекции.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }

        /// <summary>
        /// Создает экземпляр ICollectionView с использованием параметров по умолчанию.
        /// </summary>
        /// <returns>
        /// Представление по умолчанию.
        /// </returns>
        public ICollectionView CreateView()
        {
            return collectionView ?? (collectionView = new CollectionView(this));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void BeginUpdate()
        {
            Interlocked.Increment(ref updateCounter);
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndUpdate()
        {
            if (0 > updateCounter)
            {
                return;
            }

            if (0 == Interlocked.Decrement(ref updateCounter))
            {
                DoCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        private void DoCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnSourceVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            throw new NotImplementedException();
        }

        private void UnhookSourceListeners(IEnumerable value)
        {
            var collection = value as INotifyCollectionChanged;

            if (null != collection)
            {
                collection.CollectionChanged -= OnSourceCollectionChanged;
                return;
            }

            var vector = value as IObservableVector<object>;

            if (null != vector)
            {
                vector.VectorChanged -= OnSourceVectorChanged;
            }
        }

        private void HookSourceListeners(IEnumerable value)
        {
            var collection = value as INotifyCollectionChanged;

            if (null != collection)
            {
                collection.CollectionChanged += OnSourceCollectionChanged;
                return;
            }

            var vector = value as IObservableVector<object>;

            if (null != vector)
            {
                vector.VectorChanged += OnSourceVectorChanged;
            }
        }

        private void OnCanFilterChanged()
        {
            throw new NotImplementedException();
        }

        private void OnSourceChanged(IEnumerable previous, IEnumerable current)
        {
            using (new DeferredUpdater(this))
            {
                List.Clear();

                UnhookSourceListeners(previous);
                HookSourceListeners(current);
            }
        }

        private void OnFilterChanged()
        {
            Reset();
        }

        private static void OnCanFilterPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((ObservableCollectionView) source).OnCanFilterChanged();
        }

        private static void OnFilterPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((ObservableCollectionView) source).OnFilterChanged();
        }

        private static void OnSourcePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            ((ObservableCollectionView) source).OnSourceChanged((IEnumerable)e.OldValue, (IEnumerable) e.NewValue);
        }

        private class DeferredUpdater : IDisposable
        {
            private readonly ObservableCollectionView owner;

            /// <summary>
            /// Инициализирует новый экземпляр класса <see cref="T:System.Object"/>.
            /// </summary>
            public DeferredUpdater(ObservableCollectionView owner)
            {
                this.owner = owner;
                owner.BeginUpdate();
            }

            /// <summary>
            /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
            /// </summary>
            public void Dispose()
            {
                owner.EndUpdate();
            }
        }
    }
}