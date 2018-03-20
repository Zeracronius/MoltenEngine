﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Font
{
    public class ClassSetTable : FontSubTable
    {
        /// <summary>
        /// Gets an array of <see cref="ClassRuleTable"/>, ordered by preference.
        /// </summary>
        public ClassRuleTable[] Tables { get; internal set; }

        internal override void Read(EnhancedBinaryReader reader, FontReaderContext context, FontTable parent)
        {
            ushort posClassRuleCount = reader.ReadUInt16();
            ushort[] posClassRuleOffsets = reader.ReadArray<ushort>(posClassRuleCount);
            Tables = new ClassRuleTable[posClassRuleCount];
            for (int i = 0; i < posClassRuleCount; i++)
                Tables[i] = context.ReadSubTable<ClassRuleTable>(posClassRuleOffsets[i]);
        }
    }

    /// <summary>
    /// See for PosClassRuleSet: https://docs.microsoft.com/en-us/typography/opentype/spec/gpos#context-positioning-subtable-format-1-simple-glyph-contexts
    /// See for SubClassRuleSet: https://docs.microsoft.com/en-us/typography/opentype/spec/gsub#lookuptype-5-contextual-substitution-subtable
    /// </summary>
    public class ClassRuleTable : FontSubTable
    {
        /// <summary>
        /// Gets an array of classes to be matched to the input glyph sequence, beginning with the second glyph position.
        /// </summary>
        public ushort[] Classes { get; internal set; }

        /// <summary>
        /// Gets an array of positioning lookups, in design order.
        /// </summary>
        public RuleLookupRecord[] Records { get; internal set; }

        internal override void Read(EnhancedBinaryReader reader, FontReaderContext context, FontTable parent)
        {
            ushort glyphCount = reader.ReadUInt16();
            ushort posCount = reader.ReadUInt16();
            Classes = reader.ReadArray<ushort>(glyphCount - 1);
            Records = new RuleLookupRecord[posCount];
            for (int i = 0; i < posCount; i++)
            {
                Records[i] = new RuleLookupRecord(
                    seqIndex: reader.ReadUInt16(),
                    lookupIndex: reader.ReadUInt16()
                );
            }
        }
    }
}
