using System.Runtime.CompilerServices;

namespace Smoerfugl.PdfToImage.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Run()
    {
        VerifyImageMagick.RegisterComparers(0.20);
    }
}