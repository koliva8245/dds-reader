using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.IO;

namespace DDSReader
{
    public class DDSImage
    {
        private readonly Pfim.IImage _image;

        public byte[] Data
        {
            get
            {
                if (_image != null)
                    return _image.Data;
                else
                    return new byte[0];
            }
        }

        public int Width => _image.Width;

        public int Height => _image.Height;

        public DDSImage(string file)
        {
            _image = Pfim.Pfim.FromFile(file);
            Process();
        }

        public DDSImage(Stream stream)
        {
            if (stream == null)
                throw new Exception("DDSImage ctor: Stream is null");

            _image = Pfim.Dds.Create(stream, new Pfim.PfimConfig());
            Process();
        }

        public DDSImage(byte[] data)
        {
            if (data == null || data.Length <= 0)
                throw new Exception("DDSImage ctor: no data");

            _image = Pfim.Dds.Create(data, new Pfim.PfimConfig());
            Process();
        }

        public void Save(string file)
        {
            if (_image.Format == Pfim.ImageFormat.Rgba32)
                Save<Bgra32>(file);
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
                Save<Bgr24>(file);
            else
                throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }

        public void Save(string file, Point point, Size size)
        {
            if (_image.Format == Pfim.ImageFormat.Rgba32)
                Save<Bgra32>(file, point, size);
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
                Save<Bgr24>(file, point, size);
            else
                throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }

        private void Process()
        {
            if (_image == null)
                throw new Exception("DDSImage image creation failed");

            if (_image.Compressed)
                _image.Decompress();
        }

        private void Save<T>(string file)
            where T : struct, IPixel<T>
        {
            Image<T> image = Image.LoadPixelData<T>(
                _image.Data, _image.Width, _image.Height);
            image.Save(file);
        }

        private void Save<T>(string file, Point point, Size size)
            where T : struct, IPixel<T>
        {
            Image<T> image = Image.LoadPixelData<T>(
                _image.Data, _image.Width, _image.Height);

            image.Mutate(x => x.Crop(new Rectangle(point, size)));
            image.Save(file);
        }
    }
}
