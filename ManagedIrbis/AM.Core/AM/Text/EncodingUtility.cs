/* EncodingUtility.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using AM.IO;

#endregion

namespace AM.Text
{
    /// <summary>
    /// 
    /// </summary>
    public static class EncodingUtility
    {
        #region Nested classes

        private class KnownEncoding
        {
            public string Name;

            public Encoding Encoding;

            public byte[] Preamble;

            public KnownEncoding
                (
                string name,
                Encoding encoding )
            {
                Name = name;
                Encoding = encoding;
                Preamble = encoding.GetPreamble ();
            }
        }

        #endregion

        #region Properties

        private static int _maxPreambleLength;

        /// <summary>
        /// Maximum preamble length.
        /// </summary>
        public static int MaxPreambleLength
        {
            get
            {
                return _maxPreambleLength;
            }
        }

        private static Encoding _windows1251;

        /// <summary>
        /// Gets the Windows-1251 (cyrillic) <see cref="Encoding"/>.
        /// </summary>
        /// <value>The Windows-1251 encoding.</value>
        public static Encoding Windows1251
        {
            [DebuggerStepThrough]
            get
            {
                if ( _windows1251 == null )
                {
                    _windows1251 = Encoding.GetEncoding ( 1251 );
                }
                return _windows1251;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Class constructor.
        /// </summary>
        static EncodingUtility ( )
        {
            List < KnownEncoding > known = new List < KnownEncoding > ();
            known.Add
                (
                 new KnownEncoding
                     (
                     "Big-endian UTF16",
                     new UnicodeEncoding
                         (
                         true,
                         true ) ) );
            known.Add
                (
                 new KnownEncoding
                     (
                     "Little-endian UTF16",
                     new UnicodeEncoding
                         (
                         false,
                         true ) ) );
            known.Add
                (
                 new KnownEncoding
                     (
                     "UTF8",
                     new UTF8Encoding ( true ) ) );
            //known.Add ( new KnownEncoding
            //    (
            //        "UTF7",
            //        new UTF7Encoding
            //        ( 
            //            true
            //        )
            //    ) );
            known.Add
                (
                 new KnownEncoding
                     (
                     "Big-endian UTF32",
                     new UTF32Encoding
                         (
                         true,
                         true ) ) );
            known.Add
                (
                 new KnownEncoding
                     (
                     "Little-endian UTF32",
                     new UTF32Encoding
                         (
                         false,
                         true ) ) );
            _known = known.ToArray ();
            foreach ( KnownEncoding enc in _known )
            {
                if ( enc.Preamble.Length > _maxPreambleLength )
                {
                    _maxPreambleLength = enc.Preamble.Length;
                }
            }
        }

        #endregion

        #region Private members

        private static KnownEncoding[] _known;

        #endregion

        #region Public methods

        /// <summary>
        /// Determine text encoding.
        /// </summary>
        /// <param name="textWithPreamble"></param>
        /// <returns></returns>
        public static Encoding DetermineTextEncoding ( byte[] textWithPreamble )
        {
            if ( textWithPreamble == null )
            {
                throw new ArgumentNullException ( "textWithPreamble" );
            }
            foreach ( KnownEncoding known in _known )
            {
                if ( textWithPreamble.Length <= known.Preamble.Length )
                {
                    bool found = true;
                    for ( int i = 0; i < known.Preamble.Length; i++ )
                    {
                        if ( textWithPreamble [ i ] != known.Preamble [ i ] )
                        {
                            found = false;
                            break;
                        }
                    }
                    if ( found )
                    {
                        return known.Encoding;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Determine text encoding.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Encoding DetermineTextEncoding ( Stream stream )
        {
            byte[] textWithPreamble = StreamUtility.ReadAsMuchAsPossible
                (
                 stream,
                 MaxPreambleLength );
            return DetermineTextEncoding ( textWithPreamble );
        }

        /// <summary>
        /// Determines the text file encoding.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static Encoding DetermineTextEncoding ( string fileName )
        {
            using ( FileStream stream = File.OpenRead ( fileName ) )
            {
                return DetermineTextEncoding ( stream );
            }
        }

        #endregion
    }
}
