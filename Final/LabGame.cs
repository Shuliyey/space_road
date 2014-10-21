// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
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
using Windows.UI.Input;
using Windows.UI.Core;
using Windows.Devices.Sensors;

namespace Project
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;

    public class LabGame : Game
    {
        private SpriteBatch sprite;
        private GraphicsDeviceManager graphicsDeviceManager;
        public List<GameObject> gameObjects;
        private List<Shape> models;
        private SpaceTrack current_track;
        private int track_index;
        private Queue<Shape> added_models;
        private Queue<Shape> removed_models;
        private Stack<GameObject> addedGameObjects;
        private Stack<GameObject> removedGameObjects;
        private KeyboardManager keyboardManager;
        public KeyboardState keyboardState;
        private Player player;
        public AccelerometerReading accelerometerReading;
        public GameInput input;
        public MainPage mainPage;
        private Texture2D background;
        private bool right_turn = false, left_turn = false;
        // TASK 4: Use this to represent difficulty
        public float difficulty;

        // Represents the camera's position and orientation
        public Camera camera;
        public Camera camera2;
        public Camera current_camera;
        // Graphics assets
        public Assets assets;

        // Random number generator
        public Random random;

        // World boundaries that indicate where the edge of the screen is for the camera.
        public float boundaryLeft;
        public float boundaryRight;
        public float boundaryTop;
        public float boundaryBottom;

        public bool started = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="LabGame" /> class.
        /// </summary>
        public LabGame(MainPage mainPage)
        {
            models = new List<Shape>();
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";

            // Create the keyboard manager
            keyboardManager = new KeyboardManager(this);
            assets = new Assets(this);
            random = new Random();
            input = new GameInput();
            track_index = 0;
            // Set boundaries.
            boundaryLeft = -4.5f;
            boundaryRight = 4.5f;
            boundaryTop = 4;
            boundaryBottom = -4.5f;

            // Initialise event handling.
            /*
            input.gestureRecognizer.Tapped += Tapped;
            input.gestureRecognizer.ManipulationStarted += OnManipulationStarted;
            input.gestureRecognizer.ManipulationUpdated += OnManipulationUpdated;
            input.gestureRecognizer.ManipulationCompleted += OnManipulationCompleted;
            */
            this.mainPage = mainPage;
            mainPage.UpdateScore(0);
            difficulty = 1;
        }

        protected override void LoadContent()
        {
            // Initialise game object containers.
            models = new List<Shape>();
            added_models = new Queue<Shape>();
            removed_models = new Queue<Shape>();
            gameObjects = new List<GameObject>();
            addedGameObjects = new Stack<GameObject>();
            removedGameObjects = new Stack<GameObject>();

            // Load background
            background = Content.Load<Texture2D>("space");

            // Create game objects.
            player = new Player(this);
            gameObjects.Add(player);
            //gameObjects.Add(new EnemyController(this));
            SpaceTrack pre_track = new SpaceTrack(this, new Vector3(0.0f, 0.0f, -10.0f), new Vector3(0.0f, 0.0f, 1.0f), 0);
            AddModel(pre_track);
            for (int i = 1; i < 5; i++)
            {
                SpaceTrack next_track = new SpaceTrack(this, pre_track.final_position, pre_track.final_derivative, pre_track.final_pitch);
                AddModel(next_track);
                pre_track = next_track;
            }
            flushAddedAndRemovedModels();
            current_track = (SpaceTrack)(models[0]);
            // Create an input layout from the vertices

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Space Track";
            camera = new Camera(this);
            camera2 = new Camera(this);
            camera2.cameraPos += new Vector3(0, 50f, 0);
            camera2.cameraTarget = new Vector3(0, 0, 0);
            camera2.pos = new Vector3(0, 50f, 0f);
            camera2.View = Matrix.LookAtRH(camera2.cameraPos, camera2.cameraTarget, Vector3.UnitZ);
            current_camera = camera;
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            if (started)
            {
                float current_time = (float)gameTime.TotalGameTime.TotalSeconds;
                keyboardState = keyboardManager.GetState();
                flushAddedAndRemovedModels();
                flushAddedAndRemovedGameObjects();
                if (keyboardState.IsKeyPressed(Keys.Space))
                {
                    if (current_camera == camera)
                    {
                        current_camera = camera2;
                    }
                    else
                    {
                        current_camera = camera;
                    }
                }
                player.Update(gameTime);
                camera.Update();
                camera2.Update();
                //accelerometerReading = input.accelerometer.GetCurrentReading();

                // Getting the current accelerometer reading
                /*
                accelerometerReading = input.accelerometer.GetCurrentReading();

                // Changes boolean variables based on whether or not the tablet is turned right, left or neither
                if (accelerometerReading.AccelerationX > 0.3)
                {
                    right_turn = true;
                    left_turn = false;
                }
                else if (accelerometerReading.AccelerationX < -0.3)
                {
                    right_turn = false;
                    left_turn = true;
                }
                else
                {
                    right_turn = false;
                    left_turn = false;
                }
                */
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    right_turn = false;
                    left_turn = true;
                }
                else if (keyboardState.IsKeyDown(Keys.Right))
                {
                    left_turn = false;
                    right_turn = true;

                }
                else
                {
                    left_turn = false;
                    right_turn = false;
                }
                mainPage.UpdateScore((int)gameTime.TotalGameTime.Seconds);
                for (int i = models.Count-1; i >=0; i--)
                {
                    models[i].Update(gameTime);
                }

                runGame(current_time);
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    this.Exit();
                    this.Dispose();
                    App.Current.Exit();
                }
                // Handle base.Update
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            
            if (started)
            {
                // Clears the screen with the Color.CornflowerBlue
                // GraphicsDevice.Clear(Color.CornflowerBlue);
                sprite = new SpriteBatch(GraphicsDevice);
                sprite.Begin();

                // need to change the rectangle size to full screen size
                sprite.Draw(background, new RectangleF(0,0,2000,1200), Color.White);
                sprite.End();
                for (int i = models.Count - 1; i >= 0; i--)
                {
                    models[i].Draw(gameTime);
                }
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].Draw(gameTime);
                }
            }
            // Handle base.Draw
            base.Draw(gameTime);
        }
        // Count the number of game objects for a certain type.
        
        public int Count(GameObjectType type)
        {
            int count = 0;
            foreach (var obj in gameObjects)
            {
                if (obj.type == type) { count++; }
            }
            return count;
        }

        // Add a new game object.
        public void Add(GameObject obj)
        {
            if (!gameObjects.Contains(obj) && !addedGameObjects.Contains(obj))
            {
                addedGameObjects.Push(obj);
            }
        }

        // Remove a game object.
        public void Remove(GameObject obj)
        {
            if (gameObjects.Contains(obj) && !removedGameObjects.Contains(obj))
            {
                removedGameObjects.Push(obj);
            }
        }

        // Process the buffers of game objects that need to be added/removed.
        private void flushAddedAndRemovedGameObjects()
        {
            while (addedGameObjects.Count > 0) { gameObjects.Add(addedGameObjects.Pop()); }
            while (removedGameObjects.Count > 0) { gameObjects.Remove(removedGameObjects.Pop()); }
        }


        //Process the buffers of game objects that need to be added/removed.
        private void flushAddedAndRemovedModels()
        {
            while (added_models.Count > 0) { models.Add(added_models.Dequeue()); }
            while (removed_models.Count > 0) { models.Remove(removed_models.Dequeue()); track_index--;}
        }

        
        // Add a new game object.
        public void AddModel(Shape obj)
        {
            if (!models.Contains(obj) && !added_models.Contains(obj))
            {
                added_models.Enqueue(obj);
            }
        }

        // Remove a game object.
        public void RemoveModel(Shape obj)
        {
            if (models.Contains(obj) && !removed_models.Contains(obj))
            {
                removed_models.Enqueue(obj);
            }
        }
        /*
        public void OnManipulationStarted(GestureRecognizer sender, ManipulationStartedEventArgs args)
        {
            // Pass Manipulation events to the game objects.

        }

        public void Tapped(GestureRecognizer sender, TappedEventArgs args)
        {
            // Pass Manipulation events to the game objects.
            foreach (var obj in gameObjects)
            {
                obj.Tapped(sender, args);
            }
        }

        public void OnManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
            camera.pos.Z = camera.pos.Z * args.Delta.Scale;
            // Update camera position for all game objects
            foreach (var obj in gameObjects)
            {
                if (obj.basicEffect != null) { obj.basicEffect.View = camera.View; }
                obj.OnManipulationUpdated(sender, args);
            }
        }

        public void OnManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {
        }
        */

        public void runGame(float current_time)
        {
            if (!current_track.started)
            {
                current_track.start(current_time);
            }
            int new_pos = current_track.space_track_walk(camera, camera2, (Player)gameObjects[0], current_time);
            if (track_index == 2)
            {
                SpaceTrack last_track = (SpaceTrack)models[models.Count - 1];
                SpaceTrack new_track = new SpaceTrack(this, last_track.final_position, last_track.final_derivative, last_track.final_pitch);
                RemoveModel(models[0]);
                AddModel(new_track);
                /*
                for (int i = 1; i < 2; i++)
                {
                    last_track = new_track;
                    new_track = new SpaceTrack(this, last_track.final_position, last_track.final_derivative, last_track.final_pitch);
                    AddModel(new_track);
                    RemoveModel(models[i]);
                }
                */
                flushAddedAndRemovedModels();
                current_track.allow_add = false;
            }
            if ((float)current_track.pos / current_track.epsilon_num > 0.1f && (float)current_track.pos / current_track.epsilon_num < 0.9f)
            {
                /*
                if (current_track.rightTurn != right_turn || current_track.leftTurn != left_turn) 
                {
                    mainPage.UpdateScore(0);
                    MainMenu new_menu = new MainMenu(mainPage);
                    mainPage.mainMenu = new_menu;
                    mainPage.addMenu(new_menu);
                    this.started = false;
                }
                */
            }
            if (new_pos == current_track.epsilon_num)
            {
                track_index++;
                current_track = (SpaceTrack)models[track_index];
            }
        }
    }
}
