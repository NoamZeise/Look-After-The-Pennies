using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace LimitedBudgetMonojam.UI
{
    public class Menu
    {
        public bool visible = false;
        public List<Button> Buttons;

        public Vector2 Position;

        public Menu(Vector2 pos)
        {
            Buttons = new List<Button>();
            Position = pos;
        }
        public void addButton(Button button)
        {
            button.Rect.X += (int)Position.X;
            button.Rect.Y += (int)Position.Y;
            Buttons.Add(button);
        }
        public void Update(GameTime gameTime, Camera2D.Camera camera)
        {
            if(visible)
                foreach (var button in Buttons)
                    button.Update(gameTime, camera);
        }
        public void Draw(SpriteBatch spriteBatch, UITextures tex)
        {
            if(visible)
                foreach (var button in Buttons)
                    button.Draw(spriteBatch, tex);
        }
    }
}
