/* CrmOrderDirection.cs -- упорядочение событий
 */

namespace CrmCommon
{
    /// <summary>
    /// Упорядочение событий
    /// </summary>
    public enum CrmOrderDirection
    {
        /// <summary>
        /// Без упорядочения
        /// </summary>
        None = 0,

        /// <summary>
        /// По возрастанию
        /// </summary>
        Ascending,

        /// <summary>
        /// По убыванию
        /// </summary>
        Descending
    }
}
