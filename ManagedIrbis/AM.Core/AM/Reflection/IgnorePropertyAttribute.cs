/* IgnorePropertyAttribute.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;

#endregion

namespace AM.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage (
        AttributeTargets.Property | AttributeTargets.Field
        | AttributeTargets.Class )]
    public class IgnorePropertyAttribute : Attribute
    {
    }
}
