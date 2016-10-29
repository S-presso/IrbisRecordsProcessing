/* RemotingServer.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// Generic remoting server.
    /// </summary>
    public class RemotingServer : MarshalByRefObject
    {
        #region Properties

        /// <summary>
        /// Signal.
        /// </summary>
        public static readonly ServerSignal Signal = new ServerSignal ();

        /// <summary>
        /// Is server paused.
        /// </summary>
        public static bool Paused;

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RemotingServer ( )
        {
            Trace.WriteLine ( "in RemotingServer..ctor" );
        }

        #endregion

        #region Private members

        private static Dictionary < Type, FactoryDelegate > _map =
            new Dictionary < Type, FactoryDelegate > ();

        private static object _syncRoot = new object ();

        private static void _Add
            (
            Type intf,
            Type implementor )
        {
            MethodInfo miFound = null;
            foreach (
                MethodInfo mi in
                    implementor.GetMethods
                        (
                         BindingFlags.Static | BindingFlags.Public
                         | BindingFlags.NonPublic ) )
            {
                if ( mi.GetCustomAttributes
                         (
                          typeof ( FactoryMethodAttribute ),
                          false ) != null )
                {
                    miFound = mi;
                    break;
                }
            }
            if ( miFound == null )
            {
                throw new ArgumentException ( "implementor" );
            }
            FactoryDelegate fd =
                (FactoryDelegate)
                Delegate.CreateDelegate
                    (
                     typeof ( FactoryDelegate ),
                     null,
                     miFound,
                     true );
            _map [ intf ] = fd;
        }

        #endregion

        #region Public methods

        #region Instance

        /// <summary>
        /// Get service of given type.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual object GetService
            (
            Type serviceType,
            ServiceContext context )
        {
            Trace.WriteLine ( "in RemotingServer.GetService" );
            if ( Paused )
            {
                throw new ServicePausedException ();
            }
            FactoryDelegate factory = _map [ serviceType ];
            object service = factory ();
            return service;
        }

        #endregion

        #region Static

        /// <summary>
        /// Add type.
        /// </summary>
        /// <param name="intf"></param>
        /// <param name="implementor"></param>
        public static void AddType
            (
            Type intf,
            Type implementor )
        {
            lock ( _syncRoot )
            {
                _Add
                    (
                     intf,
                     implementor );
            }
        }

        /// <summary>
        /// Remove type.
        /// </summary>
        /// <param name="intf"></param>
        public static void RemoveType ( Type intf )
        {
            lock ( _syncRoot )
            {
                _map.Remove ( intf );
            }
        }

        /// <summary>
        /// Initialize type list.
        /// </summary>
        /// <param name="idl"></param>
        public static void Initialize ( InterfaceDefinitionList idl )
        {
            Trace.WriteLine ( "in RemotingServer.Initialize" );
            lock ( _syncRoot )
            {
                foreach ( InterfaceDefinition intf in idl )
                {
                    Type intfType = Type.GetType ( intf.Interface );
                    Type implType = Type.GetType ( intf.Implementor );
                    _Add
                        (
                         intfType,
                         implType );
                }
            }
        }

        /// <summary>
        /// Dump installed services.
        /// </summary>
        public static void DumpServices ( )
        {
            foreach ( Type intf in _map.Keys )
            {
                Trace.WriteLine ( intf.Name );
            }
        }

        /// <summary>
        /// Run server in console.
        /// </summary>
        public static void RunConsole ( )
        {
            Signal.WaitForCommand ();
            Thread.Sleep ( 1000 );
            //Console.ReadKey ();
        }

        /// <summary>
        /// Run server in Windows service.
        /// </summary>
        public static void RunService ( ServiceBase service )
        {
            ServiceBase.Run ( service );
        }

        #endregion

        #endregion
    }
}
