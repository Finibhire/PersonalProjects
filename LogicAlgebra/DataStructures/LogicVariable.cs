using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicAlgebra.DataStructures
{
    class LogicVariable : LogicAtom, IComparable<LogicVariable>
    {
        private readonly string id;
        public string ID {
            get
            {
                return id;
            }
        }

        public override int ContainedAtoms
        {
            get { return 0; }
        }

        //public override bool State
        //{
        //    get; set;
        //}

        public LogicVariable(string ID)
        {
            State = true;

            ID = ID.Trim();
            int idx = ID.LastIndexOf('\'');
            if (idx > 0)
            {
                ID = ID.Remove(ID.Length - 1);
                if (ID.LastIndexOf('\'') >= 0)
                {
                    throw new ArgumentException("not more than 1 apostrophy is allowed at the end to indicate inverse.");
                }
            }

            // Removal of characters come before this check
            if (String.IsNullOrWhiteSpace(ID))
            {
                throw new ArgumentNullException("ID");
            }
            if (ID.Any(x => char.IsWhiteSpace(x)))
            {
                throw new ArgumentException("The variable ID should not contain whitespaces.", "ID");
            }

            this.id = ID;
        }

        /// <summary>
        /// Compares the ID of both the variables NOT taking into account if they are inverses of each other.
        /// </summary>
        public int CompareTo(LogicVariable other)
        {
            //int c = ID.CompareTo(other.ID);
            //if (c == 0)
            //{
            //    if (other.State == this.State)
            //    {
            //        return 0;
            //    }
            //    else if (other.State == true)
            //    {
            //        return -1;
            //    }
            //    else
            //    {
            //        return 1;
            //    }
            //}
            //return c;
            return ID.CompareTo(other.ID);
        }
        public override int CompareTo(LogicAtom other)
        {
            if (other.GetType() == typeof(LogicVariable))
            {
                return CompareTo((LogicVariable)other);
            }
            return base.CompareTo(other);
        }

        public override void AppendToString(StringBuilder sb)
        {
            //sb.Append(' ');
            sb.Append(id);
            if (!State)
            {
                sb.Append('\'');
            }
            //sb.Append(' ');
        }

        public override LogicAtom Expand()
        {
            return this;
        }

        public override LogicAtom DeepCopy()
        {
            LogicVariable lv = new LogicVariable(id);
            lv.State = this.State;

            return lv;
        }

        public override void SubIn(string VarID, bool state)
        {
            return;
        }
    }
}
