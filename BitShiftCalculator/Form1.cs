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

using BitShiftCalculator.BinaryTrees;

namespace BitShiftCalculator
{
    public partial class Form1 : Form
    {
        private int DATASIZE = 32;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            string rawData = textBox1.Text.Replace(" ", "");
            int dataSize = 32;

            uint[] a = new uint[dataSize / 16];
            uint[] b = new uint[dataSize / 16];
            
            for (int i = 0; i < dataSize / 16; i++)
            {
                a[i] = uint.Parse(rawData.Substring(i * 8, 8), System.Globalization.NumberStyles.HexNumber);
            }

            for (int i = 0; i < dataSize; i++)
            {
                for (int j = 0; j < dataSize; j++)
                {
                    b[(i + j) / 32] ^= (((a[i / 32] >> (i % 32)) & (a[j / 32 + dataSize / 32] >> (j % 32)) & 1) << ((i + j) % 32));   
                    //b[(i + j) / 32] ^= (UInt16)(((UInt16)(a[i / 32] >> (i % 32)) & (UInt16)(a[j / 32 + dataSize / 32] >> (j % 32)) & 1) << ((i + j) % 32));
                }
            }



            textBox6.Text = GetHex(b);
        }

        private string GetHex(uint[] x)
        {
            StringBuilder sb = new StringBuilder(x.Length * 9);
            for (int i = 0; i < x.Length; i++)
            {
                sb.Append(x[i].ToString("X8"));
                sb.Append(" ");
            }
            sb.Length = sb.Length - 1;
            return sb.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string rawData = textBox1.Text.Replace(" ", "");
            uint[] aa = new uint[DATASIZE / 16];
            uint[] bb = new uint[DATASIZE / 16];
            BitHolder32[] a = new BitHolder32[DATASIZE / 16];
            XORCollector32Bit[] b = new XORCollector32Bit[DATASIZE / 16];
            for (int i = 0; i < DATASIZE / 16; i++)
            {
                a[i] = new BitHolder32(i * 32);
                b[i] = new XORCollector32Bit();
                aa[i] = uint.Parse(rawData.Substring(i * 8, 8), System.Globalization.NumberStyles.HexNumber);
            }

            for (int i = 0; i < DATASIZE; i++)
            {
                for (int j = 0; j < DATASIZE; j++)
                {
                    bb[(i + j) / 32] ^= (((aa[i / 32] >> (i % 32)) & (aa[j / 32 + DATASIZE / 32] >> (j % 32)) & 1) << ((i + j) % 32));   
                    BitHolder32 r1 = a[i / 32].DeepCopy();
                    r1.RightShift(i % 32);
                    BitHolder32 r2 = a[j / 32 + DATASIZE / 32].DeepCopy();
                    r2.RightShift(j % 32);
                    r1.ANDOne();
                    r1.AND(r2);
                    r1.LeftShift((i + j) % 32);
                    b[(i + j) / 32].XOR(r1);
                    if (bb[(i + j) / 32] != b[(i + j) / 32].TestPartialEncryption(aa))
                        throw new Exception("You messed up!");
                }
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < DATASIZE / 16; i++)
            {
                sb.Append("32 bits * (");
                sb.Append(i);
                sb.Append(") ");
                sb.Append('-', 10);
                sb.AppendLine();
                sb.Append(b[i].OutputResults());
                sb.AppendLine();
            }
            //tbOut.Text = sb.ToString();

            //bb = new uint[DATASIZE / 16];
            //for (int i = 0; i < DATASIZE; i++)
            //{
            //    for (int j = 0; j < DATASIZE; j++)
            //    {
            //        bb[(i + j) / 32] |= 1u << ((i + j) % 32);
            //    }
            //}

            //sb.AppendLine();
            //for (int i = 0; i < bb.Length; i++)
            //{
            //    sb.Append(bb[i].ToString("X8"));
            //    sb.Append(' ');
            //}
            //sb.Length -= 1;
            sb.AppendLine();

            tbOut.Text = sb.ToString();

            //tbOut.Text = string.Empty;

            BitLogicRecursiveTester blt = new BitLogicRecursiveTester(b, aa);
            var r = blt.EncriptUsingXORBitTracking();
            foreach (var rr in r)
            {
                tbOut.Text += rr.ToString("X8") + " ";
            }
            var rslt = blt.TestCombinations();

            int[] valueChanges = new int[64];
            for (int i = 0; i < valueChanges.Length; i++)
            {
                valueChanges[i] = -1;
            }

            for (int i = 0; i < rslt.Count; i++)
            {
                uint[] finalData = new uint[DATASIZE / 16];
                for (int j = 0; j < rslt.ElementAt(i).Length; j++)
                {
                    if (rslt.ElementAt(i)[j].Value)
                    {
                        //finalData[rslt.ElementAt(i)[j].Key / 32] |= 1u << (rslt.ElementAt(i)[j].Key % 32);
                        valueChanges[rslt.ElementAt(i)[j].Key]++;
                    }
                }
                //sb.Append("Successful Solution ");
                //sb.Append(i);
                //sb.Append(": ");
                //foreach (uint rr in finalData)
                //{
                //    sb.Append(rr.ToString("X8"));
                //    sb.Append(" ");
                //}
                //sb.AppendLine();
            }
            sb.AppendLine();
            sb.Append("No Value Changes For: ");
            for (int i = 0; i < valueChanges.Length; i++)
            {
                if (valueChanges[i] == 0)
                {
                    sb.Append(i);
                    sb.Append(", ");
                }
            }

            tbOut.Text = sb.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //int[] testValue = new int[] { 50, 20, 50 };
            int[] testValue = new int[500];

            Random rng = new Random();
            for (int i = 0; i < testValue.Length; i++)
            {
                testValue[i] = rng.Next(10, 100);
            }

            RedBlackTree<int, int> tree = new RedBlackTree<int, int>();
            for (int i = 0; i < testValue.Length; i++)
            {
                tree.Insert(testValue[i], testValue[i], true);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < testValue.Length; i++)
            {
                sb.Append(testValue[i]);
                sb.Append(' ');
            }
            sb.Length = sb.Length - 1;
            sb.AppendLine();
            sb.AppendLine();
            foreach (int i in tree.Keys)
            {
                sb.Append(i);
                sb.Append(' ');
            }

            tbOut.Text = sb.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            Stopwatch sw = new Stopwatch();
            TimeSpan slTime = TimeSpan.MinValue;
            TimeSpan treeTime = TimeSpan.MaxValue;
            //int[] testValue = new int[500000];

            Random rng = new Random();
            //for (int i = 0; i < testValue.Length; i++)
            //{
            //    testValue[i] = rng.Next();
            //}

            SortedDictionary<int, int> sl = new SortedDictionary<int, int>();
            RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

            sb.Append("1 tick = ");
            sb.Append((int)(1000000000d / ((double)Stopwatch.Frequency)));
            sb.AppendLine(" nano seconds");

            sb.AppendLine(" Entries | SL in ms | RBT adj || SL ticks | RBT adj | % Time Spent");

            const int MaxInsert = 100001;
            for (int k = 1; k < MaxInsert; k *= 10)
                for (int j = k; j < k * 10 && j < MaxInsert; j += k)
                {
                    sl.Clear();
                    tree.Clear();
                    int rngValue = rng.Next();

                    sw.Start();
                    for (int i = 0; i < j; i++)
                        sl[rngValue] = rngValue;
                    sw.Stop();
                    slTime = sw.Elapsed;
                    sw.Reset();
                    sw.Start();
                    for (int i = 0; i < j; i++)
                        tree.Insert(rngValue, rngValue, true);
                    sw.Stop();
                    treeTime = sw.Elapsed;
                    sw.Reset();

                    sb.Append(j.ToString().PadLeft(8));
                    sb.Append(" |");
                    sb.Append(slTime.Milliseconds.ToString().PadLeft(9));
                    sb.Append(" |");
                    sb.Append((treeTime.Milliseconds - slTime.Milliseconds).ToString().PadLeft(8));
                    sb.Append(" ||");
                    sb.Append(slTime.Ticks.ToString().PadLeft(9));
                    sb.Append(" |");
                    sb.Append((treeTime.Ticks - slTime.Ticks).ToString().PadLeft(8));
                    sb.Append(" |");
                    sb.Append(((double)treeTime.Ticks / (double)slTime.Ticks * 100d).ToString("F2").PadLeft(13));
                    sb.AppendLine();
                }
            tbOut.Text = sb.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            Stopwatch sw = new Stopwatch();
            TimeSpan slTime = TimeSpan.MinValue;
            TimeSpan treeTime = TimeSpan.MaxValue;
            Random rng = new Random();

            SortedDictionary<int, int> sl = new SortedDictionary<int, int>();
            RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

            sb.Append("1 tick = ");
            sb.Append((int)(1000000000d / ((double)Stopwatch.Frequency)));
            sb.AppendLine(" nano seconds");

            sb.AppendLine(" Entries | SL in ms | RBT adj || SL ticks | RBT adj | % Time Spent");

            const long MaxRotations = 1000;
            const long MinInsert = 99999;
            const long MaxInsert = 100000;
            long[,] slData = new long[MaxInsert - MinInsert + 1, MaxRotations];
            long[,] treeData = new long[MaxInsert - MinInsert + 1, MaxRotations];
            double[] slData2 = new double[MaxInsert - MinInsert + 1];
            double[] treeData2 = new double[MaxInsert - MinInsert + 1];

            for (long j = MinInsert; j <= MaxInsert; j++)
                for (long k = 0L; k < MaxRotations; k++)
                {
                    sl.Clear();
                    tree.Clear();
                    int rngValue = rng.Next();

                    sw.Start();
                    for (int i = 0; i < j; i++)
                        sl[rngValue] = rngValue;
                    sw.Stop();
                    slTime = sw.Elapsed;
                    sw.Reset();
                    sw.Start();
                    for (int i = 0; i < j; i++)
                        tree.Insert(rngValue, rngValue, true);
                    sw.Stop();
                    treeTime = sw.Elapsed;
                    sw.Reset();

                    slData[j - MinInsert, k] = slTime.Ticks;
                    treeData[j - MinInsert, k] = treeTime.Ticks;
                }

            for (long j = MinInsert; j <= MaxInsert; j++)
            {
                long slSum = 0, treeSum = 0;
                for (int k = 0; k < MaxRotations; k++)
                {
                    slSum += slData[j - MinInsert, k];
                    treeSum += treeData[j - MinInsert, k];
                }
                slData2[j - MinInsert] = (double)slSum / (double)MaxRotations;
                treeData2[j - MinInsert] = (double)treeSum / (double)MaxRotations;
            }

            for (long j = MinInsert; j <= MaxInsert; j++)
            {

                sb.Append(j.ToString().PadLeft(8));
                sb.Append(" |");
                sb.Append(string.Empty.PadLeft(9));
                sb.Append(" |");
                sb.Append(string.Empty.PadLeft(8));
                sb.Append(" ||");
                sb.Append(slData2[j - MinInsert].ToString("F2").PadLeft(9));
                sb.Append(" |");
                sb.Append((treeData2[j - MinInsert] - slData2[j - MinInsert]).ToString("F2").PadLeft(8));
                sb.Append(" |");
                sb.Append((treeData2[j - MinInsert] / slData2[j - MinInsert] * 100d).ToString("F2").PadLeft(13));
                sb.AppendLine();
            }

            tbOut.Text = sb.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string rawData = textBox1.Text.Replace(" ", "");
            uint[] aa = new uint[DATASIZE / 16];
            uint[] bb = new uint[DATASIZE / 16];
            BitHolder32[] a = new BitHolder32[DATASIZE / 16];
            XORCollector32Bit[] b = new XORCollector32Bit[DATASIZE / 16];
            for (int i = 0; i < DATASIZE / 16; i++)
            {
                a[i] = new BitHolder32(i * 32);
                b[i] = new XORCollector32Bit();
                aa[i] = uint.Parse(rawData.Substring(i * 8, 8), System.Globalization.NumberStyles.HexNumber);
            }

            for (int i = 0; i < DATASIZE; i++)
            {
                for (int j = 0; j < DATASIZE; j++)
                {
                    bb[(i + j) / 32] ^= (((aa[i / 32] >> (i % 32)) & (aa[j / 32 + DATASIZE / 32] >> (j % 32)) & 1) << ((i + j) % 32));
                    BitHolder32 r1 = a[i / 32].DeepCopy();
                    r1.RightShift(i % 32);
                    BitHolder32 r2 = a[j / 32 + DATASIZE / 32].DeepCopy();
                    r2.RightShift(j % 32);
                    r1.ANDOne();
                    r1.AND(r2);
                    r1.LeftShift((i + j) % 32);
                    b[(i + j) / 32].XOR(r1);
                    if (bb[(i + j) / 32] != b[(i + j) / 32].TestPartialEncryption(aa))
                        throw new Exception("You messed up!");
                }
            }

            var rdcer = new LogicEquationReducer(b);
            var solution = rdcer.TryThree(aa, sb);

            sb.AppendLine();
            sb.AppendLine();

            for (int i = 0; i < solution.Length; i++)
            {
                sb.Append(solution[i].ToString("X8"));
                sb.Append(" ");
            }
            tbOut.Text = sb.ToString();
        }

