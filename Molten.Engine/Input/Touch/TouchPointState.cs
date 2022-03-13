﻿using System;

namespace Molten.Input
{
    public struct TouchPointState : IInputState
    {
        /// <summary>
        /// An empty <see cref="TouchPointState"/>.
        /// </summary>
        public static readonly TouchPointState Empty = new TouchPointState();

        /// <summary>
        /// Gets the screen position of the touch point.
        /// </summary>
        public Vector2F Position;

        /// <summary>
        /// The amount that the touch point has moved since it's last state update.
        /// </summary>
        public Vector2F Delta;

        /// <summary>
        /// The state of the touch point.
        /// </summary>
        public InputAction State;

        /// <summary>
        /// The touch point ID.
        /// </summary>
        public int ID;

        /// <summary>
        /// The orientation of the touch point, relative to the default upright orientation of the device.
        /// </summary>
        public float Orientation;

        /// <summary>
        /// The pressure applied to the touch point. 
        /// If the device does not support pressure-sensing, this value will always be 1.0f.
        /// </summary>
        public float Pressure;

        /// <summary>
        /// A normalized value that describes the approximate size of the pointer 
        /// touch area in relation to the maximum detectable size of the device.
        /// </summary>
        public float Size;

        DateTime _pressTimestamp;
        public DateTime PressTimestamp
        {
            get => _pressTimestamp;
            set => _pressTimestamp = value;
        }

        ulong _updateID;
        public ulong UpdateID
        {
            get => _updateID;
            set => _updateID = value;
        }
    }
}
