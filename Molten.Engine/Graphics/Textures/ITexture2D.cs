﻿namespace Molten.Graphics
{
    public interface ITexture2D : ITexture
    {
        void Resize(uint newWidth, uint newHeight, uint newMipMapCount);

        /// <summary>
        /// Gets a new instance of the texture's 2D properties.
        /// </summary>
        /// <returns></returns>
        Texture2DProperties Get2DProperties();

        /// <summary>
        /// Changes the current texture's dimensions and format.
        /// </summary>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        /// <param name="newMipMapCount">The new mip-map level count.</param>
        /// <param name="newArraySize">The new array size. Anything greater than 1 will convert the texture into a texture array. Texture arrays can be treated as standard 2D texture.</param>
        /// <param name="newFormat">The new graphics format.</param>
        void Resize(uint newWidth, uint newHeight, uint newMipMapCount, uint newArraySize, GraphicsFormat newFormat);

        /// <summary>
        /// Changes the current texture's width and height.
        /// </summary>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        void Resize(uint newWidth, uint newHeight);

        /// <summary>Gets the height of the texture.</summary>
        uint Height { get; }
    }
}