        //private class v
        //{
        //    public const int
        //        a = 0, b = 1, c = 2, d = 3, e = 4, f = 5, g = 6, h = 7, i = 8, j = 9, k = 10;

        //}

        private void button6_Click(object sender, EventArgs eArgs)
        {
            //const int
            //    a = 0, b = 1, c = 2, d = 3, e = 4, f = 5, g = 6, h = 7, i = 8, j = 9, k = 10, l = 11, m = 12,
            //    n = 13, o = 14, p = 15, q = 16, r = 17, s = 18, t = 19, u = 20, v = 21, w = 22, x = 23, y = 24, z = 25,
            //    A = 26, B = 27, C = 28, D = 29, E = 30, F = 31, G = 32, H = 33, I = 34, J = 35, K = 36, L = 37, M = 38;
            //uint[][] ands = new uint[][]
            //{
            //    new uint[] {(1u << a)|(1u << b)},
            //    new uint[] {(1u << c)|(1u << b), (1u << a)|(1u << d)},
            //    new uint[] {(1u << e)|(1u << b), (1u << c)|(1u << d), (1u << a)|(1u << f)},
            //    new uint[] {(1u << g)|(1u << b), (1u << e)|(1u << d), (1u << c)|(1u << f), (1u << a)|(1u << h)},
            //    new uint[] {(1u << i)|(1u << b), (1u << g)|(1u << d), (1u << e)|(1u << f), (1u << c)|(1u << h), (1u << a)|(1u << j)},
            //    new uint[] {(1u << k)|(1u << b), (1u << i)|(1u << d), (1u << g)|(1u << f), (1u << e)|(1u << h), (1u << c)|(1u << j), (1u << a)|(1u << l)},
            //    new uint[] {(1u << m)|(1u << b), (1u << k)|(1u << d), (1u << i)|(1u << f), (1u << g)|(1u << h), (1u << e)|(1u << j), (1u << c)|(1u << l), (1u << a)|(1u << n)},
            //    new uint[] {(1u << o)|(1u << b), (1u << m)|(1u << d), (1u << k)|(1u << f), (1u << i)|(1u << h), (1u << g)|(1u << j), (1u << e)|(1u << l), (1u << c)|(1u << n), (1u << a)|(1u << p)},
            //    new uint[] {(1u << q)|(1u << b), (1u << o)|(1u << d), (1u << m)|(1u << f), (1u << k)|(1u << h), (1u << i)|(1u << j), (1u << g)|(1u << l), (1u << e)|(1u << n), (1u << c)|(1u << p), (1u << a)|(1u << r)},
            //    new uint[] {(1u << s)|(1u << b), (1u << q)|(1u << d), (1u << o)|(1u << f), (1u << m)|(1u << h), (1u << k)|(1u << j), (1u << i)|(1u << l), (1u << g)|(1u << n), (1u << e)|(1u << p), (1u << c)|(1u << r), (1u << a)|(1u << t)},
            //    new uint[] {(1u << u)|(1u << b), (1u << s)|(1u << d), (1u << q)|(1u << f), (1u << o)|(1u << h), (1u << m)|(1u << j), (1u << k)|(1u << l), (1u << i)|(1u << n), (1u << g)|(1u << p), (1u << e)|(1u << r), (1u << c)|(1u << r), (1u << a)|(1u << v)},
            //    new uint[] {(1u << u)|(1u << b), (1u << s)|(1u << d), (1u << q)|(1u << f), (1u << o)|(1u << h), (1u << m)|(1u << j), (1u << k)|(1u << l), (1u << i)|(1u << n), (1u << g)|(1u << p), (1u << e)|(1u << r), (1u << c)|(1u << r), (1u << a)|(1u << v), (1u << a)|(1u << v)},
            //    new uint[] {(1u << u)|(1u << b), (1u << s)|(1u << d), (1u << q)|(1u << f), (1u << o)|(1u << h), (1u << m)|(1u << j), (1u << k)|(1u << l), (1u << i)|(1u << n), (1u << g)|(1u << p), (1u << e)|(1u << r), (1u << c)|(1u << r), (1u << a)|(1u << v), (1u << a)|(1u << v), (1u << a)|(1u << v)},
            //    new uint[] {(1u << u)|(1u << b), (1u << s)|(1u << d), (1u << q)|(1u << f), (1u << o)|(1u << h), (1u << m)|(1u << j), (1u << k)|(1u << l), (1u << i)|(1u << n), (1u << g)|(1u << p), (1u << e)|(1u << r), (1u << c)|(1u << r), (1u << a)|(1u << v), (1u << a)|(1u << v), (1u << a)|(1u << v), (1u << a)|(1u << v)},
            //    new uint[] {(1u << u)|(1u << b), (1u << s)|(1u << d), (1u << q)|(1u << f), (1u << o)|(1u << h), (1u << m)|(1u << j), (1u << k)|(1u << l), (1u << i)|(1u << n), (1u << g)|(1u << p), (1u << e)|(1u << r), (1u << c)|(1u << r), (1u << a)|(1u << v), (1u << a)|(1u << v), (1u << a)|(1u << v), (1u << a)|(1u << v), (1u << a)|(1u << v)},
            //    new uint[] {(1u << u)|(1u << b), (1u << s)|(1u << d), (1u << q)|(1u << f), (1u << o)|(1u << h), (1u << m)|(1u << j), (1u << k)|(1u << l), (1u << i)|(1u << n), (1u << g)|(1u << p), (1u << e)|(1u << r), (1u << c)|(1u << r), (1u << a)|(1u << v), (1u << a)|(1u << v), (1u << a)|(1u << v), (1u << a)|(1u << v), (1u << a)|(1u << v), (1u << a)|(1u << v)}
            //};  // hand done

            //const int
            //    v00 = 00, v01 = 01, v02 = 02, v03 = 03, v04 = 04, v05 = 05, v06 = 06, v07 = 07, v08 = 08, v09 = 09,
            //    v10 = 10, v11 = 11, v12 = 12, v13 = 13, v14 = 14, v15 = 15, v16 = 16, v17 = 17, v18 = 18, v19 = 19,
            //    v20 = 20, v21 = 21, v22 = 22, v23 = 23, v24 = 24, v25 = 25, v26 = 26, v27 = 27, v28 = 28, v29 = 29,
            //    v30 = 30, v31 = 31, v32 = 32, v33 = 33, v34 = 34, v35 = 35, v36 = 36, v37 = 37, v38 = 38, v39 = 39,
            //    v40 = 40, v41 = 41, v42 = 42, v43 = 43, v44 = 44, v45 = 45, v46 = 46, v47 = 47, v48 = 48, v49 = 49,
            //    v50 = 50, v51 = 51, v52 = 52, v53 = 53, v54 = 54, v55 = 55, v56 = 56, v57 = 57, v58 = 58, v59 = 59,
            //    v60 = 60, v61 = 61, v62 = 62, v63 = 63, v64 = 64, v65 = 65, v66 = 66, v67 = 67, v68 = 68, v69 = 69,
            //    v70 = 70, v71 = 71, v72 = 72, v73 = 73, v74 = 74, v75 = 75, v76 = 76, v77 = 77, v78 = 78, v79 = 79,
            //    v80 = 80, v81 = 81, v82 = 82, v83 = 83, v84 = 84, v85 = 85, v86 = 86, v87 = 87, v88 = 88, v89 = 89,
            //    v90 = 90, v91 = 91, v92 = 92, v93 = 93, v94 = 94, v95 = 95, v96 = 96, v97 = 97, v98 = 98, v99 = 99;
            uint[][] ands = new uint[][]
            {
                //new uint[] {(1u << v00)|(1u << v01)},
                //new uint[] {(1u << v02)|(1u << v01), (1u << v00)|(1u << v03)},
                //new uint[] {(1u << v04)|(1u << v01), (1u << v02)|(1u << v03), (1u << v00)|(1u << v05)},
                //new uint[] {(1u << v06)|(1u << v01), (1u << v04)|(1u << v03), (1u << v02)|(1u << v05), (1u << v00)|(1u << v07)},
                //new uint[] {(1u << v08)|(1u << v01), (1u << v06)|(1u << v03), (1u << v04)|(1u << v05), (1u << v02)|(1u << v07), (1u << v00)|(1u << v09)},
                //new uint[] {(1u << v10)|(1u << v01), (1u << v08)|(1u << v03), (1u << v06)|(1u << v05), (1u << v04)|(1u << v07), (1u << v02)|(1u << v09), (1u << v00)|(1u << v11)},
                //new uint[] {(1u << v12)|(1u << v01), (1u << v10)|(1u << v03), (1u << v08)|(1u << v05), (1u << v06)|(1u << v07), (1u << v04)|(1u << v09), (1u << v02)|(1u << v11), (1u << v00)|(1u << v13)},
                //new uint[] {(1u << v14)|(1u << v01), (1u << v12)|(1u << v03), (1u << v10)|(1u << v05), (1u << v08)|(1u << v07), (1u << v06)|(1u << v09), (1u << v04)|(1u << v11), (1u << v02)|(1u << v13), (1u << v00)|(1u << v15)},
                //new uint[] {(1u << v16)|(1u << v01), (1u << v14)|(1u << v03), (1u << v12)|(1u << v05), (1u << v10)|(1u << v07), (1u << v08)|(1u << v09), (1u << v06)|(1u << v11), (1u << v04)|(1u << v13), (1u << v02)|(1u << v15), (1u << v00)|(1u << v17)},
                //new uint[] {(1u << v18)|(1u << v01), (1u << v16)|(1u << v03), (1u << v14)|(1u << v05), (1u << v12)|(1u << v07), (1u << v10)|(1u << v09), (1u << v08)|(1u << v11), (1u << v06)|(1u << v13), (1u << v04)|(1u << v15), (1u << v02)|(1u << v17), (1u << v00)|(1u << v19)},
                //new uint[] {(1u << v20)|(1u << v01), (1u << v18)|(1u << v03), (1u << v16)|(1u << v05), (1u << v14)|(1u << v07), (1u << v12)|(1u << v09), (1u << v10)|(1u << v11), (1u << v08)|(1u << v13), (1u << v06)|(1u << v15), (1u << v04)|(1u << v17), (1u << v02)|(1u << v19), (1u << v00)|(1u << v21)},
                //new uint[] {(1u << v22)|(1u << v01), (1u << v20)|(1u << v03), (1u << v18)|(1u << v05), (1u << v16)|(1u << v07), (1u << v14)|(1u << v09), (1u << v12)|(1u << v11), (1u << v10)|(1u << v13), (1u << v08)|(1u << v15), (1u << v06)|(1u << v17), (1u << v04)|(1u << v19), (1u << v02)|(1u << v21), (1u << v00)|(1u << v23)},
                //new uint[] {(1u << v24)|(1u << v01), (1u << v22)|(1u << v03), (1u << v20)|(1u << v05), (1u << v18)|(1u << v07), (1u << v16)|(1u << v09), (1u << v14)|(1u << v11), (1u << v12)|(1u << v13), (1u << v10)|(1u << v15), (1u << v08)|(1u << v17), (1u << v06)|(1u << v19), (1u << v04)|(1u << v21), (1u << v02)|(1u << v23), (1u << v00)|(1u << v25)},
                //new uint[] {(1u << v26)|(1u << v01), (1u << v24)|(1u << v03), (1u << v22)|(1u << v05), (1u << v20)|(1u << v07), (1u << v18)|(1u << v09), (1u << v16)|(1u << v11), (1u << v14)|(1u << v13), (1u << v12)|(1u << v15), (1u << v10)|(1u << v17), (1u << v08)|(1u << v19), (1u << v06)|(1u << v21), (1u << v04)|(1u << v23), (1u << v02)|(1u << v25), (1u << v00)|(1u << v27)},
                //new uint[] {(1u << v28)|(1u << v01), (1u << v26)|(1u << v03), (1u << v24)|(1u << v05), (1u << v22)|(1u << v07), (1u << v20)|(1u << v09), (1u << v18)|(1u << v11), (1u << v16)|(1u << v13), (1u << v14)|(1u << v15), (1u << v12)|(1u << v17), (1u << v10)|(1u << v19), (1u << v08)|(1u << v21), (1u << v06)|(1u << v23), (1u << v04)|(1u << v25), (1u << v02)|(1u << v27), (1u << v00)|(1u << v29)},
                //new uint[] {(1u << v30)|(1u << v01), (1u << v28)|(1u << v03), (1u << v26)|(1u << v05), (1u << v24)|(1u << v07), (1u << v22)|(1u << v09), (1u << v20)|(1u << v11), (1u << v18)|(1u << v13), (1u << v16)|(1u << v15), (1u << v14)|(1u << v17), (1u << v12)|(1u << v19), (1u << v10)|(1u << v21), (1u << v08)|(1u << v23), (1u << v06)|(1u << v25), (1u << v04)|(1u << v27), (1u << v02)|(1u << v29), (1u << v00)|(1u << v31)}
            };

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("const int ");
            for (int ii = 0; ii < 10; ii++)
            {
                sb.Append("    ");
                for (int jj = 0; jj < 10; jj++)
                {
                    sb.Append("v");
                    sb.AppendID(ii * 10 + jj, 2);
                    sb.Append(" = ");
                    sb.AppendID(ii * 10 + jj, 2);
                    sb.Append(", ");
                }
                sb.Length -= 1;
                sb.AppendLine();
            }
            sb.Length -= 3;
            sb.AppendLine(";");
            sb.AppendLine();

            for (int ii = 0; ii < 16; ii++)
            {
                int termCount = ii + 1;
                sb.Append("new uint[] {(1u << v");
                for (int jj = 0; jj < termCount; jj++)
                {
                    //sb.Append((char)(97 + 2 * (ii - jj)));
                    sb.AppendID(2 * (ii - jj), 2);
                    sb.Append(")|(1u << v");
                    //sb.Append((char)(97 + 2 * jj + 1));
                    sb.AppendID(2 * jj + 1, 2);
                    sb.Append("), (1u << v");
                }
                sb.Length -= 10;
                sb.AppendLine("},");
            }

            //tbOut.Text = sb.ToString();
            sb.Clear();
            
            const int MaxRow = 13;  //11
            const int NumOfVar = 2 * MaxRow;
            int TTRows = (int)Math.Pow(2, NumOfVar);

            bool[,] testResults = new bool[TTRows, MaxRow];

            for (uint itt = 0; itt < TTRows; itt++)
            {
                for (int itt2 = 0; itt2 < MaxRow; itt2++)
                {
                    for (int itt3 = 0; itt3 < itt2 + 1; itt3++)
                    {
                        if ((ands[itt2][itt3] & itt) == ands[itt2][itt3])
                        {
                            testResults[itt, itt2] = !testResults[itt, itt2];
                        }
                    }
                    int prev = itt2 - 1;
                    if (prev >= 0 && testResults[itt, prev])
                    {
                        testResults[itt, itt2] = !testResults[itt, itt2];
                    }
                }
            }

            int[] countFalses = new int[MaxRow];
            for (int itt1 = 0; itt1 < TTRows; itt1++)
            {
                for (int itt2 = 0; itt2 < MaxRow; itt2++)
                {
                    if (!testResults[itt1, itt2])
                    {
                        countFalses[itt2]++;
                    }
                }
            }

            sb.Append("The number of combinations of ");
            sb.Append(MaxRow);
            sb.AppendLine(" bits that will be valid for consecutive false bits in the encription.");
            sb.AppendLine("The tests are clumlitave and employ the results of the prior equation tests: ");
            sb.AppendLine();
            for (int i = 0; i < MaxRow; i++)
            {
                sb.Append("Eq/Bit Index ");
                sb.Append(i);
                sb.Append(": ");
                sb.Append(countFalses[i]);
                sb.AppendLine();
            }

            tbOut.Text = sb.ToString();
        }
    }

    public static class Extensions
    {
        public static void AppendID(this StringBuilder sb, int id, int length)
        {
            int add = length - 1;
            if (id > 0)
            {
                add -= (int)Math.Log10(id);
            }

            if (add > 0)
            {
                sb.Append('0', add);
            }
            sb.Append(id);
        }
    }
}
