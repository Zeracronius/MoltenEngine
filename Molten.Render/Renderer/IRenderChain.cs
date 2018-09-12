﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public interface IRenderChain
    {
        void Build(SceneRenderData sceneData, RenderCamera camera);

        void Render(SceneRenderData sceneData, RenderCamera camera, Timing time);
    }
}
