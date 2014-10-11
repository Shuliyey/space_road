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
    class Space : Shape
    {
        private Matrix World;
        private Matrix WorldInverseTranspose;
        private Random rand;

        public Space(LabGame game)
        {
            rand = new Random();
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

        private VertexPositionNormalColor[] _space_track_generate(Vector3 start_pos) 
        {
            return new VertexPositionNormalColor[]{};
        }

        private VertexPositionNormalColor[] _space_track_straight_line(Vector3 start_pos, float pitch_angle, Vector3 start_direction, int num)
        {
            float len = rand.NextFloat(50f, 150f);
            float new_pitch = rand.NextFloat(-(float)Math.PI/4, (float)Math.PI/4);
            float delta = len / num;
            float delta_pitch = (new_pitch - pitch_angle) / num;
            List<VertexPositionNormalColor> the_vertices = new List<VertexPositionNormalColor>();
            Vector3[] centres = new Vector3[num + 1];
            float[] pitches = new float[num + 1];
            centres[0] = start_pos;
            pitches[0] = pitch_angle;
            for (int i = 1; i <= num; i++)
            {
                centres[i] = centres[i - 1] + Vector3.Multiply(start_direction, delta);
                pitches[i] = pitches[i - 1] + delta_pitch;
            }
            for (int i = 1; i < num; i++)
            {

            }
            return new VertexPositionNormalColor[] { };
        }

        private VertexPositionNormalColor[] _space_track_curve(Vector3 start_pos, double pitch_angle, Vector3 start_direction, int num)
        {
            return new VertexPositionNormalColor[] { };
        }

        private VertexPositionNormalColor[] _space_track_s_shape(Vector3 start_pos, double pitch_angle, Vector3 start_direction, int num)
        {
            return new VertexPositionNormalColor[] { };
        }

        private VertexPositionNormalColor[] _space_track_spiral_down(Vector3 start_pos, Vector3 start_direction)
        {
            return new VertexPositionNormalColor[] { };
        }

        private VertexPositionNormalColor[] _space_track_zig_zag(Vector3 start_pos, Vector3 start_direction)
        {
            return new VertexPositionNormalColor[] { };
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
