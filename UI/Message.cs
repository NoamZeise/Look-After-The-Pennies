using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.UI
{
    public class Message
    {
        MouseState prev;
        string Text;
        Color colour = Color.White;
        public bool remove = false;
        double xOffset = 0;
        double yOffset = 0;
        public Message(string text)
        {
            Text = text;
        }
        public Message(string text, double xOff, double yOff)
        {
            Text = text;
            xOffset = xOff;
            yOffset = yOff;
        }

        public void Update(GameTime gameTime, Camera2D.Camera camera)
        {
            //mouse is inside button rect and is clicking


                if (Mouse.GetState().LeftButton == ButtonState.Pressed && prev.LeftButton == ButtonState.Released)
                {
                remove = true;
                }

            prev = Mouse.GetState();
        }

        public void Draw(SpriteBatch spriteBatch, UITextures tex)
        {
            var strSize = tex.buttonFont.MeasureString(Text);

            var Rect = new Rectangle(0, 0, (int)strSize.X + 30, (int)strSize.Y + 10);
            Rect.X = ((Game1.SCREEN_WIDTH / 2) - Rect.Width / 2) + (int)xOffset;
            Rect.Y = ((Game1.SCREEN_HEIGHT / 2) - Rect.Height / 2) + (int)yOffset;
            Vector2 textPos = Rect.Center.ToVector2();
            textPos.X -= (int)(strSize.X / 2);
            textPos.Y -= (int)(strSize.Y / 2);
            spriteBatch.Draw(tex.messageBox, Rect, null, colour, 0f, Vector2.Zero, SpriteEffects.None, 0.85f);
            spriteBatch.DrawString(tex.buttonFont, Text, textPos, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
        }
    }
}
