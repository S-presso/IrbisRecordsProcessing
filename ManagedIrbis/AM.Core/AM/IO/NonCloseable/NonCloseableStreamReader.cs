/* NonCloseableStreamReader.cs -- non-closeable stream reader
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.IO;
using System.Text;

#endregion

namespace AM.IO.NonCloseable
{
    /// <summary>
    /// Non-closeable stream reader.
    /// </summary>
    [Done]
    public class NonCloseableStreamReader
        : StreamReader,
          IDisposable
    {
        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public NonCloseableStreamReader ( Stream stream )
            : base ( stream )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path.</param>
        public NonCloseableStreamReader ( string path )
            : base ( path )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="detectEncodingFromByteOrderMarks">
        /// if set to <c>true</c> [detect encoding from byte order marks].</param>
        public NonCloseableStreamReader
            (
            Stream stream,
            bool detectEncodingFromByteOrderMarks )
            : base ( stream,
                     detectEncodingFromByteOrderMarks )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        public NonCloseableStreamReader
            (
            Stream stream,
            Encoding encoding )
            : base ( stream,
                     encoding )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public NonCloseableStreamReader
            (
            string path,
            bool detectEncodingFromByteOrderMarks )
            : base ( path,
                     detectEncodingFromByteOrderMarks )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        public NonCloseableStreamReader
            (
            string path,
            Encoding encoding )
            : base ( path,
                     encoding )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public NonCloseableStreamReader
            (
            Stream stream,
            Encoding encoding,
            bool detectEncodingFromByteOrderMarks )
            : base ( stream,
                     encoding,
                     detectEncodingFromByteOrderMarks )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public NonCloseableStreamReader
            (
            string path,
            Encoding encoding,
            bool detectEncodingFromByteOrderMarks )
            : base ( path,
                     encoding,
                     detectEncodingFromByteOrderMarks )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public NonCloseableStreamReader ( StreamReader reader )
            : base ( reader.BaseStream,
                     reader.CurrentEncoding )
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Really close the reader.
        /// </summary>
        public virtual void ReallyClose ( )
        {
            base.Close ();
        }

        #endregion

        #region StreamReader members

        /// <summary>
        /// NOT closes the <see cref="T:System.IO.StreamReader"></see> 
        /// object and the underlying stream, and releases any system resources 
        /// associated with the reader.
        /// </summary>
        public override void Close ( )
        {
            // Nothing to do actually
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Releases all resources used by the 
        /// <see cref="T:System.IO.TextReader"/> object.
        /// </summary>
        void IDisposable.Dispose ( )
        {
            // Nothing to do actually
        }

        #endregion
    }
}
