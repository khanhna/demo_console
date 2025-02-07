using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Versioning;

namespace Demo_Console.DevicesIntegration.Printer.DSRX;

public class ImagePrintInfo
{
    public string FilePath { get; set; } = string.Empty;
    public bool IsHalfCut { get; set; }
    public bool IsRotateRequired { get; set; }
    public int NumberOfPage { get; set; }
}

/// <summary>
/// Implementation for DNP DS-RX1 HS printer!
/// </summary>
[SupportedOSPlatform("windows")]
public static class Printer
{
    public const string PrinterName = "DS-RX1";
    public const int PageSizeHeight = 616;
    public const int PageSizeWidth = 413;
    public const float PrnSizeWidth = 604;
    public const float PrnSizeHeight = 399.8f;
    private static readonly int DefaultPrintingMargin = 6;
    // Scale image to create border without reveal white edge, can adjust by testing
    private static readonly float PrintingScaleFactor = 0.97f;
    
    public static string ImageFilePath { get; private set; } = string.Empty;

    private static int _currentPrintedPage = 1;
    private static int _totalPageToPrint = 0;
    private static bool _isHalfCut;
    private static bool _isRotateRequired;

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

            // Must have
            DevMode.PrinterName = PrinterName;
            DevMode.ExDevModeTopSearch();

            ImageFilePath = info.FilePath;
            _totalPageToPrint = info.NumberOfPage;
            _isHalfCut = info.IsHalfCut;
            _isRotateRequired = info.IsRotateRequired;

            var pd = new PrintDocument
            {
                DocumentName = Guid.NewGuid().ToString().ToLower()
            };
            pd.DefaultPageSettings.PaperSize =
                new System.Drawing.Printing.PaperSize("DefaultPaperSize", PageSizeWidth, PageSizeHeight);
            pd.PrinterSettings.Copies = (short)info.NumberOfPage;
            
            var prnSet = DefaultPrnSetting.Clone();
            // Check if the image is 2x6 paper size 
            prnSet.Cut2inch = _isHalfCut ? (short)Cut2inch.CT2 : (short)Cut2inch.CI1;
            DevMode.PrintSettingRefactored(ref prnSet, ref pd);

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
    
    // Expected Size is 604 x 399.8
    private static void PrintPage(object sender, PrintPageEventArgs e)
    {
        // Load the image from the file path
        using var img = Image.FromFile(ImageFilePath);
        if(_isRotateRequired) img.RotateFlip(RotateFlipType.Rotate90FlipNone);

        // Check if the image is 2x6 paper size
        // TODO: Remember to test with 2x6 Horizontally images!!
        if (_isHalfCut)
        {
            var ratio = (float)e.PageBounds.Width / img.Width;
            var printWidth = Math.Min(PrnSizeWidth, img.Width * ratio);
            var printHeight = Math.Min(PrnSizeHeight / 2, img.Height * ratio);

            var anchorTop = 5;
            var anchorLeft = anchorTop;
            
            e.Graphics?.DrawImage(img, anchorLeft, anchorTop, printWidth, printHeight);
            e.Graphics?.DrawImage(img, anchorLeft, anchorTop + printHeight, printWidth, printHeight);
        }
        else
        {
            if (img.Width < img.Height)
            {
                // Calculate to resize image fit with printing paper size
                var scale = Math.Min((float)PageSizeWidth / img.Width, (float)PageSizeHeight / img.Height) * PrintingScaleFactor;
                var scaledWidth = Convert.ToInt32(img.Width * scale);
                var scaledHeight = Convert.ToInt32(img.Height * scale + 10);
                
                var anchorLeft = (PageSizeWidth - scaledWidth) / 2;
                var anchorTop = (PageSizeHeight - scaledHeight) / 2;

                // If there's white edge, justify top anchor
                if (scaledHeight < PageSizeHeight) anchorTop = Convert.ToInt32(anchorTop * 0.96);
                e.Graphics?.DrawImage(img, anchorLeft, anchorTop, scaledWidth, scaledHeight);
            }
            else
            {
                var ratio = (float)((decimal)e.PageBounds.Height / img.Height) * PrintingScaleFactor;
                var scaledWidth = img.Width * ratio;
                var scaledHeight = img.Height * ratio + 10;
                
                if (scaledWidth < e.PageBounds.Width)
                {
                    ratio = (float)(e.PageBounds.Width / (double)img.Width);
                    scaledWidth = img.Width * ratio - 14;
                    scaledHeight = img.Height * ratio - 12;
                }
                
                // Centering
                var overWidth = e.PageBounds.Width - scaledWidth;
                var overHeight = e.PageBounds.Height - scaledHeight;
                var anchorTop = overWidth / 2;
                var anchorLeft = overHeight / 2;

                e.Graphics?.DrawImage(img, anchorTop, anchorLeft, scaledWidth, scaledHeight);
            }
        }

        if (_currentPrintedPage >= _totalPageToPrint) return;
        
        _currentPrintedPage++;
        e.HasMorePages = true;
    }

    private static void ResetState()
    {
        ImageFilePath = string.Empty;
        _currentPrintedPage = 1;
        _totalPageToPrint = 0;
        _isHalfCut = false;
        _isRotateRequired = false;
    }
}