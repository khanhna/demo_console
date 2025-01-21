namespace Demo_Console.DevicesIntegration.Printer.DSRX;

public enum RotateType
{
    Clockwise90,
    Clockwise180,
    Clockwise270
}

public enum FlipType
{
    Horizontal,
    Vertical
}

public enum Ratio
{
    /// <summary>
    /// 3:2
    /// </summary>
    R1 = 1,
    /// <summary>
    /// 2:3
    /// </summary>
    R2 = 2,
    /// <summary>
    /// 4:3
    /// </summary>
    R3 = 3,
    /// <summary>
    /// 3:4
    /// </summary>
    R4 = 4,
    /// <summary>
    /// 5:4
    /// </summary>
    R5 = 5,
    /// <summary>
    /// 4:5
    /// </summary>
    R6 = 6,
}

public enum PrintQuality
{
    PQ1 = 600
}

public enum PaperSize
{
    PZ1 = 127
}

public enum YResolution
{
    YR1 = 600
}

public enum Sharpness
{
    SP1 = 2
}

public enum ICMMethod
{
    IM1 = 1
}

public enum Orientation
{
    OT1 = 2
}

public enum Cut2inch
{
    CI1 = 0,
    CT2 = 1
}
