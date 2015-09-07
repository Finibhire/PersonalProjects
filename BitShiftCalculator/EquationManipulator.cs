using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitShiftCalculator
{
    public static class EquationManipulatorExtentions
    {
        public static int VarIDToIndex(this int varID)
        {
            return Math.Abs(varID) - 1;
        }

        public static bool GetBitState(this uint[] data, int index)
        {
            return (data[index / 32] >> (index % 32) & 1u) == 1u;
        }

        public static void SetBitStateTrue(this uint[] data, int index)
        {
            data[index / 32] |= 1u << (index % 32);
        }

        public static void SetBitStateFalse(this uint[] data, int index)
        {
            data[index / 32] &= ~(1u << (index % 32));
        }

        public static void AppendHex(this uint[] x, StringBuilder sb)
        {
            if (x != null)
            {
                foreach (uint y in x)
                {
                    sb.Append(y.ToString("X8"));
                    sb.Append(" ");
                }
                sb.Length -= 1;
            }
        }
    }
    class EquationManipulator
    {
        private SortedDictionary<LogicANDTerm, object> plainOrEq;
        private uint[] encryptedData;
        private LogicANFEquation[] encEqs;

        public int DataSize { get; private set; }
        public uint[] EncryptedData
        {
            get
            {
                uint[] tmp = new uint[encryptedData.Length];
                encryptedData.CopyTo(tmp, 0);
                return tmp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataSize">Number of actual bits divided by 2.</param>
        /// <param name="rawData"></param>
        public EquationManipulator(int DataSize, string rawData)
        {
            this.DataSize = DataSize;
            if (rawData == null || rawData == string.Empty)
            {
                rawData = "000073af 00000000 000073af 00000000 000073af 00000000 000073af 00000000 000073af 00000000 000073af 00000000 000073af 00000000 000073af 00000000";
                //rawData = "73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af";
            }

            {
                int maxIndex = (DataSize / 16 > 0) ? DataSize / 16 : 1;
                rawData = rawData.Replace(" ", "").PadLeft(8, '0');
                this.encryptedData = new uint[maxIndex];
                for (int i = 0; i < maxIndex; i++)
                {
                    encryptedData[i] = uint.Parse(rawData.Substring(i * 8, 8), System.Globalization.NumberStyles.HexNumber);
                }
            }
            encEqs = new LogicANFEquation[DataSize * 2];
        }

        public void Initialize()
        {
            // this next set of 4 repeat loops (nested included) is just a faster way of creating a map of the
            // operations done to each bit to end up with the encripted bit located at the same index as the index
            // of xorEq[index].  Faster compared to mapping and getting rid of duplicate procedures of the algorithm
            // that was provided to us by Nintendo/Alpha Centari.
            // The variable IDs are equal to the unencrypted bit index + 1.  This makes it easy to indicate inverse states by
            // just making the ID negative.
            for (int ebitIndex = 0; ebitIndex < DataSize; ebitIndex++)
            {
                var tmpEq = new LogicANFEquation();
                encEqs[ebitIndex] = tmpEq;
                for (int termIndex = 0; termIndex <= ebitIndex; termIndex++)
                {
                    int[] tmp = new int[2] { termIndex + 1, DataSize + ebitIndex - termIndex + 1 };
                    tmpEq.AddTerm(new LogicANDTerm(tmp));
                }
            }
            for (int ebitIndex = DataSize * 2 - 2; ebitIndex >= DataSize; ebitIndex--)
            {
                var tmpEq = new LogicANFEquation();
                encEqs[ebitIndex] = tmpEq;
                for (int termIndex = 0; termIndex <= DataSize * 2 - ebitIndex - 2; termIndex++)
                {
                    int[] tmp = new int[2] { ebitIndex - DataSize + 2 + termIndex, DataSize * 2 - termIndex };
                    tmpEq.AddTerm(new LogicANDTerm(tmp));
                }
            }

            // Build additional masks based on repeated false values from the first bit or the last bit
            // These masks will be used as additional equations to further cut down the number of options we have to test.
            {
                int lastFalseIdx = DataSize * 2 - 2;
                while (lastFalseIdx >= DataSize && !encryptedData.GetBitState(lastFalseIdx))
                {
                    lastFalseIdx--;
                }
                lastFalseIdx++;

                int numOfTerms = DataSize * 2 - lastFalseIdx;
                int numOfVarPerTerm = numOfTerms - 1;
                int firstStart = DataSize * (-1);
                int secondStart = DataSize * (-2);

                int[] tmpVars = new int[numOfVarPerTerm];

                plainOrEq = new SortedDictionary<LogicANDTerm, object>();
                if (lastFalseIdx < DataSize * 2 - 2)
                {
                    for (int splitPoint = 0; splitPoint < numOfTerms; splitPoint++)
                    {
                        int varIdx = 0;
                        for (; varIdx < splitPoint; varIdx++)
                        {
                            tmpVars[varIdx] = secondStart + varIdx;
                        }
                        for (; varIdx < numOfVarPerTerm; varIdx++)
                        {
                            tmpVars[varIdx] = firstStart - splitPoint + varIdx;
                        }
                        plainOrEq.Add(new LogicANDTerm(tmpVars), null);
                    }
                }

            }
        }

        public void SubIntoPlainOr(LogicANFEquation subEq)
        {
            int subVarID = subEq.SubVarID;
            if (!subEq.IsFullyReduced || subVarID == 0)
            {
                throw new ArgumentException("subEq is not prepared to be substituted in.", "subEq");
            }
            if (!subEq.IsFullyReduced)
            {
                throw new ArgumentException("The equation you are trying to substitute into plainOrEq must be fully reduced.", "subEq");
            }
            bool subVarState = subEq.GetReducedState();
            LogicANDTerm subAnd;
            subVarID *= -1;
            if (subVarState)  // we have to reverse the state because we will be inversing the state of subVarID;
            {
                subAnd = LogicANDTerm.ConstantZero;
            }
            else
            {
                subAnd = LogicANDTerm.ConstantOne;
            }

            var array = plainOrEq.ToArray();
            plainOrEq.Clear();
            for (int i = 0; i < array.Length; i++)
            {
                var add = array[i].Key.SubstituteIn(subAnd, subVarID);
                if (add.IsConstantOne)
                {
                    throw new Exception("Danger Danger!  Logic error by programmer!");
                }
                if (!plainOrEq.Remove(add))
                {
                    plainOrEq.Add(add, null);
                }
            }
        }

        public string Solve2()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < DataSize - 1; i++)
            {
                if (!encryptedData.GetBitState(i))
                {
                    encEqs[i].AddTerm(LogicANDTerm.ConstantOne);
                }
                if (!encryptedData.GetBitState(DataSize + i))
                {
                    encEqs[DataSize + i].AddTerm(LogicANDTerm.ConstantOne);
                }
                encEqs[i].ANDIntoThis(encEqs[DataSize + i]);
                encEqs[i].ToStringBuilder(sb);
                sb.AppendLine();
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private struct VarIDPair
        {
            public int first, last;

            public VarIDPair(int first, int last)
            {
                this.first = first;
                this.last = last;
            }
        }

        uint[][][] masks;
        VarIDPair[] newVars;
        public string TopDownRecursionTest()
        {
            // create masks
            masks = new uint[DataSize * 2 - 1][][];
            newVars = new VarIDPair[DataSize * 2 - 1];


            for (int ebitIndex = 0; ebitIndex < DataSize; ebitIndex++)
            {
                masks[ebitIndex] = new uint[ebitIndex + 1][];
                newVars[ebitIndex] = new VarIDPair(ebitIndex, ebitIndex + DataSize);
                for (int maskIndex = 0; maskIndex <= ebitIndex; maskIndex++)
                {
                    masks[ebitIndex][maskIndex] = new uint[encryptedData.Length];
                    masks[ebitIndex][maskIndex].SetBitStateTrue(maskIndex);
                    masks[ebitIndex][maskIndex].SetBitStateTrue(DataSize + ebitIndex - maskIndex);
                }
            }
            for (int ebitIndex = DataSize * 2 - 2; ebitIndex >= DataSize; ebitIndex--)
            {
                newVars[ebitIndex] = new VarIDPair(ebitIndex - DataSize + 1, ebitIndex + 1);
                masks[ebitIndex] = new uint[DataSize * 2 - ebitIndex - 1][];
                for (int maskIndex = 0; maskIndex <= DataSize * 2 - ebitIndex - 2; maskIndex++)
                {
                    masks[ebitIndex][maskIndex] = new uint[encryptedData.Length];
                    masks[ebitIndex][maskIndex].SetBitStateTrue(ebitIndex - DataSize + 1 + maskIndex);
                    masks[ebitIndex][maskIndex].SetBitStateTrue(DataSize * 2 - maskIndex - 1);
                }
            }

            List<uint[]> solutions = new List<uint[]>();
            ulong solCount = TopDownRecursion(solutions, 0, new uint[encryptedData.Length], 0);

            return solCount.ToString();
        }

        private ulong TopDownRecursion(List<uint[]> solutions, int bitIndex, uint[] solPart, ulong solCount)
        {
            bool encState = encryptedData.GetBitState(bitIndex);

            if (bitIndex == DataSize / 2)
            {
                // partial solution found
                //uint[] t = new uint[encryptedData.Length];
                //encryptedData.CopyTo(t, 0);
                //solutions.Add(t);
                return solCount + 1;
            }

            // case 1: newVar bits set to false & false
            if (TestEquation(bitIndex, solPart, encState))
            {
                solCount = TopDownRecursion(solutions, bitIndex + 1, solPart, solCount);
            }

            // case 2: newVar bits set to false & true
            solPart.SetBitStateTrue(newVars[bitIndex].first);
            if (TestEquation(bitIndex, solPart, encState))
            {
                solCount = TopDownRecursion(solutions, bitIndex + 1, solPart, solCount);
            }

            // case 3: newVar bits set to true & true
            solPart.SetBitStateTrue(newVars[bitIndex].last);
            if (TestEquation(bitIndex, solPart, encState))
            {
                solCount = TopDownRecursion(solutions, bitIndex + 1, solPart, solCount);
            }

            // case 4: newVar bits set to true & false
            solPart.SetBitStateFalse(newVars[bitIndex].first);
            if (TestEquation(bitIndex, solPart, encState))
            {
                solCount = TopDownRecursion(solutions, bitIndex + 1, solPart, solCount);
            }

            // unset the last bit and return to continue on to other possible solutions.
            solPart.SetBitStateFalse(newVars[bitIndex].last);
            return solCount;
        }

        private bool TestEquation(int bitIndex, uint[] solPart, bool encState)
        {
            bool currentState = false;
            for (int i = 0; i < masks[bitIndex].Length; i++)
            {
                bool innerState = true;
                for (int j = 0; j < encryptedData.Length; j++)
                {
                    if ((masks[bitIndex][i][j] & solPart[j]) != masks[bitIndex][i][j])
                    {
                        innerState = false;
                        break;
                    }
                }
                if (innerState)
                {
                    currentState = !currentState;
                }
            }
            return (encState == currentState);
        }

        public string Solve()
        {
            LogicANFEquation[] subEqs = new LogicANFEquation[DataSize * 2];
            if (!encryptedData.GetBitState(0))
            {
                throw new Exception("I'm screwed.");
            }
            else
            {
                var isolated = encEqs[0].GetSingleTermVarIDs();
                int first = isolated.First();
                int last = isolated.Last();
                LogicANFEquation newEq1 = new LogicANFEquation();
                LogicANFEquation newEq2 = new LogicANFEquation();
                newEq1.AddTerm(new LogicANDTerm(new int[] { first }));
                newEq2.AddTerm(new LogicANDTerm(new int[] { last }));
                newEq1 = subEqs[first.VarIDToIndex()] = newEq1.CreateSubEq(first, true);
                newEq2 = subEqs[last.VarIDToIndex()] = newEq2.CreateSubEq(last, true);

                for (int i = 0; i < DataSize * 2 - 1; i++)
                {
                    encEqs[i].SubInEq(newEq1);
                    encEqs[i].SubInEq(newEq2);
                }

                for (int i = 1; i < DataSize; i++)
                {
                    LogicANFEquation newSub = encEqs[i].CreateSubEq(encryptedData.GetBitState(i));
                    for (int j = 0; j < DataSize; j++)
                    {
                        if (j > i)
                        {
                            encEqs[j].SubInEq(newSub);
                        }
                        if (subEqs[j] != null)
                        {
                            subEqs[j].SubInEq(newSub);
                        }
                    }
                    for (int j = DataSize; j < DataSize * 2; j++)
                    {
                        if (subEqs[j] != null)
                        {
                            subEqs[j].SubInEq(newSub);
                        }
                    }
                    subEqs[newSub.SubVarID.VarIDToIndex()] = newSub;
                }
            }

            for (int i = 0; i < DataSize * 2; i++)
            {
                if (subEqs[i] != null && subEqs[i].IsFullyReduced)
                {
                    SubIntoPlainOr(subEqs[i]);
                    for (int j = 0; j < DataSize * 2 - 1; j++)
                    {
                        if (!encEqs[j].IsFullyReduced)
                        encEqs[j].SubInEq(subEqs[i]);
                    }
                }
            }

            //encEqs[62].SubInEq(subEqs[63]);
            //encEqs[62].CreateSubEq(false);


            {
                //int i = DataSize;
                //while (subEqs[i] == null || subEqs[i].IsFullyReduced)
                //{
                //    i++;
                //}
                //while (i < DataSize * 2 && subEqs[i] != null && !subEqs[i].IsFullyReduced)
                //{
                //    for (int j = 0; j < DataSize * 2 - 1; j++)
                //    {
                //        encEqs[j].SubInEq(subEqs[i]);
                //        //LogicANFEquation test = null;
                //        //try
                //        //{
                //        //    test = encEqs[j].CreateSubEq(encryptedData.GetBitState(j));
                //        //}
                //        //catch { }
                        
                //        //if (j == 60)
                //        //{
                //        //    //throw new Exception("tell me");
                //        //}
                //    }
                //    i++;
                //}
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SubInEq's by index:");
            for (int i = 0; i < DataSize * 2; i++)
            {
                sb.Append(i);
                sb.Append(":  ");
                if (subEqs[i] == null)
                {
                    sb.Append("No Equation");
                }
                else
                {
                    subEqs[i].ToStringBuilder(sb);
                }
                sb.AppendLine();
            }
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("EncEq's by index:");
            for (int i = 0; i < DataSize * 2; i++)
            {
                sb.Append(i);
                sb.Append(":  ");
                if (encEqs[i] == null)
                {
                    sb.Append("No Equation");
                }
                else
                {
                    encEqs[i].ToStringBuilder(sb);
                }
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("plainOrEq:");
            foreach (var pair in plainOrEq)
            {
                pair.Key.ToStringBuilder(sb);
                sb.AppendLine(" | ");
            }

            return sb.ToString();
        }

    }
}
