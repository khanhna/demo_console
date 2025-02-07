using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Demo_Console.DevicesIntegration.Printer.DSRX;

public class DevMode
{
    public static string PrinterName;

    //Global Memory Flags
    public const int GMEM_FIXED = 0;
    public const int GMEM_MOVEABLE = 2;
    public const int GMEM_ZEROINIT = 64;

    public const int GHND = (GMEM_MOVEABLE | GMEM_ZEROINIT);
    public const int GPTR = (GMEM_FIXED | GMEM_ZEROINIT);

    // Mode selections for the device mode function (DEVMODE)
    public const int DM_UPDATE = 1;
    public const int DM_COPY = 2;
    public const int DM_PROMPT = 4;
    public const int DM_MODIFY = 8;

    public const int DM_IN_BUFFER = DM_MODIFY;
    public const int DM_IN_PROMPT = DM_PROMPT;
    public const int DM_OUT_BUFFER = DM_COPY;
    public const int DM_OUT_DEFAULT = DM_UPDATE;

    // Field selection bits (DEVMODE)
    public const int DM_ORIENTATION = 0x1;
    public const int DM_PAPERSIZE = 0x2;
    public const int DM_PAPERLENGTH = 0x4;
    public const int DM_PAPERWIDTH = 0x8;
    public const int DM_SCALE = 0x10;
    public const int DM_COPIES = 0x100;
    public const int DM_DEFAULTSOURCE = 0x200;
    public const int DM_PRINTQUALITY = 0x400;
    public const int DM_COLOR = 0x800;
    public const int DM_DUPLEX = 0x1000;
    public const int DM_YRESOLUTION = 0x2000;
    public const int DM_TTOPTION = 0x4000;
    public const int DM_COLLATE = 0x8000;
    public const int DM_FORMNAME = 0x10000;
    public const int DM_LOGPIXELS = 0x20000;
    public const int DM_BITSPERPEL = 0x40000;
    public const int DM_PELSWIDTH = 0x80000;
    public const int DM_PELSHEIGHT = 0x100000;
    public const int DM_DISPLAYFLAGS = 0x200000;
    public const int DM_DISPLAYFREQUENCY = 0x400000;
    public const int DM_ICMMETHOD = 0x800000;

    // Orientation selections (DEVMODE)
    public const short DM_ORIENT_PORTRAIT = 1; // Portrait
    public const short DM_ORIENT_LANDSCAPE = 2; // Landscape

    // ICM selections (DEVMODE)
    public const short DM_ICMMETHOD_NONE = 1;   // ICM OFF
    public const short DM_ICMMETHOD_SYSTEM = 2; // ON

    // Color adjustment (Private DEVMODE)
    public const short DM_EXCOLORADJ_NONE = 0;   // Color Adjustment OFF
    public const short DM_EXCOLORADJ_DRIVER = 1; // ON

    // Border (Private DEVMODE)
    public const short DM_BORDER_OFF = 0; // Border OFF
    public const short DM_BORDER_ON = 1; // ON

    // Matte (Private DEVMODE)
    public const short DM_OVERCOA_GLOSSY = 0; // Glossy
    public const short DM_OVERCOA_MATTE = 1; // Matte

    // Print Re-try (Private DEVMODE)
    public const short DM_PRN_RETRY_OFF = 0; // Print Re-try OFF
    public const short DM_PRN_RETRY_ON = 1; // ON

    // Print Quality (DEVMODE)
    public const short QUALITY_300x300 = 0; // 300x300dpi
    public const short QUALITY_300x600 = 1; // 300x600dpi
    public const short QUALITY_600x600 = 2; // 600x600dpi

    // 2inch Cut
    public const short DM_CUT2INCH_OFF = 0;       // 2inch Cut OFF
    public const short DM_CUT2INCH_ON = 1;        // 2inch Cut ON

    // Top of private DEVMODE
    public static int ExDevTop;

