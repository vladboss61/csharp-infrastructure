using System;
using System.Collections.Generic;
using System.Text;

namespace CSharp.Infrastructure.Tree
{
    public class AvlNode<T>
    {
        public static readonly AvlNode<T> Empty = new NullNode();

        internal sealed class NullNode : AvlNode<T>
        {
            public override bool IsEmpty => true;
        }

        private readonly AvlNode<T> _left;
        private readonly AvlNode<T> _right;
        private readonly int _count;
        private readonly int _depth;
        
        public T Value { get; set; }

        public virtual bool IsEmpty => false;

        public AvlNode<T> Left => _left;

        public AvlNode<T> Right => _right;

        public int Count => _count;

        int Balance
        {
            get
            {
                if (IsEmpty)
                {
                    return 0;
                }
                return _left._depth - _right._depth;
            }
        }

        int Depth => _depth;

        public AvlNode()
        {
            _right = Empty;
            _left = Empty;
        }

        public AvlNode(T val) : this(val, Empty, Empty)
        {
        }

        AvlNode(T val, AvlNode<T> lt, AvlNode<T> gt)
        {
            Value = val;
            _left = lt;
            _right = gt;
            _count = 1 + _left._count + _right._count;
            _depth = 1 + Math.Max(_left._depth, _right._depth);
        }

        /// <summary>
        /// Return the subtree with the min value at the root, or Empty if Empty
        /// </summary>
        AvlNode<T> Min
        {
            get
            {
                if (IsEmpty)
                    return Empty;
                var dict = this;
                var next = dict._left;
                while (next != Empty)
                {
                    dict = next;
                    next = dict._left;
                }
                return dict;
            }
        }

        /// <summary>
        /// Fix the root balance if LTDict and GTDict have good balance
        /// Used to keep the depth less than 1.44 log_2 N (AVL tree)
        /// </summary>
        AvlNode<T> FixRootBalance()
        {
            int bal = Balance;
            if (Math.Abs(bal) < 2)
                return this;

            if (bal == 2)
            {
                if (_left.Balance == 1 || _left.Balance == 0)
                {
                    //Easy case:
                    return this.RotateToGT();
                }
                if (_left.Balance == -1)
                {
                    //Rotate LTDict:
                    var newLt = _left.RotateToLT();
                    var newRoot = NewOrMutate(Value, newLt, _right);
                    return newRoot.RotateToGT();
                }
                throw new Exception($"LTDict too unbalanced: {_left.Balance}");
            }
            if (bal == -2)
            {
                if (_right.Balance == -1 || _right.Balance == 0)
                {
                    //Easy case:
                    return this.RotateToLT();
                }
                if (_right.Balance == 1)
                {
                    //Rotate GTDict:
                    var newGt = _right.RotateToGT();
                    var newRoot = NewOrMutate(Value, _left, newGt);
                    return newRoot.RotateToLT();
                }
                throw new Exception($"LTDict too unbalanced: {_left.Balance}");
            }
            //In this case we can show: |bal| > 2
            //if( Math.Abs(bal) > 2 ) {
            throw new Exception($"Tree too out of balance: {Balance}");
        }

        public AvlNode<T> SearchNode(T value, Comparison<T> comparer)
        {
            var dict = this;
            while (dict != Empty)
            {
                int comp = comparer(dict.Value, value);
                if (comp < 0)
                {
                    dict = dict._right;
                }
                else if (comp > 0)
                {
                    dict = dict._left;
                }
                else
                {
                    //Awesome:
                    return dict;
                }
            }
            return Empty;
        }

        /// <summary>
        /// Return a new tree with the key-value pair inserted
        /// If the key is already present, it replaces the value
        /// This operation is O(Log N) where N is the number of keys
        /// </summary>
        public AvlNode<T> InsertIntoNew(int index, T val)
        {
            if (IsEmpty)
                return new AvlNode<T>(val);

            AvlNode<T> newLt = _left;
            AvlNode<T> newGt = _right;

            if (index <= _left._count)
            {
                newLt = _left.InsertIntoNew(index, val);
            }
            else
            {
                newGt = _right.InsertIntoNew(index - _left._count - 1, val);
            }

            var newRoot = NewOrMutate(Value, newLt, newGt);
            return newRoot.FixRootBalance();
        }

