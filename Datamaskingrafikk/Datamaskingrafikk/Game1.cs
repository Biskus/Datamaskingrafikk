using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Datamaskingrafikk
{
    public partial class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        KeyboardState currentKeys;

        Effect skyEffect;

        TextureCube skyTex;

        Matrix SkyWorld, View, Projection;

        Vector3 originalView = new Vector3(0, 0, 10);
        Vector3 position = Vector3.Zero;
        float angleX = 0.00f, angleY = 0.00f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {

            skyEffect = Content.Load<Effect>("SkyEffect");

            skyTex = Content.Load<TextureCube>("SkyBoxTex");

            skyEffect.Parameters["tex"].SetValue(skyTex);

            SkyWorld = Matrix.Identity;
            View = Matrix.CreateLookAt(position, originalView, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1, 20);

            //Backreference calls
            CreateCubeVertexBuffer();
            CreateCubeIndexBuffer();
        }

        protected override void UnloadContent()
        {
        }

        void UpdateView(int dir)
        {
            Vector3 tempView = Vector3.Transform(originalView, Matrix.CreateRotationX(angleX));
            tempView = Vector3.Transform(tempView, Matrix.CreateRotationY(angleY));
            Vector3 tempUp = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(angleX));
            tempUp = Vector3.Transform(tempUp, Matrix.CreateRotationY(angleY));
            position += dir * Vector3.Normalize(tempView) / 10;
            View = Matrix.CreateLookAt(Vector3.Zero + position, tempView + position, tempUp);
        }


        protected override void Update(GameTime gameTime)
        {
            currentKeys = Keyboard.GetState();

            //Press Esc To Exit
            if (currentKeys.IsKeyDown(Keys.Escape))
                this.Exit();


            //Press Directional Keys to rotate the camera
            if (currentKeys.IsKeyDown(Keys.Up))
            {
                angleX -= 0.01f;
                if (angleX < -MathHelper.PiOver2) angleX = -MathHelper.PiOver2;
                UpdateView(0);
            }
            if (currentKeys.IsKeyDown(Keys.Down))
            {
                angleX += 0.01f;
                if (angleX > MathHelper.PiOver2) angleX = MathHelper.PiOver2;
                UpdateView(0);
            }
            if (currentKeys.IsKeyDown(Keys.Left))
            {
                angleY += 0.01f;
                UpdateView(0);
            }
            if (currentKeys.IsKeyDown(Keys.Right))
            {
                angleY -= 0.01f;
                UpdateView(0);
            }

            //Page Up Page Down to move forward/backward
            if (currentKeys.IsKeyDown(Keys.PageUp))
            {
                UpdateView(1);
                SkyWorld = Matrix.CreateTranslation(position);
            }
            if (currentKeys.IsKeyDown(Keys.PageDown))
            {
                UpdateView(-1);
                SkyWorld = Matrix.CreateTranslation(position);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.SetVertexBuffer(vertices);
            GraphicsDevice.Indices = indices;

            skyEffect.Parameters["WVP"].SetValue(SkyWorld * View * Projection);
            skyEffect.CurrentTechnique.Passes[0].Apply();

            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, number_of_vertices, 0, number_of_indices / 3);

            base.Draw(gameTime);
        }
    }
}