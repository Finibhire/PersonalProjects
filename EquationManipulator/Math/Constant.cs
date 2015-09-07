using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationManipulator.Math
{
    class Constant
    {
        public string ID { get; set; }
        public double Power { get; set; }
        public string Hash
        {
            get
            {
                return ID + "^(" + Power.ToString() + ")";
            }
        }
    }
}
