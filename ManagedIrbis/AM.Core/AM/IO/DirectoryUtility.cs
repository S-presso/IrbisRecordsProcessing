/* DirectoryUtility.cs -- directory manipulation routines.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace AM.IO
{
    /// <summary>
    /// Directory manipulation routines.
    /// </summary>
    public static class DirectoryUtility
    {
        #region Private members

        private static void _GetFiles
            (
            List < string > found,
            string path,
            string[] masks,
            bool recursive )
        {
            foreach ( string mask in masks )
            {
                string[] files = Directory.GetFiles
                    (
                     path,
                     mask );
                foreach ( string file in files )
                {
                    if ( !found.Contains ( file ) )
                    {
                        found.Add ( file );
                    }
                }
            }
            if ( recursive )
            {
                string[] dirs = Directory.GetDirectories ( path );
                foreach ( string dir in dirs )
                {
                    _GetFiles
                        (
                         found,
                         dir,
                         masks,
                         recursive );
                }
            }
        }

        /// <summary>
        /// Разделитель элементов в маске файлов.
        /// Используется в GetFiles().
        /// </summary>
        private static char[] _separator = new char[]
                                           {
                                               ';'
                                           };

        #endregion

        #region Public methods

        /// <summary>
        /// Clears the specified directory. Deletes all files and subdirectories
        /// from the directory.
        /// </summary>
        /// <param name="path">Path to the directory.</param>
        public static void ClearDirectory ( string path )
        {
            ArgumentUtility.NotNull
                (
                 path,
                 "path" );

            foreach ( string subdir in Directory.GetDirectories ( path ) )
            {
                Directory.Delete
                    (
                     Path.Combine
                         (
                          path,
                          subdir ),
                     true );
            }
            foreach ( string fileName in Directory.GetFiles ( path ) )
            {
                File.Delete
                    (
                     Path.Combine
                         (
                          path,
                          fileName ) );
            }
        }

        /// <summary>
        /// Gets list of files in specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mask"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static string[] GetFiles
            (
            string path,
            string mask,
            bool recursive )
        {
            ArgumentUtility.NotNullOrEmpty
                (
                 path,
                 "path" );
            ArgumentUtility.NotNullOrEmpty
                (
                 mask,
                 "mask" );

            List < string > found = new List < string > ();
            string[] masks = mask.Split
                (
                 _separator,
                 StringSplitOptions.RemoveEmptyEntries );
            _GetFiles
                (
                 found,
                 path,
                 masks,
                 recursive );
            return found.ToArray ();
        }

        /// <summary>
        /// Расширяет регулярное выражение DOS/Windows до списка файлов.
        /// </summary>
        /// <param name="wildcard">Регулярное выражение, включающее
        /// в себя символы * и ?, например *.exe или c:\*.bat.</param>
        /// <returns>Массив имен файлов, соответствующих регулярному
        /// выражению. Если параметр <paramref name="wildcard"/>
        /// включал имя директории, то каждое имя в массив также 
        /// будет содержать имя директории.</returns>
        /// <remarks>В поиске участвуют только файлы, но не директории.
        /// </remarks>
        public static string[] Glob ( string wildcard )
        {
            ArgumentUtility.NotNullOrEmpty
                (
                 wildcard,
                 "wildcard" );

            string dir = Path.GetDirectoryName ( wildcard );
            string name = Path.GetFileName ( wildcard );
            if ( string.IsNullOrEmpty ( dir ) )
            {
                FileInfo[] files =
                    new DirectoryInfo ( Directory.GetCurrentDirectory () )
                        .GetFiles ( wildcard );
                List < string > result = new List < string > ( files.Length );
                foreach ( FileInfo file in files )
                {
                    result.Add ( file.Name );
                }
                return result.ToArray ();
            }
            return Directory.GetFiles
                (
                 dir,
                 name );
        }

        #endregion
    }
}
