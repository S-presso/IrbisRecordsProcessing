/* DoublyLinkedList.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Äâàæäû-ñâÿçàííûé ñïèñîê
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DoublyLinkedList < T > : IList < T >
    {
        #region Public properties

        private DoublyLinkedListNode < T > _firstNode;

        /// <summary>
        /// Ïåðâûé ýëåìåíò.
        /// </summary>
        /// <value></value>
        public DoublyLinkedListNode < T > FirstNode
        {
            [DebuggerStepThrough]
            get
            {
                return _firstNode;
            }
            [DebuggerStepThrough]
            protected set
            {
                _firstNode = value;
            }
        }

        private DoublyLinkedListNode < T > _lastNode;

        /// <summary>
        /// Ïîñëåäíèé ýëåìåíò.
        /// </summary>
        /// <value></value>
        public DoublyLinkedListNode < T > LastNode
        {
            [DebuggerStepThrough]
            get
            {
                return _lastNode;
            }
            [DebuggerStepThrough]
            protected set
            {
                _lastNode = value;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DoublyLinkedList&lt;T&gt;"/> class.
        /// </summary>
        public DoublyLinkedList ( )
        {
        }

        #endregion

        #region Private members

        /// <summary>
        /// _s the node at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        [CLSCompliant ( false )]
        protected DoublyLinkedListNode < T > _NodeAt ( int index )
        {
            DoublyLinkedListNode < T > result = FirstNode;

            for ( int i = 0; i < index; i++ )
            {
                if ( result == null )
                {
                    break;
                }
                result = result.Next;
            }
            return result;
        }

        /// <summary>
        /// _s the create node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [CLSCompliant ( false )]
        protected DoublyLinkedListNode < T > _CreateNode ( T value )
        {
            DoublyLinkedListNode < T > result = new DoublyLinkedListNode < T >
                ( value );
            result.List = this;
            return result;
        }

        #endregion

        #region IList<T> members

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"></see>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        /// <returns>
        /// The index of item if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf ( T item )
        {
            IEnumerator < T > enumerator = GetEnumerator ();
            for ( int i = 0; enumerator.MoveNext (); i++ )
            {
                if ( enumerator.Current.Equals ( item ) )
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"></see> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
        public void Insert
            (
            int index,
            T item )
        {
            DoublyLinkedListNode < T > exisingNode = _NodeAt ( index );
            DoublyLinkedListNode < T > newNode = _CreateNode ( item );
            DoublyLinkedListNode < T > nextNode = exisingNode.Next;
            newNode.Previous = exisingNode;
            newNode.Next = nextNode;
            exisingNode.Next = exisingNode;
            if ( nextNode != null )
            {
                nextNode.Previous = newNode;
            }
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"></see> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList`1"></see> is read-only.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
        public void RemoveAt ( int index )
        {
            DoublyLinkedListNode < T > nodeToRemove = _NodeAt ( index );
            if ( nodeToRemove != null )
            {
                DoublyLinkedListNode < T > nextNode = nodeToRemove.Next;
                DoublyLinkedListNode < T > previousNode = nodeToRemove.Previous;
                if ( previousNode != null )
                {
                    previousNode.Next = nextNode;
                }
                if ( nextNode != null )
                {
                    nextNode.Previous = previousNode;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:T"/> at the specified index.
        /// </summary>
        /// <value></value>
        public T this [ int index ]
        {
            get
            {
                DoublyLinkedListNode < T > node = _NodeAt ( index );
                return node.Value;
            }
            set
            {
                DoublyLinkedListNode < T > node = _NodeAt ( index );
                node.Value = value;
            }
        }

        #endregion

        #region ICollection<T> members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public void Add ( T item )
        {
            DoublyLinkedListNode < T > node = _CreateNode ( item );
            if ( LastNode != null )
            {
                LastNode.Next = node;
            }
            node.Previous = LastNode;
            LastNode = node;
            if ( FirstNode == null )
            {
                FirstNode = node;
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only. </exception>
        public void Clear ( )
        {
            _firstNode = null;
            _lastNode = null;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        /// </returns>
        public bool Contains ( T item )
        {
            IEnumerator < T > enumerator = GetEnumerator ();
            while ( enumerator.MoveNext () )
            {
                if ( enumerator.Current.Equals ( item ) )
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
        /// <exception cref="T:System.ArgumentNullException">array is null.</exception>
        /// <exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
        public void CopyTo
            (
            T[] array,
            int arrayIndex )
        {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
        public int Count
        {
            get
            {
                IEnumerator < T > enumerator = GetEnumerator ();
                int result = 0;
                while ( enumerator.MoveNext () )
                {
                    result++;
                }
                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
        public bool Remove ( T item )
        {
            throw new NotImplementedException ();
        }

        #endregion

        #region IEnumerable members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator ( )
        {
            for ( DoublyLinkedListNode < T > node = FirstNode;
                  node != null;
                  node = node.Next )
            {
                yield return node.Value;
            }
        }

        #endregion

        #region IEnumerable<T> members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator < T > GetEnumerator ( )
        {
            for ( DoublyLinkedListNode < T > node = FirstNode;
                  node != null;
                  node = node.Next )
            {
                yield return node.Value;
            }
        }

        #endregion
    }
}
