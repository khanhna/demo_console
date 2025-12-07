using System.Globalization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Demo_Console.Images;

public class CubeLutApplicator
{
    private float[,,][] lutData;
    private int lutSize;
    private float domainMin = 0f;
    private float domainMax = 1f;

    public void LoadCubeFile(string cubeFilePath)
    {
        var lines = File.ReadAllLines(cubeFilePath);
        var dataLines = new List<string>();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            
            // Skip comments and empty lines
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                continue;

            if (trimmed.StartsWith("LUT_3D_SIZE"))
            {
                lutSize = int.Parse(trimmed.Split(' ')[1]);
            }
            else if (trimmed.StartsWith("DOMAIN_MIN"))
            {
                var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                domainMin = float.Parse(parts[1], CultureInfo.InvariantCulture);
            }
            else if (trimmed.StartsWith("DOMAIN_MAX"))
            {
                var parts = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                domainMax = float.Parse(parts[1], CultureInfo.InvariantCulture);
            }
            else if (trimmed.StartsWith("TITLE") || trimmed.StartsWith("LUT_1D_SIZE"))
            {
                continue;
            }
            else
            {
                // This should be data
                var parts = trimmed.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3)
                {
                    dataLines.Add(trimmed);
                }
            }
        }

        // Initialize LUT data array
        lutData = new float[lutSize, lutSize, lutSize][];

        // Parse data lines
        int index = 0;
        for (int b = 0; b < lutSize; b++)
        {
            for (int g = 0; g < lutSize; g++)
            {
                for (int r = 0; r < lutSize; r++)
                {
                    if (index >= dataLines.Count)
                        throw new Exception($"Not enough data in cube file. Expected {lutSize * lutSize * lutSize} entries.");

                    var parts = dataLines[index].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    lutData[r, g, b] = new float[3];
                    lutData[r, g, b][0] = float.Parse(parts[0], CultureInfo.InvariantCulture);
                    lutData[r, g, b][1] = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    lutData[r, g, b][2] = float.Parse(parts[2], CultureInfo.InvariantCulture);
                    
                    index++;
                }
            }
        }
    }

    public void ApplyToImage(string inputPath, string outputPath)
    {
        using var image = Image.Load<Rgb24>(inputPath);
        
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < accessor.Height; y++)
            {
                Span<Rgb24> pixelRow = accessor.GetRowSpan(y);
                
                for (int x = 0; x < pixelRow.Length; x++)
                {
                    ref Rgb24 pixel = ref pixelRow[x];
                    
                    // Normalize to 0-1 range
                    float r = pixel.R / 255f;
                    float g = pixel.G / 255f;
                    float b = pixel.B / 255f;

                    // Apply LUT with trilinear interpolation
                    var result = TrilinearInterpolate(r, g, b);

                    // Convert back to 0-255 range and clamp
                    pixel.R = (byte)Clamp((int)(result[0] * 255f), 0, 255);
                    pixel.G = (byte)Clamp((int)(result[1] * 255f), 0, 255);
                    pixel.B = (byte)Clamp((int)(result[2] * 255f), 0, 255);
                }
            }
        });

        image.Save(outputPath);
    }

    private float[] TrilinearInterpolate(float r, float g, float b)
    {
        // Normalize input values to LUT space
        float rScaled = (r - domainMin) / (domainMax - domainMin) * (lutSize - 1);
        float gScaled = (g - domainMin) / (domainMax - domainMin) * (lutSize - 1);
        float bScaled = (b - domainMin) / (domainMax - domainMin) * (lutSize - 1);

        // Clamp to valid range
        rScaled = Math.Max(0, Math.Min(lutSize - 1, rScaled));
        gScaled = Math.Max(0, Math.Min(lutSize - 1, gScaled));
        bScaled = Math.Max(0, Math.Min(lutSize - 1, bScaled));

        // Get integer and fractional parts
        int r0 = (int)Math.Floor(rScaled);
        int g0 = (int)Math.Floor(gScaled);
        int b0 = (int)Math.Floor(bScaled);

        int r1 = Math.Min(r0 + 1, lutSize - 1);
        int g1 = Math.Min(g0 + 1, lutSize - 1);
        int b1 = Math.Min(b0 + 1, lutSize - 1);

        float rFrac = rScaled - r0;
        float gFrac = gScaled - g0;
        float bFrac = bScaled - b0;

        // Trilinear interpolation
        var result = new float[3];
        for (int i = 0; i < 3; i++)
        {
            float c000 = lutData[r0, g0, b0][i];
            float c100 = lutData[r1, g0, b0][i];
            float c010 = lutData[r0, g1, b0][i];
            float c110 = lutData[r1, g1, b0][i];
            float c001 = lutData[r0, g0, b1][i];
            float c101 = lutData[r1, g0, b1][i];
            float c011 = lutData[r0, g1, b1][i];
            float c111 = lutData[r1, g1, b1][i];

            float c00 = c000 * (1 - rFrac) + c100 * rFrac;
            float c01 = c001 * (1 - rFrac) + c101 * rFrac;
            float c10 = c010 * (1 - rFrac) + c110 * rFrac;
            float c11 = c011 * (1 - rFrac) + c111 * rFrac;

            float c0 = c00 * (1 - gFrac) + c10 * gFrac;
            float c1 = c01 * (1 - gFrac) + c11 * gFrac;

            result[i] = c0 * (1 - bFrac) + c1 * bFrac;
        }

        return result;
    }

    private int Clamp(int value, int min, int max)
    {
        return Math.Max(min, Math.Min(max, value));
    }
}