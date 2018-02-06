﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal struct SurfaceClearChange : ITextureChange
    {
        public RenderSurfaceBase Surface;

        public Color Color;
         
        public void Process(GraphicsPipe pipe, TextureBase texture)
        {
            Surface.Clear(pipe, Color);
        }
    }
}