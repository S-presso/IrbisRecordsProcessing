/* ThreadMethod.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

#endregion

namespace AM.Threading
{
    /// <summary>
    /// Делегат для <see cref="ThreadRunner"/>.
    /// </summary>
    public delegate void ThreadMethod ( object[] parameters );
}
