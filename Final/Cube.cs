using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Lab
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    class Cube : Shape
    {
        private Matrix World;
        private Matrix WorldInverseTranspose;

        public Cube(LabGame game)
        {
            Vector3 frontNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 backNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);

            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                new[]
                    {
                    new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), frontNormal, Color.Orange), // Front
                    new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, -1.0f), frontNormal, Color.Orange),
                    new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, -1.0f), frontNormal, Color.Orange),
                    new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), frontNormal, Color.Orange),
                    new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, -1.0f), frontNormal, Color.Orange),
                    new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, -1.0f), frontNormal, Color.Orange),
                    new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, 1.0f), backNormal, Color.Orange), // BACK
                    new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), backNormal, Color.Orange),
                    new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, 1.0f), backNormal, Color.Orange),
                    new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, 1.0f), backNormal, Color.Orange),
                    new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, 1.0f), backNormal, Color.Orange),
                    new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), backNormal, Color.Orange),
                    new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, -1.0f), topNormal, Color.OrangeRed), // Top
                    new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, 1.0f), topNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), topNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, -1.0f), topNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), topNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, -1.0f), topNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), bottomNormal, Color.OrangeRed), // Bottom
                    new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, 1.0f), bottomNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, 1.0f), bottomNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f),bottomNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, -1.0f), bottomNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, 1.0f), bottomNormal, Color.OrangeRed),
                    new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), leftNormal, Color.DarkOrange), // Left
                    new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, 1.0f), leftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, 1.0f), leftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(new Vector3(-1.0f, -1.0f, -1.0f), leftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, 1.0f), leftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(new Vector3(-1.0f, 1.0f, -1.0f), leftNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, -1.0f), rightNormal, Color.DarkOrange), // Right
                    new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), rightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, 1.0f), rightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(new Vector3(1.0f, -1.0f, -1.0f), rightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, -1.0f), rightNormal, Color.DarkOrange),
                    new VertexPositionNormalColor(new Vector3(1.0f, 1.0f, 1.0f), rightNormal, Color.DarkOrange),
                });

            effect = game.Content.Load<Effect>("myShader");

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {
            // Rotate the cube.
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            World = Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f);
            WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(World));

            effect.Parameters["World"].SetValue(World);
            effect.Parameters["Projection"].SetValue(game.camera.Projection);
            effect.Parameters["View"].SetValue(game.camera.View);
            effect.Parameters["cameraPos"].SetValue(game.camera.cameraPos);
            effect.Parameters["worldInvTrp"].SetValue(WorldInverseTranspose);
        }

        private VertexPositionNormalColor[] space_track_generate(Vector3 start_pos) {
            return new VertexPositionNormalColor[]{};
        }

        public override void Draw(GameTime gameTime)
        {
            // Set the effect values
            effect.Parameters["World"].SetValue(World);
            effect.Parameters["Projection"].SetValue(game.camera.Projection);
            effect.Parameters["View"].SetValue(game.camera.View);
            effect.Parameters["cameraPos"].SetValue(game.camera.cameraPos);
            effect.Parameters["worldInvTrp"].SetValue(WorldInverseTranspose);

            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            effect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }
    }
}
