/* IsolatedFileWriter.cs -- 
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
    public class IsolatedFileWriter : IsolatedFileHelper
    {
        #region Properties

        private StreamWriter _writer;

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <value>The writer.</value>
        public StreamWriter Writer
        {
            [DebuggerStepThrough]
            get
            {
                CheckDisposed ();
                return _writer;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="IsolatedFileWriter"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="encoding">The encoding.</param>
        protected IsolatedFileWriter
            (
            IsolatedStorageScope scope,
            string fileName,
            Encoding encoding )
            : base ( scope,
                     fileName,
                     FileMode.Create,
                     FileAccess.Write )
        {
            ArgumentUtility.NotNull
                (
                 encoding,
                 "encoding" );

            _writer = new StreamWriter
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
        public static IsolatedFileWriter ForMachine
            (
            string fileName,
            Encoding encoding )
        {
            return new IsolatedFileWriter
                (
                IsolatedStorageScope.Domain | IsolatedStorageScope.Machine,
                fileName,
                encoding );
        }

        /// <summary>
        /// Creates machine-wide isolated storage for the application.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static IsolatedFileWriter ForMachine ( string fileName )
        {
            return new IsolatedFileWriter
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
        public static IsolatedFileWriter ForUser
            (
            string fileName,
            Encoding encoding )
        {
            return new IsolatedFileWriter
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
        public static IsolatedFileWriter ForUser ( string fileName )
        {
            return new IsolatedFileWriter
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
            if ( _writer != null )
            {
                _writer.Dispose ();
                _writer = null;
            }
            base.Dispose ( disposing );
        }

        #endregion
    }
}
