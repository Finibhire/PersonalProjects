using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicAlgebra.DataStructures
{
    abstract class LogicAtom : IComparable<LogicAtom>
    {
        private static long lAtomCount;
        private readonly long uid;

        public static long LogicAtomCreatedCount
        {
            get
            {
                return lAtomCount;
            }
        }
        public long AtomUID
        {
            get
            {
                return uid;
            }
        }
        public abstract int ContainedAtoms { get; }

        private bool state;
        public virtual bool State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public abstract LogicAtom Expand();

        /// <summary>
        /// Used to provide a consistant order of LogicAtom types in a sorted collection.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        protected int CompareType(LogicAtom other)
        {
            Type tType = this.GetType();
            Type oType = other.GetType();
            if (tType == oType)
            {
                return 0;
            }
            else if (tType == typeof(LogicConstantValue))
            {
                return -1;
            }
            else if (oType == typeof(LogicConstantValue))
            {
                return 1;
            }
            else if (tType == typeof(LogicVariable))
            {
                return -1;
            }
            else if (oType == typeof(LogicVariable))
            {
                return 1;
            }
            else if (tType == typeof(Minterm))
            {
                return -1;
            }
            else if (oType == typeof(Minterm))
            {
                return 1;
            }
            //else if (tType == typeof(ANFEquation))
            //{
            //    return -1;
            //}
            //else if (oType == typeof(ANFEquation))
            //{
            //    return 1;
            //}
            else
            {
                throw new Exception("Unknown Type!  " + tType.FullName + " or " + oType.FullName);
            }
        }

        /// <summary>
        /// Inverse states should return equal (a 0 value).
        /// </summary>
        /// <returns>A value less than 0 if this instance preceds the passed in instance.</returns>
        public virtual int CompareTo(LogicAtom other)
        {
            int diff = CompareType(other);
            if (diff != 0)
            {
                return diff;
            }
            return Convert.ToInt32(this.uid - other.uid);
        }

        public LogicAtom()
        {
            state = true;
            lAtomCount++;
            uid = lAtomCount;
        }

        public abstract void AppendToString(StringBuilder sb);

        public abstract LogicAtom DeepCopy();

        public abstract void SubIn(string VarID, bool state);

        public static LogicAtom Distribute(LogicAtom a, LogicAtom b)
        {
            Type atype = a.GetType();
            Type btype = b.GetType();

            if (atype == typeof(LogicConstantValue))
            {
                if (!a.State)
                {
                    return LogicConstantValue.False;
                }
                return b.DeepCopy();
            }
            else if (btype == typeof(LogicConstantValue))
            {
                if (!b.State)
                {
                    return LogicConstantValue.False;
                }
                return a.DeepCopy();
            }
            else if (atype == typeof(LogicVariable))
            {
                if (btype == typeof(LogicVariable))
                {
                    return Distribute((LogicVariable)a, (LogicVariable)b);
                }
                else if (btype == typeof(Minterm))
                {
                    return Distribute((LogicVariable)a, (Minterm)b);
                }
                else if (btype == typeof(ANFEquation))
                {
                    return Distribute((LogicVariable)a, (ANFEquation)b);
                }
                else
                {
                    throw new Exception("Logic Error. unaccounted Logic Type.");
                }
            }
            else if (atype == typeof(Minterm))
            {
                if (btype == typeof(LogicVariable))
                {
                    return Distribute((Minterm)a, (LogicVariable)b);
                }
                else if (btype == typeof(Minterm))
                {
                    return Distribute((Minterm)a, (Minterm)b);
                }
                else if (btype == typeof(ANFEquation))
                {
                    return Distribute((Minterm)a, (ANFEquation)b);
                }
                else
                {
                    throw new Exception("Logic Error. unaccounted Logic Type.");
                }
            }
            else if (atype == typeof(ANFEquation))
            {
                if (btype == typeof(LogicVariable))
                {
                    return Distribute((ANFEquation)a, (LogicVariable)b);
                }
                else if (btype == typeof(Minterm))
                {
                    return Distribute((ANFEquation)a, (Minterm)b);
                }
                else if (btype == typeof(ANFEquation))
                {
                    return Distribute((ANFEquation)a, (ANFEquation)b);
                }
                else
                {
                    throw new Exception("Logic Error. unaccounted Logic Type.");
                }
            }
            else
            {
                throw new Exception("Logic Error. unaccounted Logic Type.");
            }
        }

        public static LogicAtom Distribute(LogicVariable a, LogicVariable b)
        {
            Minterm mt = new Minterm();
            mt.AddAtom(a.DeepCopy());
            mt.AddAtom(b.DeepCopy());
            if (mt.ContainedAtoms == 1)  // might as well reduce it if we can at this point.
            {
                if (mt.ContainsConstantFalse)
                {
                    return LogicConstantValue.False;
                }
                else if (mt.ContainsConstantTrue)
                {
                    return LogicConstantValue.True;
                }
            }
            return mt;
        }
        public static LogicAtom Distribute(LogicVariable a, Minterm b)
        {
            Minterm mt = (Minterm)b.DeepCopy();
            mt.AddAtom(a.DeepCopy());
            if (mt.ContainedAtoms == 1)  // might as well reduce it if we can at this point.
            {
                if (mt.ContainsConstantFalse)
                {
                    return LogicConstantValue.False;
                }
                else if (mt.ContainsConstantTrue)
                {
                    return LogicConstantValue.True;
                }
            }
            return mt;
        }
        public static LogicAtom Distribute(LogicVariable a, ANFEquation b)
        {
            ANFEquation eq = (ANFEquation)b.DeepCopy();
            eq.Distribute(a);
            return eq;
        }
        public static LogicAtom Distribute(Minterm a, Minterm b)
        {
            Minterm c = (Minterm)a.DeepCopy();
            c.Union(b);
            return c;
        }
        public static LogicAtom Distribute(Minterm a, ANFEquation b)
        {
            ANFEquation eq = (ANFEquation)b.DeepCopy();
            eq.Distribute(a);
            return eq;
        }
        public static LogicAtom Distribute(ANFEquation a, ANFEquation b)
        {
            ANFEquation eq = (ANFEquation)a.DeepCopy();
            eq.Distribute(b);
            return eq;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendToString(sb);
            return sb.ToString();
        }
    }
}
