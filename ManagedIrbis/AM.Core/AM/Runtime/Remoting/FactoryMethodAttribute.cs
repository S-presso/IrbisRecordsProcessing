/* FactoryMethodAttribute.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// Attribute for factory method.
    /// </summary>
    [AttributeUsage ( AttributeTargets.Method, AllowMultiple = false,
        Inherited = false )]
    public sealed class FactoryMethodAttribute : Attribute
    {
    }
}
