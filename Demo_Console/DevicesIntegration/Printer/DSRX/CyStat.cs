using System.Runtime.InteropServices;
using System.Text;

namespace Demo_Console.DevicesIntegration.Printer.DSRX;

public class CyStat
{
    public const int GROUP_USUALLY = 0x10000;
    public const int GROUP_SETTING = 0x20000;
    public const int GROUP_HARDWARE = 0x40000;
    public const int GROUP_SYSTEM = 0x80000;
    public const int GROUP_FLSHPROG = 0x100000;

    public const int STATUS_ERROR = unchecked((int)0x80000000);

    public const int STATUS_USUALLY_IDLE = GROUP_USUALLY | 0x1;
    public const int STATUS_USUALLY_PRINTING = GROUP_USUALLY | 0x2;
    public const int STATUS_USUALLY_STANDSTILL = GROUP_USUALLY | 0x4;
    public const int STATUS_USUALLY_PAPER_END = GROUP_USUALLY | 0x8;
    public const int STATUS_USUALLY_RIBBON_END = GROUP_USUALLY | 0x10;
    public const int STATUS_USUALLY_COOLING = GROUP_USUALLY | 0x20;
    public const int STATUS_USUALLY_MOTCOOLING = GROUP_USUALLY | 0x40;

    public const int STATUS_SETTING_COVER_OPEN = GROUP_SETTING | 0x1;
    public const int STATUS_SETTING_PAPER_JAM = GROUP_SETTING | 0x2;
    public const int STATUS_SETTING_RIBBON_ERR = GROUP_SETTING | 0x4;
    public const int STATUS_SETTING_PAPER_ERR = GROUP_SETTING | 0x8;
    public const int STATUS_SETTING_DATA_ERR = GROUP_SETTING | 0x10;
    public const int STATUS_SETTING_SCRAPBOX_ERR = GROUP_SETTING | 0x20;

    public const int STATUS_HARDWARE_ERR01 = GROUP_HARDWARE | 0x1;
    public const int STATUS_HARDWARE_ERR02 = GROUP_HARDWARE | 0x2;
    public const int STATUS_HARDWARE_ERR03 = GROUP_HARDWARE | 0x4;
    public const int STATUS_HARDWARE_ERR04 = GROUP_HARDWARE | 0x8;
    public const int STATUS_HARDWARE_ERR05 = GROUP_HARDWARE | 0x10;
    public const int STATUS_HARDWARE_ERR06 = GROUP_HARDWARE | 0x20;
    public const int STATUS_HARDWARE_ERR07 = GROUP_HARDWARE | 0x40;
    public const int STATUS_HARDWARE_ERR08 = GROUP_HARDWARE | 0x80;
    public const int STATUS_HARDWARE_ERR09 = GROUP_HARDWARE | 0x100;
    public const int STATUS_HARDWARE_ERR10 = GROUP_HARDWARE | 0x200;

    public const int STATUS_SYSTEM_ERR01 = GROUP_SYSTEM | 0x1;

    public const int STATUS_FLSHPROG_IDLE = GROUP_SYSTEM | 0x1;
    public const int STATUS_FLSHPROG_WRITING = GROUP_SYSTEM | 0x2;
    public const int STATUS_FLSHPROG_FINISHED = GROUP_SYSTEM | 0x4;
    public const int STATUS_FLSHPROG_DATA_ERR1 = GROUP_SYSTEM | 0x8;
    public const int STATUS_FLSHPROG_DEVICE_ERR1 = GROUP_SYSTEM | 0x10;
    public const int STATUS_FLSHPROG_OTHERS_ERR1 = GROUP_SYSTEM | 0x20;

    //For CV Printers Item
    public const int CVG_USUALLY = 0x10000;
    public const int CVG_SETTING = 0x20000;
    public const int CVG_HARDWARE = 0x40000;
    public const int CVG_SYSTEM = 0x80000;
    public const int CVG_FLSHPROG = 0x100000;

