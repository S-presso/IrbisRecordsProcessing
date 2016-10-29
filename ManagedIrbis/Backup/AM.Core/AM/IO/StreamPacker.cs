/* StreamPacker.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.IO;
using System.Text;

#endregion

/*
 * Layout of packed UInt32 value
 *
 * bit number
 * -----------------------------------------------------------
 * | 31  30         |                                2  1  0 |
 * | length minus 1 |                                        |
 * -----------------------------------------------------------
 *
 * Layout of packed UInt64 value
 *
 * bit number
 * -----------------------------------------------------------
 * | 63  62  61     |                                2  1  0 |
 * | length minus 1 |                                        |
 * -----------------------------------------------------------
 *
 */

namespace AM.IO
{
    /// <summary>
    /// Упаковщик: пытается записать данные в поток, 
    /// используя по возможности меньше байт ( но до
    /// архиватора не дотягивает ).
    /// </summary>
    /// <remark>This class is not CLS-compliant.</remark>
    [CLSCompliant ( false )]
    public static class StreamPacker
    {
        #region Public methods

        /// <summary>
        /// Выводит в поток 4-байтовое целое.
        /// </summary>
        /// <param name="stream">Поток. Может равняться null.</param>
        /// <param name="val">Целое.</param>
        /// <returns>Количество байт, необходимых для вывода.</returns>
        public static int PackUInt32
            (
            Stream stream,
            uint val )
        {
            byte[] bytes = BitConverter.GetBytes ( val );
            //Array.Reverse ( bytes );
            byte c = bytes [ 3 ];
            bytes [ 3 ] = bytes [ 0 ];
            bytes [ 0 ] = c;
            c = bytes [ 2 ];
            bytes [ 2 ] = bytes [ 1 ];
            bytes [ 1 ] = c;
            int len;

            unchecked
            {
                if ( val <= 63 ) /* 0x3F */
                {
                    len = 1;
                }
                else if ( val <= 16383 ) /* 0x3FFF */
                {
                    len = 2;
                    bytes [ 2 ] |= 0x40;
                }
                else if ( val <= 4193303 ) /* 0x3FFFFF */
                {
                    len = 3;
                    bytes [ 1 ] |= 0x80;
                }
                else if ( val <= 1073741823 ) /* 0x3FFFFFFF */
                {
                    len = 4;
                    bytes [ 0 ] |= 0xC0;
                }
                else
                {
                    throw new ArgumentException
                        (
                        "too big",
                        "val" );
                }
            }
            if ( stream != null )
            {
                stream.Write
                    (
                     bytes,
                     4 - len,
                     len );
            }

            return len;
        }

        /// <summary>
        /// Считывает 4-байтовое целое из потока.
        /// </summary>
        /// <param name="stream">Поток.</param>
        /// <returns>Считанное значение.</returns>
        public static uint UnpackUInt32 ( Stream stream )
        {
            uint res = 0;
            int fb = stream.ReadByte ();

            if ( fb < 0 )
            {
                throw new IOException ( "end of stream" );
            }
            unchecked
            {
                res = (uint) ( fb & 0x3F );
                for ( int len = fb >> 6; len > 0; len-- )
                {
                    fb = stream.ReadByte ();
                    if ( fb < 0 )
                    {
                        throw new IOException ( "end of stream" );
                    }
                    res = (uint) ( ( res << 8 ) + fb );
                }
            }
            return res;
        }

