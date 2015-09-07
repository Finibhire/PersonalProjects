using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using MeldingCalculator.DataClasses;

namespace MeldingCalculator
{

    public partial class Main : Form
    {
        /// <summary>
        /// lookup the success chance of a meld via [DegradedMateriaSlot#, MateriaRank# - 1]
        /// </summary>
        private readonly static double[,] MELDS_NEEDED = new double[4, 4] {
            {1d/0.45, 1d/0.41, 1d/0.35, 1d/0.29},
            {1d/0.24, 1d/0.22, 1d/0.19, 1d/0.16},
            {1d/0.14, 1d/0.13, 1d/0.11, 1d/0.10},
            {1d/0.08, 1d/0.08, 1d/0.07, 1d/0.04}
        };

        public Main()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            double[,] costs = new double[3, 4];

            for (int i = 1; i < 4; i++)
            {
                for (int ii = 1; ii < 5; ii++)
                {
                    try
                    {
                        costs[i - 1, ii - 1] = Convert.ToDouble(this.Controls.Find("txtMat" + i + ii, true).FirstOrDefault().Text);
                    }
                    catch { }
                }
            }

            int neededMat1 = 0, neededMat2 = 0, neededMat3 = 0;
            try
            {
                neededMat1 = Convert.ToInt32(txtNeededMat1.Text);
            }
            catch { }
            try
            {
                neededMat2 = Convert.ToInt32(txtNeededMat2.Text);
            }
            catch { }
            try
            {
                neededMat3 = Convert.ToInt32(txtNeededMat3.Text);
            }
            catch { }
            
            LinkedList<LinkedList<Materia>> type1MateriaCombos = FindPossibleMateriaCombos(neededMat1, MateriaType.Craftsmanship);
            LinkedList<LinkedList<Materia>> type2MateriaCombos = FindPossibleMateriaCombos(neededMat2, MateriaType.Control);
            LinkedList<LinkedList<Materia>> type3MateriaCombos = FindPossibleMateriaCombos(neededMat3, MateriaType.CP);
            
            LinkedList<LinkedList<Materia>> combinedCombos = new LinkedList<LinkedList<Materia>>();

            AddMateriaCombo(combinedCombos, type1MateriaCombos);
            AddMateriaCombo(combinedCombos, type2MateriaCombos);
            AddMateriaCombo(combinedCombos, type3MateriaCombos);

            double bestTotalCost = double.MaxValue;
            double[] bestMeldsNeeded = null;
            LinkedList<Materia> bestMateriaList = new LinkedList<Materia>();
            foreach(LinkedList<Materia> llm in combinedCombos)
            {
                Materia[,] orders = GetAllOrderings(llm);

                int bestOrderingIndex = -1;
                double bestOrderingTotalCost = double.MaxValue;
                double[] bestOrderingMeldsNeeded = new double[orders.GetLength(1)];
                for (int orderingIndex = 0; orderingIndex < orders.GetLength(0); orderingIndex++)
                {
                    double wTotalCost = 0;
                    double[] wMeldsNeeded = new double[orders.GetLength(1)];

                    for (int i = 0; i < orders.GetLength(1) && i < (int)spinSecureSlots.Value; i++)
                    {
                        wTotalCost += GetCost(orders[orderingIndex,i]);
                        wMeldsNeeded[i] = 1d;
                    }

                    int degradeMateriaSlot = 0;
                    for (int i = (int)spinSecureSlots.Value; i < orders.GetLength(1); i++)
                    {
                        double singleCost = GetCost(orders[orderingIndex,i]);
                        wMeldsNeeded[i] = MELDS_NEEDED[degradeMateriaSlot, (int)orders[orderingIndex, i].Rank - 1];
                        wTotalCost += singleCost * wMeldsNeeded[i];
                        degradeMateriaSlot++;
                    }

                    if (wTotalCost < bestOrderingTotalCost)
                    {
                        bestOrderingIndex = orderingIndex;
                        bestOrderingTotalCost = wTotalCost;
                        Array.Copy(wMeldsNeeded, bestOrderingMeldsNeeded, wMeldsNeeded.Length);
                    }
                }

                if (bestTotalCost > bestOrderingTotalCost)
                {
                    bestMateriaList.Clear();
                    for (int i = 0; i < orders.GetLength(1); i++)
                    {
                        bestMateriaList.AddLast(orders[bestOrderingIndex, i]);
                    }
                    bestTotalCost = bestOrderingTotalCost;
                    bestMeldsNeeded = bestOrderingMeldsNeeded;
                }
            }



