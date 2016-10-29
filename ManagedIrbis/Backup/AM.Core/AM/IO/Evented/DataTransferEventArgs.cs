/* DataTransferEventArgs.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Diagnostics;

#endregion

namespace AM.IO.Evented
{
    /// <summary>
    /// 
    /// </summary>
    public class DataTransferEventArgs : DataProcessingEventArgs
    {
        #region Properties

        private byte[] _buffer;

        ///<summary>
        /// 
        ///</summary>
        public byte[] Buffer
        {
            [DebuggerStepThrough]
            get
            {
                return _buffer;
            }
            [DebuggerStepThrough]
            set
            {
                _buffer = value;
            }
        }

        private int _count;

        ///<summary>
        /// 
        ///</summary>
        public int Count
        {
            [DebuggerStepThrough]
            get
            {
                return _count;
            }
            [DebuggerStepThrough]
            set
            {
                _count = value;
            }
        }

        private int _offset;

        ///<summary>
        /// 
        ///</summary>
        public int Offset
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

        private int _transferred;

        /// <summary>
        /// Gets or sets the transferred.
        /// </summary>
        /// <value>The transferred.</value>
        public int Transferred
        {
            [DebuggerStepThrough]
            get
            {
                return _transferred;
            }
            [DebuggerStepThrough]
            set
            {
                _transferred = value;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransferEventArgs"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public DataTransferEventArgs
            (
            EventedStream stream,
            byte[] buffer,
            int offset,
            int count )
            : base ( stream )
        {
            ArgumentUtility.NotNull
                (
                 buffer,
                 "buffer" );
            ArgumentUtility.Nonnegative
                (
                 offset,
                 "offset" );
            ArgumentUtility.Nonnegative
                (
                 count,
                 "count" );
            _buffer = buffer;
            _offset = offset;
            _count = count;
        }

        #endregion
    }
}
