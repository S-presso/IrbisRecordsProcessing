/* EnumerableUtility.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Collections;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumerableUtility
    {
        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Equalses the specified firstCollection.
        /// </summary>
        /// <param name="firstCollection">The firstCollection.</param>
        /// <param name="secondCollection">The secondCollection.</param>
        /// <returns></returns>
        public static bool Equals
            (
            IEnumerable firstCollection,
            IEnumerable secondCollection )
        {
            ArgumentUtility.NotNull
                (
                 firstCollection,
                 "firstCollection" );
            ArgumentUtility.NotNull
                (
                 secondCollection,
                 "secondCollection" );

            IEnumerator firstEnumerator = firstCollection.GetEnumerator ();
            IEnumerator secondEnumerator = secondCollection.GetEnumerator ();

            while ( true )
            {
                bool firstNext = firstEnumerator.MoveNext ();
                bool secondNext = secondEnumerator.MoveNext ();

                if ( firstNext != secondNext )
                {
                    return false;
                }
                if ( firstNext == false )
                {
                    return true;
                }

                object firstItem = firstEnumerator.Current;
                object secondItem = secondEnumerator.Current;

                if ( firstItem != secondItem )
                {
                    if ( ( firstItem == null )
                         || !firstItem.Equals ( secondItem ) )
                    {
                        return false;
                    }
                }
            }
        }

        #endregion
    }
}
