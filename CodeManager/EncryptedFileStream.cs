using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CodeManager
{
    class EncryptedFileStream : Stream, IDisposable
    {
        private FileAccess dataFileAccess;
        //private bool isDisposed_value;
        private BinaryReader cryptReader;
        private BinaryReader dataReader;
        private BinaryWriter dataWriter;

        public override bool CanRead
        {
            get {
                if ((dataFileAccess & FileAccess.Read) == FileAccess.Read)
                {
                    return true;
                }
                return false;
            }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { 
                if ((dataFileAccess & FileAccess.Write) == FileAccess.Write)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsDisposed
        {
            get {
                if (cryptReader == null)
                {
                    return true;
                }
                return false;
            }
        }

        
        public EncryptedFileStream(string maskFile, string dataFile, FileAccess access)
        {
            dataFileAccess = access;
            try
            {
                cryptReader = new BinaryReader(File.Open(maskFile, FileMode.Open, FileAccess.Read, FileShare.Read));
                var dataf = File.Open(dataFile, FileMode.Open, access, FileShare.Read);
                if ((access & FileAccess.Read) == FileAccess.Read)
                {
                    dataReader = new BinaryReader(dataf);
                }
                if ((access & FileAccess.Write) == FileAccess.Write)
                {
                    dataWriter = new BinaryWriter(dataf);
                }
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    
        public void IDisposable.Dispose()
        {
            if (cryptReader != null)
            {
                cryptReader.Dispose();
                cryptReader = null;
                if (dataReader != null)
                {
                    dataReader.Dispose();
                    dataReader = null;
                }
                if (dataWriter != null)
                {
                    dataWriter.Dispose();
                    dataWriter = null;
                }
            }
        }

        ~EncryptedFileStream()
        {
            Dispose();
        }
    }
}
