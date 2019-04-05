using SharpShader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class TranslatedShaderInfo
    {
        public string Name { get; set; }

        public List<TranslatedPassInfo> Passes { get; set; } = new List<TranslatedPassInfo>();
    }

    public class TranslatedPassInfo
    {
        public Dictionary<EntryPointType, ShaderEntryPoint> EntryPoints = new Dictionary<EntryPointType, ShaderEntryPoint>();

        public void AddEntryPoint(EntryPointType epType, ShaderEntryPoint ep)
        {
            if (ep != null)
                EntryPoints.Add(epType, ep);
        }
    }
}
