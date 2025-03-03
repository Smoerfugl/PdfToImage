using VerifyTests.DiffPlex;

namespace Smoerfugl.PdfToImage.Tests;

public class ConvertPageToImageTests
{
    private readonly PdfToImageConverter _sut = new();

    [Theory]
    [InlineData("dummy.pdf")]
    public async Task GivenPdf_ShouldReturnPageAsImage(string filename)
    {
        var image = await File.ReadAllBytesAsync(filename);

        var result = _sut.ConvertPageToImage(image, 1);

        await Verify(result, "png")
            .ImageMagickComparer(0.005)
            .SinglePage(0)
            .DisableDiff()
            .UseDirectory(Path.Combine("..", "Verify"))
            .UseDiffPlex(OutputType.Compact);    }
}