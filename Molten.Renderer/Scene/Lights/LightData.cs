﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>
    /// A vertex structure for storing capsule light data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LightData : IVertexType
    {
        /// <summary>
        /// The light transform.
        /// </summary>
        public Matrix4F Transform;

        /// <summary>
        /// The light position.
        /// </summary>
        public Vector3F Position;

        /// <summary>
        /// The reciprocal light range.
        /// </summary>
        public float RangeRcp;

        /// <summary>
        /// The light color, as 3 floats.
        /// </summary>
        public Color3 Color3;

        /// <summary>
        /// The light intensity.
        /// </summary>
        public float Intensity;

        /// <summary>
        /// The forward direction of the capsule light.
        /// </summary>
        public Vector3F Forward;

        /// <summary>
        /// The tessellation factor. A Factor of 0 will disable the light.
        /// </summary>
        public float TessFactor;

        /// <summary>
        /// The length of the capsule length.
        /// </summary>
        public float Length;

        /// <summary>
        /// The half-length of the capsule light.
        /// </summary>
        public float HalfLength;
    }
}
