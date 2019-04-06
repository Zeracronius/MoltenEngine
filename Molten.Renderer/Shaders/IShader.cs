using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public interface IShader : IDisposable
    {
        /// <summary>Gets the name of the material.</summary>
        string Name { get; }

        /// <summary>Gets the description of the material.</summary>
        string Description { get; }

        /// <summary>Gets the author of the material.</summary>
        string Author { get; }

        string Filename { get; }

        Dictionary<string, string> Metadata { get; }

        /// <summary>Gets or sets a material value.</summary>
        /// <param name="key">The value key</param>
        /// <returns></returns>
        IShaderValue this[string key] { get; set; }

        /// <summary>
        /// Gets or sets the tag object.
        /// </summary>
        object Tag { get; set; }

        /// <summary>
        /// Gets the sort key assigned to the current <see cref="IShader"/>.
        /// </summary>
        int SortKey { get; }

        /// <summary>Gets a pass at the specified index.</summary>
        /// <param name="index">The identifier.</param>
        /// <returns></returns>
        IShaderPass GetPass(int index);

        /// <summary>Gets a pass with the specified name.</summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        IShaderPass GetPass(string name);

        /// <summary>
        /// sets the default resource to be used when an object does not provide it's own resource for a particular slot.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="slot">The slot number.</param>
        void SetDefaultResource(IShaderResource resource, int slot);

        IShaderResource GetDefaultResource(int slot);

        /// <summary>Gets the number of passes in the material.</summary>
        /// <value>
        /// The pass count.
        /// </value>
        int PassCount { get; }

        ObjectMaterialProperties Object { get; }

        LightMaterialProperties Light { get; }

        SceneMaterialProperties Scene { get; }

        GBufferTextureProperties Textures { get; }

        SpriteBatchMaterialProperties SpriteBatch { get; }
    }
}
