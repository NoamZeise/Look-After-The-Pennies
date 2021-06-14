using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera2D
{
    public class Camera
    {
        GraphicsDevice _graphicsDevice;
        GraphicsDeviceManager _graphics;


        public Rectangle ScreenRectangle;
        public RenderTarget2D RenderTarget;
        public Matrix Translation;

        double wScreenTargetRatio = 1;
        double hScreenTargetRatio = 1;
        Rectangle cameraRect;
        int _targetWidth
        {
            get
            {
                return RenderTarget.Width;
            }
        }

        int _targetHeight
        {
            get
            {
                return RenderTarget.Height;
            }
        }
        Vector2 camera = new Vector2(0, 0);

        public Point independentMousePoint //offset mouse pos by camera rectangle
        {
            get
            {
                var mousePoint = Mouse.GetState().Position;

                //mousePoint = new Point((int)(SCREEN_WIDTH * ((double)mousePoint.X / (double)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)), (int)(SCREEN_HEIGHT * ((double)mousePoint.Y / (double)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)));
                mousePoint = new Point((int)(wScreenTargetRatio * mousePoint.X) + cameraRect.X, (int)(hScreenTargetRatio * mousePoint.Y) + cameraRect.Y);
                //Console.WriteLine(mousePoint.X);
                return mousePoint;
            }
        }
        public Camera(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, RenderTarget2D renderTarget)
        {
            _graphics = graphics;
            _graphicsDevice = graphicsDevice;
            RenderTarget = renderTarget;

            cameraRect = new Rectangle(0, 0, renderTarget.Width, renderTarget.Height);

            Translation = Matrix.CreateTranslation(0, 0, 0);
            PostGraphicsChange();
        }

        private Rectangle setScreenRectangle(int screenWidth, int screenHeight, int targetWidth, int targetHeight)
        {
            /*
            if (screenWidth > screenHeight || targetHeight > targetWidth)
                return new Rectangle((screenWidth - (int)((float)screenHeight * ((float)targetWidth / (float)targetHeight))) / 2, 0, (int)((float)screenHeight * ((float)targetWidth / (float)targetHeight)), screenHeight);
            if (screenHeight > screenWidth || targetWidth > targetHeight)
                return new Rectangle(0, (screenHeight - (int)((float)screenWidth * ((float)targetHeight / (float)targetWidth))) / 2, screenWidth, (int)((float)screenWidth * ((float)targetHeight / (float)targetWidth)));
            return new Rectangle(0, 0, screenWidth, screenHeight);
            */
            double widthRatio, heightRatio;
            widthRatio = (double)screenWidth / (double)targetWidth;
            heightRatio = (double)screenHeight / (double)targetHeight;
            double ratio = widthRatio;
            if (widthRatio > heightRatio)
                ratio = heightRatio;
            double topOffset, sideOffset;
            topOffset = ((double)screenHeight - ((double)targetHeight * ratio)) / 2;
            sideOffset = ((double)screenWidth - ((double)targetWidth * ratio)) / 2;
            return new Rectangle((int)sideOffset, (int)topOffset, (int)((double)targetWidth * ratio), (int)((double)targetHeight * ratio));
        }


        public void PostGraphicsChange()
        {
            //Translation = Matrix.CreateTranslation(((float)(_targetWidth * scale) - _targetWidth) / 2f, ((float)(_targetHeight * scale) - _targetWidth) / 2f, 0) *
            //   Matrix.CreateTranslation(-camera.X, -camera.Y, 0);
            RenderTarget = new RenderTarget2D(_graphicsDevice, (int)(_targetWidth), (int)(_targetHeight));
            ScreenRectangle = setScreenRectangle(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, RenderTarget.Width, RenderTarget.Height);
            wScreenTargetRatio = (double)RenderTarget.Width / (double)_graphics.PreferredBackBufferWidth;
            hScreenTargetRatio = (double)RenderTarget.Height / (double)_graphics.PreferredBackBufferHeight;
        }
        public void SetOffset(Vector2 offset)
        {
            camera = offset;
        }
        public void Follow(Vector2 pos)
        {
            camera = pos;
        }
    }
}

/*
 * EXAMPLE UPDATE FUNCTION
 * 
        protected override void Update(GameTime gameTime)
        {
           
            _camera.Follow(location);
            _camera.SetScale(scale);
        }


//////EXAMPLE DRAW FUNCTION

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_camera.RenderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.Translation);

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_camera.RenderTarget, _camera.ScreenRectangle, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
        */
