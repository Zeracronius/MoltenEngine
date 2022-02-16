﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Molten.Graphics
{
    internal class ShaderPassParser : ShaderNodeParser
    {
        internal override string[] SupportedNodes => new string[] { "pass" };

        internal override NodeParseResult Parse(HlslFoundation shader, HlslCompilerContext context, XmlNode node)
        {
            if (shader is Material material)
            {
                MaterialPass pass = new MaterialPass(material, "<Unnamed Material Pass>");

                for (int i = 0; i < material.Samplers.Length; i++)
                    pass.Samplers[i] = material.Samplers[i];

                context.Parser.ParseNode(pass, node, context);
                material.AddPass(pass);

                return new NodeParseResult(NodeParseResultType.Success);
            }
            else
            {
                return new NodeParseResult(NodeParseResultType.Ignored);
            }
        }
    }
}