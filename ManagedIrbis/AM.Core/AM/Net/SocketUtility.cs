/* SocketUtility.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Net;
using System.Net.Sockets;

#endregion

namespace AM.Net
{
    /// <summary>
    /// 
    /// </summary>
    public static class SocketUtility
    {
        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Gets IP address from hostname.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <returns>Resolved IP address of the host.</returns>
        public static IPAddress IPAddressFromHostname ( string hostname )
        {
            ArgumentUtility.NotNull
                (
                 hostname,
                 "hostname" );
            if ( hostname.SameString ( "localhost" )
                 || hostname.SameString ( "local" )
                 || hostname.SameString ( "(local)" ) )
            {
                return IPAddress.Loopback;
            }
            IPHostEntry hostEntry = Dns.GetHostEntry ( hostname );
            if ( hostEntry.AddressList.Length == 0 )
            {
                throw new SocketException ();
            }
            return
                hostEntry.AddressList [
                                       new Random ().Next
                                           ( hostEntry.AddressList.Length ) ];
        }

        #endregion
    }
}