        /// <summary>
        /// Выводит в поток 8-байтовое целое.
        /// </summary>
        /// <param name="stream">Поток. Может равняться null.</param>
        /// <param name="val">Целое.</param>
        /// <returns>Количество байт, необходимых для вывода.</returns>
        public static int PackUInt64
            (
            Stream stream,
            ulong val )
        {
            byte[] bytes = BitConverter.GetBytes ( val );
            Array.Reverse ( bytes );
            int len;

            unchecked
            {
                if ( val <= 0x1F )
                {
                    len = 1;
                }
                else if ( val <= 0x1FFF )
                {
                    len = 2;
                }
                else if ( val <= 0x1FFFFF )
                {
                    len = 3;
                }
                else if ( val <= 0x1FFFFFFFUL )
                {
                    len = 4;
                }
                else if ( val <= 0x1FFFFFFFFFUL )
                {
                    len = 5;
                }
                else if ( val <= 0x1FFFFFFFFFFFUL )
                {
                    len = 6;
                }
                else if ( val <= 0x1FFFFFFFFFFFFFUL )
                {
                    len = 7;
                }
                else if ( val <= 0x1FFFFFFFFFFFFFFFUL )
                {
                    len = 8;
                }
                else
                {
                    throw new ArgumentException
                        (
                        "too big",
                        "val" );
                }
                bytes [ 8 - len ] |= (byte) ( ( len - 1 ) << 5 );
            }
            if ( stream != null )
            {
                stream.Write
                    (
                     bytes,
                     8 - len,
                     len );
            }

            return len;
        }

        /// <summary>
        /// Считывает 8-байтовое целое из потока.
        /// </summary>
        /// <param name="stream">Поток.</param>
        /// <returns>Считанное целое.</returns>
        public static ulong UnpackUInt64 ( Stream stream )
        {
            ulong res = 0;
            int fb = stream.ReadByte ();

            if ( fb < 0 )
            {
                throw new IOException ( "end of stream" );
            }
            unchecked
            {
                res = (ulong) ( fb & 0x1F );
                for ( int len = fb >> 5; len > 0; len-- )
                {
                    fb = stream.ReadByte ();
                    if ( fb < 0 )
                    {
                        throw new IOException ( "end of stream" );
                    }
                    res = (ulong) ( ( res << 8 ) + (ulong) fb );
                }
            }
            return res;
        }

        /// <summary>
        /// Записывает массив байт в поток.
        /// </summary>
        /// <param name="stream">Поток.</param>
        /// <param name="bytes">Массив.</param>
        /// <returns>Количество байт, необходимых для вывода.</returns>
        public static uint PackBytes
            (
            Stream stream,
            byte[] bytes )
        {
            uint len = ( bytes == null )
                           ? 0
                           : unchecked (
                                 (uint) ( bytes.Length + PackUInt32
                                                             (
                                                              stream,
                                                              (uint)
                                                              bytes.Length ) ) );

            if ( ( stream != null )
                 && ( len != 0 ) )
            {
                stream.Write
                    (
                     bytes,
                     0,
                     bytes.Length );
            }

            return len;
        }

        /// <summary>
        /// Считывает массив байт из потока.
        /// </summary>
        /// <param name="stream">Поток.</param>
        /// <returns>Считанный массив.</returns>
        public static byte[] UnpackBytes ( Stream stream )
        {
            int len = unchecked ( (int) UnpackUInt32 ( stream ) );
            if ( len == 0 )
            {
                return null;
            }

            byte[] bytes = new byte[len];
            if ( stream.Read
                     (
                      bytes,
                      0,
                      len ) != len )
            {
                throw new IOException ( "end of stream" );
            }

            return bytes;
        }

        /// <summary>
        /// Записывает строку в поток в указанной кодировке.
        /// </summary>
        /// <param name="stream">Поток.</param>
        /// <param name="encoding">Кодировка.</param>
        /// <param name="value">Строка.</param>
        /// <returns>Количество байт, необходимых для вывода.</returns>
        public static uint PackString
            (
            Stream stream,
            Encoding encoding,
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
            if ( encoding == null )
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = encoding.GetBytes ( value );
            return PackBytes
                (
                 stream,
                 bytes );
        }

        /// <summary>
        /// Записывает строку в поток в UTF8.
        /// </summary>
        /// <param name="stream">Поток.</param>
        /// <param name="value">Строка.</param>
        /// <returns>Количество байт, необходимых для вывода.</returns>
        public static uint PackString
            (
            Stream stream,
            string value )
        {
            return PackString
                (
                 stream,
                 null,
                 value );
        }

