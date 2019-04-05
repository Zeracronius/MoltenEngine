using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public enum BlendFunc
    {
        //
        // Summary:
        //     The blend factor is (0, 0, 0, 0). No pre-blend operation.
        Zero = 1,

        //
        // Summary:
        //     The blend factor is (1, 1, 1, 1). No pre-blend operation.
        One = 2,

        //
        // Summary:
        //     The blend factor is (R?, G?, B?, A?), that is color data (RGB) from a pixel shader.
        //     No pre-blend operation.
        SourceColor = 3,

        //
        // Summary:
        //     The blend factor is (1 - R?, 1 - G?, 1 - B?, 1 - A?), that is color data (RGB)
        //     from a pixel shader. The pre-blend operation inverts the data, generating 1 -
        //     RGB.
        InverseSourceColor = 4,

        //
        // Summary:
        //     The blend factor is (A?, A?, A?, A?), that is alpha data (A) from a pixel shader.
        //     No pre-blend operation.
        SourceAlpha = 5,

        //
        // Summary:
        //     The blend factor is ( 1 - A?, 1 - A?, 1 - A?, 1 - A?), that is alpha data (A)
        //     from a pixel shader. The pre-blend operation inverts the data, generating 1 -
        //     A.
        InverseSourceAlpha = 6,

        //
        // Summary:
        //     The blend factor is (Ad Ad Ad Ad), that is alpha data from a render target. No
        //     pre-blend operation.
        DestinationAlpha = 7,

        //
        // Summary:
        //     The blend factor is (1 - Ad 1 - Ad 1 - Ad 1 - Ad), that is alpha data from a
        //     render target. The pre-blend operation inverts the data, generating 1 - A.
        InverseDestinationAlpha = 8,

        //
        // Summary:
        //     The blend factor is (Rd, Gd, Bd, Ad), that is color data from a render target.
        //     No pre-blend operation.
        DestinationColor = 9,

        //
        // Summary:
        //     The blend factor is (1 - Rd, 1 - Gd, 1 - Bd, 1 - Ad), that is color data from
        //     a render target. The pre-blend operation inverts the data, generating 1 - RGB.
        InverseDestinationColor = 10,

        //
        // Summary:
        //     The blend factor is (f, f, f, 1); where f = min(A?, 1 - Ad). The pre-blend operation
        //     clamps the data to 1 or less.
        SourceAlphaSaturate = 11,

        //
        // Summary:
        //     The blend factor is the blend factor set with ID3D11DeviceContext::OMSetBlendState.
        //     No pre-blend operation.
        BlendFactor = 14,

        //
        // Summary:
        //     The blend factor is the blend factor set with ID3D11DeviceContext::OMSetBlendState.
        //     The pre-blend operation inverts the blend factor, generating 1 - blend_factor.
        InverseBlendFactor = 15,

        //
        // Summary:
        //     The blend factor is data sources both as color data output by a pixel shader.
        //     There is no pre-blend operation. This blend factor supports dual-source color
        //     blending.
        SecondarySourceColor = 16,

        /// <summary>
        /// The blend factor is data sources both as color data output by a pixel shader. 
        /// The pre-blend operation inverts the data, generating 1 - RGB. This blend factor supports dual-source color blending.
        /// </summary>
        InverseSecondarySourceColor = 17,

        /// <summary>The blend factor is data sources as alpha data output by a pixel shader. 
        /// There is no pre-blend operation. This blend factor supports dual-source color blending. </summary>  
        SecondarySourceAlpha = 18,

        /// <summary>
        /// The blend factor is data sources as alpha data output by a pixel shader. 
        /// The pre-blend operation inverts the data, generating 1 - A. This blend factor supports 
        /// dual-source color blending.
        /// </summary>
        InverseSecondarySourceAlpha = 19
    }
}
