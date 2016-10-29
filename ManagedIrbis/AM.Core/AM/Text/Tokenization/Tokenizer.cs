/* Tokenizer.cs -- text tokenizer
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#endregion

namespace AM.Text.Tokenization
{
	/// <summary>
	/// Text tokenizer
	/// </summary>
	public class Tokenizer
		: IDisposable
	{
		#region Construction

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="settings"></param>
		public Tokenizer
			(
			TextReader reader,
			TokenizerSettings settings )
		{
			if ( reader == null )
			{
				throw new ArgumentNullException ();
			}
			if ( settings == null )
			{
				settings = TokenizerSettings.Default;
			}
			_reader = reader;
			_settings = settings;
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~Tokenizer ()
		{
			Dispose ();
		}

		#endregion

		#region Private members

		private TextReader _reader;

		private TokenizerSettings _settings;

		private bool _ownReader;

		#endregion

		#region Public methods

		/// <summary>
		/// Start tokenization from file.
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="encoding"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static Tokenizer FromFile
			(
			string fileName,
			Encoding encoding,
			TokenizerSettings settings )
		{
			StreamReader reader = new StreamReader ( fileName, encoding );
			Tokenizer result = new Tokenizer ( reader, settings );
			result._ownReader = true;
			return result;
		}

		private Token _peekedToken;

		private string _Collect ( CharacterClass expect, char ch )
		{
			CharacterBuffer buf = new CharacterBuffer ( ch );
			while ( true )
			{
				int cur = _reader.Peek ();
				if ( cur == -1 )
				{
					break;
				}
				ch = (char) cur;
				CharacterClass cc = _settings.Classifier [ ch ];
				if ( ( cc & expect ) == 0 )
				{
					break;
				}
				buf.Write ( (char) _reader.Read () );
			}
			return buf.ToString ();
		}

		private string _CollectUntil ( char[] stop, char[] disallowed )
		{
			CharacterBuffer buf = new CharacterBuffer ();
			while ( true )
			{
				int cur = _reader.Read ();
				if ( cur == -1 )
				{
					throw new IOException ();
				}
				char ch = (char) cur;
				if ( Array.IndexOf  ( stop, ch ) >= 0 )
				{
					break;
				}
				if ( Array.IndexOf ( disallowed, ch ) >= 0 )
				{
					throw new ApplicationException ();
				}
				buf.Write ( ch );
			}
			return buf.ToString ();
		}

		private Token _GetToken ()
		{
			Token result = new Token ();
			int cur = _reader.Read ();
			if ( cur == -1 )
			{
				result._characterClass = CharacterClass.EndOfFile;
				return result;
			}
			char ch = (char) cur;
			CharacterClass cc = _settings.Classifier [ ch ];
			CharacterBuffer cb = new CharacterBuffer ();
			cb.Write ( ch );
			switch ( cc )
			{
			case CharacterClass.Whitespace:
				result._characterClass = CharacterClass.Whitespace;
				result._value = _Collect ( CharacterClass.Whitespace, ch );
				if ( _settings.SkipWhiteSpace )
				{
					return _GetToken ();
				}
				break;
			case CharacterClass.Quote:
				result._characterClass = CharacterClass.Quote;
				result._value = _CollectUntil
					( new char[] {ch},
					  new char[] {'\n'} );
				break;
			case CharacterClass.Comment:
				result._characterClass = CharacterClass.Comment;
				result._value = _CollectUntil
					( new char[] {'\n'},
					  new char[0] );
				break;
			case CharacterClass.Digit:
				result._characterClass = CharacterClass.Digit;
				result._value = _Collect ( CharacterClass.Digit, ch );
				break;
			case CharacterClass.Punctuation:
				result._characterClass = CharacterClass.Punctuation;
				result._value = new string ( ch, 1 );
				break;
			case CharacterClass.Word:
				result._characterClass = CharacterClass.Word;
				result._value = _Collect ( CharacterClass.Word, ch );
				break;
			case CharacterClass.EndOfFile:
				result._characterClass = CharacterClass.EndOfFile;
				break;
			}
			return result;
		}

		/// <summary>
		/// Start tokenization from stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="encoding"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static Tokenizer FromStream
			(
			Stream stream,
			Encoding encoding,
			TokenizerSettings settings )
		{
			StreamReader reader = new StreamReader ( stream, encoding );
			return new Tokenizer ( reader, settings );
		}

		/// <summary>
		/// Start tokenization from string.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static Tokenizer FromString
			(
			string text,
			TokenizerSettings settings )
		{
			TextReader reader = new StringReader ( text );
			return new Tokenizer ( reader, settings );
		}

		/// <summary>
		/// Start tokenization from memory.
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="encoding"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static Tokenizer FromMemory
			(
			byte[] bytes,
			Encoding encoding,
			TokenizerSettings settings )
		{
			MemoryStream stream = new MemoryStream ( bytes, false );
			StreamReader reader = new StreamReader ( stream, encoding );
			return new Tokenizer ( reader, settings );
		}

		/// <summary>
		/// Peek next token.
		/// </summary>
		/// <returns></returns>
		public Token Peek ()
		{
			if ( _peekedToken != null )
			{
				return _peekedToken;
			}
			_peekedToken = _GetToken ();
			return _peekedToken;
		}

		/// <summary>
		/// Get next token.
		/// </summary>
		/// <returns></returns>
		public Token Next ()
		{
			Token result;
			if ( _peekedToken != null )
			{
				result = _peekedToken;
				_peekedToken = null;
			}
			else
			{
				result = _GetToken ();
			}
			return result;
		}

		/// <summary>
		/// Parse entire stream.
		/// </summary>
		/// <returns></returns>
		public Token[] Parse ()
		{
			List <Token> result = new List <Token> ();
			while ( true )
			{
				Token token = Next ();
				result.Add ( token );
				if ( ( token.CharacterClass & CharacterClass.EndOfFile ) != 0 )
				{
					break;
				}
			}
			return result.ToArray ();
		}

		#endregion

		#region IDisposable members

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose ()
		{
			if ( ( _ownReader )
			     && ( _reader != null ) )
			{
				_reader.Close ();
			}
			GC.SuppressFinalize ( this );
		}

		#endregion
	}
}