/* ServerSignal.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Threading;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ServerSignal
    {
        #region Properties

        /// <summary>
        /// Command.
        /// </summary>
        public ServerCommand Command;

        #endregion

        #region Private members

        private ManualResetEvent _event = new ManualResetEvent ( false );

        #endregion

        #region Public methods

        /// <summary>
        /// Wait for command.
        /// </summary>
        public void WaitForCommand ( )
        {
            _event.Reset ();
            _event.WaitOne ();
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void SayStop ( )
        {
            Command = ServerCommand.StopServer;
            _event.Set ();
        }

        #endregion
    }
}
