using LimitedBudgetMonojam.GameObjects.PhysicsObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.GameObjects
{
    class Ball : GameObject
    {
        public Ball(Position pos, Texture2D tex) : base(pos, tex)
        {
            PhysicsCircle circ = new PhysicsCircle(this.Rectangle.Center, tex.Width / 2);
            Colliders.Add(circ);
            Velocity = new Position(0, 0);
            Acceleration = new Position(0 , 4);
            Movable = true;
        }


    }
}
