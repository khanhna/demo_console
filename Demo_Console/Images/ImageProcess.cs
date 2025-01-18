using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp;
using Size = System.Drawing.Size;

namespace Demo_Console.Images;

public class ImageProcess
{
    public static void MergeImage()
    {
        Console.WriteLine(Directory.GetCurrentDirectory());

        var targetDirectory = Directory.GetCurrentDirectory();
        
        // Define the frame size
        var frameWidth = 1020;
        var frameHeight = 3040;
        var imageLayout = new Bitmap(frameWidth, frameHeight);
        
        using (var g = Graphics.FromImage(imageLayout))
        {
            // Load the 4 base images (replace with your file paths)
            var image1 = Image.FromFile(Path.Combine(targetDirectory, "cropped_resized_image.png"));
            
            g.DrawImage(image1, new Rectangle(58, 58, 916, 616));
            g.DrawImage(image1, new Rectangle(58, 718, 916, 616));
            g.DrawImage(image1, new Rectangle(58, 1378, 916, 616));
            g.DrawImage(image1, new Rectangle(58, 2038, 916, 616));
            
            var imageTheme = Image.FromFile(Path.Combine(targetDirectory, "133548626886541756_264A.png"));

            g.DrawImage(imageTheme, new Rectangle(0, 0, imageTheme.Width, imageTheme.Height));

            // Save the final frame
            imageLayout.Save(Path.Combine(targetDirectory, "final_frame.png"));
        }

        Console.WriteLine("Frame created successfully!");
    }

    public static void CropImageFromCenter(double ratioWidth, double ratioHeight)
    {
        var targetDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(targetDirectory, "camera-view-image.png"); // Replace with your image path
        var outputPath = "cropped_image.png";

        // Define the desired crop ratio (e.g., 16:9)
        var desiredRatio = ratioWidth / ratioHeight;

        using (var originalImage = new Bitmap(inputPath))
        {
            int originalWidth = originalImage.Width;
            int originalHeight = originalImage.Height;

            // Calculate the dimensions of the crop rectangle
            int cropWidth, cropHeight;

            if (originalWidth / (double)originalHeight > desiredRatio)
            {
                // Wider than the desired ratio
                cropHeight = originalHeight;
                cropWidth = (int)(cropHeight * desiredRatio);
            }
            else
            {
                // Taller than the desired ratio
                cropWidth = originalWidth;
                cropHeight = (int)(cropWidth / desiredRatio);
            }

            // Calculate the top-left point for cropping
            int cropX = (originalWidth - cropWidth) / 2;
            int cropY = (originalHeight - cropHeight) / 2;

            // Create the cropping rectangle
            var cropRect = new Rectangle(cropX, cropY, cropWidth, cropHeight);

            // Perform the crop
            using (var croppedImage = originalImage.Clone(cropRect, originalImage.PixelFormat))
            {
                croppedImage.Save(outputPath);
            }
        }

        Console.WriteLine("Image cropped and saved successfully!");
    }

    public static void ResizeImage(int targetWidth)
    {
        var targetDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(targetDirectory, "cropped_image.png"); // Replace with your image path
        var outputPath = "cropped_resized_image.png";

        using (var originalImage = new Bitmap(inputPath))
        {
            // Calculate the new height while keeping the aspect ratio
            var aspectRatio = (double)originalImage.Height / originalImage.Width;
            var targetHeight = (int)(targetWidth * aspectRatio);

            // Create a new bitmap with the desired dimensions
            using (var resizedImage = new Bitmap(originalImage, new Size(targetWidth, targetHeight)))
            {
                // Save the resized image
                resizedImage.Save(outputPath);
            }
        }

        Console.WriteLine("Image resized and saved successfully!");
    }