            StringBuilder sb = new StringBuilder();
            sb.Append("Best Average Cost: ");
            sb.Append(bestTotalCost);
            sb.AppendLine();
            sb.AppendLine("Materias in Order they should be added are:");
            int index = 0;
            foreach (Materia m in bestMateriaList)
            {
                sb.Append(m.Type);
                sb.Append(" ");
                sb.Append(m.Rank);
                sb.Append(" (+");
                sb.Append(m.Value);
                sb.Append("), Avg. Needed = ");
                sb.Append(bestMeldsNeeded[index]);
                sb.AppendLine();
                index++;
            }

            //foreach(var combo in combinedCombos)
            //{
            //    sb.AppendLine("Combo Option:");
            //    foreach (var m in combo)
            //    {
            //        sb.Append("(");
            //        sb.Append(m.Type);
            //        sb.Append(", ");
            //        sb.Append(m.Rank);
            //        sb.Append("), ");
            //    }
            //    sb.AppendLine();
            //    sb.AppendLine();
            //}
            
            txtOutput.Text = sb.ToString();


        }

        private double GetCost(Materia m)
        {
            int index1 = 1;
            if (m.Type == MateriaType.Control)
            {
                index1 = 2;
            }
            else if (m.Type == MateriaType.CP)
            {
                index1 = 3;
            }

            int index2 = (int)m.Rank;

            return Convert.ToDouble(this.Controls.Find("txtMat" + index1 + index2, true).FirstOrDefault().Text);
        }
        
        private Materia[,] GetAllOrderings(LinkedList<Materia> baseOptions)
        {
            if (baseOptions == null || baseOptions.Count == 0)
            {
                throw new ArgumentException("baseOptions can't be null or have zero elements");
            }

            Materia[,] permutationSet = new Materia[Factorial(baseOptions.Count), baseOptions.Count];
            LinkedList<Materia> permutationPrefix = new LinkedList<Materia>();
            int permutationIndex = 0;

            return GetAllOrderingsRecursion(permutationPrefix, baseOptions, permutationSet, ref permutationIndex);
        }
        private Materia[,] GetAllOrderingsRecursion(LinkedList<Materia> permutationPrefix, LinkedList<Materia> remainingOptions, Materia[,] permutationSet, ref int permutationIndex)
        {
            if (remainingOptions.Count == 0) {
                // permutationPrefix is now a full permutation
                LinkedListNode<Materia> node = permutationPrefix.First;
                for (int i = 0; i < permutationPrefix.Count; i++)
                {
                    permutationSet[permutationIndex, i] = node.Value;
                    node = node.Next;
                }
                permutationIndex++;
            } 
            else 
            {
                foreach (Materia m in remainingOptions) 
                {
                    permutationPrefix.AddLast(m);
                    LinkedList<Materia> newRemaining = new LinkedList<Materia>(remainingOptions);
                    newRemaining.Remove(m);
                    GetAllOrderingsRecursion(permutationPrefix, newRemaining, permutationSet, ref permutationIndex);
                    permutationPrefix.RemoveLast();
                }
            }
            return permutationSet;
        }
        
        private int Factorial(int i)
        {
            int rtn = 1;
            while (i > 1)
            {
                rtn *= i;
                i--;
            }
            return rtn;
        }

