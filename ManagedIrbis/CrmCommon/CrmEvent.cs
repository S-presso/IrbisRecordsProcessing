/* CrmEvent.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ManagedClient;

#endregion

namespace CrmCommon
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class CrmEvent
    {
        #region Properties

        public bool Index { get; set; }

        public bool IsVisit { get; set; }

        public DateTime Moment { get; set; }

        public string Name { get; set; }

        public string Ticket { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Public methods

        public static CrmEvent Parse 
            ( 
                IrbisRecord record 
            )
        {
            CrmEvent result = new CrmEvent ();

            return result;
        }

        #endregion

        #region Object members

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString ()
        {
            return Name;
        }

        #endregion
    }
}
