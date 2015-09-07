using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace BitShiftCalculator
{
    class BitLogicRecursiveTester
    {
        private XORCollector32Bit[] bitXORCollector;
        /// <summary>
        /// The order in which to test bit encription equations to see if they cause a conflict.
        /// </summary>
        private List<SortedList<int, int>> bitTestOrder;

        /// <summary>
        /// The order in which to define an unencripted test values to bits to test if they cause a conflict.
        /// </summary>
        private List<SortedList<int, int>> bitFillOrder;
        private bool[] encryptedBitStates;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsBitSet(uint data, int pos)
        {
            return (data >> pos & 1u) == 1u;
        }

        public BitLogicRecursiveTester(XORCollector32Bit[] logicData, uint[] encryptedData)
        {
            if (logicData.Length != encryptedData.Length)
                throw new ArgumentException("Unexpected Logic Error BitLogicTest constructor; input arrays not the same length.");

            bitXORCollector = logicData;
            bitTestOrder = new List<SortedList<int, int>>();
            bitFillOrder = new List<SortedList<int, int>>();

            encryptedBitStates = new bool[encryptedData.Length * 32];
            for (int i = 0; i < encryptedData.Length; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    encryptedBitStates[i * 32 + j] = IsBitSet(encryptedData[i], j);
                }
            }

            // uniqueIDs<AND/XOR bitID, Logic Equation bitID>[Logic Equation bitID]
            SortedDictionary<int, int>[] uniqueIDs = new SortedDictionary<int, int>[encryptedData.Length * 32];
            for (int i = 0; i < encryptedData.Length; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    int bitPos = i * 32 + j;

                    uniqueIDs[bitPos] = new SortedDictionary<int, int>();
                    foreach (KeyValuePair<ANDCombination, object> pair in logicData[i].xorOperations[j])
                    {
                        uniqueIDs[bitPos][pair.Key.FirstVarID] = bitPos;
                        uniqueIDs[bitPos][pair.Key.SecondVarID] = bitPos;
                    }
                }
            }

            int testRank = 0;
            SortedDictionary<int, int> definedBits = null;
            SortedDictionary<int, int> nextDefinedBits = new SortedDictionary<int, int>();
            while (nextDefinedBits != null)
            {
                bitTestOrder.Add(new SortedList<int, int>());
                bitFillOrder.Add(new SortedList<int, int>());
                int lowCount = int.MaxValue;
                definedBits = nextDefinedBits;
                nextDefinedBits = null;
                for (int j = 0; j < uniqueIDs.Length; j++)
                {
                    if (uniqueIDs[j].Count > 0)
                    {
                        if (definedBits != uniqueIDs[j])
                        {
                            foreach (KeyValuePair<int, int> b in definedBits)
                            {
                                uniqueIDs[j].Remove(b.Key);
                            }
                        }

                        if (uniqueIDs[j].Count == 0 || definedBits == uniqueIDs[j])
                        {
                            bitTestOrder[testRank].Add(j, j);
                        }
                        else if (uniqueIDs[j].Count < lowCount)
                        {
                            lowCount = uniqueIDs[j].Count;
                            nextDefinedBits = uniqueIDs[j];
                        }
                    }
                }
                foreach (KeyValuePair<int, int> b in definedBits)
                {
                    bitFillOrder[testRank].Add(b.Key, b.Key);
                }
                definedBits.Clear();
                testRank++;
            }

            bitTestOrder.RemoveAt(0);
            bitFillOrder.RemoveAt(0);
        }

        /// <summary>
        /// Used to verify that result in the bitXORCollecter32 is the same as what the encription algorithum would produe.
        /// Used for debugging.
        /// </summary>
        /// <returns></returns>
        public uint[] EncriptUsingXORBitTracking()
        {
            uint[] rtn = new uint[bitXORCollector.Length];

            for (int i = 0; i < bitXORCollector.Length; i++)
            {
                rtn[i] = 0;
                bool eqResult = false;
                for (int h = 0; h < 32; h++)
                {
                    SortedList<ANDCombination, object> bitEq = bitXORCollector[i].xorOperations[h];

                    eqResult = false;
                    bool firstAndSet = true;
                    for (int j = 0; j < bitEq.Count; j++)
                    {
                        bool andResult = true;
                        if (!encryptedBitStates[bitEq.ElementAt(j).Key.FirstVarID] || !encryptedBitStates[bitEq.ElementAt(j).Key.SecondVarID])
                        {
                            break;
                        }

                        if (firstAndSet)
                        {
                            eqResult = andResult;
                            firstAndSet = false;
                        }
                        else if (andResult)
                        {
                            eqResult = !eqResult;
                        }
                    }
                    if (eqResult)
                    {
                        rtn[i] = (rtn[i] | (1u << h));
                    }
                }
            }
            return rtn;
        }

        private bool EquationTest(SortedDictionary<int, bool> definedData, int definingRank)
        {
            foreach (KeyValuePair<int, int> pair in bitTestOrder[definingRank])
            {
                SortedList<ANDCombination, object> bitEq = bitXORCollector[pair.Key / 32].xorOperations[pair.Key % 32];

                bool eqResult = false;
                for (int j = 0; j < bitEq.Count; j++)
                {
                    bool firstDefinedBit = definedData[bitEq.ElementAt(j).Key.FirstVarID];
                    bool lastDefinedBit = definedData[bitEq.ElementAt(j).Key.SecondVarID];
                    if (firstDefinedBit == true && lastDefinedBit == true)
                    {
                        eqResult = !eqResult;
                    }
                }
                if (eqResult != encryptedBitStates[pair.Key])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Uses a recursive algorithm to test the logic equations for each bit that have been created
        /// by the BitXORCollector32.  Unfortunately because of the way the encryption algorithm works not enough
        /// possibilities are eliminated to make this testing viable.
        /// 
        /// The full domain of possibilities for the smallest "32 bit" problem that is for some reason 64 bits long
        /// is 2^64 =  ~18.4 Quintillion (x10^18) .  Though the recursive algorithm greatly reduces the number of I estimate that 
        /// I have to test at least 2^32 possibilites of that domain which is about 4.3 Billion possiblities.  I suspect if
        /// I were to find out how many I needed it would on the order of 2^5 though 2^10 larger.  This isn't going to happen
        /// because after 1 hour of running I only completed about 20 Million cases.  Thus after 9 days I would be done
        /// maybe 2^32 possiblilities.
        /// 
        /// This method just isn't viable despite possible optimizations if the program needs to run in under one second.
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<int, bool>[]> TestCombinations()
        {
            List<KeyValuePair<int, bool>[]> results = new List<KeyValuePair<int, bool>[]>();
            SortedDictionary<int, bool> dd = new SortedDictionary<int, bool>();
            TestCombination(dd, 0, results);
            return results;
        }
        
#if DEBUG
        private SortedDictionary<uint[], object> finalDataSets = new SortedDictionary<uint[], object>(Comparer<uint[]>.Create(new Comparison<uint[]>(CompareTests))); 
        
        /// <summary>
        /// less than 0    means x is less than y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int CompareTests(uint[] x, uint[] y)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < y[i])
                {
                    return -1;
                }
                else if (x[i] > y[i])
                {
                    return 1;
                }
            }
            if (x.Length == 0)
            {
                return -1;
            }
            return 0;
        }
