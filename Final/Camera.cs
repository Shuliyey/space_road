﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
namespace Project
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    public class Camera
    {
        public Matrix View;
        public Matrix Projection;
        public Game game;
        public Vector3 cameraPos;
        public Vector3 cameraTarget;
        public Vector3 viewVector;
        public Vector3 pos;
        public Vector3 oldPos;
        private float roate_speed = (float)Math.PI / 36;

        // Ensures that all objects are being rendered from a consistent viewpoint
        public Camera(Game game) {
            cameraPos = new Vector3(0, 1, -5);
            cameraTarget = new Vector3(0, 1, 0);
            pos = new Vector3(0, 1, -5);
            View = Matrix.LookAtLH(cameraPos, cameraTarget, Vector3.UnitY);
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
            this.game = game;
        }

        // If the screen is resized, the projection matrix will change
        public void Update()
        {
            Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
            viewVector = cameraTarget - cameraPos;
            var keyboard_state = ((LabGame)game).keyboardState;
            if (keyboard_state.IsKeyDown(Keys.Up))
            {
                View *= Matrix.Translation(new Vector3(0f, 0f, -0.25f));
            }

            if (keyboard_state.IsKeyDown(Keys.Down))
            {
                View *= Matrix.Translation(new Vector3(0f, 0f, 0.25f));
            }

            if (keyboard_state.IsKeyDown(Keys.Left))
            {
                View *= Matrix.Translation(new Vector3(0.25f, 0, 0));
            }


            if (keyboard_state.IsKeyDown(Keys.Right))
            {
                View *= Matrix.Translation(new Vector3(-0.25f, 0, 0));
            }

            if (keyboard_state.IsKeyDown(Keys.Q))
            {
                View *= Matrix.Translation(new Vector3(0f, -0.25f, 0));
            }


            if (keyboard_state.IsKeyDown(Keys.E))
            {
                View *= Matrix.Translation(new Vector3(0f, 0.25f, 0));
            }

            if (keyboard_state.IsKeyDown(Keys.W))
            {
                View *= Matrix.RotationX(roate_speed);
            }

            if (keyboard_state.IsKeyDown(Keys.S))
            {
                View *= Matrix.RotationX(-roate_speed);
            }

            if (keyboard_state.IsKeyDown(Keys.A))
            {
                View *= Matrix.RotationY(roate_speed);
            }

            if (keyboard_state.IsKeyDown(Keys.D))
            {
                View *= Matrix.RotationY(-roate_speed);
            }
        }
    }
}
