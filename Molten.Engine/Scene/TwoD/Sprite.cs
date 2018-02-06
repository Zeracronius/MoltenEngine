﻿using Molten.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten
{
    public class Sprite : ISprite
    {
        public ITexture2D Texture;
        public Rectangle Source;
        public Vector2 Position;
        public float Rotation;
        public Vector2 Origin;
        public Color Color = Color.White;

        public void Render(ISpriteBatch batch)
        {
            batch.Draw(Texture, Position, Source, Color, Rotation, Origin);
        }
    }
}