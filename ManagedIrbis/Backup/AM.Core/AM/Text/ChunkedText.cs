/* ChunkedText.cs
   ArsMagna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#endregion

namespace AM.Text
{
    public sealed class ChunkedText : IComparable < ChunkedText >
    {
        #region Nested classes

        private class Chunk
        {
            #region Fields

            private readonly string Text;

            private readonly ulong Value;

            private readonly bool IsNumber;

            #endregion

            #region Construction

            public Chunk
                (
                StringBuilder gathered,
                bool isNumber )
            {
                Text = gathered.ToString ();
                IsNumber = isNumber;
                if ( IsNumber )
                {
                    Value = ulong.Parse
                        (
                         Text,
                         NumberStyles.Any );
                }
                gathered.Length = 0;
            }

            #endregion

            #region Public methods

            public static int Compare
                (
                Chunk left,
                Chunk right )
            {
                if ( left.IsNumber
                     && right.IsNumber )
                {
                    return Math.Sign ( (long) ( left.Value - right.Value ) );
                }
                return string.Compare
                    (
                     left.Text,
                     right.Text,
                     true,
                     CultureInfo.CurrentCulture );
            }

            #endregion

            #region Object members

            public override string ToString ( )
            {
                return IsNumber
                           ? Value.ToString ( CultureInfo.InvariantCulture )
                           : Text;
            }

            public override int GetHashCode ( )
            {
                int result = ( IsNumber )
                                 ? Value.GetHashCode ()
                                 : Text.GetHashCode ();
                return result;
            }

            public override bool Equals ( object obj )
            {
                return ( Compare
                             (
                              this,
                              (Chunk) obj ) == 0 );
            }

            #endregion
        }

        public class Comparer : IComparer < ChunkedText >
        {
            public int Compare
                (
                ChunkedText x,
                ChunkedText y )
            {
                return x.CompareTo ( y );
            }
        }

        #endregion

        #region Private members

        private readonly List < Chunk > _chunks = new List < Chunk > ();

        #endregion

        #region Public methods

        public static int Compare
            (
            string leftText,
            string rightText )
        {
            ChunkedText left = Parse ( leftText );
            ChunkedText right = Parse ( rightText );
            return left.CompareTo ( right );
        }

        public static ChunkedText Parse ( string line )
        {
            ChunkedText result = new ChunkedText ();

            if ( !string.IsNullOrEmpty ( line ) )
            {
                line = line.Trim ();
            }

            if ( !string.IsNullOrEmpty ( line ) )
            {
                StringBuilder gathered = new StringBuilder ();
                bool isNumber = char.IsDigit
                    (
                     line,
                     0 );

                foreach ( char c in line )
                {
                    if ( isNumber == char.IsDigit ( c ) )
                    {
                        if ( !char.IsWhiteSpace ( c ) )
                        {
                            gathered.Append ( c );
                        }
                    }
                    else
                    {
                        if ( gathered.Length != 0 )
                        {
                            result._chunks.Add
                                (
                                 new Chunk
                                     (
                                     gathered,
                                     isNumber ) );
                        }
                        if ( !char.IsWhiteSpace ( c ) )
                        {
                            gathered.Append ( c );
                        }
                        isNumber = !isNumber;
                    }
                }

                if ( gathered.Length != 0 )
                {
                    result._chunks.Add
                        (
                         new Chunk
                             (
                             gathered,
                             isNumber ) );
                }
            }

            return result;
        }

        #endregion

        #region IComparable<ChunkedText> members

        public int CompareTo ( ChunkedText other )
        {
            IEnumerator < Chunk > thisEnumerator = _chunks.GetEnumerator ();
            IEnumerator < Chunk > otherEnumerator =
                other._chunks.GetEnumerator ();

            bool thisFlag = thisEnumerator.MoveNext ();
            bool otherFlag = otherEnumerator.MoveNext ();

            while ( thisFlag && otherFlag )
            {
                Chunk thisChunk = thisEnumerator.Current;
                Chunk otherChunk = otherEnumerator.Current;

                int result = Chunk.Compare
                    (
                     thisChunk,
                     otherChunk );
                if ( result != 0 )
                {
                    return result;
                }

                thisFlag = thisEnumerator.MoveNext ();
                otherFlag = otherEnumerator.MoveNext ();
            }

            return thisFlag
                       ? 1
                       : ( otherFlag
                               ? -1
                               : 0 );
        }

        #endregion

        #region Object members

        private bool Equals ( ChunkedText other )
        {
            return ( CompareTo ( other ) == 0 );
        }

        public override bool Equals ( object obj )
        {
            if ( ReferenceEquals
                (
                 null,
                 obj ) )
            {
                return false;
            }
            if ( ReferenceEquals
                (
                 this,
                 obj ) )
            {
                return true;
            }
            return ( obj is ChunkedText ) && Equals ( (ChunkedText) obj );
        }

        public override int GetHashCode ( )
        {
            return _chunks.Aggregate
                (
                 0,
                 ( current,
                   chunk ) => ( current << 3 ) + chunk.GetHashCode () );
        }

        public override string ToString ( )
        {
            StringBuilder result = new StringBuilder ();

            foreach ( Chunk chunk in _chunks )
            {
                result.AppendFormat
                    (
                     "| {0}",
                     chunk );
            }

            return result.ToString ();
        }

        #endregion
    }
}
