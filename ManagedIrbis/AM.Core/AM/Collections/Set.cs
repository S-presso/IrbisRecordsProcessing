/* Set.cs -- generic set. 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Generic set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [SuppressMessage ( "Microsoft.Naming",
        "CA1710:IdentifiersShouldHaveCorrectSuffix" )]
    public class Set < T >
        : ICollection < T >,
          IEnumerable < T >,
          ICollection,
          IEnumerable,
          ICloneable
    {
        #region Properties

        /// <summary>
        /// Count.
        /// </summary>
        /// <value></value>
        public int Count
        {
            [DebuggerStepThrough]
            get
            {
                return _data.Count;
            }
        }

        /// <summary>
        /// Is empty.
        /// </summary>
        /// <value></value>
        public bool IsEmpty
        {
            [DebuggerStepThrough]
            get
            {
                return ( Count == 0 );
            }
        }

        /// <summary>
        /// Get array of items.
        /// </summary>
        public T[] Items
        {
            [DebuggerStepThrough]
            get
            {
                return new List < T > ( _data.Keys ).ToArray ();
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Set ( )
        {
            _data = new Dictionary < T, object > ();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="capacity"></param>
        public Set ( int capacity )
        {
            _data = new Dictionary < T, object > ( capacity );
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="original"></param>
        public Set ( Set < T > original )
        {
            _data = new Dictionary < T, object > ( original._data );
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="original"></param>
        public Set ( IEnumerable < T > original )
        {
            _data = new Dictionary < T, object > ();
            AddRange ( original );
        }

        #endregion

        #region Private members

        private Dictionary < T, object > _data;

        #endregion

        #region Public methods

        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="item"></param>
        public void Add ( T item )
        {
            _data [ item ] = null;
        }

        /// <summary>
        /// Add some elements.
        /// </summary>
        /// <param name="many"></param>
        public void Add ( params T[] many )
        {
            for ( int i = 0; i < many.Length; i++ )
            {
                _data [ many [ i ] ] = null;
            }
        }

        /// <summary>
        /// Add some elements.
        /// </summary>
        /// <param name="range"></param>
        public void AddRange ( IEnumerable < T > range )
        {
            foreach ( T item in range )
            {
                Add ( item );
            }
        }

        /// <summary>
        /// Convert all.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        public Set < U > ConvertAll < U > ( Converter < T, U > converter )
        {
            Set < U > result = new Set < U > ( Count );
            foreach ( T element in this )
            {
                result.Add ( converter ( element ) );
            }
            return result;
        }

        /// <summary>
        /// True for all.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool TrueForAll ( Predicate < T > predicate )
        {
            foreach ( T element in this )
            {
                if ( !predicate ( element ) )
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Find all.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Set < T > FindAll ( Predicate < T > predicate )
        {
            Set < T > result = new Set < T > ();
            foreach ( T element in this )
            {
                if ( predicate ( element ) )
                {
                    result.Add ( element );
                }
            }
            return result;
        }

        /// <summary>
        /// For each.
        /// </summary>
        /// <param name="action"></param>
        public void ForEach ( Action < T > action )
        {
            foreach ( T element in this )
            {
                action ( element );
            }
        }

        /// <summary>
        /// Clear.
        /// </summary>
        public void Clear ( )
        {
            _data.Clear ();
        }

        /// <summary>
        /// Contains.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains ( T item )
        {
            return _data.ContainsKey ( item );
        }

        /// <summary>
        /// Copy to.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo
            (
            T[] array,
            int arrayIndex )
        {
            _data.Keys.CopyTo
                (
                 array,
                 arrayIndex );
        }

        /// <summary>
        /// Remove an element.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove ( T item )
        {
            return _data.Remove ( item );
        }

        /// <summary>
        /// Remove some elements.
        /// </summary>
        /// <param name="range"></param>
        public void Remove ( params T[] range )
        {
            for ( int i = 0; i < range.Length; i++ )
            {
                _data.Remove ( range [ i ] );
            }
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator < T > GetEnumerator ( )
        {
            return _data.Keys.GetEnumerator ();
        }

        /// <summary>
        /// Is read only.
        /// </summary>
        /// <value></value>
        public bool IsReadOnly
        {
            [DebuggerStepThrough]
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Union operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Set < T > operator | ( Set < T > left,
                                             Set < T > right )
        {
            Set < T > result = new Set < T > ( left );
            result.AddRange ( right );
            return result;
        }

        /// <summary>
        /// Union.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public Set < T > Union ( IEnumerable < T > set )
        {
            return ( this | new Set < T > ( set ) );
        }

        /// <summary>
        /// Intersection operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Set < T > operator & ( Set < T > left,
                                             Set < T > right )
        {
            Set < T > result = new Set < T > ();
            foreach ( T element in left )
            {
                if ( right.Contains ( element ) )
                {
                    result.Add ( element );
                }
            }
            return result;
        }

        /// <summary>
        /// Intersection.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public Set < T > Intersection ( IEnumerable < T > set )
        {
            return ( this & new Set < T > ( set ) );
        }

        /// <summary>
        /// Difference operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Set < T > operator - ( Set < T > left,
                                             Set < T > right )
        {
            Set < T > result = new Set < T > ();
            {
                foreach ( T element in left )
                {
                    if ( !right.Contains ( element ) )
                    {
                        result.Add ( element );
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Difference.
        /// </summary>
        /// <param name="setToCompare"></param>
        /// <returns></returns>
        public Set < T > Difference ( IEnumerable < T > setToCompare )
        {
            return ( this - new Set < T > ( setToCompare ) );
        }

        /// <summary>
        /// Symmetric difference.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Set < T > operator ^ ( Set < T > left,
                                             Set < T > right )
        {
            Set < T > result = new Set < T > ();
            {
                foreach ( T element in left )
                {
                    if ( !right.Contains ( element ) )
                    {
                        result.Add ( element );
                    }
                }
            }
            foreach ( T element in right )
            {
                if ( !left.Contains ( element ) )
                {
                    result.Add ( element );
                }
            }
            return result;
        }

        /// <summary>
        /// Symmetric difference.
        /// </summary>
        /// <param name="setToCompare"></param>
        /// <returns></returns>
        public Set < T > SymmetricDifference ( IEnumerable < T > setToCompare )
        {
            return ( this ^ new Set < T > ( setToCompare ) );
        }

        /// <summary>
        /// Empty.
        /// </summary>
        /// <value></value>
        public static Set < T > Empty
        {
            get
            {
                return new Set < T > ( 0 );
            }
        }

        /// <summary>
        /// Less or equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <= ( Set < T > left,
                                         Set < T > right )
        {
            foreach ( T element in left )
            {
                if ( !right.Contains ( element ) )
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Less.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator < ( Set < T > left,
                                        Set < T > right )
        {
            return ( ( left.Count < right.Count ) && ( left <= right ) );
        }

        /// <summary>
        /// Equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator == ( Set < T > left,
                                         Set < T > right )
        {
            return ( ( left.Count == right.Count ) && ( left <= right ) );
        }

        /// <summary>
        /// More.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator > ( Set < T > left,
                                        Set < T > right )
        {
            return ( right < left );
        }

        /// <summary>
        /// More or equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >= ( Set < T > left,
                                         Set < T > right )
        {
            return ( right <= left );
        }

        /// <summary>
        /// Not equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator != ( Set < T > left,
                                         Set < T > right )
        {
            return !( left == right );
        }

        /// <summary>
        /// Equals.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals ( object obj )
        {
            Set < T > a = this;
            Set < T > b = obj as Set < T >;
            return ( ( b == null )
                         ? false
                         : ( a == b ) );
        }

        /// <summary>
        /// Get hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode ( )
        {
            int hashcode = 0;
            foreach ( T element in this )
            {
                hashcode ^= element.GetHashCode ();
            }
            return hashcode;
        }

        /// <summary>
        /// Copy to.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        void ICollection.CopyTo
            (
            Array array,
            int index )
        {
            ( (ICollection) _data.Keys ).CopyTo
                (
                 array,
                 index );
        }

        /// <summary>
        /// Sync root.
        /// </summary>
        /// <value></value>
        object ICollection.SyncRoot
        {
            get
            {
                return ( ( (ICollection) _data.Keys ).SyncRoot );
            }
        }

        /// <summary>
        /// Is synchronized.
        /// </summary>
        /// <value></value>
        bool ICollection.IsSynchronized
        {
            get
            {
                return ( ( (ICollection) _data.Keys ).IsSynchronized );
            }
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator ( )
        {
            return ( ( (IEnumerable) _data.Keys ).GetEnumerator () );
        }

        #endregion

        #region ICloneable members

        /// <summary>
        /// Get clone of the set.
        /// </summary>
        /// <returns></returns>
        public object Clone ( )
        {
            return new Set < T > ( this );
        }

        #endregion
    }
}
