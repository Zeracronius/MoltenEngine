﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Molten.Graphics
{
    /// <summary>An entry-point tag parser used by <see cref="ComputeTask"/> headers.</summary>
    internal class ShaderPSParser : ShaderNodeParser
    {
        internal override string[] SupportedNodes => new string[] { "fragment" };

        internal override NodeParseResult Parse(HlslFoundation foundation, ShaderCompilerContext context, XmlNode node)
        {
            switch (foundation)
            {
                case Material material:
                    material.DefaultPSEntryPoint = node.InnerText;
                    return new NodeParseResult(NodeParseResultType.Success);
                case HlslPass pass:
                    pass.PixelShader.EntryPoint = node.InnerText;
                    return new NodeParseResult(NodeParseResultType.Success);
            }

            return new NodeParseResult(NodeParseResultType.Ignored);
        }
    }
}
