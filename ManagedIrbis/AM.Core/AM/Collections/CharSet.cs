/* CharSet.cs -- character set.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Character set.
    /// </summary>
    public class CharSet
        : ICloneable,
          IEnumerable
    {
        #region Nested classes

        private class CharSetEnumerator : IEnumerator
        {
            #region Construction

            public CharSetEnumerator ( CharSet charSet )
            {
                _data = charSet.ToArray ();
                _i = -1;
            }

            #endregion

            #region Private members

            private char[] _data;

            private int _i;

            #endregion

            #region IEnumerator members

            public object Current
            {
                get
                {
                    return _data [ _i ];
                }
            }

            public bool MoveNext ( )
            {
                if ( _i >= _data.Length )
                {
                    return false;
                }
                _i++;
                return true;
            }

            public void Reset ( )
            {
                _i = -1;
            }

            #endregion
        }

        #endregion

        #region Properties

        ///<summary>
        /// Доступ по индексу.
        ///</summary>
        public bool this [ char index ]
        {
            [DebuggerStepThrough]
            get
            {
                return _data [ index ];
            }
            [DebuggerStepThrough]
            set
            {
                _data [ index ] = value;
            }
        }

        /// <summary>
        /// Количество элементов.
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                int result = 0;
                for ( int i = 0; i < _data.Length; i++ )
                {
                    if ( _data [ i ] )
                    {
                        result++;
                    }
                }
                return result;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="CharSet"/> class.
        /// </summary>
        public CharSet ( )
        {
            _data = new BitArray ( 0x10000 );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharSet"/> class.
        /// </summary>
        /// <param name="original">The original.</param>
        private CharSet ( CharSet original )
        {
            _data = (BitArray) original._data.Clone ();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharSet"/> class.
        /// </summary>
        /// <param name="characters">The characters.</param>
        public CharSet ( params char[] characters )
            : this ()
        {
            AddRange ( characters );
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="characters"></param>
        public CharSet ( string characters )
            : this ()
        {
            AddRange ( characters );
        }

        #endregion

        #region Private members

        private BitArray _data;

        private void _Add
            (
            char[] ch,
            bool val )
        {
            for ( int i = 0; i < ch.Length; i++ )
            {
                _data [ (int) ( ch [ i ] ) ] = val;
            }
        }

        private void _Add
            (
            string s,
            bool val )
        {
            int len1 = s.Length - 1;
            int len2 = s.Length - 2;
            int i = 0;
            for ( ; i < len1; i++ )
            {
                if ( s [ i ] == '\\' )
                {
                    _data [ (int) s [ i + 1 ] ] = val;
                    i++;
                }
                else if ( s [ i + 1 ] == '-' )
                {
                    if ( i >= len2 )
                    {
                        throw new ArgumentException ();
                    }
                    for ( int c = s [ i ]; c <= s [ i + 2 ]; c++ )
                    {
                        _data [ (int) c ] = val;
                    }
                    i += 2;
                }
                else
                {
                    _data [ (int) s [ i ] ] = val;
                }
            }
            for ( ; i < s.Length; i++ )
            {
                if ( s [ i ] == '\\' )
                {
                    throw new ArgumentException ();
                }
                else
                {
                    _data [ (int) s [ i ] ] = val;
                }
            }
        }

        #endregion

        #region Public members

        /// <summary>
        /// Returns array.
        /// </summary>
        /// <returns></returns>
        public char[] ToArray ( )
        {
            List < char > result = new List < char > ();

            for ( int i = 0; i < _data.Length; i++ )
            {
                if ( _data [ i ] )
                {
                    result.Add ( (char) i );
                }
            }

            return result.ToArray ();
        }

        /// <summary>
        /// Add a character.
        /// </summary>
        /// <param name="character"></param>
        public void Add ( char character )
        {
            _data [ (int) character ] = true;
        }

        /// <summary>
        /// Add some characters.
        /// </summary>
        /// <param name="range"></param>
        public void AddRange ( params char[] range )
        {
            _Add
                (
                 range,
                 true );
        }

        /// <summary>
        /// Add some characters.
        /// </summary>
        /// <param name="range"></param>
        public void AddRange ( string range )
        {
            _Add
                (
                 range,
                 true );
        }

        /// <summary>
        /// Remove a character.
        /// </summary>
        /// <param name="character"></param>
        public void Remove ( char character )
        {
            _data [ (int) character ] = false;
        }

        /// <summary>
        /// Remove some characters.
        /// </summary>
        /// <param name="range"></param>
        public void RemoveRange ( params char[] range )
        {
            _Add
                (
                 range,
                 false );
        }

        /// <summary>
        /// Remove some characters.
        /// </summary>
        /// <param name="range"></param>
        public void RemoveRange ( string range )
        {
            _Add
                (
                 range,
                 false );
        }

        /// <summary>
        /// Invert.
        /// </summary>
        /// <returns></returns>
        public CharSet Not ( )
        {
            _data.Not ();
            return this;
        }

        /// <summary>
        /// Logically multiply.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public CharSet And ( CharSet set )
        {
            _data.And ( set._data );
            return this;
        }

        /// <summary>
        /// Logically add.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public CharSet Or ( CharSet set )
        {
            _data.Or ( set._data );
            return this;
        }

        /// <summary>
        /// Xor.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public CharSet Xor ( CharSet set )
        {
            _data.Xor ( set._data );
            return this;
        }

        /// <summary>
        /// Check the string for characters not included in the charset.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool CheckString ( string text )
        {
            if ( string.IsNullOrEmpty ( text ) )
            {
                return true;
            }
            for ( int i = 0; i < text.Length; i++ )
            {
                if ( !this [ text [ i ] ] )
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Logically add.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static CharSet operator + ( CharSet left,
                                           CharSet right )
        {
            CharSet result = new CharSet ( left );
            return result.Or ( right );
        }

        /// <summary>
        /// Logically substract.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static CharSet operator - ( CharSet left,
                                           CharSet right )
        {
            CharSet result = new CharSet ( left );
            return result.And ( new CharSet ( right ).Not () );
        }

        /// <summary>
        /// Logically multiply.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static CharSet operator * ( CharSet left,
                                           CharSet right )
        {
            CharSet result = new CharSet ( left );
            return result.And ( right );
        }

        #endregion

        #region ICloneable members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public object Clone ( )
        {
            return new CharSet ( this );
        }

        #endregion

        #region IEnumerable members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator ( )
        {
            return new CharSetEnumerator ( this );
        }

        #endregion

        #region Object members

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString ( )
        {
            return new string ( ToArray () );
        }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals ( object obj )
        {
            if ( obj == null )
            {
                return false;
            }
            CharSet charset = obj as CharSet;
            if ( charset != null )
            {
                return ( this.ToString () == charset.ToString () );
            }
            return base.Equals ( obj );
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode ( )
        {
            return ToString ()
                .GetHashCode ();
        }

        #endregion
    }
}
