using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquationManipulator.Math;

namespace EquationManipulator.Math
{
    class Equation : Layer
    {
        private const string SYMBOL_MULTIPLY = "*";
        private const string SYMBOL_DIVIDE = "/";
        private const string SYMBOL_ADD = "+";
        private const string SYMBOL_SUBTRACT = "-";
        private const string SYMBOL_POWER = "^";
        private const string SYMBOL_BEGINLAYER = "(";
        private const string SYMBOL_ENDLAYER = ")";

        private Layer firstLayer;
        private Layer lastLayer;

        public void Expand()
        {
            Type flt = firstLayer.GetType();
            if (flt == typeof(Equation))
            {
                Equation subEq = (Equation)firstLayer;
                Layer subLayer = subEq.ExpandInnerEquations();
            }
        }

        public static Equation ParseEquation(string eqString, List<string> variableSymbols, List<string> constantSymbols)
        {
            
        }

        private Layer ExpandInnerEquations()
        {
            return null;
        }
    }
}
