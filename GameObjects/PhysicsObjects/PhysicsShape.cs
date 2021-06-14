using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.GameObjects.PhysicsObjects
{

    public struct Position
    {
        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }
        public static Position operator +(Position p1, Position p2)
        {
            return new Position(p1.X + p2.X, p1.Y + p2.Y);
        }
        public static Position operator -(Position p1, Position p2)
        {
            return new Position(p1.X - p2.X, p1.Y - p2.Y);
        }
        public static Position operator *(double s, Position p)
        {
            return new Position(p.X * s, p.Y * s);
        }
        public static Position operator *(Position p, double s)
        {
            return new Position(p.X * s, p.Y * s);
        }
        public static Position operator /(Position p, double s)
        {
            return new Position(p.X / s, p.Y / s);
        }
        public static Position operator /(double s, Position p)
        {
            return new Position(p.X / s, p.Y / s);
        }
        public override string ToString()
        {
            return "X: " + X + "  Y: " + Y;
        }
        public double X;
        public double Y;
    }

    public abstract class PhysicsShape
    {
        public Position Position;

        public abstract Position Center
        {
            get;
        }

        public abstract double GreatestDistanceFromCenter
        {
            get;
        }
    }
}
