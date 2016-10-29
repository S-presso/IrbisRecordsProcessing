/* CharacterBuffer.cs -- fast and simple StringBuilder replacement.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

namespace AM.Text
{
    /// <summary>
    /// Character buffer -- fast and simple <see cref="StringBuilder"/>
    /// replacement.
    /// </summary>
    [Serializable]
    public sealed class CharacterBuffer
    {
        #region Constants

        /// <summary>
        /// Minimal capacity.
        /// </summary>
        public const int MinimalCapacity = 32;

        /// <summary>
        /// Default capacity.
        /// </summary>
        public const int DefaultCapacity = 256;

        #endregion

        #region Properties

        private char[] _buffer;

        /// <summary>
        /// Buffer.
        /// </summary>
        public char[] Buffer
        {
            get
            {
                return _buffer;
            }
            set
            {
                _SetBuffer ( value );
            }
        }

        private int _capacity;

        /// <summary>
        /// Capacity.
        /// </summary>
        public int Capacity
        {
            get
            {
                return _capacity;
            }
            set
            {
                _SetCapacity ( value );
            }
        }

        private int _length;

        /// <summary>
        /// Current length.
        /// </summary>
        public int Length
        {
            get
            {
                return _length;
            }
            set
            {
                _SetLength ( value );
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="capacity"></param>
        public CharacterBuffer ( int capacity )
        {
            _SetCapacity ( capacity );
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public CharacterBuffer ( )
            : this ( DefaultCapacity )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ch"></param>
        public CharacterBuffer ( char ch )
        {
            _SetCapacity ( DefaultCapacity );
            Write ( ch );
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="characters"></param>
        public CharacterBuffer ( char[] characters )
        {
            if ( characters.Length == 0 )
            {
                _SetCapacity ( DefaultCapacity );
            }
            else
            {
                _SetBuffer ( characters );
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text"></param>
        public CharacterBuffer ( string text )
        {
            if ( string.IsNullOrEmpty ( text ) )
            {
                _SetCapacity ( DefaultCapacity );
            }
            else
            {
                _SetBuffer ( text.ToCharArray () );
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader"></param>
        public CharacterBuffer ( TextReader reader )
            : this ()
        {
            const int bufLen = 1024;
            char[] buf = new char[bufLen];
            while ( true )
            {
                int readed = reader.Read
                    (
                     buf,
                     0,
                     bufLen );
                _Write
                    (
                     buf,
                     readed );
                if ( readed == 0 )
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        public CharacterBuffer
            (
            Stream stream,
            Encoding encoding )
            : this ( new StreamReader
                         (
                         stream,
                         encoding ) )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream"></param>
        public CharacterBuffer ( Stream stream )
            : this ( new StreamReader ( stream ) )
        {
        }

        #endregion

        #region Private members

        private void _SetCapacity ( int capacity )
        {
            if ( capacity <= 0 )
            {
                capacity = DefaultCapacity;
            }
            capacity = Math.Max
                (
                 capacity,
                 MinimalCapacity );
            char[] newBuffer = new char[capacity];
            if ( _buffer != null )
            {
                Array.Copy
                    (
                     _buffer,
                     newBuffer,
                     _length );
            }
            _buffer = newBuffer;
            _capacity = capacity;
        }

        private void _GrowTo ( int capacity )
        {
            if ( capacity > Capacity )
            {
                _SetCapacity ( ( capacity + 1 )*2 );
            }
        }

        private void _Grow ( int amount )
        {
            _GrowTo ( _length + amount );
        }

        private void _SetLength ( int newLength )
        {
            _GrowTo ( newLength );
            _length = newLength;
        }

        private void _SetBuffer ( char[] buffer )
        {
            if ( buffer == null )
            {
                throw new ArgumentNullException ();
            }
            _buffer = buffer;
            _length = _capacity = buffer.Length;
        }

        private void _Write
            (
            char[] characters,
            int length )
        {
            _Grow ( length );
            Array.Copy
                (
                 characters,
                 0,
                 _buffer,
                 _length,
                 length );
            _length += length;
        }

        private void _Write ( char[] characters )
        {
            _Write
                (
                 characters,
                 characters.Length );
        }

        private static CultureInfo _CultureInfo
        {
            get
            {
                return CultureInfo.InvariantCulture;
            }
        }

        private readonly char[] _crlf = Environment.NewLine.ToCharArray ();

        #endregion

        #region Public methods

        /// <summary>
        /// Clear.
        /// </summary>
        public void Clear ( )
        {
            _length = 0;
        }

        /// <summary>
        /// Remove characters.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="length"></param>
        public void Remove
            (
            int index,
            int length )
        {
            length = Math.Min
                (
                 length,
                 _length - index );
            if ( length > 0 )
            {
                Array.Copy
                    (
                     _buffer,
                     index + length,
                     _buffer,
                     index,
                     length );
                _length -= length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteLine ( )
        {
            _Write ( _crlf );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        public void Write ( CharacterBuffer buffer )
        {
            _Write
                (
                 buffer._buffer,
                 buffer._length );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        public void WriteLine ( CharacterBuffer buffer )
        {
            Write ( buffer );
            WriteLine ();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public void Write ( char character )
        {
            _Grow ( 1 );
            _buffer [ _length++ ] = character;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        public void WriteLine ( char character )
        {
            Write ( character );
            WriteLine ();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characters"></param>
        public void Write ( char[] characters )
        {
            if ( characters.Length != 0 )
            {
                _Write ( characters );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characters"></param>
        public void WriteLine ( char[] characters )
        {
            Write ( characters );
            WriteLine ();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void Write ( string text )
        {
            if ( !string.IsNullOrEmpty ( text ) )
            {
                _Write ( text.ToCharArray () );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine ( string text )
        {
            Write ( text );
            WriteLine ();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arguments"></param>
        public void Write
            (
            string format,
            params object[] arguments )
        {
            Write
                (
                 string.Format
                     (
                      _CultureInfo,
                      format,
                      arguments ) );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arguments"></param>
        public void WriteLine
            (
            string format,
            params object[] arguments )
        {
            Write
                (
                 string.Format
                     (
                      _CultureInfo,
                      format,
                      arguments ) );
            WriteLine ();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void Write ( object obj )
        {
            if ( obj != null )
            {
                Write ( obj.ToString () );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void WriteLine ( object obj )
        {
            Write ( obj );
            WriteLine ();
        }

        #endregion

        #region Object methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString ( )
        {
            return new string
                (
                _buffer,
                0,
                _length );
        }

        #endregion
    }
}
