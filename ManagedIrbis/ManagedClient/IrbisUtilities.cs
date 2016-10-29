/* IrbisUtilities.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace ManagedClient
{
    public static class IrbisUtilities
    {
        #region Public methods

        public static DateTime ParseIrbisDate
            (
                this string text
            )
        {
            DateTime result;
            DateTime.TryParseExact
                (
                    text,
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out result
                );
            return result;
        }

        public static IEnumerable<T[]> Slice<T>
            (
                this IEnumerable<T> sequence,
                int pieceSize
            )
        {
            if (pieceSize <= 0)
            {
                throw new ArgumentOutOfRangeException("pieceSize");
            }
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }

            List<T> piece = new List<T>(pieceSize);
            foreach (T item in sequence)
            {
                piece.Add(item);
                if (piece.Count >= pieceSize)
                {
                    yield return piece.ToArray();
                    piece = new List<T>(pieceSize);
                }
            }

            if (piece.Count != 0)
            {
                yield return piece.ToArray();
            }
        }

        public static string EncodePercentString
            (
                byte[] array
            )
        {
            if (ReferenceEquals(array, null)
                || (array.Length == 0))
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            foreach (byte b in array)
            {
                if (((b >= 'A') && (b <= 'Z'))
                    || ((b >= 'a') && (b <= 'z'))
                    || ((b >= '0') && (b <= '9'))
                    )
                {
                    result.Append((char) b);
                }
                else
                {
                    result.AppendFormat
                        (
                            "%{0:XX}",
                            b
                        );
                }
            }

            return result.ToString();
        }

        public static byte[] DecodePercentString
            (
                string text
            )
        {
            if (string.IsNullOrEmpty(text))
            {
                return new byte[0];
            }

            MemoryStream stream = new MemoryStream();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c != '%')
                {
                    stream.WriteByte((byte) c);
                }
                else
                {
                    if (i >= (text.Length - 2))
                    {
                        throw new FormatException("text");
                    }
                    byte b = byte.Parse
                        (
                            text.Substring(i+1,2),
                            NumberStyles.HexNumber
                        );
                    stream.WriteByte(b);
                }
            }

            return stream.ToArray();
        }

        #endregion
    }
}
