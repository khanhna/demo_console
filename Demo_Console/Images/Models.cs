using OpenCvSharp;

namespace Demo_Console.Images;

public class ShootLayoutDetail
{
    public int Id { get; set; }
    public int LayoutId { get; set; }
    
    // These info is being take from Sql Join
    public int WidthRatio { get; set; }
    public int HeighRatio { get; set; }
    public int AxisX { get; set; }
    public int AxisY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    
    // These info is being added each shoot
    public string VideoName { get; set; } = string.Empty;
    public TimeSpan MarkedTime { get; set; }
    public int FrameUsed { get; set; }
    
    // These info only required by video processor process
    public int FrameStart { get; set; }
    public int FrameEnd { get; set; }

    public ShootLayoutDetail Clone() => new()
    {
        Id = Id,
        LayoutId = LayoutId,
        WidthRatio = WidthRatio,
        HeighRatio = HeighRatio,
        AxisX = AxisX,
        AxisY = AxisY,
        Width = Width,
        Height = Height,
        VideoName = VideoName,
        MarkedTime = MarkedTime,
        FrameUsed = FrameUsed,
        FrameStart = FrameStart,
        FrameEnd = FrameEnd
    };
}