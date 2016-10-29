/* ResourceUtility.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.IO;

using AM.IO;

#endregion

namespace AM.Resources
{
    /// <summary>
    /// 
    /// </summary>
    public static class ResourceUtility
    {
        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Extracts the resource.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="destinationPath">The destination path.</param>
        public static void ExtractResource
            (
            Type type,
            string resourceName,
            string destinationPath )
        {
            ArgumentUtility.NotNull
                (
                 type,
                 "type" );
            ArgumentUtility.NotNullOrEmpty
                (
                 resourceName,
                 "resourceName" );
            ArgumentUtility.NotNullOrEmpty
                (
                 destinationPath,
                 "destinationPath" );

            Stream resourceStream = type.Assembly.GetManifestResourceStream
                (
                 type,
                 resourceName );
            using ( Stream fileStream = new FileStream
                (
                destinationPath,
                FileMode.Create ) )
            {
                StreamUtility.Copy
                    (
                     resourceStream,
                     fileStream );
            }
        }

        #endregion
    }
}
