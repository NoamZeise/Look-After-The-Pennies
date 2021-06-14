using Camera2D;
using LimitedBudgetMonojam.GameObjects.PhysicsObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LimitedBudgetMonojam.Editor
{
    class LevelEditor
    {
        Position clickMPos;
        bool holding = false;
        Position mPos;

        public struct Level
        {
            public Level(string name)
            {
                ID = name;
                Obstacles = new List<PhysicsRectangle>();
                Death = new List<PhysicsRectangle>();
                Goal = new List<PhysicsRectangle>();
                Spawn = new List<Position>();
            }
            public string ID;
            public List<PhysicsRectangle> Obstacles;
            public List<PhysicsRectangle> Death;
            public List<PhysicsRectangle> Goal;
            public List<Position> Spawn;
        }

        Level level;

        KeyboardState prevState;
        MouseState prevMouse;
        enum Tools
        {
            Rects,
            Spawn,
            Death,
            Goal
        }

        public struct Assets
        {
            public Texture2D Goal;
            public Texture2D Wall;
            public Texture2D Death;
            public Texture2D Spawn;
        }
        public Assets tex;

        public LevelEditor(ContentManager content, string name)
        {
            tex.Goal = content.Load<Texture2D>("Editor/goal");
            tex.Wall = content.Load<Texture2D>("Editor/wall");
            tex.Death = content.Load<Texture2D>("Editor/Death");
            tex.Spawn = content.Load<Texture2D>("Editor/Spawn");

            level = new Level(name);

            if(File.Exists("Levels/" + level.ID))
            {
                level = Load(level);
            }
            else
            {
                int buffer = 4;
                level.Obstacles.Add(new PhysicsRectangle(-10, -10, Game1.SCREEN_WIDTH + 20, 10 + buffer));
                level.Obstacles.Add(new PhysicsRectangle(-10, Game1.SCREEN_HEIGHT - buffer, Game1.SCREEN_WIDTH + 20, 10 + buffer));
                level.Obstacles.Add(new PhysicsRectangle(-10, -10, 10 + buffer, Game1.SCREEN_HEIGHT + 20));
                level.Obstacles.Add(new PhysicsRectangle(Game1.SCREEN_WIDTH - buffer, -10, 10 + buffer, Game1.SCREEN_HEIGHT + 20));
            }
        }
        public LevelEditor(ContentManager content)
        {
            tex.Goal = content.Load<Texture2D>("Editor/goal");
            tex.Wall = content.Load<Texture2D>("Editor/wall");
            tex.Death = content.Load<Texture2D>("Editor/Death");
            tex.Spawn = content.Load<Texture2D>("Editor/Spawn");

            level = new Level(".");
        }
        Tools currentTool = Tools.Rects;
        public void Update(GameTime gameTime, Camera camera)
        {
            mPos = new Position(camera.independentMousePoint.X, camera.independentMousePoint.Y);

            
            if(Keyboard.GetState().IsKeyDown(Keys.R))
            {
                currentTool = Tools.Rects;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                currentTool = Tools.Spawn;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.G))
            {
                currentTool = Tools.Goal;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                currentTool = Tools.Death;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.Z) &&
                (prevState.IsKeyUp(Keys.LeftControl) || prevState.IsKeyUp(Keys.Z)))
            {
                if(currentTool == Tools.Rects)
                {
                    if(level.Obstacles.Count > 0)
                    {
                        level.Obstacles.RemoveAt(level.Obstacles.Count - 1);
                    }
                }
                if (currentTool == Tools.Death)
                {
                    if (level.Death.Count > 0)
                    {
                        level.Death.RemoveAt(level.Death.Count - 1);
                    }
                }
                if (currentTool == Tools.Spawn)
                {
                    if (level.Spawn.Count > 0)
                    {
                        level.Spawn.RemoveAt(level.Spawn.Count - 1);
                    }
                }
                if (currentTool == Tools.Goal)
                {
                    if (level.Goal.Count > 0)
                    {
                        level.Goal.RemoveAt(level.Goal.Count - 1);
                    }
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.S) &&
                (prevState.IsKeyUp(Keys.LeftControl) || prevState.IsKeyUp(Keys.S)))
            {
                if(File.Exists("Levels/" + level.ID))
                {
                    File.Copy("Levels/" + level.ID, "Levels/Temp/" + level.ID, true);
                }
                try
                {
                    save();
                }
                catch
                {
                    if(File.Exists("Levels/Temp/" + level.ID))
                        File.Copy("Levels/Temp/" + level.ID, "Levels/" + level.ID);
                }
            }

                switch (currentTool)
            {
                case Tools.Spawn:
                    if(Mouse.GetState().LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        level.Spawn.Add(mPos);
                    }
                    break;
                case Tools.Rects:
                case Tools.Goal:
                case Tools.Death:
                    PhysicsRectangle r = default;
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                    {
                        clickMPos = mPos;
                        holding = true;
                    }
                    else
                    {
                        if (holding && prevMouse.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
                        {
                            double width = Math.Abs(mPos.X - clickMPos.X);
                            double height = Math.Abs(mPos.Y - clickMPos.Y);

                            if(width*height > 50)
                            {
                                r = PhysicsRectangle.rectangleBetweenPoints(mPos, clickMPos);
                            }
                            holding = false;
                        }
                    }

                    if(r != default)
                    {
                        if(currentTool == Tools.Goal)
                        {
                            level.Goal.Add(r);
                        }
                        if (currentTool == Tools.Rects)
                        {
                            level.Obstacles.Add(r);
                        }
                        if (currentTool == Tools.Death)
                        {
                            level.Death.Add(r);
                        }
                    }
                    break;
            }

            prevState = Keyboard.GetState();
            prevMouse = Mouse.GetState();
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            foreach( var b in level.Spawn)
            {
                PhysicsRectangle rect = new PhysicsRectangle(new Position(b.X - (tex.Spawn.Width / 2), b.Y - (tex.Spawn.Height / 2)), tex.Spawn.Width, tex.Spawn.Height);
                DrawRect(spriteBatch, rect, tex.Spawn);
            }
            DrawLevel(spriteBatch, level, tex);
            if (currentTool == Tools.Rects || currentTool == Tools.Goal || currentTool == Tools.Death)
            {
                if(holding)
                {
                    Texture2D t = tex.Wall;
                    if (currentTool == Tools.Goal)
                        t = tex.Goal;
                    if (currentTool == Tools.Death)
                        t = tex.Death;

                    DrawRect(spriteBatch, PhysicsRectangle.rectangleBetweenPoints(mPos, clickMPos), t);
                }
            }

        }

        public static void DrawRect(SpriteBatch spriteBatch, PhysicsRectangle pRect, Texture2D texture)
        {
            Rectangle rect = new Rectangle((int)pRect.Position.X, (int)pRect.Position.Y, (int)pRect.Width, (int)pRect.Height);
            spriteBatch.Draw(texture, rect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
        }

        public static void DrawRect(SpriteBatch spriteBatch, PhysicsRectangle pRect, Texture2D texture, float layer)
        {
            Rectangle rect = new Rectangle((int)pRect.Position.X, (int)pRect.Position.Y, (int)pRect.Width, (int)pRect.Height);
            spriteBatch.Draw(texture, rect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        public static void DrawLevel(SpriteBatch spriteBatch, Level lvl, Assets t)
        {
            foreach (var obj in lvl.Obstacles)
                LevelEditor.DrawRect(spriteBatch, obj, t.Wall, 0.5f);
            foreach (var obj in lvl.Death)
                LevelEditor.DrawRect(spriteBatch, obj, t.Death, 0.4f);
            foreach(var obj in lvl.Goal)
                LevelEditor.DrawRect(spriteBatch, obj, t.Goal, 0.3f);
        }
        void save()
        {
            StreamWriter sw = new StreamWriter("Levels/" + level.ID);
            sw.WriteLine("<spawn>");

            foreach (var s in level.Spawn)
            {
                sw.WriteLine("<s>");

                sw.WriteLine(s.X);
                sw.WriteLine(s.Y);

                sw.WriteLine("</s>");
            }

            sw.WriteLine("</spawn>");
            foreach (var g in level.Goal)
            {
                sw.WriteLine("<goal>");

                sw.WriteLine(g.Position.X);
                sw.WriteLine(g.Position.Y);
                sw.WriteLine(g.Width);
                sw.WriteLine(g.Height);

                sw.WriteLine("</goal>");
            }
            sw.WriteLine("<death>");

            foreach(var d in level.Death)
            {
                sw.WriteLine("<d>");

                sw.WriteLine(d.Position.X);
                sw.WriteLine(d.Position.Y);
                sw.WriteLine(d.Width);
                sw.WriteLine(d.Height);

                sw.WriteLine("</d>");
            }


            sw.WriteLine("</death>");
            sw.WriteLine("<rects>");

            foreach (var r in level.Obstacles)
            {
                sw.WriteLine("<r>");

                sw.WriteLine(r.Position.X);
                sw.WriteLine(r.Position.Y);
                sw.WriteLine(r.Width);
                sw.WriteLine(r.Height);

                sw.WriteLine("</r>");
            }

            sw.WriteLine("</rects>");

            sw.Close();
        }

        public static Level Load(Level lvl)
        {
            StreamReader sr = new StreamReader("Levels/" + lvl.ID);

            string line = "";
            while(!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if(line == "<spawn>")
                {
                    while (line != "</spawn>")
                    {
                        line = sr.ReadLine();

                        if (line == "<s>")
                        {
                            double x, y;
                            line = sr.ReadLine();
                            x = Convert.ToDouble(line);
                            line = sr.ReadLine();
                            y = Convert.ToDouble(line);
                            lvl.Spawn.Add(new Position(x, y));
                        }

                    }
                }
                if(line == "<goal>")
                {
                    double x, y, w, h;
                    line = sr.ReadLine();
                    x = Convert.ToDouble(line);
                    line = sr.ReadLine();
                    y = Convert.ToDouble(line);
                    line = sr.ReadLine();
                    w = Convert.ToDouble(line);
                    line = sr.ReadLine();
                    h = Convert.ToDouble(line);
                    lvl.Goal.Add(new PhysicsRectangle(x, y, w, h));
                }
                if(line == "<death>")
                {
                    while(line != "</death>")
                    {
                        line = sr.ReadLine();

                        if(line == "<d>")
                        {
                            double x, y, w, h;
                            line = sr.ReadLine();
                            x = Convert.ToDouble(line);
                            line = sr.ReadLine();
                            y = Convert.ToDouble(line);
                            line = sr.ReadLine();
                            w = Convert.ToDouble(line);
                            line = sr.ReadLine();
                            h = Convert.ToDouble(line);

                            lvl.Death.Add(new PhysicsRectangle(x, y, w, h));
                        }

                    }
                }

                if (line == "<rects>")
                {
                    while (line != "</rects>")
                    {
                        line = sr.ReadLine();

                        if (line == "<r>")
                        {
                            double x, y, w, h;
                            line = sr.ReadLine();
                            x = Convert.ToDouble(line);
                            line = sr.ReadLine();
                            y = Convert.ToDouble(line);
                            line = sr.ReadLine();
                            w = Convert.ToDouble(line);
                            line = sr.ReadLine();
                            h = Convert.ToDouble(line);

                            lvl.Obstacles.Add(new PhysicsRectangle(x, y, w, h));
                        }

                    }
                }
            }

            sr.Close();
            return lvl;
        }
    }
}
