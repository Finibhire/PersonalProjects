using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace BitShiftCalculator.BinaryTrees
{
    /// <summary>
    /// When an instance of this class first uses the Insert() method there is a load time associated with it that
    /// is significantly large.  Because of this load time on new instances of this class vs new instances of SortedList
    /// it will take inserting over 25 entries before this class will save you time as compared to SortedList.  Actual 
    /// times measured by my computer averaged over 10,000 cases to insert 25 entries into a new instance of a SortedList 
    /// or RBTree is 10.83ms.
    /// 
    /// However when an instance of an RBTree has already been used and clear() has been called vs a SortedList that has
    /// been used and clear() has been called.  RBTree will be faster than SortedList on average from the very first entry
    /// added.  Starting at about 52% of the time SortedList took to add the entry.  That value fluctuates between 45% and
    /// 80% for the first 50 entries at which point the values remain very consistant.
    ///  
    /// @ 100   entries about 53% of the time SortedList used &    3.6 ms faster
    /// @ 1000  entries about 55% of the time SortedList used &   40.7 ms faster
    /// @ 10000 entries under 52% of the time SortedList used &  452   ms faster
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    class RedBlackTree<TKey, TValue> : IDictionary<TKey, TValue>, System.Collections.IDictionary
    {
        private IComparer<TKey> Comparer;
        private RBNode<TKey, TValue> root;

        public int Count { get; private set; }
        public bool IsReadOnly { get { return false; } }

#region IDictionary Wrappers
        public TValue this[TKey key]
        {
            get
            {
                //TODO:
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public ICollection<TValue> Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        bool System.Collections.Generic.ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }
        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }
        public void CopyTo(KeyValuePair<TKey, TValue>[] destination, int index)
        {
            throw new NotImplementedException();
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            Insert(key, value, false);
        }
        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Gets an object that can be used to synchronize access to the ICollection.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }
        /// <summary>
        /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Adds an element with the provided key and value to the IDictionary object.
        /// </summary>
        void System.Collections.Generic.ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value, false);
        }

        //////////////////////////////////////////////////////
        //////////////////  - IDictionary - //////////////////
        //////////////////////////////////////////////////////
        /// <summary>
        /// Gets the number of elements contained in the ICollection.
        /// </summary>
        int System.Collections.ICollection.Count
        {
            get
            {
                return Count;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the IDictionary object has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the IDictionary object is read-only.
        /// </summary>
        bool System.Collections.IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        object System.Collections.IDictionary.this[object key]
        {
            get
            {
                TKey tKey;
                if (key is TKey)
                {
                    tKey = (TKey)key;
                }
                else
                {
                    throw new ArgumentException(
                        "Supplied key is not the correct type." + System.Environment.NewLine + 
                        "key.GetType().FullName == " + key.GetType().FullName + System.Environment.NewLine +
                        "Expected: " + typeof(TKey).FullName);
                }
                return (object)(this[tKey]);
            }
            set
            {
                TKey tKey;
                TValue tValue;
                if (key is TKey && value is TValue)
                {
                    tKey = (TKey)key;
                    tValue = (TValue)value;
                }
                else
                {
                    throw new ArgumentException(
                        "Supplied key or value is not the correct type." + System.Environment.NewLine +
                        "key.GetType().FullName == " + key.GetType().FullName + System.Environment.NewLine +
                        "value.GetType().FullName == " + value.GetType().FullName + System.Environment.NewLine +
                        "Expected: key == " + typeof(TKey).FullName + " && value == " + typeof(TValue).FullName);
                }
                this[tKey] = tValue;
            }
        }
        /// <summary>
        /// Gets an ICollection object containing the keys of the IDictionary object.
        /// </summary>
        System.Collections.ICollection System.Collections.IDictionary.Keys
        {
            get
            {
                return (System.Collections.ICollection)this.Keys;
                // this.Keys returns an array as a base class that is also an ICollection so this cast always works.
            }
        }
        /// <summary>
        /// Gets an ICollection object containing the values in the IDictionary object.
        /// </summary>
        System.Collections.ICollection System.Collections.IDictionary.Values
        {
            get
            {
                return (System.Collections.ICollection)this.Values;  
                // this.Values returns an array as a base class that is also an ICollection so this cast always works.
            }
        }
        /// <summary>
        /// Adds an element with the provided key and value to the IDictionary object.
        /// </summary>
        void System.Collections.IDictionary.Add(object key, object value)
        {
            if (!(key is TKey))
            {
                throw new ArgumentException(
                    "key is not the correct type. (key.GetType().FullName == " +
                    key.GetType().FullName +
                    ")  Expected: " + typeof(TKey).FullName);
            }
            if (!(value is TValue))
            {
                throw new ArgumentException(
                    "value is not the correct type. (value.GetType().FullName == " +
                    value.GetType().FullName +
                    ")  Expected: " + typeof(TValue).FullName);
            }
            Insert((TKey)key, (TValue)value, false);
        }
        /// <summary>
        /// Removes all elements from the IDictionary object.
        /// </summary>
        void System.Collections.IDictionary.Clear()
        {
            Clear();
        }
        /// <summary>
        /// Determines whether the IDictionary object contains an element with the specified key.
        /// </summary>
        bool System.Collections.IDictionary.Contains(object key)
        {
            if (!(key is TKey))
            {
                throw new ArgumentException(
                    "key is not the correct type. (key.GetType().FullName == " +
                    key.GetType().FullName +
                    ")  Expected: " + typeof(TKey).FullName);
            }
            return ContainsKey((TKey)key);
        }
        /// <summary>
        /// Returns an IDictionaryEnumerator object for the IDictionary object.
        /// </summary>
        System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the element with the specified key from the IDictionary object.
        /// </summary>
        void System.Collections.IDictionary.Remove(object key)
        {
            throw new NotImplementedException();
        }
#endregion

        /// <summary>
        /// Creates a new instance of a Red-Black sorted and ballanced binary tree structure.  TKey must impliment the
        /// IComparable<TKey> interface or an IComparer<TKey> must be supplied.
        /// </summary>
        /// <param name="comparer">If supplied this comparer will be used over the IComparable<TKey> interface.</param>
        public RedBlackTree(IComparer<TKey> comparer = null)
        {
            if (comparer == null)
            {
                bool interfaceMissing = true;
                Type[] interfaces = (typeof(TKey)).GetInterfaces();
                for (int i = 0; i < interfaces.Length; i++)
                {
                    if (interfaces[i] == typeof(IComparable<TKey>))
                    {
                        interfaceMissing = false;
                        break;
                    }
                }
                if (interfaceMissing)
                {
                    throw new ArgumentException("TKey must inherit IComparable<TKey> or a Comparer<TKey> must be supplied.");
                }
                Comparer = Comparer<TKey>.Default;
            }
            else
            {
                Comparer = comparer;
            }
            root = null;
            Count = 0;
        }

        public ICollection<TKey> Keys
        {
            get
            {
                TKey[] rtn = new TKey[Count];
                if (root != null)
                {
                    root.GetKeys(rtn, 0, Count);
                }
                return rtn;
            }
        }

        public void Clear()
        {
            root = null;
            Count = 0;
        }

        /// <summary>
        /// Add a key and associated value to the Red-Black Tree structure.
        /// Throws an exception if a duplicate key is found.  This can be avoided with the indexer assignment statement
        /// or by setting canOverwriteValue to true. 
        /// example:  RBTree[Tkey] = (TValue)Value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="canOverwriteValue">
        /// If true when a duplicate key is found the value associated with that key is updated to the 
        /// value passed into this method.
        /// </param>
        /// <returns>True if the operation was succesfull.</returns>
        public bool Insert(TKey key, TValue value, bool canOverwriteValue = false)
        {
            bool success;

            if (key == null)
                throw new ArgumentNullException("key");

            if (root == null)
            {
                root = new RBNode<TKey, TValue>(key, value, Comparer);
                success = true;
                Count++;
            }
            else
            {
                bool valueOverwritten;
                success = root.Insert(key, value, canOverwriteValue, out valueOverwritten, out root);
                if (!success)
                {
                    throw new ArgumentException("An element with the same key already exists in the IDictionary<TKey, TValue>.");
                }
                else if (!valueOverwritten)
                {
                    Count++;
                }
            }
            return success;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }
        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

#pragma warning disable 693
        private class RBNode<TKey, TValue>
#pragma warning restore
        {
            private IComparer<TKey> Comparer;
            private bool IsBlack;
            private TKey Key;
            private TValue Value;
            private RBNode<TKey, TValue> leftChild;
            private RBNode<TKey, TValue> rightChild;
            private RBNode<TKey, TValue> parent;

            public RBNode(TKey key, TValue value, IComparer<TKey> comparer) 
                : this(key, value, comparer, null, true) 
            { }
            private RBNode(TKey key, TValue value, IComparer<TKey> comparer, RBNode<TKey, TValue> parent, bool isBlack = false)
            {
                this.Comparer = comparer;
                this.Key = key;
                this.Value = value;
                this.IsBlack = isBlack;
                this.parent = parent;
            }
            
            /// <param name="dest">array to fill with the keys</param>
            /// <param name="fillIndex">start filling the array at this index</param>
            /// <param name="lengthPlusStartIndex">last index to fill + 1</param>
            /// <returns>startIndex + number of items filled in the array</returns>
            public int GetKeys(TKey[] dest, int fillIndex, int lengthPlusStartIndex)
            {
                if (fillIndex < lengthPlusStartIndex)
                {
                    if (leftChild != null)
                    {
                        fillIndex = leftChild.GetKeys(dest, fillIndex, lengthPlusStartIndex);
                        if (fillIndex >= lengthPlusStartIndex)
                        {
                            return fillIndex;
                        }
                    }
                    dest[fillIndex] = Key;
                    fillIndex++;
                    if (rightChild != null)
                    {
                        fillIndex = rightChild.GetKeys(dest, fillIndex, lengthPlusStartIndex);
                    }
                }
                return fillIndex;
            }

            /// <summary>
            /// For the Insert() method, this method is used to find and fix issues that compromise the properties of
            /// the Red-Black tree after a value is inserted.
            /// To be called on the return trip (decrimenting call stack) from the search for the insert point.
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="gParent"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void InsertConflictResolution()
            {
                if (parent == null) // this must be the root
                {
                    IsBlack = true;
                    return;
                }

                if (!(IsBlack || parent.IsBlack))  // true when this and parent are both red
                {
                    RBNode<TKey, TValue> uncle, gParent;
                    gParent = parent.parent;  // exists because parent.IsRed
                    bool leftOfParent, leftOfGParent;
                    if (parent == gParent.leftChild) 
                    {
                        leftOfGParent = true;
                        uncle = gParent.rightChild;
                    }
                    else
                    {
                        leftOfGParent = false;
                        uncle = gParent.leftChild;
                    }

                    if (uncle != null && !uncle.IsBlack)
                    {
                        uncle.IsBlack = true;
                        parent.IsBlack = true;
                        gParent.IsBlack = false;
                        return;
                    }
                    else
                    {
                        leftOfParent = (this == parent.leftChild) ? true : false;
                        if (leftOfGParent)
                        {
                            if (!leftOfParent)  // rotate left on parent
                            {
                                if (leftChild != null) 
                                    leftChild.parent = parent;
                                parent.rightChild = leftChild;
                                leftChild = parent;
                                gParent.leftChild = this;
                                parent.parent = this;
                                parent = gParent;
                                return;
                            }
                            else // rotate right on grand parrent
                            {
                                gParent.IsBlack = false;
                                parent.IsBlack = true;
                                parent.parent = gParent.parent;

                                if (gParent.parent != null)
                                {
                                    if (gParent.parent.leftChild == gParent)
                                    {
                                        gParent.parent.leftChild = parent;
                                    }
                                    else
                                    {
                                        gParent.parent.rightChild = parent;
                                    }
                                }

                                gParent.parent = parent;
                                gParent.leftChild = parent.rightChild;
                                if (parent.rightChild != null)
                                    parent.rightChild.parent = gParent;
                                parent.rightChild = gParent;
                                return;
                            }
                        }
                        else // (leftOfGParent == false)
                        {
                            if (leftOfParent)  // rotate right on parent
                            {
                                if (rightChild != null)
                                    rightChild.parent = parent;
                                gParent.rightChild = this;
                                parent.parent = this;
                                parent.leftChild = this.rightChild;
                                this.rightChild = parent;
                                this.parent = gParent;
                                return;
                            }
                            else // rotate left on grand parent
                            {
                                gParent.IsBlack = false;
                                parent.IsBlack = true;
                                parent.parent = gParent.parent;

                                if (gParent.parent != null)
                                {
                                    if (gParent.parent.leftChild == gParent)
                                    {
                                        gParent.parent.leftChild = parent;
                                    }
                                    else
                                    {
                                        gParent.parent.rightChild = parent;
                                    }
                                }

                                gParent.parent = this.parent;
                                gParent.rightChild = parent.leftChild;
                                if (parent.leftChild != null)
                                    parent.leftChild.parent = gParent;
                                parent.leftChild = gParent;
                                return;
                            }
                        }
                    }
                }
                return;  // parent.isblack == true || this.isblack == true
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            /// <param name="canOverwriteValue">
            /// If true then this method will overwrite the value data at the key location with the value passed in.</param>
            /// <param name="didOverwriteValue">
            /// Returns true if the method did overwrite a value at the passed in key location.</param>
            /// <param name="newRoot">Returns the root node for the RBTree which can change during this operation.</param>
            /// <returns>
            /// True if the insert was successful; false otherwise (which happens when a key already exists and
            /// canOverwriteValue is set to false).
            /// </returns>
            public bool Insert(TKey key, TValue value, bool canOverwriteValue, out bool didOverwriteValue, out RBNode<TKey, TValue> newRoot)
            {
                bool success;
                success = Insert(key, value, canOverwriteValue, out didOverwriteValue);
                newRoot = (parent == null) ? this : parent;
                return success;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool Insert(TKey key, TValue value, bool canOverwriteValue, out bool didOverwriteValue)
            {
                RBNode<TKey, TValue> newNode;
                bool success;

                int cr = Comparer.Compare(key, Key);  // compare result
                if (cr < 0)
                {
                    if (leftChild == null)
                    {
                        newNode = new RBNode<TKey,TValue>(key, value, Comparer, this);
                        leftChild = newNode;
                        newNode.InsertConflictResolution();
                        InsertConflictResolution();
                        didOverwriteValue = false;
                        return true;
                    }
                    else
                    {
                        success = leftChild.Insert(key, value, canOverwriteValue, out didOverwriteValue);
                        InsertConflictResolution();
                        return success;
                    }
                }
                else if (cr > 0)
                {
                    if (rightChild == null)
                    {
                        newNode = new RBNode<TKey, TValue>(key, value, Comparer, this);
                        rightChild = newNode;
                        newNode.InsertConflictResolution();
                        InsertConflictResolution();
                        didOverwriteValue = false;
                        return true;
                    }
                    else
                    {
                        success = rightChild.Insert(key, value, canOverwriteValue, out didOverwriteValue);
                        InsertConflictResolution();
                        return success;
                    }
                }
                else // (cr == 0)
                {
                    if (canOverwriteValue)
                    {
                        this.Value = value;
                        didOverwriteValue = true;
                        return true;
                    }
                    else
                    {
                        didOverwriteValue = false;
                        return false;
                    }
                }
            }

            

        }
    }
}
