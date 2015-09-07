using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitShiftCalculator
{
    public static class BitExtentions
    {
    }

    class LogicEquationReducer
    {
        private XORCollector32Bit[] xorCollector;

        public LogicEquationReducer(XORCollector32Bit[] xorCollector)
        {
            this.xorCollector = xorCollector;

            
        }

        public class LogicTermOld : IComparable<LogicTermOld>
        {
            //public const int ConstZero = -1;
            public const int ConstOne = -2;

            public bool IsConstOne { get; private set; }
            //public bool IsConstZero 
            //{ 
            //    get 
            //    {
            //        return (varIDs.Count == 0);
            //    } 
            //}
            private SortedDictionary<int, object> varIDs;

            public LogicTermOld()
            {
                varIDs = new SortedDictionary<int, object>();
                IsConstOne = false;
            }

            public void AddVarID(int id)
            {
                if (id == ConstOne)
                {
                    if (varIDs.Count == 0)
                    {
                        IsConstOne = true;
                    }
                    // do nothing otherwise
                }
                else if (id >= 0)
                {
                    varIDs[id] = null;
                }
                else
                {
                    throw new Exception("Invalid VarID");
                }
            }

            public void ToStringBuilder(StringBuilder sb)
            {
                if (IsConstOne)
                {
                    sb.Append("ValTrue");
                }
                else
                {
                    foreach (var pair in varIDs)
                    {
                        sb.Append(pair.Key);
                        sb.Append(".");
                    }
                    if (varIDs.Count == 0)
                    {
                        sb.Append("ValFalse");
                    }
                    else
                    {
                        sb.Length -= 1;
                    }
                }
            }

            public bool ContainsVarID(int varID)
            {
                return varIDs.ContainsKey(varID);
            }

            public IEnumerable<int> GetAllVarIDs()
            {
                return varIDs.Keys;
            }

            public static LogicTermOld GetNewConstOne()
            {
                LogicTermOld r = new LogicTermOld();
                r.AddVarID(ConstOne);
                return r;
            }

            private bool CopyKeysTo(LogicTermOld term, int subVarID, int reducer1, int reducer2, bool withScan = false)
            {
                bool reducer1Found = false;
                bool reducer2Found = false;

                if (withScan)
                {
                    foreach (var pair in term.varIDs)
                    {
                        if (pair.Key == reducer1)
                        {
                            if (reducer1 >= 0)
                                reducer1Found = true;
                        }
                        else if (pair.Key == reducer2)
                        {
                            if (reducer2 >= 0)
                                reducer2Found = true;
                        }
                    }
                }

                foreach (var pair in this.varIDs)
                {
                    if (pair.Key == reducer1)
                    {
                        if (reducer2Found)
                        {
                            return false;
                        }
                        if (reducer1 >= 0)
                            reducer1Found = true;
                    }
                    else if (pair.Key == reducer2)
                    {
                        if (reducer1Found)
                        {
                            return false;
                        }
                        if (reducer2 >= 0)
                            reducer2Found = true;
                    }

                    if (pair.Key != subVarID)
                    {
                        term.varIDs[pair.Key] = null;
                    }
                }
                return true;
            }

            public LogicTermOld GetUnionWith(LogicTermOld other, int subVarID, int reducer1, int reducer2)
            {
                LogicTermOld c = new LogicTermOld();
                if (this.IsConstOne && other.IsConstOne)
                {
                    c.IsConstOne = true;
                }
                else if (this.IsConstOne)
                {
                    if (!other.CopyKeysTo(c, subVarID, reducer1, reducer2))
                    {
                        return null;
                    }
                }
                else if (other.IsConstOne)
                {
                    if (!this.CopyKeysTo(c, subVarID, reducer1, reducer2))
                    {
                        return null;
                    }
                }
                else
                {
                    if (!this.CopyKeysTo(c, subVarID, reducer1, reducer2))
                    {
                        return null;
                    }
                    if (!other.CopyKeysTo(c, subVarID, reducer1, reducer2, true))
                    {
                        return null;
                    }
                }
                if (c.varIDs.Count == 0 && (this.IsConstOne || other.IsConstOne))
                {
                    c.IsConstOne = true;
                }
                return c;
            }

            public LogicTermOld DeepCopy()
            {
                LogicTermOld c = new LogicTermOld();
                c.IsConstOne = IsConstOne;
                foreach (KeyValuePair<int, object> pair in varIDs)
                {
                    c.varIDs.Add(pair.Key, null);
                }
                return c;
            }

            public int GetSingleVarID()
            {
                if (varIDs.Count == 1)
                {
                    var t = varIDs.First();
                    if (t.Key < 0)
                    {
                        throw new Exception("This term only has a constant/coefficient in it.");
                    }
                    return t.Key;
                }
                throw new Exception("This term has no VarIDs or too many.");
            }

            public bool IsSingleVar()
            {
                if (varIDs.Count == 1)
                {
                    var t = varIDs.First();
                    if (t.Key < 0)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            public bool IsSingleVar(int varID)
            {
                if (varIDs.Count == 1)
                {
                    return varIDs.ContainsKey(varID);
                }
                return false;
            }

            public int CompareTo(LogicTermOld other)
            {
                if (!this.IsConstOne && this.varIDs.Count == 0)
                {
                    throw new Exception("Debug this!");
                }
                if (!other.IsConstOne && other.varIDs.Count == 0)
                {
                    throw new Exception("Debug this!");
                }

                SortedDictionary<int, object>.Enumerator thisEn = varIDs.GetEnumerator();
                SortedDictionary<int, object>.Enumerator otherEn = other.varIDs.GetEnumerator();

                while (thisEn.MoveNext())
                {
                    if (!otherEn.MoveNext())
                    {
                        thisEn.Dispose();
                        otherEn.Dispose();
                        return 1;
                    }
                    int diff = thisEn.Current.Key - otherEn.Current.Key;
                    if (diff != 0)
                    {
                        thisEn.Dispose();
                        otherEn.Dispose();
                        return diff;
                    }
                }
                if (otherEn.MoveNext())
                {
                    thisEn.Dispose();
                    otherEn.Dispose();
                    return -1;
                }
                thisEn.Dispose();
                otherEn.Dispose();
                return 0;
            }
        }

        private class LogicEquationOld
        {
            private SortedDictionary<LogicTermOld, object> terms = new SortedDictionary<LogicTermOld, object>();

            public int SubVarID
            {
                get;
                private set;
            }
            public int TermCount
            {
                get
                {
                    return terms.Count;
                }
            }

            public LogicEquationOld()
            {
                SubVarID = -1;
            }
            private LogicEquationOld(int subVarID)
            {
                this.SubVarID = subVarID;
            }

            public void ToStringBuilder(StringBuilder sb)
            {
                if (SubVarID >= 0)
                {
                    sb.Append(SubVarID);
                    sb.Append(" = ");
                }
                foreach (var v in terms)
                {
                    v.Key.ToStringBuilder(sb);
                    sb.Append(" | ");
                }
                if (terms.Count == 0)
                {
                    sb.Append("ValFalse");
                }
                else
                {
                    sb.Length -= 3;
                }
            }

            public void AddTerm(LogicTermOld term)
            {
                if (!term.IsConstOne && term.GetAllVarIDs().Count() == 0)
                    throw new Exception("Debug This!");
                if (!RemoveTerm(term))
                {
                    terms.Add(term, null);
                }
            }

            public bool RemoveTerm(LogicTermOld term)
            {
                return terms.Remove(term);
            }

            public void SubInEq(LogicEquationOld sub, int reducer1, int reducer2)
            {
#if DEBUG
                if (sub.SubVarID < 0)
                    throw new Exception("You are trying to sub in an LogicEquation that is not preped for subbing.");
#endif
                bool isConstant = false;
                bool constValue = false;

                var subTerms = sub.terms.Keys.ToArray();
                if (subTerms.Length == 0)
                {
                    isConstant = true;
                    constValue = false;
                }
                else if (subTerms.Length == 1 && subTerms[0].IsConstOne)
                {
                    isConstant = true;
                    constValue = true;
                }
                var thisTerms = terms.Keys.ToArray();

                for (int i = 0; i < thisTerms.Length; i++)
                {
                    if (thisTerms[i].ContainsVarID(sub.SubVarID))
                    {
                        if (isConstant)
                        {
                            if (constValue) // true AND term == term 
                            {               // we passed in a single term as a sub that was a constant true so just remove
                                // that VarID from any terms that already have it.
                                var varids = thisTerms[i].GetAllVarIDs();
                                LogicTermOld tmp = new LogicTermOld();
                                bool empty = true;
                                foreach (int varID in varids)
                                {
                                    if (varID != sub.SubVarID)
                                    {
                                        tmp.AddVarID(varID);
                                        empty = false;
                                    }
                                }
                                if (!empty)
                                {
                                    AddTerm(tmp);
                                }
                            }
                            // false AND term == false => false XOR eq == eq
                            // so do nothing if false and let this term be removed.
                        }
                        else
                        {
                            foreach (LogicTermOld subTerm in subTerms)
                            {
                                LogicTermOld term = subTerm.GetUnionWith(thisTerms[i], sub.SubVarID, reducer1, reducer2);
                                if (term != null)
                                {
                                    AddTerm(term);
                                }
                            }
                        }
                        terms.Remove(thisTerms[i]);
                    }
                }
            }

            public IEnumerable<int> GetSingleTermVarIDs()
            {
                var termsEnumer = terms.GetEnumerator();
                if (termsEnumer.MoveNext())
                {
                    LogicTermOld t1 = termsEnumer.Current.Key;
                    if (t1.IsConstOne)
                    {
                        if (!termsEnumer.MoveNext())
                        {
                            termsEnumer.Dispose();
                            throw new Exception("This equation had no terms in it with VarIDs");
                        }
                        t1 = termsEnumer.Current.Key;
                    }
                    if (termsEnumer.MoveNext())
                    {
                        termsEnumer.Dispose();
                        throw new Exception("This equation had more than one term in it with VarIDs");
                    }
                    termsEnumer.Dispose();
                    return t1.GetAllVarIDs();
                }
                termsEnumer.Dispose();
                throw new Exception("This equation has no terms in it.");
            }

            public bool Contains(LogicTermOld term)
            {
                return terms.ContainsKey(term);
            }

            public bool IsFullyReduced
            {
                get
                {
                    if (terms.Count == 0)
                    {
                        return true;
                    }
                    else if (terms.Count == 1 && (terms.First().Key.IsConstOne))
                    {
                        return true;
                    }
                    return false;
                }
            }

            public LogicEquationOld CreateSubEq(bool encryptedBitState)
            {
                LogicEquationOld r = new LogicEquationOld();
                LogicTermOld hi = null;
                foreach (KeyValuePair<LogicTermOld, object> pair in terms)
                {
                    if (pair.Key.IsSingleVar())
                    {
                        if (hi != null)
                        {
                            r.terms.Add(hi.DeepCopy(), null);
                        }
                        hi = pair.Key;
                    }
                    else
                    {
                        r.terms.Add(pair.Key.DeepCopy(), null);
                    }
                }
                if (hi == null)
                {
                    throw new Exception("Cannot make a subsitution equation from this equation.");
                }

                r.SubVarID = hi.GetSingleVarID();
                if (encryptedBitState)
                {
                    r.AddTerm(LogicTermOld.GetNewConstOne());
                }
                return r;
            }

            public LogicEquationOld CreateSubEq(int subVarID, bool encryptedBitState)
            {
                LogicEquationOld r = new LogicEquationOld(subVarID);
                foreach (KeyValuePair<LogicTermOld, object> pair in terms)
                {
                    if (!pair.Key.IsSingleVar(subVarID))
                    {
                        r.terms.Add(pair.Key.DeepCopy(), null);
                    }
                }
                if (encryptedBitState)
                {
                    r.AddTerm(LogicTermOld.GetNewConstOne());
                }
                return r;
            }
        }

        public uint[] TryThree(uint[] encryptedData, StringBuilder log = null)
        {
            LogicEquationOld[] eq = new LogicEquationOld[encryptedData.Length * 32];
            LogicEquationOld[] eqSubs = new LogicEquationOld[encryptedData.Length * 32];

            for (int crptBitID = 0; crptBitID < eq.Length; crptBitID++)
            {
                eq[crptBitID] = new LogicEquationOld();
                foreach (var x in xorCollector[crptBitID / 32].xorOperations[crptBitID % 32])
                {
                    LogicTermOld nxt = new LogicTermOld();
                    nxt.AddVarID(x.Key.FirstVarID);
                    if (x.Key.FirstVarID >= 0 && x.Key.SecondVarID >= 0)
                    {
                        nxt.AddVarID(x.Key.SecondVarID);
                    }
                    eq[crptBitID].AddTerm(nxt);
                }
            }

            // solving this devolves into 3 cases when an equation consists of 1 LogicTerm with 2 VariableIDs and possibly
            // 1 ConstOne (true state) LogicTerm.  Each of the 3 cases simplifies the equation at index+2 to a single LogicTerm
            // with 2 VariableIDs and possibly 1 ConstOne LogicTerm.  Thus we can repeat this cycle over and over till we solve
            // the encryption.
            // Cases:
            // #1  Bit0 = True -> Bit1 = True -> Bit3 = ab^cd

            LogicTermOld constOne = LogicTermOld.GetNewConstOne();
            int reducer1 = -1;
            int reducer2 = -1;

            bool bitState = encryptedData.GetBitState(0);
            //if (eq[0].Contains(constOne))
            //{
            //    bitState = !bitState;
            //}
            if (bitState) // case #1
            {
                var solvedIDs = eq[0].GetSingleTermVarIDs();

                foreach (int id in solvedIDs)
                {
                    LogicEquationOld e = new LogicEquationOld();
                    LogicTermOld t = new LogicTermOld();
                    t.AddVarID(id);
                    e.AddTerm(t);
                    eqSubs[id] = e.CreateSubEq(id, true);
                    for (int j = 0; j < eq.Length; j++)
                    {
                        if (eq[j] != null)
                        {
                            eq[j].SubInEq(eqSubs[id], reducer1, reducer2);
                        }
                    }
                }

                int lastIndex = encryptedData.Length * 32 - 2;
                if (encryptedData.GetBitState(lastIndex))
                {
                    solvedIDs = eq[lastIndex].GetSingleTermVarIDs();

                    foreach (int id in solvedIDs)
                    {
                        LogicEquationOld e = new LogicEquationOld();
                        LogicTermOld t = new LogicTermOld();
                        t.AddVarID(id);
                        e.AddTerm(t);
                        eqSubs[id] = e.CreateSubEq(id, true);
                        for (int j = 0; j < eq.Length; j++)
                        {
                            if (eq[j] != null)
                            {
                                eq[j].SubInEq(eqSubs[id], reducer1, reducer2);
                            }
                        }
                    }
                }
                else
                {
                    solvedIDs = eq[lastIndex].GetSingleTermVarIDs();
                    reducer1 = solvedIDs.First();
                    reducer2 = solvedIDs.Last();
                }
            }
            else
            {
                bitState = encryptedData.GetBitState(1);
                if (bitState) // case #2
                {
                    var solvedIDs = eq[0].GetSingleTermVarIDs();

                    int largestVarID = -1;
                    LogicEquationOld e = new LogicEquationOld();
                    LogicTermOld t = new LogicTermOld();
                    foreach (int id in solvedIDs)
                    {
                        t.AddVarID(id);
                        e.AddTerm(t);
                        largestVarID = id; // this is already sorted lowest to highest;
                    }
                    eqSubs[largestVarID] = e.CreateSubEq(largestVarID, true);
                    for (int j = 0; j < eq.Length; j++)
                    {
                        if (eq[j] != null)
                        {
                            eq[j].SubInEq(e, reducer1, reducer2);
                        }
                    }
                }
                else // case # 3
                {
                    throw new Exception("Let's be honest; I'm not smart enough to figure this one out.");
                }
            }

            int split = encryptedData.Length * 16;
            if (reducer1 >= 0)
            {
                split *= 2;
            }

            for (int i = 1; i < split; i++)
            {
                bitState = encryptedData.GetBitState(i);
                if (!eq[i].IsFullyReduced)
                {
                    LogicEquationOld subEq = eq[i].CreateSubEq(bitState);

                    if (eqSubs[subEq.SubVarID] != null)
                    {
                        throw new Exception("Unexpected Logic Path.");
                    }
                    eqSubs[subEq.SubVarID] = subEq;
                    if (log != null)
                    {
                        subEq.ToStringBuilder(log);
                        log.AppendLine();
                    }

                    for (int j = 0; j < encryptedData.Length * 32; j++)
                    {
                        eq[j].SubInEq(subEq, reducer1, reducer2);
                    }
                    for (int j = 0; j < encryptedData.Length * 32; j++)
                    {
                        if (eqSubs[j] != null)
                        {
                            eqSubs[j].SubInEq(subEq, reducer1, reducer2);
                        }
                    }
                }
            }

            //eq[encryptedData.Length * 32 - 2].SubInEq(eqSubs[encryptedData.Length * 32 - 1]);

            for (int i = encryptedData.Length * 32 - 2; i >= split; i--)
            {
                bitState = encryptedData.GetBitState(i);
                if (!eq[i].IsFullyReduced)
                {
                    LogicEquationOld subEq = eq[i].CreateSubEq(bitState);

                    if (eqSubs[subEq.SubVarID] != null)
                    {
                        throw new Exception("Unexpected Logic Path.");
                    }
                    eqSubs[subEq.SubVarID] = subEq;
                    if (log != null)
                    {
                        subEq.ToStringBuilder(log);
                        log.AppendLine();
                    }

                    for (int j = 0; j < encryptedData.Length * 32 - 1; j++)
                    {
                        eq[j].SubInEq(subEq, reducer1, reducer2);
                    }
                    for (int j = 0; j < encryptedData.Length * 32; j++)
                    {
                        if (eqSubs[j] != null)
                        {
                            eqSubs[j].SubInEq(subEq, reducer1, reducer2);
                        }
                    }
                }
            }

            uint[] r = new uint[eqSubs.Length / 32];
            for (int i = 0; i < eqSubs.Length; i++)
            {
                if (eqSubs[i] != null)
                {
                    if (eqSubs[i].TermCount == 1)
                    {
                        r[i / 32] |= (1u << (i % 32));
                    }
                    else if (eqSubs[i].TermCount > 1)
                    {
                        //throw new Exception("Tell me why!");
                    }
                }
            }
            return r;
        }

        public void FigureItOut(uint[] encryptedData)
        {
            bool bitState = false;
            //bool xor1state = false;
            SortedDictionary<ANDCombination, object> eq = new SortedDictionary<ANDCombination, object>();
            bool[] usedVarIDs = new bool[encryptedData.Length * 32];

            for (int i = 0; i < xorCollector.Length * 32; i++)
            {
                if (xorCollector[i / 32].xorOperations[i % 32].Count > 0)
                {
                    if ((encryptedData[i / 32] >> (i % 32) & 1u) == 1u)
                    {
                        bitState = !bitState;
                    }

                    
                    foreach (KeyValuePair<ANDCombination, object> pair in xorCollector[i / 32].xorOperations[i % 32])
                    {
                        ANDCombination tmp;
                        if (pair.Key.FirstVarID == -2)
                        {
                            //xor1state = !xor1state;
                            throw new Exception("Unexpected Data");
                        }
                        else if (pair.Key.SecondVarID >= 0)
                        {
                            tmp = new ANDCombination(pair.Key.FirstVarID, pair.Key.SecondVarID);
                            usedVarIDs[pair.Key.FirstVarID] = true;
                            usedVarIDs[pair.Key.SecondVarID] = true;
                        }
                        else
                        {
                            tmp = new ANDCombination(pair.Key.FirstVarID);
                            usedVarIDs[pair.Key.FirstVarID] = true;
                        }

                        if (!eq.Remove(tmp)) // remove duplicates because: A ^ A ^ B == 0 ^ B == B
                        {
                            eq.Add(tmp, null);
                        }
                    }
                }
            } 
            // using the properties of XOR and treating every ANDCombination as a single variable I can
            // add all the equations together.  On one side I end up with "bitState" on the other I have all the 
            // ANDCombinations XOR'd against each other

            /* DataStructure Layout
               origin (0, 0) is top left of the Harvard Chart
             * chart[x, y] is
               varID0, varID1... varIDn, varID0 + varID1, varID0 + varID2... varID0 + varIDn, varID1 + varID2, varID1 + varID3...
             
             */
        }
    }
}
