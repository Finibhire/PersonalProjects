using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicAlgebra
{
    static class BitwiseExtentions
    {
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

        public static void SetBitState(this uint[] data, int index, bool state)
        {
            if (state)
            {
                SetBitStateTrue(data, index);
            }
            else
            {
                SetBitStateFalse(data, index);
            }
        }

        public static string ToHexString(this uint[] x)
        {
            StringBuilder sb = new StringBuilder(9 * x.Length);
            AppendHex(x, sb);
            return sb.ToString();
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
}
