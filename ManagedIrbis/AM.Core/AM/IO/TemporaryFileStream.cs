/* TemporaryFileStream.cs -- file-backed stream that self-destroy on dispose
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.IO;

#endregion

namespace AM.IO
{
    /// <summary>
    /// File backed <see cref="Stream"/> that self-destroy
    /// on <see cref="Stream.Dispose"/>.
    /// </summary>
    public class TemporaryFileStream : FileStream
    {
        #region Fields

        /// <summary>
        /// 
        /// </summary>
        public static FileShare FileShareMode = FileShare.None;

        #endregion Fields 

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryFileStream"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="shareMode">The share mode.</param>
        /// <param name="deleteOnDispose">if set to <c>true</c> [delete on dispose].</param>
        public TemporaryFileStream
            (
            string fileName,
            FileShare shareMode,
            bool deleteOnDispose )
            : base ( fileName,
                     FileMode.OpenOrCreate,
                     FileAccess.ReadWrite,
                     shareMode )
        {
            DeleteOnDispose = deleteOnDispose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryFileStream"/> class.
        /// </summary>
        /// <param name="shareMode">The share mode.</param>
        /// <param name="deleteOnDispose">if set to <c>true</c> [delete on dispose].</param>
        public TemporaryFileStream
            (
            FileShare shareMode,
            bool deleteOnDispose )
            : this ( Path.GetTempFileName (),
                     shareMode,
                     deleteOnDispose )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryFileStream"/> class.
        /// </summary>
        public TemporaryFileStream ( )
            : this ( FileShareMode,
                     true )
        {
        }

        #endregion Constructors 

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [delete on dispose].
        /// </summary>
        /// <value><c>true</c> if [delete on dispose]; otherwise, <c>false</c>.</value>
        public bool DeleteOnDispose { get; set; }

        #endregion Properties 

        #region Methods

        // Protected Methods

        /// <summary>
        /// Releases the unmanaged resources used by the 
        /// <see cref="FileStream"/> and optionally releases 
        /// the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both 
        /// managed and unmanaged resources; false to release 
        /// only unmanaged resources.</param>
        protected override void Dispose ( bool disposing )
        {
            base.Dispose ( disposing );
            if ( DeleteOnDispose && File.Exists ( Name ) )
            {
                File.Delete ( Name );
            }
        }

        #endregion Methods 
    }
}
