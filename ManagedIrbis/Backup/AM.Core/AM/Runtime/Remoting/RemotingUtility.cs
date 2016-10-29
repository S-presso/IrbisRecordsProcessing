/* RemotingUtility.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Runtime.Remoting;

using AM.Configuration;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// Some useful methods for remoting.
    /// </summary>
    public static class RemotingUtility
    {
        #region Properties

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Configure.
        /// </summary>
        public static void Configure ( )
        {
            RemotingConfiguration.Configure
                (
                 ConfigurationUtility.ConfigFileName,
                 false );
        }

        #endregion
    }
}
