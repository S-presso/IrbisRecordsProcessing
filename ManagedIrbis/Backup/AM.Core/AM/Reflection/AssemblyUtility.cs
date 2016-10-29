/* AssemblyUtility.cs -- collection of assembly manipulation routines.
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace AM.Reflection
{
    /// <summary>
    /// Collection of assembly manipulation routines.
    /// </summary>
    public static class AssemblyUtility
    {
        #region Public methods

        /// <summary>
        /// Check an assembly to see if it has the given public key token.
        /// </summary>
        /// <param name="assembly">Assembly to check.</param>
        /// <param name="expectedToken"></param>
        /// <returns></returns>
        /// <remarks>Does not check to make sure the assembly's signature 
        /// is valid.</remarks>
        public static bool CheckForToken
            (
            Assembly assembly,
            byte[] expectedToken )
        {
            ArgumentUtility.NotNull
                (
                 assembly,
                 "assembly" );
            ArgumentUtility.NotNull
                (
                 expectedToken,
                 "expectedToken" );

            byte[] realToken = assembly.GetName ()
                                       .GetPublicKeyToken ();
            if ( realToken == null )
            {
                return false;
            }
            return ( ArrayUtility.Compare
                         (
                          realToken,
                          expectedToken ) == 0 );
        }

        /// <summary>
        /// Check an assembly to see if it has the given public key token.
        /// </summary>
        /// <param name="pathToAssembly"></param>
        /// <param name="expectedToken"></param>
        /// <returns></returns>
        public static bool CheckForToken
            (
            string pathToAssembly,
            byte[] expectedToken )
        {
            Assembly assembly = Assembly.ReflectionOnlyLoadFrom
                ( pathToAssembly );
            return CheckForToken
                (
                 assembly,
                 expectedToken );
        }

        /// <summary>
        /// Determines whether the specified assembly is debug version.
        /// </summary>
        /// <param name="assembly">The assembly to check.</param>
        /// <returns>
        /// 	<c>true</c> if the specified assembly is debug version; 
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is <c>null</c>.
        /// </exception>
        public static bool IsDebug ( Assembly assembly )
        {
            ArgumentUtility.NotNull
                (
                 assembly,
                 "assembly" );

            object[] attributes = assembly.GetCustomAttributes
                (
                 typeof ( DebuggableAttribute ),
                 false );
            return ( attributes.Length != 0 );
        }

        /// <summary>
        /// Check an assembly whether it has Microsoft public key token.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static bool IsMicrosoftSigned ( Assembly assembly )
        {
            ArgumentUtility.NotNull
                (
                 assembly,
                 "assembly" );

            return ( CheckForToken
                         (
                          assembly,
                          PublicKeyTokens.MicrosoftClr )
                     || CheckForToken
                            (
                             assembly,
                             PublicKeyTokens.MicrosoftFX ) );
        }

        /// <summary>
        /// Check an assembly whether it has Microsoft public key token.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsMicrosoftSigned ( string path )
        {
            ArgumentUtility.NotNullOrEmpty
                (
                 path,
                 "path" );

            return ( CheckForToken
                         (
                          path,
                          PublicKeyTokens.MicrosoftClr )
                     || CheckForToken
                            (
                             path,
                             PublicKeyTokens.MicrosoftFX ) );
        }

        #endregion
    }
}
