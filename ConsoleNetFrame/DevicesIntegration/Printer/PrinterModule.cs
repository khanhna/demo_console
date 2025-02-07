using System;
using System.Runtime.InteropServices;

namespace ConsoleNetFrame.DevicesIntegration.Printer
{
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
        public static extern int EnumJobs(int hPrinter, int FirstJob, int NoJobs, int Level, ref IntPtr pJob, int cdBuf, ref int pcbNeeded, ref int pcReturned);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);
        //Get printer status
        public string GetStateStr(int UsbPort)
        {
            int state;
            string stateStr = "";

            state = CyStat.GetStatus(UsbPort);
            if ((state & CyStat.GROUP_USUALLY) != 0)
            {
                switch (state)
                {
                    case CyStat.STATUS_USUALLY_IDLE:
                        stateStr = "Idle";
                        break;
                    case CyStat.STATUS_USUALLY_PRINTING:
                        stateStr = "Printing";
                        break;
                    case CyStat.STATUS_USUALLY_STANDSTILL:
                        stateStr = "STANDSTILL";
                        break;
                    case CyStat.STATUS_USUALLY_PAPER_END:
                        stateStr = "Paper End";
                        break;
                    case CyStat.STATUS_USUALLY_RIBBON_END:
                        stateStr = "Ribbon End";
                        break;
                    case CyStat.STATUS_USUALLY_COOLING:
                        stateStr = "Head Cooling Down";
                        break;
                    case CyStat.STATUS_USUALLY_MOTCOOLING:
                        stateStr = "Motor Cooling Down";
                        break;
                }
            }
            else if ((state & CyStat.GROUP_SETTING) != 0)
            {
                switch (state)
                {
                    case CyStat.STATUS_SETTING_COVER_OPEN:
                        stateStr = "Cover Open";
                        break;
                    case CyStat.STATUS_SETTING_PAPER_JAM:
                        stateStr = "Paper Jam";
                        break;
                    case CyStat.STATUS_SETTING_RIBBON_ERR:
                        stateStr = "Ribbon Error";
                        break;
                    case CyStat.STATUS_SETTING_PAPER_ERR:
                        stateStr = "Paper definition Error";
                        break;
                    case CyStat.STATUS_SETTING_DATA_ERR:
                        stateStr = "Data Error";
                        break;
                    case CyStat.STATUS_SETTING_SCRAPBOX_ERR:
                        stateStr = "Scrap Box Error";
                        break;
                }
            }
            else if ((state & CyStat.GROUP_HARDWARE) != 0)
            {
                switch (state)
                {
                    case CyStat.STATUS_HARDWARE_ERR01:
                        stateStr = "Head Voltage Error";
                        break;
                    case CyStat.STATUS_HARDWARE_ERR02:
                        stateStr = "Head Position Error";
                        break;
                    case CyStat.STATUS_HARDWARE_ERR03:
                        stateStr = "Fan Stop Error";
                        break;
                    case CyStat.STATUS_HARDWARE_ERR04:
                        stateStr = "Cutter Error";
                        break;
                    case CyStat.STATUS_HARDWARE_ERR05:
                        stateStr = "Pinch Roller Error";
                        break;
                    case CyStat.STATUS_HARDWARE_ERR06:
                        stateStr = "Illegal Head Temperature";
                        break;
                    case CyStat.STATUS_HARDWARE_ERR07:
                        stateStr = "Illegal Media Temperature";
                        break;
                    case CyStat.STATUS_HARDWARE_ERR08:
                        stateStr = "Ribbon Tension Error";
                        break;
                    case CyStat.STATUS_HARDWARE_ERR09:
                        stateStr = "RFID Module Error";
                        break;
                    case CyStat.STATUS_HARDWARE_ERR10:
                        stateStr = "Illegal Motor Temperature";
                        break;
                }
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
}