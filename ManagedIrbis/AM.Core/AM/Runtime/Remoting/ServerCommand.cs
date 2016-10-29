/* ServerCommand.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// Server command.
    /// </summary>
    public enum ServerCommand
    {
        /// <summary>
        /// Stop server.
        /// </summary>
        StopServer,

        /// <summary>
        /// Pause processing.
        /// </summary>
        Pause,

        /// <summary>
        /// Resume processing.
        /// </summary>
        Resume
    }
}
