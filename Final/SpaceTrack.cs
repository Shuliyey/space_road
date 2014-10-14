using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using Windows.UI.Input;
using Windows.UI.Core;

namespace Project
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    class SpaceTrack : Shape
    {
        private Matrix World;
        private Matrix WorldInverseTranspose;
        private Random rand;
        private const int epsilon_num = 1000;
        private float start_pitch;
        private Vector3 start_position;
        private Vector3 start_derivative;
        public float final_pitch;
        public Vector3 final_position;
        public Vector3 final_derivative;

        public SpaceTrack(LabGame game, Vector3 start, Vector3 velocity, float angle)
        {
            rand = game.random;
            start_pitch = angle;
            start_position = start;
            start_derivative = velocity;
            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                _space_track_generate(start_position, start_pitch, start_derivative, epsilon_num)
            );

            effect = game.Content.Load<Effect>("myShader");
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {
            // Rotate the cube.
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            World = Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f);
            //World = Matrix.Identity;
            WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(World));

            effect.Parameters["World"].SetValue(World);
            effect.Parameters["Projection"].SetValue(game.camera.Projection);
            effect.Parameters["View"].SetValue(game.camera.View);
            effect.Parameters["cameraPos"].SetValue(game.camera.cameraPos);
            effect.Parameters["worldInvTrp"].SetValue(WorldInverseTranspose);
        }

        private VertexPositionNormalColor[] _space_track_generate(Vector3 start_pos, float pitch_angle, Vector3 start_direction, int num) 
        {
            /*
            Vector3 frontNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 backNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);
            return new[] {
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
            };
            */
            return _space_track_straight_line(start_pos, pitch_angle, start_direction, num);
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
            Vector3 direction_xz = new Vector3(start_direction.X, 0, start_direction.Z);
            direction_xz.Normalize();
            Quaternion rotate_quat = Quaternion.RotationAxis(new Vector3(0.0f, 1.0f, 0.0f), (float)Math.PI/2);
            Vector3 zero_pitch_vec = Vector3.Transform(direction_xz, rotate_quat);
            Quaternion quat = Quaternion.RotationAxis(start_direction, pitches[0]);
            Vector3 pre_vec1 = centres[0] + Vector3.Multiply(Vector3.Transform(zero_pitch_vec, quat), 1.5f);
            Vector3 pre_vec2 = centres[0] + Vector3.Multiply(Vector3.Transform(-zero_pitch_vec, quat), 1.5f);
            for (int i = 1; i <= num; i++)
            {
                quat = Quaternion.RotationAxis(start_direction, pitches[i]);
                Vector3 vec1 = centres[i] + Vector3.Multiply(Vector3.Transform(zero_pitch_vec, quat), 1.5f);
                Vector3 vec2 = centres[i] + Vector3.Multiply(Vector3.Transform(-zero_pitch_vec, quat), 1.5f);
                Vector3 plane1_vec1 = vec2 - pre_vec1;
                Vector3 plane1_vec2 = vec1 - pre_vec1;
                Vector3 tri1_normal = Vector3.Cross(plane1_vec1, plane1_vec2);
                Vector3 plane2_vec1 = vec2 - pre_vec2;
                Vector3 plane2_vec2 = pre_vec1 - pre_vec2;
                Vector3 tri2_normal = Vector3.Cross(plane2_vec1, plane2_vec2);
                the_vertices.Add(new VertexPositionNormalColor(pre_vec1, tri1_normal, Color.Orange));
                the_vertices.Add(new VertexPositionNormalColor(vec2, tri1_normal, Color.Orange));
                the_vertices.Add(new VertexPositionNormalColor(vec1, tri1_normal, Color.Orange));
                the_vertices.Add(new VertexPositionNormalColor(pre_vec2, tri2_normal, Color.Orange));
                the_vertices.Add(new VertexPositionNormalColor(vec2, tri2_normal, Color.Orange));
                the_vertices.Add(new VertexPositionNormalColor(pre_vec1, tri2_normal, Color.Orange));
                pre_vec1 = vec1;
                pre_vec2 = vec2;
            }
            final_derivative = start_derivative;
            final_pitch = pitches[num];
            final_position = centres[num];
            return the_vertices.ToArray();
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
