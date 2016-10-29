/* SubField.cs
 */

#region Using directives

using System;
using System.Globalization;

#endregion

namespace ManagedClient
{
    /// <summary>
    /// Подполе MARC-записи.
    /// </summary>
    [Serializable]
    public class SubField
    {
        #region Properties

        /// <summary>
        /// Код подполя.
        /// </summary>
        public char Code { get; set; }

        /// <summary>
        /// Значение подполя.
        /// </summary>
        public string Text { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="SubField" /> class.
        /// </summary>
        public SubField()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubField" /> class.
        /// </summary>
        /// <param name="code">The code.</param>
        public SubField(char code)
        {
            Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubField" /> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="text">The text.</param>
        public SubField(char code, string text)
        {
            Code = code;
            Text = text;
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public SubField Clone ( )
        {
            SubField result = new SubField
                                  {
                                      Code = Code,
                                      Text = Text
                                  };
            return result;
        }

        public static int Compare
            (
                SubField subField1, 
                SubField subField2,
                bool verbose
            )
        {
            int result = subField1.Code.CompareTo(subField2.Code);
            if (result != 0)
            {
                if (verbose)
                {
                    Console.WriteLine
                        (
                            "SubField1 Code={0}, SubField2 Code={1}",
                            subField1.Code,
                            subField2.Code
                        );
                }
                return result;
            }
            
            result = string.CompareOrdinal(subField1.Text, subField2.Text);
            if (verbose && (result != 0))
            {
                Console.WriteLine
                    (
                        "SubField1 Text={0}, SubField2 Text={1}",
                        subField1.Text,
                        subField2.Text
                    );
            }
            return result;
        }

        #endregion

        #region Object members

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format
                (
                    "^{0}{1}",
                    Code,
                    Text
                );
        }

        #endregion
    }
}
