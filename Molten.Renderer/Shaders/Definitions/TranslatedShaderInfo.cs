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
        public ShaderEntryPoint Vertex;

        public ShaderEntryPoint Fragment;

        public ShaderEntryPoint Geometry;

        public ShaderEntryPoint Hull;

        public ShaderEntryPoint Domain;

        public ShaderEntryPoint Compute;
    }
}
