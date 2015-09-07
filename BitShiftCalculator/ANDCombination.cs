using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitShiftCalculator
{
    public class ANDCombination : IComparable<ANDCombination>, IEquatable<ANDCombination>
    {
        private const int ConstZeroID = -1;
        private const int ConstOneID = -2;
        private const int ConstUndefinedID = -3;

        private int firstVarID;
        private int secondVarID;
        //private bool secondVarDefined;

        public ANDCombination(int firstVarID)
            : this(firstVarID, ConstUndefinedID, 0)
        {
#if DEBUG
            if (firstVarID < 0)
                throw new ArgumentException("firstVarID must be non-negative.");
#endif
        }
        public ANDCombination(int firstVarID, int secondVarID)
            : this(firstVarID, secondVarID, 0)
        {
#if DEBUG
            if (firstVarID < 0)
                throw new ArgumentException("firstVarID must be non-negative.");
            if (secondVarID < 0)
                throw new ArgumentException("secondVarID must be non-negative.");
#endif
        }
        private ANDCombination(int firstVarID, int secondVarID, int filler)
        {
            if (secondVarID >= 0 && secondVarID < firstVarID)
            {
                this.firstVarID = secondVarID;
                this.secondVarID = firstVarID;
            }
            else
            {
                this.firstVarID = firstVarID;
                this.secondVarID = secondVarID;
            }
        }
        private ANDCombination(bool createConstOne)
        {
            secondVarID = ConstUndefinedID;
            //secondVarDefined = false;
            if (createConstOne)
            {
                this.firstVarID = ConstOneID;
            }
            else
            {
                this.firstVarID = ConstZeroID;
            }
        }

        public static ANDCombination CreateConstZero()
        {
            return new ANDCombination(false);
        }
        public static ANDCombination CreateConstOne()
        {
            return new ANDCombination(true);
        }

        public bool IsZero
        {
            get
            {
                if (firstVarID == ConstZeroID)
                {
                    return true;
                }
                return false;
            }
        }
        public bool IsOne
        {
            get
            {
                if (firstVarID == ConstOneID)
                {
                    return true;
                }
                return false;
            }
        }

        public int FirstVarID
        {
            get
            {
#if DEBUG
                if (firstVarID < 0)
                    throw new Exception("This ANDCombination is a constant value and has no variable IDs associated with it.");
#endif
                return firstVarID;
            }
        }

        public int SecondVarID
        {
            get
            {
#if DEBUG
                if (secondVarID < 0)
                    throw new Exception("This ANDCombination has no second variable ID associated with it.");
#endif
                return secondVarID;
            }
        }

        /// <summary>
        /// Produces the product of AND'ing this instance with the passed in instance.  The result replaces the values
        /// in this instance.
        /// 
        /// This method automatically reduces the data and places it in a sorted fashion such that:
        /// FirstVarID Less/= SecondVarID
        /// AND'ing with a ConstZero results in ConstZero
        /// AND'ing with a ConstOne will result in the single remaining VarID in the FirstVarID and SecondVarID is undefined.
        /// </summary>
        /// <param name="second"></param>
        public void ANDWith(ANDCombination second)
        {
#if DEBUG
            if (secondVarID >= 0 && second.firstVarID >= 0) // ANDing with 0 or 1 will not result in more data than we can handle
                throw new Exception("This ANDCombination already has a SecondVarID associated with it.");
            if (second.secondVarID >= 0)
                throw new Exception("Passed in ANDCombination has more than one VarID associated with it.");
#endif
            if (second.firstVarID == ConstZeroID || firstVarID == ConstZeroID)
            {
                this.firstVarID = ConstZeroID;
                this.secondVarID = ConstUndefinedID;
            }
            else if (second.firstVarID == ConstOneID || firstVarID == second.firstVarID)
            {
                // do nothing
            }
            else if (firstVarID == ConstOneID)
            {
                this.firstVarID = second.firstVarID;
            }
            else if (second.secondVarID <= firstVarID)
            {
                this.secondVarID = second.firstVarID;
            }
            else
            {
                secondVarID = firstVarID;
                firstVarID = second.firstVarID;
            }
        }

        /// <summary>
        /// Provides a unique hash while values of firstVarID and secondVarID
        /// are under 2147483644  (2^16 - 4)
        /// 
        /// When used for sorting this creates a sort order where firstVarID is most significant
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int rtn = firstVarID;
            if (secondVarID >= 0)
            {
                rtn <<= 16;
                rtn |= secondVarID & 0x0000FFFF;
            }
            return rtn;
        }

        public int CompareTo(ANDCombination other)
        {
            int rtn = this.firstVarID - other.firstVarID;
            if (rtn == 0)
            {
                return this.secondVarID - other.secondVarID;
            }
            return rtn;
        }

        public bool Equals(ANDCombination other)
        {
            if (this.firstVarID == other.firstVarID)
            {
                if (this.firstVarID < 0 || this.secondVarID == other.secondVarID)
                {
                    return true;
                }
                //else if (this.secondVarID < 0 && other.secondVarID < 0)
                //{
                //    return true;
                //}
            }
            return false;
        }
    }
}
