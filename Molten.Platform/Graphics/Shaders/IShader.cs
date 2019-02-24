﻿using System;
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
    }
}
