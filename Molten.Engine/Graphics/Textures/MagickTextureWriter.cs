﻿using ImageMagick;
using System.IO;

namespace Molten.Graphics.Textures
{
    public abstract class MagickTextureWriter : TextureWriter
    {
        MagickFormat _writeFormat;

        protected MagickTextureWriter(MagickFormat writeFormat)
        {
            _writeFormat = writeFormat;
        }

        public override void WriteData(Stream stream, TextureData data, Logger log, string filename = null)
        {
            TextureData newData = data.Clone();
            newData.ToRGBA(log);

            TextureData.Slice slice;
            for (int i = 0; i < newData.Levels.Length; i++)
            {
                slice = newData.Levels[i];
                using (MagickImage image = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), (int)slice.Width, (int)slice.Height))
                {
                    IPixelCollection<byte> p = image.GetPixels();
                    p.SetPixels(slice.Data);

                    image.Format = MagickFormat.Rgba;
                    image.Quality = 100;
                    image.Interlace = Interlace.NoInterlace;
                    image.Write(stream, _writeFormat);
                }
            }
        }

        protected override void OnDispose() { }
    }

    public class PNGWriter : MagickTextureWriter
    {
        public PNGWriter() : base(MagickFormat.Png) { }
    }

    public class JPEGWriter : MagickTextureWriter
    {
        public JPEGWriter() : base(MagickFormat.Jpeg) { }
    }

    public class BMPWriter : MagickTextureWriter
    {
        public BMPWriter() : base(MagickFormat.Bmp) { }
    }
}
