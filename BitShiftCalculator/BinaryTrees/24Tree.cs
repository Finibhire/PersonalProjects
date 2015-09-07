using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace BitShiftCalculator.BinaryTrees
{
    class TwoFourTree<TKey, TValue>
    {
        private IComparer<TKey> Comparer;

        public TwoFourTree(IComparer<TKey> comparer = null)
        {
            if (comparer == null)
            {
                if (!(typeof(TKey) is IComparable<TKey>))
                {
                    throw new ArgumentException("TKey must inherit IComparable<TKey> or a Comparer<TKey> must be supplied.");
                }
                Comparer = Comparer<TKey>.Default;
            }
            else
            {
                Comparer = comparer;
            }
        }



        private class TwoFourNode<TKey, TValue>
        {
            private IComparer<TKey> Comparer;

            private bool RootNode;
            private TwoFourNode<TKey, TValue> LeftNode;
            private TwoFourNode<TKey, TValue> MiddleLeftNode;
            private TwoFourNode<TKey, TValue> MiddleRightNode;
            private TwoFourNode<TKey, TValue> RightNode;

            private int ValueCount;
            public KeyValuePair<TKey, TValue> LeftPair { get; private set; }
            public KeyValuePair<TKey, TValue> MiddlePair { get; private set; }
            public KeyValuePair<TKey, TValue> RightPair { get; private set; }


            public TwoFourNode(TKey key, TValue value, IComparer<TKey> comparer)
            {
                Comparer = comparer;
                RootNode = true;
                ValueCount = 1;
                LeftPair = new KeyValuePair<TKey, TValue>(key, value);
            }
            private TwoFourNode(KeyValuePair<TKey, TValue> pair, IComparer<TKey> comparer)
            {
                Comparer = comparer;
                RootNode = false;
                ValueCount = 1;
                LeftPair = pair;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool SubTreeNullCheck(TwoFourNode<TKey, TValue> subTree, TKey key, out TValue value)
            {
                if (subTree == null) // nowhere left to go (leaf node?)
                {
                    value = default(TValue);
                    return false;
                }
                else
                {
                    return subTree.FindKey(key, out value);
                }
            }


            private enum PathFollowedDown
            {
                Left = 1, MiddleLeft, MiddleRight, Right
            }

            /// <summary>
            /// This method should be optimized for the highest likely scenarios first and least amount of comparisons nessicary
            /// when some comparisons can be ruled out in best case scenarios.  Granted this leads to a complex and long if-then 
            /// logic tree that could be simplified at no additional cost for worst case scenarios but this tree is all about performance
            /// so let's get it where we can.
            /// </summary>
            /// <param name="key">The Key to search for.</param>
            /// <param name="value">The Value associated with the Key if the key is found.  Otherwise default or null.</param>
            /// <returns>True if the Key exists in the Tree.  False otherwise.</returns>
            public bool Insert(TKey key, TValue value, bool OverwriteValue)
            {
                throw new NotImplementedException();
            }
            private bool Insert(TKey key, TValue value, bool OverwriteValue, TwoFourNode<TKey, TValue> parent, PathFollowedDown path)
            {
                IComparer<TKey> c = Comparer;
                int cr;  // compare results

                if (ValueCount == 3)
                {
                    cr = c.Compare(MiddlePair.Key, key);
                    if (cr == 0)
                    {
                        if (OverwriteValue)
                        {
                            MiddlePair = new KeyValuePair<TKey, TValue>(key, value);
                            return true;
                        }
                        return false;
                    }
                    if (parent.ValueCount == 1)
                    {
                        if (path == PathFollowedDown.MiddleLeft)
                        {
                            parent.MiddlePair = MiddlePair;
                            TwoFourNode<TKey, TValue> tmp = new TwoFourNode<TKey, TValue>(RightPair, c);
                            tmp.LeftNode = MiddleRightNode;
                            tmp.MiddleLeftNode = RightNode;
                            parent.MiddleRightNode = tmp;
                            ValueCount = 1;
                            if (cr < 0)
                            {
                                tmp = this;
                            }
                            tmp.Insert(key, value, OverwriteValue, null, 0);
                        }
                        else
                        {
                            /////////
                        }
                    }
                }

                if (ValueCount == 1)  // less comparisons needed for some extra coding here
                {
                    cr = c.Compare(LeftPair.Key, key);
                    if (cr < 0) // LeftKey
                    {
                        if (LeftNode == null)
                        {
                            MiddlePair = LeftPair;
                            LeftPair = new KeyValuePair<TKey, TValue>(key, value);
                            return true;
                        }
                        return LeftNode.Insert(key, value, OverwriteValue, parent);
                    }
                    else if (cr > 0) // LeftKey
                    {
                        if (LeftNode == null)
                        {
                            MiddlePair = new KeyValuePair<TKey, TValue>(key, value);
                            return true;
                        }
                        return MiddleLeftNode.Insert(key, value, OverwriteValue, parent);
                    }
                    else // (cr == 0)  LeftKey
                    {
                        if (OverwriteValue)
                        {
                            LeftPair = new KeyValuePair<TKey, TValue>(key, value);
                            return true;
                        }
                        return false;
                    }
                }
                else // ValueCount == (2 || 3)  less comparisons needed for some extra coding here continued...
                {
                    cr = c.Compare(MiddlePair.Key, key);
                    if (cr > 0)  // MiddleKey
                    {
                        if (ValueCount == 2)  // don't need to compare against the third value if only 2 exist
                        {
                            if (MiddleRightNode == null)
                            {
                                RightPair = new KeyValuePair<TKey, TValue>(key, value);
                                return true;
                            }
                            return MiddleRightNode.Insert(key, value, OverwriteValue, parent);
                        }
                        else // ValueCount == 3  // compare against thrid value and go down appropriate tree
                        {
                            ///////////
                            cr = c.Compare(RightPair.Key, key);
                            if (cr > 0)  // RightKey
                            {
                                return SubTreeNullCheck(RightNode, key, out value);
                            }
                            else if (cr < 0)  // RightKey
                            {
                                return SubTreeNullCheck(MiddleRightNode, key, out value);
                            }
                            else  // (cr == 0)  RightKey
                            {
                                value = RightPair.Value;
                                return true;
                            }
                        }
                    }
                    else if (cr < 0) // MiddleKey
                    {
                        cr = c.Compare(LeftPair.Key, key);
                        if (cr < 0)  // LeftKey
                        {
                            return SubTreeNullCheck(LeftNode, key, out value);
                        }
                        else if (cr > 0)  // LeftKey
                        {
                            return SubTreeNullCheck(MiddleLeftNode, key, out value);
                        }
                        else  // (cr == 0)  LeftKey
                        {
                            value = LeftPair.Value;
                            return true;
                        }
                    }
                    else // (cr == 0) MiddleKey
                    {
                        value = MiddlePair.Value;
                        return true;
                    }
                }
            } // end FindKey()



            /// <summary>
            /// This method should be optimized for the highest likely scenarios first and least amount of comparisons nessicary
            /// when some comparisons can be ruled out in best case scenarios.  Granted this leads to a complex and long if-then 
            /// logic tree that could be simplified at no additional cost for worst case scenarios but this tree is all about performance
            /// so let's get it where we can.
            /// </summary>
            /// <param name="key">The Key to search for.</param>
            /// <param name="value">The Value associated with the Key if the key is found.  Otherwise default or null.</param>
            /// <returns>True if the Key exists in the Tree.  False otherwise.</returns>
            public bool FindKey(TKey key, out TValue value)
            {
                IComparer<TKey> c = Comparer;
                int cr;  // compare results

                if (ValueCount == 1)  // less comparisons needed for some extra coding here
                {
                    cr = c.Compare(LeftPair.Key, key);
                    if (cr < 0) // LeftKey
                    {
                        return SubTreeNullCheck(LeftNode, key, out value);
                    }
                    else if (cr > 0) // LeftKey
                    {
                        return SubTreeNullCheck(MiddleLeftNode, key, out value);
                    }
                    else // (cr == 0)  LeftKey
                    {
                        value = LeftPair.Value;
                        return true;
                    }
                }
                else // ValueCount == (2 || 3)  less comparisons needed for some extra coding here continued...
                {
                    cr = c.Compare(MiddlePair.Key, key);
                    if (cr > 0)  // MiddleKey
                    {
                        if (ValueCount == 2)  // don't need to compare against the third value if only 2 exist
                        {
                            return SubTreeNullCheck(MiddleRightNode, key, out value);
                        }
                        else // ValueCount == 3  // compare against thrid value and go down appropriate tree
                        {
                            cr = c.Compare(RightPair.Key, key);
                            if (cr > 0)  // RightKey
                            {
                                return SubTreeNullCheck(RightNode, key, out value);
                            }
                            else if (cr < 0)  // RightKey
                            {
                                return SubTreeNullCheck(MiddleRightNode, key, out value);
                            }
                            else  // (cr == 0)  RightKey
                            {
                                value = RightPair.Value;
                                return true;
                            }
                        }
                    }
                    else if (cr < 0) // MiddleKey
                    {
                        cr = c.Compare(LeftPair.Key, key);
                        if (cr < 0)  // LeftKey
                        {
                            return SubTreeNullCheck(LeftNode, key, out value);
                        }
                        else if (cr > 0)  // LeftKey
                        {
                            return SubTreeNullCheck(MiddleLeftNode, key, out value);
                        }
                        else  // (cr == 0)  LeftKey
                        {
                            value = LeftPair.Value;
                            return true;
                        }
                    }
                    else // (cr == 0) MiddleKey
                    {
                        value = MiddlePair.Value;
                        return true;
                    }
                }
            } // end FindKey()


        }
    }
}
