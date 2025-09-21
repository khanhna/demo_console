using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Demo_Console.Images;

public class ImageCompositor
{
    private readonly Dictionary<string, Func<Image<Rgba32>, Image<Rgba32>, float, Image<Rgba32>>> _blendModes;

    public ImageCompositor()
    {
        _blendModes = new Dictionary<string, Func<Image<Rgba32>, Image<Rgba32>, float, Image<Rgba32>>>
        {
            ["normal"] = BlendNormal,
            ["multiply"] = BlendMultiply,
            ["screen"] = BlendScreen,
            ["overlay"] = BlendOverlay,
            ["soft_light"] = BlendSoftLight,
            ["hard_light"] = BlendHardLight,
            ["darken"] = BlendDarken,
            ["lighten"] = BlendLighten,
        };
    }

    /// <summary>
    /// Remove background from image based on brightness threshold.
    /// </summary>
    /// <param name="image">Input image</param>
    /// <param name="threshold">Brightness threshold (0-255)</param>
    /// <param name="feather">Edge softening amount</param>
    /// <param name="invert">If true, remove bright areas instead of dark areas</param>
    /// <returns>Image with transparent background</returns>
    public Image<Rgba32> RemoveBackground(Image<Rgba32> image, int threshold, int feather, bool invert)
    {
        var result = image.Clone();

        result.ProcessPixelRows(accessor =>
        {
            for (var y = 0; y < accessor.Height; y++)
            {
                var pixelRow = accessor.GetRowSpan(y);

                for (var x = 0; x < pixelRow.Length; x++)
                {
                    ref var pixel = ref pixelRow[x];

                    // Calculate brightness
                    var brightness = (pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f);

                    // Create alpha mask
                    float alpha;
                    if (invert)
                    {
                        // Remove bright areas (for light backgrounds)
                        alpha = brightness < threshold ? 1.0f : 0.0f;
                    }
                    else
                    {
                        // Remove dark areas (for dark backgrounds)
                        alpha = brightness >= threshold ? 1.0f : 0.0f;
                    }

                    pixel.A = (byte)(alpha * 255);
                }
            }
        });

        // Apply feathering if specified
        if (feather > 0)
        {
            result.Mutate(x => x.GaussianBlur(feather));
        }

        return result;
    }

    /// <summary>
    /// Resize overlay to completely cover the background image.
    /// Uses the larger of width or height scale to ensure full coverage.
    /// </summary>
    /// <param name="overlay">Overlay image</param>
    /// <param name="background">Background image</param>
    /// <returns>Resized overlay</returns>
    public Image<Rgba32> ResizeOverlayToCover(Image<Rgba32> overlay, Image<Rgba32> background)
    {
        var bgWidth = background.Width;
        var bgHeight = background.Height;
        var overlayWidth = overlay.Width;
        var overlayHeight = overlay.Height;

        // Calculate scale factors
        var scaleX = (float)bgWidth / overlayWidth;
        var scaleY = (float)bgHeight / overlayHeight;

        // Use the larger scale to ensure complete coverage
        var scale = Math.Max(scaleX, scaleY);

        // Calculate new dimensions
        var newWidth = (int)(overlayWidth * scale);
        var newHeight = (int)(overlayHeight * scale);

        // Resize overlay
        var resized = overlay.Clone();
        resized.Mutate(x => x.Resize(newWidth, newHeight, KnownResamplers.Lanczos3));

        // If the resized overlay is larger than background, crop it from center
        if (newWidth > bgWidth || newHeight > bgHeight)
        {
            var left = (newWidth - bgWidth) / 2;
            var top = (newHeight - bgHeight) / 2;
            var cropRect = new Rectangle(left, top, bgWidth, bgHeight);

            resized.Mutate(x => x.Crop(cropRect));
        }

        return resized;
    }

