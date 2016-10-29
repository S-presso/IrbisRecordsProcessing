/* CompressionUtility.cs -- useful routines that simplifies data compression. 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.IO;
using System.IO.Compression;

#endregion

namespace AM.IO.Compression
{
    /// <summary>
    /// Useful routines that simplifies data compression/decomression.
    /// </summary>
    [Done]
    public static class CompressionUtility
    {
        #region Public methods

        /// <summary>
        /// Compress data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Comressed data.</returns>
        public static byte[] Compress ( byte[] data )
        {
            ArgumentUtility.NotNull
                (
                 data,
                 "data" );

            MemoryStream ms = new MemoryStream ();
            using ( DeflateStream zip = new DeflateStream
                (
                ms,
                CompressionMode.Compress,
                true ) )
            {
                zip.Write
                    (
                     data,
                     0,
                     data.Length );
            }

            return ms.ToArray ();
        }

        /// <summary>
        /// Decompress data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Decomressed data.</returns>
        public static byte[] Decompress ( byte[] data )
        {
            ArgumentUtility.NotNull
                (
                 data,
                 "data" );

            MemoryStream ms = new MemoryStream ( data );
            using ( DeflateStream zip = new DeflateStream
                (
                ms,
                CompressionMode.Decompress ) )
            {
                MemoryStream result = new MemoryStream ();
                StreamUtility.Copy
                    (
                     zip,
                     result );

                return result.ToArray ();
            }
        }

        #endregion
    }
}
