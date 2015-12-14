using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace EthereumThrottle
{
    public partial class MainForm : Form
    {
        private const int LogSize = 1024 * 1024;

        private Process ethminer;
        private StringBuilder OutBuffer;
        private DataReceivedEventHandler dEthminerDataRecieved;

        private Thread monitor;
        private bool monitorOn;

        [DllImport("Atiadlxx.dll")]
        internal static extern int ADL2_Main_Control_Create(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters, out IntPtr context);

        [DllImport("Atiadlxx.dll")]
        internal static extern int ADL2_Main_Control_Destroy(IntPtr context);

        [DllImport("Atiadlxx.dll")]
        internal static extern int ADL2_Overdrive6_Temperature_Get(IntPtr context, int iAdapterIndex, out IntPtr lpTemperature);

        [DllImport("Atiadlxx.dll")]
        internal static extern int ADL2_Adapter_AdapterInfoX2_Get(IntPtr context, out ADLAdapterInfoArray lpInfo);

        #region ADLAdapterInfo
        internal const int ADL_MAX_PATH = 256;

        [StructLayout(LayoutKind.Sequential)]
        internal struct ADLAdapterInfo
        {
            int Size;
            internal int AdapterIndex;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL_MAX_PATH)]
            internal string UDID;
            internal int BusNumber;
            internal int DriverNumber;
            internal int FunctionNumber;
            internal int VendorID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL_MAX_PATH)]
            internal string AdapterName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL_MAX_PATH)]
            internal string DisplayName;
            internal int Present;
            internal int Exist;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL_MAX_PATH)]
            internal string DriverPath;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL_MAX_PATH)]
            internal string DriverPathExt;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)ADL_MAX_PATH)]
            internal string PNPString;
            internal int OSDisplayIndex;
        }


        internal const int ADL_MAX_ADAPTERS = 40 /* 150 */;

        [StructLayout(LayoutKind.Sequential)]
        internal struct ADLAdapterInfoArray
        {
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)ADL_MAX_ADAPTERS)]
            [MarshalAs(UnmanagedType.ByValArray)]
            internal ADLAdapterInfo[] ADLAdapterInfo;
        }
        #endregion ADLAdapterInfo

        public MainForm()
        {
            InitializeComponent();

            OutBuffer = new StringBuilder(LogSize);
            dEthminerDataRecieved = EthminerDataReceived;
        }

        private void MonitorStart()
        {
            int returnCode = 0;

            IntPtr context = IntPtr.Zero;
            returnCode = ADL2_Main_Control_Create(d_ADL_Main_Memory_Alloc, 1, out context);

            ADLAdapterInfoArray ai = new ADLAdapterInfoArray();
            returnCode = ADL2_Adapter_AdapterInfoX2_Get(context, out ai);

            while (monitorOn && ethminer != null && !ethminer.HasExited)
            {
                int maxTemp = 0;
                do
                {
                    //try
                    //{
                    //    MessageBox.Show("go");
                    //    char[] buffer = new char[256];
                    //    int len = ethminer.StandardOutput.Read(buffer, 0, buffer.Length);
                    //    if (len > 0)
                    //    {
                    //        txtOut.Text = len.ToString();
                    //        this.BeginInvoke(
                    //            new DataReceivedEventHandler(EthminerDataReceived),
                    //            new object[] { new String(buffer, 0, len), (DataReceivedEventArgs)null });
                    //    }
                    //    MessageBox.Show("len: " + len);

                    //}
                    //catch (Exception ee) {
                    //    MessageBox.Show(ee.Message);
                    //}
                    Thread.Sleep(500);
                    maxTemp = GetMaxTemperature(context);
                } while (maxTemp < 85000 && monitorOn);

                if (monitorOn && ethminer != null && !ethminer.HasExited)
                {
                    ethminer.SuspendProcess();

                    do
                    {
                        Thread.Sleep(500);
                        maxTemp = GetMaxTemperature(context);
                    } while (maxTemp >= 80000 && maxTemp <= 91000 && monitorOn);

                    if (maxTemp >= 91000)
                    {
                        this.BeginInvoke(new EventHandler(btnStart_Click), null, null);
                        do
                        {
                            Thread.Sleep(1000);
                            maxTemp = GetMaxTemperature(context);
                        } while (maxTemp >= 80000);
                        this.BeginInvoke(new EventHandler(btnStart_Click), null, null);
                    }

                    if (ethminer != null && !ethminer.HasExited)
                        ethminer.ResumeProcess();
                }
            }

            returnCode = ADL2_Main_Control_Destroy(context);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (ethminer != null)
            {
                monitorOn = false;
                if (!ethminer.HasExited)
                {
                    //ethminer.CancelOutputRead();
                    ethminer.CloseMainWindow();
                    //ethminer.StandardInput.Write('\x3');
                    //ethminer.StandardInput.Flush();
                    //ethminer.StandardInput.Close();
                    ethminer.WaitForExit(20000);
                    if (!ethminer.HasExited)
                        ethminer.Kill();
                }
                ethminer.Dispose();
                ethminer = null;
                btnStart.Text = "Start";

                if (monitor != null && monitor.IsAlive)
                    monitor.Join(11000);
                return;
            }

            ethminer = new Process();
            //ethminer.OutputDataReceived += new DataReceivedEventHandler(EthminerDataReceived);
            //ethminer.ErrorDataReceived += dEthminerDataRecieved;

            ethminer.StartInfo.FileName = txtFileLocation.Text;
            ethminer.StartInfo.Arguments = "-F http://eth-us.suprnova.cc:3000/finibhire.Charon0/1 -G --farm-recheck 200 --opencl-device 0 --cl-extragpu-mem 1892 -v 9";
            ethminer.StartInfo.UseShellExecute = false;
            //ethminer.StartInfo.RedirectStandardInput = true;
            ethminer.StartInfo.RedirectStandardOutput = true;
            //ethminer.StartInfo.RedirectStandardError = true;
            //ethminer.StartInfo.CreateNoWindow = true;
            //ethminer.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ethminer.Start();
            //ethminer.BeginOutputReadLine();
            //ethminer.BeginErrorReadLine();
            btnStart.Text = "Stop";

            Thread.Sleep(500);
            ethminer.SuspendProcess();
            EditEthMinerMemory();
            ethminer.ResumeProcess();

            monitorOn = true;
            monitor = new Thread(new ThreadStart(MonitorStart));
            monitor.Start();
        }

        private void EthminerDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                DataReceivedEventHandler proc = EthminerDataReceived;
                this.BeginInvoke(proc, new object[] { sender, e });
                return;
            }

            string data = (sender != null && sender.GetType() == typeof(string)) ? (string)sender : e.Data;

            int newSize = OutBuffer.Length + data.Length;
            if (newSize > LogSize)
            {
                OutBuffer.Remove(0, data.Length);
            }
            OutBuffer.Append(data);

            txtOut.Text = OutBuffer.ToString();
            txtOut.SelectionStart = OutBuffer.Length;
            txtOut.ScrollToCaret();
        }

        private void btnTestRead_Click(object sender, EventArgs e)
        {
            if (monitorOn)
            {
                monitorOn = false;
                if (monitor != null && monitor.IsAlive)
                    monitor.Join(11000);
                btnTestRead.Text = "Monitor";
            }
            else if (ethminer != null && !ethminer.HasExited)
            {
                monitorOn = true;
                monitor = new Thread(new ThreadStart(MonitorStart));
                monitor.Start();
                btnTestRead.Text = "Stop Monitoring";
            }
        }

        private int GetMaxTemperature(IntPtr context)
        {
            IntPtr temperature = IntPtr.Zero;
            int returnCode;
            int i = 0;
            int maxTemp = 0;
            do
            {
                returnCode = ADL2_Overdrive6_Temperature_Get(context, i, out temperature);
                if ((int)temperature > maxTemp)
                    maxTemp = (int)temperature;
                i++;
            } while (returnCode == 0);

            //MessageBox.Show("Temperature: " + maxTemp + "    Return Code: " + returnCode);
            if (i == 1)
            {
                return int.MaxValue;
            }
            return maxTemp;
        }

        #region ADL_Main_Memory_Alloc
        internal delegate IntPtr ADL_Main_Memory_Alloc(int size);
        internal static ADL_Main_Memory_Alloc d_ADL_Main_Memory_Alloc = ADL_Main_Memory_Alloc_;
        private static IntPtr ADL_Main_Memory_Alloc_(int size)
        {
            IntPtr result = Marshal.AllocCoTaskMem(size);
            return result;
        }
        #endregion ADL_Main_Memory_Alloc

        #region ADL_Main_Memory_Free
        internal static void ADL_Main_Memory_Free(IntPtr buffer)
        {
            if (IntPtr.Zero != buffer)
            {
                Marshal.FreeCoTaskMem(buffer);
            }
        }
        #endregion ADL_Main_Memory_Free

        private void EditEthMinerMemory()
        {
            //int access = Kernel32.PROCESS_VM_READ | Kernel32.PROCESS_VM_WRITE | Kernel32.PROCESS_VM_OPERATION;
            int access = Kernel32.PROCESS_ALL_ACCESS;
            IntPtr processHandle = Kernel32.OpenProcess(access, false, ethminer.Id);

            IntPtr memAddress = (IntPtr)0x18B05;
            foreach (ProcessModule mod in ethminer.Modules)
            {
                txtOut.Text += mod.FileName + ": " + mod.BaseAddress + Environment.NewLine;
                if (mod.FileName.EndsWith("ethminer.exe", true, null))
                {
                    memAddress = IntPtr.Add(mod.BaseAddress, 0x00077B05);
                    break;
                }
            }
            //MessageBox.Show("Works!");

            IntPtr bytesRead = IntPtr.Zero;
            byte[] buffer = new byte[1];

            if (!Kernel32.ReadProcessMemory(processHandle, memAddress, buffer, (UIntPtr)buffer.Length, out bytesRead))
            {
                MessageBox.Show("Can't Read: " + Kernel32.GetLastError().ToString());
                return;
            }
            //MessageBox.Show("Works!");

            Kernel32.MEMORY_BASIC_INFORMATION m;
            UIntPtr result = Kernel32.VirtualQueryEx(processHandle, memAddress, out m, (IntPtr)Marshal.SizeOf(typeof(Kernel32.MEMORY_BASIC_INFORMATION)));

            //MessageBox.Show("Works!");
            uint oldProtect = 0;
            if (!Kernel32.VirtualProtectEx(processHandle, m.BaseAddress, m.RegionSize, Kernel32.PAGE_READWRITE, out oldProtect))
            {
                MessageBox.Show("Can't Change Protect: " + Kernel32.GetLastError().ToString());
                return;
            }
            //MessageBox.Show("Works!");

            if (buffer[0] == (byte)'h')
            {
                IntPtr bytesWritten = IntPtr.Zero;
                buffer[0] = (byte)'i';
                if (!Kernel32.WriteProcessMemory(processHandle, memAddress, buffer, (UIntPtr)buffer.Length, out bytesWritten))
                {
                    MessageBox.Show("Can't Write: " + Kernel32.GetLastError().ToString());
                    return;
                }
                //MessageBox.Show("Works!");

                if (!Kernel32.VirtualProtectEx(processHandle, m.BaseAddress, m.RegionSize, Kernel32.PAGE_READONLY, out oldProtect))
                {
                    MessageBox.Show("Can't Change Protect to READONLY: " + Kernel32.GetLastError().ToString());
                    return;
                }

                //MessageBox.Show("Works!");
            }
            else
            {
                MessageBox.Show("Fail :(");
            }

            Kernel32.CloseHandle(processHandle);
        }

    }

    static class ProcessExentionMethods
    {

        public static void SuspendProcess(this Process process)
        {
            //var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = Kernel32.OpenThread(Kernel32.ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                Kernel32.SuspendThread(pOpenThread);

                Kernel32.CloseHandle(pOpenThread);
            }
        }

        public static void ResumeProcess(this Process process)
        {
            //var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = Kernel32.OpenThread(Kernel32.ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = Kernel32.ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                Kernel32.CloseHandle(pOpenThread);
            }
        }
    }
}

