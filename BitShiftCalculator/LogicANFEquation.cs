using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitShiftCalculator
{

    public class LogicANFEquation
    {
        private SortedDictionary<LogicANDTerm, object> terms = new SortedDictionary<LogicANDTerm, object>();

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

        public LogicANFEquation()
        {
            SubVarID = 0;
        }
        private LogicANFEquation(int subVarID)
        {
            this.SubVarID = subVarID;
        }

        public void ToStringBuilder(StringBuilder sb, string separator = " ^ ")
        {
            if (SubVarID >= 0)
            {
                sb.Append(SubVarID);
                sb.Append(" = ");
            }
            foreach (var v in terms)
            {
                v.Key.ToStringBuilder(sb);
                sb.Append(separator);
            }
            if (terms.Count == 0)
            {
                sb.Append("FALSE");
            }
            else
            {
                sb.Length -= separator.Length;
            }
        }

        public void AddTerm(LogicANDTerm term)
        {
            if (term.IsConstantZero)
            {
                return;
            }
            else if (!RemoveTerm(term))
            {
                terms.Add(term, null);
            }
        }

        public bool RemoveTerm(LogicANDTerm term)
        {
            return terms.Remove(term);
        }

        public void SubInEq(LogicANFEquation other, bool includeInverse = false)
        {
#if DEBUG
            if (other.SubVarID == 0)
                throw new Exception("You are trying to sub in an LogicEquation that is not preped for subbing.");
#endif
            SubInEq(other, false, (object)null);
            if (includeInverse)
            {
                SubInEq(other, true, (object)null);
            }
        }

        private void SubInEq(LogicANFEquation other, bool inverse, object diffSig)
        {
            int otherSubVarID;
            if (inverse)
            {
                otherSubVarID = other.SubVarID * -1;
            }
            else
            {
                otherSubVarID = other.SubVarID;
            }
            var subTerms = other.terms.Keys.ToArray();
            var thisTerms = terms.Keys.ToArray();
            terms.Clear();

            if (subTerms.Length == 0)
            {
                subTerms = new LogicANDTerm[] { LogicANDTerm.ConstantZero };
            }

            for (int i = 0; i < thisTerms.Length; i++)
            {
                if (thisTerms[i].ContainsVarID(otherSubVarID))
                {
                    for (int j = 0; j < subTerms.Length; j++)
                    {
                        AddTerm(thisTerms[i].SubstituteIn(subTerms[j], otherSubVarID));
                    }
                    if (inverse)
                    {
                        AddTerm(thisTerms[i].DeepCopy(otherSubVarID));
                    }
                }
                else
                {
                    AddTerm(thisTerms[i]);
                }
            }
        }

        public IEnumerable<int> GetSingleTermVarIDs()
        {
            var termsEnumer = terms.GetEnumerator();

            while (termsEnumer.MoveNext())
            {
                if (!termsEnumer.Current.Key.IsConstantZero && !termsEnumer.Current.Key.IsConstantOne)
                {
                    LogicANDTerm t1 = termsEnumer.Current.Key;
                    if (termsEnumer.MoveNext())
                    {
                        termsEnumer.Dispose();
                        throw new Exception("This equation had more than one term in it with VarIDs");
                    }
                    termsEnumer.Dispose();
                    return t1.GetAllVarIDs();
                }
            }
            termsEnumer.Dispose();
            throw new Exception("This equation has no terms in it with VarIDs.");
        }

        public bool Contains(LogicANDTerm term)
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
                else
                {
                    foreach (KeyValuePair<LogicANDTerm, object> p in terms)
                    {
                        if (!p.Key.IsConstantZero && !p.Key.IsConstantOne)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }

        public bool GetReducedState()
        {
            if (terms.Count > 1)
            {
                throw new Exception("This Equation is not Fully Reduced");
            }
            else if (terms.Count == 0)
            {
                return false;
            }
            else
            {
                return terms.First().Key.IsConstantOne;
            }
        }

        private LogicANFEquation CreateSubEq(List<LogicANDTerm> possibles, bool encryptedBitState)
        {
            bool works = false;
            int newSubVarID = 0;
            int workIdx;
            for (workIdx = possibles.Count - 1; workIdx >= 0; workIdx--)
            {
                newSubVarID = possibles[workIdx].GetSingleVarID();
                works = true;
                foreach (var pair in terms)
                {
                    if (pair.Key != possibles[workIdx] && pair.Key.ContainsVarID(newSubVarID))
                    {
                        works = false;
                        break;
                    }
                }
                if (works)
                {
                    break;
                }
            }

            if (!works)
            {
                throw new Exception("Cannot make a subsitution equation from this equation.");
            }

            LogicANFEquation r = new LogicANFEquation();
            r.SubVarID = newSubVarID;
            foreach (var pair in terms)
            {
                if (pair.Key != possibles[workIdx])
                {
                    r.terms.Add(pair.Key, null);
                }
            }

            if (encryptedBitState)
            {
                r.AddTerm(LogicANDTerm.ConstantOne);
            }
            return r;
        }

        /// <summary>
        /// Creates an equation that can be subed into other equations using the highest value Variable ID that is the
        /// single Variable ID in a LogicANDTerm.
        /// </summary>
        public LogicANFEquation CreateSubEq(bool encryptedBitState)
        {
            List<LogicANDTerm> possibles = new List<LogicANDTerm>();
            foreach (var pair in terms)
            {
                if (pair.Key.IsSingleVar())
                {
                    possibles.Add(pair.Key);
                }
            }
            return CreateSubEq(possibles, encryptedBitState);
        }

        public LogicANFEquation CreateSubEq(int subVarID, bool encryptedBitState)
        {
            List<LogicANDTerm> possibles = new List<LogicANDTerm>();
            foreach (var pair in terms)
            {
                if (pair.Key.IsSingleVar(subVarID))
                {
                    possibles.Add(pair.Key);
                    break;
                }
            }
            return CreateSubEq(possibles, encryptedBitState);
        }

        public void ANDIntoThis(LogicANFEquation other)
        {
            var thisTerms = terms.Keys.ToArray();
            this.terms.Clear();

            foreach (LogicANDTerm tTerm in thisTerms)
            {
                foreach (LogicANDTerm oTerm in other.terms.Keys)
                {
                    AddTerm(tTerm.Union(oTerm));
                }
            }
        }
    }
}
