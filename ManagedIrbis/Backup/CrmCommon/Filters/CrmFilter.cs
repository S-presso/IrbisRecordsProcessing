/* CrmFilter.cs
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
    public abstract class CrmFilter
    {
        #region Properties

        public int Limit { get; set; }

        public string OrderBy { get; set; }

        public CrmOrderDirection OrderDirection { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        protected virtual Comparer < CrmEvent > GetComparer ()
        {
            return Comparer < CrmEvent >.Default;
        }

        protected abstract IEnumerable < CrmEvent > DoFilter
            ( IEnumerable < CrmEvent > events );

        protected virtual IEnumerable < CrmEvent > DoOrder
            ( IEnumerable < CrmEvent > events )
        {
            Comparer < CrmEvent > comparer = GetComparer ();
            return events.OrderBy ( e => e, comparer );
        }

        protected virtual IEnumerable < CrmEvent > DoLimit 
            (
                IEnumerable <CrmEvent>  events
            )
        {
            return (Limit <= 0)
                ? events
                : events.Take ( Limit );
        }

        #endregion

        #region Public methods

        public IEnumerable < CrmEvent > FilterEvents
            ( 
                IEnumerable < CrmEvent > events 
            )
        {
            if ( ReferenceEquals ( events, null ) )
            {
                throw new ArgumentNullException("events");
            }

            IEnumerable < CrmEvent > result = DoFilter ( events );
            result = DoOrder ( result );
            result = DoLimit ( result );

            return result;
        }

        public static Type[] GetKnownFilterTypes ()
        {
            return new []
                   {
                       typeof(CrmDateFilter),
                       typeof(CrmVisitFilter)
                   };
        }

        #endregion

        #region Object members

        #endregion
    }
}
