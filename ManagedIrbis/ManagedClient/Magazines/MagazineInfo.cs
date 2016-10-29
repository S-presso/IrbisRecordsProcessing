/* MagazineInfo.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Magazines
{
    /// <summary>
    /// Информация о журнале в целом.
    /// </summary>
    [Serializable]
    public sealed class MagazineInfo
    {
        #region Constants

        #endregion

        #region Properties

        public string Index { get; set; }

        public string Title { get; set; }

        #endregion

        #region Construction

        #endregion

        #region Private members

        #endregion

        #region Public methods

        public static MagazineInfo Parse
            (
                IrbisRecord record
            )
        {
            if (ReferenceEquals(record, null))
            {
                throw new ArgumentNullException("record");
            }

            MagazineInfo result = new MagazineInfo();
            return result;
        }

        #endregion

        #region Object members

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Title;
        }

        #endregion
    }
}
