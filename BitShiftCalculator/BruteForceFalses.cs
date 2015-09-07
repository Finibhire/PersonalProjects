using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace BitShiftCalculator
{
    public partial class BruteForceFalses : Form
    {
        public BruteForceFalses()
        {
            InitializeComponent();
        }

        private void btnBuildCode_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("const int ");
            {
                int i = 0;
                while (i < numBits.Value)
                {
                    sb.Append("    ");
                    for (int j = 0; j < 10 && i < numBits.Value; j++)
                    {
                        sb.Append("v");
                        sb.AppendID(i, 2);
                        sb.Append(" = ");
                        sb.AppendID(i, 2);
                        sb.Append(", ");
                        i++;
                    }
                    sb.Length -= 1;
                    sb.AppendLine();
                }
            }
            sb.Length -= 3;
            sb.AppendLine(";");
            sb.AppendLine();


            int DataSize = (int)numBits.Value / 2;
            for (int i = 0; i < DataSize; i++)
            {
                int termCount = i + 1;
                sb.Append("new ulong[] {(1uL << v");
                for (int j = 0; j < termCount; j++)
                {
                    //sb.Append((char)(97 + 2 * (ii - jj)));
                    sb.AppendID(j, 2);
                    sb.Append(")|(1uL << v");
                    //sb.Append((char)(97 + 2 * jj + 1));
                    sb.AppendID(DataSize + i - j, 2);
                    sb.Append("), (1uL << v");
                }
                sb.Length -= 10;
                sb.AppendLine("},");
            }
            for (int i = DataSize; i < DataSize * 2; i++)
            {
                int termCount = DataSize * 2 - i - 1;
                sb.Append("new ulong[] {(1uL << v");
                for (int j = 0; j < termCount; j++)
                {
                    //sb.Append((char)(97 + 2 * (ii - jj)));
                    sb.AppendID(i - DataSize + j + 1, 2);
                    sb.Append(")|(1uL << v");
                    //sb.Append((char)(97 + 2 * jj + 1));
                    sb.AppendID(DataSize * 2 - 1 - j, 2);
                    sb.Append("), (1uL << v");
                }
                sb.Length -= 10;
                sb.AppendLine("},");
            }
            sb.Length -= 1;

            tbOut.Text = sb.ToString();
        }

        /// <summary>
        /// Used for debuging.  Returns an output exactly the same as the encryption algorithim would produce
        /// given the collected data about XOR's and AND operations so far.
        /// </summary>
        private bool TestPartialEncryption(uint[] unencryptedData, uint[] semiEncyptedData, SortedDictionary<ANDCombination, object>[] operations)
        {
            uint[] result = new uint[unencryptedData.Length];
            for (int i = 0; i < operations.Length; i++)
            {
                bool state = false;
                foreach (var a in operations[i])
                {
                    if (unencryptedData.GetBitState(a.Key.FirstVarID - 1) && unencryptedData.GetBitState(a.Key.SecondVarID - 1))
                    {
                        state = !state;
                    }
                }
                if (state)
                {
                    result[i / 32] |= 1u << (i % 32);
                }
            }
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != semiEncyptedData[i])
                {
                    return false;
                }
            }
            return true;
        }

        private void AppendStructure(StringBuilder sb, SortedDictionary<ANDCombination, object>[] xorEq)
        {
            const int varIDlength = 2;

            for (int i = 0; i < xorEq.Length; i++)
            {
                sb.Append(i.ToString().PadLeft(2));
                sb.Append(":  ");
                for (int j = 0; j < xorEq[i].Count; j++)
                {
                    var t = xorEq[i].ElementAt(j).Key;
                    sb.AppendID(t.FirstVarID, varIDlength);
                    sb.Append('.');
                    sb.AppendID(t.SecondVarID, varIDlength);
                    sb.Append(" | ");
                }
                sb.Length = sb.Length - 2;
                sb.AppendLine();
            }
        }

        private void btnStructure_Click(object sender, EventArgs e)
        {
            int DATASIZE = (int)numBits.Value;
            //string rawData = "000073af00000000";
            string rawData = "000000b5";
            //string rawData = "73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af";
            uint[] aa = new uint[(DATASIZE / 16 > 0) ? DATASIZE / 16 : 1];
            uint[] bb = new uint[(DATASIZE / 16 > 0) ? DATASIZE / 16 : 1];
            BitHolder32[] a = new BitHolder32[(DATASIZE / 16 > 0) ? DATASIZE / 16 : 1];
            XORCollector32Bit[] b = new XORCollector32Bit[(DATASIZE / 16 > 0) ? DATASIZE / 16 : 1];
            for (int i = 0; i < ((DATASIZE / 16 > 0) ? DATASIZE / 16 : 1); i++)
            {
                a[i] = new BitHolder32(i * 32);
                b[i] = new XORCollector32Bit();
                aa[i] = uint.Parse(rawData.Substring(i * 8, 8), System.Globalization.NumberStyles.HexNumber);
            }

            SortedDictionary<ANDCombination, object>[] xorEq = new SortedDictionary<ANDCombination, object>[DATASIZE * 2];
            for (int i = 0; i < xorEq.Length; i++)
            {
                xorEq[i] = new SortedDictionary<ANDCombination, object>();
            }

            for (int i = 0; i < DATASIZE; i++)
            {
                for (int j = 0; j < DATASIZE; j++)
                {
                    bb[(i + j) / 32] ^= (((aa[i / 32] >> (i % 32)) & (aa[j / 32 + DATASIZE / 32] >> (j + DATASIZE % 32)) & 1) << ((i + j) % 32));
                    ANDCombination andc = new ANDCombination(i + 1, j + DATASIZE + 1);
                    xorEq[(i + j)][andc] = null;
                    //if (!TestPartialEncryption(aa, bb, xorEq))
                    //    throw new Exception("You messed up!");
                }
            }

            StringBuilder sb = new StringBuilder();
            AppendStructure(sb, xorEq);

            sb.AppendLine();
            sb.AppendLine();
            bb.AppendHex(sb);
            sb.AppendLine();

            tbOut.Text = sb.ToString();
        }

        const int
            v00 = 00, v01 = 01, v02 = 02, v03 = 03, v04 = 04, v05 = 05, v06 = 06, v07 = 07, v08 = 08, v09 = 09,
            v10 = 10, v11 = 11, v12 = 12, v13 = 13, v14 = 14, v15 = 15, v16 = 16, v17 = 17, v18 = 18, v19 = 19,
            v20 = 20, v21 = 21, v22 = 22, v23 = 23, v24 = 24, v25 = 25, v26 = 26, v27 = 27, v28 = 28, v29 = 29,
            v30 = 30, v31 = 31;

        private static readonly ulong[][] AndsList =
            new ulong[][] {

            new ulong[] {(1uL << v00)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v17), (1uL << v01)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v18), (1uL << v01)|(1uL << v17), (1uL << v02)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v19), (1uL << v01)|(1uL << v18), (1uL << v02)|(1uL << v17), (1uL << v03)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v20), (1uL << v01)|(1uL << v19), (1uL << v02)|(1uL << v18), (1uL << v03)|(1uL << v17), (1uL << v04)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v21), (1uL << v01)|(1uL << v20), (1uL << v02)|(1uL << v19), (1uL << v03)|(1uL << v18), (1uL << v04)|(1uL << v17), (1uL << v05)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v22), (1uL << v01)|(1uL << v21), (1uL << v02)|(1uL << v20), (1uL << v03)|(1uL << v19), (1uL << v04)|(1uL << v18), (1uL << v05)|(1uL << v17), (1uL << v06)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v23), (1uL << v01)|(1uL << v22), (1uL << v02)|(1uL << v21), (1uL << v03)|(1uL << v20), (1uL << v04)|(1uL << v19), (1uL << v05)|(1uL << v18), (1uL << v06)|(1uL << v17), (1uL << v07)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v24), (1uL << v01)|(1uL << v23), (1uL << v02)|(1uL << v22), (1uL << v03)|(1uL << v21), (1uL << v04)|(1uL << v20), (1uL << v05)|(1uL << v19), (1uL << v06)|(1uL << v18), (1uL << v07)|(1uL << v17), (1uL << v08)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v25), (1uL << v01)|(1uL << v24), (1uL << v02)|(1uL << v23), (1uL << v03)|(1uL << v22), (1uL << v04)|(1uL << v21), (1uL << v05)|(1uL << v20), (1uL << v06)|(1uL << v19), (1uL << v07)|(1uL << v18), (1uL << v08)|(1uL << v17), (1uL << v09)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v26), (1uL << v01)|(1uL << v25), (1uL << v02)|(1uL << v24), (1uL << v03)|(1uL << v23), (1uL << v04)|(1uL << v22), (1uL << v05)|(1uL << v21), (1uL << v06)|(1uL << v20), (1uL << v07)|(1uL << v19), (1uL << v08)|(1uL << v18), (1uL << v09)|(1uL << v17), (1uL << v10)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v27), (1uL << v01)|(1uL << v26), (1uL << v02)|(1uL << v25), (1uL << v03)|(1uL << v24), (1uL << v04)|(1uL << v23), (1uL << v05)|(1uL << v22), (1uL << v06)|(1uL << v21), (1uL << v07)|(1uL << v20), (1uL << v08)|(1uL << v19), (1uL << v09)|(1uL << v18), (1uL << v10)|(1uL << v17), (1uL << v11)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v28), (1uL << v01)|(1uL << v27), (1uL << v02)|(1uL << v26), (1uL << v03)|(1uL << v25), (1uL << v04)|(1uL << v24), (1uL << v05)|(1uL << v23), (1uL << v06)|(1uL << v22), (1uL << v07)|(1uL << v21), (1uL << v08)|(1uL << v20), (1uL << v09)|(1uL << v19), (1uL << v10)|(1uL << v18), (1uL << v11)|(1uL << v17), (1uL << v12)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v29), (1uL << v01)|(1uL << v28), (1uL << v02)|(1uL << v27), (1uL << v03)|(1uL << v26), (1uL << v04)|(1uL << v25), (1uL << v05)|(1uL << v24), (1uL << v06)|(1uL << v23), (1uL << v07)|(1uL << v22), (1uL << v08)|(1uL << v21), (1uL << v09)|(1uL << v20), (1uL << v10)|(1uL << v19), (1uL << v11)|(1uL << v18), (1uL << v12)|(1uL << v17), (1uL << v13)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v30), (1uL << v01)|(1uL << v29), (1uL << v02)|(1uL << v28), (1uL << v03)|(1uL << v27), (1uL << v04)|(1uL << v26), (1uL << v05)|(1uL << v25), (1uL << v06)|(1uL << v24), (1uL << v07)|(1uL << v23), (1uL << v08)|(1uL << v22), (1uL << v09)|(1uL << v21), (1uL << v10)|(1uL << v20), (1uL << v11)|(1uL << v19), (1uL << v12)|(1uL << v18), (1uL << v13)|(1uL << v17), (1uL << v14)|(1uL << v16),},
            new ulong[] {(1uL << v00)|(1uL << v31), (1uL << v01)|(1uL << v30), (1uL << v02)|(1uL << v29), (1uL << v03)|(1uL << v28), (1uL << v04)|(1uL << v27), (1uL << v05)|(1uL << v26), (1uL << v06)|(1uL << v25), (1uL << v07)|(1uL << v24), (1uL << v08)|(1uL << v23), (1uL << v09)|(1uL << v22), (1uL << v10)|(1uL << v21), (1uL << v11)|(1uL << v20), (1uL << v12)|(1uL << v19), (1uL << v13)|(1uL << v18), (1uL << v14)|(1uL << v17), (1uL << v15)|(1uL << v16),},
            new ulong[] {(1uL << v01)|(1uL << v31), (1uL << v02)|(1uL << v30), (1uL << v03)|(1uL << v29), (1uL << v04)|(1uL << v28), (1uL << v05)|(1uL << v27), (1uL << v06)|(1uL << v26), (1uL << v07)|(1uL << v25), (1uL << v08)|(1uL << v24), (1uL << v09)|(1uL << v23), (1uL << v10)|(1uL << v22), (1uL << v11)|(1uL << v21), (1uL << v12)|(1uL << v20), (1uL << v13)|(1uL << v19), (1uL << v14)|(1uL << v18), (1uL << v15)|(1uL << v17),},
            new ulong[] {(1uL << v02)|(1uL << v31), (1uL << v03)|(1uL << v30), (1uL << v04)|(1uL << v29), (1uL << v05)|(1uL << v28), (1uL << v06)|(1uL << v27), (1uL << v07)|(1uL << v26), (1uL << v08)|(1uL << v25), (1uL << v09)|(1uL << v24), (1uL << v10)|(1uL << v23), (1uL << v11)|(1uL << v22), (1uL << v12)|(1uL << v21), (1uL << v13)|(1uL << v20), (1uL << v14)|(1uL << v19), (1uL << v15)|(1uL << v18),},
            new ulong[] {(1uL << v03)|(1uL << v31), (1uL << v04)|(1uL << v30), (1uL << v05)|(1uL << v29), (1uL << v06)|(1uL << v28), (1uL << v07)|(1uL << v27), (1uL << v08)|(1uL << v26), (1uL << v09)|(1uL << v25), (1uL << v10)|(1uL << v24), (1uL << v11)|(1uL << v23), (1uL << v12)|(1uL << v22), (1uL << v13)|(1uL << v21), (1uL << v14)|(1uL << v20), (1uL << v15)|(1uL << v19),},
            new ulong[] {(1uL << v04)|(1uL << v31), (1uL << v05)|(1uL << v30), (1uL << v06)|(1uL << v29), (1uL << v07)|(1uL << v28), (1uL << v08)|(1uL << v27), (1uL << v09)|(1uL << v26), (1uL << v10)|(1uL << v25), (1uL << v11)|(1uL << v24), (1uL << v12)|(1uL << v23), (1uL << v13)|(1uL << v22), (1uL << v14)|(1uL << v21), (1uL << v15)|(1uL << v20),},
            new ulong[] {(1uL << v05)|(1uL << v31), (1uL << v06)|(1uL << v30), (1uL << v07)|(1uL << v29), (1uL << v08)|(1uL << v28), (1uL << v09)|(1uL << v27), (1uL << v10)|(1uL << v26), (1uL << v11)|(1uL << v25), (1uL << v12)|(1uL << v24), (1uL << v13)|(1uL << v23), (1uL << v14)|(1uL << v22), (1uL << v15)|(1uL << v21),},
            new ulong[] {(1uL << v06)|(1uL << v31), (1uL << v07)|(1uL << v30), (1uL << v08)|(1uL << v29), (1uL << v09)|(1uL << v28), (1uL << v10)|(1uL << v27), (1uL << v11)|(1uL << v26), (1uL << v12)|(1uL << v25), (1uL << v13)|(1uL << v24), (1uL << v14)|(1uL << v23), (1uL << v15)|(1uL << v22),},
            new ulong[] {(1uL << v07)|(1uL << v31), (1uL << v08)|(1uL << v30), (1uL << v09)|(1uL << v29), (1uL << v10)|(1uL << v28), (1uL << v11)|(1uL << v27), (1uL << v12)|(1uL << v26), (1uL << v13)|(1uL << v25), (1uL << v14)|(1uL << v24), (1uL << v15)|(1uL << v23),},
            new ulong[] {(1uL << v08)|(1uL << v31), (1uL << v09)|(1uL << v30), (1uL << v10)|(1uL << v29), (1uL << v11)|(1uL << v28), (1uL << v12)|(1uL << v27), (1uL << v13)|(1uL << v26), (1uL << v14)|(1uL << v25), (1uL << v15)|(1uL << v24),},
            new ulong[] {(1uL << v09)|(1uL << v31), (1uL << v10)|(1uL << v30), (1uL << v11)|(1uL << v29), (1uL << v12)|(1uL << v28), (1uL << v13)|(1uL << v27), (1uL << v14)|(1uL << v26), (1uL << v15)|(1uL << v25),},
            new ulong[] {(1uL << v10)|(1uL << v31), (1uL << v11)|(1uL << v30), (1uL << v12)|(1uL << v29), (1uL << v13)|(1uL << v28), (1uL << v14)|(1uL << v27), (1uL << v15)|(1uL << v26),},
            new ulong[] {(1uL << v11)|(1uL << v31), (1uL << v12)|(1uL << v30), (1uL << v13)|(1uL << v29), (1uL << v14)|(1uL << v28), (1uL << v15)|(1uL << v27),},
            new ulong[] {(1uL << v12)|(1uL << v31), (1uL << v13)|(1uL << v30), (1uL << v14)|(1uL << v29), (1uL << v15)|(1uL << v28),},
            new ulong[] {(1uL << v13)|(1uL << v31), (1uL << v14)|(1uL << v30), (1uL << v15)|(1uL << v29),},
            new ulong[] {(1uL << v14)|(1uL << v31), (1uL << v15)|(1uL << v30),},
            new ulong[] {(1uL << v15)|(1uL << v31),}
            };

        

        private void btnFalseTester_Click(object sender, EventArgs e)
        {
            var thread = new Thread(new ThreadStart(BruteForceFailCheck));
            thread.Priority = ThreadPriority.Highest;
            lock (ProgressThreadLock)
            {
                BitCount = (int)numBits.Value;
                CurrentTTIndex = 0;
                //private ulong[] FalseCounts;
                //private int CurrentProgress = 0;
                //private int OverallProgress = 0;
                //private TimeSpan EndTime;
                //private DateTime startTime;
            }
            thread.Start();
            
        }

        private void BruteForceFailCheck()
        {
            int bitCount;
            lock (ProgressThreadLock)
            {
                bitCount = BitCount;
                CurrentTTIndex = 0;
                FalseCounts = new ulong[bitCount / 2];
                startTime = DateTime.Now;
                OverallProgress = 0;
            }
            ProgressUpdate(null, null);
            int NumOfVar = bitCount;
            ulong TTRows = (ulong)Math.Pow(2, NumOfVar);

            //bool[,] testResults = new bool[TTRows, MaxRow];

            bool runningResult;
            ulong[] falseCount = new ulong[bitCount / 2];

            for (ulong ttRow = 0; ttRow < TTRows; ttRow++)
            {
                runningResult = false;
                int falseCountIndex = 0;
                for (int bitEqIndex = bitCount - 1; bitEqIndex >= bitCount / 2; bitEqIndex--)
                {
                    for (int eqTerm = 0; eqTerm < bitCount - bitEqIndex - 1; eqTerm++)
                    {
                        if ((AndsList[bitEqIndex][eqTerm] & ttRow) == AndsList[bitEqIndex][eqTerm])
                        {
                            runningResult = !runningResult;
                        }
                    }
                    if (runningResult == false)
                    {
                        falseCount[falseCountIndex]++;
                    }
                    falseCountIndex++;
                }
                if (ttRow % 100000 == 0)
                {
                    lock (ProgressThreadLock)
                    {
                        Array.Copy(falseCount, FalseCounts, bitCount / 2);
                        OverallProgress = (int)((double)ttRow / (double)TTRows);
                        double timeSoFar = (DateTime.Now - startTime).TotalSeconds;
                        TimeLeft = TimeSpan.FromSeconds(timeSoFar * TTRows / (ttRow+1) - timeSoFar);
                        CurrentTTIndex = ttRow;
                    }
                    ProgressUpdate(null, null);
                }
            }

            lock (ProgressThreadLock)
            {
                Array.Copy(falseCount, FalseCounts, bitCount / 2);
                CurrentTTIndex = 0;
                CurrentProgress = 100;
                OverallProgress = 100;
            }
            ProgressUpdate(null, null);
        }

        private static readonly object ProgressThreadLock = new object();
        private int BitCount = 0;
        private ulong CurrentTTIndex = 0;
        private ulong[] FalseCounts;
        private int CurrentProgress = 0;
        private int OverallProgress = 0;
        private TimeSpan TimeLeft;
        private DateTime startTime;

        private void ProgressUpdate(object sender, EventArgs ea)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, EventArgs>(ProgressUpdate), sender, ea);
                return;
            }
            
            lock (ProgressThreadLock)
            {
                progressCurrent.Value = CurrentProgress;
                progressOverall.Value = OverallProgress;
                lblProgress.Text = "Processing Truth Table Index: " + CurrentTTIndex + "...    Time Left To Completion: "
                    + TimeLeft.Hours + ":" + TimeLeft.Minutes + ":" + TimeLeft.Seconds;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Itterating though all possibile values for ");
            sb.Append(BitCount);
            sb.AppendLine(" bits that will be valid for consecutive false bits in the encription.");
            sb.AppendLine("The tests are clumlitave and employ the results of the prior equation tests: ");
            sb.AppendLine();
            for (int i = 0; i < BitCount / 2; i++)
            {
                sb.Append("Eq/Bit Index ");
                sb.Append(i);
                sb.Append(": ");
                sb.Append(FalseCounts[i]);
                sb.AppendLine();
            }

            tbOut.Text = sb.ToString();
        }

        private void btnTryFour_Click(object sender, EventArgs e)
        {
            EquationManipulator em = new EquationManipulator((int)256, "738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1738377c1");
            em.Initialize();

            tbOut.Text = em.Solve();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch s = new Stopwatch();

            s.Start();
            EquationManipulator em = new EquationManipulator((int)256, "0000000500000005000000050000000500000005000000050000000500000005000000050000000500000005000000050000000500000005000000050000000500000005000000050000000500000005");
            tbOut.Text = "Solution Count: " + em.TopDownRecursionTest();
            s.Stop();

            tbOut.Text += System.Environment.NewLine + s.Elapsed.ToString();

            
        }
    }
}
