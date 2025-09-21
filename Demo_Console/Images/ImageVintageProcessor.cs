using System.Numerics;
using Demo_Console.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

namespace Demo_Console.Images;

/// <summary>
/// Vintage Film Effect processor for applying film-style effects to images
/// </summary>
public class VintageProcessor
{
    private readonly Random _random = new();

    /// <summary>
    /// Add film grain noise to the image
    /// </summary>
    public void AddGrain(Image<Rgba32> image, float intensity = 0.1f)
    {
        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                for (var x = 0; x < pixelRow.Length; x++)
                {
                    ref var pixel = ref pixelRow[x];
                    
                    // Generate noise for each channel
                    var noiseR = (float)(_random.NextGaussian() * intensity * 255);
                    var noiseG = (float)(_random.NextGaussian() * intensity * 255);
                    var noiseB = (float)(_random.NextGaussian() * intensity * 255);
                    
                    // Apply noise
                    pixel.R = (byte)Math.Clamp(pixel.R + noiseR, 0, 255);
                    pixel.G = (byte)Math.Clamp(pixel.G + noiseG, 0, 255);
                    pixel.B = (byte)Math.Clamp(pixel.B + noiseB, 0, 255);
                }
            }
        });
    }

    /// <summary>
    /// Add vignette (darkened edges) effect
    /// </summary>
    public void AddVignette(Image<Rgba32> image, float intensity = 0.5f)
    {
        var width = image.Width;
        var height = image.Height;
        var centerX = width / 2f;
        var centerY = height / 2f;
        var maxRadius = MathF.Sqrt(centerX * centerX + centerY * centerY);

        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                for (var x = 0; x < pixelRow.Length; x++)
                {
                    ref var pixel = ref pixelRow[x];
                    
                    // Calculate distance from center
                    var dx = x - centerX;
                    var dy = y - centerY;
                    var distance = MathF.Sqrt(dx * dx + dy * dy);
                    var normalizedDistance = distance / maxRadius;
                    
                    // Create vignette mask
                    var vignette = Math.Clamp(1 - (normalizedDistance * normalizedDistance) * intensity, 0, 1);
                    
                    // Apply vignette
                    pixel.R = (byte)(pixel.R * vignette);
                    pixel.G = (byte)(pixel.G * vignette);
                    pixel.B = (byte)(pixel.B * vignette);
                }
            }
        });
    }

    /// <summary>
    /// Lift the blacks to create a faded film look
    /// </summary>
    public void LiftBlacks(Image<Rgba32> image, float amount = 0.1f)
    {
        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                for (var x = 0; x < pixelRow.Length; x++)
                {
                    ref var pixel = ref pixelRow[x];
                    
                    // Lift blacks formula: pixel = pixel * (1 - amount) + amount
                    pixel.R = Convert.ToByte(Math.Clamp((pixel.R / 255f) * (1 - amount) + amount, 0, 1) * 255);
                    pixel.G = Convert.ToByte(Math.Clamp((pixel.G / 255f) * (1 - amount) + amount, 0, 1) * 255);
                    pixel.B = Convert.ToByte(Math.Clamp((pixel.B / 255f) * (1 - amount) + amount, 0, 1) * 255);
                }
            }
        });
    }

    /// <summary>
    /// Apply color grading/tinting to the image
    /// </summary>
    public void ApplyColorGrading(Image<Rgba32> image, Vector3 tintColor, float intensity = 0.3f)
    {
        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                for (var x = 0; x < pixelRow.Length; x++)
                {
                    ref var pixel = ref pixelRow[x];
                    
                    // Apply tint
                    pixel.R = (byte)Math.Clamp(pixel.R * (1 - intensity) + tintColor.X * intensity, 0, 255);
                    pixel.G = (byte)Math.Clamp(pixel.G * (1 - intensity) + tintColor.Y * intensity, 0, 255);
                    pixel.B = (byte)Math.Clamp(pixel.B * (1 - intensity) + tintColor.Z * intensity, 0, 255);
                }
            }
        });
    }
    
    /// <summary>
    /// Adjust RGB channels independently
    /// </summary>
    public void AdjustRGBChannels(Image<Rgba32> image, float redMultiplier = 1.0f, float greenMultiplier = 1.0f, float blueMultiplier = 1.0f)
    {
        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                for (var x = 0; x < pixelRow.Length; x++)
                {
                    ref var pixel = ref pixelRow[x];
                    
                    // Apply channel multipliers
                    pixel.R = (byte)Math.Clamp(pixel.R * redMultiplier, 0, 255);
                    pixel.G = (byte)Math.Clamp(pixel.G * greenMultiplier, 0, 255);
                    pixel.B = (byte)Math.Clamp(pixel.B * blueMultiplier, 0, 255);
                }
            }
        });
    }

    /// <summary>
    /// Adjust overall brightness of the image
    /// </summary>
    public void AdjustBrightness(Image<Rgba32> image, float brightness = 1.0f)
    {
        image.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);
                for (var x = 0; x < pixelRow.Length; x++)
                {
                    ref var pixel = ref pixelRow[x];
                    
                    // Apply brightness adjustment
                    pixel.R = (byte)Math.Clamp(pixel.R * brightness, 0, 255);
                    pixel.G = (byte)Math.Clamp(pixel.G * brightness, 0, 255);
                    pixel.B = (byte)Math.Clamp(pixel.B * brightness, 0, 255);
                }
            }
        });
    }

    /// <summary>
    /// Calculate probability of artifacts based on distance from center
    /// </summary>
    private float GetEdgeProbability(int x, int y, int width, int height, float centerClearRatio = 0.7f, float minProb = 0.2f, float maxProb = 1.0f)
    {
        var centerX = width / 2f;
        var centerY = height / 2f;
        var maxDistance = MathF.Sqrt(centerX * centerX + centerY * centerY);
        var distance = MathF.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
        var normalizedDistance = distance / maxDistance;

        if (normalizedDistance < centerClearRatio * 0.5f)
        {
            return minProb * 0.5f; // Very center - minimum probability
        }
        else if (normalizedDistance < centerClearRatio)
        {
            // Transition zone
            var transitionFactor = (normalizedDistance - centerClearRatio * 0.5f) / (centerClearRatio * 0.5f);
            return minProb * 0.5f + (minProb * 0.5f) * transitionFactor;
        }
        else
        {
            // Edge zone
            var edgeFactor = (normalizedDistance - centerClearRatio) / (1 - centerClearRatio);
            return minProb + (maxProb - minProb) * edgeFactor;
        }
    }

    /// <summary>
    /// Determine if an artifact should be placed at given coordinates
    /// </summary>
    private bool ShouldPlaceArtifact(int x, int y, int width, int height)
    {
        var probability = GetEdgeProbability(x, y, width, height);
        return _random.NextSingle() < probability;
    }

    /// <summary>
    /// Generate a position that's biased toward edges
    /// </summary>
    private Point GetEdgeBiasedPosition(int width, int height, float edgeBias = 0.65f)
    {
        if (_random.NextSingle() < edgeBias)
        {
            // Choose edge-biased position
            var edges = new[] { "top", "bottom", "left", "right", "corner" };
            var edge = edges[_random.Next(edges.Length)];

            return edge switch
            {
                "top" => new Point(_random.Next(width), _random.Next(height / 4)),
                "bottom" => new Point(_random.Next(width), _random.Next(3 * height / 4, height)),
                "left" => new Point(_random.Next(width / 4), _random.Next(height)),
                "right" => new Point(_random.Next(3 * width / 4, width), _random.Next(height)),
                "corner" => GetCornerPosition(width, height),
                _ => new Point(_random.Next(width), _random.Next(height))
            };
        }
        else
        {
            // Random position
            return new Point(_random.Next(width), _random.Next(height));
        }
    }

    private Point GetCornerPosition(int width, int height)
    {
        var corners = new[] { "top-left", "top-right", "bottom-left", "bottom-right" };
        var corner = corners[_random.Next(corners.Length)];

        return corner switch
        {
            "top-left" => new Point(_random.Next(width / 3), _random.Next(height / 3)),
            "top-right" => new Point(_random.Next(2 * width / 3, width), _random.Next(height / 3)),
            "bottom-left" => new Point(_random.Next(width / 3), _random.Next(2 * height / 3, height)),
            "bottom-right" => new Point(_random.Next(2 * width / 3, width), _random.Next(2 * height / 3, height)),
            _ => new Point(width / 2, height / 2)
        };
    }

    /// <summary>
    /// Add a single scratch line to the image with edge-biased placement
    /// </summary>
    private void AddScratch(Image<Rgba32> scratchLayer, int colorMin = 160, int colorMax = 200)
    {
        var width = scratchLayer.Width;
        var height = scratchLayer.Height;
        
        // Get edge-biased starting position
        var startPos = GetEdgeBiasedPosition(width, height, 0.65f);

        // Calculate scratch length (0.5% to 3% of image height)
        var minLength = (int)(height * 0.005);
        var maxLength = (int)(height * 0.03);
        var scratchLength = _random.Next(minLength, maxLength);

        var scratchTypes = new[] { "vertical", "horizontal", "diagonal" };
        var scratchType = scratchTypes[_random.Next(scratchTypes.Length)];

        var points = new List<PointF>();

        switch (scratchType)
        {
            case "vertical":
                {
                    var direction = _random.Next(2) == 0 ? -1 : 1;
                    var endY = Math.Clamp(startPos.Y + direction * scratchLength, 0, height - 1);
                    var endX = Math.Clamp(startPos.X + _random.Next(-20, 21), 0, width - 1);

                    var numPoints = _random.Next(5, 16);
                    for (int i = 0; i < numPoints; i++)
                    {
                        var t = i / (float)(numPoints - 1);
                        var x = Math.Clamp(startPos.X + t * (endX - startPos.X) + _random.Next(-10, 11), 0, width - 1);
                        var y = startPos.Y + t * (endY - startPos.Y);

                        if (ShouldPlaceArtifact((int)x, (int)y, width, height))
                        {
                            points.Add(new PointF(x, y));
                        }
                    }
                    break;
                }
            case "horizontal":
                {
                    var direction = _random.Next(2) == 0 ? -1 : 1;
                    var endX = Math.Clamp(startPos.X + direction * scratchLength, 0, width - 1);
                    var endY = Math.Clamp(startPos.Y + _random.Next(-20, 21), 0, height - 1);

                    var numPoints = _random.Next(5, 11);
                    for (int i = 0; i < numPoints; i++)
                    {
                        var t = i / (float)(numPoints - 1);
                        var x = startPos.X + t * (endX - startPos.X);
                        var y = Math.Clamp(startPos.Y + t * (endY - startPos.Y) + _random.Next(-5, 6), 0, height - 1);

                        if (ShouldPlaceArtifact((int)x, (int)y, width, height))
                        {
                            points.Add(new PointF(x, y));
                        }
                    }
                    break;
                }
            case "diagonal":
                {
                    var angle = _random.NextSingle() * 2 * MathF.PI;
                    var endX = Math.Clamp(startPos.X + (int)(scratchLength * MathF.Cos(angle)), 0, width - 1);
                    var endY = Math.Clamp(startPos.Y + (int)(scratchLength * MathF.Sin(angle)), 0, height - 1);

                    if (ShouldPlaceArtifact(startPos.X, startPos.Y, width, height))
                        points.Add(new PointF(startPos.X, startPos.Y));
                    if (ShouldPlaceArtifact(endX, endY, width, height))
                        points.Add(new PointF(endX, endY));
                    break;
                }
        }

        // Draw the scratch segments
        if (points.Count > 1)
        {
            var brightness = _random.Next(colorMin, colorMax + 1);
            var color = Color.FromRgb((byte)brightness, (byte)brightness, (byte)brightness);
            
            for (var i = 0; i < points.Count - 1; i++)
            {
                scratchLayer.Mutate(ctx => ctx.DrawLine(color, 1, points[i], points[i + 1]));
            }
        }
    }

    /// <summary>
    /// Add hair or fiber-like artifacts with edge-biased placement
    /// </summary>
    private void AddHairFiber(Image<Rgba32> scratchLayer)
    {
        var width = scratchLayer.Width;
        var height = scratchLayer.Height;
        
        // Get edge-biased starting position
        var startPos = GetEdgeBiasedPosition(width, height, 0.75f);

        var numSegments = _random.Next(10, 31);
        var points = new List<PointF> { new(startPos.X, startPos.Y) };

        var angle = _random.NextSingle() * 2 * MathF.PI;
        var curveStrength = _random.NextSingle() * 0.2f + 0.1f;

        for (var i = 0; i < numSegments; i++)
        {
            // Add randomness to angle
            angle += (_random.NextSingle() - 0.5f) * 2 * curveStrength;

            // Step size
            var step = _random.Next(5, 16);

            // Calculate next point
            var nextX = Math.Clamp(points[^1].X + step * MathF.Cos(angle), 0, width - 1);
            var nextY = Math.Clamp(points[^1].Y + step * MathF.Sin(angle), 0, height - 1);

            // Only add point if it passes the edge probability check
            if (ShouldPlaceArtifact((int)nextX, (int)nextY, width, height))
            {
                points.Add(new PointF(nextX, nextY));
            }
            else
            {
                break; // Break the fiber if we move too far into center
            }
        }

        // Draw the hair/fiber segments
        if (points.Count > 1)
        {
            var brightness = _random.Next(160, 201);
            var color = Color.FromRgb((byte)brightness, (byte)brightness, (byte)brightness);
            
            for (var i = 0; i < points.Count - 1; i++)
            {
                scratchLayer.Mutate(ctx => ctx.DrawLine(color, 1, points[i], points[i + 1]));
            }
        }
    }

    /// <summary>
    /// Add a cluster of dust particles
    /// </summary>
    private void AddDustCluster(Image<Rgba32> image, int centerX, int centerY, int radius = 15, float density = 0.2f)
    {
        var width = image.Width;
        var height = image.Height;
        var particleCount = (int)(radius * radius * density);

        for (var i = 0; i < particleCount; i++)
        {
            var angle = _random.NextSingle() * 2 * MathF.PI;
            var r = _random.NextSingle() * radius;

            var x = (int)(centerX + r * MathF.Cos(angle));
            var y = (int)(centerY + r * MathF.Sin(angle));

            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                if (!ShouldPlaceArtifact(x, y, width, height))
                    continue;

                var brightness = _random.Next(140, 181);
                var size = _random.Next(1, 3);

                // Apply dust particle with subtle blending
                for (int dx = -size / 2; dx <= size / 2; dx++)
                {
                    for (int dy = -size / 2; dy <= size / 2; dy++)
                    {
                        var px = x + dx;
                        var py = y + dy;
                        
                        if (px >= 0 && px < width && py >= 0 && py < height)
                        {
                            var blendFactor = _random.NextSingle() * 0.2f + 0.1f;
                            
                            image.ProcessPixelRows(accessor =>
                            {
                                var pixel = accessor.GetRowSpan(py)[px];
                                pixel.R = (byte)(pixel.R * (1 - blendFactor) + brightness * blendFactor);
                                pixel.G = (byte)(pixel.G * (1 - blendFactor) + brightness * blendFactor);
                                pixel.B = (byte)(pixel.B * (1 - blendFactor) + brightness * blendFactor);
                                accessor.GetRowSpan(py)[px] = pixel;
                            });
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Add realistic dust, scratches, and hair/fiber artifacts focused on edges
    /// </summary>
    public void AddDustScratches(Image<Rgba32> image, float density = 0.0003f, int scratchCount = 3, int hairCount = 2)
    {
        var width = image.Width;
        var height = image.Height;

        // Add individual dust particles
        var numSpecs = (int)(width * height * density);
        for (int i = 0; i < numSpecs; i++)
        {
            var x = _random.Next(width);
            var y = _random.Next(height);

            if (!ShouldPlaceArtifact(x, y, width, height))
                continue;

            var brightness = _random.Next(130, 171);
            var rBrightness = Math.Clamp(brightness + _random.Next(-10, 11), 0, 255);
            var gBrightness = Math.Clamp(brightness + _random.Next(-10, 11), 0, 255);
            var bBrightness = Math.Clamp(brightness + _random.Next(-10, 11), 0, 255);

            var blendFactor = _random.NextSingle() * 0.2f + 0.15f;

            image.ProcessPixelRows(accessor =>
            {
                var pixel = accessor.GetRowSpan(y)[x];
                pixel.R = (byte)(pixel.R * (1 - blendFactor) + rBrightness * blendFactor);
                pixel.G = (byte)(pixel.G * (1 - blendFactor) + gBrightness * blendFactor);
                pixel.B = (byte)(pixel.B * (1 - blendFactor) + bBrightness * blendFactor);
                accessor.GetRowSpan(y)[x] = pixel;
            });
        }

        // Add dust clusters
        var numClusters = _random.Next(1, 5);
        for (var i = 0; i < numClusters; i++)
        {
            var clusterPos = GetEdgeBiasedPosition(width, height, 0.75f);
            var clusterRadius = _random.Next(8, 21);
            AddDustCluster(image, clusterPos.X, clusterPos.Y, clusterRadius, 0.25f);
        }

        // Create scratch layer
        using var scratchLayer = new Image<Rgba32>(width, height, Color.Transparent);

        // Add scratches
        for (var i = 0; i < scratchCount; i++)
        {
            AddScratch(scratchLayer, 160, 200);
        }

        // Add hair/fiber artifacts
        for (var i = 0; i < hairCount; i++)
        {
            AddHairFiber(scratchLayer);
        }

        // Blend scratch layer with main image
        image.ProcessPixelRows(scratchLayer, (mainAccessor, scratchAccessor) =>
        {
            for (var y = 0; y < mainAccessor.Height; y++)
            {
                var mainRow = mainAccessor.GetRowSpan(y);
                var scratchRow = scratchAccessor.GetRowSpan(y);

                for (var x = 0; x < mainRow.Length; x++)
                {
                    ref var mainPixel = ref mainRow[x];
                    ref readonly var scratchPixel = ref scratchRow[x];

                    if (scratchPixel.A > 0) // If scratch layer has content
                    {
                        // Blend with 40% scratch, 60% original (more subtle)
                        mainPixel.R = (byte)(mainPixel.R * 0.6f + scratchPixel.R * 0.4f);
                        mainPixel.G = (byte)(mainPixel.G * 0.6f + scratchPixel.G * 0.4f);
                        mainPixel.B = (byte)(mainPixel.B * 0.6f + scratchPixel.B * 0.4f);
                    }
                }
            }
        });
    }

    /// <summary>
    /// Apply all vintage effects to an image
    /// </summary>
    public async Task ApplyVintageEffectAsync(
        string imagePath,
        string outputPath,
        float grain = 0.05f,
        float vignette = 0.4f,
        float fade = 0.15f,
        float tintIntensity = 0.25f,
        float dust = 0.0003f,
        int scratches = 3,
        int hairs = 2,
        float blur = 0.5f)
    {
        using var image = await Image.LoadAsync<Rgba32>(imagePath);
    
        // Apply slight blur first (simulates old lens softness)
        if (blur > 0)
        {
            image.Mutate(x => x.GaussianBlur(blur));
        }
    
        // Reduce contrast slightly
        image.Mutate(x => x.Contrast(0.85f));
    
        // Apply effects
        AddGrain(image, grain);
        AddVignette(image, vignette);
        LiftBlacks(image, fade);
        ApplyColorGrading(image, new Vector3(0, 40, 30), tintIntensity);
        AddDustScratches(image, dust, scratches, hairs);
    
        // Create output directory if it doesn't exist
        var outputDir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }
    
        // Save result
        await image.SaveAsJpegAsync(outputPath);
        Console.WriteLine($"Saved vintage effect image to: {outputPath}");
    }
    
    /// <summary>
    /// Apply all vintage effects to an image
    /// </summary>
    public async Task<Image<Rgba32>?> ApplyVintageEffectAsync(
        string imagePath,
        string? overlayPath,
        int threshold = 70, // should be random 55-70
        int feather = 1,
        float opacity = 1,
        string blendMode = "screen",
        bool invertThreshold = false,
        float contrast = 0.85f,
        float grain = 0.05f,
        float vignette = 0.4f,
        float fade = 0.15f,
        float tintIntensity = 0.25f,
        float dust = 0.0003f,
        int scratches = 3,
        int hairs = 2,
        float blur = 0.5f,
        float adjustRed = 1.035f,
        float adjustGreen = 1.011f,
        float adjustBlue = 1.007f,
        float brightness = 0.9f)
    {
        var result = await Image.LoadAsync<Rgba32>(imagePath);
        if (!string.IsNullOrEmpty(overlayPath))
        {
            var compositor = new ImageCompositor();
            result = await compositor.CompositeImagesAsync(result, overlayPath, threshold, feather, opacity, blendMode,
                invertThreshold);
        }

        if (result == null) return null;

        // Apply slight blur first (simulates old lens softness)
        if (blur > 0) result.Mutate(x => x.GaussianBlur(blur));

        // Reduce contrast slightly
        result.Mutate(x => x.Contrast(contrast));

        // Apply effects
        AddGrain(result, grain);
        AddVignette(result, vignette);
        LiftBlacks(result, fade);
        ApplyColorGrading(result, new Vector3(0, 40, 30), tintIntensity);
        AddDustScratches(result, dust, scratches, hairs);
        AdjustRGBChannels(result, adjustRed, adjustGreen, adjustBlue);
        AdjustBrightness(result, brightness);

        return result;
    }

    
    // Original
    // using var image = await processor.ApplyVintageEffectAsync(
    //     "C:\\Workspace\\NetCorePractice\\gfpgan\\straight_single_003.png",
    //     "C:\\Workspace\\NetCorePractice\\gfpgan\\dust_sample_005_edited.jpg",
    //     60, 1, 1, "screen", false, 
    //     0.85f, 0.04f, 0.35f, 0.01f, -0.2f, 0.01f, 2, 6, 0.5f,
    //             1.035f, 1.011f, 1.007f, 0.9f);
    
    // Photoshop > Filter > scratch and noise > 6 + 20
    // Threshold: 45-60, feather: 0-1, blur: 0.45-0.5
    // using var image = await processor.ApplyVintageEffectAsync(
    //     "C:\\Workspace\\NetCorePractice\\gfpgan\\straight_single_003.png",
    //     "C:\\Workspace\\NetCorePractice\\gfpgan\\dust_sample_005_edited.jpg",
    //     45, 1, 1, "screen", false, 
    //     0.85f, 0.04f, 0.35f, 0.01f, -0.2f, 0.01f, 2, 6, 0.4f,
    //     1.035f, 1.011f, 1.007f, 0.9f);
}