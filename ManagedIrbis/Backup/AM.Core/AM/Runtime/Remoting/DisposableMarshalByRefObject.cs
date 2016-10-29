/* DisposableMarshalByRefObject.cs -- disposable Marshal-By-Reference object
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// Disposable Marshal-By-Reference object.
    /// </summary>
    public class DisposableMarshalByRefObject
        : MarshalByRefObject,
          IDisposable
    {
        #region Properties

        private bool _disposed;

        /// <summary>
        /// Is instance disposed.
        /// </summary>
        public bool Disposed
        {
            get
            {
                return _disposed;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when the objec is disposing.
        /// </summary>
        public event EventHandler Disposing;

        #endregion

        #region Construction

        /// <summary>
        /// Destructor.
        /// </summary>
        ~DisposableMarshalByRefObject ( )
        {
            if ( !_disposed )
            {
                Dispose ( false );
                _Cleanup ();
            }
        }

        #endregion

        #region Private members

        private void _Cleanup ( )
        {
            Trace.WriteLine ( "in DisposableMarshalByRefObject._Cleanup" );
            _disposed = true;
            if ( Disposing != null )
            {
                Disposing
                    (
                     this,
                     EventArgs.Empty );
            }
            RemotingServices.Disconnect ( this );
            GC.SuppressFinalize ( this );
        }

        /// <summary>
        /// Hook on Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose ( bool disposing )
        {
            // Nothing to do here.
        }

        #endregion

        #region IDisposable members

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <remarks>Non-thread-safe.</remarks>
        [OneWay]
        public void Dispose ( )
        {
            if ( !_disposed )
            {
                Dispose ( true );
                _Cleanup ();
            }
        }

        #endregion
    }
}
