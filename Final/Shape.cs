using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Lab
{
    using SharpDX.Toolkit.Graphics;

    abstract public class Shape
    {
        public Effect effect;
        public VertexInputLayout inputLayout;
		public Buffer<VertexPositionNormalColor> vertices;
        public LabGame game;
        public BasicEffect basicEffect;

        public abstract void Update(GameTime gametime);
        public abstract void Draw(GameTime gametime);
    }
}
