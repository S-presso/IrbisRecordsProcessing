/* MagazineIssueInfo.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Magazines
{
    [Serializable]
    public sealed class MagazineIssueInfo
    {
        #region Properties

        /// <summary>
        /// Шифр документа в базе. Поле 903.
        /// </summary>
        public string DocumentCode { get; set; }

        /// <summary>
        /// Шифр журнала. Поле 933.
        /// </summary>
        public string MagazineCode { get; set; }

        /// <summary>
        /// Год. Поле 934.
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// Том. Поле 935.
        /// </summary>
        public string Volume { get; set; }

        /// <summary>
        /// Номер, часть. Поле 936.
        /// </summary>
        public string Number { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        #endregion

        #region Public methods

        public static MagazineIssueInfo Parse
            (
                IrbisRecord record
            )
        {
            if (ReferenceEquals(record, null))
            {
                throw new ArgumentNullException("record");
            }

            MagazineIssueInfo result = new MagazineIssueInfo();
            return result;
        }

        #endregion
    }
}