    public static void HorizontallyFlipImage()
    {
        var targetDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(targetDirectory, "cropped_resized_image.png"); // Replace with your image path
        var outputPath = "cropped_resized_flipped_image.png";

        using var originalImage = Image.FromFile(inputPath);
        // Create a new empty bitmap with the same size as the original image
        using var flippedImage = new Bitmap(originalImage.Width, originalImage.Height);

        using var g = Graphics.FromImage(flippedImage);
        // Apply a horizontal flip transformation
        g.ScaleTransform(-1, 1); // Flip horizontally
        g.TranslateTransform(-originalImage.Width, 0); // Adjust position

        // Draw the original image onto the flipped graphics context
        g.DrawImage(originalImage, 0, 0);
        
        flippedImage.Save(outputPath);
    }

    public static void ExtractAllFramesFromVideo()
    {
        var videoPath = "camera-view-image_001.mp4"; // Path to the video file
        var outputFolder = "frames"; // Folder to save frames
        
        // Ensure output folder exists
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        using var capture = new VideoCapture(videoPath);
        if (!capture.IsOpened())
        {
            Console.WriteLine("Failed to open video.");
            return;
        }

        int frameIndex = 0;
        var frame = new Mat();

        while (capture.Read(frame))
        {
            if (frame.Empty())
            {
                break;
            }

            var frameFileName = Path.Combine(outputFolder, $"frame_{frameIndex:D4}.png");
            Cv2.ImWrite(frameFileName, frame);

            Console.WriteLine($"Saved frame {frameIndex} to {frameFileName}");
            frameIndex++;
        }

        Console.WriteLine($"Extraction complete. Total frames: {frameIndex}");
    }

    public static void ExtractSpecificFrames(int[] framesToTake)
    {
        var videoPath = "camera-view-image_001.mp4"; // Path to the video file
        var outputFolder = "SpecificFrames"; // Folder to save frames
        
        // Ensure output folder exists
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        using var capture = new VideoCapture(videoPath);
        if (!capture.IsOpened())
        {
            Console.WriteLine("Failed to open video.");
            return;
        }
        
        var frame = new Mat();
        for (var i = 0; i < framesToTake.Length; i++)
        {
            // Set the video capture to the desired frame position
            capture.Set(VideoCaptureProperties.PosFrames, framesToTake[i]);
            
            // Read the frame at the specified position
            if (capture.Read(frame) && !frame.Empty())
            {
                var frameFileName = Path.Combine(outputFolder, $"frame_{(i + 1):D4}.png");
                Cv2.ImWrite(frameFileName, frame);
                Console.WriteLine($"Saved frame {i + 1} to {frameFileName}");
            }
            else
            {
                Console.WriteLine($"Failed to capture frame at position {i}");
            }
        }
    }
    
    public static Image MatToImage(Mat mat)
    {
        // Convert Mat to a byte array (e.g., PNG format)
        var imageData = mat.ToBytes();

        // Load the byte array into a MemoryStream
        using var ms = new MemoryStream(imageData);

        // Create and return an Image from the MemoryStream
        return Image.FromStream(ms);
    }
}

