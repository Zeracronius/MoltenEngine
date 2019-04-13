using SharpShader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class TranslatedPassInfo
    {
        public Dictionary<EntryPointType, ShaderEntryPoint> EntryPoints { get; } = new Dictionary<EntryPointType, ShaderEntryPoint>();

        public ShaderPassDefinition Definition { get; }

        internal TranslatedPassInfo(ShaderPassDefinition def)
        {
            Definition = def;
        }

        public void AddEntryPoint(EntryPointType epType, ShaderEntryPoint ep)
        {
            if (ep != null)
                EntryPoints.Add(epType, ep);
        }
    }
}
