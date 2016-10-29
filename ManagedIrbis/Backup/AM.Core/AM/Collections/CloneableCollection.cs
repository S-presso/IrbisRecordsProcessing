/* CloneableCollection.cs -- cloneable collection.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Cloneable collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Done]
    [Serializable]
    [DebuggerDisplay ( "Count={Count}" )]
    public class CloneableCollection < T >
        : Collection < T >,
          ICloneable
    {
        #region ICloneable members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone ( )
        {
            CloneableCollection < T > result = new CloneableCollection < T > ();
            Type type = typeof ( T );

            if ( !type.IsValueType )
            {
                if ( typeof ( T ).IsAssignableFrom ( typeof ( ICloneable ) ) )
                {
                    throw new ArgumentException ( type.Name );
                }
                foreach ( T item in Items )
                {
                    if ( item == null )
                    {
                        result.Add ( item );
                    }
                    else
                    {
                        T clone = (T) ( (ICloneable) item ).Clone ();
                        result.Add ( clone );
                    }
                }
            }
            else
            {
                foreach ( T item in Items )
                {
                    result.Add ( item );
                }
            }

            return result;
        }

        #endregion
    }
}
