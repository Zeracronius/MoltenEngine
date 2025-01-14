﻿  
# Molten.Font.GPOS
Index-to-location table.<para /><para>The indexToLoc table stores the offsets to the locations of the glyphs in the font, relative to the beginning of the glyphData table. In order to compute the length of the last glyph element, there is an extra entry after the last valid index.</para><para>By definition, index zero points to the "missing character," which is the character that appears if a character is not found in the font. The missing character is commonly represented by a blank box or a space. If the font does not contain an outline for the missing character, then the first and second offsets should have the same value. This also applies to any other characters without an outline, such as the space character. If a glyph has no outline, then loca[n] = loca [n+1]. In the particular case of the last glyph(s), loca[n] will be equal the length of the glyph data ('glyf') table. The offsets must be in ascending order with loca[n] less-or-equal-to loca[n+1].</para>
            See: https://docs.microsoft.com/en-us/typography/opentype/spec/gpos 
  
*  [Class1Record](docs/Molten.Font/Molten/Font/GPOS/Class1Record.md)  
*  [Class2Record](docs/Molten.Font/Molten/Font/GPOS/Class2Record.md)  
    *  [Record1](docs/Molten.Font/Molten/Font/GPOS/Class2Record/Record1.md)  
    *  [Record2](docs/Molten.Font/Molten/Font/GPOS/Class2Record/Record2.md)  
*  [PairValueRecord](docs/Molten.Font/Molten/Font/GPOS/PairValueRecord.md)  
    *  [Record1](docs/Molten.Font/Molten/Font/GPOS/PairValueRecord/Record1.md)  
    *  [Record2](docs/Molten.Font/Molten/Font/GPOS/PairValueRecord/Record2.md)  
    *  [SecondGlyphID](docs/Molten.Font/Molten/Font/GPOS/PairValueRecord/SecondGlyphID.md)  
*  [ValueFormat](docs/Molten.Font/Molten/Font/GPOS/ValueFormat.md)  
    *  [Reserved](docs/Molten.Font/Molten/Font/GPOS/ValueFormat/Reserved.md)  
    *  [XAdvance](docs/Molten.Font/Molten/Font/GPOS/ValueFormat/XAdvance.md)  
    *  [XAdvanceDevice](docs/Molten.Font/Molten/Font/GPOS/ValueFormat/XAdvanceDevice.md)  
    *  [XPlacement](docs/Molten.Font/Molten/Font/GPOS/ValueFormat/XPlacement.md)  
    *  [XPlacementDevice](docs/Molten.Font/Molten/Font/GPOS/ValueFormat/XPlacementDevice.md)  
    *  [YAdvance](docs/Molten.Font/Molten/Font/GPOS/ValueFormat/YAdvance.md)  
    *  [YAdvanceDevice](docs/Molten.Font/Molten/Font/GPOS/ValueFormat/YAdvanceDevice.md)  
    *  [YPlacement](docs/Molten.Font/Molten/Font/GPOS/ValueFormat/YPlacement.md)  
    *  [YPlacementDevice](docs/Molten.Font/Molten/Font/GPOS/ValueFormat/YPlacementDevice.md)  
*  [ValueRecord](docs/Molten.Font/Molten/Font/GPOS/ValueRecord.md)  
    *  [Format](docs/Molten.Font/Molten/Font/GPOS/ValueRecord/Format.md)  
    *  [XAdvance](docs/Molten.Font/Molten/Font/GPOS/ValueRecord/XAdvance.md)  
    *  [XAdvDeviceOffset](docs/Molten.Font/Molten/Font/GPOS/ValueRecord/XAdvDeviceOffset.md)  
    *  [XPlacement](docs/Molten.Font/Molten/Font/GPOS/ValueRecord/XPlacement.md)  
    *  [XPlaDeviceOffset](docs/Molten.Font/Molten/Font/GPOS/ValueRecord/XPlaDeviceOffset.md)  
    *  [YAdvance](docs/Molten.Font/Molten/Font/GPOS/ValueRecord/YAdvance.md)  
    *  [YAdvDeviceOffset](docs/Molten.Font/Molten/Font/GPOS/ValueRecord/YAdvDeviceOffset.md)  
    *  [YPlacement](docs/Molten.Font/Molten/Font/GPOS/ValueRecord/YPlacement.md)  
    *  [YPlaDeviceOffset](docs/Molten.Font/Molten/Font/GPOS/ValueRecord/YPlaDeviceOffset.md)