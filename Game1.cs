using Camera2D;
using LimitedBudgetMonojam.Editor;
using LimitedBudgetMonojam.GameObjects;
using LimitedBudgetMonojam.GameObjects.PhysicsObjects;
using LimitedBudgetMonojam.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using static LimitedBudgetMonojam.Editor.LevelEditor;

namespace LimitedBudgetMonojam
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Camera _camera;

        public const int SCREEN_WIDTH = 600;
        public const int SCREEN_HEIGHT = 800;

        KeyboardState prevKey;
        MouseState prevMouse;
        UIManager uIManager;

        List<GameObject> gObj;
        GameObject mouse;
        double force = 300;
        Texture2D gamebg;
        Texture2D titlebg;

        int cLvl, maxLvl;
        double mWidth = 5;
        enum State
        {
            Menu,
            paused,
            playing,
            toy,
            levelEditor
        }

        State gameState = State.Menu;
        State prevState = State.paused;

        LevelEditor editor;

        Level currentLevel;

        bool forceOff = false;

        SoundEffectInstance hit;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            RenderTarget2D rt = new RenderTarget2D(GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT);
            _camera = new Camera(GraphicsDevice, _graphics, rt);
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
            _camera.PostGraphicsChange();

            gamebg = Content.Load<Texture2D>("bg");
            titlebg = Content.Load<Texture2D>("title");
            hit = Content.Load<SoundEffect>("hit").CreateInstance();

            uIManager = new UIManager(Content);
            uIManager.menus.Main.Buttons[0].WhenPressed += PlayButtonFunct;
            uIManager.menus.Main.Buttons[1].WhenPressed += EditorButtonFunct;
            uIManager.menus.Main.Buttons[2].WhenPressed += ToyButtonFunct;
            uIManager.menus.Pause.Buttons[1].WhenPressed += menuButtonFunct;
            maxLvl = 0;
            while(true)
            {
                if (File.Exists("Levels/" + (maxLvl + 1)))
                {
                    maxLvl++;
                }
                else
                    break;
            }
            cLvl = 1;
            editor = new LevelEditor(Content, (maxLvl + 1).ToString());
            if (maxLvl >= cLvl)
                LoadLevel(cLvl.ToString());

        }

        void LoadLevel(string id)
        {
            currentLevel = new Level(id);
            currentLevel = LevelEditor.Load(currentLevel);
            var map = new Map(currentLevel);
            mouse = new GameObject(new Position(_camera.independentMousePoint.X, _camera.independentMousePoint.Y), Content.Load<Texture2D>("GameObjects/mouse"));
            mWidth = Content.Load<Texture2D>("GameObjects/mouse").Width;
            mouse.Layer = 0.7f;
            gObj = new List<GameObject>();
            gObj.Add(mouse);
            gObj.Add(map);
            foreach( var s in currentLevel.Spawn)
            {
                AddBall(s);
            }
        }

        private void AddBall(Position pos)
        {
            Position adjPos = new Position(pos.X - (editor.tex.Spawn.Width / 2), pos.Y - (editor.tex.Spawn.Height / 2));
            var b = new Ball(adjPos, editor.tex.Spawn);
            gObj.Add(b);
        }

        protected override void Update(GameTime gameTime)
        {
            uIManager.Update(gameTime, _camera);

            if (uIManager.shouldClose)
                Exit();

            if (uIManager.messages.Count == 0)
            {
                if (gameState != State.Menu)
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape) && prevKey.IsKeyUp(Keys.Escape))
                    {
                        bool paused = uIManager.togglePause();
                        if (paused)
                        {
                            prevState = gameState;
                            gameState = State.paused;
                        }
                        else
                        {
                            gameState = prevState;
                        }
                    }
                if(gameState == State.paused)
                {
                    if (!uIManager.getPause())
                    {
                        gameState = prevState;
                    }
                }

                if (gameState == State.playing || gameState == State.toy)
                {
                    if (IsMouseVisible)
                        IsMouseVisible = false;

                    Position mPos = new Position(_camera.independentMousePoint.X, _camera.independentMousePoint.Y);
                    mouse.Position = mPos;
                    int bCount = 0;
                    foreach (var obj in gObj)
                    {
                        if (obj is Ball)
                        {
                            bCount++;
                            if (gameState == State.playing || !forceOff)
                            {
                                var tPos = new Position(mPos.X + (mWidth / 2), mPos.Y + (mWidth / 2));
                                if (Physics.Colliding(new PhysicsCircle(tPos, mWidth / 2), obj.Colliders[0] as PhysicsCircle))
                                {
                                    var uV = Physics.UnitVectorBetween(obj.Colliders[0].Center, tPos);
                                    obj.Velocity = uV * force;
                                }
                            }
                            foreach (var d in currentLevel.Death)
                            {
                                if (Physics.Colliding(obj.Colliders[0] as PhysicsCircle, d))
                                {
                                    obj.ChangePosition(obj.Inital);
                                    obj.Velocity = new Position(0, 0);
                                }
                            }
                            foreach (var g in currentLevel.Goal)
                            {
                                if (Physics.Colliding(g, obj.Colliders[0] as PhysicsCircle))
                                {
                                    obj.isRemoved = true;
                                }
                            }
                        }
                        obj.Update(gameTime);
                    }

                    CollisionManager.HandleCollisions(gObj);

                    for (int i = 0; i < gObj.Count; i++)
                    {
                        if (gObj[i].isRemoved)
                            gObj.RemoveAt(i--);
                    }
                    
                    if(bCount == 0 && gameState == State.playing)
                    {
                        if (cLvl + 1 <= maxLvl)
                        {
                            LoadLevel((++cLvl).ToString());
                        }
                        else
                        {
                            uIManager.ShowMessage("you won! thanks for playing...", 0, -300);
                            menuButtonFunct(default, default);
                        }
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) && prevKey.IsKeyUp(Keys.LeftShift))
                    {
                        force *= 2;
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.LeftShift) && prevKey.IsKeyDown(Keys.LeftShift))
                    {
                        force /= 2;
                    }
                    if (gameState == State.toy)
                    {
                        if(Mouse.GetState().LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
                        {
                            AddBall(new Position(_camera.independentMousePoint.X + 4, _camera.independentMousePoint.Y));
                        }
                        if(Keyboard.GetState().IsKeyDown(Keys.C) && prevKey.IsKeyUp(Keys.C))
                        {
                            foreach(var o in gObj)
                            {
                                if (o is Ball)
                                    (o as Ball).isRemoved = true;
                            }
                        }
                        if (Keyboard.GetState().IsKeyUp(Keys.F) && prevKey.IsKeyDown(Keys.F))
                        {
                            forceOff = !forceOff;
                        }
                    }
                }
                else
                {
                    if (!IsMouseVisible)
                        IsMouseVisible = true;
                }

                if (gameState == State.levelEditor)
                {
                    editor.Update(gameTime, _camera);
                }

                if (gameState == State.Menu)
                {
                    
                }
            }
            prevKey = Keyboard.GetState();
            prevMouse = Mouse.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_camera.RenderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.Translation, sortMode: SpriteSortMode.FrontToBack);


            uIManager.Draw(_spriteBatch);

            if ((gameState == State.playing || (prevState == State.playing && gameState == State.paused)) ||
                    (gameState == State.toy || (prevState == State.toy && gameState == State.toy)))
            {
                _spriteBatch.Draw(gamebg, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
                foreach (var obj in gObj)
                    obj.Draw(_spriteBatch);
                LevelEditor.DrawLevel(_spriteBatch, currentLevel, editor.tex);
            }

            if (gameState == State.levelEditor || (prevState == State.Menu && gameState == State.levelEditor))
            {
                editor.Draw(_spriteBatch);
            }

            if(gameState == State.Menu || (prevState == State.Menu && gameState == State.paused))
            {
                _spriteBatch.Draw(titlebg, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.1f);
            }

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_camera.RenderTarget, _camera.ScreenRectangle, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void menuButtonFunct(object sender, UIEventArgs e)
        {
            gameState = State.Menu;
            prevState = State.Menu;
            uIManager.menus.Main.visible = true;
            uIManager.menus.Pause.visible = false;
            while (true)
            {
                if (File.Exists("Levels/" + (maxLvl + 1)))
                {
                    maxLvl++;
                }
                else
                    break;
            }
            editor = new LevelEditor(Content, (maxLvl + 1).ToString());
            if (maxLvl >= cLvl)
                LoadLevel(cLvl.ToString());
        }

        public void PlayButtonFunct(object sender, UIEventArgs e)
        {
            gameState = State.playing;
            uIManager.menus.Main.visible = false;

            LoadLevel(cLvl.ToString());
        }
        public void ToyButtonFunct(object sender, UIEventArgs e)
        {
            gameState = State.toy;
            uIManager.menus.Main.visible = false;
            LoadLevel("toy");
        }

        public void EditorButtonFunct(object sender, UIEventArgs e)
        {
            gameState = State.levelEditor;
            uIManager.menus.Main.visible = false;
        }
    }
}
