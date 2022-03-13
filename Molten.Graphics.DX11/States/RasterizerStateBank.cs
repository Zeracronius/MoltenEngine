﻿using Silk.NET.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal class RasterizerStateBank : GraphicsStateBank<GraphicsRasterizerState, RasterizerPreset>
    {
        internal RasterizerStateBank(Device device)
        {
            AddPreset(RasterizerPreset.Default, new GraphicsRasterizerState(device));

            //wireframe preset.
             AddPreset(RasterizerPreset.Wireframe, new GraphicsRasterizerState(device)
            {
                FillMode = FillMode.FillWireframe,
            });

            //scissor test preset
             AddPreset(RasterizerPreset.ScissorTest, new GraphicsRasterizerState(device)
            {
                IsScissorEnabled = true,
            });

            //no culling preset.
             AddPreset(RasterizerPreset.NoCulling, new GraphicsRasterizerState(device)
            {
                CullMode = CullMode.CullNone,
            });

             AddPreset(RasterizerPreset.DefaultMultisample, new GraphicsRasterizerState(device)
            {
                IsMultisampleEnabled = true,
            });

             AddPreset(RasterizerPreset.ScissorTestMultisample, new GraphicsRasterizerState(device)
            {
                IsScissorEnabled = true,
                IsMultisampleEnabled = true,
            });
        }

        internal override GraphicsRasterizerState GetPreset(RasterizerPreset value)
        {
            return _presets[(int)value];
        }
    }

    /// <summary>Represents several rasterizer state presets.</summary>
    public enum RasterizerPreset
    {
        /// <summary>The default rasterizer state.</summary>
        Default = 0,

        /// <summary>The same as the default rasterizer state, but with wireframe enabled.</summary>
        Wireframe = 1,

        /// <summary>The same as the default rasterizer state, but with scissor testing enabled.</summary>
        ScissorTest = 2,

        /// <summary>Culling is disabled. Back and front faces will be drawn.</summary>
        NoCulling = 3,

        /// <summary>
        /// The same as <see cref="Default"/> but with multisampling enabled.
        /// </summary>
        DefaultMultisample = 4,

        /// <summary>
        /// The same as <see cref="ScissorTest"/> but with multisampling enabled.
        /// </summary>
        ScissorTestMultisample = 5,
    }
}
