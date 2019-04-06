using SharpShader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public class ShaderEntryPoint
    {
        public readonly ShaderTranslationResult Result;

        public readonly EntryPointInfo Info;

        public string FinalSource;

        public string Error;

        internal ShaderEntryPoint(ShaderTranslationResult result, EntryPointInfo ep)
        {
            Result = result;
            Info = ep;
            FinalSource = result.SourceCode;
        }
    }
}
