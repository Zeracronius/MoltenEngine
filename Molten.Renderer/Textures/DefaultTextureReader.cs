﻿using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics.Textures
{
    public class DefaultTextureReader : TextureReader
    {
        Logger _log;
        string _filename;

        public override TextureData Read(BinaryReader reader, Logger log, string filename = null)
        {
            _log = log;
            _filename = null;

            TextureData data = new TextureData()
            {
                IsCompressed = false,
                HighestMipMap = 0,
                MipMapLevels = 1,
                SampleCount = 1,
                Flags = TextureFlags.None,
                Format = GraphicsFormat.R8G8B8A8_UNorm,
            };

            using (MagickImage image = new MagickImage(reader.BaseStream))
            {
                image.Warning += Image_Warning;
                data.Width = image.Width;
                data.Height = image.Height;
                IPixelCollection pixels = image.GetPixels();
                TextureData.Slice slice = new TextureData.Slice()
                {
                    Data = pixels.ToByteArray(PixelMapping.RGBA),
                    Width = image.Width,
                    Height = image.Height,
                    Pitch = image.Width * 4 // We're using 4 bytes per pixel (RGBA)
                };

                slice.TotalBytes = slice.Data.Length;
                data.Levels = new TextureData.Slice[] { slice };

                image.Warning -= Image_Warning;
            }

            _log = null;
            return data;
        }

        private void Image_Warning(object sender, WarningEventArgs e)
        {
            _log.WriteWarning(e.Message, _filename);
            if (e.Exception != null)
                _log.WriteError(e.Exception, true);
        }
    }
}
