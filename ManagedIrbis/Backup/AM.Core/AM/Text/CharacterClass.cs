/* CharacterClass.cs -- class of the character.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;

#endregion

namespace AM.Text
{
    /// <summary>
    /// Character classification.
    /// </summary>
    [Done]
    [Flags]
    public enum CharacterClass
    {
        /// <summary>
        /// Whitespace.
        /// </summary>
        Whitespace = 0x01,

        /// <summary>
        /// Striing delimiter.
        /// </summary>
        Quote = 0x02,

        /// <summary>
        /// Line comment begin.
        /// </summary>
        Comment = 0x04,

        /// <summary>
        /// Digit: 0-9.
        /// </summary>
        Digit = 0x08,

        //		/// <summary>
        //		/// Шестнадцатеричная цифра: 0-9, a-f, A-F.
        //		/// </summary>
        //		Hexadecimal = 0x10,

        /// <summary>
        /// Punctuaction: comma, dot, parenthesis etc.
        /// </summary>
        Punctuation = 0x20,

        /// <summary>
        /// Word.
        /// </summary>
        Word = 0x40,

        /// <summary>
        /// End of file.
        /// </summary>
        EndOfFile = 0x80
    }
}
