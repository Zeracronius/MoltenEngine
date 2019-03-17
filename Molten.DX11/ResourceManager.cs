﻿using SharpDX.DXGI;
using System;
using System.Collections.Generic;

namespace Molten.Graphics
{
    /// <summary>DirectX 11 implementation of <see cref="IResourceManager"/>.</summary>
    public class ResourceManager : IResourceManager
    {
        RendererDX11 _renderer;
        List<SpriteFont> _fontTable;

        internal ResourceManager(RendererDX11 renderer)
        {
            _renderer = renderer;
            _fontTable = new List<SpriteFont>();
        }

        public IDepthStencilSurface CreateDepthSurface(
            int width,
            int height,
            DepthFormat format = DepthFormat.R24G8_Typeless,
            int mipCount = 1,
            int arraySize = 1,
            int sampleCount = 1,
            TextureFlags flags = TextureFlags.None)
        {
            return new DepthStencilSurface(_renderer, width, height, format, mipCount, arraySize, sampleCount, flags);
        }

        public INativeSurface CreateFormSurface(string formTitle, string formName, int mipCount = 1, int sampleCount = 1)
        {
            return new RenderFormSurface(formTitle, formName, _renderer, mipCount);
        }

        public INativeSurface CreateControlSurface(string formTitle, string controlName, int mipCount = 1, int sampleCount = 1)
        {
            return new RenderControlSurface(formTitle, controlName, _renderer, mipCount);
        }

        public IRenderSurface CreateSurface(
            int width,
            int height,
            GraphicsFormat format = GraphicsFormat.R8G8B8A8_SNorm,
            int mipCount = 1,
            int arraySize = 1,
            int sampleCount = 1,
            TextureFlags flags = TextureFlags.None)
        {
            return new RenderSurface(_renderer, width, height, (Format)format, mipCount, arraySize, sampleCount, flags);
        }

        public ITexture CreateTexture1D(Texture1DProperties properties)
        {
            return new Texture1DDX11(_renderer, properties.Width, properties.Format.ToApi(), properties.MipMapLevels, properties.ArraySize, properties.Flags);
        }

        public ITexture CreateTexture1D(TextureData data)
        {
            Texture1DDX11 tex = new Texture1DDX11(_renderer, data.Width, data.Format.ToApi(), data.MipMapLevels, data.ArraySize, data.Flags);
            tex.SetData(data, 0, 0, data.MipMapLevels, data.ArraySize);
            return tex;
        }

        public ITexture2D CreateTexture2D(Texture2DProperties properties)
        {
            return new Texture2DDX11(_renderer,
                properties.Width,
                properties.Height,
                properties.Format.ToApi(),
                properties.MipMapLevels,
                properties.ArraySize,
                properties.Flags,
                properties.SampleCount);
        }

        public ITexture2D CreateTexture2D(TextureData data)
        {
            Texture2DDX11 tex = new Texture2DDX11(_renderer,
                data.Width,
                data.Height,
                data.Format.ToApi(),
                data.MipMapLevels,
                data.ArraySize,
                data.Flags,
                data.SampleCount);

            tex.SetData(data, 0, 0, data.MipMapLevels, data.ArraySize);
            return tex;
        }

        public ITextureCube CreateTextureCube(Texture2DProperties properties)
        {
            int cubeCount = Math.Max(properties.ArraySize / 6, 1);
            return new TextureCubeDX11(_renderer, properties.Width, properties.Height, properties.Format.ToApi(), properties.MipMapLevels, cubeCount, properties.Flags);
        }

        public ITextureCube CreateTextureCube(TextureData data)
        {
            int cubeCount = Math.Max(data.ArraySize / 6, 1);
            TextureCubeDX11 tex = new TextureCubeDX11(_renderer, data.Width, data.Height, data.Format.ToApi(), data.MipMapLevels, cubeCount, data.Flags);
            tex.SetData(data, 0, 0, data.MipMapLevels, data.ArraySize);
            return tex;
        }

