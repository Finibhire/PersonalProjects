using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitShiftCalculator
{
    //public enum ExtendedBitState
    //{
    //    Unknown = -1, Zero = 0, One = 1
    //}


    class XORCollector32Bit
    {
        private bool[] xor1state;

        public SortedList<ANDCombination, object>[] xorOperations { get; private set; }

        public XORCollector32Bit()
        {
            xor1state = new bool[32];
            xorOperations = new SortedList<ANDCombination, object>[32];
            for (int i = 0; i < 32; i++)
            {
                xorOperations[i] = new SortedList<ANDCombination, object>();
            }
        }

        private bool GetState(uint[] unencryptedState, int index)
        {
            return (unencryptedState[index / 32] >> (index % 32) & 1u) == 1u;
        }

        /// <summary>
        /// Used for debuging.  Returns an output exactly the same as the encryption algorithim would produce
        /// given the collected data about XOR's and AND operations so far.
        /// </summary>
        public uint TestPartialEncryption(uint[] unencryptedState)
        {
            uint result = 0;
            for (int i = 0; i < 32; i++)
            {
                bool state = false;
                foreach (var a in xorOperations[i])
                {
                    if (GetState(unencryptedState, a.Key.FirstVarID) && GetState(unencryptedState, a.Key.SecondVarID))
                    {
                        state = !state;
                    }
                }
                if (state)
                {
                    result |= 1u << i;
                }
            }
            return result;
        }

        public string OutputResults()
        {
            const int varIDlength = 2;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 32; i++)
            {
                sb.Append(i.ToString().PadLeft(2));
                sb.Append(":  ");
                for (int j = 0; j < xorOperations[i].Count; j++)
                {
                    var t = xorOperations[i].ElementAt(j).Key;
                    sb.AppendID(t.FirstVarID, varIDlength);
                    sb.Append('.');
                    sb.AppendID(t.SecondVarID, varIDlength);
                    sb.Append(" | ");
                }
                sb.Length = sb.Length - 2;
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// It doesn't matter how many XOR_0's you have because y ^ 0 = y;
        /// We need to track XOR_1's because y ^ 1 = !y  && y ^ 1 ^ 1 = y;
        /// </summary>
        /// <param name="x"></param>
        public void XOR(BitHolder32 x)
        {
            for (int i = 0; i < 32; i++)
            {
                int ele0 = x.AndOperations[i][0];  // we need to check ele0 before we sort the data
                int ele1 = x.AndOperations[i][1];  // we need to verify consistant data
                if (ele0 == -2)
                {
                    if (ele1 != -2)  // I've programmed in some sort of logic error if this is the case.
                    {
                        throw new Exception("Unexpected Data");
                    }
                    xor1state[i] = !xor1state[i];
                    throw new Exception("Unexpected Data");
                }
                else if (ele0 == -1)
                {
                    // do nothing in this case because A ^ 0 == A
                }
                else if (ele1 == -1) // ele0 should have been set to 0 (-1) also because A & 0 == 0
                {                    // this is definately a logic error on my part.
                    throw new Exception("Unexpected Data");
                }
                else if (ele1 == -2)  // this case is fine because ele0 & 1 == ele0
                {
                    var t = new ANDCombination(ele0, ele1);
                    if (!xorOperations[i].Remove(t))    // if it existed in the xor chain list then remove it
                    {                                   // because (A ^ A ^ B) => (0 ^ B) => (B)
                        xorOperations[i].Add(t, null);  // if it wasn't in the xor chain list then we add it
                    }
                    throw new Exception("Unexpected Logic Path"); // I don't believe I ever set my data to look like this.
                    // it should come in the form A & A, not A & 1.
                }
                else // both ele0 and ele1 must not be a set value of 0 or 1 (-1 or -2) now and refer to bitIDs
                {
                    var t = new ANDCombination(ele0, ele1);

                    // if it existed in the xor chain list then remove it
                    // because (A ^ A ^ B == 0 ^ B) => (B ^ 0 == B)
                    if (!xorOperations[i].Remove(t))
                    {   // if it wasn't in the xor chain list then we add it
                        xorOperations[i].Add(t, null);
                    }
                }
            }
        }


    }
}
