/* IrbisApplication.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ManagedClient;

using CM=System.Configuration.ConfigurationManager;

#endregion

namespace IrbisAutomation
{
    public abstract class IrbisApplication
        : IDisposable
    {
        #region Properties

        public ManagedClient64 Client
        {
            get
            {
                return _client;
            }
        }

        public virtual string ConnectionString
        {
            get
            {
                return _connectionString
                    ?? CM.AppSettings["connection-string"]
                    ?? DefaultConnectionString;
            }
        }

        public virtual string DefaultConnectionString
        {
            get
            {
                return "host=127.0.0.1;port=6666;user=1;"
                    + "password=1;db=IBIS;";
            }
        }

        public TimeSpan Elapsed
        {
            get
            {
                return _stopwatch.Elapsed;
            }
        }

        public int TotalProcessed { get; set; }

        public object UserData { get; set; }

        #endregion

        #region Construction

        // ReSharper disable once DoNotCallOverridableMethodsInConstructor
        protected IrbisApplication()
        {
            _stopwatch = new Stopwatch ();
            _client = new ManagedClient64();
            OnApplicationInit ();
        }

        protected IrbisApplication 
            (
                string connectionString
            )
            : this()
        {
            _connectionString = connectionString;
        }

        #endregion

        #region Private members

        private readonly ManagedClient64 _client;

        private readonly string _connectionString;

        private readonly Stopwatch _stopwatch;

        #endregion

        #region Protected members

        protected virtual void OnApplicationInit ()
        {
            // Nothing to do here
        }

        protected virtual void OnBeforeConnect()
        {
            // Nothing to do here
        }

        protected virtual void OnAfterConnect()
        {
            // Nothing to do here
        }

        protected virtual bool OnError 
            (
                Exception exception
            )
        {
            // Nothing to do here
            return false;
        }

        protected bool ProcessBatch 
            (
                BatchRecordReader batch
            )
        {
            TotalProcessed = 0;
            _stopwatch.Start ();
            try
            {
                using (
                    IEnumerator < IrbisRecord > enumerator =
                        batch.GetEnumerator () )
                {
                    while ( true )
                    {
                        try
                        {
                            if ( !enumerator.MoveNext () )
                            {
                                break;
                            }
                            IrbisRecord record = enumerator.Current;
                            bool flag = ProcessRecord ( record );
                            TotalProcessed++;
                            flag &= ReportProgress ();
                            if ( !flag )
                            {
                                return false;
                            }
                        }
                        catch ( Exception exception )
                        {
                            if ( !OnError ( exception ) )
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch ( Exception exception )
            {
                OnError ( exception );
                return false;
            }
            finally
            {
                _stopwatch.Stop ();
                WriteLine ( string.Empty );
                WriteLine 
                    (  
                        "Processed: {0}, elapsed: {1}",
                        TotalProcessed,
                        Elapsed
                    );
            }
            return true;
        }

        #endregion

        #region Public methods

        public void Connect()
        {
            if (!Client.Connected)
            {
                try
                {
                    OnBeforeConnect ();
                    Client.ParseConnectionString 
                        ( 
                            ConnectionString 
                        );
                    Client.Connect ();
                    OnAfterConnect ();
                }
                catch ( Exception exception )
                {
                    OnError ( exception );
                }
            }
        }

        public virtual bool ProcessRecord
            (
                IrbisRecord record
            )
        {
            // Nothing to do here
            return true;
        }

        public bool ProcessRecords
            (
                IEnumerable<int> mfns
            )
        {
            BatchRecordReader batch = new BatchRecordReader 
                (  
                    Client,
                    mfns
                );
            return ProcessBatch 
                ( 
                    batch 
                );
        }

        public bool ProcessRecords
            (
                string expression,
                params object[] args
            )
        {
            BatchRecordReader batch = new BatchRecordReader 
                (
                    Client,
                    expression,
                    args
                );
            return ProcessBatch 
                ( 
                    batch 
                );
        }

        public bool ProcessRecords()
        {
            BatchRecordReader batch = new BatchRecordReader 
                (
                    Client
                );
            return ProcessBatch 
                ( 
                    batch
                );
        }

        public virtual bool ReportProgress ()
        {
            // Nothing to do here
            return true;
        }

        public virtual void WriteLine
            (
                string format,
                params object[] args
            )
        {
            // Nothing to do here
        }

        #endregion

        #region IDisposable members

        public virtual void Dispose()
        {
            if ( !ReferenceEquals ( Client, null ) )
            {
                Client.Dispose();
            }
        }

        #endregion
    }
}
