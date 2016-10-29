/* ResizeEventArgs.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Diagnostics;

#endregion

namespace AM.IO.Evented
{
    /// <summary>
    /// 
    /// </summary>
    public class ResizeEventArgs : DataProcessingEventArgs
    {
        #region Properties

        private long _length;

        ///<summary>
        /// 
        ///</summary>
        public long Length
        {
            [DebuggerStepThrough]
            get
            {
                return _length;
            }
            [DebuggerStepThrough]
            set
            {
                _length = value;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ResizeEventArgs"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="length">The length.</param>
        public ResizeEventArgs
            (
            EventedStream stream,
            long length )
            : base ( stream )
        {
            _length = length;
        }

        #endregion
    }
}
