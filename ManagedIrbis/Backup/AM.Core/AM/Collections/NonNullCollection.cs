/* NonNullCollection.cs -- collection with items that can't be null.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// <see cref="Collection{T}"/> with items that can't be <c>null</c>.
    /// </summary>
    public class NonNullCollection < T > : Collection < T >
        where T : class
    {
        #region Collection<T> members

        /// <summary>
        /// Inserts an element into the 
        /// <see cref="Collection{T}"/>
        /// at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item 
        /// should be inserted.</param>
        /// <param name="item">The object to insert. The value can 
        /// be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-index is greater than 
        /// <see cref="Collection{T}.Count"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item"/> is <c>null</c>.
        /// </exception>
        protected override void InsertItem
            (
            int index,
            T item )
        {
            ArgumentUtility.NotNull
                (
                 item,
                 "item" );
            base.InsertItem
                (
                 index,
                 item );
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the 
        /// element to replace.</param>
        /// <param name="item">The new value for the element 
        /// at the specified index. The value can be null for reference types.
        /// </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.-or-index is greater 
        /// than <see cref="Collection{T}.Count"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item"/> is <c>null</c>.
        /// </exception>
        protected override void SetItem
            (
            int index,
            T item )
        {
            ArgumentUtility.NotNull
                (
                 item,
                 "item" );
            base.SetItem
                (
                 index,
                 item );
        }

        #endregion

        #region Public members

        /// <summary>
        /// Converts the collection to <see cref="Array"/> of elements
        /// of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>Array of items of type <typeparamref name="T"/>.
        /// </returns>
        public T[] ToArray ( )
        {
            List < T > result = new List < T > ( this );
            return result.ToArray ();
        }

        #endregion
    }
}
