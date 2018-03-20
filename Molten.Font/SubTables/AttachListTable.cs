﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Font
{
    public class AttachListTable : FontSubTable
    {
        /// <summary>Gets an array containing AttachPoint tables ordered by coverage index, which hold contour point indices.</summary>
        public AttachPointTable[] PointTables { get; private set; }

        /// <summary>
        /// Gets a <see cref="CoverageTable"/> containing glyph IDs.
        /// </summary>
        public CoverageTable Coverage { get; internal set; }

        internal override void Read(EnhancedBinaryReader reader, FontReaderContext context, FontTable parent)
        {
            ushort coverageOffset = reader.ReadUInt16();
            ushort glyphCount = reader.ReadUInt16();
            PointTables = new AttachPointTable[glyphCount];
            ushort[] attachPointOffsets = reader.ReadArray<ushort>(glyphCount);

            // prepare attach point tables with their respective offsets.
            for (int i = 0; i < glyphCount; i++)
                PointTables[i] = context.ReadSubTable<AttachPointTable>(attachPointOffsets[i]);

            // Read the coverage table.
            CoverageTable coverage = context.ReadSubTable<CoverageTable>(coverageOffset);
        }
    }
}
