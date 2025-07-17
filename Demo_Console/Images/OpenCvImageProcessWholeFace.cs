using OpenCvSharp;

namespace Demo_Console.Images;

public class OpenCvImageProcessWholeFace : IDisposable
{
    // Skin color range in YCrCb
    private static readonly Scalar SkinColorLowerBound = new(0, 133, 77);
    private static readonly Scalar SkinColorUpperBound = new(255, 173, 127);
    private const int STANDARD_AREA_PIXEL = 5;
    
    private readonly CascadeClassifier _faceClassifier;
    
    public OpenCvImageProcessWholeFace()
    {
        _faceClassifier = new CascadeClassifier("Images/haarcascade_frontalface_alt.xml");
        
        if (_faceClassifier.Empty())
            throw new Exception("Trained model not being configured correctly, please contact your administrator.");
    }
    
    public void SmoothFaceSkin(string inputPath, string outputPath, double smoothingStrength = 0.8)
    {
        // Load the image
        using var image = new Mat(inputPath, ImreadModes.Color);
        if (image.Empty()) throw new Exception("Could not load image");
            
        // Convert to grayscale for face detection
        using var grayImage = new Mat();
        Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
                
        // Detect faces
        var faces = _faceClassifier.DetectMultiScale(
            grayImage,
            scaleFactor: 1.1,
            minNeighbors: 3,
            flags: HaarDetectionTypes.ScaleImage,
            minSize: new Size(50, 50)
        );
                
        Console.WriteLine($"Found {faces.Length} face(s)");
                
        // Process each detected face
        foreach (var faceRect in faces)
        {
            SmoothSkinInFaceRegion(image, faceRect, smoothingStrength);
        }
                
        // Save the result
        image.SaveImage(outputPath);
        Console.WriteLine($"Smoothed image saved to: {outputPath}");
    }
    
    private void SmoothSkinInFaceRegion(Mat image, Rect faceRect, double smoothingStrength)
    {
        // Expand face region slightly to ensure we get all skin
        var padding = (int)(faceRect.Width * 0.1);
        var expandedRect = new Rect(
            Math.Max(0, faceRect.X - padding),
            Math.Max(0, faceRect.Y - padding),
            Math.Min(image.Width - faceRect.X + padding, faceRect.Width + 2 * padding),
            Math.Min(image.Height - faceRect.Y + padding, faceRect.Height + 2 * padding)
        );
        
        // Extract face region
        using var faceRegion = new Mat(image, expandedRect);
        using var smoothedFace = new Mat();
        using var skinMask = new Mat();
        
        // Create skin mask using color thresholding
        CreateSkinMask(faceRegion, skinMask);
            
        // Apply bilateral filter for skin smoothing, decide image sharpness
        Cv2.BilateralFilter(
            faceRegion, 
            smoothedFace, 
            d: 10, 
            sigmaColor: 80, 
            sigmaSpace: 80
        );
            
        // Apply additional smoothing with Gaussian blur
        // using var extraSmooth = new Mat();
        // Cv2.GaussianBlur(smoothedFace, extraSmooth, new Size(STANDARD_AREA_PIXEL, STANDARD_AREA_PIXEL), 0);
        // Cv2.BilateralFilter(smoothedFace, extraSmooth, 9, 75, 75);
                
        // Blend between original and smoothed based on strength
        // Cv2.AddWeighted(smoothedFace, 1 - smoothingStrength, extraSmooth, smoothingStrength, 0, smoothedFace);

        // Apply smoothing only to skin areas
        ApplySmoothingWithMask(faceRegion, smoothedFace, skinMask);
    }
    
    private void CreateSkinMask(Mat faceRegion, Mat skinMask)
    {
        // Convert to YCrCb color space for better skin detection
        using var ycrcb = new Mat();
        Cv2.CvtColor(faceRegion, ycrcb, ColorConversionCodes.BGR2YCrCb);
            
        // Create initial mask
        Cv2.InRange(ycrcb, SkinColorLowerBound, SkinColorUpperBound, skinMask);
            
        // Morphological operations to clean up the mask
        using var kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(3, 3));
        Cv2.MorphologyEx(skinMask, skinMask, MorphTypes.Open, kernel);
        Cv2.MorphologyEx(skinMask, skinMask, MorphTypes.Close, kernel);

        // Gaussian blur to soften mask edges
        // Cv2.GaussianBlur(skinMask, skinMask, new Size(STANDARD_AREA_PIXEL, STANDARD_AREA_PIXEL), 0);
        // Cv2.BilateralFilter(skinMask, skinMask, 9, 75, 75);
    }
    
    private void ApplySmoothingWithMask(Mat original, Mat smoothed, Mat mask)
    {
        // Normalize mask to 0-1 range
        using var normalizedMask = new Mat();
        mask.ConvertTo(normalizedMask, MatType.CV_32F, 1.0 / 255.0);
            
        // Apply smoothing only where mask is white (skin areas)
        for (var y = 0; y < original.Height; y++)
        {
            for (var x = 0; x < original.Width; x++)
            {
                var maskValue = normalizedMask.At<float>(y, x);
                if (maskValue <= 0.1) continue; // Only apply to skin areas
                
                var originalPixel = original.At<Vec3b>(y, x);
                var smoothedPixel = smoothed.At<Vec3b>(y, x);
                        
                // Blend based on mask strength
                var blendedPixel = new Vec3b(
                    (byte)(originalPixel.Item0 * (1 - maskValue) + smoothedPixel.Item0 * maskValue),
                    (byte)(originalPixel.Item1 * (1 - maskValue) + smoothedPixel.Item1 * maskValue),
                    (byte)(originalPixel.Item2 * (1 - maskValue) + smoothedPixel.Item2 * maskValue)
                );
                        
                original.Set(y, x, blendedPixel);
            }
        }
    }

    public void Dispose()
    {
        _faceClassifier?.Dispose();
    }
}