        /// <summary>
        /// Resolves a source texture into a destination texture. <para/>
        /// This is most useful when re-using the resulting rendertarget of one render pass as an input to a second render pass. <para/>
        /// Another common use is transferring (resolving) a multisampled texture into a non-multisampled texture.
        /// </summary>
        /// <param name="source">The source texture.</param>
        /// <param name="destination">The destination texture.</param>
        public void ResolveTexture(ITexture source, ITexture destination)
        {
            if (source.Format != destination.Format)
                throw new Exception("The source and destination texture must be the same format.");

            int arrayLevels = Math.Min(source.ArraySize, destination.ArraySize);
            int mipLevels = Math.Min(source.MipMapCount, destination.MipMapCount);

            for (int i = 0; i < arrayLevels; i++)
            {
                for (int j = 0; j < mipLevels; j++)
                {
                    TextureResolve task = TextureResolve.Get();
                    task.Source = source as TextureBase;
                    task.Destination = destination as TextureBase;
                    task.SourceMipLevel = j;
                    task.SourceArraySlice = i;
                    task.DestMipLevel = j;
                    task.DestArraySlice = i;
                    _renderer.PushTask(task);
                }
            }
        }

        /// <summary>Resources the specified sub-resource of a source texture into the sub-resource of a destination texture.</summary>
        /// <param name="source">The source texture.</param>
        /// <param name="destination">The destination texture.</param>
        /// <param name="sourceMipLevel">The source mip-map level.</param>
        /// <param name="sourceArraySlice">The source array slice.</param>
        /// <param name="destMiplevel">The destination mip-map level.</param>
        /// <param name="destArraySlice">The destination array slice.</param>
        public void ResolveTexture(ITexture source, ITexture destination,
            int sourceMipLevel,
            int sourceArraySlice,
            int destMiplevel,
            int destArraySlice)
        {
            if (source.Format != destination.Format)
                throw new Exception("The source and destination texture must be the same format.");

            TextureResolve task = TextureResolve.Get();
            task.Source = source as TextureBase;
            task.Destination = destination as TextureBase;
            _renderer.PushTask(task);
        }

        public void Dispose()
        {
            for (int i = 0; i < _fontTable.Count; i++)
                _fontTable[i].Dispose();

            _fontTable.Clear();
        }

        public ISpriteRenderer CreateSpriteRenderer(Action<SpriteBatcher> callback = null)
        {
            return new SpriteRendererDX11(_renderer.Device, callback);
        }

        IMesh<GBufferVertex> IResourceManager.CreateMesh(int maxVertices, VertexTopology topology, bool dynamic)
        {
            return new StandardMesh(_renderer, maxVertices, topology, dynamic);
        }

        public IIndexedMesh<GBufferVertex> CreateIndexedMesh(int maxVertices,
            int maxIndices, 
            VertexTopology topology = VertexTopology.TriangleList, 
            bool dynamic = false)
        {
            return new StandardIndexedMesh(_renderer, maxVertices, maxIndices, topology, IndexBufferFormat.Unsigned32Bit, dynamic);
        }

        public IMesh<T> CreateMesh<T>(int maxVertices, VertexTopology topology = VertexTopology.TriangleList, bool dynamic = false) 
            where T : struct, IVertexType
        {
            return new Mesh<T>(_renderer, maxVertices, topology, dynamic);
        }

        public IIndexedMesh<T> CreateIndexedMesh<T>(
            int maxVertices, 
            int maxIndices, 
            VertexTopology topology = VertexTopology.TriangleList, 
            IndexBufferFormat indexFormat = IndexBufferFormat.Unsigned32Bit, 
            bool dynamic = false)
            where T : struct, IVertexType
        {
            return new IndexedMesh<T>(_renderer, maxVertices, maxIndices, topology, indexFormat, dynamic);
        }
    }
}
