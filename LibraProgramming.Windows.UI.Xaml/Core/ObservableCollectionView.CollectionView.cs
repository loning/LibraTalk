using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;

namespace LibraProgramming.Windows.UI.Xaml.Core
{
    public partial class ObservableCollectionView
    {
        private class CollectionView : ICollectionView
        {
            private readonly ObservableCollectionView owner;

            /// <summary>
            /// Получает число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <returns>
            /// Число элементов, содержащихся в интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </returns>
            public int Count => owner.List.Count;

            /// <summary>
            /// Получает значение, указывающее, является ли объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступным только для чтения.
            /// </summary>
            /// <returns>
            /// Значение true, если <see cref="T:System.Collections.Generic.ICollection`1"/> доступна только для чтения; в противном случае — значение false.
            /// </returns>
            public bool IsReadOnly => owner.List.IsReadOnly;

            /// <summary>
            /// Возвращает все группы коллекций, связанные с представлением.
            /// </summary>
            /// <returns>
            /// Коллекция векторов возможных представлений.
            /// </returns>
            public IObservableVector<object> CollectionGroups
            {
                get;
            }

            /// <summary>
            /// Получает текущий элемент в представлении.
            /// </summary>
            /// <returns>
            /// Текущий элемент в представлении или значение null, если текущий элемент отсутствует.
            /// </returns>
            public object CurrentItem
            {
                get
                {
                    //                    EnsureCurrentPosition();
                    if (owner.List.Count >= CurrentPosition)
                    {
                        return null;
                    }

                    return owner.List[CurrentPosition];
                }
            }

            /// <summary>
            /// Получает порядковый номер элемента CurrentItem в представлении.
            /// </summary>
            /// <returns>
            /// Порядковый номер элемента CurrentItem в представлении.
            /// </returns>
            public int CurrentPosition
            {
                get;
                private set;
            }

            /// <summary>
            /// Получает значение-метку, которое поддерживает реализации инкрементной загрузки. См. также раздел LoadMoreItemsAsync.
            /// </summary>
            /// <returns>
            /// Значение true, если дополнительные выгруженные элементы остаются в представлении; в противном случае - значение false.
            /// </returns>
            public bool HasMoreItems
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            /// Получает значение, показывающее, находится ли элемент CurrentItem представления за концом коллекции.
            /// </summary>
            /// <returns>
            /// Значение true, если свойство CurrentItem представления находится за пределами конца коллекции; в противном случае - false.
            /// </returns>
            public bool IsCurrentAfterLast
            {
                get;
                private set;
            }

            /// <summary>
            /// Получает значение, указывающее, находится ли элемент CurrentItem представления за началом коллекции.
            /// </summary>
            /// <returns>
            /// Значение true, если свойство CurrentItem представления находится за пределами начала коллекции; в противном случае - значение false.
            /// </returns>
            public bool IsCurrentBeforeFirst
            {
                get;
                private set;
            }

            /// <summary>
            /// Получает или задает элемент с указанным индексом.
            /// </summary>
            /// <returns>
            /// Элемент с заданным индексом.
            /// </returns>
            /// <param name="index">Отсчитываемый с нуля индекс получаемого или задаваемого элемента.</param><exception cref="T:System.ArgumentOutOfRangeException">Параметр <paramref name="index"/> не является допустимым индексом в списке <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">Свойство задано, и объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.</exception>
            public object this[int index]
            {
                get
                {
                    return owner.List[index];
                }
                set
                {
                    owner.List[index] = value;

                }
            }

            public event CurrentChangingEventHandler CurrentChanging;

            public event EventHandler<object> CurrentChanged;

            public event VectorChangedEventHandler<object> VectorChanged;

            public CollectionView(ObservableCollectionView owner)
            {
                this.owner = owner;

                CurrentPosition = owner.List.Count > 0 ? 0 : -1;
            }

            /// <summary>
            /// Возвращает перечислитель, выполняющий перебор элементов коллекции.
            /// </summary>
            /// <returns>
            /// Интерфейс <see cref="T:System.Collections.Generic.IEnumerator`1"/>, который может использоваться для перебора элементов коллекции.
            /// </returns>
            public IEnumerator<object> GetEnumerator()
            {
                return owner.List.GetEnumerator();
            }

