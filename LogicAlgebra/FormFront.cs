using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LogicAlgebra.DataStructures;

namespace LogicAlgebra
{
    public partial class FormFront : Form
    {
        public FormFront()
        {
            InitializeComponent();
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            ANFEquation eq = new ANFEquation(tbInput.Text);
            //tbOut.Text = eq.ToString();
            eq = (ANFEquation)eq.Expand();
            //tbOut.Text += System.Environment.NewLine + System.Environment.NewLine + eq.ToString();
            tbOut.Text = eq.ToString();
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            ANFEquation eq1 = new ANFEquation(tbOut.Text);
            ANFEquation eq2 = new ANFEquation(tbExtract.Text);

            eq1 = eq1.Extract(eq2);
            tbOut.Text = eq1.ToString();
        }

        private void btnTestReduce_Click(object sender, EventArgs e)
        {
            EquationSetSolver solver = new EquationSetSolver(null, 32);
            solver.ReduceEncryptionEquations();
            tbOut.Text = solver.ToString();
        }

        private void btnFullExpand_Click(object sender, EventArgs e)
        {
            EquationSetSolver solver = new EquationSetSolver(null, 32);
            tbOut.Text = solver.FullExpand();
        }


    }
}
