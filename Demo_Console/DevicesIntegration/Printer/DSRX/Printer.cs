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
    private const string PrinterName = "DS-RX1";
    // The PrintPageEventArgs page bound is 615x413, different page size won't be process correctly
    private const int PageSizeWidth = 615;
    private const int PageSizeHeight = 413;
    private const float PrnSizeWidth = 604;
    private const float PrnSizeHeight = 399.8f;
    private const int DefaultPrintingMargin = 5;
    private const float PrintingFluctuationAllowance = 2;
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
        Orientation = (short)Orientation.OT1, // Print Horizontally
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

            // Must have | Retest!!
            DevMode.PrinterName = PrinterName;
            DevMode.ExDevModeTopSearch();

            using var img = Image.FromFile(ImageFilePath);
            ImageFilePath = info.FilePath;
            _totalPageToPrint = info.NumberOfPage;
            _isHalfCut = IsNeedHalfCut(img.Width, img.Height);
            _isRotateRequired = IsNeedRotate(img.Width, img.Height);

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
    private static bool IsNeedHalfCut(int width, int height) => Math.Abs((decimal)width / height) < 0.35m;
    
    /// <summary>
    /// Determine should the image be rotated or not
    /// </summary>
    private static bool IsNeedRotate(int width, int height) => width < height;
    
    private static void PrintPage(object sender, PrintPageEventArgs e)
    {
        // Load the image from the file path
        using var img = Image.FromFile(ImageFilePath);
        if(_isRotateRequired) img.RotateFlip(RotateFlipType.Rotate90FlipNone);
        var ratio = (float)e.PageBounds.Width / img.Width;

        // Check if the image is 2x6 paper size
        if (_isHalfCut)
        {
            var printWidth = Math.Min(PrnSizeWidth, img.Width * ratio);
            var printHeight = Math.Min(PrnSizeHeight / 2, img.Height * ratio);

            e.Graphics?.DrawImage(img, DefaultPrintingMargin, DefaultPrintingMargin,
                IsFluctuationWithinPercentConstraint(printWidth, PrintingFluctuationAllowance)
                    ? printWidth
                    : PrnSizeWidth,
                IsFluctuationWithinPercentConstraint(printHeight, PrintingFluctuationAllowance)
                    ? printHeight
                    : PrnSizeHeight);
            e.Graphics?.DrawImage(img, DefaultPrintingMargin, DefaultPrintingMargin + printHeight,
                IsFluctuationWithinPercentConstraint(printWidth, PrintingFluctuationAllowance)
                    ? printWidth
                    : PrnSizeWidth,
                IsFluctuationWithinPercentConstraint(printHeight, PrintingFluctuationAllowance)
                    ? printHeight
                    : PrnSizeHeight);
        }
        else
        {
            var printWidth = Math.Min(PrnSizeWidth, img.Width * ratio);
            var printHeight = Math.Min(PrnSizeHeight, img.Height * ratio);

            e.Graphics?.DrawImage(img, DefaultPrintingMargin, DefaultPrintingMargin,
                IsFluctuationWithinPercentConstraint(printWidth, PrintingFluctuationAllowance)
                    ? printWidth
                    : PrnSizeWidth,
                IsFluctuationWithinPercentConstraint(printHeight, PrintingFluctuationAllowance)
                    ? printHeight
                    : PrnSizeHeight);
        }

        if (_currentPrintedPage >= _totalPageToPrint) return;
        
        _currentPrintedPage++;
        e.HasMorePages = true;
    }

    private static bool IsFluctuationWithinPercentConstraint(float value, float percentage)
    {
        if(percentage <= 0) throw new ArgumentException("Percentage must be greater than 0", nameof(percentage));

        var fluctuation = value / 100 * percentage;
        return value >= value - fluctuation && value <= value + fluctuation;
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