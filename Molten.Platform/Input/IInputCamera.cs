﻿using Molten.Graphics;

namespace Molten.Input
{
    public delegate void InputCameraSurfaceHandler(IInputCamera camera, IRenderSurface surface);

    /// <summary>
    /// Represents an implementation of a camera through which user input is received.
    /// </summary>
    public interface IInputCamera
    {
        event InputCameraSurfaceHandler OnSurfaceChanged;

        IRenderSurface OutputSurface { get; }
    }
}
