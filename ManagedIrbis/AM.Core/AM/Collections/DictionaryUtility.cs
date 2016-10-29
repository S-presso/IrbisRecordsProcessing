/* DictionaryUtility.cs -- dictionary manipulation helpers
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// <see cref="Dictionary{Key,Value}" /> manipulation
    /// helper methods.
    /// </summary>
    public sealed class DictionaryUtility
    {
        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Merges the specified dictionaries.
        /// </summary>
        /// <param name="dictionaries">Dictionaries to merge.</param>
        /// <returns>Merged dictionary.</returns>
        /// <exception cref="ArgumentNullException">
        /// One or more dictionaries is <c>null</c>.
        /// </exception>
        public static Dictionary < TKey, TValue > Merge < TKey, TValue >
            ( params Dictionary < TKey, TValue >[] dictionaries )
        {
            for ( int i = 0; i < dictionaries.Length; i++ )
            {
                if ( dictionaries [ i ] == null )
                {
                    throw new ArgumentNullException ( "dictionaries" );
                }
            }
            Dictionary < TKey, TValue > result =
                new Dictionary < TKey, TValue > ();
            for ( int i = 0; i < dictionaries.Length; i++ )
            {
                Dictionary < TKey, TValue > dic = dictionaries [ i ];
                foreach ( KeyValuePair < TKey, TValue > pair in dic )
                {
                    result.Add
                        (
                         pair.Key,
                         pair.Value );
                }
            }
            return result;
        }

        #endregion
    }
}
