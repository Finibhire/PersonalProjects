using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitShiftCalculator
{

    /// <summary>
    /// Immuteable after creation.
    /// </summary>
    public class LogicANDTerm : IComparable<LogicANDTerm>
    {
        private static LogicANDTerm constantZero, constantOne;
        private readonly bool isConstantZero, isConstantOne;

        private readonly int[] varIDs;

        public static LogicANDTerm ConstantZero
        {
            get
            {
                if (constantZero == null)
                {
                    constantZero = new LogicANDTerm(false);
                }
                return constantZero;
            }
        }
        public static LogicANDTerm ConstantOne
        {
            get
            {
                if (constantOne == null)
                {
                    constantOne = new LogicANDTerm(true);
                }
                return constantOne;
            }
        }

        public bool IsConstantZero { get { return isConstantZero; } }
        public bool IsConstantOne { get { return isConstantOne; } }


        /// <summary>
        /// Instances of this class should be non-mutable and if they are mutable that's a programming error.
        /// Pass in a list of VariableIDs that are linked together in a term by AND operations.
        /// VariableIDs must be non-Zero.  Negative VariableIDs indicate a NOT(VariableID) operation.
        /// </summary>
        public LogicANDTerm(IEnumerable<int> VariableIDs)
        {
            varIDs = VariableIDs.Distinct<int>().OrderBy<int, int>(x => x).ToArray();
            isConstantZero = false;
            isConstantOne = false;

            if (varIDs.Length == 0)
            {
                throw new ArgumentException("There must be at least one VariableID in the list provided.", "VariableIDs");
            }
            if (Array.BinarySearch<int>(varIDs, 0) >= 0)
            {
                throw new ArgumentException("VariableIDs must be non-zero.", "VariableIDs");
            }
        }
        private LogicANDTerm(int[] copy)
        {
            varIDs = new int[copy.Length];
            copy.CopyTo(varIDs, 0);
            isConstantZero = false;
            isConstantOne = false;
        }

        /// <summary>
        /// Used to create the static ConstantZero and ConstantOne variables.
        /// </summary>
        private LogicANDTerm(bool ConstantState)
        {
            isConstantZero = !ConstantState;
            isConstantOne = ConstantState;
        }


        public void ToStringBuilder(StringBuilder sb, string separator = ".")
        {
            if (isConstantZero)
            {
                sb.Append("FALSE");
            }
            else if (isConstantOne)
            {
                sb.Append("TRUE");
            }
            else
            {
                for (int i = 0; i < varIDs.Length; i++)
                {
                    sb.Append(varIDs[i]);
                    sb.Append(separator);
                }
                sb.Length -= 1;
            }
        }

        public bool ContainsVarID(int VarID, bool SearchInverseState = false)
        {
            if (isConstantOne || isConstantZero)
            {
                return false;
            }
            bool found = (Array.BinarySearch(varIDs, VarID) >= 0);
            if (found)
            {
                return true;
            }
            else if (SearchInverseState)
            {
                found = (Array.BinarySearch(varIDs, VarID * (-1)) >= 0);
                return found;
            }
            return false;
        }

        public int[] GetAllVarIDs()
        {
            int[] r = new int[varIDs.Length];
            varIDs.CopyTo(r, 0);
            return r;
        }
        public int[] GetAllVarIDs(int removeVarID)
        {
            int idx;
            if ((idx = Array.BinarySearch<int>(varIDs, removeVarID)) >= 0)
            {
                int[] r = new int[varIDs.Length - 1];
                Array.Copy(varIDs, 0, r, 0, idx);
                Array.Copy(varIDs, idx + 1, r, idx, r.Length - idx);
                return r;
            }
            return GetAllVarIDs();
        }

        /// <summary>
        /// Substitute in another LogicANDTerm into this term in place of the Substitution Variable ID.
        /// If the SubstitutionVarID does not exist in this term then this method returns "this" instance.
        /// Otherwise this method unions the two terms together and returns the result in a new LogicANDTerm instance.
        /// </summary>
        public LogicANDTerm SubstituteIn(LogicANDTerm other, int SubstitutionVarID)
        {
            int idx;
            if (this.isConstantZero)
            {
                return LogicANDTerm.ConstantZero;
            }
            else if (this.isConstantOne || (idx = Array.BinarySearch<int>(varIDs, SubstitutionVarID)) < 0)
            {
                return this;
            }
            else
            {
                if (other.isConstantZero)
                {
                    return LogicANDTerm.ConstantZero;
                }
                else if (other.isConstantOne)
                {
                    if (varIDs.Length == 1)
                    {
                        return LogicANDTerm.ConstantOne;
                    }
                    else
                    {
                        return new LogicANDTerm(varIDs.OrderBy<int, int>(x => x).Where<int>(x => x != SubstitutionVarID));
                    }
                }
                else
                {
#if DEBUG
                    idx = Array.BinarySearch<int>(other.varIDs, SubstitutionVarID);
                    if (idx >= 0)
                    {
                        throw new Exception("Other term substiuting into this term has the indicated VariableID.  This operation doesn't make any sense and should be debuged.");
                    }
#endif
                    var unionVarIDs = varIDs
                        .Union<int>(other.varIDs)
                        .Distinct<int>()
                        .Where<int>(x => x != SubstitutionVarID)
                        .ToArray<int>();
                    return new LogicANDTerm(unionVarIDs);
                }
            }
        }

        public LogicANDTerm Union(LogicANDTerm other)
        {
            if (this.isConstantZero || other.isConstantZero)
            {
                return LogicANDTerm.ConstantZero;
            }
            else if (this.isConstantOne)
            {
                return other;
            }
            else if (other.isConstantOne)
            {
                return this;
            }
            return new LogicANDTerm(varIDs.Union(other.varIDs));
        }

        public int GetSingleVarID()
        {
            if (varIDs.Length == 1)
            {
                return varIDs[0];
            }
            throw new Exception("This term has no VarIDs or too many.");
        }

        public bool IsSingleVar()
        {
            if (varIDs.Length == 1)
            {
                return true;
            }
            return false;
        }
        public bool IsSingleVar(int varID)
        {
            if (varIDs.Length == 1)
            {
                return varIDs[0] == varID;
            }
            return false;
        }

        public LogicANDTerm DeepCopy()
        {
            if (this.isConstantZero || this.isConstantOne)
            {
                return this;
            }
            else
            {
                return new LogicANDTerm(varIDs);
            }
        }

        public LogicANDTerm DeepCopy(int removeVarID)
        {
            if (this.isConstantZero || this.isConstantOne || !this.ContainsVarID(removeVarID))
            {
                throw new Exception("Serious Logic Error!  I should not be deapcopying while trying to remove a varID when the VarID doesn't exist!");
                //return this;
            }
            else
            {
                return new LogicANDTerm(varIDs.Where(x => x != removeVarID));
            }
        }

        public int CompareTo(LogicANDTerm other)
        {
            bool test1 = (this.isConstantOne || this.isConstantZero);
            bool test2 = (other.isConstantOne || other.isConstantZero);
            if (this.isConstantOne && other.isConstantZero)
            {
                return 1;
            }
            else if (other.isConstantOne && this.isConstantZero)
            {
                return -1;
            }
            else if (test1)
            {
                if (test2)
                {
                    return 0;
                }
                return -1;
            }
            else if (test2)
            {
                return 1;
            }

            int diff;
            for (int i = 0; i < varIDs.Length && i < other.varIDs.Length; i++)
            {
                diff = this.varIDs[i] - other.varIDs[i];
                if ( diff != 0 )
                {
                    return diff;
                }
            }

            diff = this.varIDs.Length - other.varIDs.Length;
            return diff;
        }
    }
}
