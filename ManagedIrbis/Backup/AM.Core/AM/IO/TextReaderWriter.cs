/* TextReaderWriter.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.IO;
using System.Text;

#endregion

namespace AM.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class TextReaderWriter : IDisposable
    {
        #region Properties

        public static Encoding DefaultEncoding = Encoding.Default;

        public Stream Stream
        {
            get
            {
                return _stream;
            }
        }

        public TextReader Reader
        {
            get
            {
                return _reader;
            }
        }

        public TextWriter Writer
        {
            get
            {
                return _writer;
            }
        }

        #endregion

        #region Private members

        private readonly Stream _stream;

        private readonly TextReader _reader;

        private readonly TextWriter _writer;

        #endregion

        #region Construction

        public TextReaderWriter
            (
            Stream stream,
            Encoding encoding )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            _stream = stream;
            _reader = new StreamReader
                (
                stream,
                encoding );
            _writer = new StreamWriter
                (
                stream,
                encoding );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReaderWriter"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="writer">The writer.</param>
        public TextReaderWriter
            (
            TextReader reader,
            TextWriter writer )
        {
            ArgumentUtility.NotNull
                (
                 reader,
                 "reader" );
            ArgumentUtility.NotNull
                (
                 writer,
                 "writer" );

            _reader = reader;
            _writer = writer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReaderWriter"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="access">The access.</param>
        /// <param name="share">The share.</param>
        public TextReaderWriter
            (
            string fileName,
            Encoding encoding,
            FileMode mode,
            FileAccess access,
            FileShare share )
        {
            ArgumentUtility.NotNullOrEmpty
                (
                 fileName,
                 "fileName" );
            ArgumentUtility.NotNull
                (
                 encoding,
                 "encoding" );

            _stream = File.Open
                (
                 fileName,
                 mode,
                 access,
                 share );
            _reader = new StreamReader
                (
                _stream,
                encoding );
            _writer = new StreamWriter
                (
                _stream,
                encoding );
        }

        public TextReaderWriter ( )
        {
            _stream = new MemoryStream ();
            _reader = new StreamReader
                (
                _stream,
                DefaultEncoding );
            _writer = new StreamWriter
                (
                _stream,
                DefaultEncoding );
        }

        #endregion

        #region Public methods

        public void Close ( )
        {
            Dispose ();
        }

        public int Peek ( )
        {
            return Reader.Peek ();
        }

        public int Read ( )
        {
            return Reader.Read ();
        }

        public int Read
            (
            char[] buffer,
            int index,
            int count )
        {
            return Reader.Read
                (
                 buffer,
                 index,
                 count );
        }

        public int ReadBlock
            (
            char[] buffer,
            int index,
            int count )
        {
            return Reader.ReadBlock
                (
                 buffer,
                 index,
                 count );
        }

        public string ReadLine ( )
        {
            return Reader.ReadLine ();
        }

        public string ReadToEnd ( )
        {
            return Reader.ReadToEnd ();
        }

        public void Flush ( )
        {
            Writer.Flush ();
        }

        public void Write
            (
            string format,
            params object[] args )
        {
            string text = string.Format
                (
                 format,
                 args );
            Writer.Write ( text );
        }

        public void WriteLine ( )
        {
            Writer.WriteLine ();
        }

        #endregion

        #region IDisposable members

        public void Dispose ( )
        {
            if ( _writer != null )
            {
                _writer.Dispose ();
            }
            if ( _reader != null )
            {
                _reader.Dispose ();
            }
            if ( _stream != null )
            {
                _stream.Dispose ();
            }
        }

        #endregion
    }
}