        /// <summary>
        /// Return a new tree with the key-value pair inserted
        /// If the key is already present, it replaces the value
        /// This operation is O(Log N) where N is the number of keys
        /// </summary>
        public AvlNode<T> InsertIntoNew(T val, Comparison<T> comparer)
        {
            if (IsEmpty)
                return new AvlNode<T>(val);

            AvlNode<T> newlt = _left;
            AvlNode<T> newgt = _right;

            int comp = comparer(Value, val);
            T newValue = Value;
            if (comp < 0)
            {
                //Let the GTDict put it in:
                newgt = _right.InsertIntoNew(val, comparer);
            }
            else if (comp > 0)
            {
                //Let the LTDict put it in:
                newlt = _left.InsertIntoNew(val, comparer);
            }
            else
            {
                //Replace the current value:
                newValue = val;
            }
            var newroot = NewOrMutate(newValue, newlt, newgt);
            return newroot.FixRootBalance();
        }

        public AvlNode<T> SetItem(int index, T val)
        {
            AvlNode<T> newLt = _left;
            AvlNode<T> newGt = _right;

            if (index < _left._count)
            {
                newLt = _left.SetItem(index, val);
            }
            else if (index > _left._count)
            {
                newGt = _right.SetItem(index - _left._count - 1, val);
            }
            else
            {
                return NewOrMutate(val, newLt, newGt);
            }
            return NewOrMutate(Value, newLt, newGt);
        }

        public AvlNode<T> GetNodeAt(int index)
        {
            if (index < _left._count)
            {
                return _left.GetNodeAt(index);
            }
            if (index > _left._count)
            {
                return _right.GetNodeAt(index - _left._count - 1);
            }
            return this;
        }

        /// <summary>
        /// Try to remove the key, and return the resulting Dict
        /// if the key is not found, old_node is Empty, else old_node is the Dict
        /// with matching Key
        /// </summary>
        public AvlNode<T> RemoveFromNew(int index, out bool found)
        {
            if (IsEmpty)
            {
                found = false;
                return Empty;
            }

            if (index < _left._count)
            {
                var newLt = _left.RemoveFromNew(index, out found);
                if (!found)
                {
                    //Not found, so nothing changed
                    return this;
                }
                var newRoot = NewOrMutate(Value, newLt, _right);
                return newRoot.FixRootBalance();
            }

            if (index > _left._count)
            {
                var newGt = _right.RemoveFromNew(index - _left._count - 1, out found);
                if (!found)
                {
                    //Not found, so nothing changed
                    return this;
                }
                var newRoot = NewOrMutate(Value, _left, newGt);
                return newRoot.FixRootBalance();
            }

            //found it
            found = true;
            return RemoveRoot();
        }

        /// <summary>
        /// Try to remove the key, and return the resulting Dict
        /// if the key is not found, old_node is Empty, else old_node is the Dict
        /// with matching Key
        /// </summary>
        public AvlNode<T> RemoveFromNew(T val, Comparison<T> comparer, out bool found)
        {
            if (IsEmpty)
            {
                found = false;
                return Empty;
            }
            int comp = comparer(Value, val);
            if (comp < 0)
            {
                var newGt = _right.RemoveFromNew(val, comparer, out found);
                if (!found)
                {
                    //Not found, so nothing changed
                    return this;
                }
                var newRoot = NewOrMutate(Value, _left, newGt);
                return newRoot.FixRootBalance();
            }
            if (comp > 0)
            {
                var newLt = _left.RemoveFromNew(val, comparer, out found);
                if (!found)
                {
                    //Not found, so nothing changed
                    return this;
                }
                var newroot = NewOrMutate(Value, newLt, _right);
                return newroot.FixRootBalance();
            }
            //found it
            found = true;
            return RemoveRoot();
        }

        AvlNode<T> RemoveMax(out AvlNode<T> max)
        {
            if (IsEmpty)
            {
                max = Empty;
                return Empty;
            }
            if (_right.IsEmpty)
            {
                //We are the max:
                max = this;
                return _left;
            }
            else
            {
                //Go down:
                var newGt = _right.RemoveMax(out max);
                var newRoot = NewOrMutate(Value, _left, newGt);
                return newRoot.FixRootBalance();
            }
        }

