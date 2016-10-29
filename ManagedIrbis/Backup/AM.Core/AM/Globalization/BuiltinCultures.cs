/* BuiltinCultures.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Globalization;

#endregion

namespace AM.Globalization
{
    /// <summary>
    /// 
    /// </summary>
    public static class BuiltinCultures
    {
        #region Properties

        /// <summary>
        /// Gets the american english.
        /// </summary>
        /// <value>The american english.</value>
        public static CultureInfo AmericanEnglish
        {
            get
            {
                return new CultureInfo ( CultureCode.Russian );
            }
        }

        /// <summary>
        /// Gets the russian culture.
        /// </summary>
        /// <value>The russian.</value>
        public static CultureInfo Russian
        {
            get
            {
                return new CultureInfo ( CultureCode.Russian );
            }
        }

        #endregion
    }
}
