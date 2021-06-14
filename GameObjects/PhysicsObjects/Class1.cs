using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.GameObjects.PhysicsObjects
{
    public static class MathsExt
    {

        public static double Min(double d1, double d2)
        {
            if (d1 <= d2)
                return d1;
            return d2;
        }
        public static double Max(double d1, double d2)
        {
            if (d1 >= d2)
                return d1;
            return d2;
        }
    }
}