    private Image<Rgba32> BlendNormal(Image<Rgba32> baseImg, Image<Rgba32> overlay, float opacity)
    {
        var result = baseImg.Clone();

        result.ProcessPixelRows(overlay, (baseAccessor, overlayAccessor) =>
        {
            for (var y = 0; y < baseAccessor.Height; y++)
            {
                var baseRow = baseAccessor.GetRowSpan(y);
                var overlayRow = overlayAccessor.GetRowSpan(y);

                for (var x = 0; x < baseRow.Length; x++)
                {
                    ref var basePixel = ref baseRow[x];
                    ref readonly var overlayPixel = ref overlayRow[x];

                    var alpha = overlayPixel.A / 255.0f * opacity;
                    basePixel.R = (byte)(basePixel.R * (1 - alpha) + overlayPixel.R * alpha);
                    basePixel.G = (byte)(basePixel.G * (1 - alpha) + overlayPixel.G * alpha);
                    basePixel.B = (byte)(basePixel.B * (1 - alpha) + overlayPixel.B * alpha);
                }
            }
        });

        return result;
    }

    private Image<Rgba32> BlendMultiply(Image<Rgba32> baseImg, Image<Rgba32> overlay, float opacity)
    {
        var result = baseImg.Clone();

        result.ProcessPixelRows(overlay, (baseAccessor, overlayAccessor) =>
        {
            for (var y = 0; y < baseAccessor.Height; y++)
            {
                var baseRow = baseAccessor.GetRowSpan(y);
                var overlayRow = overlayAccessor.GetRowSpan(y);

                for (var x = 0; x < baseRow.Length; x++)
                {
                    ref var basePixel = ref baseRow[x];
                    ref readonly var overlayPixel = ref overlayRow[x];

                    if (overlayPixel.A == 0) continue;

                    var alpha = overlayPixel.A / 255.0f * opacity;
                    var blendedR = (basePixel.R / 255.0f) * (overlayPixel.R / 255.0f) * 255;
                    var blendedG = (basePixel.G / 255.0f) * (overlayPixel.G / 255.0f) * 255;
                    var blendedB = (basePixel.B / 255.0f) * (overlayPixel.B / 255.0f) * 255;

                    basePixel.R = (byte)(basePixel.R * (1 - alpha) + blendedR * alpha);
                    basePixel.G = (byte)(basePixel.G * (1 - alpha) + blendedG * alpha);
                    basePixel.B = (byte)(basePixel.B * (1 - alpha) + blendedB * alpha);
                }
            }
        });

        return result;
    }

    private Image<Rgba32> BlendScreen(Image<Rgba32> baseImg, Image<Rgba32> overlay, float opacity)
    {
        var result = baseImg.Clone();

        result.ProcessPixelRows(overlay, (baseAccessor, overlayAccessor) =>
        {
            for (var y = 0; y < baseAccessor.Height; y++)
            {
                var baseRow = baseAccessor.GetRowSpan(y);
                var overlayRow = overlayAccessor.GetRowSpan(y);

                for (var x = 0; x < baseRow.Length; x++)
                {
                    ref var basePixel = ref baseRow[x];
                    ref readonly var overlayPixel = ref overlayRow[x];

                    if (overlayPixel.A == 0) continue;

                    var alpha = overlayPixel.A / 255.0f * opacity;
                    var blendedR = (1 - (1 - basePixel.R / 255.0f) * (1 - overlayPixel.R / 255.0f)) * 255;
                    var blendedG = (1 - (1 - basePixel.G / 255.0f) * (1 - overlayPixel.G / 255.0f)) * 255;
                    var blendedB = (1 - (1 - basePixel.B / 255.0f) * (1 - overlayPixel.B / 255.0f)) * 255;

                    basePixel.R = (byte)(basePixel.R * (1 - alpha) + blendedR * alpha);
                    basePixel.G = (byte)(basePixel.G * (1 - alpha) + blendedG * alpha);
                    basePixel.B = (byte)(basePixel.B * (1 - alpha) + blendedB * alpha);
                }
            }
        });

        return result;
    }

