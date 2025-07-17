using OpenCvSharp;

namespace Demo_Console.Images;

public class OpenCvImageProcessFaceExcludingReserved : IDisposable
{
    private const int STANDARD_AREA_PIXEL = 5;
    
    // Skin color range in YCrCb
    private static readonly Scalar SkinColorLowerBound = new(0, 133, 77);
    private static readonly Scalar SkinColorUpperBound = new(255, 173, 127);
    
    private CascadeClassifier faceClassifier;
    private CascadeClassifier eyeClassifier;
    private CascadeClassifier mouthClassifier;

    public OpenCvImageProcessFaceExcludingReserved()
    {
        faceClassifier = new CascadeClassifier("Images/haarcascade_frontalface_alt.xml");
        eyeClassifier = new CascadeClassifier("Images/haarcascade_eye.xml");
        mouthClassifier = new CascadeClassifier("Images/haarcascade_smile.xml");

        if (faceClassifier.Empty())
            throw new Exception(
                "Face trained model not being configured correctly, please contact your administrator.");
        
        if (eyeClassifier.Empty())
            throw new Exception(
                "Eyes trained model not being configured correctly, please contact your administrator.");
        
        if (mouthClassifier.Empty())
            throw new Exception(
                "Mouth trained model not being configured correctly, please contact your administrator.");
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
        var faces = faceClassifier.DetectMultiScale(
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
            SmoothSkinInFaceRegion(image, faceRect, grayImage, smoothingStrength);
        }
                
        // Save the result
        image.SaveImage(outputPath);
        Console.WriteLine($"Smoothed image saved to: {outputPath}");
    }
    
    private void SmoothSkinInFaceRegion(Mat image, Rect faceRect, Mat grayImage, double smoothingStrength)
    {
        // Expand face region slightly to ensure we get all skin
        var padding = Convert.ToInt32(faceRect.Width * 0.1);
        var expandedRect = new Rect(
            Math.Max(0, faceRect.X - padding),
            Math.Max(0, faceRect.Y - padding),
            Math.Min(image.Width - faceRect.X + padding, faceRect.Width + 2 * padding),
            Math.Min(image.Height - faceRect.Y + padding, faceRect.Height + 2 * padding)
        );
        
        // Extract face region
        using var faceRegion = new Mat(image, expandedRect);
        using var faceGray = new Mat(grayImage, expandedRect);
        using var smoothedFace = new Mat();
        using var skinMask = new Mat();
        
        // Create skin mask using color thresholding
        CreateSkinMask(faceRegion, skinMask);
            
        // Create exclusion mask for eyes and mouth
        using var exclusionMask = new Mat();
        CreateExclusionMask(faceGray, exclusionMask, faceRect);
                
        // Combine skin mask with exclusion mask
        Cv2.BitwiseAnd(skinMask, exclusionMask, skinMask);

        // Apply bilateral filter for skin smoothing
        Cv2.BilateralFilter(
            faceRegion, 
            smoothedFace, 
            d: 10, // 15
            sigmaColor: 40, // 80 
            sigmaSpace: 40 // 80
        );
            
        // Apply additional smoothing with Gaussian blur
        using var extraSmooth = new Mat();
        Cv2.GaussianBlur(smoothedFace, extraSmooth, new Size(STANDARD_AREA_PIXEL, STANDARD_AREA_PIXEL), 0);
                
        // Blend between original and smoothed based on strength
        Cv2.AddWeighted(smoothedFace, 1 - smoothingStrength, extraSmooth, smoothingStrength, 0, smoothedFace);

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
        Cv2.GaussianBlur(skinMask, skinMask, new Size(STANDARD_AREA_PIXEL, STANDARD_AREA_PIXEL), 0);
    }
    