#endif
        private void TestCombination(SortedDictionary<int, bool> definedData, int definingRank, List<KeyValuePair<int, bool>[]> validSet)
        {
            int defCount = bitFillOrder[definingRank].Count;
            bool[,] options = new bool[defCount * defCount, defCount];

            for (uint i = 0; i < defCount * defCount; i++)
            {
                for (int j = 0; j < defCount; j++)
                {
                    options[i, j] = IsBitSet(i, j);
                }
            }

            for (uint i = 0; i < defCount * defCount; i++)
            {
                for (int j = 0; j < defCount; j++)
                {
                    definedData[bitFillOrder[definingRank].ElementAt(j).Key] = options[i, j];
                }

                bool pass = EquationTest(definedData, definingRank);

#if DEBUG
                if (definingRank == bitTestOrder.Count - 1)
                {
                    //StringBuilder sb = new StringBuilder();
                    uint[] testData = new uint[definedData.Count / 32];
                    for (int t = 0; t < definedData.Count; t++)
                    {
                        if (definedData[t])
                        {
                            testData[t / 32] |= 1u << (t % 32);
                        }
                    }
                    //if (CompareTests(previousFinalTest, testData) >= 0)
                    //{
                    //    throw new Exception("duplicate found");
                    //}
                    finalDataSets.Add(testData, null);
                }
#endif
                if (pass)
                {
                    if (definingRank == bitTestOrder.Count - 12) // all the data is defined at this point.
                    {                                           // if it works now we have a working final combination.
                        KeyValuePair<int, bool>[] r = new KeyValuePair<int, bool>[definedData.Count];
                        int k = 0;
                        foreach (KeyValuePair<int, bool> pair in definedData)
                        {
                            r[k] = pair;
                            k++;
                        }
                        validSet.Add(r);
                    }
                    else // pass it forward to define more states for bits.
                    {
                        TestCombination(definedData, definingRank + 1, validSet);
                    }
                }  // if not a pass then we just want to test the next combination of possible bits.
            }

            // we have tested all possible combinations of this set of bits at this point
            // we need to clear them as defined and return from this method to continue on in the previous definingRank
            for (int i = 0; i < bitFillOrder[definingRank].Count; i++)
            {
                definedData.Remove(bitFillOrder[definingRank].ElementAt(i).Key);
            }


            return;
        }
    }
}
