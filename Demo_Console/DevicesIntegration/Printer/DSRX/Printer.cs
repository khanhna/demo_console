using System.Drawing;
using System.Drawing.Printing;

namespace Demo_Console.DevicesIntegration.Printer.DSRX;

public class ImagePrintInfo
{
    public string FilePath { get; set; } = string.Empty;
    public bool IsHalfCut { get; set; }
    public int NumberOfPage { get; set; }
}

// Implementation for DNP DS-RX1 HS printer!
public static class Printer
{
    public const int PageSizeHeight = 616;
    public const int PageSizeWidth = 413;
    private static readonly int DefaultPrintingMargin = 6;
    
    public static string ImageFilePath { get; private set; } = string.Empty;

    private static int _currentPrintedPage = 1;
    private static int _totalPageToPrint = 0;
    private static bool _isHalfCut;

    public static readonly DevMode.PRINT_SETTING DefaultPrnSetting = new()
    {
        PrintQuality = (short)PrintQuality.PQ1,
        YResolution = (short)YResolution.YR1,
        Sharpness = (short)Sharpness.SP1,
        PaperSize = (short)PaperSize.PZ1,
        Orientation = (short)Orientation.OT1,
        ICMMethod = 0,
        ColorAdjustment = 0,
        // Gamma
        AdjGammaR = 0,
        AdjGammaG = 0,
        AdjGammaB = 0,
        // Brightness
        AdjBrightnessR = 0,
        AdjBrightnessG = 0,
        AdjBrightnessB = 0,
        // Contrast,
        AdjContrastR = 0,
        AdjContrastG = 0,
        AdjContrastB = 0,
        //Chroma
        AdjChromaR = 0,
        AdjChromaG = 0,
        AdjChromaB = 0
    };
    
    public static (bool, string) PrintImage(ImagePrintInfo info)
    {
        try
        {
            if (!string.IsNullOrEmpty(ImageFilePath)) return (false, "Printing is in progress!");
            if (string.IsNullOrEmpty(info.FilePath)) return (false, "No file specified!");
            if (!(info.FilePath.EndsWith(".png") || info.FilePath.EndsWith(".jpg") || info.FilePath.EndsWith(".jpeg")))
                return (false, "Only accept *.png, *.jpg, *.jpeg file format!");
            if (!File.Exists(info.FilePath)) return (false, $"File specify at {info.FilePath} is not found!");
            if (info.NumberOfPage < 1) return (false, $"Invalid printing number - {info.NumberOfPage}");

            ImageFilePath = info.FilePath;
            _totalPageToPrint = info.NumberOfPage;
            _isHalfCut = info.IsHalfCut;

            var pd = new PrintDocument
            {
                DocumentName = Guid.NewGuid().ToString().ToLower(),
            };
            pd.DefaultPageSettings.PaperSize =
                new System.Drawing.Printing.PaperSize("DefaultPaperSize", PageSizeWidth, PageSizeHeight);
            pd.PrinterSettings.Copies = (short)info.NumberOfPage;

            using var img = Image.FromFile(ImageFilePath);
            var prnSet = DefaultPrnSetting.Clone();
            // Check if the image is 2x6 paper size 
            prnSet.Cut2inch = _isHalfCut ? (short)Cut2inch.CT2 : (short)Cut2inch.CI1;
            DevMode.PrintSetting(ref prnSet, ref pd);

            // Call the Print method
            _currentPrintedPage = 1;
            pd.PrintPage += PrintPage;
            pd.Print();
            ResetState();
            
            return (true, "Successfully printed!");
        }
        catch (Exception ex)
        {
            // Logging
            return (false, ex.Message);
        }
    }

    /// <summary>
    /// Determine should the image be cut in half or not, usually for detecting 2x6 paper size
    /// </summary>
    public static bool IsNeedHalfCut(int width, int height) => Math.Abs((double)width / height - 0.3333) < 0.01;
    
    private static void PrintPage(object sender, PrintPageEventArgs e)
    {
        // Load the image from the file path
        using var img = Image.FromFile(ImageFilePath);

        // Check if the image is 2x6 paper size
        // TODO: Remember to test with 2x6 Horizontally images!!
        if (_isHalfCut)
        {
            var widthDraw = e.PageBounds.Width / 2;
            var heightDraw = e.PageBounds.Height - (DefaultPrintingMargin * 2);
            e.Graphics?.DrawImage(img,
                new Rectangle(DefaultPrintingMargin, DefaultPrintingMargin, widthDraw - DefaultPrintingMargin,
                    heightDraw));
            e.Graphics?.DrawImage(img,
                new Rectangle(widthDraw - 1, DefaultPrintingMargin, widthDraw - DefaultPrintingMargin, heightDraw));
        }
        else
        {
            e.Graphics?.DrawImage(img,
                new Rectangle(DefaultPrintingMargin, DefaultPrintingMargin,
                    e.PageBounds.Width - (DefaultPrintingMargin * 2),
                    e.PageBounds.Height - (DefaultPrintingMargin * 2)));
        }

        if (_currentPrintedPage >= _totalPageToPrint) return;
        
        _currentPrintedPage++;
        e.HasMorePages = true;
    }

    private static void ResetState() => ImageFilePath = string.Empty;
}