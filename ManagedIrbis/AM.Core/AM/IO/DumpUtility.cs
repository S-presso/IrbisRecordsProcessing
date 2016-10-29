/* DumpUtility.cs -- 
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
    public static class DumpUtility
    {
        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="data"></param>
        public static void Dump < T >
            (
            TextWriter writer,
            T[] data )
        {
            string fmt = " {0:X4}";
            if ( ( data is byte[] )
                 || ( data is sbyte[] ) )
            {
                fmt = " {0:X2}";
            }
            if ( ( data is long[] )
                 || ( data is ulong[] ) )
            {
                fmt = " {0:X8}";
            }
            for ( int i = 0; i < data.Length; i++ )
            {
                if ( ( i%16 ) == 0 )
                {
                    writer.WriteLine ();
                    writer.Write
                        (
                         "{0:X6}> ",
                         i );
                }
                if ( ( i%4 ) == 0 )
                {
                    writer.Write ( " " );
                }
                T item = data [ i ];
                writer.Write
                    (
                     fmt,
                     item );
            }
            writer.WriteLine ();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        public static void Dump < T >
            (
            Stream stream,
            T[] data )
        {
            StreamWriter writer = new StreamWriter
                (
                stream,
                Encoding.Default );
            Dump
                (
                 writer,
                 data );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public static void Dump < T > ( T[] data )
        {
            Dump
                (
                 Console.Out,
                 data );
        }

        #endregion
    }
}
