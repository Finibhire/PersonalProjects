using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Cryptography;

namespace TrueRandomBitGenerator
{
    public partial class Main : Form
    {
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate void UpdateEvent();

        public const int WH_MOUSE = 7;
        public const int WH_MOUSE_LL = 14;
        public const int MOVEMENTS_PER_HASH = 256;

        private static IntPtr hHook = IntPtr.Zero;
        private static BinaryWriter dataFile;
        //private static StreamWriter testFile;
        private static byte[] mouseData = new byte[MOVEMENTS_PER_HASH * sizeof(int) * 2];
        private static int mouseDataIdx = 0;
        private static SHA256Managed sha = new SHA256Managed();
        private static int bitsWritten = 0;
        private static Main tMain;

        private HookProc MouseHookProcedure;
        private UpdateEvent BitsWrittenUpdatedMethod;

        private ScreenCaptureRandom scr;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public Main()
        {
            if (tMain != null)
                throw new Exception();

            tMain = this;
            BitsWrittenUpdatedMethod = this.UpdateBitsWritten;
            InitializeComponent();
        }

        private void btnChangeFile_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            tbFileLocation.Text = saveFileDialog1.FileName;
        }

        private void btnAppend_Click(object sender, EventArgs e)
        {
            if (hHook == IntPtr.Zero)
            {
                bool validPath = false;
                try
                {
                    tbFileLocation.Text = Path.GetFullPath(tbFileLocation.Text);
                    if (!String.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(tbFileLocation.Text)))
                    {
                        validPath = true;
                    }
                }
                catch {}

                if ( validPath )
                {
                    MouseHookProcedure = new HookProc(Main.MouseHookCallback);

                    using (Process curProcess = Process.GetCurrentProcess())
                    using (ProcessModule curModule = curProcess.MainModule)
                    {
                        hHook = SetWindowsHookEx(WH_MOUSE_LL,
                                                    MouseHookProcedure,
                                                    GetModuleHandle(curModule.ModuleName),
                                                    0);
                    }

                    if (hHook == IntPtr.Zero)
                    {
                        MessageBox.Show("Could not set windows hook.");
                        return;
                    }
                    
                    dataFile = new BinaryWriter(File.Open(tbFileLocation.Text, FileMode.Append));
                    //testFile = new StreamWriter(File.Open(tbFileLocation.Text, FileMode.Append));

                    btnAppend.Text = TrueRandomBitGenerator.Properties.Resources.btnAppend_StopText;
                }
            }
            else
            {
                dataFile.Close();
                //testFile.Close();

                bool ret = UnhookWindowsHookEx(hHook);
                if (!ret)
                {
                    MessageBox.Show("Unhook Windows Hook Failed.");
                    return;
                }
                hHook = IntPtr.Zero;

                btnAppend.Text = TrueRandomBitGenerator.Properties.Resources.btnAppend_StartText;
            }
        }

        public static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (MouseMessages)wParam == MouseMessages.WM_MOUSEMOVE)
            {
                MSLLHOOKSTRUCT mouseParamData = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                //testFile.WriteAsync(Environment.NewLine + mouseParamData.pt.x + ", " + mouseParamData.pt.y);

                unsafe 
                {
                    fixed (byte* p1 = mouseData)
                    {
                        *(int*)(p1 + mouseDataIdx) = mouseParamData.pt.x;
                        mouseDataIdx += sizeof(int);
                        *(int*)(p1 + mouseDataIdx) = mouseParamData.pt.y;
                        mouseDataIdx += sizeof(int);
                    }
                }
                if (mouseDataIdx == MOVEMENTS_PER_HASH * sizeof(int) * 2)
                {
                    mouseDataIdx = 0;
                    dataFile.Write(sha.ComputeHash(mouseData));
                    bitsWritten += 256;  //sha256
                    tMain.Invoke(tMain.BitsWrittenUpdatedMethod);
                }
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }
        
        private void UpdateBitsWritten()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(BitsWrittenUpdatedMethod);
                return;
            }

            lblDataCollected.Text = bitsWritten + " Bits";
        }

        private void btnCaptureGraphics_Click(object sender, EventArgs e)
        {
            if (scr != null && scr.Capturing)
            {
                scr.CaptureStop();
                btnCaptureGraphics.Text = "Start Graph";
            }
            else
            {
                scr = new ScreenCaptureRandom(@"E:\Screen.data");
                scr.CaptureStart();
                btnCaptureGraphics.Text = "Stop Graph";
            }
        }

        private MediaRandom mediaRNG;
        private void btnMedia_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.ShowDialog();
            mediaRNG = new MediaRandom(ofd.FileNames, "E:\\MediaDump.data");
            mediaRNG.CaptureStart();
        }
    }

    public enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