public class ImageProcessTestOpenCvSharp
{
    public static void MergeToFinalFrameOpenCv()
    {
        var videoPath = "camera-view-image_001.mp4"; // Path to the video file
        var outputFolder = "openCvFrames"; // Folder to save frames
        var targetDirectory = Directory.GetCurrentDirectory();
        
        // Ensure output folder exists
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        
        // Define the frame size
        // In order for finalImage.CopyTo(roi); to work, Cv2.ImRead flag must not be ImreadModes.Unchanged!!!
        // MatType.CV_8UC3 is the MatType when read PNG Image with ImreadModes.Color flag!!
        using var imageTheme = Cv2.ImRead(Path.Combine(targetDirectory, "133548626886541756_264A.png"),
            ImreadModes.Unchanged);
        using var baseImage = new Mat(new OpenCvSharp.Size(imageTheme.Width, imageTheme.Height), MatType.CV_8UC3);

        using var capture = new VideoCapture(videoPath);
        if (!capture.IsOpened())
        {
            Console.WriteLine("Failed to open video.");
            return;
        }

        var frameIndex = 1;
        using var frame = new Mat();

        while (capture.Read(frame))
        {
            if (frame.Empty())
            {
                break;
            }
            
            using var croppedImage = CropImageCenter(frame, 3, 2);
            using var resizedImage = ResizeImage(croppedImage, 916);
            using var finalImage = FlipImage(resizedImage);
            
            // Define the ROI on the base image
            var roiRect = new Rect(58, 58, finalImage.Width, finalImage.Height);
            var roi = new Mat(baseImage, roiRect);
            finalImage.CopyTo(roi);
            
            roiRect = new Rect(58, 718, finalImage.Width, finalImage.Height);
            roi = new Mat(baseImage, roiRect);
            finalImage.CopyTo(roi);
            
            roiRect = new Rect(58, 1378, finalImage.Width, finalImage.Height);
            roi = new Mat(baseImage, roiRect);
            finalImage.CopyTo(roi);
            
            roiRect = new Rect(58, 2038, finalImage.Width, finalImage.Height);
            roi = new Mat(baseImage, roiRect);
            finalImage.CopyTo(roi);
            roi.Dispose();

            // Add an alpha channel to the overlay image
            using var alphaChannel = new Mat(); // Alpha mask
            Cv2.ExtractChannel(imageTheme, alphaChannel, 3); // coi 0 > 3 ~ RGBA

            // Normalize the alpha mask to the range [0, 1]
            using var alphaMask = alphaChannel / 255.0;
            using var invertedAlphaMask = Mat.Ones(alphaMask.Size(), alphaMask.Type()) - alphaMask;
            
            var channels = new Mat[3];
            // Perform alpha blending for each channel
            for (var c = 0; c < 3; c++) // Iterate over RGB channels
            {
                using var baseChannel = baseImage.ExtractChannel(c);
                using var overlayChannel = imageTheme.ExtractChannel(c);

                // Blend using alpha
                channels[c] = baseChannel.Mul(invertedAlphaMask) + overlayChannel.Mul(alphaMask);
            }
            
            // Blend the channels into a single BGR image
            using var blendedImage = new Mat();
            Cv2.Merge(channels, blendedImage);
            
            // Save or display the result
            Cv2.ImWrite(Path.Combine(targetDirectory, "openCvProcessed", $"final_frames_{frameIndex:D4}.png"), blendedImage);
            
            Console.WriteLine($"Saved frame {frameIndex}");
            frameIndex++;

            foreach (var channel in channels)
            {
                channel.Dispose();
            }
        }
    }
    
