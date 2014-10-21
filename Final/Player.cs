using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using Windows.UI.Input;
using Windows.UI.Core;

namespace Project
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    // Player class.
    class Player : GameObject
    {

        private BoundingSphere modelBounds;
        public Matrix world;
        private Matrix view;
        private Matrix projection;
        public float speed = 20f;
        public float pitch = 0f;
        public Vector3 direction_vec = new Vector3(0, 0, 1f);
        private float difficulty = 1f;

        public Player(LabGame game)
        {
            this.game = game;
            //type = GameObjectType.Player;
            //myModel = game.assets.GetModel("player", CreatePlayerModel);
            pos = new SharpDX.Vector3(0, game.boundaryBottom + 0.5f, 0);
            model = game.Content.Load<Model>("SpaceShip");
            //Calculates the world and the view based on the model size
            const float MaxModelSize = 10.0f;
            var scaling = MaxModelSize / modelBounds.Radius;
            view = game.camera.View;
            projection = game.camera.Projection;
            world = Matrix.Identity * Matrix.Scaling(0.5f);
            //GetParamsFromModel();
        }

        // Frame update.
        public override void Update(GameTime gameTime)
        {
            
            // Change the speed over time to increase the difficulty the longer you play
            speed += (float)gameTime.ElapsedGameTime.TotalSeconds * difficulty;
            // Keep within the boundaries.
            /*
            if (pos.X < game.boundaryLeft) { pos.X = game.boundaryLeft; }
            if (pos.X > game.boundaryRight) { pos.X = game.boundaryRight; }
            */
            float turning_angle = calculateAngle();
            world = Matrix.Scaling(0.5f) * Matrix.RotationY(turning_angle) * Matrix.RotationAxis(direction_vec, pitch) * Matrix.Translation(pos);
        }

        public float calculateAngle()
        {
            Vector3 zero_angle = new Vector3(0f, 0f, 1f);
            float turning_angle = (float)Math.Acos(Vector3.Dot(zero_angle, direction_vec));
            turning_angle = direction_vec.X < 0 ? ((float)(2 * Math.PI) - turning_angle) : turning_angle;
            return turning_angle;
        }

        public override void Draw(GameTime gameTime)
        {
            modelBounds = model.CalculateBounds();
            const float MaxModelSize = 10.0f;
            var scaling = MaxModelSize / modelBounds.Radius;
            //world = Matrix.Scaling(0.25f) * Matrix.Translation(-modelBounds.Center.X, -modelBounds.Center.Y, -modelBounds.Center.Z) * Matrix.Scaling(scaling) * Matrix.RotationY((float)gameTime.TotalGameTime.TotalSeconds);
            //world = Matrix.Identity * Matrix.Scaling(0.5f);
            BasicEffect.EnableDefaultLighting(model, true);
            //model.Draw(game.GraphicsDevice, Matrix.Identity, game.camera.View, game.camera.Projection);
            //model.Draw(game.GraphicsDevice, world, game.camera.View, game.camera.Projection);
            model.Draw(game.GraphicsDevice, world, game.current_camera.View, game.current_camera.Projection);
        }

        public void ChangeDifficulty(float difficulty)
        {
            this.difficulty = difficulty;
        }


        /*
        public MyModel CreatePlayerModel()
        {
            return game.assets.CreateTexturedCube("player.png", 0.7f);
        }

        // Method to create projectile texture to give to newly created projectiles.
        private MyModel CreatePlayerProjectileModel()
        {
            return game.assets.CreateTexturedCube("player projectile.png", new Vector3(0.3f, 0.2f, 0.25f));
        }

        // Shoot a projectile.
        private void fire()
        {
            game.Add(new Projectile(game,
                game.assets.GetModel("player projectile", CreatePlayerProjectileModel),
                pos,
                new Vector3(0, projectileSpeed, 0),
                GameObjectType.Enemy
            ));
        }

       

        // React to getting hit by an enemy bullet.
        public void Hit()
        {
            game.Exit();
        }

        public override void Tapped(GestureRecognizer sender, TappedEventArgs args)
        {
            fire();
        }
         
        
        public override void OnManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
            pos.X += (float)args.Delta.Translation.X / 100;
        }*/
    }
}