        private LinkedList<LinkedList<Materia>> AddMateriaCombo(LinkedList<LinkedList<Materia>> combinedCombos, LinkedList<LinkedList<Materia>> materiaCombo)
        {
            if (combinedCombos.Count == 0)
            {
                foreach (LinkedList<Materia> materiaTypeCombo in materiaCombo )
                {
                    LinkedList<Materia> newMateriaCombo = new LinkedList<Materia>();
                    foreach (Materia m in materiaTypeCombo)
                    {
                        newMateriaCombo.AddLast(m);
                    }
                    combinedCombos.AddLast(newMateriaCombo);
                }
            }
            else if (materiaCombo.Count > 0)
            {
                LinkedListNode<LinkedList<Materia>> currentComboNode = combinedCombos.First;
                while (currentComboNode != null)
                {
                    foreach (LinkedList<Materia> addTypeCombo in materiaCombo)
                    {
                        if (currentComboNode.Value.Count + addTypeCombo.Count <= 5)
                        {
                            LinkedList<Materia> combinedCombo = new LinkedList<Materia>();
                            foreach (Materia m in currentComboNode.Value)
                            {
                                combinedCombo.AddLast(m);
                            }
                            foreach (Materia m in addTypeCombo)
                            {
                                combinedCombo.AddLast(m);
                            }
                            combinedCombos.AddBefore(currentComboNode, combinedCombo);
                        }
                    }

                    currentComboNode = currentComboNode.Next;
                    if (currentComboNode == null)
                    {
                        combinedCombos.RemoveLast();
                    }
                    else
                    {
                        combinedCombos.Remove(currentComboNode.Previous);
                    }
                }
            }
            return combinedCombos;
        }

        private LinkedList<LinkedList<Materia>> FindPossibleMateriaCombos(int scoreNeeded, MateriaType mType)
        {
            int scoreBump = mType.GetBump();
            LinkedList<LinkedList<Materia>> allPossibles = new LinkedList<LinkedList<Materia>>();
            int tally = 0;
            if (scoreNeeded > 0)
            {
                for (int slot1 = 4; slot1 > 0; slot1--)
                {
                    tally = slot1 + scoreBump;
                    Materia m1 = new Materia((MateriaRank)slot1, mType);
                    if (tally < scoreNeeded)
                    {
                        for (int slot2 = slot1; slot2 > 0; slot2--)
                        {
                            tally = slot1 + slot2 + scoreBump * 2;
                            Materia m2 = new Materia((MateriaRank)slot2, mType);
                            if (tally < scoreNeeded)
                            {
                                for (int slot3 = slot2; slot3 > 0; slot3--)
                                {
                                    tally = slot1 + slot2 + slot3 + scoreBump * 3;
                                    Materia m3 = new Materia((MateriaRank)slot3, mType);
                                    if (tally < scoreNeeded)
                                    {
                                        for (int slot4 = slot3; slot4 > 0; slot4--)
                                        {
                                            tally = slot1 + slot2 + slot3 + slot4 + scoreBump * 4;
                                            Materia m4 = new Materia((MateriaRank)slot4, mType);
                                            if (tally < scoreNeeded)
                                            {
                                                for (int slot5 = slot4; slot5 > 0; slot5--)
                                                {
                                                    tally = slot1 + slot2 + slot3 + slot4 + slot5 + scoreBump * 5;
                                                    Materia m5 = new Materia((MateriaRank)slot5, mType);
                                                    if (tally >= scoreNeeded)
                                                    {
                                                        LinkedList<Materia> poss = new LinkedList<Materia>();
                                                        poss.AddLast(m1);
                                                        poss.AddLast(m2);
                                                        poss.AddLast(m3);
                                                        poss.AddLast(m4);
                                                        poss.AddLast(m5);
                                                        allPossibles.AddLast(poss);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                LinkedList<Materia> poss = new LinkedList<Materia>();
                                                poss.AddLast(m1);
                                                poss.AddLast(m2);
                                                poss.AddLast(m3);
                                                poss.AddLast(m4);
                                                allPossibles.AddLast(poss);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LinkedList<Materia> poss = new LinkedList<Materia>();
                                        poss.AddLast(m1);
                                        poss.AddLast(m2);
                                        poss.AddLast(m3);
                                        allPossibles.AddLast(poss);
                                    }
                                }
                            }
                            else
                            {
                                LinkedList<Materia> poss = new LinkedList<Materia>();
                                poss.AddLast(m1);
                                poss.AddLast(m2);
                                allPossibles.AddLast(poss);
                            }
                        }
                    }
                    else
                    {
                        LinkedList<Materia> poss = new LinkedList<Materia>();
                        poss.AddLast(m1);
                        allPossibles.AddLast(poss);
                    }
                }
            }
            return allPossibles;
        }
    }
}
