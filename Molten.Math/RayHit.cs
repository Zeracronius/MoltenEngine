﻿using Molten;

namespace Molten
{
    ///<summary>
    /// Contains ray hit data.
    ///</summary>
    public struct RayHit
    {
        ///<summary>
        /// Location of the ray hit.
        ///</summary>
        public Vector3F Location;
        ///<summary>
        /// Normal of the ray hit.
        ///</summary>
        public Vector3F Normal;

        ///<summary>
        /// T parameter of the ray hit.  
        /// The ray hit location is equal to the ray origin added to the ray direction multiplied by T.
        ///</summary>
        public float T;
    }
}
