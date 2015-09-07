using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationManipulator.Math
{
    class Term : Layer
    {
        /// <summary>
        /// <Variable.ID, Variable>
        /// </summary>
        private SortedList<string, Variable> variablesList;
        /// <summary>
        /// <Constant.ID, Variable>
        /// </summary>
        private SortedList<string, Constant> constantsList;

        public Term()
        {
            variablesList = new SortedList<string, Variable>();
            constantsList = new SortedList<string, Constant>();
        }

        public void AddVariable(Variable v)
        {
            Variable lv;
            if (variablesList.TryGetValue(v.ID, out lv))
            {
                int newNum = lv.PowerNumerator * v.PowerDenominator + v.PowerNumerator * lv.PowerDenominator;
                int newDen = lv.PowerDenominator * v.PowerDenominator;
                lv.SetPower(newNum, newDen);
                
                if (lv.PowerNumerator == 0)
                {
                    variablesList.Remove(lv.ID);
                }
            }
            else
            {
                variablesList.Add(v.ID, v);
            }
        }

        public void AddConstant(Constant c)
        {
            Constant lc;
            if (constantsList.TryGetValue(c.ID, out lc))
            {
                lc.Power += c.Power;
                if (lc.Power < 1e-323 && lc.Power > -1e-323)  // very close to zero to account for rounding errors.
                {
                    constantsList.Remove(lc.ID);
                }
            }
            else
            {
                constantsList.Add(c.ID, c);
            }
        }
    }
}