            /// <summary>
            /// Возвращает перечислитель, который осуществляет итерацию по коллекции.
            /// </summary>
            /// <returns>
            /// Объект <see cref="T:System.Collections.IEnumerator"/>, который может использоваться для перебора коллекции.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Добавляет элемент в коллекцию <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <param name="item">Объект, добавляемый в коллекцию <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
            public void Add(object item)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Удаляет все элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
            public void Clear()
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Определяет, содержит ли коллекция <see cref="T:System.Collections.Generic.ICollection`1"/> указанное значение.
            /// </summary>
            /// <returns>
            /// Значение true, если объект <paramref name="item"/> найден в <see cref="T:System.Collections.Generic.ICollection`1"/>; в противном случае — значение false.
            /// </returns>
            /// <param name="item">Объект, который требуется найти в <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
            public bool Contains(object item)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Копирует элементы <see cref="T:System.Collections.Generic.ICollection`1"/> в массив <see cref="T:System.Array"/>, начиная с указанного индекса <see cref="T:System.Array"/>.
            /// </summary>
            /// <param name="array">Одномерный массив <see cref="T:System.Array"/>, в который копируются элементы из интерфейса <see cref="T:System.Collections.Generic.ICollection`1"/>. Индексация в массиве <see cref="T:System.Array"/> должна начинаться с нуля.</param><param name="arrayIndex">Индекс (с нуля) в массиве <paramref name="array"/>, с которого начинается копирование.</param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="array"/> имеет значение null.</exception><exception cref="T:System.ArgumentOutOfRangeException">Значение параметра <paramref name="arrayIndex"/> меньше 0.</exception><exception cref="T:System.ArgumentException">Количество элементов в исходной коллекции <see cref="T:System.Collections.Generic.ICollection`1"/> превышает доступное место в целевом массиве <paramref name="array"/>, начиная с индекса <paramref name="arrayIndex"/> до конца массива.</exception>
            public void CopyTo(object[] array, int arrayIndex)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Удаляет первый экземпляр указанного объекта из коллекции <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </summary>
            /// <returns>
            /// Значение true, если объект <paramref name="item"/> успешно удален из <see cref="T:System.Collections.Generic.ICollection`1"/>, в противном случае — значение false. Этот метод также возвращает значение false, если параметр <paramref name="item"/> не найден в исходном интерфейсе <see cref="T:System.Collections.Generic.ICollection`1"/>.
            /// </returns>
            /// <param name="item">Объект, который необходимо удалить из коллекции <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.ICollection`1"/> доступен только для чтения.</exception>
            public bool Remove(object item)
            {
                throw new System.NotImplementedException();
            }
            
            /// <summary>
            /// Определяет индекс заданного элемента коллекции <see cref="T:System.Collections.Generic.IList`1"/>.
            /// </summary>
            /// <returns>
            /// Индекс <paramref name="item"/> если он найден в списке; в противном случае его значение равно -1.
            /// </returns>
            /// <param name="item">Объект, который требуется найти в <see cref="T:System.Collections.Generic.IList`1"/>.</param>
            public int IndexOf(object item)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Вставляет элемент в список <see cref="T:System.Collections.Generic.IList`1"/> по указанному индексу.
            /// </summary>
            /// <param name="index">Индекс (с нуля), по которому вставляется <paramref name="item"/>.</param><param name="item">Объект, вставляемый в <see cref="T:System.Collections.Generic.IList`1"/>.</param><exception cref="T:System.ArgumentOutOfRangeException">Параметр <paramref name="index"/> не является допустимым индексом в списке <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.</exception>
            public void Insert(int index, object item)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Удаляет элемент <see cref="T:System.Collections.Generic.IList`1"/> по указанному индексу.
            /// </summary>
            /// <param name="index">Отсчитываемый от нуля индекс удаляемого элемента.</param><exception cref="T:System.ArgumentOutOfRangeException">Параметр <paramref name="index"/> не является допустимым индексом в списке <see cref="T:System.Collections.Generic.IList`1"/>.</exception><exception cref="T:System.NotSupportedException">Объект <see cref="T:System.Collections.Generic.IList`1"/> доступен только для чтения.</exception>
            public void RemoveAt(int index)
            {
                throw new System.NotImplementedException();
            }
            
            /// <summary>
            /// Задает указанный элемент в качестве CurrentItem в представлении.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            /// <param name="item">Элемент, устанавливаемый в качестве CurrentItem.</param>
            public bool MoveCurrentTo(object item)
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Задает элемент по заданному индексу в качестве CurrentItem в представлении.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            /// <param name="index">Индекс перемещаемого элемента.</param>
            public bool MoveCurrentToPosition(int index)
            {
                if (index < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                if (owner.List.Count >= index)
                {
                    return false;
                }

                if (CanChangeCurrent())
                {
                    CurrentPosition = index;
                    DoCurrentChanged(CurrentItem);

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Задает первый элемент в представлении в качестве CurrentItem.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            public bool MoveCurrentToFirst()
            {
                return MoveCurrentToPosition(0);
            }

            /// <summary>
            /// Задает последний элемент в представлении в качестве CurrentItem.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            public bool MoveCurrentToLast()
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Задает элемент после CurrentItem в представлении в качестве элемента CurrentItem.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            public bool MoveCurrentToNext()
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Устанавливает элемент перед элементом CurrentItem в качестве CurrentItem.
            /// </summary>
            /// <returns>
            /// Значение true, если полученный элемент CurrentItem принадлежит представлению; в противном случае - значение false.
            /// </returns>
            public bool MoveCurrentToPrevious()
            {
                throw new System.NotImplementedException();
            }

            /// <summary>
            /// Инициализирует инкрементную загрузку из представления.
            /// </summary>
            /// <returns>
            /// Свернутые результаты операции загрузки.
            /// </returns>
            /// <param name="count">Число загружаемых элементов.</param>
            public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
            {
                throw new System.NotImplementedException();
            }

            private void EnsureCurrentPosition()
            {
                if (null == owner.List || owner.List.Count < 1)
                {
                    throw new IndexOutOfRangeException();
                }
            }

            private bool CanChangeCurrent()
            {
                var e = new CurrentChangingEventArgs(true);

                CurrentChanging?.Invoke(this, e);

                return false == e.Cancel;
            }

            private void DoCurrentChanged(object current)
            {
                CurrentChanged?.Invoke(this, current);
            }
        }
    }
}