/* EventedStream.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.IO;

#endregion

namespace AM.IO.Evented
{
    /// <summary>
    /// 
    /// </summary>
    public class EventedStream : Stream
    {
        #region Events

        /// <summary>
        /// Fired before changing current position in the stream.
        /// </summary>
        public event SeekEventHandler ChangePosition;

        /// <summary>
        /// Fired before closing the stream.
        /// </summary>
        public event DataProcessingEventHandler CloseStream;

        /// <summary>
        /// Fired before flushing the stream.
        /// </summary>
        public event DataProcessingEventHandler FlushStream;

        /// <summary>
        /// Fired before reading data from the stream.
        /// </summary>
        public event DataTransferEventHandler ReadData;

        /// <summary>
        /// Fired before changing length of the stream.
        /// </summary>
        public event ResizeEventHandler Resize;

        /// <summary>
        /// Fired before writing data to the stream.
        /// </summary>
        public event DataTransferEventHandler WriteData;

        #endregion

        #region Properties

        private Stream _innerStream;

        /// <summary>
        /// Gets the inner stream.
        /// </summary>
        /// <value>The inner stream.</value>
        public virtual Stream InnerStream
        {
            get
            {
                return _innerStream;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:EventedStream"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public EventedStream ( Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            _innerStream = stream;
        }

        #endregion

        #region Private members

        /// <summary>
        /// Called when [change position].
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        protected virtual long OnChangePosition
            (
            long offset,
            SeekOrigin origin )
        {
            bool cancel = false;
            SeekEventHandler handler = ChangePosition;
            if ( handler != null )
            {
                SeekEventArgs ea = new SeekEventArgs
                    (
                    this,
                    offset,
                    origin );
                handler
                    (
                     this,
                     ea );
                cancel = ea.Cancel;
                offset = ea.Offset;
                origin = ea.Origin;
            }
            if ( cancel )
            {
                return offset;
            }
            else
            {
                return InnerStream.Seek
                    (
                     offset,
                     origin );
            }
        }

        /// <summary>
        /// Called when [close].
        /// </summary>
        protected virtual void OnClose ( )
        {
            bool cancel = false;
            DataProcessingEventHandler handler = CloseStream;
            if ( handler != null )
            {
                DataProcessingEventArgs ea = new DataProcessingEventArgs
                    ( this );
                handler
                    (
                     this,
                     ea );
            }
            if ( !cancel )
            {
                InnerStream.Close ();
            }
        }

        /// <summary>
        /// Called when [flush].
        /// </summary>
        protected virtual void OnFlush ( )
        {
            bool cancel = false;
            DataProcessingEventHandler handler = FlushStream;
            if ( handler != null )
            {
                DataProcessingEventArgs ea = new DataProcessingEventArgs
                    ( this );
                handler
                    (
                     this,
                     ea );
                cancel = ea.Cancel;
            }
            if ( !cancel )
            {
                InnerStream.Flush ();
            }
        }

        /// <summary>
        /// Called when [read data].
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        protected virtual int OnReadData
            (
            byte[] buffer,
            int offset,
            int count )
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
            bool cancel = false;
            DataTransferEventHandler handler = ReadData;
            if ( handler != null )
            {
                DataTransferEventArgs ea = new DataTransferEventArgs
                    (
                    this,
                    buffer,
                    offset,
                    count );
                handler
                    (
                     this,
                     ea );
                cancel = ea.Cancel;
                buffer = ea.Buffer;
                offset = ea.Offset;
                count = ea.Count;
                if ( cancel )
                {
                    return ea.Transferred;
                }
            }
            return InnerStream.Read
                (
                 buffer,
                 offset,
                 count );
        }

        /// <summary>
        /// Called when [resize].
        /// </summary>
        /// <param name="length">The length.</param>
        protected virtual void OnResize ( long length )
        {
            ArgumentUtility.Nonnegative
                (
                 length,
                 "length" );
            bool cancel = false;
            ResizeEventHandler handler = Resize;
            if ( handler != null )
            {
                ResizeEventArgs ea = new ResizeEventArgs
                    (
                    this,
                    length );
                handler
                    (
                     this,
                     ea );
                cancel = ea.Cancel;
                length = ea.Length;
            }
            if ( !cancel )
            {
                InnerStream.SetLength ( length );
            }
        }

        /// <summary>
        /// Called when [write data].
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        protected virtual void OnWriteData
            (
            byte[] buffer,
            int offset,
            int count )
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
            bool cancel = false;
            DataTransferEventHandler handler = WriteData;
            if ( handler != null )
            {
                DataTransferEventArgs ea = new DataTransferEventArgs
                    (
                    this,
                    buffer,
                    offset,
                    count );
                handler
                    (
                     this,
                     ea );
                cancel = ea.Cancel;
                buffer = ea.Buffer;
                offset = ea.Offset;
                count = ea.Count;
            }
            if ( !cancel )
            {
                InnerStream.Write
                    (
                     buffer,
                     offset,
                     count );
            }
        }

        #endregion

        #region Stream members

        /// <summary>
        /// When overridden in a derived class, gets a value 
        /// indicating whether the current stream supports reading.
        /// </summary>
        /// <value></value>
        /// <returns><c>true</c> if the stream supports reading; 
        /// otherwise, <c>false</c>.</returns>
        public override bool CanRead
        {
            get
            {
                return InnerStream.CanRead;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value 
        /// indicating whether the current stream supports seeking.
        /// </summary>
        /// <value></value>
        /// <returns><c>true</c> if the stream supports seeking; 
        /// otherwise, false.</returns>
        public override bool CanSeek
        {
            get
            {
                return InnerStream.CanSeek;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value 
        /// indicating whether the current stream supports writing.
        /// </summary>
        /// <value></value>
        /// <returns><c>true</c> if the stream supports 
        /// writing; otherwise, <c>false</c>.</returns>
        public override bool CanWrite
        {
            get
            {
                return InnerStream.CanWrite;
            }
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers 
        /// for this stream and causes any buffered data to be 
        /// written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs. </exception>
        public override void Flush ( )
        {
            OnFlush ();
        }

        /// <summary>
        /// When overridden in a derived class, gets the length 
        /// in bytes of the stream.
        /// </summary>
        /// <value></value>
        /// <returns>A long value representing the length 
        /// of the stream in bytes.</returns>
        /// <exception cref="T:System.NotSupportedException">
        /// A class derived from Stream does not support seeking.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed. </exception>
        public override long Length
        {
            get
            {
                return InnerStream.Length;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the 
        /// position within the current stream.
        /// </summary>
        /// <value></value>
        /// <returns>The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.
        /// </exception>
        public override long Position
        {
            get
            {
                return InnerStream.Position;
            }
            set
            {
                OnChangePosition
                    (
                     value,
                     SeekOrigin.Begin );
            }
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence 
        /// of bytes from the current stream and advances the position 
        /// within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. 
        /// When this method returns, the buffer contains the specified 
        /// byte array with the values between offset and 
        /// (offset + count - 1) replaced by the bytes read from 
        /// the current source.</param>
        /// <param name="offset">The zero-based byte offset in 
        /// buffer at which to begin storing the data read from the 
        /// current stream.</param>
        /// <param name="count">The maximum number of bytes 
        /// to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be 
        /// less than the number of bytes requested if that many bytes 
        /// are not currently available, or zero (0) if the end of the stream 
        /// has been reached.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of offset and count is larger than the buffer length.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed. </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support reading. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// buffer is null. </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// offset or count is negative. </exception>
        public override int Read
            (
            byte[] buffer,
            int offset,
            int count )
        {
            return OnReadData
                (
                 buffer,
                 offset,
                 count );
        }

        /// <summary>
        /// When overridden in a derived class, sets the position 
        /// within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the 
        /// origin parameter.</param>
        /// <param name="origin">A value of type 
        /// <see cref="T:System.IO.SeekOrigin"></see> 
        /// indicating the reference point used to obtain 
        /// the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support seeking, such as if the stream 
        /// is constructed from a pipe or console output. </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed.</exception>
        public override long Seek
            (
            long offset,
            SeekOrigin origin )
        {
            return OnChangePosition
                (
                 offset,
                 origin );
        }

        /// <summary>
        /// When overridden in a derived class, sets the length
        /// of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current 
        /// stream in bytes.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support both writing and seeking, 
        /// such as if the stream is constructed from a pipe or console output.
        /// </exception>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs. </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed. </exception>
        public override void SetLength ( long value )
        {
            OnResize ( value );
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of 
        /// bytes to the current stream and advances the current position 
        /// within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method 
        /// copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer 
        /// at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written 
        /// to the current stream.</param>
        /// <exception cref="T:System.IO.IOException">
        /// An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The stream does not support writing. </exception>
        /// <exception cref="T:System.ObjectDisposedException">
        /// Methods were called after the stream was closed. </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// buffer is null. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The sum of offset and count is greater than the buffer length. 
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// offset or count is negative. </exception>
        public override void Write
            (
            byte[] buffer,
            int offset,
            int count )
        {
            OnWriteData
                (
                 buffer,
                 offset,
                 count );
        }

        #endregion
    }
}
