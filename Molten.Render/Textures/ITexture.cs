﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public delegate void TextureHandler(ITexture texture);

    /// <summary>Represents a 1D texture, while also acting as the base for all other texture implementations.</summary>
    /// <seealso cref="System.IDisposable" />
    public interface ITexture : IShaderResource
    {
        event TextureHandler OnPreResize;

        event TextureHandler OnPostResize;

        void Resize(int newWidth);

        /// <summary>Generates any missing mip-maps for a texture, so long as it's creation flags included <see cref="TextureFlags.AllowMipMapGeneration"/>.</summary>
        void GenerateMipMaps();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="srcMipIndex"></param>
        /// <param name="srcArraySlice"></param>
        /// <param name="mipCount"></param>
        /// <param name="arrayCount"></param>
        /// <param name="destMipIndex"></param>
        /// <param name="destArraySlice"></param>
        void SetData(TextureData data, int srcMipIndex, int srcArraySlice, int mipCount, int arrayCount, int destMipIndex = 0, int destArraySlice = 0);

        /// <summary>Copies the provided data into the texture.</summary>
        /// <param name="data">The data to copy to the texture.</param>
        /// <param name="level">The mip-map level to copy the data to.</param>
        /// <param name="count">The number of elements to copy from the provided data array.</param>
        /// <param name="mipIndex">The index at which to start copying from the provided data array.</param>
        /// <param name="arraySlice">The position in the texture array to start copying the texture data to. For a non-array texture, this should be 0.</param>
        void SetData<T>(int level, T[] data, int startIndex, int count, int pitch, int arraySlice = 0) where T : struct;

        /// <summary>Copies the provided data into the texture.</summary>
        /// <param name="data">The slice data to copy to the texture.</param>
        /// <param name="mipCount">The number of mip maps to copy from the source data.</param>
        /// <param name="mipLevel">The mip-map level at which to start copying to within the texture.</param>
        /// <param name="arraySlice">The position in the texture array to start copying the texture data to. For a non-array texture, this should be 0.</param>
        void SetData(TextureData.Slice data, int mipLevel, int arraySlice);

        /// <summary>Returns the data contained within a texture via a staging texture or directly from the texture itself if possible.</summary>
        /// <param name="stagingTexture">A staging texture to use when retrieving data from the GPU. Only textures
        /// with the staging flag set will work.</param>
        TextureData GetData(ITexture stagingTexture);

        /// <summary>Returns the data from a single mip-map level within a slice of the texture. For 2D, non-array textures, this will always be slice 0.</summary>
        /// <param name="stagingTexture">The staging texture to copy the data to, from the GPU.</param>
        /// <param name="level">The mip-map level to retrieve.</param>
        /// <param name="arraySlice">The array slice to access.</param>
        TextureData.Slice GetData(ITexture stagingTexture, int level, int arraySlice);

        /// <summary>Gets the flags that were passed in when the texture was created.</summary>
        TextureFlags Flags { get; }

        /// <summary>Gets the format of the texture.</summary>
        GraphicsFormat Format { get; }

        /// <summary>Gets whether or not the texture is using a supported block-compressed format.</summary>
        bool IsBlockCompressed { get; }

        /// <summary>Gets the width of the texture.</summary>
        int Width { get; }

        /// <summary>Gets the number of mip map levels in the texture.</summary>
        int MipMapLevels { get; }

        /// <summary>Gets the number of array slices in the texture.</summary>
        int ArraySize { get; }

        /// <summary>Gets whether or not the texture is a texture array.</summary>
        bool IsTextureArray { get; }
    }
}