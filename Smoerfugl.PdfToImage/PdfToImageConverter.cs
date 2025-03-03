using Docnet.Core;
using Docnet.Core.Converters;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using SkiaSharp;

namespace Smoerfugl.PdfToImage;

public class PdfToImageConverter
{
    public byte[] ConvertPageToImage(byte[] file, int pageNumber)
    {
        using var reader = DocLib.Instance.GetDocReader(file, new PageDimensions(1080, 1920));
        var pageCount = reader.GetPageCount();
        if (pageNumber < 1 || pageNumber > pageCount)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), $"Page number must be between 1 and {pageCount}");
        }

        // Page numbers are 1-based, but the reader is 0-based
        var pageReader = reader.GetPageReader(pageNumber - 1);
        var width = pageReader.GetPageWidth();
        var height = pageReader.GetPageHeight();
        var info = new SKImageInfo(width, height, SKColorType.Bgra8888);

        using var surface = SKSurface.Create(info);
        using var canvas = surface.Canvas;
        canvas.Clear(SKColors.White);

        RenderImage(pageReader, info, canvas);
        using var finalImage = surface.Snapshot();

        return finalImage.Encode().ToArray();
    }

    private static void RenderImage(IPageReader pageReader, SKImageInfo info, SKCanvas canvas)
    {
        var bytes = pageReader.GetImage(new NaiveTransparencyRemover(), RenderFlags.RenderAnnotations);
        using var image = SKImage.FromPixelCopy(info, bytes);
        using var tmpImage = image.Encode();
        using var bitmap = SKBitmap.Decode(tmpImage);
        canvas.DrawBitmap(bitmap, bitmap.Info.Rect);
        canvas.Flush();
    }
}