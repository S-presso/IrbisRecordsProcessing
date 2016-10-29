/* ProcessUtility.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

#endregion

namespace AM.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProcessUtility
    {
        #region Private members

        private static object _GetProcessProperty
            (
            int processId,
            string propName )
        {
            string wmiQuery = string.Format
                (
                 "SELECT * FROM Win32_Process WHERE Handle={0}",
                 processId );
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                ( wmiQuery );
            return ( from ManagementObject mob in searcher.Get ()
                     select mob [ propName ] ).FirstOrDefault ();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether the current process started 
        /// by the Service Control Manager.
        /// </summary>
        /// <returns><c>true</c> if the process started
        /// by the SCM; <c>false</c> otherwise.</returns>
        public static bool StartedBySCM ( )
        {
            int parentPID = Convert.ToInt32
                (
                 _GetProcessProperty
                     (
                      Process.GetCurrentProcess ()
                             .Id,
                      "ParentProcessId" ) );
            string parentName = Convert.ToString
                (
                 _GetProcessProperty
                     (
                      parentPID,
                      "Name" ) );
            return ( string.Compare
                         (
                          parentName,
                          "services.exe",
                          true ) == 0 );
        }

        #endregion
    }
}