        /// <summary>
        /// Считывает строку из потока в заданной кодировке.
        /// </summary>
        /// <param name="stream">Поток.</param>
        /// <param name="enc">Кодировка.</param>
        /// <returns>Считанная строка.</returns>
        public static string UnpackString
            (
            Stream stream,
            Encoding enc )
        {
            if ( enc == null )
            {
                enc = Encoding.UTF8;
            }
            byte[] bytes = UnpackBytes ( stream );
            return ( bytes == null )
                       ? null
                       : enc.GetString ( bytes );
        }

        /// <summary>
        /// Считывает строку из потока в UTF8.
        /// </summary>
        /// <param name="stream">Поток.</param>
        /// <returns>Считанная строка.</returns>
        public static string UnpackString ( Stream stream )
        {
            return UnpackString
                (
                 stream,
                 null );
        }

        #endregion
    }
}

#if NOTDEF

class Test
{
	static void Check ( uint first, uint count )
	{
		using ( Stream strm = new MemoryStream ( (int) ( count * 4 ) ) )
		{
			uint i, j, k;

			for ( i = 0, k = first; i < count; i++, k++ )
				StreamPacker.PackUInt32 ( strm, k );

			strm.Position = 0;

			for ( i = 0, k = first; i < count; i++, k++ )
			{
				j = StreamPacker.UnpackInt32 ( strm );
				if ( j != k )
					throw new Exception ( string.Format ( "failed on {0}: {1}", k, j ) ); 
			}
		}
	}

	static void Check ( ulong first, ulong count )
	{
		using ( Stream strm = new MemoryStream ( (int) ( count * 8 ) ) )
		{
			ulong i, j, k;

			for ( i = 0, k = first; i < count; i++, k++ )
				StreamPacker.PackUInt64 ( strm, k );

			strm.Position = 0;

			for ( i = 0, k = first; i < count; i++, k++ )
			{
				j = StreamPacker.UnpackInt64 ( strm );
				if ( j != k )
					throw new Exception ( string.Format ( "failed on {0}: {1}", k, j ) ); 
			}
		}
	}

	const uint block = 10000;
	const uint last = 1000000000;

	static void ShowOne ( ulong val )
	{
		using ( MemoryStream ms = new MemoryStream () )
		{
			StreamPacker.PackUInt64 ( ms, val );
			ms.Position = 0;
			byte[] bytes = ms.ToArray ();
			Console.Write ( "{0,7}: ", val );
			foreach ( byte b in bytes )
				Console.Write ( "{0,2:X} ", b );
//			Console.WriteLine ();
		}
	}

	static void ShowOne ( uint val )
	{
		using ( MemoryStream ms = new MemoryStream () )
		{
			StreamPacker.PackUInt32 ( ms, val );
			ms.Position = 0;
			byte[] bytes = ms.ToArray ();
			Console.Write ( "{0,7}: ", val );
			foreach ( byte b in bytes )
				Console.Write ( "{0,2:X} ", b );
//			Console.WriteLine ();
		}
	}

	static void Main2 ()
	{
		for ( uint i = 0; i < 100000; i++ )
		{
			ShowOne ( i );
			Console.Write ( "\t\t" );
			ShowOne ( (ulong)i );
			Console.WriteLine ();
		}
	}

	static void Main1 ()
	{
		for ( uint first = 0; first < last; first += block )
		{
			Console.Write ( "\r{0:0000000000} of {1:0000000000}", first, last );
			Check ( first, block );
		}
		Console.WriteLine ( "\nOK" );
	}

	static void Main ()
	{
		for ( uint first = 0; first < last; first += block )
		{
			Console.Write ( "\r{0:0000000000} of {1:0000000000}", first, last );
			Check ( (ulong)first, (ulong)block );
		}
		Console.WriteLine ( "\nOK" );
	}
}

#endif
