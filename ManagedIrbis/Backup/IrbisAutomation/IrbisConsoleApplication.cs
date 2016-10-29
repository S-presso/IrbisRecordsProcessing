/* IrbisConsoleApplication.cs
 */

#region Using directives

using System;

#endregion

namespace IrbisAutomation
{
    public class IrbisConsoleApplication
        : IrbisApplication
    {
        #region Construction

        public IrbisConsoleApplication ()
        {
        }

        public IrbisConsoleApplication
            (
                string connectionString
            )
            : base(connectionString)
        {
        }

        #endregion

        #region Private members

        private bool _cancelPending;

        private void _CancelKeyPress
            (
                object sender, 
                ConsoleCancelEventArgs e
            )
        {
            _cancelPending = true;
            e.Cancel = true;
        }

        #endregion

        #region Protected members

        protected override void OnApplicationInit ()
        {
            Console.TreatControlCAsInput = false;
            base.OnApplicationInit ();
            Console.CancelKeyPress += _CancelKeyPress;
        }

        protected override bool OnError 
            ( 
                Exception exception 
            )
        {
            Console.WriteLine ( exception );
            return base.OnError ( exception );
        }

        public override bool ReportProgress ()
        {
            if ( ( TotalProcessed % 50 ) == 1 )
            {
                Console.WriteLine();
                Console.Write 
                    ( 
                        "{0:000 000}> ",
                        TotalProcessed - 1
                    );
            }
            if ( ( TotalProcessed % 5 ) == 1 )
            {
                Console.Write ( " " );
            }
            if ( ( TotalProcessed % 10 ) == 1 )
            {
                Console.Write ( " " );
            }

            Console.Write (".");
            
            return 
                (
                    _cancelPending 
                    || base.ReportProgress ()
                );
        }

        public override void WriteLine
            (
                string format,
                params object[] args 
            )
        {
            Console.WriteLine
                (
                    format, 
                    args
                );
        }

        #endregion

        #region Public methods

        #endregion

        #region IDisposable members

        public override void Dispose ()
        {
            Console.CancelKeyPress -= _CancelKeyPress;
            base.Dispose ();
        }

        #endregion
    }
}
