﻿using System;

namespace Molten.Graphics
{
    [AttributeUsage(AttributeTargets.Field)]
    public class VertexElementAttribute : Attribute
    {
        public VertexElementAttribute(VertexElementType type, VertexElementUsage usage, int semanticIndex,
            VertexInputType classification = VertexInputType.PerVertexData)
        {
            Type = type;
            Usage = usage;
            SemanticIndex = semanticIndex;
            Classification = classification;
        }

        /// <summary>The element type.</summary>
        public VertexElementType Type;

        /// <summary>Gets the vertex element usage.</summary>
        public VertexElementUsage Usage;

        /// <summary>Gets or sets the semantic slot of the element (e.g. usage as a position with slot 0 would create SV_POSITION0 in hlsl).</summary>
        public int SemanticIndex;

        /// <summary>Gets the data classification of the element.</summary>
        public VertexInputType Classification;
    }
}
