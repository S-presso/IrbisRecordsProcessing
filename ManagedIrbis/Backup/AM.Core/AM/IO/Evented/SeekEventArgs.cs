/* SeekEventArgs.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Diagnostics;
using System.IO;

#endregion

namespace AM.IO.Evented
{
    /// <summary>
    /// 
    /// </summary>
    public class SeekEventArgs : DataProcessingEventArgs
    {
        #region Properties

        private long _offset;

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public long Offset
        {
            [DebuggerStepThrough]
            get
            {
                return _offset;
            }
            [DebuggerStepThrough]
            set
            {
                _offset = value;
            }
        }

        private SeekOrigin _origin;

        ///<summary>
        /// 
        ///</summary>
        public SeekOrigin Origin
        {
            [DebuggerStepThrough]
            get
            {
                return _origin;
            }
            [DebuggerStepThrough]
            set
            {
                _origin = value;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SeekEventArgs
            (
            EventedStream stream,
            long offset,
            SeekOrigin origin )
            : base ( stream )
        {
            _offset = offset;
            _origin = origin;
        }

        #endregion
    }
}