    public static void MergeToFinalFrameGraphic()
    {
        var videoPath = "camera-view-image_001.mp4"; // Path to the video file
        var outputFolder = "openCvFrames"; // Folder to save frames
        var targetDirectory = Directory.GetCurrentDirectory();
        
        // Ensure output folder exists
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        
        // Define the frame size
        // In order for finalImage.CopyTo(roi); to work, Cv2.ImRead flag must not be ImreadModes.Unchanged!!!
        // MatType.CV_8UC3 is the MatType when read PNG Image with ImreadModes.Color flag!!
        using var imageTheme = Cv2.ImRead(Path.Combine(targetDirectory, "133548626886541756_264A.png"),
            ImreadModes.Unchanged);
        using var baseImage = new Mat(new OpenCvSharp.Size(imageTheme.Width, imageTheme.Height), MatType.CV_8UC3);

        using var capture = new VideoCapture(videoPath);
        if (!capture.IsOpened())
        {
            Console.WriteLine("Failed to open video.");
            return;
        }

        var frameIndex = 1;
        using var frame = new Mat();

        while (capture.Read(frame))
        {
            if (frame.Empty())
            {
                break;
            }
            
            using var croppedImage = CropImageCenter(frame, 3, 2);
            using var resizedImage = ResizeImage(croppedImage, 916);
            using var finalImage = FlipImage(resizedImage);
            
            // Define the ROI on the base image
            var roiRect = new Rect(58, 58, finalImage.Width, finalImage.Height);
            var roi = new Mat(baseImage, roiRect);
            finalImage.CopyTo(roi);
            
            roiRect = new Rect(58, 718, finalImage.Width, finalImage.Height);
            roi = new Mat(baseImage, roiRect);
            finalImage.CopyTo(roi);
            
            roiRect = new Rect(58, 1378, finalImage.Width, finalImage.Height);
            roi = new Mat(baseImage, roiRect);
            finalImage.CopyTo(roi);
            
            roiRect = new Rect(58, 2038, finalImage.Width, finalImage.Height);
            roi = new Mat(baseImage, roiRect);
            finalImage.CopyTo(roi);
            roi.Dispose();
            
            // Process final frame by Graphics & Bitmap, quite slow!!
            using var imageLayout = new Bitmap(baseImage.Width, baseImage.Height);
            using var themeImage = ImageProcess.MatToImage(imageTheme);
            using var finalImageBitmap = ImageProcess.MatToImage(finalImage);

            using (var g = Graphics.FromImage(imageLayout))
            {
                g.DrawImage(finalImageBitmap, new Rectangle(58, 58, finalImageBitmap.Width, finalImageBitmap.Height));
                g.DrawImage(finalImageBitmap, new Rectangle(58, 718, finalImageBitmap.Width, finalImageBitmap.Height));
                g.DrawImage(finalImageBitmap, new Rectangle(58, 1378, finalImageBitmap.Width, finalImageBitmap.Height));
                g.DrawImage(finalImageBitmap, new Rectangle(58, 2038, finalImageBitmap.Width, finalImageBitmap.Height));
                
                g.DrawImage(themeImage, new Rectangle(0, 0, imageLayout.Width, imageLayout.Height));
                imageLayout.Save(Path.Combine(targetDirectory, "openCvProcessed", $"final_frames_{frameIndex:D4}.png"));
            }
        }
    }
    
    public static Mat FlipImage(Mat image)
    {
        var flippedHorizontally = new Mat();
        Cv2.Flip(image, flippedHorizontally, FlipMode.Y);
        return flippedHorizontally;
    }
    
    public static void FlipImage()
    {
        var targetDirectory = Directory.GetCurrentDirectory();
        
        var image = Cv2.ImRead(Path.Combine(targetDirectory, "openCvProcessed", "cropped_resized_Image.png"));
        
        var flippedHorizontally = new Mat();
        Cv2.Flip(image, flippedHorizontally, FlipMode.Y);
        Cv2.ImWrite(Path.Combine(targetDirectory, "openCvProcessed", "cropped_resized_flipped_Image.png"), flippedHorizontally);
    }

    public static Mat ResizeImage(Mat image, int desiredWidth)
    {
        // Calculate the aspect ratio
        var aspectRatio = (double)image.Height / image.Width;

        // Calculate the new height to maintain the aspect ratio
        var newHeight = (int)(desiredWidth * aspectRatio);

        // Resize the image
        var resizedImage = new Mat();
        Cv2.Resize(image, resizedImage, new OpenCvSharp.Size(desiredWidth, newHeight));

        return resizedImage;
    }
    
    public static void ResizeImage(int desiredWidth)
    {
        var targetDirectory = Directory.GetCurrentDirectory();
        
        // Load the image
        var image = Cv2.ImRead(Path.Combine(targetDirectory, "openCvProcessed", "cropped_Image.png"));

        // Calculate the aspect ratio
        var aspectRatio = (double)image.Height / image.Width;

        // Calculate the new height to maintain the aspect ratio
        var newHeight = (int)(desiredWidth * aspectRatio);

        // Resize the image
        var resizedImage = new Mat();
        Cv2.Resize(image, resizedImage, new OpenCvSharp.Size(desiredWidth, newHeight), 0, 0, InterpolationFlags.Linear);

        // Save or display the resized image
        Cv2.ImWrite(Path.Combine(targetDirectory, "openCvProcessed", "cropped_resized_Image.png"), resizedImage);
    }

