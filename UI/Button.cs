using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.UI
{
    public class Button
    {
        public EventHandler<UIEventArgs> WhenPressed;
        MouseState prev;
        public Rectangle Rect;
        string Text;
        Color colour = Color.White;

        public Button(Rectangle rect, string text)
        {
            Rect = rect;
            Text = text;
        }

        public void Update(GameTime gameTime, Camera2D.Camera camera)
        {
            //mouse is inside button rect and is clicking

            if (Rect.Contains(camera.independentMousePoint))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && prev.LeftButton == ButtonState.Released)
                {
                    EventHandler<UIEventArgs> pressed = WhenPressed;

                    if (pressed != null)
                    {
                        pressed(this, new UIEventArgs());
                    }
                }
                colour = Color.Gray;
            }
            else
            {
                colour = Color.White;
            }



            prev = Mouse.GetState();
        }

        public void Draw(SpriteBatch spriteBatch, UITextures tex)
        {

            spriteBatch.Draw(tex.button, Rect,  null, colour, 0f, Vector2.Zero, SpriteEffects.None, 0.85f);
            Vector2 textPos = Rect.Center.ToVector2();
            textPos.X -= (int)(tex.buttonFont.MeasureString(Text).X / 2);
            textPos.Y -= (int)(tex.buttonFont.MeasureString(Text).Y / 2);
            spriteBatch.DrawString(tex.buttonFont, Text, textPos, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9f);
        }

    }

}
