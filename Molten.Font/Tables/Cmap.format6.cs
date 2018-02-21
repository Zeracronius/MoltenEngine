﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Font
{
    public class CmapFormat6SubTable : Cmap.SubTable
    {
        ushort _startCode;
        ushort _endCode;
        ushort[] _glyphIdArray;

        public CmapFormat6SubTable(Cmap.EncodingRecord record) :
            base(record)
        { }

        public override ushort CharPairToGlyphIndex(int codepoint, ushort defaultGlyphIndex, int nextCodepoint)
        {
            return 0;
        }

        public override ushort CharToGlyphIndex(int codepoint)
        {
            /* The firstCode and entryCount values specify a subrange (beginning at firstCode,length = entryCount) within the range of possible character codes. 
             * Codes outside of this subrange are mapped to glyph index 0. 
             * The offset of the code (from the first code) within this subrange is used as index to the glyphIdArray, which provides the glyph index value.*/
            if (codepoint >= _startCode && codepoint <= _endCode)
                return _glyphIdArray[codepoint - _startCode];
            else
                return 0;
        }

        internal override void Read(BinaryEndianAgnosticReader reader, Logger log, TableHeader header)
        {
            ushort length = reader.ReadUInt16();
            Language = reader.ReadUInt16();
            _startCode = reader.ReadUInt16();
            ushort entryCount = reader.ReadUInt16();
            _endCode = (ushort)(_startCode + (entryCount-1));
            _glyphIdArray = reader.ReadArrayUInt16(entryCount);
        }
    }
}