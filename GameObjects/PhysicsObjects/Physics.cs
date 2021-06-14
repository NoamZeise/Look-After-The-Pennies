using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.GameObjects.PhysicsObjects
{
    public static class Physics
    {

        public static bool Colliding(PhysicsRectangle rect1, PhysicsRectangle rect2)
        {
            return rect1.Position.X + rect1.Width >= rect2.Position.X &&
                rect1.Position.X <= rect2.Position.X + rect2.Width &&
                rect1.Position.Y + rect1.Height >= rect2.Position.Y &&
                rect1.Position.Y <= rect2.Position.Y + rect2.Height;
        }

        public static bool Colliding(PhysicsCircle circ1, PhysicsCircle circ2)
        {
            return Distance(circ1.Position, circ2.Position) < circ1.Radius + circ2.Radius;
        }

        public static bool Colliding(PhysicsCircle circ, PhysicsRectangle rect)
        {
            Position closest = circ.Position;

            if (circ.Position.X < rect.Position.X)
                closest.X = rect.Position.X;
            else if (circ.Position.X > rect.Position.X + rect.Width)
                closest.X = rect.Position.X + rect.Width;

            if (circ.Position.Y < rect.Position.Y)
                closest.Y = rect.Position.Y;
            else if (circ.Position.Y > rect.Position.Y + rect.Height)
                closest.Y = rect.Position.Y + rect.Height;

            return Distance(closest, circ.Position) <= circ.Radius;

        }

        public static bool Colliding(PhysicsRectangle rect, PhysicsCircle circ) =>
            Colliding(circ, rect);

        public static bool Colliding(PhysicsShape s1, PhysicsShape s2)
        {
            if(s1 is PhysicsRectangle)
            {
                if(s2 is PhysicsRectangle)
                {
                    return Colliding(s1 as PhysicsRectangle, s2 as PhysicsRectangle);
                }
                if (s2 is PhysicsCircle)
                {
                    return Colliding(s1 as PhysicsRectangle, s2 as PhysicsCircle);
                }
            }
            if (s1 is PhysicsCircle)
            {
                if (s2 is PhysicsRectangle)
                {
                    return Colliding(s1 as PhysicsCircle, s2 as PhysicsRectangle);
                }
                if (s2 is PhysicsCircle)
                {
                    return Colliding(s1 as PhysicsCircle, s2 as PhysicsCircle);
                }
            }
            return false;
        }

        public static double Distance(Position pos1, Position pos2)
        {
            double xDist = pos2.X - pos1.X;
            double yDist = pos2.Y - pos1.Y;
            return Math.Sqrt((xDist * xDist) + (yDist * yDist));
        }

        public static bool Contains(Position pos, PhysicsCircle circ)
        {
            return Distance(pos, circ.Position) < circ.Radius;
        }
        public static bool Contains(PhysicsCircle circ, Position pos)
        {
            return Distance(pos, circ.Position) < circ.Radius;
        }

        public static Position UnitVectorBetween(Position pos1, Position pos2)
        {
            var pos = pos1 - pos2;
            pos /= Math.Sqrt((pos.X * pos.X) + (pos.Y * pos.Y));
            return pos;
        }

    }
}
