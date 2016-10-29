/* CrmEventKind.cs
 */

namespace CrmCommon
{
    /// <summary>
    /// Типы событий.
    /// </summary>
    public enum CrmEventKind
    {
        /// <summary>
        /// Все события
        /// </summary>
        AllEvents = 0,

        /// <summary>
        /// Только посещения
        /// </summary>
        VisitsOnly,

        /// <summary>
        /// Только книговыдачи
        /// </summary>
        LoansOnly
    }
}
