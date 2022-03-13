﻿using Molten.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Molten.Graphics
{
    /// <summary>A shader matrix variable.</summary>
    internal unsafe class ScalarFloat4x4Variable : ShaderConstantVariable
    {
        Matrix4F _value;

        public ScalarFloat4x4Variable(ShaderConstantBuffer parent)
            : base(parent)
        {
            SizeOf = (uint)sizeof(Matrix4F);
        }

        public override void Dispose() { }

        internal override void Write(byte* pDest)
        {
            ((Matrix4F*)pDest)[0] = _value;
        }

        public override object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = (Matrix4F)value;
                _value.Transpose();
                DirtyParent();
            }
        }
    }
}
