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
        public int epsilon_num = 1000;
        public int points_per_curve = 1000;
        private float start_pitch;
        private Vector3 start_position;
        private Vector3 start_derivative;
        public float final_pitch;
        public Vector3 final_position;
        public Vector3 final_derivative;
        public Vector3[] centres;
        public float[] pitches;
        public Vector3[] directions;
        public Vector3[] normals;
        public bool started = false;
        public float start_time;
        public float[] distances;
        public float period = 0.0f;
        public float length;
        public int pos = 0;
        public bool allow_add = true;
        private static bool straight = true;
        private const float track_thinkness = 2f;
        public bool rightTurn = false;
        public bool leftTurn = false;

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
            
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            World = Matrix.Identity;
            WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(World));
            Vector3 cameraViewVector = new Vector3(game.camera.viewVector.X, game.camera.viewVector.Y, game.camera.viewVector.Z);
            cameraViewVector.Normalize();
            cameraViewVector = Vector3.Multiply(cameraViewVector, 50f);

            //Change the position of the light focus based on the current camera position
            Vector4 lightpos = new Vector4(game.camera.cameraPos.X + cameraViewVector.X, game.camera.cameraPos.Y + cameraViewVector.Y, game.camera.cameraPos.Z + cameraViewVector.Z, 1f);
            
            //Update the basic effect parameters
            effect.Parameters["World"].SetValue(World);
            effect.Parameters["Projection"].SetValue(game.camera.Projection);
            effect.Parameters["View"].SetValue(game.camera.View);
            effect.Parameters["cameraPos"].SetValue(game.camera.cameraPos);
            effect.Parameters["worldInvTrp"].SetValue(WorldInverseTranspose);
            effect.Parameters["lightPntPos"].SetValue(lightpos);
        }

        private VertexPositionNormalColor[] _space_track_generate(Vector3 start_pos, float pitch_angle, Vector3 start_direction, int num) 
        {
            //Flips generation of the track between a straight section and a curved section
            if(straight){
                straight = false;
                return _space_track_straight_line(start_pos, pitch_angle, start_direction, num); 
            }
            straight = true;
            return _space_track_curve(start_pos, pitch_angle, start_direction);
        }

        private VertexPositionNormalColor[] _space_track_straight_line(Vector3 start_pos, float pitch_angle, Vector3 start_direction, int num)
        {
            length = rand.NextFloat(50f, 150f);
            float new_pitch = rand.NextFloat(-(float)Math.PI/4, (float)Math.PI/4);
            float delta = length / num;
            float delta_pitch = (new_pitch - pitch_angle) / num;
            List<VertexPositionNormalColor> the_vertices = new List<VertexPositionNormalColor>();
            centres = new Vector3[num + 1];
            pitches = new float[num + 1];
            directions = new Vector3[num + 1];
            normals = new Vector3[num + 1];
            distances = new float[num + 1];
            centres[0] = start_pos;
            pitches[0] = pitch_angle;
            directions[0] = start_derivative;
            distances[0] = 0f;
            for (int i = 1; i <= num; i++)
            {
                centres[i] = centres[i - 1] + Vector3.Multiply(start_direction, delta);
                distances[i] = distances[i-1] + delta;
                directions[i] = start_derivative;
                pitches[i] = pitches[i - 1] + delta_pitch;
            }
            Vector3 direction_xz = new Vector3(start_direction.X, 0, start_direction.Z);
            direction_xz.Normalize();
            Quaternion rotate_quat = Quaternion.RotationAxis(new Vector3(0f, 1f, 0f), (float)Math.PI/2);
            Vector3 zero_pitch_vec = Vector3.Transform(direction_xz, rotate_quat);
            Quaternion quat = Quaternion.RotationAxis(start_direction, pitches[0]);
            Vector3 pitch_vec = Vector3.Transform(zero_pitch_vec, quat);
            normals[0] = Vector3.Cross(directions[0], pitch_vec);
            Vector3 pre_vec1 = centres[0] + Vector3.Multiply(pitch_vec, track_thinkness);
            Vector3 pre_vec2 = centres[0] + Vector3.Multiply(-pitch_vec, track_thinkness);
            for (int i = 1; i <= num; i++)
            {
                quat = Quaternion.RotationAxis(start_direction, pitches[i]);
                pitch_vec = Vector3.Transform(zero_pitch_vec, quat);
                normals[i] = Vector3.Cross(directions[i], pitch_vec);
                Vector3 vec1 = centres[i] + Vector3.Multiply(pitch_vec, track_thinkness);
                Vector3 vec2 = centres[i] + Vector3.Multiply(-pitch_vec, track_thinkness);
                Vector3 plane1_vec1 = vec2 - pre_vec1;
                Vector3 plane1_vec2 = vec1 - pre_vec1;
                Vector3 tri1_normal = Vector3.Cross(plane1_vec2, plane1_vec1);
                Vector3 plane2_vec1 = vec2 - pre_vec2;
                Vector3 plane2_vec2 = pre_vec1 - pre_vec2;
                Vector3 tri2_normal = Vector3.Cross(plane2_vec2, plane2_vec1);
                the_vertices.Add(new VertexPositionNormalColor(pre_vec1, tri1_normal, Color.Yellow));
                the_vertices.Add(new VertexPositionNormalColor(vec1, tri1_normal, Color.Yellow));
                the_vertices.Add(new VertexPositionNormalColor(vec2, tri1_normal, Color.Yellow));
                the_vertices.Add(new VertexPositionNormalColor(pre_vec2, tri2_normal, Color.Yellow));
                the_vertices.Add(new VertexPositionNormalColor(pre_vec1, tri2_normal, Color.Yellow));
                the_vertices.Add(new VertexPositionNormalColor(vec2, tri2_normal, Color.Yellow));
                // rendering the back
                the_vertices.Add(new VertexPositionNormalColor(pre_vec1, -tri1_normal, Color.DarkOrange));
                the_vertices.Add(new VertexPositionNormalColor(vec2, -tri1_normal, Color.DarkOrange));
                the_vertices.Add(new VertexPositionNormalColor(vec1, -tri1_normal, Color.DarkOrange));
                the_vertices.Add(new VertexPositionNormalColor(pre_vec2, -tri2_normal, Color.DarkOrange));
                the_vertices.Add(new VertexPositionNormalColor(vec2, -tri2_normal, Color.DarkOrange));
                the_vertices.Add(new VertexPositionNormalColor(pre_vec1, -tri2_normal, Color.DarkOrange));
                pre_vec1 = vec1;
                pre_vec2 = vec2;
            }
            final_derivative = start_derivative;
            final_pitch = pitches[num];
            final_position = centres[num];
            return the_vertices.ToArray();
        }

        private VertexPositionNormalColor[] _space_track_curve(Vector3 start_pos, float pitch_angle, Vector3 start_direction)
        {
            // The track generation creates an arc based on a randomised period and radius, period ranging between -PI and PI
            period = rand.Next(0,2) == 0? rand.NextFloat((float)Math.PI/4, (float)Math.PI): rand.NextFloat(-(float)Math.PI, -(float)Math.PI/4);

            // Defines whether this section of track is a right turn or a left turn
            rightTurn = period < 0;
            leftTurn = period > 0;

            float radius = rand.NextFloat(16f, 50f);
            List<VertexPositionNormalColor> the_vertices = new List<VertexPositionNormalColor>();
            float new_pitch = rand.NextFloat(-(float)Math.PI / 9, (float)Math.PI / 9);
            float delta = period / points_per_curve;
            float delta_pitch = (new_pitch - pitch_angle) / points_per_curve;
            centres = new Vector3[points_per_curve + 1];
            pitches = new float[points_per_curve + 1];
            directions = new Vector3[points_per_curve + 1];
            normals = new Vector3[points_per_curve + 1];
            distances = new float[points_per_curve + 1];
            distances[0] = 0f;
            int num = points_per_curve;
            centres[0] = start_pos;
            pitches[0] = pitch_angle;
            directions[0] = start_derivative;
            Vector3 current_direction = start_direction;
            float delta_length = (float)Math.Sqrt(Math.Pow(radius - radius * Math.Cos(delta), 2) + Math.Pow(-radius * Math.Sin(delta), 2));
            length = delta_length * points_per_curve;

            // For each point that will be in the curve, calculate the center point of the track and change the current direction towards
            // the curvature of the arc
            for (int i = 1; i <= points_per_curve; i++)
            {
                current_direction = Vector3.Transform(current_direction, Quaternion.RotationAxis(new Vector3(0f, 1f, 0f), delta));
                current_direction.Normalize();
                distances[i] = distances[i - 1] + delta_length;
                centres[i] = centres[i - 1] + Vector3.Multiply(current_direction, delta_length);
                pitches[i] = pitches[i - 1]+ delta_pitch;
                directions[i] = current_direction;
            }
            Vector3 direction_xz = new Vector3(directions[0].X, 0, directions[0].Z);
            direction_xz.Normalize();
            Quaternion rotate_quat = Quaternion.RotationAxis(new Vector3(0.0f, 1.0f, 0.0f), (float)Math.PI / 2);
            Vector3 zero_pitch_vec = Vector3.Transform(direction_xz, rotate_quat);
            Quaternion quat = Quaternion.RotationAxis(directions[0], pitches[0]);
            Vector3 pitch_vec = Vector3.Transform(zero_pitch_vec, quat);
            normals[0] = Vector3.Cross(directions[0], pitch_vec);
            Vector3 pre_vec1 = centres[0] + Vector3.Multiply(pitch_vec, track_thinkness);
            Vector3 pre_vec2 = centres[0] + Vector3.Multiply(-pitch_vec, track_thinkness);
            for (int i = 1; i <= num; i++)
            {
                direction_xz = new Vector3(directions[i].X, 0, directions[i].Z);
                direction_xz.Normalize();
                rotate_quat = Quaternion.RotationAxis(new Vector3(0.0f, 1.0f, 0.0f), (float)Math.PI / 2);
                zero_pitch_vec = Vector3.Transform(direction_xz, rotate_quat);
                quat = Quaternion.RotationAxis(directions[i], pitches[i]);
                pitch_vec = Vector3.Transform(zero_pitch_vec, quat);
                normals[i] = Vector3.Cross(directions[i], pitch_vec);
                Vector3 vec1 = centres[i] + Vector3.Multiply(pitch_vec, track_thinkness);
                Vector3 vec2 = centres[i] + Vector3.Multiply(-pitch_vec, track_thinkness);
                Vector3 plane1_vec1 = vec2 - pre_vec1;
                Vector3 plane1_vec2 = vec1 - pre_vec1;
                Vector3 tri1_normal = Vector3.Cross(plane1_vec2, plane1_vec1);
                Vector3 plane2_vec1 = vec2 - pre_vec2;
                Vector3 plane2_vec2 = pre_vec1 - pre_vec2;
                Vector3 tri2_normal = Vector3.Cross(plane2_vec2, plane2_vec1);
                // Renders the track based on the verticies to the left and right of both the current center point
                // and the one calculated beforehand
                the_vertices.Add(new VertexPositionNormalColor(pre_vec1, tri1_normal, Color.Yellow));
                the_vertices.Add(new VertexPositionNormalColor(vec1, tri1_normal, Color.Yellow));
                the_vertices.Add(new VertexPositionNormalColor(vec2, tri1_normal, Color.Yellow));
                the_vertices.Add(new VertexPositionNormalColor(pre_vec2, tri2_normal, Color.Yellow));
                the_vertices.Add(new VertexPositionNormalColor(pre_vec1, tri2_normal, Color.Yellow));
                the_vertices.Add(new VertexPositionNormalColor(vec2, tri2_normal, Color.Yellow));
                // Rendering the underside of the track
                the_vertices.Add(new VertexPositionNormalColor(pre_vec1, -tri1_normal, Color.DarkOrange));
                the_vertices.Add(new VertexPositionNormalColor(vec2, -tri1_normal, Color.DarkOrange));
                the_vertices.Add(new VertexPositionNormalColor(vec1, -tri1_normal, Color.DarkOrange));
                the_vertices.Add(new VertexPositionNormalColor(pre_vec2, -tri2_normal, Color.DarkOrange));
                the_vertices.Add(new VertexPositionNormalColor(vec2, -tri2_normal, Color.DarkOrange));
                the_vertices.Add(new VertexPositionNormalColor(pre_vec1, -tri2_normal, Color.DarkOrange));
                pre_vec1 = vec1;
                pre_vec2 = vec2;
            }
            final_derivative = current_direction;
            final_pitch = pitches[num];
            final_position = centres[num];

            return the_vertices.ToArray();
        }

        public int space_track_walk(Camera camera, Player space_ship, float current_time)
        {
            // Finds the path that the ship will take if it follows the center line of the track
            float move_speed = space_ship.speed;
            float total_distance = move_speed * (current_time - start_time);
            while (pos < epsilon_num && distances[pos] < total_distance)
            {
                pos++;
            }
            Vector3 centre_pos = centres[pos];
            Vector3 normal_vec = normals[pos];
            Vector3 new_space_ship_pos = centre_pos + Vector3.Multiply(normal_vec, 0.5f);
            Vector3 new_camera_pos = centre_pos + Vector3.Multiply(normal_vec,1);
            camera.cameraPos = new_camera_pos;
            camera.cameraTarget = new_camera_pos + Vector3.Multiply(directions[pos], 5f);
            camera.pos = new_camera_pos;
            camera.View = Matrix.LookAtRH(camera.cameraPos, camera.cameraTarget, normal_vec);
            space_ship.pos = new_space_ship_pos;
            return pos;
        }

        public void start(float time)
        {
            started = true;
            start_time = time;
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
