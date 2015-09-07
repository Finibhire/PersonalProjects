using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicAlgebra.DataStructures
{
    class Minterm : LogicAtom, IComparable<Minterm>
    {
        private bool containsConstTrue;
        private bool containsConstFalse;
        private SortedDictionary<LogicAtom, LogicAtom> lAtoms;

        public override bool State
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int ContainedAtoms
        {
            get
            {
                return lAtoms.Count;
            }
        }

        public bool ContainsConstantFalse
        {
            get
            {
                return containsConstFalse;
            }
        }
        public bool ContainsConstantTrue
        {
            get
            {
                return containsConstTrue;
            }
        }

        public Minterm()
        {
            containsConstTrue = false;
            containsConstFalse = false;
            lAtoms = new SortedDictionary<LogicAtom, LogicAtom>();
        }

        /// <summary>
        /// Inverses of ANFEquations aren't detected yet because it doesn't have a good CompareTo() method that returns
        /// a value indicating that the two equations are equal.
        /// </summary>
        public void AddAtom(LogicAtom lAtom)
        {
            if (containsConstFalse)
            {
                return;
            }
            else if (lAtom.GetType() == typeof(LogicConstantValue))
            {
                if (lAtom.State)
                {
                    if (!containsConstTrue)
                    {
                        lAtoms.Add(LogicConstantValue.True, LogicConstantValue.True);
                        containsConstTrue = true;
                    }
                    return;
                }
                else
                {
                    containsConstFalse = true;
                    containsConstTrue = false;
                    lAtoms.Clear();  // false & A = false
                    lAtoms.Add(LogicConstantValue.False, LogicConstantValue.False);
                    return;
                }
            }

            LogicAtom existing = null;
            if (lAtoms.TryGetValue(lAtom, out existing))
            {
                if (lAtom.State != existing.State)
                {
                    lAtoms.Clear();
                    containsConstFalse = true;
                    containsConstTrue = false;
                    lAtoms.Add(LogicConstantValue.False, LogicConstantValue.False);
                }
                // else do nothing because it already exists in this Minterm and A * A == A
            }
            else
            {
                lAtoms.Add(lAtom, lAtom);
            }
        }

        /// <summary>
        /// If an inverse of the variable/atom allready exists in this Minterm then we will annialate all variables and 
        /// set the state of this Minterm to (false).
        /// </summary>
        public void AddVariable(LogicVariable lv) 
        {
            AddAtom(lv);
        }

        public override void AppendToString(StringBuilder sb)
        {
            sb.Append('(');
            if (lAtoms.Count > 0)
            {
                foreach (var pair in lAtoms)
                {
                    pair.Key.AppendToString(sb);
                    sb.Append(' ');
                }
                sb.Length -= 1;
            }
            else
            {
                sb.Append('#');
            }
            sb.Append(')');
        }

        public void Union(Minterm other)
        {
            foreach (var pair in other.lAtoms)
            {
                AddAtom(pair.Key);
            }
        }

        public override int CompareTo(LogicAtom other)
        {
            if (other.GetType() == typeof(Minterm))
            {
                return CompareTo((Minterm)other);
            }
            return base.CompareTo(other);
        }

        public int CompareTo(Minterm other)
        {
            var tEn = lAtoms.GetEnumerator();
            var oEn = other.lAtoms.GetEnumerator();

            if (containsConstTrue != other.containsConstTrue)
            {
                if (containsConstTrue && other.lAtoms.Count > 0)
                {
                    tEn.MoveNext();
                }
                else if (other.containsConstTrue && lAtoms.Count > 0)
                {
                    oEn.MoveNext();
                }
            }

            while (tEn.MoveNext())
            {
                if (!oEn.MoveNext())
                {
                    tEn.Dispose();
                    oEn.Dispose();
                    return 1;
                }
                int diff = tEn.Current.Key.CompareTo(oEn.Current.Key);
                if (diff == 0)
                {
                    if (tEn.Current.Key.State != oEn.Current.Key.State)
                    {
                        if (tEn.Current.Key.State == true)
                        {
                            tEn.Dispose();
                            oEn.Dispose();
                            return 1;
                        }
                        else
                        {
                            tEn.Dispose();
                            oEn.Dispose();
                            return -1;
                        }
                    }
                }
                else
                {
                    return diff;
                }
            }
            if (oEn.MoveNext())
            {
                tEn.Dispose();
                oEn.Dispose();
                return -1;
            }
            tEn.Dispose();
            oEn.Dispose();
            return 0;
        }

        public override LogicAtom Expand()
        {
            if (containsConstFalse)
            {
                return LogicConstantValue.False;
            }
            if (containsConstTrue)
            {
                lAtoms.Remove(LogicConstantValue.True);
            }
            Minterm expandedEqs = new Minterm();
            Minterm expandedVars = new Minterm();

            foreach (var pair in lAtoms)
            {
                LogicAtom ex = pair.Key.Expand();
                if (ex != null)
                {
                    Type exType = ex.GetType();
                    if (exType == typeof(ANFEquation))
                    {
                        if (ex.ContainedAtoms > 0)
                        {
                            expandedEqs.AddAtom(ex);
                        }
                    }
                    else if (exType == typeof(LogicVariable))
                    {
                        expandedVars.AddAtom(ex);
                    }
                    else if (exType == typeof(Minterm))
                    {
                        expandedVars.Union((Minterm)ex);
                    }
                }
            }

            if (expandedVars.ContainsConstantFalse || expandedEqs.ContainsConstantFalse)
            {
                return LogicConstantValue.False;
            }

            if (expandedEqs.ContainedAtoms == 0)
            {
                if (containsConstTrue && expandedVars.ContainedAtoms == 0)
                {
                    return LogicConstantValue.True;
                }
                else if (expandedVars.ContainedAtoms == 1)
                {
                    return expandedVars.lAtoms.First().Key;
                }
                return expandedVars;
            }

            ANFEquation finalEq;
            var e = expandedEqs.lAtoms.GetEnumerator();

            e.MoveNext();
            finalEq = (ANFEquation)e.Current.Key;
            if (expandedVars.ContainedAtoms != 0)
            {
                finalEq.Distribute(expandedVars);
            }
            while (e.MoveNext())
            {
                finalEq.Distribute((ANFEquation)e.Current.Key);
            }
            e.Dispose();

            return finalEq;
        }

        public override LogicAtom DeepCopy()
        {
            Minterm mt = new Minterm();

            foreach (var pair in lAtoms)
            {
                LogicAtom a = pair.Key.DeepCopy();
                mt.AddAtom(a);
            }

            return mt;
        }

        public bool ContainsAllOf(Minterm other)
        {
            if (ContainsConstantFalse)
            {
                if (other.ContainsConstantFalse)
                {
                    return true;
                }
                return false;
            }
            var oen = other.lAtoms.GetEnumerator();
            var ten = lAtoms.GetEnumerator();

            //if (containsConstTrue != other.containsConstTrue)
            //{
            //    if (containsConstTrue && other.lAtoms.Count > 0)
            //    {
            //        ten.MoveNext();
            //    }
            //}


            while (oen.MoveNext())
            {
                if (oen.Current.Key.GetType() == typeof(LogicConstantValue) && oen.Current.Key.State)
                {
                    if (this.lAtoms.Count > 0)
                    {
                        continue;
                    }
                    oen.Dispose();
                    ten.Dispose();
                    return false;
                }

                while (ten.MoveNext() && ten.Current.Key.CompareTo(oen.Current.Key) < 0)
                { }

                if (ten.Current.Key == null || ten.Current.Key.CompareTo(oen.Current.Key) != 0)
                {
                    oen.Dispose();
                    ten.Dispose();
                    return false;
                }
            }
            oen.Dispose();
            ten.Dispose();
            return true;
        }

        public Minterm GetOtherMinusThis(Minterm other)
        {
            if (containsConstFalse)
            {
                throw new Exception("Logic Error!");
            }

            other = (Minterm)other.DeepCopy();
            if (other.lAtoms.Count == 0)
            {
                return other;
            }

            var en = lAtoms.GetEnumerator();
            while (en.MoveNext())
            {
                other.lAtoms.Remove(en.Current.Key);
            }
            en.Dispose();
            other.AddAtom(LogicConstantValue.True);
            return other;
        }

        public override void SubIn(string VarID, bool state)
        {
            LogicVariable removeThis = null;
            foreach (var pair in lAtoms)
            {
                if (pair.Key.GetType() != typeof(LogicVariable))
                {
                    pair.Key.SubIn(VarID, state);
                }
                else if (removeThis == null)
                {
                    LogicVariable v = (LogicVariable)pair.Key;
                    if (v.ID == VarID)
                    {
                        if (state)
                        {
                            removeThis = v;
                        }
                        else
                        {
                            AddAtom(LogicConstantValue.False);
                            return;
                        }
                    }
                }
            }

            if (removeThis != null)
            {
                lAtoms.Remove(removeThis);
                if (lAtoms.Count == 0)
                {
                    AddAtom(LogicConstantValue.True);
                }
            }
        }
    }
}
