﻿namespace Molten.Graphics
{
    /// <summary>Represents the mode to apply when setting a depth surface.</summary>
    public enum GraphicsDepthWritePermission
    {
        /// <summary>The depth buffer can be read and written.</summary>
        Enabled = 0,

        /// <summary>The depth buffer is disabled (cannot be read from or written to).</summary>
        Disabled = 1,

        /// <summary>The depth buffer cannot be written to, only read from.</summary>
        ReadOnly = 2,
    }
}
