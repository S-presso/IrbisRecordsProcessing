/* ServiceContext.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// Context to run service in.
    /// </summary>
    [Serializable]
    public class ServiceContext
    {
        #region Properties

        private string _userName;

        ///<summary>
        /// 
        ///</summary>
        public string UserName
        {
            [DebuggerStepThrough]
            get
            {
                return _userName;
            }
        }

        private string _password;

        ///<summary>
        /// 
        ///</summary>
        public string Password
        {
            [DebuggerStepThrough]
            get
            {
                return _password;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServiceContext ( )
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ServiceContext
            (
            string userName,
            string password )
        {
            _userName = userName;
            _password = password;
        }

        #endregion
    }
}
