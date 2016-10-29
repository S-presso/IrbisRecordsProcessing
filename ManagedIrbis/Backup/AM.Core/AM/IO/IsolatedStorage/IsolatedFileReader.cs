/* IsolatedFileReader.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;

#endregion

namespace AM.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class IsolatedFileReader : IsolatedFileHelper
    {
        #region Properties

        private StreamReader _reader;

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <value>The reader.</value>
        public StreamReader Reader
        {
            [DebuggerStepThrough]
            get
            {
                CheckDisposed ();
                return _reader;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="IsolatedFileReader"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="encoding">The encoding.</param>
        protected IsolatedFileReader
            (
            IsolatedStorageScope scope,
            string fileName,
            Encoding encoding )
            : base ( scope,
                     fileName,
                     FileMode.Open,
                     FileAccess.Read )
        {
            ArgumentUtility.NotNull
                (
                 encoding,
                 "encoding" );

            _reader = new StreamReader
                (
                Stream,
                encoding );
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Creates machine-wide isolated storage for the application.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static IsolatedFileReader ForMachine
            (
            string fileName,
            Encoding encoding )
        {
            return new IsolatedFileReader
                (
                IsolatedStorageScope.Machine,
                fileName,
                encoding );
        }

        /// <summary>
        /// Creates machine-wide isolated storage for the application.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static IsolatedFileReader ForMachine ( string fileName )
        {
            return new IsolatedFileReader
                (
                IsolatedStorageScope.Machine,
                fileName,
                Encoding.Default );
        }

        /// <summary>
        /// Creates user-specific isolated storage for the application.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        public static IsolatedFileReader ForUser
            (
            string fileName,
            Encoding encoding )
        {
            return new IsolatedFileReader
                (
                IsolatedStorageScope.User,
                fileName,
                encoding );
        }

        /// <summary>
        /// Creates user-specific isolated storage for the application.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static IsolatedFileReader ForUser ( string fileName )
        {
            return new IsolatedFileReader
                (
                IsolatedStorageScope.User,
                fileName,
                Encoding.Default );
        }

        #endregion

        #region DisposableObject members

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">Is method called from 
        /// <c>Dispose()</c>.</param>
        protected override void Dispose ( bool disposing )
        {
            if ( _reader != null )
            {
                _reader.Dispose ();
                _reader = null;
            }
            base.Dispose ( disposing );
        }

        #endregion
    }
}
