using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.GameObjects.PhysicsObjects
{
    public class PhysicsCircle : PhysicsShape
    {
        public double Radius;

        public PhysicsCircle(Position pos, double radius)
        {
            Position = pos;
            Radius = radius;
        }

        public override Position Center 
        {
            get
            {
                return Position;
            }
        }

        public override double GreatestDistanceFromCenter 
        {
            get
            {
                return Radius;
            }
        }
    }
}
