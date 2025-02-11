﻿namespace Molten.Graphics.Overlays
{
    public interface IRenderOverlay
    {
        void OnRender(Timing time, SpriteBatcher sb, SpriteFont font, RenderProfiler rendererProfiler, RenderProfiler sceneProfiler, RenderCamera camera);

        /// <summary>
        /// Gets the title of the debug overlay. This must be unique when added to a <see cref="Molten.Graphics.OverlayProvider"/>.
        /// </summary>
        string Title { get; }
    }
}
