﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    /// <summary>
    /// See for info: https://github.com/microsoft/DirectXShaderCompiler/blob/dc7789738c51994559424c67629acc90f4ba69ad/include/dxc/dxcapi.h#L135
    /// </summary>
    public enum DxcCompilerArg
    {
        None = 0,

        DebugNameForBinary = 1,

        DebugNameForSource = 2,

        AllResourcesBound = 3,

        ResourcesMayAlias = 4,

        WarningsAreErrors = 5,

        OptimizationLevel3 = 6,

        OptimizationLevel2 = 7,

        OptimizationLevel1 = 8,

        OptimizationLevel0 = 9,

        IeeeStrictness = 10,

        EnableBackwardsCompatibility = 11,

        EnableStrictness = 12,

        PreferFlowControl = 13,

        AvoidFlowControl = 14,

        PackMatrixRowMajor = 15,

        PackMatrixColumnMajor = 16,

        Debug = 17,

        SkipValidation = 18,

        SkipOptimizations = 19,

        EntryPoint = 20,

        TargetProfile = 21,

        NoLogo = 22,

        /// <summary>
        /// Don't emit warnings for unused driver arguments
        /// </summary>
        IgnoreUnusedArgs = 23,

        OutputAssemblyFile = 24,

        OutputDebugFile = 25,

        OutputErrorFile = 26,

        OutputHeaderFile = 27,

        OutputObjectFile = 28,

        /// <summary>
        /// Output hexadecimal literals
        /// </summary>
        OutputHexLiterals = 29,

        /// <summary>
        /// Output instruction numbers in assembly listings
        /// </summary>
        OutputInstructionNumbers = 30,

        NoWarnings = 31,

        /// <summary>
        /// Output instruction byte offsets in assembly listings
        /// </summary>
        OutputInstructionOffsets = 32,

        StripDebug = 33,

        StripPrivate = 34,

        StripReflection = 35,

        StripRootSignature = 36,

        /// <summary>
        /// Send pre-processing results to file. This argument must be used alone.
        /// </summary>
        PreProcessToFile = 37,
    }
}
