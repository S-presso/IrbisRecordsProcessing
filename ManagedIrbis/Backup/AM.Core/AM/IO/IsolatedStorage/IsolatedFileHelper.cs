/* IsolatedStorageHelper.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;

#endregion

namespace AM.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class IsolatedFileHelper : DisposableObject
    {
        #region Properties

        private IsolatedStorageFile _storage;

        /// <summary>
        /// Gets the storage.
        /// </summary>
        /// <value>The storage.</value>
        public IsolatedStorageFile Storage
        {
            [DebuggerStepThrough]
            get
            {
                CheckDisposed ();
                return _storage;
            }
        }

        private IsolatedStorageFileStream _stream;

        /// <summary>
        /// Gets or sets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public IsolatedStorageFileStream Stream
        {
            [DebuggerStepThrough]
            get
            {
                CheckDisposed ();
                return _stream;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="IsolatedFileHelper"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <param name="fileAccess">The file access.</param>
        protected IsolatedFileHelper
            (
            IsolatedStorageScope scope,
            string fileName,
            FileMode fileMode,
            FileAccess fileAccess )
        {
            ArgumentUtility.NotNullOrEmpty
                (
                 fileName,
                 "fileName" );

//			_storage = IsolatedStorageFile.GetStore
//				(
//				scope,
//				null
//				);
            _stream = new IsolatedStorageFileStream
                (
                fileName,
                fileMode,
                fileAccess //,
//				_storage
                );
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates machine-wide isolated storage for the application.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <param name="fileAccess">The file access.</param>
        /// <returns></returns>
        public static IsolatedFileHelper ForMachine
            (
            string fileName,
            FileMode fileMode,
            FileAccess fileAccess )
        {
            return new IsolatedFileHelper
                (
                IsolatedStorageScope.Machine,
                fileName,
                fileMode,
                fileAccess );
        }


        /// <summary>
        /// Creates user-specific isolated storage for the application.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <param name="fileAccess">The file access.</param>
        /// <returns></returns>
        public static IsolatedFileHelper ForUser
            (
            string fileName,
            FileMode fileMode,
            FileAccess fileAccess )
        {
            return new IsolatedFileHelper
                (
                IsolatedStorageScope.User,
                fileName,
                fileMode,
                fileAccess );
        }

        #endregion

        #region IDisposable members

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Is method called from 
        /// <c>Dispose()</c>.</param>
        protected override void Dispose ( bool disposing )
        {
            if ( _stream != null )
            {
                _stream.Dispose ();
                _stream = null;
            }
            if ( _storage != null )
            {
                _storage.Dispose ();
                _storage = null;
            }
            base.Dispose ( disposing );
        }

        #endregion
    }
}