    public static Mat CropImageCenter(Mat image, int widthRatio, int heightRatio)
    {
        // Desired aspect ratio (width:height, e.g., 16:9)
        var desiredRatio = (double)widthRatio / heightRatio;

        // Get original image dimensions
        var originalWidth = image.Width;
        var originalHeight = image.Height;

        // Calculate the crop dimensions
        int cropWidth, cropHeight;

        if (originalWidth / (double)originalHeight > desiredRatio)
        {
            // Image is wider than the desired aspect ratio
            cropHeight = originalHeight;
            cropWidth = (int)(cropHeight * desiredRatio);
        }
        else
        {
            // Image is taller than the desired aspect ratio
            cropWidth = originalWidth;
            cropHeight = (int)(cropWidth / desiredRatio);
        }

        // Calculate the top-left corner of the crop rectangle
        var x = (originalWidth - cropWidth) / 2;
        var y = (originalHeight - cropHeight) / 2;

        // Define the cropping rectangle
        var cropRect = new Rect(x, y, cropWidth, cropHeight);

        // Perform the crop
        return new Mat(image, cropRect);
    }
    
    public static void CropImageCenter(int widthRatio, int heightRatio)
    {
        var targetDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(targetDirectory, "camera-view-image.png"); // Replace with your image path
        var outputPath = "processed_image.png";
        // Load the image
        var image = Cv2.ImRead(inputPath);

        // Desired aspect ratio (width:height, e.g., 16:9)
        var desiredRatio = (double)widthRatio / heightRatio;

        // Get original image dimensions
        var originalWidth = image.Width;
        var originalHeight = image.Height;

        // Calculate the crop dimensions
        int cropWidth, cropHeight;

        if (originalWidth / (double)originalHeight > desiredRatio)
        {
            // Image is wider than the desired aspect ratio
            cropHeight = originalHeight;
            cropWidth = (int)(cropHeight * desiredRatio);
        }
        else
        {
            // Image is taller than the desired aspect ratio
            cropWidth = originalWidth;
            cropHeight = (int)(cropWidth / desiredRatio);
        }

        // Calculate the top-left corner of the crop rectangle
        var x = (originalWidth - cropWidth) / 2;
        var y = (originalHeight - cropHeight) / 2;

        // Define the cropping rectangle
        var cropRect = new Rect(x, y, cropWidth, cropHeight);

        // Perform the crop
        var croppedImage = new Mat(image, cropRect);

        // Save or display the cropped image
        Cv2.ImWrite(Path.Combine(targetDirectory, "openCvProcessed", "cropped_Image.png"), croppedImage);
    }
}

public class ImageProcessTestNatively
{
    public static void MergeToFinalFrame()
    {
        var videoPath = "camera-view-image_001.mp4"; // Path to the video file
        var outputFolder = "frames"; // Folder to save frames
        var targetDirectory = Directory.GetCurrentDirectory();
        
        // Ensure output folder exists
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        
        // Define the frame size
        var frameWidth = 1020;
        var frameHeight = 3040;
        var imageLayout = new Bitmap(frameWidth, frameHeight);
        using var g = Graphics.FromImage(imageLayout);
        using var imageTheme = Image.FromFile(Path.Combine(targetDirectory, "133548626886541756_264A.png"));

        using var capture = new VideoCapture(videoPath);
        if (!capture.IsOpened())
        {
            Console.WriteLine("Failed to open video.");
            return;
        }

        int frameIndex = 1;
        var frame = new Mat();

        while (capture.Read(frame))
        {
            if (frame.Empty())
            {
                break;
            }

            using var targetImage = ImageProcess.MatToImage(frame);
            using var targetImageAsBitmap = new Bitmap(targetImage);
            using var processedImage = CropResizeFlipImageFromCenter(targetImageAsBitmap, 3, 2, 916);
            
            g.DrawImage(processedImage, new Rectangle(58, 58, 916, 616));
            g.DrawImage(processedImage, new Rectangle(58, 718, 916, 616));
            g.DrawImage(processedImage, new Rectangle(58, 1378, 916, 616));
            g.DrawImage(processedImage, new Rectangle(58, 2038, 916, 616));
            g.DrawImage(imageTheme, new Rectangle(0, 0, imageTheme.Width, imageTheme.Height));

            // Save the final frame
            imageLayout.Save(Path.Combine(targetDirectory, "processedFrames", $"final_frame_{frameIndex:D4}.png"));

            Console.WriteLine($"Saved frame {frameIndex}");
            frameIndex++;
        }
    }

