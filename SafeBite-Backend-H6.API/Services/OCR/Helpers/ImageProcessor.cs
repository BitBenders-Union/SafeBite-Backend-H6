using SafeBite_Backend_H6.API.Interfaces.Services.OCR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace SafeBiteApi.Services.OCR.Helpers
{
    public class ImageProcessor : IImageProcessor
    {
        public byte[] PreprocessForOcr(Stream input)
        {
            input.Position = 0;
            using var img = Image.Load<Rgba32>(input);

            img.Mutate(picture => picture
                .AutoOrient()
                .Resize(new ResizeOptions
                {
                    Size = new Size(1200, 1200),
                    Mode = ResizeMode.Max
                })
                .Grayscale()
                .Contrast(1.8f)
                .Brightness(1.1f)
                .GaussianSharpen(0.4f)
            );

            using var output = new MemoryStream();
            img.SaveAsPng(output);
            return output.ToArray();
        }
    }
}
