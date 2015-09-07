using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace BitShiftCalculator
{
    class BitHolder32
    {
        public int[][] AndOperations { get; private set; }
        //private int startBitID;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startBitID"></param>
        public BitHolder32(int startBitID)
        {
            if (startBitID < 0 || startBitID > int.MaxValue - 32)
                throw new ArgumentException("Invalid startBitId");
            AndOperations = new int[32][];
            for (int i = 0; i < 32; i++)
            {
                AndOperations[i] = new int[2];
                AndOperations[i][0] = startBitID + i;
            }
        }
        private BitHolder32(BitHolder32 other)
        {
            AndOperations = new int[32][];
            for (int i = 0; i < 32; i++)
            {
                AndOperations[i] = new int[2];
                other.AndOperations[i].CopyTo(AndOperations[i], 0);
            }
        }

        public BitHolder32 DeepCopy()
        {
            return new BitHolder32(this);
        }

        public void RightShift(int shift)
        {
            if (shift > 0)
            {
                for (int i = 0; i < 32 - shift; i++)
                {
                    AndOperations[i] = AndOperations[i + shift];
                }
                for (int i = 32 - shift; i < 32; i++)
                {
                    AndOperations[i] = new int[2] { -1, -1 }; // -1 represents an actual value of 0
                }


                //int[] tmp = AndOperations[0];
                //for (int i = 1; i < 32; i++)
                //{
                //    int sourceIndex = i * shift % 32;
                //    int destIndex = (i - 1) * shift % 32;
                //    AndOperations[destIndex] = AndOperations[sourceIndex];
                //}
                //AndOperations[32 - shift] = tmp;

                //for (int i = 32 - shift; i < 32; i++)
                //{
                //    AndOperations[i][0] = -1;
                //    AndOperations[i][1] = -1;  // -1 represents an actual value of 0
                //}
            }
            else if (shift < 0)
            {
                LeftShift(shift * (-1));
            }
        }

        public void LeftShift(int shift)
        {
            if (shift > 0)
            {
                for (int i = 31; i >= shift; i--)
                {
                    AndOperations[i] = AndOperations[i - shift];
                }
                for (int i = shift - 1; i >= 0; i--)
                {
                    AndOperations[i] = new int[2] { -1, -1 }; // -1 represents an actual value of 0
                }



                //int[] tmp = AndOperations[32 - shift];
                //for (int i = 31; i > 0; i--)
                //{
                //    int sourceIndex = (i - 1) * shift % 32;
                //    int destIndex = i * shift % 32;
                //    AndOperations[destIndex] = AndOperations[sourceIndex];
                //}
                //AndOperations[0] = tmp;

                //for (int i = 0; i < shift; i++)
                //{
                //    AndOperations[i][0] = -1;
                //    AndOperations[i][1] = -1;  // -1 represents an actual value of 0
                //}
            }
            else if (shift < 0)
            {
                RightShift(shift * (-1));
            }
        }

        public void ANDOne()
        {
            for (int i = 1; i < 32; i++)
            {
                AndOperations[i][0] = -1;
                AndOperations[i][1] = -1;
            }
        }

        public void AND(BitHolder32 other)
        {
            for (int i = 0; i < 32; i++)
            {
                int a = AndOperations[i][0];
                int b = other.AndOperations[i][0];
                if (a != -1) // -1 indicates an actual value of 0  ( ANDing 0 always results in 0 )
                {            // do nothing in that case because we already have a 0 value in AndOperations[i][0 && 1]
                    if (b == -1)  // if the other value we are ANDing against is 0 then set this AndOperations[i][0 && 1] to 0
                    {
                        AndOperations[i][0] = -1;
                        AndOperations[i][1] = -1;  // -1 represents an actual value of 0
                    }
                    else if (a == -2)  // -2 indicates and actual value of 1
                    {
                        AndOperations[i] = other.AndOperations[i];
                        throw new Exception("Unexpected Case");   // this case shouldn't ever happen
                    }
                    else if (b == -2) // a & 1 == a;
                    {
                        AndOperations[i][1] = AndOperations[i][1];  // same as setting it to -2 but bitID ordering is perserved this way.
                        throw new Exception("Unexpected Case");   // this case shouldn't ever happen
                    }
                    else
                    {
                        AndOperations[i][1] = b;
                    }
                }
            }
        }
    }
}
