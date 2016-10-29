/* CrmDateFilter.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace CrmCommon.Filters
{
    [Serializable]
    public sealed class CrmDateFilter
        : CrmFilter
    {
        #region Properties

        public DateTime FirstDate { get; set; }

        public DateTime LastDate { get; set; }

        #endregion

        #region CrmFilter members

        protected override IEnumerable < CrmEvent > DoFilter 
            ( 
                IEnumerable < CrmEvent > events 
            )
        {
            return events.Where 
                (
                    e => (e.Moment >= FirstDate) && (e.Moment <= LastDate)
                );
        }

        #endregion
    }
}
