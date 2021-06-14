using LimitedBudgetMonojam.GameObjects.PhysicsObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.GameObjects
{
    public class GameObject
    {
        public float Layer = 0.1f;
        Texture2D texture;
        public Position Position;
        public Position Velocity;
        public Position Inital;
        public Position Acceleration;
        public Position FrameChange;
        public bool Movable = false;
        public bool xColliding = false;
        public double cTimer = 0;
        public bool isRemoved = false;
        public bool justCollided = false;


        public List<PhysicsShape> Colliders;

        public void ChangePosition(Position newPos)
        {
            Position diff = Position - newPos;
            foreach (var c in Colliders)
                c.Position -= diff;
            Position = newPos;
        }


        public PhysicsRectangle Rectangle
        {
            get
            {
                return new PhysicsRectangle(Position, texture.Width, texture.Height);
            }
        }
        public GameObject(Position pos, Texture2D tex)
        {
            Colliders = new List<PhysicsShape>();
            Position = pos;
            Inital = pos;
            texture = tex;
        }
        public virtual void Update(GameTime gameTime)
        {
            if (Movable)
            {
                Velocity += Acceleration;
                FrameChange = Velocity * gameTime.ElapsedGameTime.TotalSeconds;

                if (xColliding)
                {
                    cTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if(cTimer > 0.3)
                    {
                        Velocity.X *= 0.99;
                    }
                }
                else
                    cTimer = 0;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(texture != default)
                DrawRect(spriteBatch, new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height));
        }

        protected void DrawRect(SpriteBatch spriteBatch, Rectangle rect)
        {
            spriteBatch.Draw(texture, rect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Layer);
        }
    }
}
