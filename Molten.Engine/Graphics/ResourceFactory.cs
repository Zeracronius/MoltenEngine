﻿using Molten.IO;
using System;
using System.Reflection;

namespace Molten.Graphics
{
    /// <summary>
    /// Represents an implementation of a renderer's resource factory.
    /// </summary>
    public abstract class ResourceFactory : EngineObject
    {
        RenderService _renderer;
        ShaderCompiler _compiler;


        public ResourceFactory(RenderService renderer, ShaderCompiler sCompiler)
        {
            _renderer = renderer;
            _compiler = sCompiler;
        }

        public IRenderSurface CreateSurface(Texture2DProperties properties)
        {
            return CreateSurface(properties.Width,
                properties.Height,
                properties.Format,
                properties.MipMapLevels,
                properties.ArraySize,
                properties.SampleCount,
                properties.Flags);
        }

        public abstract IRenderSurface CreateSurface(uint width, uint height, GraphicsFormat format = GraphicsFormat.R8G8B8A8_SNorm,
            uint mipCount = 1, uint arraySize = 1, uint sampleCount = 1, TextureFlags flags = TextureFlags.None);

        public abstract IDepthStencilSurface CreateDepthSurface(uint width, uint height, DepthFormat format = DepthFormat.R24G8_Typeless, uint mipCount = 1, uint arraySize = 1, uint sampleCount = 1,
            TextureFlags flags = TextureFlags.None);

        /// <summary>Creates a form with a surface which can be rendered on to.</summary>
        /// <param name="formTitle">The title of the form.</param>
        /// <param name="formName">The internal name of the form.</param>
        /// <param name="mipCount">The number of mip map levels of the form surface.</param>
        /// <param name="sampleCount">The number of samples. Anything greater than 1 will return a multi-sampled surface.</param>
        /// <returns></returns>
        public abstract INativeSurface CreateFormSurface(string formTitle, string formName, uint mipCount = 1, uint sampleCount = 1);

        /// <summary>Creates a GUI control with a surface which can be rendered on to.</summary>
        /// <param name="controlTitle">The title of the form.</param>
        /// <param name="controlName">The internal name of the control.</param>
        /// <param name="mipCount">The number of mip map levels of the form surface.</param>
        /// <param name="sampleCount">The number of samples. Anything greater than 1 will return a multi-sampled surface.</param>
        /// <returns></returns>
        public abstract INativeSurface CreateControlSurface(string controlTitle, string controlName, uint mipCount = 1, uint sampleCount = 1);

        /// <summary>Creates a new 1D texture and returns it.</summary>
        /// <param name="properties">A set of 1D texture properties.</param>
        public abstract ITexture CreateTexture1D(Texture1DProperties properties);

        /// <summary>Creates a new 1D texture and returns it.</summary>
        /// <param name="data">The data from which to create the texture.</param>
        public abstract ITexture CreateTexture1D(TextureData data);

        /// <summary>Creates a new 2D texture and returns it.</summary>
        /// <param name="properties">A set of 2D texture properties.</param>
        public abstract ITexture2D CreateTexture2D(Texture2DProperties properties);

        /// <summary>Creates a new 2D texture and returns it.</summary>
        /// <param name="data">The data from which to create the texture.</param>
        public abstract ITexture2D CreateTexture2D(TextureData data);

        /// <summary>Creates a new cube texture (cube-map) and returns it.</summary>
        /// <param name="properties">A set of 2D texture properties.</param>
        public abstract ITextureCube CreateTextureCube(Texture2DProperties properties);

        /// <summary>Creates a new cube texture (cube-map) and returns it.</summary>
        /// <param name="data">The data from which to create the texture.</param>
        public abstract ITextureCube CreateTextureCube(TextureData data);

        /// <summary>
        /// Resolves a source texture into a destination texture. <para/>
        /// This is most useful when re-using the resulting rendertarget of one render pass as an input to a second render pass. <para/>
        /// Another common use is transferring (resolving) a multisampled texture into a non-multisampled texture.
        /// </summary>
        /// <param name="source">The source texture.</param>
        /// <param name="destination">The destination texture.</param>
        public abstract void ResolveTexture(ITexture source, ITexture destination);

        /// <summary>Resources the specified sub-resource of a source texture into the sub-resource of a destination texture.</summary>
        /// <param name="source">The source texture.</param>
        /// <param name="destination">The destination texture.</param>
        /// <param name="sourceMipLevel">The source mip-map level.</param>
        /// <param name="sourceArraySlice">The source array slice.</param>
        /// <param name="destMiplevel">The destination mip-map level.</param>
        /// <param name="destArraySlice">The destination array slice.</param>
        public abstract void ResolveTexture(ITexture source, ITexture destination, uint sourceMipLevel, uint sourceArraySlice, uint destMiplevel, uint destArraySlice);

