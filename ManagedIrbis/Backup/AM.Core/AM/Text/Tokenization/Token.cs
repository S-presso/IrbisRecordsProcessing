/* Token.cs -- text token.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Diagnostics;

#endregion

namespace AM.Text.Tokenization
{
	/// <summary>
	/// Text token.
	/// </summary>
	public sealed class Token
	{
		#region Properties

		internal CharacterClass _characterClass;

		/// <summary>
		/// Character class.
		/// </summary>
		public CharacterClass CharacterClass
		{
			[DebuggerStepThrough]
			get
			{
				return _characterClass;
			}
		}

		internal string _value;

		/// <summary>
		/// Value of the token.
		/// </summary>
		public string Value
		{
			[DebuggerStepThrough]
			get
			{
				return _value;
			}
		}

		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="Token"/> class.
		/// </summary>
		internal Token ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Token"/> class.
		/// </summary>
		/// <param name="characterClass">The character class.</param>
		/// <param name="value">The value.</param>
		internal Token ( CharacterClass characterClass, string value )
		{
			_characterClass = characterClass;
			_value = value;
		}

		#endregion
	}
}