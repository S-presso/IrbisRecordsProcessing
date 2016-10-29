/* IrbisDirectReader.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ManagedClient;

#endregion

namespace ManagedClient
{
    public sealed class IrbisDirectReader
        : IDisposable
    {
        #region Properties

        public MstFile Mst { get; private set; }

        public XrfFile Xrf { get; private set; }

        public InvertedFile InvertedFile { get; private set; }

        public string Database { get; private set; }

        #endregion

        #region Construction

        public IrbisDirectReader 
            (
                string masterFile,
                bool inMemory
            )
        {
            Database = Path.GetFileNameWithoutExtension(masterFile);
            Mst = new MstFile
                (
                    Path.ChangeExtension
                        (
                            masterFile, 
                            ".mst"
                        )
                );
            Xrf = new XrfFile
                (
                    Path.ChangeExtension
                    (
                        masterFile, 
                        ".xrf"
                    ),
                    inMemory
                );
            InvertedFile = new InvertedFile
                (
                    Path.ChangeExtension
                    (
                        masterFile,
                        ".ifp"
                    )
                );
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        public int GetMaxMfn()
        {
            return Mst.ControlRecord.NextMfn - 1;
        }

        public IrbisRecord ReadRecord
            (
                int mfn
            )
        {
            XrfRecord xrfRecord = Xrf.ReadRecord(mfn);
            if ( xrfRecord.Offset == 0 )
            {
                return null;
            }
            MstRecord mstRecord = Mst.ReadRecord2(xrfRecord.Offset);
            IrbisRecord result = mstRecord.DecodeRecord();
            result.Database = Database;
            return result;
        }

        public IrbisRecord[] ReadAllRecordVersions
            (
                int mfn
            )
        {
            List<IrbisRecord> result = new List<IrbisRecord>();
            IrbisRecord lastVersion = ReadRecord(mfn);
            if (lastVersion != null)
            {
                result.Add(lastVersion);
                while (true)
                {
                    long offset = lastVersion.PreviousOffset;
                    if (offset == 0)
                    {
                        break;
                    }
                    MstRecord mstRecord = Mst.ReadRecord2(offset);
                    IrbisRecord previousVersion = mstRecord.DecodeRecord();
                    if (previousVersion != null)
                    {
                        result.Add(previousVersion);
                        lastVersion = previousVersion;
                    }
                }
            }

            return result.ToArray();
        }

        //public IrbisRecord ReadRecord2
        //    (
        //        int mfn
        //    )
        //{
        //    XrfRecord xrfRecord = Xrf.ReadRecord(mfn);
        //    MstRecord mstRecord = Mst.ReadRecord2(xrfRecord.Offset);
        //    IrbisRecord result = mstRecord.DecodeRecord();
        //    result.Database = Database;
        //    return result;
        //}

        public int[] SearchSimple(string key)
        {
            int[] mfns = InvertedFile.SearchSimple(key);
            List<int> result = new List<int>();
            foreach (int mfn in mfns)
            {
                if (!Xrf.ReadRecord(mfn).Deleted)
                {
                    result.Add(mfn);
                }
            }
            return result.ToArray();
        }

        public IrbisRecord[] SearchReadSimple(string key)
        {
            int[] mfns = InvertedFile.SearchSimple(key);
            List<IrbisRecord> result = new List<IrbisRecord>();
            foreach (int mfn in mfns)
            {
                try
                {
                    XrfRecord xrfRecord = Xrf.ReadRecord(mfn);
                    if (!xrfRecord.Deleted)
                    {
                        MstRecord mstRecord = Mst.ReadRecord2(xrfRecord.Offset);
                        if (!mstRecord.Deleted)
                        {
                            IrbisRecord irbisRecord = mstRecord.DecodeRecord();
                            irbisRecord.Database = Database;
                            result.Add(irbisRecord);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return result.ToArray();
        }

        #endregion

        #region IDisposable members

        public void Dispose ()
        {
            if ( Mst != null )
            {
                Mst.Dispose ();
                Mst = null;
            }
            if ( Xrf != null )
            {
                Xrf.Dispose ();
                Xrf = null;
            }
            if (InvertedFile != null)
            {
                InvertedFile.Dispose();
                InvertedFile = null;
            }
        }

        #endregion
    }
}
