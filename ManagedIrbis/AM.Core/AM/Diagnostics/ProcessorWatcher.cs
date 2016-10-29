/* ProcessorWatcher.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessorWatcher : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the percent busy.
        /// </summary>
        /// <value>The percent busy.</value>
        public float PercentBusy
        {
            [DebuggerStepThrough]
            get
            {
                return _counter.NextValue ();
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessorWatcher"/> class.
        /// </summary>
        public ProcessorWatcher ( )
        {
            _counter = new PerformanceCounter
                (
                "Processor",
                "% Processor Time",
                "_total",
                true );
        }

        #endregion

        #region Private members

        private readonly PerformanceCounter _counter;

        #endregion

        #region Public methods

        #endregion

        #region IDisposable members

        ///<summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        ///</summary>
        public void Dispose ( )
        {
            if ( _counter != null )
            {
                _counter.Dispose ();
            }
        }

        #endregion
    }
}
