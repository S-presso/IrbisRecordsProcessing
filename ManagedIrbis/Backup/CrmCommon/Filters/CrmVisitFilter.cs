/* CrmVisitFilter.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

#endregion

namespace CrmCommon.Filters
{
    public sealed class CrmVisitFilter
        : CrmFilter
    {
        #region Properties

        [DefaultValue(CrmEventKind.AllEvents)]
        public CrmEventKind EventKind { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        private bool _FilterRoutine
            (
                CrmEvent evt
            )
        {
            switch (EventKind)
            {
                case CrmEventKind.AllEvents:
                    return true;
                case CrmEventKind.VisitsOnly:
                    return evt.IsVisit;
                case CrmEventKind.LoansOnly:
                    return !evt.IsVisit;
            }

            return false;
        }

        #endregion

        #region CrmFilter members

        protected override IEnumerable<CrmEvent> DoFilter
            (
                IEnumerable<CrmEvent> events
            )
        {
            return events
                .Where ( _FilterRoutine );
        }

        #endregion
    }
}
