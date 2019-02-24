﻿using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Molten.Graphics.Textures.DDS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class TextureCubeDX11 : TextureBase, ITextureCube
    {
        Texture2D _texture;
        Texture2DDescription _description;
        int _cubeCount;

        internal TextureCubeDX11(RendererDX11 renderer, int width,
            int height, Format format = SharpDX.DXGI.Format.R8G8B8A8_UNorm, int mipCount = 1, int cubeCount = 1, TextureFlags flags = TextureFlags.None)
            : base(renderer, width, height, 1, mipCount, 6, 1, format, flags)
        {
            _cubeCount = cubeCount;
            _description = new Texture2DDescription()
            {
                Width = width,
                Height = height,
                MipLevels = mipCount,
                ArraySize = 6 * _cubeCount,
                Format = format,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = GetAccessFlags(),
                SampleDescription = new SampleDescription()
                {
                    Count = 1,
                    Quality = 0,
                },
                Usage = GetUsageFlags(),
                OptionFlags = GetResourceFlags() | ResourceOptionFlags.TextureCube,
            };
        }

        protected override void SetSRVDescription(ref ShaderResourceViewDescription desc)
        {
            desc.Format = _format;
            desc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.TextureCubeArray;
            desc.TextureCubeArray = new ShaderResourceViewDescription.TextureCubeArrayResource()
            {
                MostDetailedMip = 0,
                MipLevels = _description.MipLevels,
                CubeCount = _cubeCount,
                First2DArrayFace = 0,
            };
        }

        protected override void SetUAVDescription(ShaderResourceViewDescription srvDesc, ref UnorderedAccessViewDescription desc)
        {
            desc.Format = SRV.Description.Format;
            desc.Dimension = UnorderedAccessViewDimension.Texture2DArray;

            desc.Texture2DArray = new UnorderedAccessViewDescription.Texture2DArrayResource()
            {
                ArraySize = _description.ArraySize,
                FirstArraySlice = srvDesc.Texture2DArray.FirstArraySlice,
                MipSlice = 0,
            };

            desc.Buffer = new UnorderedAccessViewDescription.BufferResource()
            {
                FirstElement = 0,
                ElementCount = _description.Width * _description.Height * _description.ArraySize,
            };
        }

        protected override SharpDX.Direct3D11.Resource CreateResource(bool resize)
        {
            _texture = new Texture2D(Device.D3d, _description);
            return _texture;
        }

        protected override void UpdateDescription(int newWidth, int newHeight, int newDepth, int newMipMapCount, int newArraySize, Format newFormat)
        {
            _description.Width = newWidth;
            _description.Height = newHeight;
            _description.MipLevels = newMipMapCount;
            _description.Format = newFormat;
        }

        public void Resize(int newWidth, int newHeight, int newMipMapCount)
        {
            QueueChange(new TextureResize()
            {
                NewWidth = newWidth,
                NewHeight = newHeight,
                NewMipMapCount = newMipMapCount,
            });
        }

        public void Resize(int newWidth, int newMipMapCount)
        {
            QueueChange(new TextureResize()
            {
                NewWidth = newWidth,
                NewHeight = _height,
                NewMipMapCount = newMipMapCount,
            });
        }

        /// <summary>Gets the underlying DirectX Texture2D object.</summary>
        public Texture2D TextureResource
        {
            get { return _texture; }
        }

        /// <summary>Gets information about the texture.</summary>
        public Texture2DDescription Description { get { return _description; } }

        /// <summary>Gets the number of cube maps stored in the texture. This is greater than 1 if the texture is a cube-map array.</summary>
        public int CubeCount => _cubeCount;
    }
}
