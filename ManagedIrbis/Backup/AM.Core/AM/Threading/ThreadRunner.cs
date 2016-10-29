/* ThreadRunner.cs -- runs method in new thread
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Threading;

#endregion

namespace AM.Threading
{
    /// <summary>
    /// Runs specified method ( delegate ) in new <see cref="Thread"/>.
    /// </summary>
    public class ThreadRunner
    {
        #region Construction

        /// <summary>
        /// Don't allow somebody to create instance.
        /// </summary>
        private ThreadRunner ( )
        {
            // Nothing to do.
        }

        #endregion

        #region Private members

        private object[] _parameters;

        private ThreadMethod _method;

        private void _RunMethod ( )
        {
            _method ( _parameters );
        }

        private Delegate _delegate;

        private void _RunDelegate ( )
        {
            _delegate.DynamicInvoke ( _parameters );
        }

        private void _MethodCallback ( object notUsed )
        {
            _method ( _parameters );
        }

        #endregion

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Thread RunThread
            (
            ThreadMethod method,
            params object[] parameters )
        {
            if ( method == null )
            {
                throw new ArgumentNullException ( "method" );
            }
            ThreadRunner runner = new ThreadRunner ();
            runner._method = method;
            runner._parameters = parameters;
            ThreadStart start = new ThreadStart ( runner._RunMethod );
            Thread result = new Thread ( start );
            result.IsBackground = true;
            result.Start ();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Thread RunThread
            (
            Delegate method,
            params object[] parameters )
        {
            if ( method == null )
            {
                throw new ArgumentNullException ( "method" );
            }
            ThreadRunner runner = new ThreadRunner ();
            runner._delegate = method;
            runner._parameters = parameters;
            ThreadStart start = new ThreadStart ( runner._RunDelegate );
            Thread result = new Thread ( start );
            result.IsBackground = true;
            result.Start ( parameters );
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        public static void AddToPool
            (
            ThreadMethod method,
            params object[] parameters )
        {
            if ( method == null )
            {
                throw new ArgumentNullException ( "method" );
            }
            ThreadRunner runner = new ThreadRunner ();
            runner._method = method;
            runner._parameters = parameters;
            ThreadPool.QueueUserWorkItem ( runner._MethodCallback );
        }

        #endregion
    }
}
