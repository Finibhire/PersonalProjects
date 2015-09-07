using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationManipulator.Math
{
    enum OperationType
    {
        Plus, Subtract, Multiply, Divide, Exponent
    }

    abstract class Layer
    {
        public OperationType FollowingOperation { get; set; }
        public Layer FollowingLayer { get; set; }
    }
}
