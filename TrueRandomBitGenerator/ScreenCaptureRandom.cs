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
    class ScreenCaptureRandom : IDisposable
    {
        public const int xMin = 450, xMax = 550, yMin = 350, yMax = 450, CaptureFrequency = 281;

        private Thread captureThread;
        private SHA512Managed sha;
        private string outFilePath;

        private bool capturing;
        public bool Capturing
        {
            get
            {
                return capturing;
            }
        }

        public ScreenCaptureRandom(string path)
        {
            capturing = false;
            outFilePath = path;
            sha = new SHA512Managed();
            captureThread = new Thread(new ThreadStart(this.CaptureThreadMethod));
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

        private void CaptureThreadMethod()
        {
            byte[] lastHash = new byte[512 / 8], hash;

            using (BinaryWriter bWriter = new BinaryWriter(File.Open(outFilePath, FileMode.Append, FileAccess.Write, FileShare.Read)))
            using (Bitmap bmpScreenCapture = new Bitmap(xMax - xMin, yMax - yMin))
            using (Graphics g = Graphics.FromImage(bmpScreenCapture))
            using (MemoryStream ms = new MemoryStream())
            {
                while (capturing)
                {
                    Thread.Sleep(CaptureFrequency);

                    g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X + xMin,
                                        Screen.PrimaryScreen.Bounds.Y + yMin,
                                        0, 0,
                                        bmpScreenCapture.Size,
                                        CopyPixelOperation.SourceCopy);
                    bmpScreenCapture.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    hash = sha.ComputeHash(ms);

                    bool eq = true;
                    unsafe
                    {
                        fixed (byte* lh = lastHash)
                        fixed (byte* h = hash)
                        {
                            int* plh = (int*)lh;
                            int* ph = (int*)h;
                            int* plhBail = (int*)(lh + (512 / 8));
                            while (plh < plhBail)
                            {
                                if (*plh != *ph)
                                {
                                    eq = false;
                                    break;
                                }
                                plh++;
                                ph++;
                            }
                        }
                    }

                    if (!eq)
                    {
                        bWriter.Write(hash);
                        lastHash = hash;
                    }
                }
            }
        }

        public void Dispose()
        {
            CaptureStop();
            sha.Dispose();
        }

        ~ScreenCaptureRandom()
        {
            Dispose();
        }
    }
}
