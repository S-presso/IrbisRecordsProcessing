/* IPropertyConverter.cs -- 
   Ars Magna project, http://library.istu.edu/am */

namespace AM.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPropertyConverter
    {
        /// <summary>
        /// Converts the property.
        /// </summary>
        /// <param name="propertyValue">The property value.</param>
        /// <returns></returns>
        object ConvertProperty ( object propertyValue );
    }
}