    // Size of secured area for private DEVMODE
    public const short EX_DEV_SIZE = 1024;

    // Header of private DEVMODE
    public const int EX_DEV_SIGN = 0x4D534654;     // "MSFT"

    // Position of private DEVMODE
    public const int ExStrSize = 0;                // Size of private DEVMODE
    public const int ExSign = 1;                   // Header("MSFT")
    public const int ExVer = 2;                    // Version of private DEVMODE
    public const int ExBorder = 3;                 // Border
    public const int ExColorAdjustment = 4;        // Color Adjustment
    public const int ExSharpness = 5;              // Sharpness
    public const int ExAdjGammaR = 6;              // Gamma R
    public const int ExAdjGammaG = 7;              // G
    public const int ExAdjGammaB = 8;              // B
    public const int ExAdjBrightnessR = 10;        // Brightness R
    public const int ExAdjBrightnessG = 11;        // G
    public const int ExAdjBrightnessB = 12;        // B
    public const int ExAdjContrastR = 14;          // Contrast R
    public const int ExAdjContrastG = 15;          // G
    public const int ExAdjContrastB = 16;          // B
    public const int ExAdjChromaR = 18;            // Chroma R
    public const int ExAdjChromaG = 19;            // G
    public const int ExAdjChromaB = 20;            // B
    public const int ExOvercoarFinish = 27;        // OvercoarFinishiGlossy /Mattej
    public const int ExPrintRetry = 29;     // Print Re-try
    public const int ExCut2inch = 30;       // 2inch Cut

