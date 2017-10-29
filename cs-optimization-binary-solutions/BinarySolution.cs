using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BinaryOptimization
{
    public class BinarySolution : BaseSolution<int>
    {
        public BinarySolution()
        {

        }

        public BinarySolution(int[] s, double fitness)
            : base(s, fitness)
        {
        }

        public override BaseSolution<int> Clone()
        {
            BinarySolution clone = new BinarySolution(mValues, mCost);
            return clone;
        }

        public void FlipBit(int i)
        {
            mValues[i] = 1 - mValues[i];
        }

        public override bool Equals(object obj)
        {
            if (obj is BaseSolution<int>)
            {
                BaseSolution<int> cast_obj = obj as BaseSolution<int>;
                int length1 = this.Length;
                int length2 = cast_obj.Length;
                if (length1 == length2)
                {
                    for (int i = 0; i < length1; ++i)
                    {
                        if (this[i] != cast_obj[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