    public const int CVSTATUS_ERROR = unchecked((int)0x80000000);

    public const int CVS_USUALLY_IDLE = CVG_USUALLY | 0x1;
    public const int CVS_USUALLY_PRINTING = CVG_USUALLY | 0x2;
    public const int CVS_USUALLY_STANDSTILL = CVG_USUALLY | 0x4;
    public const int CVS_USUALLY_PAPER_END = CVG_USUALLY | 0x8;
    public const int CVS_USUALLY_RIBBON_END = CVG_USUALLY | 0x10;
    public const int CVS_USUALLY_COOLING = CVG_USUALLY | 0x20;
    public const int CVS_USUALLY_MOTCOOLING = CVG_USUALLY | 0x40;

    public const int CVS_SETTING_COVER_OPEN = CVG_SETTING | 0x1;
    public const int CVS_SETTING_PAPER_JAM = CVG_SETTING | 0x2;
    public const int CVS_SETTING_RIBBON_ERR = CVG_SETTING | 0x4;
    public const int CVS_SETTING_PAPER_ERR = CVG_SETTING | 0x8;
    public const int CVS_SETTING_DATA_ERR = CVG_SETTING | 0x10;
    public const int CVS_SETTING_SCRAP_ERR = CVG_SETTING | 0x20;

    public const int CVS_HARDWARE_ERR01 = CVG_HARDWARE | 0x1;
    public const int CVS_HARDWARE_ERR02 = CVG_HARDWARE | 0x2;
    public const int CVS_HARDWARE_ERR03 = CVG_HARDWARE | 0x4;
    public const int CVS_HARDWARE_ERR04 = CVG_HARDWARE | 0x8;
    public const int CVS_HARDWARE_ERR05 = CVG_HARDWARE | 0x10;
    public const int CVS_HARDWARE_ERR06 = CVG_HARDWARE | 0x20;
    public const int CVS_HARDWARE_ERR07 = CVG_HARDWARE | 0x40;
    public const int CVS_HARDWARE_ERR08 = CVG_HARDWARE | 0x80;
    public const int CVS_HARDWARE_ERR09 = CVG_HARDWARE | 0x100;
    public const int CVS_HARDWARE_ERR10 = CVG_HARDWARE | 0x200;

    public const int CVS_SYSTEM_ERR01 = CVG_SYSTEM | 0x1;

    public const int CVS_FLSHPROG_IDLE = CVG_FLSHPROG | 0x1;
    public const int CVS_FLSHPROG_WRITING = CVG_FLSHPROG | 0x2;
    public const int CVS_FLSHPROG_FINISHED = CVG_FLSHPROG | 0x4;
    public const int CVS_FLSHPROG_DATA_ERR1 = CVG_FLSHPROG | 0x8;
    public const int CVS_FLSHPROG_DEVICE_ERR1 = CVG_FLSHPROG | 0x10;
    public const int CVS_FLSHPROG_OTHERS_ERR1 = CVG_FLSHPROG | 0x20;

    public const short CUTTER_MODE_STANDARD = 0x0;
    public const short CUTTER_MODE_NONSCRAP = 0x1;
    public const short CUTTER_MODE_2INCHCUT = 0x78;

