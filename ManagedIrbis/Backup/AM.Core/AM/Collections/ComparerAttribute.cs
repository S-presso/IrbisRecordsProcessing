/* ComparerAttribute.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;

using AM.Reflection;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage ( AttributeTargets.Class )]
    public class ComparerAttribute : Attribute
    {
        #region Properties

        private Type _comparerType;

        /// <summary>
        /// Gets the type of the comparer.
        /// </summary>
        /// <value>The type of the comparer.</value>
        public Type ComparerType
        {
            get
            {
                return _comparerType;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparerAttribute"/> 
        /// class.
        /// </summary>
        /// <param name="comparerType">Type of the comparer.</param>
        public ComparerAttribute ( Type comparerType )
        {
            _comparerType = comparerType;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Finds the comparer for given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Type FindComparer ( Type type )
        {
            ArgumentUtility.NotNull
                (
                 type,
                 "type" );

            ComparerAttribute comparerAttribute =
                ReflectionUtility.GetCustomAttribute < ComparerAttribute >
                    (
                     type,
                     true );

            return ( comparerAttribute == null )
                       ? null
                       : comparerAttribute.ComparerType;
        }

        #endregion
    }
}
