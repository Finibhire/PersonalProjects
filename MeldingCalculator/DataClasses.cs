using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MeldingCalculator.DataClasses
{
    public class Materia
    {
        private MateriaRank rank;
        public MateriaRank Rank
        {
            get
            {
                return rank;
            }
        }
        private MateriaType type;
        public MateriaType Type
        {
            get
            {
                return type;
            }
        }

        public Materia(MateriaRank Rank, MateriaType Type)
        {
            rank = Rank;
            type = Type;
        }

        public int Value
        {
            get
            {
                return (int)Rank + Type.GetBump();
            }
        }
    }

    public enum MateriaRank
    {
        I = 1, II, III, IV
    }

    public enum MateriaType
    {
        Craftsmanship = 1, Control, CP
    }
    public static class MateriaTypeExtentions
    {
        public static int GetBump(this MateriaType type)
        {
            if (type == MateriaType.Craftsmanship)
            {
                return 2;
            }
            return 0;
        }
    }

    //public class MateriaList : LinkedList<Materia>
    //{
    //    public double 
    //}
}
