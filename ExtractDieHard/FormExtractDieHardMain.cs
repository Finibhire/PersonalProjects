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

namespace ExtractDieHard
{
    public partial class FormExtractDieHardMain : Form
    {
        private readonly int[] collectionLines =
        {
            46, 47,
            59, 60,
            72, 73,
            85, 86,
            98, 99,
            111, 112,
            124, 125,
            137, 138,
            150, 151,
            176, 177,
            179, 180,
            197, 198,
            216, 217,
            239, 240,
            247, 248,
            255, 256,
            263, 264,
            271, 272,
            279, 280,
            287, 288,
            295, 296,
            303, 304,
            311, 312,
            319, 320,
            327, 328,
            335, 336,
            343, 344,
            351, 352,
            359, 360,
            367, 368,
            375, 376,
            383, 384,
            391, 392,
            399, 400,
            407, 408,
            415, 416,
            423, 424,
            431, 432,
            465, 485,
            527, 550,
            553, 581,
            584, 615,
            640, 642,
            668, 693,
            721, 731,
            754, 774,
            793, 813,
            841, 842,
            857, 867,
            889, 891,
            892, 894,
            938, 940};

        public FormExtractDieHardMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            lblSource.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            lblDest.Text = saveFileDialog1.FileName;
        }

        private void btnExtractData_Click(object sender, EventArgs e)
        {
            List<double> pValues = new List<double>();

            using (StreamReader sr = new StreamReader(File.Open(lblSource.Text, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                int idx = 0;
                int line = 1;

                while (idx < collectionLines.Length)
                {
                    int start = collectionLines[idx];
                    idx++;
                    int end = collectionLines[idx];
                    idx++;

                    while (line < start)
                    {
                        sr.ReadLine();
                        line++;
                    }

                    while (line < end)
                    {
                        string txt = sr.ReadLine();
                        txt = txt.Substring(txt.LastIndexOf('.'));
                        pValues.Add(Convert.ToDouble(txt));
                        line++;
                    }
                }
            }
            pValues.Sort();
            using (StreamWriter sw = new StreamWriter(File.Open(lblDest.Text, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)))
            {
                double xInc = 1d / (double)(pValues.Count - 1);
                double x = 0;
                foreach (double d in pValues)
                {
                    sw.WriteLine(x.ToString() + ", " + d.ToString());
                    x += xInc;
                }
            }
        }


    }
}
