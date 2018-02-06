﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>Represents an indexed mesh. These store mesh data by referring to vertices using index values stored in an index buffer. 
    /// In most cases this reduces the vertex data size drastically.</summary>
    /// <typeparam name="T">The vertex type in the form of a <see cref="IVertexType"/> type.</typeparam>
    /// <seealso cref="Molten.Graphics.Mesh{T}" />
    /// <seealso cref="Molten.Graphics.IIndexedMesh" />
    public class IndexedMesh<T> : Mesh<T>, IIndexedMesh<T> where T : struct, IVertexType
    {
        BufferSegment _ib;
        int _maxIndices;
        IndexBufferFormat _iFormat;

        internal IndexedMesh(RendererDX11 renderer, int maxVertices, int maxIndices, VertexTopology topology, IndexBufferFormat indexFormat, bool visible) : 
            base(renderer, maxVertices, topology)
        {
            _maxIndices = maxIndices;
            _iFormat = indexFormat;

            switch (_iFormat)
            {
                case IndexBufferFormat.Unsigned16Bit:
                    _ib = renderer.StaticIndexBuffer.Allocate<ushort>(maxIndices);
                    break;

                case IndexBufferFormat.Unsigned32Bit:
                    _ib = renderer.StaticIndexBuffer.Allocate<uint>(maxIndices);
                    break;
            }
        }

        public void SetIndices<I>(I[] data) where I : struct
        {
            throw new NotImplementedException();
        }

        public void SetIndices<I>(I[] data, int count) where I : struct
        {
            throw new NotImplementedException();
        }

        public void SetIndices<I>(I[] data, int startIndex, int count) where I : struct
        {
            _ib.SetData(_renderer.Device.ExternalContext, data, startIndex, count);
        }

        internal override void ApplyBuffers(GraphicsPipe pipe)
        {
            base.ApplyBuffers(pipe);

            // TODO call PipelineInput.SetIndexSegment(_ib);
        }

        internal override void Render(GraphicsPipe pipe, RendererDX11 renderer, ObjectRenderData data, SceneRenderDataDX11 sceneData)
        {
            ApplyBuffers(pipe);

            //renderer.Device.DrawIndexed();
        }

        public int MaxIndices => _maxIndices;
    }
}