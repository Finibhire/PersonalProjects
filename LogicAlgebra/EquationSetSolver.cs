using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LogicAlgebra.DataStructures;

namespace LogicAlgebra
{
    class EquationSetSolver
    {
        private uint[] encryptedMsg;
        private ANFEquation[] encriptionEqs;
        private readonly int dataSize;
        private readonly int VariableIDLength;

        public int DataSize
        { get { return dataSize; } }

        public EquationSetSolver(string EncryptedHexMsg, int DataSize)
        {
            this.dataSize = DataSize;
            this.VariableIDLength = (int)Math.Ceiling(Math.Log10(DataSize * 2));

            if (EncryptedHexMsg == null || EncryptedHexMsg == string.Empty)
            {
                EncryptedHexMsg = "000073af 00000000 000073af 00000000 000073af 00000000 000073af 00000000 000073af 00000000 000073af 00000000 000073af 00000000 000073af 00000000";
                //rawData = "73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af73af";
            }

            {
                int maxIndex = (DataSize / 16 > 0) ? DataSize / 16 : 1;
                EncryptedHexMsg = EncryptedHexMsg.Replace(" ", "").PadLeft(8, '0');
                this.encryptedMsg = new uint[maxIndex];
                for (int i = 0; i < maxIndex; i++)
                {
                    encryptedMsg[i] = uint.Parse(EncryptedHexMsg.Substring(i * 8, 8), System.Globalization.NumberStyles.HexNumber);
                }
            }
            encriptionEqs = new ANFEquation[DataSize * 2 - 1];

            // this next set of 4 repeat loops (nested included) is just a faster way of creating a map of the
            // operations done to each bit to end up with the encripted bit located at the same index as the index
            // of xorEq[index].  Faster compared to mapping and getting rid of duplicate procedures of the algorithm
            // that was provided to us by Nintendo/Alpha Centari.
            // The variable IDs are equal to the unencrypted bit index.  
            for (int ebitIndex = 0; ebitIndex < DataSize; ebitIndex++)
            {
                var tmpEq = new ANFEquation();
                encriptionEqs[ebitIndex] = tmpEq;
                for (int termIndex = 0; termIndex <= ebitIndex; termIndex++)
                {
                    //int[] tmp = new int[2] { termIndex + 1, DataSize + ebitIndex - termIndex + 1 };
                    Minterm mt = new Minterm();
                    mt.AddAtom(new LogicVariable((termIndex).ToString().PadLeft(VariableIDLength, '0')));
                    mt.AddAtom(new LogicVariable((DataSize + ebitIndex - termIndex).ToString().PadLeft(VariableIDLength, '0')));
                    tmpEq.AddAtom(mt);
                }
                if (!encryptedMsg.GetBitState(ebitIndex))
                {
                    tmpEq.AddAtom(LogicConstantValue.True);
                }
            }
            for (int ebitIndex = DataSize * 2 - 2; ebitIndex >= DataSize; ebitIndex--)
            {
                var tmpEq = new ANFEquation();
                encriptionEqs[ebitIndex] = tmpEq;
                for (int termIndex = 0; termIndex <= DataSize * 2 - ebitIndex - 2; termIndex++)
                {
                    //int[] tmp = new int[2] { ebitIndex - DataSize + 2 + termIndex, DataSize * 2 - termIndex };
                    Minterm mt = new Minterm();
                    mt.AddAtom(new LogicVariable((ebitIndex - DataSize + 1 + termIndex).ToString().PadLeft(VariableIDLength, '0')));
                    mt.AddAtom(new LogicVariable((DataSize * 2 - termIndex - 1).ToString().PadLeft(VariableIDLength, '0')));
                    tmpEq.AddAtom(mt);
                }
                if (!encryptedMsg.GetBitState(ebitIndex))
                {
                    tmpEq.AddAtom(LogicConstantValue.True);
                }
            }
        }

        public void ReduceEncryptionEquations()
        {
            if (encryptedMsg.GetBitState(0) == true)
            {
                string id1 = "".PadLeft(VariableIDLength, '0');
                string id2 = (dataSize).ToString().PadLeft(VariableIDLength, '0');
                for (int i = 0; i < encriptionEqs.Count(); i++)
                {
                    encriptionEqs[i].SubIn(id1, true);
                    encriptionEqs[i].SubIn(id2, true);
                }
            }
            if (encryptedMsg.GetBitState(dataSize * 2 - 2) == true)
            {
                string id1 = (dataSize - 1).ToString().PadLeft(VariableIDLength, '0');
                string id2 = (dataSize * 2 - 1).ToString().PadLeft(VariableIDLength, '0');
                for (int i = 0; i < encriptionEqs.Count(); i++)
                {
                    encriptionEqs[i].SubIn(id1, true);
                    encriptionEqs[i].SubIn(id2, true);
                }
            }

            for (int i = 1; i < dataSize; i++)
            {
                for (int j = i + 1; j < dataSize * 2 - 1; j++)
                {
                    Minterm mt = new Minterm();
                    ANFEquation a = new ANFEquation();
                    mt.AddAtom(encriptionEqs[i].DeepCopy());
                    mt.AddAtom(encriptionEqs[j].DeepCopy());
                    a.AddAtom(mt);
                    a = a.Extract(encriptionEqs[i]);
                    if (a != null)
                    {
                        encriptionEqs[j] = a;
                    }
                }
            }

            for (int i = dataSize * 2 - 2; i >= dataSize; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    Minterm mt = new Minterm();
                    ANFEquation a = new ANFEquation();
                    mt.AddAtom(encriptionEqs[i].DeepCopy());
                    mt.AddAtom(encriptionEqs[j].DeepCopy());
                    a.AddAtom(mt);
                    a = a.Extract(encriptionEqs[i]);
                    if (a != null)
                    {
                        encriptionEqs[j] = a;
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encriptionEqs.Length; i++)
            {
                encriptionEqs[i].AppendToString(sb);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public string FullExpand()
        {

            if (encryptedMsg.GetBitState(0) == true)
            {
                string id1 = "".PadLeft(VariableIDLength, '0');
                string id2 = (dataSize).ToString().PadLeft(VariableIDLength, '0');
                for (int i = 0; i < encriptionEqs.Count(); i++)
                {
                    encriptionEqs[i].SubIn(id1, true);
                    encriptionEqs[i].SubIn(id2, true);
                }
            }
            if (encryptedMsg.GetBitState(dataSize * 2 - 2) == true)
            {
                string id1 = (dataSize - 1).ToString().PadLeft(VariableIDLength, '0');
                string id2 = (dataSize * 2 - 1).ToString().PadLeft(VariableIDLength, '0');
                for (int i = 0; i < encriptionEqs.Count(); i++)
                {
                    encriptionEqs[i].SubIn(id1, true);
                    encriptionEqs[i].SubIn(id2, true);
                }
            }

            ANFEquation final = new ANFEquation();

            final.AddAtom(encriptionEqs[0].DeepCopy());

            for (int i = 1; i < dataSize * 2 - 1; i++)
            {
                Minterm mt = new Minterm();
                mt.AddAtom(final);
                mt.AddAtom(encriptionEqs[i].DeepCopy());
                final = (ANFEquation)mt.Expand();
            }

            return final.ToString();
        }

        //public string TheoryTest2()
        //{
        //    int[][]
        //}
    }
}