    private Image<Rgba32> BlendOverlay(Image<Rgba32> baseImg, Image<Rgba32> overlay, float opacity)
    {
        var result = baseImg.Clone();

        result.ProcessPixelRows(overlay, (baseAccessor, overlayAccessor) =>
        {
            for (var y = 0; y < baseAccessor.Height; y++)
            {
                var baseRow = baseAccessor.GetRowSpan(y);
                var overlayRow = overlayAccessor.GetRowSpan(y);

                for (var x = 0; x < baseRow.Length; x++)
                {
                    ref var basePixel = ref baseRow[x];
                    ref readonly var overlayPixel = ref overlayRow[x];

                    if (overlayPixel.A == 0) continue;

                    var alpha = overlayPixel.A / 255.0f * opacity;
                    var blendedR = OverlayBlend(basePixel.R / 255.0f, overlayPixel.R / 255.0f) * 255;
                    var blendedG = OverlayBlend(basePixel.G / 255.0f, overlayPixel.G / 255.0f) * 255;
                    var blendedB = OverlayBlend(basePixel.B / 255.0f, overlayPixel.B / 255.0f) * 255;

                    basePixel.R = (byte)(basePixel.R * (1 - alpha) + blendedR * alpha);
                    basePixel.G = (byte)(basePixel.G * (1 - alpha) + blendedG * alpha);
                    basePixel.B = (byte)(basePixel.B * (1 - alpha) + blendedB * alpha);
                }
            }
        });

        return result;
    }

    private static float OverlayBlend(float baseValue, float overlayValue)
    {
        return baseValue < 0.5f
            ? 2 * baseValue * overlayValue
            : 1 - 2 * (1 - baseValue) * (1 - overlayValue);
    }

    private Image<Rgba32> BlendSoftLight(Image<Rgba32> baseImg, Image<Rgba32> overlay, float opacity)
    {
        var result = baseImg.Clone();

        result.ProcessPixelRows(overlay, (baseAccessor, overlayAccessor) =>
        {
            for (var y = 0; y < baseAccessor.Height; y++)
            {
                var baseRow = baseAccessor.GetRowSpan(y);
                var overlayRow = overlayAccessor.GetRowSpan(y);

                for (var x = 0; x < baseRow.Length; x++)
                {
                    ref var basePixel = ref baseRow[x];
                    ref readonly var overlayPixel = ref overlayRow[x];

                    if (overlayPixel.A == 0) continue;

                    var alpha = overlayPixel.A / 255.0f * opacity;
                    var blendedR = SoftLightBlend(basePixel.R / 255.0f, overlayPixel.R / 255.0f) * 255;
                    var blendedG = SoftLightBlend(basePixel.G / 255.0f, overlayPixel.G / 255.0f) * 255;
                    var blendedB = SoftLightBlend(basePixel.B / 255.0f, overlayPixel.B / 255.0f) * 255;

                    basePixel.R = (byte)(basePixel.R * (1 - alpha) + blendedR * alpha);
                    basePixel.G = (byte)(basePixel.G * (1 - alpha) + blendedG * alpha);
                    basePixel.B = (byte)(basePixel.B * (1 - alpha) + blendedB * alpha);
                }
            }
        });

        return result;
    }

    private static float SoftLightBlend(float baseValue, float overlayValue)
    {
        return overlayValue < 0.5f
            ? baseValue - (1 - 2 * overlayValue) * baseValue * (1 - baseValue)
            : baseValue + (2 * overlayValue - 1) * (MathF.Sqrt(baseValue) - baseValue);
    }

