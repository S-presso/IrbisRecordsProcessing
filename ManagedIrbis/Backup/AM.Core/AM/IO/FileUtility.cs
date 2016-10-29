/* FileUtility.cs -- file manipulation routines.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;

#endregion

namespace AM.IO
{
    /// <summary>
    /// File manipulation routines.
    /// </summary>
    public static class FileUtility
    {
        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Copies given file only if source is newer than destination.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="backup">If set to <c>true</c> 
        /// create backup copy of destination file.</param>
        /// <returns><c>true</c> if file copied; <c>false</c> otherwise.
        /// </returns>
        public static bool CopyNewer
            (
            string sourcePath,
            string targetPath,
            bool backup )
        {
            ArgumentUtility.FileExists
                (
                 sourcePath,
                 "sourcePath" );
            ArgumentUtility.NotNullOrEmpty
                (
                 targetPath,
                 "targetPath" );

            if ( File.Exists ( targetPath ) )
            {
                FileInfo sourceInfo = new FileInfo ( sourcePath );
                FileInfo targetInfo = new FileInfo ( targetPath );
                if ( sourceInfo.LastWriteTime < targetInfo.LastWriteTime )
                {
                    return false;
                }
                if ( backup )
                {
                    CreateBackup
                        (
                         targetPath,
                         true );
                }
            }
            File.Copy
                (
                 sourcePath,
                 targetPath,
                 true );
            return true;
        }

        /// <summary>
        /// Copies given file and creates backup copy of target file.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <returns>Name of backup file or <c>null</c>
        /// if no backup created.</returns>
        public static string CopyWithBackup
            (
            string sourcePath,
            string targetPath )
        {
            ArgumentUtility.FileExists
                (
                 sourcePath,
                 "sourcePath" );
            ArgumentUtility.NotNullOrEmpty
                (
                 targetPath,
                 "targetPath" );

            string result = null;
            if ( File.Exists ( targetPath ) )
            {
                result = CreateBackup
                    (
                     targetPath,
                     true );
            }
            File.Copy
                (
                 sourcePath,
                 targetPath,
                 false );

            return result;
        }


        /// <summary>
        /// Creates backup copy for given file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="rename">If set to <c>true</c> 
        /// given file will be renamed; otherwise it will be copied.</param>
        /// <returns>Name of the backup file.</returns>
        public static string CreateBackup
            (
            string path,
            bool rename )
        {
            ArgumentUtility.FileExists
                (
                 path,
                 "path" );

            string result = GetNotExistantFileName
                (
                 path,
                 "_backup_" );
            if ( rename )
            {
                File.Move
                    (
                     path,
                     result );
            }
            else
            {
                File.Copy
                    (
                     path,
                     result,
                     false );
            }

            return result;
        }

        /// <summary>
        /// Deletes specified file if it exists.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public static void DeleteIfExists ( string fileName )
        {
            ArgumentUtility.NotNullOrEmpty
                (
                 fileName,
                 "fileName" );

            if ( File.Exists ( fileName ) )
            {
                File.Delete ( fileName );
            }
        }

        /// <summary>
        /// Gets the name of the not existant file.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>Name of notexistant file.</returns>
        /// <exception cref="ApplicationException">
        /// All of possible names for given file is busy.
        /// </exception>
        public static string GetNotExistantFileName
            (
            string original,
            string suffix )
        {
            ArgumentUtility.NotNullOrEmpty
                (
                 original,
                 "original" );
            ArgumentUtility.NotNullOrEmpty
                (
                 suffix,
                 "suffix" );

            string path = Path.GetDirectoryName ( original );
            string name = Path.GetFileNameWithoutExtension ( original );
            string ext = Path.GetExtension ( original );

            for ( int i = 1; i < 10000; i++ )
            {
                string result = Path.Combine
                    (
                     path,
                     name + suffix + i + ext );
                if ( !File.Exists ( result )
                     && !Directory.Exists ( result ) )
                {
                    return result;
                }
            }

            // TODO diagnostics
            throw new ApplicationException ();
        }

        /// <summary>
        /// Побайтовое сравнение двух файлов.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>0, если файлы побайтово совпадают.</returns>
        public static int Compare
            (
            string first,
            string second )
        {
            ArgumentUtility.FileExists
                (
                 first,
                 "first" );
            ArgumentUtility.FileExists
                (
                 second,
                 "second" );
            using ( FileStream firstStream = File.OpenRead ( first ),
                               secondStream = File.OpenRead ( second ) )
            {
                return StreamUtility.Compare
                    (
                     firstStream,
                     secondStream );
            }
        }

        /// <summary>
        /// Copies the specified source file to the specified
        /// destination.
        /// </summary>
        /// <param name="sourceName">Name of the source file.
        /// </param>
        /// <param name="targetName">Name of the target file.
        /// </param>
        /// <param name="overwrite"><c>true</c> if the 
        /// destination file can be overwritten; otherwise, 
        /// <c>false</c>.</param>
        public static void Copy
            (
            string sourceName,
            string targetName,
            bool overwrite )
        {
            ArgumentUtility.NotNull
                (
                 sourceName,
                 "sourceName" );
            ArgumentUtility.NotNull
                (
                 targetName,
                 "targetName" );
            File.Copy
                (
                 sourceName,
                 targetName,
                 overwrite );
            DateTime creationTime = File.GetCreationTime ( sourceName );
            File.SetCreationTime
                (
                 targetName,
                 creationTime );
            DateTime lastAccessTime = File.GetLastAccessTime ( sourceName );
            File.SetLastAccessTime
                (
                 targetName,
                 lastAccessTime );
            DateTime lastWriiteTime = File.GetLastWriteTime ( sourceName );
            File.SetLastWriteTime
                (
                 targetName,
                 lastWriiteTime );
            FileAttributes attributes = File.GetAttributes ( sourceName );
            File.SetAttributes
                (
                 targetName,
                 attributes );
            try
            {
                FileSecurity security = File.GetAccessControl ( sourceName );
                if ( security != null )
                {
                    File.SetAccessControl
                        (
                         targetName,
                         security );
                }
            }
            catch ( Exception exception )
            {
                // TODO some exception handling?
                Trace.WriteLine ( exception.Message );
            }
        }

        /// <summary>
        /// Sets file modification date to current date.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <remarks>If no such file exists it will be created.</remarks>
        public static void Touch ( string fileName )
        {
            ArgumentUtility.NotNullOrEmpty
                (
                 fileName,
                 "fileName" );

            if ( File.Exists ( fileName ) )
            {
                File.SetLastWriteTime
                    (
                     fileName,
                     DateTime.Now );
            }
            else
            {
                File.WriteAllBytes
                    (
                     fileName,
                     new byte[0] );
            }
        }

        #endregion
    }
}
