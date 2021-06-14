using LimitedBudgetMonojam.GameObjects.PhysicsObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.GameObjects
{
    static class CollisionManager
    {
        const double loss = 0.7;
        public static bool HandleCollisions(List<GameObject> objs)
        {
            bool collision = false;
            foreach(var obj1 in objs)
            {
                if (!obj1.Movable)
                    continue;

                addChange(obj1, new Position(obj1.FrameChange.X, 0));
                obj1.xColliding = false;
                foreach(var obj2 in objs)
                {
                    if (obj1 == obj2)
                        continue;
                    if(Colliding(obj1, obj2))
                    {
                        obj1.Velocity.X *= -loss;
                        addChange(obj1, new Position(-obj1.FrameChange.X, 0));
                        if (obj2.Movable)
                        {
                            obj2.Velocity.X += obj1.Velocity.X;
                            obj2.Velocity.X *= -loss;
                        }
                        if (Math.Abs(obj1.Velocity.X) > 20)
                            collision = true;
                    }
                }
                addChange(obj1, new Position(0, obj1.FrameChange.Y));
                foreach (var obj2 in objs)
                {
                    if (obj1 == obj2)
                        continue;
                    if (Colliding(obj1, obj2))
                    {
                        obj1.Velocity.Y *= -loss;
                        addChange(obj1, new Position(0, -obj1.FrameChange.Y));
                        if (obj2.Movable)
                        {
                            obj2.Velocity.Y += obj1.Velocity.Y;
                            obj2.Velocity.Y *= -loss;
                        }
                        else
                            obj1.xColliding = true; //on floor
                        if (Math.Abs(obj1.Velocity.Y) > 20)
                            collision = true;
                    }
                }
            }
            return collision;
        }

        static void addChange(GameObject obj, Position change)
        {
            obj.Position += change;
            foreach (var col in obj.Colliders)
                col.Position += change;
        }

        public static bool Colliding(GameObject obj1, GameObject obj2)
        {
            bool collisionFound = false;

            foreach(var s1 in obj1.Colliders)
            {
                foreach(var s2 in obj2.Colliders)
                {
                    if(Physics.Colliding(s1, s2))
                    {
                        collisionFound = true;
                    }
                }
            }


            return collisionFound;
        }
    }
}
