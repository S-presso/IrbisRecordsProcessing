/* MstFile.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace ManagedClient
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MstFile
        : IDisposable
    {
        #region Constants
        #endregion

        #region Properties

        public static int PreloadLength = 10*1024;

        public MstControlRecord ControlRecord { get; private set; }

        public string FileName { get; private set; }

        #endregion

        #region Construction

        public MstFile ( string fileName )
        {
            FileName = fileName;

            _stream = new FileStream 
                (
                    fileName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite
                );

            _ReadControlRecord ();
        }

        #endregion

        #region Private members

        private bool _lockFlag;

        private readonly FileStream _stream;

        private void _ReadControlRecord ()
        {
            ControlRecord = new MstControlRecord
                                {
                                    Reserv1 = _stream.ReadInt32Network (),
                                    NextMfn = _stream.ReadInt32Network (),
                                    NextPosition = _stream.ReadInt64Network (),
                                    Reserv2 = _stream.ReadInt32Network (),
                                    Reserv3 = _stream.ReadInt32Network (),
                                    Reserv4 = _stream.ReadInt32Network (),
                                    Blocked = _stream.ReadInt32Network ()
                                };
        }

        #endregion

        #region Public methods

        public MstRecord ReadRecord ( long offset )
        {
            if ( _stream.Seek ( offset, SeekOrigin.Begin ) != offset )
            {
                throw new IOException();
            }

            //new ObjectDumper()
            //    .DumpStream(_stream,offset,64)
            //    .WriteLine();

            Encoding encoding = new UTF8Encoding(false,true);

            MstRecordLeader leader = MstRecordLeader.Read(_stream);

            List <MstDictionaryEntry> dictionary 
                = new List < MstDictionaryEntry > ();

            for ( int i = 0; i < leader.Nvf; i++ )
            {
                MstDictionaryEntry entry = new MstDictionaryEntry
                                               {
                                                   Tag = _stream.ReadInt32Network (),
                                                   Position = _stream.ReadInt32Network (),
                                                   Length = _stream.ReadInt32Network ()
                                               };
                dictionary.Add ( entry );
            }

            foreach ( MstDictionaryEntry entry in dictionary )
            {
                long endOffset = offset + leader.Base + entry.Position;
                _stream.Seek ( endOffset, SeekOrigin.Begin );
                entry.Bytes = _stream.ReadBytes ( entry.Length );
                if ( entry.Bytes != null )
                {
                    entry.Text = encoding.GetString ( entry.Bytes );
                }
            }

            MstRecord result = new MstRecord
                                   {
                                       Leader = leader,
                                       Dictionary = dictionary
                                   };
            return result;
        }

        private static void _AppendStream
            (
                Stream source,
                Stream target,
                int amount
            )
        {
            if (amount <= 0)
            {
                throw new IOException();
                //return false;
            }
            long savedPosition = target.Position;
            target.Position = target.Length;

            byte[] buffer = new byte[amount];
            int readed = source.Read(buffer, 0, amount);
            if (readed <= 0)
            {
                throw new IOException();
                //return false;
            }
            target.Write(buffer,0,readed);
            target.Position = savedPosition;
            //return true;
        }

        public MstRecord ReadRecord2
            (
                long offset
            )
        {
            if (_stream.Seek(offset, SeekOrigin.Begin) != offset)
            {
                throw new IOException();
            }

            Encoding encoding = new UTF8Encoding(false, true);

            MemoryStream memory = new MemoryStream(PreloadLength);
            _AppendStream(_stream, memory, PreloadLength);
            memory.Position = 0;

            MstRecordLeader leader = MstRecordLeader.Read(memory);
            int amountToRead = (int) (leader.Length - memory.Length);
            if (amountToRead > 0)
            {
                _AppendStream(_stream, memory, amountToRead);
            }

            List<MstDictionaryEntry> dictionary
                = new List<MstDictionaryEntry>();

            for (int i = 0; i < leader.Nvf; i++)
            {
                MstDictionaryEntry entry = new MstDictionaryEntry
                {
                    Tag = memory.ReadInt32Network(),
                    Position = memory.ReadInt32Network(),
                    Length = memory.ReadInt32Network()
                };
                dictionary.Add(entry);
            }

            foreach (MstDictionaryEntry entry in dictionary)
            {
                long endOffset = leader.Base + entry.Position;
                memory.Seek(endOffset, SeekOrigin.Begin);
                entry.Bytes = memory.ReadBytes(entry.Length);
                if (entry.Bytes != null)
                {
                    entry.Text = encoding.GetString(entry.Bytes);
                }
            }

            MstRecord result = new MstRecord
            {
                Leader = leader,
                Dictionary = dictionary
            };
            return result;
        }


        /// <summary>
        /// Блокировка базы данных в целом.
        /// </summary>
        /// <param name="flag"></param>
        public void LockDatabase
            (
                bool flag
            )
        {
            byte[] buffer = new byte[4];

            _stream.Position = MstControlRecord.LockFlagPosition;
            if (flag)
            {
                _stream.Lock(0, MstControlRecord.RecordSize);
                buffer[0] = 1;
                _stream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                _stream.Write(buffer, 0, buffer.Length);
                _stream.Unlock(0, MstControlRecord.RecordSize);
            }
            _lockFlag = flag;
        }

        /// <summary>
        /// Чтение флага блокировки базы данных в целом.
        /// </summary>
        public bool ReadDatabaseLockedFlag()
        {
            byte[] buffer = new byte[4];

            _stream.Position = MstControlRecord.LockFlagPosition;
            _stream.Read(buffer, 0, buffer.Length);
            return Convert.ToBoolean(BitConverter.ToInt32(buffer, 0));
        }

        #endregion

        #region IDisposable members

        public void Dispose ()
        {
            if ( _stream != null )
            {
                if (_lockFlag)
                {
                    LockDatabase(false);
                }

                _stream.Dispose ();
            }
        }

        #endregion

        #region Object members

        #endregion
    }
}
