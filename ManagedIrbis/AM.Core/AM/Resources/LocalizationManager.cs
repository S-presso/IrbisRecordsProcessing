/* LocalizationManager.cs -- localization manager. 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Resources;

#endregion

namespace AM.Resources
{
    /// <summary>
    /// Localization manager.
    /// </summary>
    public sealed class LocalizationManager
    {
        #region Properties

        private Dictionary < string, ResourceManager > _managers;

        private static LocalizationManager _instance;

        /// <summary>
        /// Singleton.
        /// </summary>
        public static LocalizationManager Instance
        {
            [DebuggerStepThrough]
            get
            {
                lock ( typeof ( LocalizationManager ) )
                {
                    if ( _instance == null )
                    {
                        _instance = new LocalizationManager ();
                    }
                    return _instance;
                }
            }
        }

        #endregion

        #region Construction

        private LocalizationManager ( )
        {
            _managers = new Dictionary < string, ResourceManager > ();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Get resource manager for given assembly and resource file.
        /// </summary>
        /// <param name="asm">Assembly.</param>
        /// <param name="resFile">Resource file.</param>
        /// <returns>Resource manager.</returns>
        public ResourceManager GetResourceManager
            (
            Assembly asm,
            string resFile )
        {
            string rmName = asm.FullName + "@" + resFile;

            lock ( _managers )
            {
                ResourceManager result = (ResourceManager) _managers [ rmName ];

                if ( result == null )
                {
                    result = new ResourceManager
                        (
                        resFile,
                        asm );
                    _managers.Add
                        (
                         rmName,
                         result );
                }

                return result;
            }
        }

        /// <summary>
        /// Get localized string.
        /// </summary>
        /// <param name="asm">Assembly</param>
        /// <param name="resFile">Resource file.</param>
        /// <param name="resName">Resource name.</param>
        /// <returns>Localized string.</returns>
        public string GetString
            (
            Assembly asm,
            string resFile,
            string resName )
        {
            ResourceManager rm = GetResourceManager
                (
                 asm,
                 resFile );
            return rm.GetString ( resName );
        }

        /// <summary>
        /// Get localized string.
        /// </summary>
        /// <param name="resFile">Resource file (located in
        /// calling assembly).</param>
        /// <param name="resName">Resource name.</param>
        /// <returns>Localized string.</returns>
        public string GetString
            (
            string resFile,
            string resName )
        {
            return GetString
                (
                 Assembly.GetCallingAssembly (),
                 resFile,
                 resName );
        }

        /// <summary>
        /// Get localized string.
        /// </summary>
        /// <param name="resClass">Class type.</param>
        /// <param name="resName">Resource name.</param>
        /// <returns>Localized string.</returns>
        public string GetString
            (
            Type resClass,
            string resName )
        {
            return GetString
                (
                 resClass.Namespace + ".LocStrings",
                 resName );
        }

        /// <summary>
        /// Get localized string.
        /// </summary>
        /// <param name="resName">Resource name.</param>
        /// <returns>Localized string.</returns>
        public string GetString ( string resName )
        {
            return GetString
                (
                 ( new StackFrame ( 1 ) ).GetMethod ()
                                         .DeclaringType,
                 resName );
        }

        /// <summary>
        /// Get localized string.
        /// </summary>
        public string this [ string resName ]
        {
            [DebuggerStepThrough]
            get
            {
                return GetString ( resName );
            }
        }

        /// <summary>
        /// Get localized string.
        /// </summary>
        public string this [ Type resClass,
                             string resName ]
        {
            [DebuggerStepThrough]
            get
            {
                return GetString
                    (
                     resClass,
                     resName );
            }
        }

        /// <summary>
        /// Get localized string.
        /// </summary>
        public string this [ string resFile,
                             string resName ]
        {
            [DebuggerStepThrough]
            get
            {
                return GetString
                    (
                     resFile,
                     resName );
            }
        }

        /// <summary>
        /// Get localized string.
        /// </summary>
        public string this [ Assembly asm,
                             string resFile,
                             string resName ]
        {
            [DebuggerStepThrough]
            get
            {
                return GetString
                    (
                     asm,
                     resFile,
                     resName );
            }
        }

        #endregion
    }
}
