﻿using Molten.IO;

namespace Molten.Font
{
    public class ScriptListTable : FontSubTable
    {
        public ScriptRecord[] Records { get; internal set; }

        internal ScriptListTable(EnhancedBinaryReader reader, Logger log, IFontTable parent, long offset) :
            base(reader, log, parent, offset)
        {
            ushort scriptCount = reader.ReadUInt16();
            Records = new ScriptRecord[scriptCount];

            ushort[] scriptTableOffsets = new ushort[scriptCount];
            for (int i = 0; i < scriptCount; i++)
            {
                Records[i] = new ScriptRecord()
                {
                    Tag = FontUtil.ReadTag(reader),
                };

                scriptTableOffsets[i] = reader.ReadUInt16();
            }

            for (int i = 0; i < scriptCount; i++)
                Records[i].Table = new ScriptTable(reader, log, this, scriptTableOffsets[i]);
        }
    }

    public class ScriptRecord
    {
        /// <summary>
        /// Gets a tag identifier which, 4 characters in length.
        /// </summary>
        public string Tag { get; internal set; }

        public ScriptTable Table { get; internal set; }
    }

    public class ScriptTable : FontSubTable
    {
        public LangSysRecord[] Records { get; internal set; }

        public LangSysTable Default { get; internal set; }

        internal ScriptTable(EnhancedBinaryReader reader, Logger log, IFontTable parent, long offset) :
            base(reader, log, parent, offset)
        {
            ushort defaultLangSys = reader.ReadUInt16();
            ushort langSysCount = reader.ReadUInt16();

            ushort[] langSysOffsets = new ushort[langSysCount];
            Records = new LangSysRecord[langSysCount];
            for (int i = 0; i < langSysCount; i++)
            {
                Records[i] = new LangSysRecord()
                {
                    Tag = FontUtil.ReadTag(reader),
                };

                langSysOffsets[i] = reader.ReadUInt16();
            }

            Default = new LangSysTable(reader, log, this, defaultLangSys);

            for (int i = 0; i < langSysCount; i++)
                Records[i].Table = new LangSysTable(reader, log, this, langSysOffsets[i]);
        }
    }

    public class LangSysRecord
    {
        /// <summary>
        /// Gets a tag identifier which, 4 characters in length.
        /// </summary>
        public string Tag { get; internal set; }

        /// <summary>
        /// The <see cref="LangSysTable"/> associated with this record.
        /// </summary>
        public LangSysTable Table { get; internal set; }
    }

    public class LangSysTable : FontSubTable
    {
        /// <summary>
        /// Gets the lookup order. Usually reserved for an offset to a reordering table, so will be equal to <see cref="FontUtil.NULL"/>.
        /// </summary>
        public ushort LookupOrder { get; internal set; }

        /// <summary>Gets an array of indices into the FeatureList, in arbitrary order.</summary>
        public ushort[] FeatureIndices { get; internal set; }

        /// <summary>
        /// Gets the index of a feature required for this language system; if no required features = 0xFFFF (65535)
        /// </summary>
        public ushort RequiredFeatureIndex { get; internal set; }

        internal LangSysTable(EnhancedBinaryReader reader, Logger log, IFontTable parent, long offset) :
            base(reader, log, parent, offset)
        {
            LookupOrder = reader.ReadUInt16();
            RequiredFeatureIndex = reader.ReadUInt16();
            ushort featureIndexCount = reader.ReadUInt16();
            FeatureIndices = reader.ReadArray<ushort>(featureIndexCount);
        }
    }
}
