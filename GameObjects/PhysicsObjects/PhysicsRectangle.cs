using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.GameObjects.PhysicsObjects
{
    public class PhysicsRectangle : PhysicsShape
    {
        public double Width;
        public double Height;
        double greatestDist;
        public PhysicsRectangle(Position pos, double width, double height)
        {
            Position = pos;
            Width = width;
            Height = height;

            greatestDist = Physics.Distance(Center, Position);
        }
        public PhysicsRectangle(double x, double y, double width, double height)
        {
            Position = new Position(x, y);
            Width = width;
            Height = height;

            greatestDist = Physics.Distance(Center, Position);
        }

        public override Position Center
        {
            get
            {
                return new Position(Position.X + (Width / 2), Position.Y + (Height / 2));
            }
        }

        public override double GreatestDistanceFromCenter
        {
            get
            {
                return greatestDist;
            }
        }

        public static PhysicsRectangle rectangleBetweenPoints(Position pos1, Position pos2)
        {
            double maxX, maxY;
            maxX = MathsExt.Min(pos1.X, pos2.X);
            maxY = MathsExt.Min(pos1.Y, pos2.Y);
            //rectangle of mouse area
            return new PhysicsRectangle(maxX, maxY, Math.Abs(pos1.X - pos2.X), Math.Abs(pos1.Y - pos2.Y));
        }


    }
}