    private Image<Rgba32> BlendHardLight(Image<Rgba32> baseImg, Image<Rgba32> overlay, float opacity)
    {
        var result = baseImg.Clone();

        result.ProcessPixelRows(overlay, (baseAccessor, overlayAccessor) =>
        {
            for (var y = 0; y < baseAccessor.Height; y++)
            {
                var baseRow = baseAccessor.GetRowSpan(y);
                var overlayRow = overlayAccessor.GetRowSpan(y);

                for (var x = 0; x < baseRow.Length; x++)
                {
                    ref var basePixel = ref baseRow[x];
                    ref readonly var overlayPixel = ref overlayRow[x];

                    if (overlayPixel.A == 0) continue;

                    var alpha = overlayPixel.A / 255.0f * opacity;
                    var blendedR = OverlayBlend(overlayPixel.R / 255.0f, basePixel.R / 255.0f) * 255;
                    var blendedG = OverlayBlend(overlayPixel.G / 255.0f, basePixel.G / 255.0f) * 255;
                    var blendedB = OverlayBlend(overlayPixel.B / 255.0f, basePixel.B / 255.0f) * 255;

                    basePixel.R = (byte)(basePixel.R * (1 - alpha) + blendedR * alpha);
                    basePixel.G = (byte)(basePixel.G * (1 - alpha) + blendedG * alpha);
                    basePixel.B = (byte)(basePixel.B * (1 - alpha) + blendedB * alpha);
                }
            }
        });

        return result;
    }

    private Image<Rgba32> BlendDarken(Image<Rgba32> baseImg, Image<Rgba32> overlay, float opacity)
    {
        var result = baseImg.Clone();

        result.ProcessPixelRows(overlay, (baseAccessor, overlayAccessor) =>
        {
            for (int y = 0; y < baseAccessor.Height; y++)
            {
                var baseRow = baseAccessor.GetRowSpan(y);
                var overlayRow = overlayAccessor.GetRowSpan(y);

                for (int x = 0; x < baseRow.Length; x++)
                {
                    ref var basePixel = ref baseRow[x];
                    ref readonly var overlayPixel = ref overlayRow[x];

                    if (overlayPixel.A == 0) continue;

                    var alpha = overlayPixel.A / 255.0f * opacity;
                    var blendedR = Math.Min(basePixel.R, overlayPixel.R);
                    var blendedG = Math.Min(basePixel.G, overlayPixel.G);
                    var blendedB = Math.Min(basePixel.B, overlayPixel.B);

                    basePixel.R = (byte)(basePixel.R * (1 - alpha) + blendedR * alpha);
                    basePixel.G = (byte)(basePixel.G * (1 - alpha) + blendedG * alpha);
                    basePixel.B = (byte)(basePixel.B * (1 - alpha) + blendedB * alpha);
                }
            }
        });

        return result;
    }

    private Image<Rgba32> BlendLighten(Image<Rgba32> baseImg, Image<Rgba32> overlay, float opacity)
    {
        var result = baseImg.Clone();

        result.ProcessPixelRows(overlay, (baseAccessor, overlayAccessor) =>
        {
            for (var y = 0; y < baseAccessor.Height; y++)
            {
                var baseRow = baseAccessor.GetRowSpan(y);
                var overlayRow = overlayAccessor.GetRowSpan(y);

                for (var x = 0; x < baseRow.Length; x++)
                {
                    ref var basePixel = ref baseRow[x];
                    ref readonly var overlayPixel = ref overlayRow[x];

                    if (overlayPixel.A == 0) continue;

                    var alpha = overlayPixel.A / 255.0f * opacity;
                    var blendedR = Math.Max(basePixel.R, overlayPixel.R);
                    var blendedG = Math.Max(basePixel.G, overlayPixel.G);
                    var blendedB = Math.Max(basePixel.B, overlayPixel.B);

                    basePixel.R = (byte)(basePixel.R * (1 - alpha) + blendedR * alpha);
                    basePixel.G = (byte)(basePixel.G * (1 - alpha) + blendedG * alpha);
                    basePixel.B = (byte)(basePixel.B * (1 - alpha) + blendedB * alpha);
                }
            }
        });

        return result;
    }
    
