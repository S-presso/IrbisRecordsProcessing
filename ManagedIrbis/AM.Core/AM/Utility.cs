/* Utility.cs -- bunch of useful routines.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace AM
{
    /// <summary>
    /// Bunch of useful routines.
    /// </summary>
    public static class Utility
    {
        #region Public methods

        /// <summary>
        /// Выборка элемента из массива.
        /// </summary>
        /// <remarks>
        /// <para>Значения <paramref name="occurence"/>, меньшие нуля,
        /// отсчитываются с конца массива.
        /// </para>
        /// </remarks>
        public static T GetOccurence < T >
            (
            this T[] array,
            int occurence )
        {
            occurence = ( occurence >= 0 )
                            ? occurence
                            : array.Length - occurence;

            T result = default( T );
            if ( ( occurence >= 0 )
                 && ( occurence < array.Length ) )
            {
                result = array [ occurence ];
            }

            return result;
        }

        /// <summary>
        /// Выборка элемента из списка.
        /// </summary>
        /// <remarks>
        /// <para>Значения <paramref name="occurence"/>, меньшие нуля,
        /// отсчитываются с конца списка.
        /// </para>
        /// </remarks>
        public static T GetOccurence < T >
            (
            this IList < T > list,
            int occurence )
        {
            occurence = ( occurence >= 0 )
                            ? occurence
                            : list.Count - occurence;

            T result = default( T );
            if ( ( occurence >= 0 )
                 && ( occurence < list.Count ) )
            {
                result = list [ occurence ];
            }

            return result;
        }


        public static bool IsOneOf < T >
            (
            this T value,
            IEnumerable < T > list ) where T : IComparable < T >
        {
            return list.Any ( one => value.CompareTo ( one ) == 0 );
        }


        /// <summary>
        /// Swaps two values.
        /// </summary>
        /// <typeparam name="T">Type of both values.</typeparam>
        /// <param name="first">First value.</param>
        /// <param name="second">Second value.</param>
        /// <returns>Value of first argument.</returns>
        public static T Swap < T >
            (
            ref T first,
            ref T second )
        {
            T temp = first;
            first = second;
            second = temp;
            return temp;
        }

        #endregion
    }
}
