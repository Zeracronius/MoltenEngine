﻿namespace Molten.Graphics.Textures
{
    internal abstract class BCContext
    {
        internal float[] fDir = new float[4];

        // Calculate new steps
        internal Color4[] pSteps = new Color4[4];
    }
}
