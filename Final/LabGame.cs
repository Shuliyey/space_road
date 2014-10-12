﻿// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;

namespace Lab
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    public class LabGame : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private List<Shape> models;
        private Stack<Shape> added_models;
        private Stack<Shape> removed_models;
        private KeyboardManager keyboardManager;
        public KeyboardState keyboardState;
        public Camera camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabGame" /> class.
        /// </summary>
        public LabGame()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            // Create the keyboard manager
            keyboardManager = new KeyboardManager(this);
            
        }

        protected override void LoadContent()
        {
            camera = new Camera(this);
            models = new List<Shape>();
            added_models = new Stack<Shape>();
            removed_models = new Stack<Shape>();
            models.Add(new SpaceTrack(this, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), 0));
            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Space Road";

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            keyboardState = keyboardManager.GetState();
            flushAddedAndRemovedModels();
            camera.Update();
            for (int i = 0; i < models.Count; i++)
            {
                models[i].Update(gameTime);
            }

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
                this.Dispose();
            }
            // Handle base.Update
            base.Update(gameTime);

        }

        // Process the buffers of game objects that need to be added/removed.
        private void flushAddedAndRemovedModels()
        {
            while (added_models.Count > 0) { models.Add(added_models.Pop()); }
            while (removed_models.Count > 0) { models.Remove(removed_models.Pop()); }
        }


        // Add a new game object.
        public void Add(Shape obj)
        {
            if (!models.Contains(obj) && !added_models.Contains(obj))
            {
                added_models.Push(obj);
            }
        }

        // Remove a game object.
        public void Remove(Shape obj)
        {
            if (models.Contains(obj) && !removed_models.Contains(obj))
            {
                removed_models.Push(obj);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            for (int i = 0; i < models.Count; i++)
            {
                models[i].Draw(gameTime);
            }
            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
