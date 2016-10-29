/* ListUtility.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListUtility
    {
        #region Public methods

        /// <summary>
        /// Collects only unique elements from given list.
        /// </summary>
        /// <typeparam name="T">Type of list item.</typeparam>
        /// <param name="sourceList">List to process.</param>
        /// <returns>List containing only unique elements.</returns>
        public static List < T > Unique < T > ( List < T > sourceList )
        {
            ArgumentUtility.NotNull
                (
                 sourceList,
                 "sourceList" );

            Dictionary < T, object > dic = new Dictionary < T, object > ();
            foreach ( T t in sourceList )
            {
                dic [ t ] = null;
            }
            return new List < T > ( dic.Keys );
        }

        #endregion
    }
}
