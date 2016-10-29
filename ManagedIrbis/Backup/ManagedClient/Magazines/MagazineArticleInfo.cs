/* MagazineArticleInfo.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Magazines
{
    /// <summary>
    /// Информация о статье.
    /// </summary>
    [Serializable]
    public sealed class MagazineArticleInfo
    {
        #region Properties

        #endregion

        #region Construction

        #endregion

        #region Private members

        #endregion

        #region Public methods

        public static MagazineArticleInfo Parse
            (
                IrbisRecord record
            )
        {
            if (ReferenceEquals(record, null))
            {
                throw new ArgumentNullException("record");
            }

            MagazineArticleInfo result = new MagazineArticleInfo();
            return result;
        }

        #endregion
    }
}
