/* PathUtility.cs -- path manipulation routines
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.IO;

using AM.Runtime;

#endregion

namespace AM.IO
{
    /// <summary>
    /// Path manipulation routines.
    /// </summary>
    [Done]
    public static class PathUtility
    {
        #region Private members

        private static string _backslash = new string
            (
            Path.DirectorySeparatorChar,
            1 );

        #endregion

        #region Public methods

        /// <summary>
        /// Appends trailing backslash (if none exists) to given path.
        /// </summary>
        /// <param name="path">Path to convert.</param>
        /// <returns>Converted path.</returns>
        /// <remarks>Path need NOT to be existant.</remarks>
        public static string AppendBackslash ( string path )
        {
            path = ConvertSlashes ( path );
            if ( !path.EndsWith ( _backslash ) )
            {
                path = path + _backslash;
            }
            return path;
        }

        /// <summary>
        /// Converts ordinary slaches to backslashes.
        /// </summary>
        /// <param name="path">Path to convert.</param>
        /// <returns>Converted path.</returns>
        /// <remarks>Path need NOT to be existant.</remarks>
        public static string ConvertSlashes ( string path )
        {
            ArgumentUtility.NotNull
                (
                 path,
                 "path" );

            return path.Replace
                (
                 Path.AltDirectorySeparatorChar,
                 Path.DirectorySeparatorChar );
        }

        /// <summary>
        /// Maps the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string MapPath ( string path )
        {
            ArgumentUtility.NotNull
                (
                 path,
                 "path" );

            string appDir = Path.GetDirectoryName
                ( RuntimeUtility.ExecutableFileName );
            string result = Path.Combine
                (
                 appDir,
                 path );
            return result;
        }

        /// <summary>
        /// Strips extension from given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string StripExtension ( string path )
        {
            ArgumentUtility.NotNull
                (
                 path,
                 "path" );

            string extension = Path.GetExtension ( path );
            if ( !string.IsNullOrEmpty ( extension ) )
            {
                path = path.Substring
                    (
                     0,
                     path.Length - extension.Length );
            }

            return path;
        }

        /// <summary>
        /// Removes trailing backslash (if exists) from the path.
        /// </summary>
        /// <param name="path">Path to convert.</param>
        /// <returns>Converted path.</returns>
        /// <remarks>Path need NOT to be existant.</remarks>
        public static string StripTrailingBackslash ( string path )
        {
            ArgumentUtility.NotNull
                (
                 path,
                 "path" );

            path = ConvertSlashes ( path );
            while ( path.EndsWith ( _backslash ) )
            {
                path = path.Substring
                    (
                     0,
                     path.Length - _backslash.Length );
            }
            return path;
        }

        #endregion
    }
}
