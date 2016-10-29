/* RemotingClient.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Diagnostics;
using System.Runtime.Remoting;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// 
    /// </summary>
    public static class RemotingClient
    {
        #region Properties

        private static bool _workLocally;

        ///<summary>
        /// 
        ///</summary>
        public static bool WorkLocally
        {
            [DebuggerStepThrough]
            get
            {
                return _workLocally;
            }
            [DebuggerStepThrough]
            set
            {
                _workLocally = value;
            }
        }

        private static RemotingServer _server;

        /// <summary>
        /// Server proxy instance.
        /// </summary>
        public static RemotingServer Server
        {
            [DebuggerStepThrough]
            get
            {
                if ( _server == null )
                {
                    lock ( _syncRoot )
                    {
                        if ( _server == null )
                        {
                            _server = new RemotingServer ();
                            if ( !WorkLocally )
                            {
                                Debug.Assert
                                    (
                                     RemotingServices.IsTransparentProxy
                                         ( _server ) );
                            }
                        }
                    }
                }
                return _server;
            }
        }

        #endregion

        #region Private members

        private static object _syncRoot = new object ();

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService < T > ( )
        {
            Trace.WriteLine ( "in RemotingClient.GetService" );
            return (T) Server.GetService
                           (
                            typeof ( T ),
                            null );
        }

        #endregion
    }
}
