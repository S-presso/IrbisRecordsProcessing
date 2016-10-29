/* DataProcessingEventArgs.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM.IO.Evented
{
    /// <summary>
    /// 
    /// </summary>
    public class DataProcessingEventArgs : EventArgs
    {
        #region Properties

        private bool _cancel;

        ///<summary>
        /// 
        ///</summary>
        public bool Cancel
        {
            [DebuggerStepThrough]
            get
            {
                return _cancel;
            }
            [DebuggerStepThrough]
            set
            {
                _cancel = value;
            }
        }

        private EventedStream _stream;

        ///<summary>
        /// 
        ///</summary>
        public EventedStream Stream
        {
            [DebuggerStepThrough]
            get
            {
                return _stream;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DataProcessingEventArgs"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public DataProcessingEventArgs ( EventedStream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            _stream = stream;
        }

        #endregion
    }
}