    public static void TestSingleImage()
    {
        var targetDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(targetDirectory, "camera-view-image.png"); // Replace with your image path
        var outputPath = "processed_image.png";

        using var originalImage = new Bitmap(inputPath);
        using var processedImage = CropResizeFlipImageFromCenter(originalImage, 3, 2, 916);
        processedImage.Save(outputPath);
    }
    
    public static Image CropResizeFlipImageFromCenter(Bitmap originalImage, double ratioWidth, double ratioHeight, int targetWidth)
    {
        // Define the desired crop ratio (e.g., 16:9)
        var desiredRatio = ratioWidth / ratioHeight;
        int originalWidth = originalImage.Width;
        int originalHeight = originalImage.Height;

        // Calculate the dimensions of the crop rectangle
        int cropWidth, cropHeight;

        if (originalWidth / (double)originalHeight > desiredRatio)
        {
            // Wider than the desired ratio
            cropHeight = originalHeight;
            cropWidth = (int)(cropHeight * desiredRatio);
        }
        else
        {
            // Taller than the desired ratio
            cropWidth = originalWidth;
            cropHeight = (int)(cropWidth / desiredRatio);
        }

        // Calculate the top-left point for cropping
        int cropX = (originalWidth - cropWidth) / 2;
        int cropY = (originalHeight - cropHeight) / 2;

        // Create the cropping rectangle
        var cropRect = new Rectangle(cropX, cropY, cropWidth, cropHeight);

        // Perform the crop
        using var croppedImage = originalImage.Clone(cropRect, originalImage.PixelFormat);
        
        // Calculate the new height while keeping the aspect ratio
        var aspectRatio = (double)originalImage.Height / originalImage.Width;
        var targetHeight = (int)(targetWidth * aspectRatio);

        using var croppedResizeImage = new Bitmap(originalImage, new Size(targetWidth, targetHeight));
        var flippedImage = new Bitmap(croppedResizeImage.Width, croppedResizeImage.Height);
        using var g = Graphics.FromImage(flippedImage);
        // Apply a horizontal flip transformation
        g.ScaleTransform(-1, 1); // Flip horizontally
        g.TranslateTransform(-croppedResizeImage.Width, 0); // Adjust position

        // Draw the original image onto the flipped graphics context
        g.DrawImage(croppedResizeImage, 0, 0);
        
        return flippedImage;
    }
    
    public static Image ResizeImage(Image originalImage, int targetWidth)
    {
        // Calculate the new height while keeping the aspect ratio
        var aspectRatio = (double)originalImage.Height / originalImage.Width;
        var targetHeight = (int)(targetWidth * aspectRatio);

        // Create a new bitmap with the desired dimensions
        return new Bitmap(originalImage, new Size(targetWidth, targetHeight));
    }
    
    public static Image HorizontallyFlipImage(Image originalImage)
    {
        // Create a new empty bitmap with the same size as the original image
        var flippedImage = new Bitmap(originalImage.Width, originalImage.Height);

        using var g = Graphics.FromImage(flippedImage);
        // Apply a horizontal flip transformation
        g.ScaleTransform(-1, 1); // Flip horizontally
        g.TranslateTransform(-originalImage.Width, 0); // Adjust position

        // Draw the original image onto the flipped graphics context
        g.DrawImage(originalImage, 0, 0);

        return flippedImage;
    }
}