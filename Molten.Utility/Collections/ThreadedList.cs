﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Molten.Collections
{
    /// <summary>A thread-safe list implementation. Basically wraps thread-safety around the vanilla list.</summary>
    /// <typeparam name="T">The type of object to be stored in the list.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public partial class ThreadedList<T> : IList<T>, IProducerConsumerCollection<T>, IReadOnlyList<T>
    {
        static readonly T[] _emptyArray = new T[0];

        T[] _items;
        int _count;
        int _capacity;
        int _version;

        object _locker;
        Interlocker _interlocker;

        /// <summary>
        /// Creates a new instance of <see cref="ThreadedList{T}"/>.
        /// </summary>
        public ThreadedList()
        {
            _interlocker = new Interlocker();
            _items = _emptyArray;
            _capacity = 0;
            _locker = new object();
        }

        /// <summary>
        /// Creates a new instance of <see cref="ThreadedList{T}"/>, initialized to a specific capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the list.</param>
        public ThreadedList(int initialCapacity)
        {
            if (initialCapacity < 0)
                throw new ArgumentOutOfRangeException("Cannot have a capacity less than 0.");

            _interlocker = new Interlocker();
            _items = new T[initialCapacity];
            _capacity = _items.Length;
            _locker = new object();
        }

        public ThreadedList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            _interlocker = new Interlocker();
            _items = _emptyArray;
            _capacity = 0;
            _locker = new object();
            AddRange(collection);
        }


        public void Add(T item)
        {
            _interlocker.Lock(() =>
            {
                AddElement(item);
                _version++;
            });
        }

        public void AddRange(IEnumerable<T> collection)
        {
            _interlocker.Lock(() =>
            {
                // Determine if enumerable is a colllection.
                ICollection<T> c = collection as ICollection<T>;
                if (c != null)
                {
                    EnsureCapacityInternal(_count + c.Count);
                    c.CopyTo(_items, _count);
                    _count += c.Count;
                }
                else
                {
                    // Since the collection is not an ICollection, we're forced to enumerate over items, one at a time.
                    using (IEnumerator<T> e = collection.GetEnumerator())
                    {
                        while (e.MoveNext())
                            AddElement(e.Current);
                    }
                }

                // Release lock
                _version++;
            });
        }

        private void AddElement(T item)
        {
            EnsureCapacityInternal(_count + 1);
            _items[_count++] = item;
        }

        /// <summary>Ensures the list has at least the minimum specified capacity.</summary>
        /// <param name="min">The minimum capacity to ensure.</param>
        public void EnsureCapacity(int min)
        {
            _interlocker.Lock(() =>
            {
                EnsureCapacityInternal(min);
                _version++;
            });
        }

        /// <summary>Internal method for ensuring capacity.</summary>
        /// <param name="min">The minimum capacity to ensure.</param>
        private void EnsureCapacityInternal(int min)
        {
            if (min >= _items.Length)
            {
                int newCap = _capacity == 0 ? 1 : _capacity * 2;
                if (newCap < min) newCap = min;
                SetCapacity(newCap);
            }
        }

        /// <summary>Sets the internal capacity of the list.</summary>
        /// <param name="value">The total capacity required.</param>
        private void SetCapacity(int value)
        {
            _interlocker.Lock(() =>
            {
                if (value != _items.Length)
                {
                    if (value < _count)
                        _interlocker.Throw<IndexOutOfRangeException>("Capacity must be greater or equal to the number of stored items.");

                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (_count > 0)
                            Array.Copy(_items, 0, newItems, 0, _count);

                        _items = newItems;
                    }
                    else
                    {
                        _items = _emptyArray;
                    }

                    _capacity = value;
                }
            });
        }

        public void Clear()
        {
            _interlocker.Lock(() =>
            {
                _count = 0;
                _version++;
            });
        }

        /// <summary>Copies the contents of the list to the provided array.</summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="index">The index within array to copy to.</param>
        public void CopyTo(Array array, int index)
        {
            CopyToInternal(array, index);
        }

        /// <summary>Copies the contents of the list to the provided array.</summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index within array to copy to.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyToInternal(array, arrayIndex);
        }

        private void CopyToInternal(Array array, int index)
        {
            _interlocker.Lock(() =>
            {
                int targetAvailable = array.Length - index;
                if (targetAvailable < _count)
                {
                    _interlocker.Throw<IndexOutOfRangeException>("Target array does not have enough free space.");
                }
                else
                {
                    Array.Copy(_items, 0, array, index, _count);
                    _version++;
                }

            });
        }

        public bool Contains(T item)
        {
            bool found = false;
            _interlocker.Lock(() =>
            {
                if (item == null)
                {
                    for (int i = 0; i < _count; i++)
                    {
                        if (_items[i] == null)
                        {
                            found = true;
                            break;
                        }
                    }
                }
                else
                {
                    EqualityComparer<T> c = EqualityComparer<T>.Default;
                    for (int i = 0; i < _count; i++)
                    {
                        if (c.Equals(_items[i], item))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                _version++;
            });

            return found;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public int IndexOf(T item)
        {
            int index = -1;
            _interlocker.Lock(() =>
            {
                index = Array.IndexOf(_items, item);
            });

            return index;
        }

        public void Insert(int index, T item)
        {
            _interlocker.Lock(() =>
            {
                InsertElement(item, index);
                _version++;
            });
        }

        public void InsertRange(IEnumerable<T> collection, int index)
        {
            _interlocker.Lock(() =>
            {
                if (index == _count)
                {
                    AddRange(collection);
                }
                else
                {
                    if (index > _count)
                    {
                        _interlocker.Throw<IndexOutOfRangeException>("Cannot insert beyond the number of items in the list.");
                    }
                    else
                    {
                        // Determine if enumerable is a colllection.
                        ICollection<T> c = collection as ICollection<T>;
                        if (c != null)
                        {
                            EnsureCapacityInternal(_count + c.Count);
                            Array.Copy(_items, index, _items, index + c.Count, _count - index);
                            c.CopyTo(_items, index);
                            _count += c.Count;
                        }
                        else
                        {
                            // Since the collection does not implement ICollection, we're forced to enumerate over items, one at a time.
                            using (IEnumerator<T> e = collection.GetEnumerator())
                            {
                                int startIndex = index;
                                while (e.MoveNext())
                                {
                                    InsertElement(e.Current, startIndex);
                                    startIndex++;
                                }
                            }
                        }

                        _version++;
                    }
                }
            });
        }

        private void InsertElement(T item, int index)
        {
            if (index == _count)
            {
                AddElement(item);
            }
            else
            {
                if (index > _count)
                {
                    _interlocker.Throw<IndexOutOfRangeException>("Cannot insert beyond the number of items in the list.");
                }
                else
                {
                    EnsureCapacityInternal(_count + 1);
                    Array.Copy(_items, index, _items, index + 1, _count - index); // Move items in front up by one index.
                    _items[index] = item;
                    _count++;
                }
            }
        }

        /// <summary>Removes an item from the list.</summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            bool found = false;
            _interlocker.Lock(() =>
            {
                int index = Array.IndexOf<T>(_items, item);
                if (index > -1)
                {
                    RemoveElement(index);
                    _version++;
                    found = true;
                }
            });

            return found;
        }

        /// <summary>Removes an item from the list at the specified index.</summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException("Index cannot be less than 0.");

            _interlocker.Lock(() =>
            {
                if (index >= _count)
                    _interlocker.Throw<IndexOutOfRangeException>("Index must be less than the item count.");

                RemoveElement(index);
                _version++;
            });
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0)
                throw new IndexOutOfRangeException("Index cannot be less than 0.");

            _interlocker.Lock(() =>
            {
                int lastElement = index + count;

                if (index >= _count)
                    _interlocker.Throw<IndexOutOfRangeException>("Index must be less than the item count.");
                else if (lastElement > _count)
                    _interlocker.Throw<IndexOutOfRangeException>("Index plus count cannot exceed the number of items stored in the list.");


                Array.Copy(_items, lastElement, _items, index, _count - lastElement);
                _count -= count;
                Array.Clear(_items, _count, count); // Clear old spaces of re-positioned elements.
                _version++;
            });
        }

        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException("Match cannot be null.");

            int result = 0;
            _interlocker.Lock(() =>
            {
                int freeIndex = 0;   // the first free slot in items array

                // Find the first item which needs to be removed.
                while (freeIndex < _count && !match(_items[freeIndex])) freeIndex++;
                if (freeIndex >= _count)
                {
                    result = 0;
                }
                else
                {
                    int current = freeIndex + 1;
                    while (current < _count)
                    {
                        // Find the first item which needs to be kept.
                        while (current < _count && match(_items[current])) current++;

                        if (current < _count)
                        {
                            // copy item to the free slot.
                            _items[freeIndex++] = _items[current++];
                        }
                    }

                    Array.Clear(_items, freeIndex, _count - freeIndex);
                    result = _count - freeIndex;
                    _count = freeIndex;
                    _version++;
                }
            });

            return result;
        }

        private void RemoveElement(int index)
        {
            _count--;

            // If index was not the last element, update array structure to close gap.
            if (index != _count)
            {
                Array.Copy(_items, index + 1, _items, index, _count - index); // Move items ahead of the index, back one element.
                _items[_count] = default(T); // Clear last element, since it moved back by one index.
            }
            else
            {
                _items[index] = default(T);
            }
        }

        public void Reverse()
        {
            Reverse(0, _count);
        }

        /// <summary>Reverses the elements in a range of this list. Following a call to this method, 
        /// an element in the range given by index and count which was previously located 
        /// at index i will now be located at index index + (index + count - i - 1). 
        /// This method uses the Array.Reverse method to reverse the elements.</summary>
        /// <param name="index">The index to start reversing elements.</param>
        /// <param name="count">The number of elements to reverse</param>
        public void Reverse(int index, int count)
        {
            if (count < 0)
                throw new IndexOutOfRangeException("Count cannot be less than 0");
            else if (index < 0)
                throw new IndexOutOfRangeException("Index cannot be less than 0.");

            _interlocker.Lock(() =>
            {
                if (_count - index < count)
                    _interlocker.Throw<ArgumentException>("Index and count will go out of bounds. Invalid.");

                Array.Reverse(_items, index, count);
                _version++;
            });
        }

        /// Sorts the elements in this list.  Uses the default comparer and 
        /// Array.Sort.
        public void Sort()
        {
            Sort(0, Count, null);
        }

        /// Sorts the elements in this list.  Uses Array.Sort with the
        /// provided comparer.
        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        /// <summary>Sorts the elements in a section of this list. The sort compares the 
        /// elements to each other using the given IComparer interface. If
        /// comparer is null, the elements are compared to each other using 
        /// the IComparable interface, which in that case must be implemented by all 
        /// elements of the list.
        /// This method uses the Array.Sort method to sort the elements. </summary>
        /// <param name="index">The index at which to start sorting.</param>
        /// <param name="count">The number of items to sort.</param>
        /// <param name="comparer">A comparer used for performing sort operation and validation.</param>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if (index < 0)
                throw new IndexOutOfRangeException("Index cannot be less than 0.");
            if (count < 0)
                throw new IndexOutOfRangeException("Count cannot be less than 0.");

            _interlocker.Lock(() =>
            {
                if (_count - index < count)
                    _interlocker.Throw<ArgumentException>("Index plus count cannot exceed the number of items in the list.");

                Array.Sort<T>(_items, index, count, comparer);
                _version++;
            });
        }

        /// <summary>Sets the capacity of this list to the size of the list. This method can 
        /// be used to minimize a list's memory overhead once it is known that no
        /// new elements will be added to the list. To completely clear a list and 
        /// release all memory referenced by the list.</summary>
        public void TrimExcess()
        {
            _interlocker.Lock(() =>
            {
                SetCapacity(_count);
                _version++;
            });
        }

        /// <summary>Copies the contents of the list to an array and returns it.</summary>
        /// <returns>An array containing the list contents.</returns>
        public T[] ToArray()
        {
            T[] result = null;
            _interlocker.Lock(() =>
            {
                result = new T[_count];
                if (_count > 0)
                    Array.Copy(_items, 0, result, 0, _count);
            });

            return result;
        }

        /// <summary>Tries to add an item to the list. This will always succeed due to the lock-free nature of <see cref="ThreadedList{T}"/></summary>
        /// <param name="item">The item to be added.</param>
        /// <returns></returns>
        public bool TryAdd(T item)
        {
            Add(item);
            return true;
        }

        /// <summary>Remove's an item from the end of the list and returns it. This replicates stack functionality (FILO).</summary>
        /// <param name="item">An output for the returned item.</param>
        /// <returns>True if an item was taken.</returns>
        public bool TryTake(out T item)
        {
            bool hasItem = false;
            T temp = default(T);

            _interlocker.Lock(() =>
            {
                if ((_count > 0))
                {
                    temp = _items[--_count];
                    hasItem = true;
                }
                else
                {
                    temp = default(T);
                }

                _version++;
            });

            item = temp;
            return hasItem;
        }

        /// <summary>Runs a foreach loop inside an interlock on the current <see cref="ThreadedList{T}"/> instance. This allows the collection to be iterated over in a thread-safe manner. 
        /// However, it can hurt performance if the loop takes too long to execute while other threads are waiting to access the list. Return true from the callback to break out of the for loop.</summary>
        /// <param name="callback">The callback to run on each iteration. The callback can optionally return true to break out of the loop.</param>
        public void ForEach(Action<T> callback)
        {
            _interlocker.Lock(() =>
            {
                foreach (T item in this)
                    callback(item);
            });
        }

        /// <summary>Runs a foreach loop inside an interlock on the current <see cref="ThreadedList{T}"/> instance. This allows the collection to be iterated over in a thread-safe manner. 
        /// However, it can hurt performance if the loop takes too long to execute while other threads are waiting to access the list. Return true from the callback to break out of the for loop.</summary>
        /// <param name="callback">The callback to run on each iteration. The callback can optionally return true to break out of the loop.</param>
        public void ForEach(Func<T, bool> callback)
        {
            _interlocker.Lock(() =>
            {
                foreach (T item in this)
                {
                    if (callback(item))
                        break;
                }
            });
        }

        /// <summary>Runs a for loop inside an interlock on the current <see cref="ThreadedList{T}"/> instance. instance. This allows the collection to be iterated over in a thread-safe manner. 
        /// However, it can hurt performance if the loop takes too long to execute while other threads are waiting to access the list. <para/>
        /// Return true from the callback to break out of the for loop.</summary>
        /// <param name="start">The start index.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="callback">The callback to run on each iteration. The callback can optionally return true to break out of the loop.</param>
        public void For(int start, int increment, Action<int, T> callback)
        {
            _interlocker.Lock(() =>
            {
                for (int i = start; i < _count; i += increment)
                    callback(i, _items[i]);
            });
        }

        /// <summary>Runs a for loop inside an interlock on the current <see cref="ThreadedList{T}"/> instance. This allows the collection to be iterated over in a thread-safe manner. 
        /// However, it can hurt performance if the loop takes too long to execute while other threads are waiting to access the list. <para/>
        /// Return true from the callback to break out of the for loop.</summary>
        /// <param name="start">The start index.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="callback">The callback to run on each iteration. The callback can optionally return true to break out of the loop.</param>
        public void For(int start, int increment, Func<int, T, bool> callback)
        {
            _interlocker.Lock(() =>
            {
                for (int i = start; i < _count; i += increment)
                {
                    if (callback(i, _items[i]))
                        break;
                }
            });
        }

        /// <summary>Runs a for loop inside an interlock on the current <see cref="ThreadedList{T}"/> instance. This allows the collection to be iterated over in a thread-safe manner. 
        /// However, it can hurt performance if the loop takes too long to execute while other threads are waiting to access the list. <para/>
        /// Return true from the callback to break out of the for loop.</summary>
        /// <param name="start">The start index.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="end">The element to iterate up to.</param>
        /// <param name="callback">The callback to run on each iteration. The callback can optionally return true to break out of the loop.</param>
        public void For(int start, int increment, int end, Func<int, T, bool> callback)
        {
            _interlocker.Lock(() =>
            {
                // Figure out which is the smallest condition value. 
                int last = Math.Min(_count, end);
                for (int i = start; i < last; i += increment)
                {
                    if (callback(i, _items[i]))
                        break;
                }
            });
        }

        /// <summary>Runs a reversed for loop inside an interlock on the current <see cref="ThreadedList{T}"/> instance. This allows the collection to be iterated over in a thread-safe manner. 
        /// However, it can hurt performance if the loop takes too long to execute while other threads are waiting to access the list. <para/>
        /// Return true from the callback to break out of the for loop.</summary>
        /// <param name="decrement">The decremental value.</param>
        /// <param name="callback">The callback to run on each iteration. The callback can optionally return true to break out of the loop.</param>
        public void ForReverse(int decrement, Action<int, T> callback)
        {
            _interlocker.Lock(() =>
            {
                for (int i = _count - 1; i >= 0; i -= decrement)
                    callback(i, _items[i]);
            });
        }

        /// <summary>Runs a reversed for loop inside an interlock on the current <see cref="ThreadedList{T}"/> instance. This allows the collection to be iterated over in a thread-safe manner. 
        /// However, it can hurt performance if the loop takes too long to execute while other threads are waiting to access the list. <para/>
        /// Return true from the callback to break out of the for loop.</summary>
        /// <param name="start">The start index.</param>
        /// <param name="decrement">The decremental value.</param>
        /// <param name="callback">The callback to run on each iteration. The callback can optionally return true to break out of the loop.</param>
        public void ForReverse(int start, int decrement, Action<int, T> callback)
        {
            _interlocker.Lock(() =>
            {
                for (int i = start; i >= 0; i -= decrement)
                    callback(i, _items[i]);
            });
        }

        /// <summary>Runs a reversed for loop inside an interlock on the current <see cref="ThreadedList{T}"/> instance. This allows the collection to be iterated over in a thread-safe manner. 
        /// However, it can hurt performance if the loop takes too long to execute while other threads are waiting to access the list. <para/>
        /// Return true from the callback to break out of the for loop.</summary>
        /// <param name="decrement">The decremental value.</param>
        /// <param name="callback">The callback to run on each iteration. The callback can optionally return true to break out of the loop.</param>
        public void ForReverse(int decrement, Func<int, T, bool> callback)
        {
            _interlocker.Lock(() =>
            {
                for (int i = _count - 1; i >= 0; i -= decrement)
                {
                    if (callback(i, _items[i]))
                        break;
                }
            });
        }

        /// <summary>Runs a reversed for loop inside an interlock on the current <see cref="ThreadedList{T}"/> instance. This allows the collection to be iterated over in a thread-safe manner. 
        /// However, it can hurt performance if the loop takes too long to execute while other threads are waiting to access the list. <para/>
        /// Return true from the callback to break out of the for loop.</summary>
        /// <param name="start">The start index.</param>
        /// <param name="decrement">The decremental value.</param>
        /// <param name="callback">The callback to run on each iteration. The callback can optionally return true to break out of the loop.</param>
        public void ForReverse(int start, int decrement, Func<int, T, bool> callback)
        {
            _interlocker.Lock(() =>
            {
                for (int i = start; i >= 0; i -= decrement)
                {
                    if (callback(i, _items[i]))
                        break;
                }
            });
        }

        /// <summary>Gets or sets a value at the given index.</summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                T result = default(T);
                _interlocker.Lock(() =>
                {
                    if (index >= _count)
                        _interlocker.Throw<IndexOutOfRangeException>("Index must be less than item count.");
                    else if (index < 0)
                        _interlocker.Throw<IndexOutOfRangeException>("Index cannot be less than 0.");

                    result = _items[index];
                });
                return result;
            }

            set
            {
                _interlocker.Lock(() =>
                {
                    if (index >= _count)
                        _interlocker.Throw<IndexOutOfRangeException>("Index must be less than item count.");
                    else if (index < 0)
                        _interlocker.Throw<IndexOutOfRangeException>("Index cannot be less than 0.");

                    _items[index] = value;
                    _version++;
                });
            }
        }

        public override string ToString() => $"Count: {_count}";

        /// <summary>Gets the number of items in the list.</summary>
        public int Count => _count;

        /// <summary>Gets whether or not the list is read-only.</summary>
        public bool IsReadOnly => false;

        /// <summary>Gets whether the List synchronized (thread-safe).</summary>
        public bool IsSynchronized => true;

        object ICollection.SyncRoot => _locker;

        public int Capacity
        {
            get => _capacity;
            set => SetCapacity(value);
        }
    }
}