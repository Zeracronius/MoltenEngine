﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Font
{
    /// <summary>A base implementation for GPOS and GSUB tables.</summary>
    public abstract class FontGTable : FontTable
    {
        public ushort MajorVersion { get; internal set; }

        public ushort MinorVersion { get; internal set; }

        //public ScriptList ScriptList { get; internal set; }

        //public FeatureList FeatureList { get; internal set; }

        public LookupListTable LookupTable { get; internal set; }

        internal abstract class Parser : FontTableParser
        {
            Type[] _lookupTypeIndex;

            internal Parser()
            {
                _lookupTypeIndex = GetLookupTypeIndex();
            }

            protected abstract Type[] GetLookupTypeIndex();

            protected abstract FontGTable CreateTable(BinaryEndianAgnosticReader reader, TableHeader header, Logger log, FontTableList dependenceies);

            internal override FontTable Parse(BinaryEndianAgnosticReader reader, TableHeader header, Logger log, FontTableList dependencies)
            {
                /* Certain structures are used across multiple GPOS Lookup subtable types and formats. All Lookup subtables use the Coverage table, 
                 * which is defined in the OpenType Layout Common Table Formats chapter. 
                 * Single and pair adjustments (LookupTypes 1 and 2) use a ValueRecord structure and associated ValueFormat enumeration, which are defined later in this chapter. 
                 * Attachment subtables (LookupTypes 3, 4, 5 and 6) use Anchor and MarkArray tables, also defined later in this chapter.*/

                FontGTable table = CreateTable(reader, header, log, dependencies);
                table.MajorVersion =  reader.ReadUInt16();
                table.MinorVersion = reader.ReadUInt16();
                ushort scriptListOffset = reader.ReadUInt16();
                ushort featureListOffset = reader.ReadUInt16();
                ushort lookupListOffset = reader.ReadUInt16();
                uint featureVariationsOffset = 0;

                // version 1.1
                if (table.MajorVersion == 1 && table.MinorVersion == 1)
                {
                    featureVariationsOffset = reader.ReadUInt32();
                    // TODO read variation list (if featureVariationOffset > 0 (NULL)
                }

                // TODO read script list

                // TODO read feature list.

                table.LookupTable = new LookupListTable(reader, log, header, _lookupTypeIndex, header.Offset + lookupListOffset);
                reader.Position = header.Offset + header.Length;
                return table;
            }
        }
    }
}
