using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationManipulator.Math
{
    class Variable : IComparable<Variable>
    {
        private string hash;
        private string id;
        private int powerNumerator;
        private int powerDenominator;

        public Variable(string id, int powerNumerator = 1, int powerDenominator = 1)
        {
            this.id = id;
            SetPower(powerNumerator, powerDenominator);
            //UpdateHash();  done in SetPower()
        }
        
        private void UpdateHash()
        {
            if (powerNumerator == 0) 
            {
                hash = id;
            }
            else if (powerNumerator == 0)
            {
                hash = "1";
            }
            else if (powerDenominator == 1)
            {
                hash = id + "^" + powerNumerator.ToString();
            }
            else
            {
                hash = id + "^(" + powerNumerator.ToString() + "/" + powerDenominator.ToString() + ")";
            }
        }

        public string ID 
        { 
            get
            {
                return id;
            }
            set
            {
                id = value;
                UpdateHash();
            }
        }
        public int PowerNumerator 
        { 
            get
            {
                return powerNumerator;
            }
        }
        public int PowerDenominator 
        { 
            get
            {
                return powerDenominator;
            }
        }

        public void SetPower(int numerator, int denominator)
        {
            
            if (denominator == 0)
            {
                throw new ArgumentOutOfRangeException("denominator", "Must be non-zero");
            }
            else if (denominator < 0)
            {
                numerator *= -1;
                denominator *= -1;
            }

            if (numerator == 0)
            {
                powerNumerator = 0;
                powerDenominator = 1;
            }
            else
            {
                int gcf = GreatestCommonFactor(numerator, denominator);
                powerNumerator = numerator / gcf;
                powerDenominator = denominator / gcf;
            }

            UpdateHash();
        }

        public string Hash
        {
            get
            {
                return hash;
            }
        }

        public int CompareTo(Variable v)
        {
            return hash.CompareTo(v.hash);
        }



        private int GreatestCommonFactor(int a, int b)
        {
            if (a < 0)
            {
                a *= -1;
            }
            if (b < 0)
            {
                b *= -1;
            }
            if (a < b)
            {
                int c = a;
                a = b;
                b = c;
            }
            
            if (b == 0)
            {
                return 1;
            }
            return GreatestCommonFactor(a, b);
        }
        private int GreatestCommonFactorRecursion(int a, int b)
        {
            int remainder = a % b;

            if (remainder == 0)
            {
                return b;
            }
            else
            {
                return GreatestCommonFactorRecursion(b, remainder);
            }
        }
    }
}
