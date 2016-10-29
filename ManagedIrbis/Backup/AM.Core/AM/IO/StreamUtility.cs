/* StreamUtility.cs -- stream manipulation routines.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

#endregion

namespace AM.IO
{
    /// <summary>
    /// Stream manipulation routines.
    /// </summary>
    [Done]
    public static class StreamUtility
    {
        #region Private members

        private static byte[] _Read
            (
            Stream stream,
            int length )
        {
            var buffer = new byte[length];
            if ( stream.Read
                     (
                      buffer,
                      0,
                      length ) != length )
            {
                throw new IOException ();
            }
            return buffer;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Appends one's stream contents (starting from current position)
        /// to another stream.
        /// </summary>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="destinationStream">The destination stream.</param>
        /// <param name="chunkSize">Size of the chunk. 
        /// If <paramref name="chunkSize"/> is less that 0, it will
        /// be choosen by routine.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sourceStream"/> or 
        /// <paramref name="destinationStream"/> is <c>null</c>.
        /// </exception>
        public static void Append
            (
            this Stream sourceStream,
            Stream destinationStream,
            int chunkSize )
        {
            ArgumentUtility.NotNull
                (
                 sourceStream,
                 "sourceStream" );
            ArgumentUtility.NotNull
                (
                 destinationStream,
                 "destinationStream" );

            if ( chunkSize <= 0 )
            {
                chunkSize = 4*1024;
            }

            var buffer = new byte[chunkSize];
            destinationStream.Seek
                (
                 0,
                 SeekOrigin.End );
            while ( true )
            {
                var readed = sourceStream.Read
                    (
                     buffer,
                     0,
                     chunkSize );
                if ( readed <= 0 )
                {
                    break;
                }
                destinationStream.Write
                    (
                     buffer,
                     0,
                     readed );
            }
        }

        /// <summary>
        /// Compares two <see cref="Stream"/>'s from current position.
        /// </summary>
        /// <param name="firstStream">The first stream.</param>
        /// <param name="secondStream">The second stream.</param>
        /// <returns>0, if both streams are identical.</returns>
        public static int Compare
            (
            this Stream firstStream,
            Stream secondStream )
        {
            ArgumentUtility.NotNull
                (
                 firstStream,
                 "firstStream" );
            ArgumentUtility.NotNull
                (
                 secondStream,
                 "secondStream" );

            const int bufferSize = 1024;
            while ( true )
            {
                var firstBuffer = new byte[bufferSize];
                var firstReaded = firstStream.Read
                    (
                     firstBuffer,
                     0,
                     bufferSize );
                var secondBuffer = new byte[bufferSize];
                var secondReaded = secondStream.Read
                    (
                     secondBuffer,
                     0,
                     bufferSize );
                var difference = firstReaded - secondReaded;
                if ( difference != 0 )
                {
                    return difference;
                }
                if ( firstReaded == 0 )
                {
                    return 0;
                }
                for ( var i = 0; i < firstReaded; i++ )
                {
                    difference = firstBuffer [ i ] - secondBuffer [ i ];
                    if ( difference != 0 )
                    {
                        return difference;
                    }
                }
            }
        }

        /// <summary>
        /// Appends one's stream contents (starting from current position)
        /// to another stream.
        /// </summary>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="destinationStream">The destination stream.</param>
        public static void Copy
            (
            this Stream sourceStream,
            Stream destinationStream )
        {
            sourceStream.Append
                (
                 destinationStream,
                 0 );
        }

        /// <summary>
        /// Шестнадцатиричный дамп массива байт.
        /// </summary>
        /// <param name="writer">Куда писать.</param>
        /// <param name="buffer">Байты.</param>
        /// <param name="offset">Начальное смещение.</param>
        /// <param name="count">Количество байт для дампа.</param>
        public static void DumpBytes
            (
            TextWriter writer,
            byte[] buffer,
            int offset,
            int count )
        {
            for ( int i = 0; i < count; i++ )
            {
                if ( i != 0 )
                {
                    writer.Write ( " " );
                }
                writer.Write
                    (
                     "{0:X2}",
                     buffer [ offset + i ] );
            }
            writer.WriteLine ();
        }

        /// <summary>
        /// Read as up to <paramref name="maximum"/> bytes 
        /// from the given stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="maximum">Maximum bytes to read.</param>
        /// <returns>Readed data.</returns>
        /// <remarks>Don't make <paramref name="maximum"/>
        /// <c>Int32.Max</c> or so.
        /// </remarks>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="maximum"/> is less than zero.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        public static byte[] ReadAsMuchAsPossible
            (
            this Stream stream,
            int maximum )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.Positive
                (
                 maximum,
                 "maximum" );

            var result = new byte[maximum];
            var readed = stream.Read
                (
                 result,
                 0,
                 maximum );
            if ( readed <= 0 )
            {
                return new byte[0];
            }
            Array.Resize
                (
                 ref result,
                 readed );

            return result;
        }

        /// <summary>
        /// Reads <see cref="Boolean"/> value from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,bool)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="Write(Stream,bool)"/>
        public static bool ReadBoolean ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var value = stream.ReadByte ();
            switch ( value )
            {
                case 0:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Reads <see cref="Int16"/> value from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,short)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadUInt16"/>
        /// <seealso cref="Write(Stream,short)"/>
        public static short ReadInt16 ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            return BitConverter.ToInt16
                (
                 _Read
                     (
                      stream,
                      sizeof ( short ) ),
                 0 );
        }

        /// <summary>
        /// Reads <see cref="UInt16"/> value from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,ushort)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadInt16"/>
        /// <seealso cref="Write(Stream,ushort)"/>
        [CLSCompliant ( false )]
        public static ushort ReadUInt16 ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            return BitConverter.ToUInt16
                (
                 _Read
                     (
                      stream,
                      sizeof ( ushort ) ),
                 0 );
        }


        public static short ReadNetInt16 ( this Stream stream )
        {
            byte[] buffer = new byte[2];

            int readed = stream.Read
                (
                 buffer,
                 0,
                 2 );
            if ( readed != 2 )
            {
                throw new IOException ();
            }
            short result = BitConverter.ToInt16
                (
                 buffer,
                 0 );
            result = IPAddress.NetworkToHostOrder ( result );
            return result;
        }


        /// <summary>
        /// Reads <see cref="Int32"/> value from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,int)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadUInt32"/>
        /// <seealso cref="Write(Stream,int)"/>
        public static int ReadInt32 ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            return BitConverter.ToInt32
                (
                 _Read
                     (
                      stream,
                      sizeof ( int ) ),
                 0 );
        }

        public static int ReadNetInt32 ( this Stream stream )
        {
            byte[] buffer = new byte[4];

            int readed = stream.Read
                (
                 buffer,
                 0,
                 4 );
            if ( readed != 4 )
            {
                throw new IOException ();
            }
            int result = BitConverter.ToInt32
                (
                 buffer,
                 0 );
            result = IPAddress.NetworkToHostOrder ( result );
            return result;
        }

        /// <summary>
        /// Reads <see cref="UInt32"/> value from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,uint)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadInt32"/>
        /// <seealso cref="Write(Stream,uint)"/>
        [CLSCompliant ( false )]
        public static uint ReadUInt32 ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            return BitConverter.ToUInt32
                (
                 _Read
                     (
                      stream,
                      sizeof ( uint ) ),
                 0 );
        }

        /// <summary>
        /// Reads <see cref="Int64"/> value from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,long)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadUInt64"/>
        /// <seealso cref="Write(Stream,long)"/>
        public static long ReadInt64 ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            return BitConverter.ToInt64
                (
                 _Read
                     (
                      stream,
                      sizeof ( long ) ),
                 0 );
        }

        public static long ReadNetInt64 ( this Stream stream )
        {
            byte[] buffer = new byte[8];

            int readed = stream.Read
                (
                 buffer,
                 0,
                 8 );
            if ( readed != 8 )
            {
                throw new IOException ();
            }
            int lowPart = BitConverter.ToInt32
                (
                 buffer,
                 0 );
            lowPart = IPAddress.NetworkToHostOrder ( lowPart );
            int highPart = BitConverter.ToInt32
                (
                 buffer,
                 4 );
            highPart = IPAddress.NetworkToHostOrder ( highPart );
            long result = ( ( ( (long) highPart ) << 32 ) + lowPart );
            return result;
        }


        /// <summary>
        /// Reads <see cref="UInt64"/> value from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,ulong)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadInt64"/>
        /// <seealso cref="Write(Stream,ulong)"/>
        [CLSCompliant ( false )]
        public static ulong ReadUInt64 ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            return BitConverter.ToUInt64
                (
                 _Read
                     (
                      stream,
                      sizeof ( ulong ) ),
                 0 );
        }

        /// <summary>
        /// Reads <see cref="Single"/> value from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,float)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadDouble"/>
        /// <seealso cref="Write(Stream,float)"/>
        public static float ReadSingle ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            return BitConverter.ToSingle
                (
                 _Read
                     (
                      stream,
                      sizeof ( float ) ),
                 0 );
        }

        /// <summary>
        /// Reads <see cref="Double"/> value from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,double)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadSingle"/>
        /// <seealso cref="Write(Stream,double)"/>
        public static double ReadDouble ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            return BitConverter.ToDouble
                (
                 _Read
                     (
                      stream,
                      sizeof ( double ) ),
                 0 );
        }

        /// <summary>
        /// Reads <see cref="String"/> value from the 
        /// <see cref="Stream"/>
        /// using specified <see cref="Encoding"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="encoding">Encoding to use.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,string,Encoding)"/> 
        /// or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either
        /// <paramref name="stream"/> or <paramref name="encoding"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadString(Stream)"/>
        /// <seealso cref="Write(Stream,string,Encoding)"/>
        public static string ReadString
            (
            this Stream stream,
            Encoding encoding )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.NotNull
                (
                 encoding,
                 "encoding" );

            return encoding.GetString
                (
                 _Read
                     (
                      stream,
                      stream.ReadInt32 () ) );
        }

        /// <summary>
        /// Reads <see cref="Boolean"/> value from the 
        /// <see cref="Stream"/> using UTF-8 <see cref="Encoding"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,string)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadString(Stream,Encoding)"/>
        /// <seealso cref="Write(Stream,string)"/>
        public static string ReadString ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            return stream.ReadString ( Encoding.UTF8 );
        }

        /// <summary>
        /// Reads array of <see cref="Int16"/> values from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,IEnumerable{short})"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadUInt16Array"/>
        /// <seealso cref="Write(Stream,IEnumerable{short})"/>
        public static short[] ReadInt16Array ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var length = stream.ReadInt32 ();
            var result = new short[length];
            for ( var i = 0; i < length; i++ )
            {
                result [ i ] = stream.ReadInt16 ();
            }

            return result;
        }

        /// <summary>
        /// Reads array of <see cref="UInt16"/> values from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,IEnumerable{ushort})"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadInt16Array"/>
        /// <seealso cref="Write(Stream,IEnumerable{ushort})"/>
        [CLSCompliant ( false )]
        public static ushort[] ReadUInt16Array ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var length = stream.ReadInt32 ();
            var result = new ushort[length];
            for ( var i = 0; i < length; i++ )
            {
                result [ i ] = stream.ReadUInt16 ();
            }

            return result;
        }

        /// <summary>
        /// Reads array of <see cref="Int32"/> values from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,IEnumerable{int})"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadUInt32Array"/>
        /// <seealso cref="Write(Stream,IEnumerable{int})"/>
        public static int[] ReadInt32Array ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var length = stream.ReadInt32 ();
            var result = new int[length];
            for ( var i = 0; i < length; i++ )
            {
                result [ i ] = stream.ReadInt32 ();
            }

            return result;
        }

        /// <summary>
        /// Reads array of <see cref="UInt32"/> values from the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,IEnumerable{uint})"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadInt32Array"/>
        /// <seealso cref="Write(Stream,IEnumerable{uint})"/>
        [CLSCompliant ( false )]
        public static uint[] ReadUInt32Array ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var length = stream.ReadInt32 ();
            var result = new uint[length];
            for ( var i = 0; i < length; i++ )
            {
                result [ i ] = stream.ReadUInt32 ();
            }

            return result;
        }

        /// <summary>
        /// Reads array of <see cref="String"/>'s from 
        /// the given stream until the end
        /// of the stream using specified <see cref="Encoding"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="encoding">Encoding.</param>
        /// <returns>Readed strings.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,IEnumerable{string},Encoding)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">Either 
        /// <paramref name="stream"/> or 
        /// <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        /// <seealso cref="ReadStringArray(Stream)"/>
        /// <seealso cref="Write(Stream,IEnumerable{string})"/>
        public static string[] ReadStringArray
            (
            this Stream stream,
            Encoding encoding )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.NotNull
                (
                 encoding,
                 "encoding" );

            var length = ReadInt32 ( stream );
            var result = new string[length];
            for ( var i = 0; i < length; i++ )
            {
                result [ i ] = stream.ReadString ( encoding );
            }

            return result;
        }

        /// <summary>
        /// Считывает из потока максимально возможное число байт.
        /// </summary>
        /// <remarks>Полезно для считывания из сети (сервер высылает
        /// ответ, после чего закрывает соединение).</remarks>
        /// <param name="stream">Поток для чтения.</param>
        /// <returns>Массив считанных байт.</returns>
        public static byte[] ReadToEnd ( this Stream stream )
        {
            MemoryStream result = new MemoryStream ();

            while ( true )
            {
                byte[] buffer = new byte[10*1024];
                int read = stream.Read
                    (
                     buffer,
                     0,
                     buffer.Length );
                if ( read <= 0 )
                {
                    break;
                }
                result.Write
                    (
                     buffer,
                     0,
                     read );
            }

            return result.ToArray ();
        }

        /// <summary>
        /// Reads array of <see cref="String"/>'s from the 
        /// <see cref="Stream"/> using UTF-8 <see cref="Encoding"/>.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written by 
        /// <see cref="Write(Stream,IEnumerable{string})"/> 
        /// or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream 
        /// input happens.</exception>
        /// <seealso cref="ReadStringArray(Stream,Encoding)"/>
        /// <seealso cref="Write(Stream,IEnumerable{string})"/>
        public static string[] ReadStringArray ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            return stream.ReadStringArray ( Encoding.UTF8 );
        }

        /// <summary>
        /// Reads the <see cref="Decimal"/> from the specified 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Readed value.</returns>
        /// <remarks>Value must be written with 
        /// <see cref="Write(Stream,decimal)"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">Error during stream input
        /// happens.</exception>
        /// <seealso cref="Write(Stream,decimal)"/>
        public static decimal ReadDecimal ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var bits = new int[4];
            bits [ 0 ] = stream.ReadInt32 ();
            bits [ 1 ] = stream.ReadInt32 ();
            bits [ 2 ] = stream.ReadInt32 ();
            bits [ 3 ] = stream.ReadInt32 ();

            return new decimal ( bits );
        }

        /// <summary>
        /// Reads the date/time structure.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        /// <remarks>Value must be written with 
        /// <see cref="Write(Stream,DateTime)"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">Error during stream 
        /// input happens.</exception>
        /// <seealso cref="Write(Stream,DateTime)"/>
        public static DateTime ReadDateTime ( this Stream stream )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var binary = ReadInt64 ( stream );

            return DateTime.FromBinary ( binary );
        }

        /// <summary>
        /// Writes the <see cref="Boolean"/> value to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write.</param>
        /// <param name="value">Value to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadBoolean"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="ReadBoolean"/>
        public static void Write
            (
            this Stream stream,
            bool value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            stream.WriteByte
                (
                 value
                     ? (byte) 1
                     : (byte) 0 );
        }

        /// <summary>
        /// Writes the <see cref="Int16"/> value to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Value to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadInt16"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,ushort)"/>
        /// <see cref="ReadInt16"/>
        public static void Write
            (
            this Stream stream,
            short value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var bytes = BitConverter.GetBytes ( value );
            stream.Write
                (
                 bytes,
                 0,
                 bytes.Length );
        }

        /// <summary>
        /// Writes the <see cref="UInt16"/> value to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Value to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadUInt16"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,short)"/>
        /// <seealso cref="ReadUInt16"/>
        [CLSCompliant ( false )]
        public static void Write
            (
            this Stream stream,
            ushort value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var bytes = BitConverter.GetBytes ( value );
            stream.Write
                (
                 bytes,
                 0,
                 bytes.Length );
        }

        /// <summary>
        /// Writes the <see cref="Int32"/> to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Value to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadInt32"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,uint)"/>
        /// <seealso cref="ReadInt32"/>
        public static void Write
            (
            this Stream stream,
            int value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var bytes = BitConverter.GetBytes ( value );
            stream.Write
                (
                 bytes,
                 0,
                 bytes.Length );
        }

        /// <summary>
        /// Writes the <see cref="UInt32"/> to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Value to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadUInt32"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,int)"/>
        /// <seealso cref="ReadInt32"/>
        [CLSCompliant ( false )]
        public static void Write
            (
            this Stream stream,
            uint value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var bytes = BitConverter.GetBytes ( value );
            stream.Write
                (
                 bytes,
                 0,
                 bytes.Length );
        }

        /// <summary>
        /// Writes the <see cref="Int64"/> to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Value to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadInt64"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,ulong)"/>
        /// <seealso cref="ReadInt64"/>
        public static void Write
            (
            this Stream stream,
            long value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var bytes = BitConverter.GetBytes ( value );
            stream.Write
                (
                 bytes,
                 0,
                 bytes.Length );
        }

        /// <summary>
        /// Writes the <see cref="UInt64"/> to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Value to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadUInt64"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,long)"/>
        /// <seealso cref="ReadUInt64"/>
        [CLSCompliant ( false )]
        public static void Write
            (
            this Stream stream,
            ulong value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var bytes = BitConverter.GetBytes ( value );
            stream.Write
                (
                 bytes,
                 0,
                 bytes.Length );
        }

        /// <summary>
        /// Writes the <see cref="Single"/> to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Value to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadSingle"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,double)"/>
        /// <seealso cref="ReadSingle"/>
        public static void Write
            (
            this Stream stream,
            float value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var bytes = BitConverter.GetBytes ( value );
            stream.Write
                (
                 bytes,
                 0,
                 bytes.Length );
        }

        /// <summary>
        /// Writes the <see cref="Double"/> to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">Value to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadDouble"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,float)"/>
        /// <seealso cref="ReadDouble"/>
        public static void Write
            (
            this Stream stream,
            double value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var bytes = BitConverter.GetBytes ( value );
            stream.Write
                (
                 bytes,
                 0,
                 bytes.Length );
        }

        /// <summary>
        /// Writes the <see cref="String"/> to the 
        /// <see cref="Stream"/>
        /// using specified <see cref="Encoding"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">String to write.</param>
        /// <param name="encoding">Encoding to use.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadString(Stream,Encoding)"/> 
        /// or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either 
        /// <paramref name="stream"/> or <paramref name="value"/>
        /// or <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="IOException">An error during 
        /// stream output happens.</exception>
        /// <seealso cref="Write(Stream,string)"/>
        /// <see cref="ReadString(Stream)"/>
        public static void Write
            (
            this Stream stream,
            string value,
            Encoding encoding )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.NotNull
                (
                 value,
                 "value" );
            ArgumentUtility.NotNull
                (
                 encoding,
                 "encoding" );

            var bytes = encoding.GetBytes ( value );
            stream.Write ( bytes.Length );
            stream.Write
                (
                 bytes,
                 0,
                 bytes.Length );
        }

        /// <summary>
        /// Writes the <see cref="String"/> to the 
        /// <see cref="Stream"/>
        /// using UTF-8 <see cref="Encoding"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="value">String to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadString(Stream)"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either 
        /// <paramref name="stream"/> or <paramref name="value"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,string,Encoding)"/>
        /// <seealso cref="ReadString(Stream)"/>
        public static void Write
            (
            this Stream stream,
            string value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.NotNull
                (
                 value,
                 "value" );

            stream.Write
                (
                 value,
                 Encoding.UTF8 );
        }

        /// <summary>
        /// Writes the array of <see cref="Int16"/> to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="values">Array of signed short integer numbers.
        /// </param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadInt16Array"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either 
        /// <paramref name="stream"/> or <paramref name="values"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,IEnumerable{ushort})"/>
        /// <seealso cref="ReadInt16Array"/>
        // ReSharper disable PossibleMultipleEnumeration
        public static void Write
            (
            this Stream stream,
            IEnumerable < short > values )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.NotNull
                (
                 values,
                 "values" );

            var asArray = values.ToArray ();
            stream.Write ( asArray.Length );
            for ( var i = 0; i < asArray.Length; i++ )
            {
                stream.Write ( asArray [ i ] );
            }
        }

        // ReSharper restore PossibleMultipleEnumeration

        /// <summary>
        /// Writes the array of <see cref="UInt16"/> 
        /// to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="values">Array of usingned short integer 
        /// numbers.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadUInt16Array"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either 
        /// <paramref name="stream"/> or <paramref name="values"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,IEnumerable{short})"/>
        /// <seealso cref="ReadUInt16Array"/>
        // ReSharper disable PossibleMultipleEnumeration
        [CLSCompliant ( false )]
        public static void Write
            (
            this Stream stream,
            IEnumerable < ushort > values )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.NotNull
                (
                 values,
                 "values" );

            var asArray = values.ToArray ();
            stream.Write ( asArray.Length );
            for ( var i = 0; i < asArray.Length; i++ )
            {
                stream.Write ( asArray [ i ] );
            }
        }

        // ReSharper restore PossibleMultipleEnumeration

        /// <summary>
        /// Writes the array of <see cref="Int32"/> 
        /// to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="values">Array of signed integer numbers.
        /// </param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadInt32Array"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either 
        /// <paramref name="stream"/> or <paramref name="values"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <see cref="Write(Stream,IEnumerable{uint})"/>
        /// <see cref="ReadInt32Array"/>
        // ReSharper disable PossibleMultipleEnumeration
        public static void Write
            (
            this Stream stream,
            IEnumerable < int > values )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.NotNull
                (
                 values,
                 "values" );

            var asArray = values.ToArray ();
            stream.Write ( asArray.Length );
            for ( int i = 0; i < asArray.Length; i++ )
            {
                stream.Write ( asArray [ i ] );
            }
        }

        // ReSharper restore PossibleMultipleEnumeration

        /// <summary>
        /// Writes the array of <see cref="UInt32"/> to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="values">Array of unsigned integer numbers.
        /// </param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadUInt32Array"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either 
        /// <paramref name="stream"/> or <paramref name="values"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,IEnumerable{int})"/>
        /// <see cref="ReadUInt32Array"/>
        // ReSharper disable PossibleMultipleEnumeration
        [CLSCompliant ( false )]
        public static void Write
            (
            this Stream stream,
            IEnumerable < uint > values )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.NotNull
                (
                 values,
                 "values" );

            var asArray = values.ToArray ();
            stream.Write ( asArray.Length );
            for ( var i = 0; i < asArray.Length; i++ )
            {
                stream.Write ( asArray [ i ] );
            }
        }

        // ReSharper restore PossibleMultipleEnumeration

        /// <summary>
        /// Writes the array of <see cref="String"/> to the 
        /// <see cref="Stream"/>
        /// using specified <see cref="Encoding"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="lines">Strings to write.</param>
        /// <param name="encoding">Encoding to use.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadStringArray(Stream,Encoding)"/> 
        /// or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either 
        /// <paramref name="stream"/> or <paramref name="lines"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,IEnumerable{string})"/>
        /// <see cref="ReadStringArray(Stream,Encoding)"/>
        // ReSharper disable PossibleMultipleEnumeration
        public static void Write
            (
            this Stream stream,
            IEnumerable < string > lines,
            Encoding encoding )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.NotNull
                (
                 lines,
                 "lines" );
            ArgumentUtility.NotNull
                (
                 encoding,
                 "encoding" );

            var asArray = lines.ToArray ();
            stream.Write ( asArray.Length );
            for ( var i = 0; i < asArray.Length; i++ )
            {
                stream.Write
                    (
                     asArray [ i ],
                     encoding );
            }
        }

        // ReSharper restore PossibleMultipleEnumeration

        /// <summary>
        /// Writes the array of <see cref="String"/> to the 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="lines">Strings to write.</param>
        /// <remarks>Value can be readed with 
        /// <see cref="ReadStringArray(Stream)"/> or compatible method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Either 
        /// <paramref name="stream"/> or <paramref name="lines"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="IOException">An error during stream
        /// output happens.</exception>
        /// <seealso cref="Write(Stream,IEnumerable{string},Encoding)"/>
        /// <seealso cref="ReadStringArray(Stream)"/>
        // ReSharper disable PossibleMultipleEnumeration
        public static void Write
            (
            this Stream stream,
            IEnumerable < string > lines )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );
            ArgumentUtility.NotNull
                (
                 lines,
                 "lines" );

            stream.Write
                (
                 lines,
                 Encoding.UTF8 );
        }

        // ReSharper restore PossibleMultipleEnumeration

        /// <summary>
        /// Writes the <see cref="Decimal"/> to the specified 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        /// <remarks>Value can be readed with <see cref="ReadDecimal"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">Error during stream output
        /// happens.</exception>
        /// <seealso cref="ReadDecimal"/>
        public static void Write
            (
            this Stream stream,
            decimal value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            var bits = decimal.GetBits ( value );
            stream.Write ( bits [ 0 ] );
            stream.Write ( bits [ 1 ] );
            stream.Write ( bits [ 2 ] );
            stream.Write ( bits [ 3 ] );
        }

        /// <summary>
        /// Writes the <see cref="DateTime"/> to the specified 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        /// <remarks>Value can be readed with <see cref="ReadDateTime"/>
        /// or compatible method.</remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">Error during stream input
        /// happens.</exception>
        /// <seealso cref="ReadDateTime"/>
        public static void Write
            (
            this Stream stream,
            DateTime value )
        {
            ArgumentUtility.NotNull
                (
                 stream,
                 "stream" );

            stream.Write ( value.ToBinary () );
        }

        #endregion
    }
}
