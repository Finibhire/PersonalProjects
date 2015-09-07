using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace LogicAlgebra.DataStructures
{
    class ANFEquation : LogicAtom
    {
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
            get { return lAtoms.Count; }
        }

        public ANFEquation()
            : base()
        {
            lAtoms = new SortedDictionary<LogicAtom, LogicAtom>();
        }

        public ANFEquation(string parseData)
            : this()
        {

            Parse(parseData);
        }

        public bool ContainsOnlyConstantFalse
        {
            get
            {
                if (lAtoms.Count == 1)
                {
                    LogicAtom a = lAtoms.First().Key;
                    if (a.GetType() == typeof(LogicConstantValue))
                    {
                        return !a.State;
                    }
                }
                return false;
            }
        }
        public bool ContainsOnlyConstantTrue
        {
            get
            {
                if (lAtoms.Count == 1)
                {
                    LogicAtom a = lAtoms.First().Key;
                    if (a.GetType() == typeof(LogicConstantValue))
                    {
                        return a.State;
                    }
                }
                return false;
            }
        }

        public void Parse(string data)
        {
            pParse(data, 0);
        }

        private int pParse(string data, int startIndex)
        {
            int idx = startIndex;
            StringBuilder newVarID = new StringBuilder();
            Minterm mt = new Minterm();
            
            while (idx < data.Length)
            {
                char nextChar = data[idx];
                switch (nextChar)
                {
                    case '(':
                        ANFEquation newEq = new ANFEquation();
                        idx = newEq.pParse(data, idx + 1);
                        if (data[idx] == '\'')
                        {
                            newEq.State = !newEq.State;
                            idx++;
                        }
                        mt.AddAtom(newEq);
                        break;
                    case ')':
                        if (newVarID.Length > 0)
                        {
                            mt.AddAtom(new LogicVariable(newVarID.ToString()));
                            //newVarID.Clear();
                        }
                        if (mt.ContainedAtoms > 0)
                        {
                            AddAtom(mt);
                            //mt = new Minterm();
                        }
                        return idx;
                    case '~':
                        mt.AddAtom(LogicConstantValue.False);
                        break;
                    case '!':
                        mt.AddAtom(LogicConstantValue.True);
                        break;
                    case '^':
                        if (newVarID.Length > 0)
                        {
                            mt.AddAtom(new LogicVariable(newVarID.ToString()));
                            newVarID.Clear();
                        }
                        if (mt.ContainedAtoms > 0)
                        {
                            AddAtom(mt);
                            mt = new Minterm();
                        }
                        break;
                    default:
                        if (char.IsWhiteSpace(nextChar))
                        {
                            if (newVarID.Length > 0)
                            {
                                LogicVariable lv = new LogicVariable(newVarID.ToString());
                                mt.AddAtom(lv);
                                newVarID.Clear();
                            }
                        }
                        else
                        {
                            newVarID.Append(nextChar);
                        }
                        break;
                }
                idx++;
            }

            if (startIndex != 0)
            {
                throw new Exception("Missing at least one ending parentheses.");
            }
            else
            {
                if (newVarID.Length > 0)
                {
                    mt.AddAtom(new LogicVariable(newVarID.ToString()));
                    //newVarID.Clear();
                }
                if (mt.ContainedAtoms > 0)
                {
                    AddAtom(mt);
                    //mt = new Minterm();
                }
            }
            return 0;
        }

        /// <summary>
        /// returns true if the atom was added to this instance.  False is returned if the atom was removed.
        /// however adding constantvalue true or false will always return true.
        /// </summary>
        /// <param name="atom"></param>
        /// <returns></returns>
        public bool AddAtom(LogicAtom atom)
        {
            if (atom.GetType() == typeof(Minterm))  // reduce a minterm to a constant if we can
            {
                if (atom.ContainedAtoms == 1)
                {
                    Minterm mt = (Minterm)atom;
                    if (mt.ContainsConstantFalse)
                    {
                        atom = LogicConstantValue.False;
                    }
                    else if (mt.ContainsConstantTrue)
                    {
                        atom = LogicConstantValue.True;
                    }
                }
            }
            else if (atom.GetType() == typeof(ANFEquation)) // reduce an equation to a constant if we can
            {
                if (atom.ContainedAtoms == 1)
                {
                    ANFEquation eq = (ANFEquation)atom;
                    if (eq.ContainsOnlyConstantFalse)
                    {
                        atom = LogicConstantValue.False;
                    }
                    else if (eq.ContainsOnlyConstantTrue)
                    {
                        atom = LogicConstantValue.True;
                    }
                }
            }

            if (atom.GetType() == typeof(LogicConstantValue))  // deal with the case were we are adding only a constant
            {                                                  // to this instance that has only one constant in it already
                if (!atom.State)
                {
                    if (ContainsOnlyConstantFalse)
                    {
                        return true;
                    }
                    lAtoms.Add(LogicConstantValue.False, LogicConstantValue.False);
                    return true;
                }
                else if (atom.State)
                {
                    if (ContainsOnlyConstantTrue)
                    {
                        lAtoms.Clear();
                        lAtoms.Add(LogicConstantValue.False, LogicConstantValue.False);
                        return true;
                    }
                    if (!lAtoms.Remove(LogicConstantValue.True))
                    {
                        lAtoms.Add(LogicConstantValue.True, LogicConstantValue.True);
                    }
                    return true;
                }
            }

            if (ContainsOnlyConstantFalse)  // we are adding something that will make this pointless to have in the
            {                               // equation because: false XOR B == B
                lAtoms.Clear();
            }

            if (!lAtoms.Remove(atom))
            {
                lAtoms.Add(atom, atom);
                return true;
            }
            else if (lAtoms.Count == 0)  // we cancled the last item in this equation so this equation should result in a
            {                            // state that contains a ConstantFalse.
                lAtoms.Add(LogicConstantValue.False, LogicConstantValue.False);
            }
            // else we removed it from the list so do nothing more because: A XOR A XOR B == 0 XOR B == B
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendToString(sb);

            if (State)
            {
                sb.Remove(0, 1);
                sb.Length -= 1;
            }
            return sb.ToString();
        }

        public override void AppendToString(StringBuilder sb)
        {
            sb.Append('[');
            foreach (var pair in lAtoms)
            {
                pair.Key.AppendToString(sb);
                sb.Append('^');
            }
            sb.Length -= 1;

            sb.Append(']');
            if (!State)
            {
                sb.Append('\'');
            }
        }

        /// <summary>
        /// The items of this instance and inner atoms are mutated to include the distributed item.
        /// The other instance should be preserved in it's original state.
        /// </summary>
        public void Distribute(ANFEquation other)
        {
            if (this.lAtoms.Count == 0 || other.lAtoms.Count == 0)
            {
                throw new Exception("Trying to distribute with an empty equation.");
            }

            LogicAtom[] oldAtoms = lAtoms.Keys.ToArray();
            SortedDictionary<LogicAtom, LogicAtom> otherAtoms = other.lAtoms;
            lAtoms.Clear();
            for (int i = 0; i < oldAtoms.Length; i++)
            {
                LogicAtom iAtom = oldAtoms[i];
                Type iType = iAtom.GetType();
                foreach (var pair in otherAtoms)
                {
                    LogicAtom jAtom = pair.Key;
                    Type jType = jAtom.GetType();
                    
                    if (iType == typeof(LogicConstantValue))
                    {
                        if (iAtom.State)
                        {
                            AddAtom(jAtom.DeepCopy());
                        }
                        else
                        {
                            AddAtom(iAtom);
                        }
                    }
                    else if (jType == typeof(LogicConstantValue))
                    {
                        if (jAtom.State)
                        {
                            AddAtom(iAtom.DeepCopy());
                        }
                        else
                        {
                            AddAtom(jAtom);
                        }
                    }
                    else if (jType == typeof(ANFEquation))
                    {
                        if (iType == typeof(LogicVariable))
                        {
                            ANFEquation x = (ANFEquation)jAtom.DeepCopy();
                            x.Distribute((LogicVariable)iAtom);
                            AddAtom(x);
                        }
                        else if (iType == typeof(Minterm))
                        {
                            ANFEquation x = (ANFEquation)jAtom.DeepCopy();
                            x.Distribute((Minterm)iAtom);
                            AddAtom(x);
                        }
                        else if (iType == typeof(ANFEquation))
                        {
                            ANFEquation x = (ANFEquation)jAtom.DeepCopy();
                            x.Distribute((Minterm)iAtom);
                            AddAtom(x);
                        }
                        else
                        {
                            throw new Exception("Unexpected Type!");
                        }
                    }
                    else if (jType == typeof(LogicVariable))
                    {
                        if (iType == typeof(LogicVariable))
                        {
                            if (iAtom.CompareTo(jAtom) == 0)
                            {
                                AddAtom(iAtom.DeepCopy());
                            }
                            else
                            {
                                Minterm mt = new Minterm();
                                mt.AddAtom(iAtom.DeepCopy());
                                mt.AddAtom(jAtom.DeepCopy());
                                AddAtom(mt);
                            }
                        }
                        else if (iType == typeof(Minterm))
                        {
                            Minterm mt = (Minterm)iAtom.DeepCopy();
                            mt.AddAtom(jAtom.DeepCopy());
                            AddAtom(mt);
                        }
                        else if (iType == typeof(ANFEquation))
                        {
                            ANFEquation eq = (ANFEquation)iAtom.DeepCopy();
                            eq.Distribute((LogicVariable)jAtom);
                            AddAtom(eq);
                        }
                        else
                        {
                            throw new Exception("Unexpected Type!");
                        }
                    }
                    else if (jType == typeof(Minterm))
                    {
                        if (iType == typeof(LogicVariable))
                        {
                            Minterm mt = (Minterm)jAtom.DeepCopy();
                            mt.AddAtom(iAtom.DeepCopy());
                            AddAtom(mt);
                        }
                        else if (iType == typeof(Minterm))
                        {
                            Minterm mt = (Minterm)iAtom.DeepCopy();
                            mt.Union((Minterm)jAtom);
                            AddAtom(mt);
                        }
                        else if (iType == typeof(ANFEquation))
                        {
                            ANFEquation eq = (ANFEquation)iAtom.DeepCopy();
                            eq.Distribute((Minterm)jAtom);
                            AddAtom(eq);
                        }
                        else
                        {
                            throw new Exception("Unexpected Type!");
                        }
                    }
                    else
                    {
                        throw new Exception("Unexpected Type!");
                    }
                }
            }
        }

        public void Distribute(Minterm other)
        {
            if (other.ContainsConstantFalse)
            {
                lAtoms.Clear();
                lAtoms.Add(LogicConstantValue.False, LogicConstantValue.False);
                return;
            }
            else if (this.lAtoms.Count == 0 || other.ContainedAtoms == 0)
            {
                throw new Exception("Trying to distribute with an empty equation or minterm.");
            }

            
            LogicAtom[] oldAtoms = lAtoms.Keys.ToArray();
            lAtoms.Clear();
            for (int i = 0; i < oldAtoms.Length; i++)
            {
                if (oldAtoms[i].GetType() == typeof(LogicConstantValue))
                {
                    if (oldAtoms[i].State)
                    {
                        AddAtom(other.DeepCopy());
                    }
                    else
                    {
                        AddAtom(LogicConstantValue.False);
                    }
                }
                else if (oldAtoms[i].GetType() == typeof(Minterm))
                {
                    Minterm mt = (Minterm)oldAtoms[i];
                    mt.Union(other);
                    AddAtom(mt);
                }
                else if (oldAtoms[i].GetType() == typeof(ANFEquation))
                {
                    ((ANFEquation)oldAtoms[i]).Distribute(other);
                    AddAtom(oldAtoms[i]);
                }
            }
        }

        public void Distribute(LogicVariable other)
        {
            if (this.lAtoms.Count == 0)
            {
                throw new Exception("Trying to distribute with an empty equation.");
            }

            LogicAtom[] oldAtoms = lAtoms.Keys.ToArray();
            lAtoms.Clear();
            for (int i = 0; i < oldAtoms.Length; i++)
            {
                if (oldAtoms[i].GetType() == typeof(LogicConstantValue))
                {
                    if (oldAtoms[i].State)
                    {
                        AddAtom(other.DeepCopy());
                    }
                    else
                    {
                        AddAtom(LogicConstantValue.False);
                    }
                }
                else if (oldAtoms[i].GetType() == typeof(Minterm))
                {
                    Minterm mt = (Minterm)oldAtoms[i];
                    mt.AddAtom(other.DeepCopy());
                    AddAtom(mt);
                }
                else if (oldAtoms[i].GetType() == typeof(ANFEquation))
                {
                    ((ANFEquation)oldAtoms[i]).Distribute(other);
                    AddAtom(oldAtoms[i]);
                }
            }
        }

        public override LogicAtom Expand()
        {
            SortedDictionary<LogicAtom, LogicAtom> newAtoms = new SortedDictionary<LogicAtom, LogicAtom>();

            foreach (var pair in lAtoms)
            {
                LogicAtom next = pair.Key.Expand();

                if (next.GetType() == typeof(Minterm) || next.GetType() == typeof(LogicVariable) || next.GetType() == typeof(LogicConstantValue))
                {
                    LogicAtom found = null;
                    if (newAtoms.TryGetValue(next, out found))
                    {
                        newAtoms.Remove(next);
                        if (found.State == next.State)
                        {
                            if (!newAtoms.Remove(LogicConstantValue.True))
                            {
                                newAtoms.Add(LogicConstantValue.True, LogicConstantValue.True);
                            }
                        }
                    }
                    else
                    {
                        newAtoms.Add(next, next);
                    }
                }
                else if (next.GetType() == typeof(ANFEquation))
                {
                    foreach (var pair2 in ((ANFEquation)next).lAtoms)
                    {
                        if (!newAtoms.Remove(pair2.Key))
                        {
                            newAtoms.Add(pair2.Key, pair2.Key);
                        }
                    }
                }
            }

            if (lAtoms.Count > 0 && newAtoms.Count == 0)
            {
                return LogicConstantValue.False;
            }
            if (newAtoms.Count == 1)
            {
                LogicAtom a = newAtoms.First().Key;
                //if (a.ContainedAtoms == 2)
                //    a = a;
                return a;
            }

            lAtoms = newAtoms;
            return this;
        }

        public override LogicAtom DeepCopy()
        {
            ANFEquation eq = new ANFEquation();

            foreach (var pair in lAtoms)
            {
                eq.AddAtom(pair.Key.DeepCopy());
            }

            return eq;
        }

        private class MintermHolder : IComparable<MintermHolder>
        {
            public bool Removed { get; set; }
            public Minterm Minterm { get; set; }

            public int CompareTo(MintermHolder other)
            {
                return Minterm.CompareTo(other.Minterm);
            }
        }
        public ANFEquation Extract(ANFEquation divisor)
        {
            if (divisor == null || divisor.lAtoms.Count == 0)
            {
                throw new DivideByZeroException();
            }
            else if (this.lAtoms.Count == 0 || ContainsOnlyConstantFalse)
            {
                return this;
            }
            ANFEquation finalEq = new ANFEquation();
            ANFEquation te = (ANFEquation)this.DeepCopy().Expand();  // thisExpanded
            LogicAtom a = divisor.DeepCopy().Expand();  // divisorExpanded
            if (a.GetType() == typeof(LogicVariable))
            {
                Minterm b = new Minterm();
                b.AddAtom(a);
                a = b;
            }
            if (a.GetType() == typeof(Minterm))
            {
                ANFEquation b = new ANFEquation();
                b.AddAtom(a);
                a = b;
            }
            divisor = (ANFEquation)a;

            SortedDictionary<MintermHolder, MintermHolder> work = new SortedDictionary<MintermHolder, MintermHolder>();
            SortedDictionary<Minterm, Minterm> remainders = new SortedDictionary<Minterm, Minterm>();

            foreach (var pair in te.lAtoms)
            {
                if (pair.Key.GetType() == typeof(LogicVariable) || pair.Key.GetType() == typeof(LogicConstantValue))
                {
                    MintermHolder mth = new MintermHolder();
                    mth.Minterm = new Minterm();
                    mth.Minterm.AddAtom(pair.Key);
                    work.Add(mth, mth);
                }
                else
                {
                    MintermHolder mth = new MintermHolder();
                    mth.Minterm = (Minterm)pair.Key;
                    work.Add(mth, mth);
                }
            }

            Minterm div;
            Minterm[] rest = new Minterm[divisor.lAtoms.Count - 1];

            bool divContainsSoloTrue = false;
            var en1 = divisor.lAtoms.GetEnumerator();
            en1.MoveNext();
            if (en1.Current.Key.GetType() == typeof(LogicConstantValue))
            {
                if (!en1.Current.Key.State)
                {
                    throw new DivideByZeroException();
                }
                divContainsSoloTrue = true;
                rest = new Minterm[divisor.lAtoms.Count - 2];
                en1.MoveNext();
            }
            if (en1.Current.Key.GetType() == typeof(LogicVariable))
            {
                div = new Minterm();
                div.AddAtom(en1.Current.Key);
            }
            else
            {
                div = (Minterm)en1.Current.Key;
            }
            div.AddAtom(LogicConstantValue.True);
            int i = 0;
            while (en1.MoveNext())
            {
                if (en1.Current.Key.GetType() == typeof(LogicVariable))
                {
                    rest[i] = new Minterm();
                    rest[i].AddAtom(en1.Current.Key);
                }
                else
                {
                    rest[i] = (Minterm)en1.Current.Key;
                }
                rest[i].AddAtom(LogicConstantValue.True);

                if (rest[i].ContainedAtoms <= div.ContainedAtoms)
                {
                    Minterm temp = div;
                    div = rest[i];
                    rest[i] = temp;
                }
                i++;
            }
            en1.Dispose();

            int workDone = 1;
            var en = work.GetEnumerator();
            while (workDone > 0)
            {
                workDone = 0;

                while (en.MoveNext())
                {
                    if (en.Current.Key.Removed)
                    {
                        continue;
                    }
                    if (divContainsSoloTrue && en.Current.Key.Minterm.ContainedAtoms == 1 && en.Current.Key.Minterm.ContainsConstantTrue)
                    {
                        workDone--;
                        finalEq.AddAtom(LogicConstantValue.True);
                        AddRemainder(work, remainders, new MintermHolder() { Minterm = (Minterm)div.DeepCopy() });
                        foreach (Minterm r in rest)
                        {
                            AddRemainder(work, remainders, new MintermHolder() { Minterm = (Minterm)r.DeepCopy() });
                        }
                        continue;
                    }

                    if (en.Current.Key.Minterm.ContainsAllOf(div))
                    {
                        workDone++;
                        work[en.Current.Key].Removed = true;
                        Minterm mt = div.GetOtherMinusThis(en.Current.Key.Minterm);
                        if (!finalEq.AddAtom(mt))
                        {
                            //en.Dispose();
                            //return null;
                        }
                        if (divContainsSoloTrue)
                        {
                            AddRemainder(work, remainders, new MintermHolder() { Minterm = (Minterm)mt.DeepCopy() });
                        }
                        foreach (Minterm r in rest)
                        {
                            MintermHolder remains = new MintermHolder();
                            remains.Minterm = (Minterm)mt.DeepCopy();
                            remains.Minterm.Union(r);
                            AddRemainder(work, remainders, remains);
                        }
                    }
                }
                
                if (workDone <= 0)
                {
                    en.Dispose();
                    return null;
                }

                var reducedWork = work.Where(x => !x.Key.Removed).ToArray();
                work.Clear();
                foreach (var pair in reducedWork)
                {
                    work.Add(pair.Key, pair.Key);
                }

                foreach (var pair in remainders)
                {
                    MintermHolder mth = new MintermHolder();
                    mth.Minterm = pair.Key;
                    if (!work.Remove(mth))
                    {
                        work.Add(mth, mth);
                    }
                }

                if (remainders.Count == 0 && work.Count == 0)
                {
                    // success!
                    en.Dispose();
                    return finalEq;
                }

                remainders.Clear();
                en.Dispose();
                en = work.GetEnumerator();
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddRemainder(
            SortedDictionary<MintermHolder, MintermHolder> work, 
            SortedDictionary<Minterm, Minterm> remainders,
            MintermHolder remains)
        {
            if (work.ContainsKey(remains) && !work[remains].Removed)
            {
                work[remains].Removed = true;
            }
            else
            {
                if (!remainders.Remove(remains.Minterm))
                {
                    remainders.Add(remains.Minterm, remains.Minterm);
                }
            }
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
