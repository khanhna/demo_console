using System.Runtime.InteropServices;

namespace Demo_Console.DevicesIntegration.Printer.DSRX;

public class PrinterModule
{
    public const int MediaNumMax = 32; // Number of paper size

    public const short COLOR_RED = 0xFF;
    public const int COLOR_WHITE = 0xFFFFFF;
    public const int COLOR_BLUE = 0xFFFFC0;
    public const int COLOR_GRAY = 0xC0C0C0;

    struct PRINTER_DEFAULTS
    {
        public int pDatatype;
        public int pDevMode;
        public int DesiredAccess;
    }

    [DllImport("winspool.Drv", EntryPoint = "OpenPrinterW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern int OpenPrinter(string src, out IntPtr hPrinter, IntPtr pd);

    [DllImport("winspool.Drv", EntryPoint = "EnumJobsA", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern int EnumJobs(int hPrinter, int firstJob, int noJobs, int level, ref IntPtr pJob, int cdBuf, ref int pcbNeeded, ref int pcReturned);

    [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool ClosePrinter(IntPtr hPrinter);
    
    //Get printer status
    public string GetStateStr(int usbPort)
    {
        var state = CyStat.GetStatus(usbPort);
        var stateStr = string.Empty;
        
        if ((state & CyStat.GROUP_USUALLY) != 0)
        {
            stateStr = state switch
            {
                CyStat.STATUS_USUALLY_IDLE => "Idle",
                CyStat.STATUS_USUALLY_PRINTING => "Printing",
                CyStat.STATUS_USUALLY_STANDSTILL => "STANDSTILL",
                CyStat.STATUS_USUALLY_PAPER_END => "Paper End",
                CyStat.STATUS_USUALLY_RIBBON_END => "Ribbon End",
                CyStat.STATUS_USUALLY_COOLING => "Head Cooling Down",
                CyStat.STATUS_USUALLY_MOTCOOLING => "Motor Cooling Down",
                _ => stateStr
            };
        }
        else if ((state & CyStat.GROUP_SETTING) != 0)
        {
            stateStr = state switch
            {
                CyStat.STATUS_SETTING_COVER_OPEN => "Cover Open",
                CyStat.STATUS_SETTING_PAPER_JAM => "Paper Jam",
                CyStat.STATUS_SETTING_RIBBON_ERR => "Ribbon Error",
                CyStat.STATUS_SETTING_PAPER_ERR => "Paper definition Error",
                CyStat.STATUS_SETTING_DATA_ERR => "Data Error",
                CyStat.STATUS_SETTING_SCRAPBOX_ERR => "Scrap Box Error",
                _ => stateStr
            };
        }
        else if ((state & CyStat.GROUP_HARDWARE) != 0)
        {
            stateStr = state switch
            {
                CyStat.STATUS_HARDWARE_ERR01 => "Head Voltage Error",
                CyStat.STATUS_HARDWARE_ERR02 => "Head Position Error",
                CyStat.STATUS_HARDWARE_ERR03 => "Fan Stop Error",
                CyStat.STATUS_HARDWARE_ERR04 => "Cutter Error",
                CyStat.STATUS_HARDWARE_ERR05 => "Pinch Roller Error",
                CyStat.STATUS_HARDWARE_ERR06 => "Illegal Head Temperature",
                CyStat.STATUS_HARDWARE_ERR07 => "Illegal Media Temperature",
                CyStat.STATUS_HARDWARE_ERR08 => "Ribbon Tension Error",
                CyStat.STATUS_HARDWARE_ERR09 => "RFID Module Error",
                CyStat.STATUS_HARDWARE_ERR10 => "Illegal Motor Temperature",
                _ => stateStr
            };
        }
        else if ((state & CyStat.GROUP_SYSTEM) != 0)
        {
            stateStr = "SYSTEM ERROR";
        }
        else if ((state & CyStat.GROUP_FLSHPROG) != 0)
        {
            stateStr = "FLSHPROG MODE";
        }
        else if (state < 0)
        {
            stateStr = " ---";
        }

        return stateStr;
    }
}