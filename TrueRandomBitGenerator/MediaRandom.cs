using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Drawing;
using System.Windows.Forms;

namespace TrueRandomBitGenerator
{
    class MediaRandom : IDisposable
    {
        //public const int xMin = 450, xMax = 550, yMin = 350, yMax = 450;
        public const int TargetSize = 0x01000000;  //16,777,216 = 2^24
        public const int MaxStreamSizePerHash = 0x00400000;  //4,194,304 = 4Mb
        public const int MinStreamSizePerHash = 0x00000400;  //1,024 = 1kb
        public const double HeadTruncateSize = 0.1d;
        public const double TailTruncateSize = 0.05d;

        private readonly int HashCount;

        private Thread captureThread;
        private SHA512Managed sha;
        private FileInfo saveFile;
        private FileInfo[] sourceFiles;

        private bool capturing;
        public bool Capturing
        {
            get
            {
                return capturing;
            }
        }

        public MediaRandom(string[] sourcePaths, string savePath)
        {
            capturing = false;
            this.saveFile = new FileInfo(savePath);
            this.sourceFiles = new FileInfo[sourcePaths.Length];
            for (int i = 0; i < sourcePaths.Length; i++)
            {
                sourceFiles[i] = new FileInfo(sourcePaths[i]);
            }
            sha = new SHA512Managed();
            captureThread = new Thread(new ThreadStart(this.BuildThreadMethod));

            HashCount = TargetSize / (sha.HashSize / 8);
            //bWriter = new BinaryWriter(File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read));

        }

        public void CaptureStart()
        {
            if (!capturing)
            {
                capturing = true;
                captureThread.Start();
            }
        }

        public void CaptureStop()
        {
            if (capturing)
            {
                capturing = false;
                captureThread.Join();
            }
        }

        private void BuildThreadMethod()
        {
            long totalSize = 0;
            long maxTotalSize = MaxStreamSizePerHash * (TargetSize / ((long)sha.HashSize / 8L));
            long minTotalSize = MinStreamSizePerHash * (TargetSize / ((long)sha.HashSize / 8L));
            double trunc = 1d - HeadTruncateSize - TailTruncateSize;

            for (int i = 0; i < sourceFiles.Length && totalSize < maxTotalSize; i++)
            {
                totalSize += (long)((double)sourceFiles[i].Length * trunc);
            }
            if (totalSize > maxTotalSize)
            {
                totalSize = maxTotalSize;
            }
            else if (totalSize < minTotalSize)
            {
                throw new Exception("not enough data");
            }
            long streamSize = totalSize / HashCount - 1L;
            byte[] concatStream = new byte[streamSize];

            int streamIdx = 0;
            int sourceFileIdx = 0;
            long sourceStartIdx = (long)((double)sourceFiles[sourceFileIdx].Length * HeadTruncateSize);
            long sourceEndIdx = (long)((double)sourceFiles[sourceFileIdx].Length * (1d - TailTruncateSize));
            BinaryReader br = new BinaryReader(sourceFiles[sourceFileIdx].Open(FileMode.Open, FileAccess.Read, FileShare.Read));
            br.BaseStream.Seek(sourceStartIdx, SeekOrigin.Begin);

            using (BinaryWriter bw = new BinaryWriter(saveFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)))
            {
                for (int i = 0; i < HashCount; i++)
                {
                    streamIdx = 0;

                    bool reading = true;
                    while (reading)
                    {
                        //reading = true;
                        long readLength = sourceEndIdx - sourceStartIdx;
                        if (readLength > streamSize - streamIdx)
                        {
                            readLength = streamSize - streamIdx;
                            reading = false;
                        }

                        br.Read(concatStream, streamIdx, (int)readLength);
                        sourceStartIdx += readLength;
                        streamIdx += (int)readLength;

                        if (reading)
                        {
                            sourceFileIdx++;
                            br.Dispose();
                            br = new BinaryReader(sourceFiles[sourceFileIdx].Open(FileMode.Open, FileAccess.Read, FileShare.Read));
                            sourceStartIdx = (long)((double)sourceFiles[sourceFileIdx].Length * HeadTruncateSize);
                            sourceEndIdx = (long)((double)sourceFiles[sourceFileIdx].Length * (1d - TailTruncateSize));
                            br.BaseStream.Seek(sourceStartIdx, SeekOrigin.Begin);
                        }
                    }

                    unsafe
                    {
                        fixed (byte* p = concatStream)
                        {
                            *(int*)p = i;
                        }
                    }

                    byte[] hash = sha.ComputeHash(concatStream);
                    bw.Write(hash);
                    bw.Flush();
                }
            }
            br.Dispose();
            br = null;

            capturing = false;
        }

        public void Dispose()
        {
            CaptureStop();
            sha.Dispose();
        }

        ~MediaRandom()
        {
            Dispose();
        }
    }
}
