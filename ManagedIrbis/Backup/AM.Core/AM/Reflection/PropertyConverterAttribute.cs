/* PropertyConverterAttribute.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage ( AttributeTargets.Property | AttributeTargets.Field )]
    public class PropertyConverterAttribute : Attribute
    {
        #region Properties

        private Type _converterType;

        /// <summary>
        /// Gets the type of the converter.
        /// </summary>
        /// <value>The type of the converter.</value>
        public Type ConverterType
        {
            [DebuggerStepThrough]
            get
            {
                return _converterType;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="PropertyConverterAttribute"/> class.
        /// </summary>
        /// <param name="converterType">Type of the converter.</param>
        public PropertyConverterAttribute ( Type converterType )
        {
            ArgumentUtility.NotNull
                (
                 converterType,
                 "converterType" );
            _converterType = converterType;
        }

        #endregion
    }
}
