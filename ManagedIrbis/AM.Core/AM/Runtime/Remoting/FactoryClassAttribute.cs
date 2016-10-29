/* FactoryClassAttribute.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// Mark class for factory.
    /// </summary>
    [AttributeUsage ( AttributeTargets.Class, AllowMultiple = false,
        Inherited = false )]
    public sealed class FactoryClassAttribute : Attribute
    {
        #region Properties

        private Type _interface;

        /// <summary>
        /// Implemented interface.
        /// </summary>
        public Type Interface
        {
            get
            {
                return _interface;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        public FactoryClassAttribute ( Type implementedInterface )
        {
            _interface = implementedInterface;
        }

        #endregion
    }
}