        /// <summary>
        /// Creates a renderer for drawing sprites and primitives with a <see cref="SpriteBatcher"/> via the provided callback.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public abstract ISpriteRenderer CreateSpriteRenderer(Action<SpriteBatcher> callback = null);

        /// <summary>
        /// Creates a standard mesh. Standard meshes enforce stricter rules aimed at deferred rendering.
        /// </summary>
        /// <param name="maxVertices"></param>
        /// <param name="topology"></param>
        /// <param name="dynamic"></param>
        /// <returns></returns>
        public abstract IMesh<GBufferVertex> CreateMesh(uint maxVertices,
            VertexTopology topology = VertexTopology.TriangleList,
            bool dynamic = false);

        /// <summary>
        /// Creates the indexed mesh.
        /// </summary>
        /// <param name="dynamic">if set to <c>true</c> [dynamic].</param>
        /// <param name="dedicatedResource">if set to <c>true</c> [dedicated resource].</param>
        /// <returns></returns>
        public abstract IIndexedMesh<GBufferVertex> CreateIndexedMesh(uint maxVertices, uint maxIndices,
            VertexTopology topology = VertexTopology.TriangleList,
            bool dynamic = false);

        /// <summary>Creates a new unindexed mesh. Unindexed meshes do not contain an index buffer to reduce vertex data size.</summary>
        /// <param name="dynamic">if set to <c>true</c> [dynamic].</param>
        /// <param name="dedicatedResource">if set to <c>true</c>, the mesh is given its own dedicated resource buffer.</param>
        /// <returns></returns>
        public abstract IMesh<T> CreateMesh<T>(
            uint maxVertices,
            VertexTopology topology = VertexTopology.TriangleList,
            bool dynamic = false)
            where T : unmanaged, IVertexType;

        /// <summary>
        /// Creates the indexed mesh.
        /// </summary>
        /// <param name="dynamic">if set to <c>true</c> [dynamic].</param>
        /// <param name="dedicatedResource">if set to <c>true</c> [dedicated resource].</param>
        /// <returns></returns>
        public abstract IIndexedMesh<T> CreateIndexedMesh<T>(uint maxVertices, uint maxIndices,
            VertexTopology topology = VertexTopology.TriangleList,
            IndexBufferFormat indexFormat = IndexBufferFormat.Unsigned32Bit,
            bool dynamic = false)
            where T : unmanaged, IVertexType;

        /// <summary>
        /// Loads an embedded shader from the target assembly. If an assembly is not provided, the current renderer's assembly is used instead.
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="filename"></param>
        /// <param name="assembly">The assembly that contains the embedded shadr. If an assembly is not provided, the current renderer's assembly is used instead.</param>
        /// <returns></returns>
        public ShaderCompileResult LoadEmbeddedShader(string nameSpace, string filename, Assembly assembly = null)
        {
            string src = "";
            assembly = assembly ?? _renderer.GetType().Assembly;
            Stream stream = EmbeddedResource.TryGetStream($"{nameSpace}.{filename}", assembly);
            if(stream != null)
            {
                using (StreamReader reader = new StreamReader(stream))
                    src = reader.ReadToEnd();

                stream.Dispose();
            }
            else
            {
                _renderer.Log.Error($"Attempt to load embedded shader failed: '{filename}' not found in namespace '{nameSpace}' of assembly '{assembly.FullName}'");
                return new ShaderCompileResult();
            }

            return _compiler.CompileShader(in src, filename, ShaderCompileFlags.EmbeddedFile, assembly, nameSpace);
        }

        /// <summary>Compiles a set of shaders from the provided source string.</summary>
        /// <param name="source">The source code to be parsed and compiled.</param>
        /// <param name="filename">The name of the source file. Used as a pouint of reference in debug/error messages only.</param>
        /// <returns></returns>
        public ShaderCompileResult CompileShaders(ref string source, string filename = null)
        {
            ShaderCompileFlags flags = ShaderCompileFlags.EmbeddedFile;

            if (!string.IsNullOrWhiteSpace(filename))
            {
                FileInfo fInfo = new FileInfo(filename);
                DirectoryInfo dir = fInfo.Directory;
                flags = ShaderCompileFlags.None;
            }

            return _compiler.CompileShader(in source, filename, flags, null, null);
        }
    }
}
