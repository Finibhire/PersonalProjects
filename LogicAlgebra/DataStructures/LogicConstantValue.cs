using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicAlgebra.DataStructures
{
    class LogicConstantValue : LogicAtom
    {
        private static LogicConstantValue lcvTrue;
        private static LogicConstantValue lcvFalse;

        private readonly bool state;
        public override bool State
        {
            get
            {
                return state;
            }
            set
            {
                throw new Exception("The state of a constant can not be changed.");
            }
        }
        public override int ContainedAtoms
        {
            get { return 0; }
        }

        public static LogicConstantValue False
        {
            get
            {
                if (lcvFalse == null)
                {
                    lcvFalse = new LogicConstantValue(false);
                }
                return lcvFalse;
            }
        }
        public static LogicConstantValue True
        {
            get
            {
                if (lcvTrue == null)
                {
                    lcvTrue = new LogicConstantValue(true);
                }
                return lcvTrue;
            }
        }

        private LogicConstantValue(bool state)
        {
            this.state = state;
        }

        public override void AppendToString(StringBuilder sb)
        {
            if (state)
            {
                sb.Append('!');
            }
            else
            {
                sb.Append('~');
            }
        }

        public override LogicAtom Expand()
        {
            return this;
        }

        public override LogicAtom DeepCopy()
        {
            return this;
        }

        public override int CompareTo(LogicAtom other)
        {
            if (other.GetType() == typeof(LogicConstantValue))
            {
                if (state != other.State)
                {
                    if (state)
                    {
                        return 1;
                    }
                    return -1;
                }
                return 0;
            }
            return base.CompareTo(other);
        
        
        }

        public override void SubIn(string VarID, bool state)
        {
            return;
        }
    }
}
