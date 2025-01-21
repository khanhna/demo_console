using System.Runtime.InteropServices;
using System.Text;

namespace Demo_Console.DevicesIntegration.Printer.DSRX;

public class PrinterPaper
{
    // Paper size information
    public struct StructPaperSize
    {
        public string Name; // size name
        public int Number; // size number
        public int MultiCut; // number of page
    }

    public static StructPaperSize[] PaperSize = new StructPaperSize[PrinterModule.MediaNumMax];

    [DllImport("winspool.drv", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    public static extern int DeviceCapabilities(string pDevice, string pPort, int fwCapability, IntPtr pOutput, IntPtr pDevMode);

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
    public static extern void MoveMemory(string destination, IntPtr source, int length);

    const int DC_PAPERNAMES = 16;
    const int DC_PAPERS = 2;

    public static short PrinterModel;
    public const short PRN_6IN = 0;
    public const short PRN_8IN = 1;

    // Get paper size information
    public static void GetPaperSizeInfo()
    {
        GetPaperSizeName();
        GetPaperSizeNumber();
        MultiCutNum();
    }

    // Get paper size name
    public static void GetPaperSizeName()
    {
        var prnName = DevMode.PrinterName;
        var strPortName = PrinterPort.GetPortName(ref prnName);
        var lngBuffSize = DeviceCapabilities(prnName, strPortName, DC_PAPERNAMES, IntPtr.Zero, IntPtr.Zero);
        var bytPaperName = new byte[lngBuffSize * 64];

        var gchPn = GCHandle.Alloc(bytPaperName, GCHandleType.Pinned);
        var lpPn = gchPn.AddrOfPinnedObject().ToInt32();
        DeviceCapabilities(prnName, strPortName, DC_PAPERNAMES,new IntPtr(lpPn), IntPtr.Zero);
        gchPn.Free();

        for (var n = 0; n < lngBuffSize; n++)
        {
            var paperSizeNameBuilder = new StringBuilder(64);
            
            for (var m = 0; m < 64; m++)
            {
                var b = bytPaperName[(n * 64) + m];
                if (b == 0) break;
                paperSizeNameBuilder.Append((char)b);
            }
            PaperSize[n].Name = paperSizeNameBuilder.ToString();
        }
    }

    //Get paper size number
    public static void GetPaperSizeNumber()
    {
        var strPrnName = DevMode.PrinterName;
        var strPortName = PrinterPort.GetPortName(ref strPrnName);

        var lngBuffSize = DeviceCapabilities(strPrnName, strPortName, DC_PAPERS, IntPtr.Zero, IntPtr.Zero);
        var intOutPut = new short[lngBuffSize];

        var gchPn = GCHandle.Alloc(intOutPut, GCHandleType.Pinned);
        var lpPn = gchPn.AddrOfPinnedObject().ToInt32();
        DeviceCapabilities(strPrnName, strPortName, DC_PAPERS, new IntPtr(lpPn), IntPtr.Zero);
        gchPn.Free();

        for (var n = 0; n < lngBuffSize; n++)
        {
            PaperSize[n].Number = intOutPut[n];
        }
    }

    // Set multi cut number
    public static void MultiCutNum()
    {
        for (var n = 0; n <= PrinterModule.MediaNumMax - 1; n++)
        {
            PaperSize[n].MultiCut = 1; // clear old MultiCut

            PaperSize[n].MultiCut = PaperSize[n].Name switch
            {
                "(5x3.5)" or "(5x7)" or "(6x4)" or "(6x8)" or "(6x9)" or "PR (4x6)" or "PR (3.5x5)" => 1,
                "(6x4) x 2" or "PR (4x6) x 2" or "(5x3.5) x 2" or "PR (3.5x5) x 2" => 2,
                _ => 1
            };
        }
    }
}