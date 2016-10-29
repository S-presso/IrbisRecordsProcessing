/* IIndexable.cs -- indexable object interface
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Indexable object interface.
    /// </summary>
    [Done]
    public interface IIndexable
    {
        /// <summary>
        /// Gives access to indexed members.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <returns>Indexed member.</returns>
        object this [ int index ] { get; set; }

        /// <summary>
        /// Returns number of indexed members.
        /// </summary>
        /// <value>Number of indexed members.</value>
        int Count { get; }
    }
}
