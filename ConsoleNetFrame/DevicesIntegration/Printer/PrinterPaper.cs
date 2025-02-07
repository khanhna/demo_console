using System;
using System.Runtime.InteropServices;

namespace ConsoleNetFrame.DevicesIntegration.Printer
{
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
        public static extern void MoveMemory(string Destination, IntPtr Source, int Length);

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
            string strPrnName = DevMode.PrinterName;
            string strPortName = PrinterPort.GetPortName(ref strPrnName);
            int lngBuffSize = DeviceCapabilities(strPrnName, strPortName, DC_PAPERNAMES, IntPtr.Zero, IntPtr.Zero);
            byte[] bytPaperName = new byte[lngBuffSize * 64];

            GCHandle gchPn = GCHandle.Alloc(bytPaperName, GCHandleType.Pinned);
            int lpPn = gchPn.AddrOfPinnedObject().ToInt32();
            int lngRetCode = DeviceCapabilities(strPrnName, strPortName, DC_PAPERNAMES,new IntPtr(lpPn), IntPtr.Zero);
            gchPn.Free();

            string strTempBuffer;
            byte b;
            for (int n = 0; n < lngBuffSize; n++)
            {
                strTempBuffer = "";
                for (int m = 0; m < 64; m++)
                {
                    b = bytPaperName[(n * 64) + m];
                    if (b == 0)
                    {
                        break;
                    }
                    strTempBuffer += (char)b;
                }
                PaperSize[n].Name = strTempBuffer;
            }
        }

        //Get paper size number
        public static void GetPaperSizeNumber()
        {
            string strPrnName = DevMode.PrinterName;
            string strPortName = PrinterPort.GetPortName(ref strPrnName);
            short[] intOutPut;
            int lngBuffSize;
            int lngRetCode;
            int n;

            lngBuffSize = DeviceCapabilities(strPrnName, strPortName, DC_PAPERS, IntPtr.Zero, IntPtr.Zero);

            intOutPut = new short[lngBuffSize];

            GCHandle gchPn = GCHandle.Alloc(intOutPut, GCHandleType.Pinned);
            int lpPn = gchPn.AddrOfPinnedObject().ToInt32();
            lngRetCode = DeviceCapabilities(strPrnName, strPortName, DC_PAPERS, new IntPtr(lpPn), IntPtr.Zero);
            gchPn.Free();

            for (n = 0; n < lngBuffSize; n++)
            {
                PaperSize[n].Number = intOutPut[n];
            }
        }

        // Set multi cut number
        public static void MultiCutNum()
        {
            for (int n = 0; n <= PrinterModule.MediaNumMax - 1; n++)
            {
                PaperSize[n].MultiCut = 1; // clear

                switch (PaperSize[n].Name)
                {
                    case "(5x3.5)":
                    case "(5x7)":
                    case "(6x4)":
                    case "(6x8)":
                    case "(6x9)":
                    case "PR (4x6)":
                    case "PR (3.5x5)":
                        PaperSize[n].MultiCut = 1;
                        break;

                    case "(6x4) x 2":
                    case "PR (4x6) x 2":
                    case "(5x3.5) x 2":
                    case "PR (3.5x5) x 2":
                        PaperSize[n].MultiCut = 2;
                        break;

                    default:
                        PaperSize[n].MultiCut = 1;
                        break;
                }
            }
        }

    }
}