    [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
    public static extern int CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
    public static extern int ResetDC(int hdc, IntPtr lpInitData);

    [DllImport("winspool.drv", EntryPoint = "DocumentPropertiesW", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    public static extern int DocumentProperties(int hwnd, int hPrinter, [MarshalAs(UnmanagedType.LPWStr)] string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern int GlobalAlloc(int uFlags, int dwBytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern int GlobalFree(int hMem);

    [DllImport("kernel32.dll")]
    public static extern int GlobalLock(int hMem);

    // Structure of print setting
    public struct PRINT_SETTING
    {
        public short PaperSize;
        public short Orientation;
        public short PrintQuality;
        public short YResolution;
        public int ICMMethod;
        public int Border;
        public int Sharpness;
        public int ColorAdjustment;
        public int AdjGammaR;
        public int AdjGammaG;
        public int AdjGammaB;
        public int AdjBrightnessR;
        public int AdjBrightnessG;
        public int AdjBrightnessB;
        public int AdjContrastR;
        public int AdjContrastG;
        public int AdjContrastB;
        public int AdjChromaR;
        public int AdjChromaG;
        public int AdjChromaB;
        public int OvercoatFinish;
        public int PrintRetry;
        public int Cut2inch;

        public readonly PRINT_SETTING Clone() => new()
        {
            PaperSize = PaperSize,
            Orientation = Orientation,
            PrintQuality = PrintQuality,
            YResolution = YResolution,
            ICMMethod = ICMMethod,
            Border = Border,
            Sharpness = Sharpness,
            ColorAdjustment = ColorAdjustment,
            AdjGammaR = AdjGammaR,
            AdjGammaG = AdjGammaG,
            AdjGammaB = AdjGammaB,
            AdjBrightnessR = AdjBrightnessR,
            AdjBrightnessG = AdjBrightnessG,
            AdjBrightnessB = AdjBrightnessB,
            AdjContrastR = AdjContrastR,
            AdjContrastG = AdjContrastG,
            AdjContrastB = AdjContrastB,
            AdjChromaR = AdjChromaR,
            AdjChromaG = AdjChromaG,
            AdjChromaB = AdjChromaB,
            OvercoatFinish = OvercoatFinish,
            PrintRetry = PrintRetry,
            Cut2inch = Cut2inch
        };
    }

    // Structure of DEVMODE-W (for Print Document)
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct DEVMODEW
    {
        const int CCHDEVICENAME = 32 * 2;
        const int CCHFORMNAME = 32 * 2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = CCHDEVICENAME)]
        public byte[] dmDeviceName;

        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;
        public short dmOrientation;
        public short dmPaperSize;
        public short dmPaperLength;
        public short dmPaperWidth;
        public short dmScale;
        public short dmCopies;
        public short dmDefaultSource;
        public short dmPrintQuality;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = CCHFORMNAME)]
        public byte[] dmFormName;

        public short dmUnusedPadding;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmICCManufacturer;
        public int dmICCModel;
        public int dmPanningWidth;
        public int dmPanningHeight;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = EX_DEV_SIZE)]
        public int[] ExDevMode;
    }

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
    private static extern void Mem2DevCopy(ref DEVMODEW dest, IntPtr hpvSource, int cbCopy);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
    private static extern void Dev2MemCopy(IntPtr hpvDest, ref DEVMODEW hpvSource, int cbCopy);

    public static void ExDevModeTopSearch()
    {
        PrinterModule.OpenPrinter(PrinterName, out var hPrinter, IntPtr.Zero);
        var devModeSize = DocumentProperties(0, hPrinter.ToInt32(), PrinterName, IntPtr.Zero, IntPtr.Zero, 0);
        if (devModeSize == 0)
        {
            PrinterModule.ClosePrinter(hPrinter);
            return;
        }

        var hDevMode = GlobalAlloc(GHND, devModeSize);
        var lpDevModeDmW = new IntPtr(GlobalLock(hDevMode));
        var resultCode = DocumentProperties(0, hPrinter.ToInt32(), PrinterName, lpDevModeDmW, IntPtr.Zero, DM_OUT_BUFFER);
        if (resultCode < 0)
        {
            GlobalFree(lpDevModeDmW.ToInt32());
            PrinterModule.ClosePrinter(hPrinter);
            return;
        }

        var dmw = default(DEVMODEW);
        Mem2DevCopy(ref dmw, lpDevModeDmW, devModeSize);

        var n = 0;
        do
        {
            if (dmw.ExDevMode[n] == EX_DEV_SIGN)
            {
                ExDevTop = n - 1; // Top position
                break;
            }

            if (n >= EX_DEV_SIZE)
            {
                break; // "MSFT" is not found.
            }
            n++;
        } while (true);

        GlobalFree(lpDevModeDmW.ToInt32());
        PrinterModule.ClosePrinter(hPrinter);
    }
    
    [SupportedOSPlatform("windows")]
    public static void PrintSetting(ref PRINT_SETTING prnSet, ref PrintDocument pd)
    {
        try
        {
            IntPtr hPrinter;
            int ResultCode;
            int DevModeSize;
            DEVMODEW dmw = new DEVMODEW();

            int hDevMode;
            int lpDevModeDmW;

            ResultCode = PrinterModule.OpenPrinter(PrinterName, out hPrinter, IntPtr.Zero);
            DevModeSize = DocumentProperties(0, hPrinter.ToInt32(), PrinterName, IntPtr.Zero, IntPtr.Zero, 0);
            if (DevModeSize == 0)
            {
                PrinterModule.ClosePrinter(hPrinter);
                return;
            }

            hDevMode = GlobalAlloc(GHND, DevModeSize);
            lpDevModeDmW = GlobalLock(hDevMode);

            ResultCode = DocumentProperties(0, hPrinter.ToInt32(), PrinterName, new IntPtr(lpDevModeDmW), IntPtr.Zero, DM_OUT_BUFFER);
            if (ResultCode < 0)
            {
                GlobalFree(lpDevModeDmW);
                PrinterModule.ClosePrinter(hPrinter);
                return;
            }
            IntPtr ptr = new IntPtr(lpDevModeDmW);
            var dm = IntPtr.Zero;
            Mem2DevCopy(ref dmw, ptr, DevModeSize);

            // Setting of DevMode
            // Paper size
            dmw.dmPaperSize = prnSet.PaperSize;

            // Orientation
            dmw.dmOrientation = prnSet.Orientation;

            // Border
            dmw.ExDevMode[ExDevTop + ExBorder] = prnSet.Border;

            // Sharpness
            dmw.ExDevMode[ExDevTop + ExSharpness] = prnSet.Sharpness;

            // Resolution
            dmw.dmPrintQuality = prnSet.PrintQuality;
            dmw.dmYResolution = prnSet.YResolution;

            // ICM
            dmw.dmICMMethod = prnSet.ICMMethod;

            // Color Adjustment
            dmw.ExDevMode[ExDevTop + ExColorAdjustment] = prnSet.ColorAdjustment;

            // Gamma
            dmw.ExDevMode[ExDevTop + ExAdjGammaR] = prnSet.AdjGammaR;
            dmw.ExDevMode[ExDevTop + ExAdjGammaG] = prnSet.AdjGammaG;
            dmw.ExDevMode[ExDevTop + ExAdjGammaB] = prnSet.AdjGammaG;

            // Brightness
            dmw.ExDevMode[ExDevTop + ExAdjBrightnessR] = prnSet.AdjBrightnessR;
            dmw.ExDevMode[ExDevTop + ExAdjBrightnessG] = prnSet.AdjBrightnessG;
            dmw.ExDevMode[ExDevTop + ExAdjBrightnessB] = prnSet.AdjBrightnessB;

            // Contrast
            dmw.ExDevMode[ExDevTop + ExAdjContrastR] = prnSet.AdjContrastR;
            dmw.ExDevMode[ExDevTop + ExAdjContrastG] = prnSet.AdjContrastR;
            dmw.ExDevMode[ExDevTop + ExAdjContrastB] = prnSet.AdjContrastR;

            // Chroma
            dmw.ExDevMode[ExDevTop + ExAdjChromaR] = prnSet.AdjChromaR;
            dmw.ExDevMode[ExDevTop + ExAdjChromaG] = prnSet.AdjChromaG;
            dmw.ExDevMode[ExDevTop + ExAdjChromaB] = prnSet.AdjChromaB;

            // Overcoar Finish
            // Overcoar Finish
            dmw.ExDevMode[ExDevTop + ExOvercoarFinish] = prnSet.OvercoatFinish;

            // Print Re-try
            dmw.ExDevMode[ExDevTop + ExPrintRetry] = prnSet.PrintRetry;

            // 2inch Cut
            if (PrinterPaper.PrinterModel == PrinterPaper.PRN_6IN)
            {
                dmw.ExDevMode[ExDevTop + ExCut2inch] = prnSet.Cut2inch;
            }

            dmw.dmFields |= DM_PAPERSIZE | DM_PRINTQUALITY | DM_YRESOLUTION | DM_ORIENTATION | DM_ICMMETHOD;
            dmw.dmFields &= ~(DM_PAPERLENGTH | DM_PAPERWIDTH);
            var inpt = new IntPtr(lpDevModeDmW);
            Dev2MemCopy(inpt, ref dmw, DevModeSize);
            PrinterModule.ClosePrinter(hPrinter);
            pd.PrinterSettings.SetHdevmode(inpt);
            ResultCode = GlobalFree(lpDevModeDmW);
        }
        catch { }
    }

    [SupportedOSPlatform("windows")]
    public static void PrintSettingRefactored(ref PRINT_SETTING prnSet, ref PrintDocument pd)
    {
        try
        {
            PrinterModule.OpenPrinter(PrinterName, out var hPrinter, IntPtr.Zero);
            var devModeSize = DocumentProperties(0, hPrinter.ToInt32(), PrinterName, IntPtr.Zero, IntPtr.Zero, 0);
            if (devModeSize == 0)
            {
                PrinterModule.ClosePrinter(hPrinter);
                return;
            }

            var hDevMode = GlobalAlloc(GHND, devModeSize);
            var lpDevModeDmW = GlobalLock(hDevMode);

            if (DocumentProperties(0, hPrinter.ToInt32(), PrinterName, new IntPtr(lpDevModeDmW), IntPtr.Zero,
                    DM_OUT_BUFFER) < 0)
            {
                GlobalFree(lpDevModeDmW);
                PrinterModule.ClosePrinter(hPrinter);
                return;
            }

            var dmw = new DEVMODEW();
            var ptr = new IntPtr(lpDevModeDmW);
            Mem2DevCopy(ref dmw, ptr, devModeSize);

            // Setting of DevMode
            // Paper size
            dmw.dmPaperSize = prnSet.PaperSize;
            // Orientation
            dmw.dmOrientation = prnSet.Orientation;
            // Border
            dmw.ExDevMode[ExDevTop + ExBorder] = prnSet.Border;
            // Sharpness
            dmw.ExDevMode[ExDevTop + ExSharpness] = prnSet.Sharpness;
            // Resolution
            dmw.dmPrintQuality = prnSet.PrintQuality;
            dmw.dmYResolution = prnSet.YResolution;
            // ICM
            dmw.dmICMMethod = prnSet.ICMMethod;
            // Color Adjustment
            dmw.ExDevMode[ExDevTop + ExColorAdjustment] = prnSet.ColorAdjustment;
            // Gamma
            dmw.ExDevMode[ExDevTop + ExAdjGammaR] = prnSet.AdjGammaR;
            dmw.ExDevMode[ExDevTop + ExAdjGammaG] = prnSet.AdjGammaG;
            dmw.ExDevMode[ExDevTop + ExAdjGammaB] = prnSet.AdjGammaG;
            // Brightness
            dmw.ExDevMode[ExDevTop + ExAdjBrightnessR] = prnSet.AdjBrightnessR;
            dmw.ExDevMode[ExDevTop + ExAdjBrightnessG] = prnSet.AdjBrightnessG;
            dmw.ExDevMode[ExDevTop + ExAdjBrightnessB] = prnSet.AdjBrightnessB;
            // Contrast
            dmw.ExDevMode[ExDevTop + ExAdjContrastR] = prnSet.AdjContrastR;
            dmw.ExDevMode[ExDevTop + ExAdjContrastG] = prnSet.AdjContrastR;
            dmw.ExDevMode[ExDevTop + ExAdjContrastB] = prnSet.AdjContrastR;
            // Chroma
            dmw.ExDevMode[ExDevTop + ExAdjChromaR] = prnSet.AdjChromaR;
            dmw.ExDevMode[ExDevTop + ExAdjChromaG] = prnSet.AdjChromaG;
            dmw.ExDevMode[ExDevTop + ExAdjChromaB] = prnSet.AdjChromaB;
            // Overcoar Finish
            dmw.ExDevMode[ExDevTop + ExOvercoarFinish] = prnSet.OvercoatFinish;
            // Print Re-try
            dmw.ExDevMode[ExDevTop + ExPrintRetry] = prnSet.PrintRetry;

            // 2inch Cut
            if (PrinterPaper.PrinterModel == PrinterPaper.PRN_6IN)
            {
                dmw.ExDevMode[ExDevTop + ExCut2inch] = prnSet.Cut2inch;
            }

            dmw.dmFields |= DM_PAPERSIZE | DM_PRINTQUALITY | DM_YRESOLUTION | DM_ORIENTATION | DM_ICMMETHOD;
            dmw.dmFields &= ~(DM_PAPERLENGTH | DM_PAPERWIDTH);
            var inpt = new IntPtr(lpDevModeDmW);
            Dev2MemCopy(inpt, ref dmw, devModeSize);
            PrinterModule.ClosePrinter(hPrinter);
            pd.PrinterSettings.SetHdevmode(inpt);
            GlobalFree(lpDevModeDmW);
        }
        catch (Exception)
        {
            // Logging
        }
    }
}