using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace LimitedBudgetMonojam.UI
{
    public struct UITextures
    {
        public Texture2D button;
        public Texture2D messageBox; 
        public SpriteFont buttonFont;
    }

    public class UIManager
    {
        UITextures tex;
        public struct Menus
        {
            public Menu Pause;
            public Menu Main;
        }
        public bool shouldClose = false;
        public bool mainClosed = false;
        public Menus menus;

        public List<Message> messages = new List<Message>();
        public UIManager(ContentManager content)
        {
            tex.button = content.Load<Texture2D>("UI/button");
            tex.messageBox = content.Load<Texture2D>("UI/msg");
            tex.buttonFont = content.Load<SpriteFont>("UI/buttonFont");

            menus.Pause = new Menu(new Vector2((Game1.SCREEN_WIDTH / 2 ) - 100 , (Game1.SCREEN_HEIGHT / 2) - 100));
            Button unpaused = new Button(new Rectangle(0, 0, 200, 80), "Unpause");
            unpaused.WhenPressed += pauseButtonFunct;
            menus.Pause.addButton(unpaused);
            Button menu = new Button(new Rectangle(0, 100, 200, 80), "Menu");
            menus.Pause.addButton(menu);
            Button exit = new Button(new Rectangle(0, 200, 200, 80), "Exit");
            exit.WhenPressed += exitButtonFunct;
            menus.Pause.addButton(exit);

            menus.Main = new Menu(new Vector2((Game1.SCREEN_WIDTH / 2) - 100, (Game1.SCREEN_HEIGHT / 2) - 100));
            Button Play = new Button(new Rectangle(0, 0, 200, 80), "Play");
            menus.Main.addButton(Play);
            Button Editor = new Button(new Rectangle(0, 200, 200, 80), "Editor");
            menus.Main.addButton(Editor);
            Button Toy = new Button(new Rectangle(0, 100, 200, 80), "Toy");
            menus.Main.addButton(Toy);
            exit = new Button(new Rectangle(0, 300, 200, 80), "Exit");
            exit.WhenPressed += exitButtonFunct;
            menus.Main.addButton(exit);
            menus.Main.visible = true;
        }

        public void Update(GameTime gameTime, Camera2D.Camera camera)
        {
            if (messages.Count == 0)
            {
                menus.Pause.Update(gameTime, camera);
                menus.Main.Update(gameTime, camera);
            }
            for (int i = 0; i < messages.Count; i++)
            {
                messages[i].Update(gameTime, camera);
                if (messages[i].remove)
                    messages.RemoveAt(i--);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            menus.Pause.Draw(spriteBatch, tex);
            menus.Main.Draw(spriteBatch, tex);
            foreach (var m in messages)
                m.Draw(spriteBatch, tex);
        }
        public void exitButtonFunct(object sender, UIEventArgs e)
        {
            shouldClose = true;
        }
        public bool togglePause()
        {
            menus.Pause.visible = !menus.Pause.visible;
            return menus.Pause.visible;
        }
        public bool getPause()
        {
            return menus.Pause.visible;
        }
        public void pauseButtonFunct(object sender, UIEventArgs e)
        {
            togglePause();
        }

        public void ShowMessage(string txt)
        {
            messages.Add(new Message(txt));
        }
        public void ShowMessage(string txt, double xOff, double yOff)
        {
            messages.Add(new Message(txt, xOff, yOff));
        }
    }
}
