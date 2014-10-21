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

    // Super class for all game objects.
    abstract public class GameObject
    {

        public Model model;
        public Vector3 pos;


        public Effect effect;

        public VertexInputLayout inputLayout;
        public Buffer<VertexPositionNormalColor> vertices;
        public LabGame game;
        public BasicEffect basicEffect;

        public abstract void Update(GameTime gametime);
        public abstract void Draw(GameTime gametime);
    }
}