    // <summary>
    /// Main composition function.
    /// </summary>
    public async Task<bool> CompositeImagesAsync(
        string backgroundPath,
        string overlayPath,
        string outputPath,
        int threshold = 70, // should be random 55-70
        int feather = 1,
        float opacity = 1,
        string blendMode = "screen",
        bool invertThreshold = false,
        bool preview = false)
    {
        Console.WriteLine("Loading images...");

        try
        {
            using var background = await Image.LoadAsync<Rgba32>(backgroundPath);
            using var overlay = await Image.LoadAsync<Rgba32>(overlayPath);
            
            Console.WriteLine($"Background: {background.Width}x{background.Height}");
            Console.WriteLine($"Overlay: {overlay.Width}x{overlay.Height}");

            // Remove background from overlay
            Console.WriteLine($"Removing background (threshold: {threshold}, feather: {feather})...");
            using var overlayProcessed = RemoveBackground(overlay, threshold, feather, invertThreshold);

            // Resize overlay to cover background completely
            Console.WriteLine("Resizing overlay to cover background...");
            using var overlayResized = ResizeOverlayToCover(overlayProcessed, background);
            Console.WriteLine($"Overlay resized to: {overlayResized.Width}x{overlayResized.Height}");

            // Save preview of processed overlay if requested
            if (preview)
            {
                var previewPath = Path.ChangeExtension(outputPath, null) + "_overlay_preview.png";
                await overlayResized.SaveAsync(previewPath);
                Console.WriteLine($"Overlay preview saved: {previewPath}");
            }

            // Ensure overlay is same size as background
            using var finalOverlay = overlayResized.Width != background.Width || overlayResized.Height != background.Height
                ? overlayResized.Clone(x => x.Resize(background.Width, background.Height, KnownResamplers.Lanczos3))
                : overlayResized.Clone();

            // Apply blend mode
            Console.WriteLine($"Applying {blendMode} blend with {opacity * 100:F0}% opacity...");
            Image<Rgba32> result;

            result = _blendModes.TryGetValue(blendMode, out var mode)
                ? mode(background, finalOverlay, opacity)
                : BlendNormal(background, finalOverlay, opacity);

            // Save result
            Console.WriteLine($"Saving result to: {outputPath}");
            
            // Create output directory if it doesn't exist
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            await result.SaveAsync(outputPath);
            result.Dispose();

            Console.WriteLine("✅ Composition complete!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing images: {ex.Message}");
            return false;
        }
    }
    
    // <summary>
    /// Main composition function.
    /// </summary>
    public async Task<Image<Rgba32>?> CompositeImagesAsync(
        Image<Rgba32>? majorImage,
        string overlayPath,
        int threshold = 70, // should be random 55-70
        int feather = 1,
        float opacity = 1,
        string blendMode = "screen",
        bool invertThreshold = false)
    {
        try
        {
            if (majorImage == null) return null;

            using var overlay = await Image.LoadAsync<Rgba32>(overlayPath);

            // Remove background from overlay
            using var overlayProcessed = RemoveBackground(overlay, threshold, feather, invertThreshold);

            // Resize overlay to cover background completely
            using var overlayResized = ResizeOverlayToCover(overlayProcessed, majorImage);

            // Ensure overlay is same size as background
            using var finalOverlay =
                overlayResized.Width != majorImage.Width || overlayResized.Height != majorImage.Height
                    ? overlayResized.Clone(x => x.Resize(majorImage.Width, majorImage.Height, KnownResamplers.Lanczos3))
                    : overlayResized.Clone();

            // Apply blend mode
            var result = _blendModes.TryGetValue(blendMode, out var mode)
                ? mode(majorImage, finalOverlay, opacity)
                : BlendNormal(majorImage, finalOverlay, opacity);

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing images: {ex.Message}");
        }
        finally
        {
            majorImage?.Dispose();
        }
        
        return null;
    }
}
