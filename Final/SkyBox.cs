using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;


namespace Project
{
    using SharpDX.Toolkit.Graphics;
    class SkyBox: Shape2
    {
        public Vector3 centre_pos;
        public SkyBox(LabGame game) : base(game)
        {
            // Specify the file name of the texture being used
            centre_pos = new Vector3(0f, 0f, 0f);
            textureName = "project2_galaxy_background.png";
            basicEffect.Texture = game.Content.Load<Texture2D>(textureName);
            basicEffect.TextureEnabled = true;
            basicEffect.VertexColorEnabled = false;

            // This is where the vertices of the cube are specififed.  Each vertex has a position and a colour.
            // TASK 1: Complete the cube by adding vertex definitions for the front and back faces

            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                new[]
                    {
                        new VertexPositionTexture(new Vector3(-50f, -49f, -55f), new Vector2(0.0f, 0.5f)), // Front
                        new VertexPositionTexture(new Vector3(-50f, 51f, -55f), new Vector2(0.0f, 0.0f)),
                        new VertexPositionTexture(new Vector3(50f, 51f, -55f), new Vector2(0.3333f, 0.0f)),
                        new VertexPositionTexture(new Vector3(-50f, -49f, -55f), new Vector2(0.0f, 0.5f)),
                        new VertexPositionTexture(new Vector3(50f, 51f, -55f), new Vector2(0.3333f, 0.0f)),
                        new VertexPositionTexture(new Vector3(50f, -49f, -55f), new Vector2(0.3333f, 0.5f)),

                        new VertexPositionTexture(new Vector3(-50f, -49f, 45f), new Vector2(1.0f, 1.0f)), // BACK
                        new VertexPositionTexture(new Vector3(50f, -49f, 45f), new Vector2(0.666667f, 1.0f)),
                        new VertexPositionTexture(new Vector3(50f, 51f, 45f), new Vector2(0.666667f, 0.5f)),
                        new VertexPositionTexture(new Vector3(-50f, -49f, 45f), new Vector2(1.0f, 1.0f)),
                        new VertexPositionTexture(new Vector3(50f, 51f, 45f), new Vector2(0.666667f, 0.5f)),
                        new VertexPositionTexture(new Vector3(-50f, 51f, 45f), new Vector2(1.0f, 0.5f)),

                        new VertexPositionTexture(new Vector3(-50f, 51f, -55f), new Vector2(0.33333f, 0.5f)), // Top
                        new VertexPositionTexture(new Vector3(-50f, 51f, 45f), new Vector2(0.33333f, 0.0f)),
                        new VertexPositionTexture(new Vector3(50f, 51f, 45f), new Vector2(0.66667f, 0.0f)),
                        new VertexPositionTexture(new Vector3(-50f, 51f, -55f), new Vector2(0.33333f, 0.5f)),
                        new VertexPositionTexture(new Vector3(50f, 51f, 45f), new Vector2(0.66667f, 0.0f)),
                        new VertexPositionTexture(new Vector3(50f, 51f, -55f), new Vector2(0.66667f, 0.5f)),

                        new VertexPositionTexture(new Vector3(-50f, -49f, -55f), new Vector2(0.33333f, 0.5f)), // Bottom
                        new VertexPositionTexture(new Vector3(50f, -49f, 45f), new Vector2(0.666667f, 1.0f)),
                        new VertexPositionTexture(new Vector3(-50f, -49f, 45f), new Vector2(0.33333f, 1.0f)),
                        new VertexPositionTexture(new Vector3(-50f, -49f, -55f), new Vector2(0.33333f, 0.5f)),
                        new VertexPositionTexture(new Vector3(50f, -49f, -55f), new Vector2(0.666667f, 0.5f)),
                        new VertexPositionTexture(new Vector3(50f, -49f, 45f), new Vector2(0.666667f, 1.0f)),

                        new VertexPositionTexture(new Vector3(-50.0f, -49f, -55f), new Vector2(1.0f, 0.5f)), // Left
                        new VertexPositionTexture(new Vector3(-50.0f, -49f, 45f), new Vector2(0.66667f, 0.5f)),
                        new VertexPositionTexture(new Vector3(-50.0f, 51f, 45f), new Vector2(0.66667f, 0.0f)),
                        new VertexPositionTexture(new Vector3(-50.0f, -49f, -55f), new Vector2(1.0f, 0.5f)),
                        new VertexPositionTexture(new Vector3(-50.0f, 51f, 45f), new Vector2(0.666667f, 0.0f)),
                        new VertexPositionTexture(new Vector3(-50.0f, 51f, -55f), new Vector2(1.0f, 0.0f)),

                        new VertexPositionTexture(new Vector3(50.0f, -49f, -55f), new Vector2(0.0f, 1.0f)), // Right
                        new VertexPositionTexture(new Vector3(50.0f, 51f, 45f), new Vector2(0.33333f, 0.5f)),
                        new VertexPositionTexture(new Vector3(50.0f, -49f, 45f), new Vector2(0.33333f, 1.0f)),
                        new VertexPositionTexture(new Vector3(50.0f, -49f, -55f), new Vector2(0.0f, 1.0f)),
                        new VertexPositionTexture(new Vector3(50.0f, 51f, -55f), new Vector2(0.0f, 0.5f)),
                        new VertexPositionTexture(new Vector3(50.0f, 51f, 45f), new Vector2(0.33333f, 0.5f)),

            });
        }

        public override void Update(GameTime gameTime)
        {
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            basicEffect.View = this.game.current_camera.View;
            basicEffect.World = Matrix.Translation(centre_pos);
        }
    }
}
