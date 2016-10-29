/* Pair.cs -- holds pair of objects of given types.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Simple container that holds pair of objects of given types.
    /// </summary>
    /// <typeparam name="T1">Type of first object.</typeparam>
    /// <typeparam name="T2">Type of second object.</typeparam>
    /// <seealso cref="Triplet{T1,T2,T3}"/>
    /// <seealso cref="Quartet{T1,T2,T3,T4}"/>
    [Done]
    [Serializable]
    [TypeConverter ( typeof ( IndexableConverter ) )]
    public class Pair < T1, T2 >
        : IList,
          IIndexable,
          ICloneable
    {
        #region Properties

        private T1 _first;

        /// <summary>
        /// First element of the pair.
        /// </summary>
        /// <value>Value of first element.</value>
        [XmlElement ( "first" )]
        public T1 First
        {
            [DebuggerStepThrough]
            get
            {
                return _first;
            }
            [DebuggerStepThrough]
            set
            {
                if ( _isReadOnly )
                {
                    throw new NotSupportedException ();
                }
                _first = value;
            }
        }

        private T2 _second;

        /// <summary>
        /// Second element of the pair.
        /// </summary>
        /// <value>Value of second element.</value>
        [XmlElement ( "second" )]
        public T2 Second
        {
            [DebuggerStepThrough]
            get
            {
                return _second;
            }
            [DebuggerStepThrough]
            set
            {
                if ( _isReadOnly )
                {
                    throw new NotSupportedException ();
                }
                _second = value;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Pair{T1, T2}"/> class.
        /// Both <see cref="First"/> and <see cref="Second"/>
        /// members of the pair remains unassigned.
        /// </summary>
        public Pair ( )
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Pair{T1, T2}"/> class.
        /// </summary>
        /// <param name="pair">The pair.</param>
        public Pair ( Pair < T1, T2 > pair )
        {
            ArgumentUtility.NotNull
                (
                 pair,
                 "pair" );

            First = pair.First;
            Second = pair.Second;
            _isReadOnly = pair._isReadOnly;
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Pair{T1, T2}"/> class
        /// and assigns <see cref="First"/> member of the pair.
        /// <see cref="Second"/> member remains unassigned.
        /// </summary>
        /// <param name="first">Initial value for <see cref="First"/>
        /// member of the pair.</param>
        public Pair ( T1 first )
        {
            First = first;
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Pair{T1, T2}"/> class.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        public Pair
            (
            T1 first,
            T2 second )
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="Pair{T1, T2}"/> class.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="readOnly">Specifies whether the pair
        /// should be read-only.</param>
        public Pair
            (
            T1 first,
            T2 second,
            bool readOnly )
        {
            First = first;
            Second = second;
            _isReadOnly = readOnly;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Returns read-only copy of the pair.
        /// </summary>
        /// <returns>Read-only copy of the pair.</returns>
        public Pair < T1, T2 > AsReadOnly ( )
        {
            return new Pair < T1, T2 >
                (
                First,
                Second,
                true );
        }

        #endregion

        #region IList members

        ///<summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"/>.
        ///</summary>
        ///<returns>
        /// The position into which the new element was inserted.
        ///</returns>
        ///<param name="value">The <see cref="T:System.Object"/>
        /// to add to the <see cref="T:System.Collections.IList"/>.
        /// </param>
        ///<exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </exception><filterpriority>2</filterpriority>
        int IList.Add ( object value )
        {
            throw new NotSupportedException ();
        }

        ///<summary>
        /// Determines whether the <see cref="T:System.Collections.IList"/>
        /// contains a specific value.
        ///</summary>
        ///<returns>
        /// <c>true</c> if the <see cref="T:System.Object"/>
        /// is found in the <see cref="T:System.Collections.IList"/>;
        /// otherwise, <c>false</c>.
        ///</returns>
        ///<param name="value">The <see cref="T:System.Object"/>
        /// to locate in the <see cref="T:System.Collections.IList"/>.
        /// </param>
        bool IList.Contains ( object value )
        {
            foreach ( object o in this )
            {
                if ( o == value )
                {
                    return true;
                }
            }
            return false;
        }

        ///<summary>
        /// Removes all items from the 
        /// <see cref="T:System.Collections.IList"/>.
        ///</summary>
        ///<exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </exception>
        void IList.Clear ( )
        {
            throw new NotSupportedException ();
        }

        ///<summary>
        /// Determines the index of a specific item in the 
        /// <see cref="T:System.Collections.IList"/>.
        ///</summary>
        ///<returns>
        /// The index of value if found in the list; otherwise, -1.
        ///</returns>
        ///<param name="value">The <see cref="T:System.Object"/>
        /// to locate in the <see cref="T:System.Collections.IList"/>.
        /// </param>
        int IList.IndexOf ( object value )
        {
            int index = 0;
            foreach ( object o in this )
            {
                if ( o == value )
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        ///<summary>
        /// Inserts an item to the <see cref="T:System.Collections.IList"/>
        /// at the specified index.
        ///</summary>
        ///<param name="value">The <see cref="T:System.Object"/>
        /// to insert into the <see cref="T:System.Collections.IList"/>.
        /// </param>
        ///<param name="index">The zero-based index at which 
        /// value should be inserted.</param>
        ///<exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </exception>
        void IList.Insert
            (
            int index,
            object value )
        {
            throw new NotSupportedException ();
        }

        ///<summary>
        /// Removes the first occurrence of a specific object from the 
        /// <see cref="T:System.Collections.IList"/>.
        ///</summary>
        ///<param name="value">The <see cref="T:System.Object"/>
        /// to remove from the <see cref="T:System.Collections.IList"/>.
        /// </param>
        ///<exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </exception><filterpriority>2</filterpriority>
        void IList.Remove ( object value )
        {
            throw new NotSupportedException ();
        }

        ///<summary>
        /// Removes the <see cref="T:System.Collections.IList"/>
        /// item at the specified index.
        ///</summary>
        ///<param name="index">The zero-based index of the 
        /// item to remove.</param>
        ///<exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IList"/> has a fixed size.
        /// </exception>
        void IList.RemoveAt ( int index )
        {
            throw new NotSupportedException ();
        }

        ///<summary>
        /// Gets or sets the element at the specified index.
        ///</summary>
        ///<returns>
        /// The element at the specified index.
        ///</returns>
        ///<param name="index">The zero-based index of 
        /// the element to get or set. </param>
        ///<exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not a valid index in the 
        /// <see cref="T:System.Collections.IList"/>.</exception>
        public object this [ int index ]
        {
            get
            {
                switch ( index )
                {
                    case 0:
                        return First;
                    case 1:
                        return Second;
                    default:
                        throw new ArgumentOutOfRangeException ( "index" );
                }
            }
            set
            {
                if ( _isReadOnly )
                {
                    throw new NotSupportedException ();
                }
                switch ( index )
                {
                    case 0:
                        First = (T1) value;
                        break;
                    case 1:
                        Second = (T2) value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException ( "index" );
                }
            }
        }

        private bool _isReadOnly;

        ///<summary>
        /// Gets a value indicating whether the 
        /// <see cref="T:System.Collections.IList"/> is read-only.
        ///</summary>
        ///<returns>
        /// <c>true</c> if the <see cref="T:System.Collections.IList"/>
        /// is read-only; otherwise, <c>false</c>.
        ///</returns>
        bool IList.IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
        }

        ///<summary>
        /// Gets a value indicating whether the 
        /// <see cref="T:System.Collections.IList" /> has a fixed size.
        ///</summary>
        ///<returns>
        /// <c>true</c> if the <see cref="T:System.Collections.IList" />
        /// has a fixed size; otherwise, <c>false</c>.
        ///</returns>
        ///<remarks><see cref="Pair{T1,T2}"/>
        /// yields <c>true</c>.
        /// </remarks>
        bool IList.IsFixedSize
        {
            get
            {
                return true;
            }
        }

        ///<summary>
        /// Copies the elements of the 
        /// <see cref="T:System.Collections.ICollection"/>
        /// to an <see cref="T:System.Array"/>, 
        /// starting at a particular <see cref="T:System.Array"/> index.
        ///</summary>
        ///<param name="array">The one-dimensional 
        /// <see cref="T:System.Array"/> that is the destination 
        /// of the elements copied from 
        /// <see cref="T:System.Collections.ICollection"/>. 
        /// The <see cref="T:System.Array"/>
        /// must have zero-based indexing.</param>
        ///<param name="index">The zero-based index in array 
        /// at which copying begins.</param>
        ///<exception cref="T:System.NotImplementedException">
        /// Method not implemented.</exception>
        /// <remarks>Method not implemented.</remarks>
        [NotImplemented]
        void ICollection.CopyTo
            (
            Array array,
            int index )
        {
            throw new NotImplementedException ();
        }

        ///<summary>
        /// Gets the number of elements contained in the 
        /// <see cref="T:System.Collections.ICollection"/>.
        ///</summary>
        ///<returns>
        /// The number of elements contained in the 
        /// <see cref="T:System.Collections.ICollection" />.
        ///</returns>
        ///<remarks><see cref="Pair{T1,T2}"/>
        /// yields <c>2</c>.
        /// </remarks>
        public int Count
        {
            [DebuggerStepThrough]
            get
            {
                return 2;
            }
        }

        ///<summary>
        /// Gets an object that can be used to synchronize 
        /// access to the <see cref="T:System.Collections.ICollection"/>.
        ///</summary>
        ///<returns>
        /// An object that can be used to synchronize access to the 
        /// <see cref="T:System.Collections.ICollection"/>.
        ///</returns>
        ///<remarks><see cref="Pair{T1,T2}"/>
        /// yields <c>this</c>.
        /// </remarks>
        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        ///<summary>
        /// Gets a value indicating whether access to the 
        /// <see cref="T:System.Collections.ICollection"/>
        /// is synchronized (thread safe).
        ///</summary>
        ///<returns>
        /// <c>true</c> if access to the 
        /// <see cref="T:System.Collections.ICollection"/>
        /// is synchronized (thread safe); otherwise, <c>false</c>.
        ///</returns>
        /// <remarks><see cref="Pair{T1,T2}"/>
        /// yields <c>false</c>.
        /// </remarks>
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        ///<summary>
        /// Returns an enumerator that iterates through a collection.
        ///</summary>
        ///<returns>
        /// An <see cref="T:System.Collections.IEnumerator"/>
        /// object that can be used to iterate through the collection.
        ///</returns>
        ///<remarks><see cref="Pair{T1,T2}"/>
        /// yields sequentially: <see cref="First"/>,
        /// <see cref="Second"/>.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator ( )
        {
            yield return First;
            yield return Second;
        }

        #endregion

        #region ICloneable members

        ///<summary>
        /// Creates a new object that is a copy of the current instance.
        ///</summary>
        ///<returns>
        /// A new object that is a copy of this instance.
        ///</returns>
        public object Clone ( )
        {
            return new Pair < T1, T2 >
                (
                First,
                Second,
                _isReadOnly );
        }

        #endregion

        #region Object members

        ///<summary>
        /// Returns a <see cref="T:System.String"/> that represents 
        /// the current <see cref="T:System.Object"/>.
        ///</summary>
        ///<returns>
        /// A <see cref="T:System.String"/> that represents 
        /// the current <see cref="T:System.Object"/>.
        ///</returns>
        public override string ToString ( )
        {
            return string.Format
                (
                 "{0};{1}",
                 First,
                 Second );
        }

        #endregion
    }
}
