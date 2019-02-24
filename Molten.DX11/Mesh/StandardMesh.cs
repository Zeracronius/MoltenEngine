﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class StandardMesh : Mesh<GBufferVertex>
    {
        internal StandardMesh(RendererDX11 renderer, int maxVertices, VertexTopology topology, bool dynamic) : 
            base(renderer, maxVertices, topology, dynamic)
        {

        }

        private protected override void OnRender(PipeDX11 pipe, RendererDX11 renderer, RenderCamera camera, ObjectRenderData data)
        {
            ApplyBuffers(pipe);
            IShaderResource normal = GetResource(1);
            Material mat = _material;

            if (mat == null)
            {
                int[] test;
                // Use whichever default one fits the current configuration.
                if (normal == null)
                    mat = renderer.StandardMeshMaterial_NoNormalMap;
                else
                    mat = renderer.StandardMeshMaterial;

                mat.Object.EmissivePower.Value = EmissivePower;
            }

            mat.Object.World.Value = data.RenderTransform;
            mat.Object.Wvp.Value = Matrix4F.Multiply(data.RenderTransform, camera.ViewProjection);

            ApplyResources(mat);
            renderer.Device.Draw(mat, _vertexCount, _topology);
        }
    }
}
