using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Demo_Console.DevicesIntegration.Printer.DSRX;

public class PrinterPort
{
    [DllImport("kernel32.dll", EntryPoint = "lstrlenA", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern int lstrlen(IntPtr lpString);

    [DllImport("winspool.Drv", EntryPoint = "OpenPrinterW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool OpenPrinter(string src, out IntPtr hPrinter, IntPtr pd);

    [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool EnumJobs(IntPtr hPrinter, int firstJob, int noJobs, int level, IntPtr pJob, int cdBuf,
        out int pcbNeeded, out int pcReturned);

    [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.drv", SetLastError = true)]
    private static extern bool GetPrinter(IntPtr hPrinter, int dwLevel, IntPtr pPrinter, int cbBuf, out int pcbNeeded);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PRINTER_INFO_5
    {
        public IntPtr pPrinterName;
        public IntPtr pPortName;
        public int Attributes;
        public int DeviceNotSelectedTimeout;
        public int TransmissionRetryTimeout;
    }

    private const int LngPrinterInfo5Level = 5;
    private const int PRINTER_ATTRIBUTE_DEFAULT = 0x4;
    private const int PRINTER_ATTRIBUTE_ENABLE_BIDI = 0x800;
    private const int PRINTER_ATTRIBUTE_WORK_OFFLINE = 0x400;

    public static string GetPortName(ref string strPrinterDeviceName)
    {
        if (!OpenPrinter(strPrinterDeviceName, out var hPrinter, IntPtr.Zero))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        var pPrinterInfo = IntPtr.Zero;

        try
        {
            GetPrinter(hPrinter, LngPrinterInfo5Level, IntPtr.Zero, 0, out var lngPrinterInfo5Needed);
            if (lngPrinterInfo5Needed <= 0) throw new Exception("error!");

            pPrinterInfo = Marshal.AllocHGlobal(lngPrinterInfo5Needed);

            if (!GetPrinter(hPrinter, LngPrinterInfo5Level, pPrinterInfo, lngPrinterInfo5Needed, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            var printerInfo = (PRINTER_INFO_5)Marshal.PtrToStructure(pPrinterInfo, typeof(PRINTER_INFO_5))!;

            if ((printerInfo.Attributes & PRINTER_ATTRIBUTE_ENABLE_BIDI) != 0 &&
                (printerInfo.Attributes & PRINTER_ATTRIBUTE_WORK_OFFLINE) == 0)
            {
                var size = lstrlen(printerInfo.pPortName);

                var bytPortName = new byte[size];
                Marshal.Copy(printerInfo.pPortName, bytPortName, 0, size);
                return System.Text.Encoding.GetEncoding("Shift_JIS").GetString(bytPortName);
            }
            else return string.Empty;
        }
        finally
        {
            ClosePrinter(hPrinter);
            Marshal.FreeHGlobal(pPrinterInfo);
        }
    }
}