    [DllImport("CyStat64.dll", CharSet = CharSet.Unicode)]
    public static extern int CvInitialize([MarshalAs(UnmanagedType.LPWStr)] string pszPortName);
    [DllImport("CyStat64.dll")]
    public static extern int CvGetVersion(int lPortNum, string s);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetSensorInfo(int lPortNum, string s);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetMedia(int lPortNum, string s);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetStatus(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetPQTY(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetCounterL(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetCounterA(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetCounterB(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetMediaCounter(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetMediaColorOffset(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetMediaLotNo(int lPortNum, string s);

    [DllImport("CyStat64.dll", EntryPoint = "CvSetColorDataVersion", CharSet = CharSet.Ansi)]
    public static extern int CvSetColorDataVersion_(int lPortNum, IntPtr lpBuff, int bLen);

    [DllImport("CyStat64.dll", EntryPoint = "SetColorDataWrite", CharSet = CharSet.Ansi)]
    public static extern int CvSetColorDataWrite_(int lPortNum, IntPtr lpBuff, int bLen);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetColorDataVersion(int lPortNum, string s);

    [DllImport("CyStat64.dll")]
    public static extern int CvSetColorDataClear(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetColorDataChecksum(int lPortNum, string s);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetSerialNo(int lPortNum, string s);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetResolutionH(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetResolutionV(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvGetFreeBuffer(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvSetClearCounterA(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvSetClearCounterB(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int CvSetFirmwUpdateMode(int lPortNum);

    [DllImport("CyStat64.dll", EntryPoint = "SetFirmwDataWrite", CharSet = CharSet.Ansi)]
    public static extern int CvSetFirmwDataWrite_(int lPortNum, IntPtr lpBuff, int bLen);

    [DllImport("CyStat64.dll", EntryPoint = "CvSetCommand", CharSet = CharSet.Ansi)]
    public static extern int CvSetCommand_(int lPortNum, IntPtr lpCmd, int dwCmdLen);

    [DllImport("CyStat64.dll", EntryPoint = "CvGetCommand", CharSet = CharSet.Ansi)]
    public static extern int CvGetCommand_(int lPortNum, IntPtr lpCmd, int dwCmdLen, StringBuilder lpRetBuff, int dwRetBuffSize, ref int lpdwRetLen);

    [DllImport("CyStat64.dll", EntryPoint = "CvGetCommandEX", CharSet = CharSet.Ansi)]
    public static extern int CvGetCommandEX(int lPortNum, IntPtr lpCmd, int dwCmdLen, StringBuilder lpRetBuff, int dwRetBuffSize);

    [DllImport("CyStat64.dll")]
    public static extern int PortInitialize([MarshalAs(UnmanagedType.LPWStr)] string pszPortName);

    [DllImport("CyStat64.dll")]
    public static extern int GetFirmwVersion(int lPortNum, StringBuilder s);

    [DllImport("CyStat64.dll")]
    public static extern int GetSensorInfo(int lPortNum, StringBuilder s);

    [DllImport("CyStat64.dll")]
    public static extern int GetMedia(int lPortNum, StringBuilder s);

    [DllImport("CyStat64.dll")]
    public static extern int GetStatus(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetPQTY(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetCounterL(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetCounterA(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetCounterB(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetCounterP(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetCounterMatte(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetCounterM(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetMediaCounter(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetMediaCounter_R(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetMediaColorOffset(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetMediaLotNo(int lPortNum, StringBuilder s);

    [DllImport("CyStat64.dll")]
    public static extern int GetMediaIdSetInfo(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int SetColorDataVersion_(int lPortNum, IntPtr lpBuff, int bLen);

    [DllImport("CyStat64.dll", EntryPoint = "SetColorDataWrite")]
    public static extern int SetColorDataWrite_(int lPortNum, IntPtr lpBuff, int bLen);

    [DllImport("CyStat64.dll")]
    public static extern int GetColorDataVersion(int lPortNum, StringBuilder s);

    [DllImport("CyStat64.dll")]
    public static extern int SetColorDataClear(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetColorDataChecksum(int lPortNum, string s);

    [DllImport("CyStat64.dll")]
    public static extern int GetSerialNo(int lPortNum, string s);

    [DllImport("CyStat64.dll")]
    public static extern int GetResolutionH(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetResolutionV(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetFreeBuffer(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int SetClearCounterA(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int SetClearCounterB(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int SetClearCounterM(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int SetCounterP(int lPortNum, int Counter);

    [DllImport("CyStat64.dll")]
    public static extern int SetFirmwUpdateMode(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int SetFirmwDataWrite_(int lPortNum, IntPtr lpBuff, int bLen);

    [DllImport("CyStat64.dll")]
    public static extern int SetCutterMode(int lPortNum, int d);

    [DllImport("CyStat64.dll")]
    public static extern int SetCommand_(int lPortNum, IntPtr lpCmd, int dwCmdLen);

    [DllImport("CyStat64.dll")]
    public static extern int GetCommand_(int lPortNum, IntPtr lpCmd, int dwCmdLen, ref StringBuilder lpRetBuff, int dwRetBuffSize, out IntPtr lpdwRetLen);

    [DllImport("CyStat64.dll")]
    public static extern int GetCommandEX_(int lPortNum, IntPtr lpCmd, int dwCmdLen, StringBuilder lpRetBuff, int dwRetBuffSize);

    [DllImport("CyStat64.dll")]
    public static extern int GetRfidMediaClass(int lPortNum, string s);

    [DllImport("CyStat64.dll")]
    public static extern int GetRfidReserveData(int lPortNum, string s, int dwPage);

    [DllImport("CyStat64.dll")]
    public static extern int GetInitialMediaCount(int lPortNum);

    [DllImport("CyStat64.dll")]
    public static extern int GetMediaCountOffset(int lPortNum);

    public int SetCommand(int lPortNum, string cmd, int dwCmdLen)
    {
        var byteArray = Encoding.GetEncoding(65001).GetBytes(cmd);
        var gchCmd = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
        var lpCmd = gchCmd.AddrOfPinnedObject().ToInt64();
        var myIntPtrValue = new IntPtr(lpCmd);
        var result = SetCommand_(lPortNum, myIntPtrValue, dwCmdLen);
        gchCmd.Free();
        return result;
    }

    public int GetCommand(int lPortNum, string cmd, int dwCmdLen, ref StringBuilder rb, int dwRetBuffSize,
        ref int dwRetLen)
    {
        var byteArray = System.Text.Encoding.GetEncoding(65001).GetBytes(cmd);
        var gchCmd = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
        var gchRetLen = GCHandle.Alloc(dwRetLen, GCHandleType.Pinned);
        var lpCmd = gchCmd.AddrOfPinnedObject();
        var lpdwRetLen = gchCmd.AddrOfPinnedObject();
        var result = GetCommand_(lPortNum, lpCmd, dwCmdLen, ref rb, dwRetBuffSize, out lpdwRetLen);

        gchCmd.Free();
        gchRetLen.Free();
        return result;
    }

    public int GetCommandEX(int lPortNum, string cmd, int dwCmdLen, ref StringBuilder rb, int dwRetBuffSize)
    {
        var byteArray = System.Text.Encoding.GetEncoding(65001).GetBytes(cmd);
        var gchCmd = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
        var lpCmd = gchCmd.AddrOfPinnedObject();
        var result = GetCommandEX_(lPortNum, lpCmd, dwCmdLen, rb, dwRetBuffSize);
        gchCmd.Free();
        return result;
    }

    public int SetFirmwDataWrite(int lPortNum, byte[] byteArray, int dwLen)
    {
        var gchCmd = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
        var lpCmd = gchCmd.AddrOfPinnedObject();
        var result = SetFirmwDataWrite_(lPortNum, lpCmd, dwLen);
        gchCmd.Free();
        return result;
    }

    public static int SetColorDataVersion(int lPortNum, string cmd, int dwCmdLen)
    {
        var byteArray = Encoding.GetEncoding(65001).GetBytes(cmd);
        var gchCmd = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
        var lpCmd = gchCmd.AddrOfPinnedObject();
        var result = SetColorDataVersion_(lPortNum, lpCmd, dwCmdLen);
        gchCmd.Free();
        return result;
    }

    public static int SetColorDataWrite(int lPortNum, byte[] byteArray, int dwLen)
    {
        var gchCmd = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
        var lpCmd = gchCmd.AddrOfPinnedObject();
        var result = SetColorDataWrite_(lPortNum, lpCmd, dwLen);
        gchCmd.Free();
        return result;
    }

}