    private void CreateExclusionMask(Mat faceGray, Mat exclusionMask, Rect originalFaceRect)
    {
        // Create a white mask (allow smoothing everywhere initially)
        exclusionMask.Create(faceGray.Size(), MatType.CV_8UC1);
        exclusionMask.SetTo(Scalar.White);
        
        // Detect eyes in the face region
        if (!eyeClassifier.Empty())
        {
            var eyes = eyeClassifier.DetectMultiScale(
                faceGray,
                scaleFactor: 1.1,
                minNeighbors: 3,
                minSize: new Size(15, 15),
                maxSize: new Size(originalFaceRect.Width / 3, originalFaceRect.Height / 3)
            );
            
            // Create exclusion zones around eyes
            foreach (var eyeRect in eyes)
            {
                // Expand eye region to ensure we exclude eyelashes and eyebrows
                var eyePadding = Math.Max(eyeRect.Width / 4, eyeRect.Height / 4);
                var expandedEyeRect = new Rect(
                    Math.Max(0, eyeRect.X - eyePadding),
                    Math.Max(0, eyeRect.Y - eyePadding),
                    Math.Min(faceGray.Width - (eyeRect.X - eyePadding), eyeRect.Width + 2 * eyePadding),
                    Math.Min(faceGray.Height - (eyeRect.Y - eyePadding), eyeRect.Height + 2 * eyePadding)
                );
                
                // Create elliptical mask for more natural exclusion
                using Mat eyeMask = Mat.Zeros(faceGray.Size(), MatType.CV_8UC1);
                var center = new Point(
                    expandedEyeRect.X + expandedEyeRect.Width / 2,
                    expandedEyeRect.Y + expandedEyeRect.Height / 2
                );
                var axes = new Size(expandedEyeRect.Width / 2, expandedEyeRect.Height / 2);
                    
                Cv2.Ellipse(eyeMask, center, axes, 0, 0, 360, Scalar.White, -1);
                    
                // Subtract eye region from exclusion mask (make it black = exclude from smoothing)
                using var invertedEyeMask = new Mat();
                Cv2.BitwiseNot(eyeMask, invertedEyeMask);
                Cv2.BitwiseAnd(exclusionMask, invertedEyeMask, exclusionMask);
            }
        }
        
        // Detect mouth in the lower half of the face
        if (!mouthClassifier.Empty())
        {
            // Focus on lower half of face for mouth detection
            var mouthSearchY = faceGray.Height / 2;
            var mouthSearchHeight = faceGray.Height - mouthSearchY;

            using var lowerFace = new Mat(faceGray, new Rect(0, mouthSearchY, faceGray.Width, mouthSearchHeight));
            var mouths = mouthClassifier.DetectMultiScale(
                lowerFace,
                scaleFactor: 1.1,
                minNeighbors: 3,
                minSize: new Size(20, 10),
                maxSize: new Size(originalFaceRect.Width / 2, originalFaceRect.Height / 3)
            );
                
            // Create exclusion zones around mouth
            foreach (var mouthRect in mouths)
            {
                // Adjust mouth coordinates to full face region
                var adjustedMouthRect = new Rect(
                    mouthRect.X,
                    mouthRect.Y + mouthSearchY,
                    mouthRect.Width,
                    mouthRect.Height
                );
                    
                // Expand mouth region
                var mouthPadding = Math.Max(adjustedMouthRect.Width / 6, adjustedMouthRect.Height / 3);
                var expandedMouthRect = new Rect(
                    Math.Max(0, adjustedMouthRect.X - mouthPadding),
                    Math.Max(0, adjustedMouthRect.Y - mouthPadding),
                    Math.Min(faceGray.Width - (adjustedMouthRect.X - mouthPadding), adjustedMouthRect.Width + 2 * mouthPadding),
                    Math.Min(faceGray.Height - (adjustedMouthRect.Y - mouthPadding), adjustedMouthRect.Height + 2 * mouthPadding)
                );
                    
                // Create elliptical mask for mouth
                using Mat mouthMask = Mat.Zeros(faceGray.Size(), MatType.CV_8UC1);
                var center = new Point(
                    expandedMouthRect.X + expandedMouthRect.Width / 2,
                    expandedMouthRect.Y + expandedMouthRect.Height / 2
                );
                var axes = new Size(expandedMouthRect.Width / 2, expandedMouthRect.Height / 2);
                        
                Cv2.Ellipse(mouthMask, center, axes, 0, 0, 360, Scalar.White, -1);
                        
                // Subtract mouth region from exclusion mask
                using var invertedMouthMask = new Mat();
                Cv2.BitwiseNot(mouthMask, invertedMouthMask);
                Cv2.BitwiseAnd(exclusionMask, invertedMouthMask, exclusionMask);
            }
        }
        
        // Add manual exclusion for typical eye and mouth areas if detection fails
        /*
        if (eyeClassifier.Empty() || mouthClassifier.Empty())
            AddManualExclusionAreas(exclusionMask, faceGray.Size());
        */
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
                if (!(maskValue > 0.1)) continue;
                
                // Only apply to skin areas
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
    
    private void AddManualExclusionAreas(Mat exclusionMask, Size faceSize)
    {
        // Add typical eye areas (upper third of face, left and right sides)
        var eyeY = (int)(faceSize.Height * 0.25);
        var eyeHeight = (int)(faceSize.Height * 0.15);
        var eyeWidth = (int)(faceSize.Width * 0.25);
        
        // Left eye area
        var leftEyeArea = new Rect(
            (int)(faceSize.Width * 0.15),
            eyeY,
            eyeWidth,
            eyeHeight
        );
        
        // Right eye area
        var rightEyeArea = new Rect(
            (int)(faceSize.Width * 0.6),
            eyeY,
            eyeWidth,
            eyeHeight
        );
        
        // Mouth area (lower third of face, center)
        var mouthArea = new Rect(
            (int)(faceSize.Width * 0.3),
            (int)(faceSize.Height * 0.65),
            (int)(faceSize.Width * 0.4),
            (int)(faceSize.Height * 0.2)
        );
        
        // Create elliptical exclusions
        CreateEllipticalExclusion(exclusionMask, leftEyeArea);
        CreateEllipticalExclusion(exclusionMask, rightEyeArea);
        CreateEllipticalExclusion(exclusionMask, mouthArea);
    }
    
    private void CreateEllipticalExclusion(Mat exclusionMask, Rect area)
    {
        var center = new Point(area.X + area.Width / 2, area.Y + area.Height / 2);
        var axes = new Size(area.Width / 2, area.Height / 2);
        
        Cv2.Ellipse(exclusionMask, center, axes, 0, 0, 360, Scalar.Black, -1);
        
        if (eyeClassifier.Empty())
            Console.WriteLine("Warning: Could not load eye cascade classifier. Eyes will not be excluded from smoothing.");
        
        if (mouthClassifier.Empty())
            Console.WriteLine("Warning: Could not load mouth cascade classifier. Mouth will not be excluded from smoothing.");
    }

    public void Dispose()
    {
        faceClassifier?.Dispose();
        eyeClassifier?.Dispose();
        mouthClassifier?.Dispose();
    }
}