/* TokenizerSettings.cs -- settings for tokenizer.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;

#endregion

namespace AM.Text.Tokenization
{
	/// <summary>
	/// Settings for <see cref="Tokenizer"/>.
	/// </summary>
	[Serializable]
	public class TokenizerSettings
	{
		#region Properties

		/// <summary>
		/// Character classifier.
		/// </summary>
		public CharacterClassifier Classifier;

		/// <summary>
		/// Skip whitespace.
		/// </summary>
		public bool SkipWhiteSpace;

		/// <summary>
		/// Skip comments.
		/// </summary>
		public bool SkipComments;

		#endregion

		#region Public methods

		/// <summary>
		/// Default tokenizer settings.
		/// </summary>
		public static TokenizerSettings Default
		{
			get
			{
				TokenizerSettings result = new TokenizerSettings ();

				result.Classifier = CharacterClassifier.Default;
				result.SkipComments = true;
				result.SkipWhiteSpace = true;

				return result;
			}
		}

		#endregion
	}
}