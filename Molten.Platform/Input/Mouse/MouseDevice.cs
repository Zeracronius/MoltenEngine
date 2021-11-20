﻿namespace Molten.Input
{
    public delegate void MouseEventHandler(MouseDevice mouse);

    /// <summary>
    /// Represents an implementation of a mouse or pointer device.
    /// </summary>
    public abstract class MouseDevice : InputDevice<MouseButtonState, MouseButton>
    {
        public MouseDevice(IInputManager manager, Logger log) : 
            base(manager, manager.Settings.MouseBufferSize, log)
        {

        }

        /// <summary>
        /// Occurs when the mouse cursor was inside it's bound surface, but just left it.
        /// </summary>
        public abstract event MouseEventHandler OnLeaveSurface;

        /// <summary>
        /// Occurs when the mouse cursor was outside of it's bound surface, but just entered it.
        /// </summary>
        public abstract event MouseEventHandler OnEnterSurface;

        /// <summary>Positions the mouse cursor at the center of the currently-bound <see cref="IInputCamera"/>.</summary>
        public abstract void CenterInView();

        /// <summary>Returns the amount the mouse cursor has moved a long X and Y since the last frame/update.</summary>
        public abstract Vector2I Delta { get; }

        /// <summary>Gets the amount the mouse wheel has been moved since the last frame.</summary>
        public abstract float WheelDelta { get; }

        /// <summary>Gets the current scroll wheel position.</summary>
        public abstract float WheelPosition { get; }

        /// <summary>
        /// Gets the position of the mouse cursor.
        /// </summary>
        public abstract Vector2I Position { get; }

        /// <summary>Gets or sets whether or not the mouse cursor is visible.</summary>
        public abstract bool CursorVisible { get; set; }

        /// <summary>Gets or sets whether or not the mouse is contrained to the bounds of the main output.</summary>
        public abstract bool IsConstrained { get; set; }
    }
}