        AvlNode<T> RemoveMin(out AvlNode<T> min)
        {
            if (IsEmpty)
            {
                min = Empty;
                return Empty;
            }
            if (_left.IsEmpty)
            {
                //We are the minimum:
                min = this;
                return _right;
            }
            //Go down:
            var newLt = _left.RemoveMin(out min);
            var newRoot = NewOrMutate(Value, newLt, _right);
            return newRoot.FixRootBalance();
        }

        /// <summary>
        /// Return a new dict with the root key-value pair removed
        /// </summary>
        AvlNode<T> RemoveRoot()
        {
            if (IsEmpty)
            {
                return this;
            }
            if (_left.IsEmpty)
            {
                return _right;
            }
            if (_right.IsEmpty)
            {
                return _left;
            }
            //Neither are empty:
            if (_left._count < _right._count)
            {
                //LTDict has fewer, so promote from GTDict to minimize depth
                AvlNode<T> min;
                var newGt = _right.RemoveMin(out min);
                var newRoot = NewOrMutate(min.Value, _left, newGt);
                return newRoot.FixRootBalance();
            }
            else
            {
                AvlNode<T> max;
                var newLt = _left.RemoveMax(out max);
                var newRoot = NewOrMutate(max.Value, newLt, _right);
                return newRoot.FixRootBalance();
            }
        }

        /// <summary>
        /// Move the Root into the GTDict and promote LTDict node up
        /// If LTDict is empty, this operation returns this
        /// </summary>
        AvlNode<T> RotateToGT()
        {
            if (_left.IsEmpty || IsEmpty)
            {
                return this;
            }
            var newLeft = _left;
            var lL = newLeft._left;
            var lR = newLeft._right;
            var newRight = NewOrMutate(Value, lR, _right);
            return newLeft.NewOrMutate(newLeft.Value, lL, newRight);
        }

        /// <summary>
        /// Move the Root into the LTDict and promote GTDict node up
        /// If GTDict is empty, this operation returns this
        /// </summary>
        AvlNode<T> RotateToLT()
        {
            if (_right.IsEmpty || IsEmpty)
            {
                return this;
            }
            var newRight = _right;
            var rL = newRight._left;
            var rR = newRight._right;
            var newLeft = NewOrMutate(Value, _left, rL);
            return newRight.NewOrMutate(newRight.Value, newLeft, rR);
        }

        /// <summary>
        /// Enumerate from largest to smallest key
        /// </summary>
        public IEnumerator<T> GetEnumerator(bool reverse)
        {
            var visited = new Stack<AvlNode<T>>();
            visited.Push(this);
            while (visited.Count > 0)
            {
                var this_d = visited.Pop();
            continue_without_pop:
                if (this_d.IsEmpty)
                {
                    continue;
                }
                if (reverse)
                {
                    if (this_d._right.IsEmpty)
                    {
                        //This is the next biggest value in the Dict:
                        yield return this_d.Value;
                        this_d = this_d._left;
                        goto continue_without_pop;
                    }
                    else
                    {
                        //Break it up
                        visited.Push(this_d._left);
                        visited.Push(new AvlNode<T>(this_d.Value));
                        this_d = this_d._right;
                        goto continue_without_pop;
                    }
                }
                else
                {
                    if (this_d._left.IsEmpty)
                    {
                        //This is the next biggest value in the Dict:
                        yield return this_d.Value;
                        this_d = this_d._right;
                        goto continue_without_pop;
                    }
                    else
                    {
                        //Break it up
                        if (!this_d._right.IsEmpty)
                            visited.Push(this_d._right);
                        visited.Push(new AvlNode<T>(this_d.Value));
                        this_d = this_d._left;
                        goto continue_without_pop;
                    }
                }
            }
        }

        public IEnumerable<T> Enumerate(int index, int count, bool reverse)
        { 
            int i;
            var e = GetEnumerator(reverse);
            if (!reverse)
            {
                i = 0;
                while (e.MoveNext())
                {
                    if (index <= i)
                        yield return e.Current;
                    i++;
                    if (i >= index + count)
                        break;
                }
            }
            else
            {
                i = Count - 1;
                while (e.MoveNext())
                {
                    if (i <= index)
                        yield return e.Current;
                    i--;
                    if (i <= index - count)
                        break;
                }
            }
        }

        public virtual AvlNode<T> NewOrMutate(T newValue, AvlNode<T> newLeft, AvlNode<T> newRight)
        {
            return new AvlNode<T>(newValue, newLeft, newRight);
        }
    }
}
