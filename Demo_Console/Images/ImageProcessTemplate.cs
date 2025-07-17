using System.Drawing;
using OpenCvSharp;
using Xabe.FFmpeg;
using Size = System.Drawing.Size;

namespace Demo_Console.Images;

public class ImageProcessTemplate
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
    private static readonly TimeSpan StandardRecordDuration = new(0, 0, 0, 10);
    
    /// <summary>
    /// <b>Only work with 60 fps and 30 fps input video</b>
    /// </summary>
    /// <param name="shootingDetailInfos"></param>
    public static async Task MergeToFinalFrameOpenCv(IList<ShootLayoutDetail> shootingDetailInfos)
    {
        var outputFolder = "openCvFrames"; // Folder to save frames
        var targetDirectory = Directory.GetCurrentDirectory();
        
        // Ensure output folder exists
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        
        // Define the frame size
        // In order for finalImage.CopyTo(roi); to work, Cv2.ImRead flag must not be ImreadModes.Unchanged!!!
        // MatType.CV_8UC3(standard 3 channel RGB) is the MatType when read PNG Image with ImreadModes.Color flag!!
        using var imageTheme = Cv2.ImRead(Path.Combine(targetDirectory, "133548626886541756_264A.png"),
            ImreadModes.Unchanged);
        using var rootImage = Cv2.ImRead(Path.Combine(targetDirectory, "133548626886541756_264A.png"));

        var videos = new Dictionary<string, VideoCapture>(2);

        foreach (var item in shootingDetailInfos.Where(x => !string.IsNullOrEmpty(x.VideoName))
                     .DistinctBy(x => x.VideoName))
        {
            var capture = new VideoCapture(item.VideoName);
            if (!capture.IsOpened())
            {
                Console.WriteLine("Failed to open video.");
                foreach (var video in videos)
                {
                    video.Value.Dispose();
                }
                return;
            }

            videos[item.VideoName] = capture;
        }
        
        if(videos.Count == 0) return;
        var captureRanges = ExtractFrameIndices(shootingDetailInfos, videos);
        
        var frameIndex = 1;
        var isReachedEnd = false;
        var jpegFormatCompressionParams = new ImageEncodingParam(ImwriteFlags.JpegQuality, 100);

        // We target to generate 30fps video
        for (var i = 0; i < 300; i++)
        {
            if(isReachedEnd) break;
            using var baseImage = rootImage.Clone();
            
            foreach (var capture in captureRanges)
            {
                if (string.IsNullOrEmpty(capture.VideoName))
                {// Fill white
                    var height = capture.Width * capture.HeighRatio / capture.WidthRatio;
                    using var whitePlaceholder = new Mat(new OpenCvSharp.Size(capture.Width, height), MatType.CV_8UC3);
                    whitePlaceholder.SetTo(new Scalar(255, 255, 255));
                    var roiRect = new Rect(capture.AxisX, capture.AxisY, capture.Width, height);
                    // Define the ROI on the base image
                    using var roi = new Mat(baseImage, roiRect);
                    whitePlaceholder.CopyTo(roi);
                    continue;
                }
                
                var video = videos[capture.VideoName];
                var frameToTake = capture.FrameStart + (video.Fps > 30 ? i * 2 : i);
                if (frameToTake > capture.FrameEnd) isReachedEnd = true;

                video.Set(VideoCaptureProperties.PosFrames, frameToTake);
                    
                using var frame = new Mat();
                // Read the frame at the specified position
                if (video.Read(frame) && !frame.Empty())
                {
                    using var processedFrame = frame.Clone().CropImageCenter(capture.WidthRatio, capture.HeighRatio)
                        .ResizeImage(capture.Width).FlipImage(FlipMode.Y);
                    var roiRect = new Rect(capture.AxisX, capture.AxisY, processedFrame.Width, processedFrame.Height);
                    // Define the ROI on the base image
                    using var roi = new Mat(baseImage, roiRect);
                    processedFrame.CopyTo(roi);
                }
                else
                {// Fill white
                    var height = capture.Width * capture.HeighRatio / capture.WidthRatio;
                    using var whitePlaceholder = new Mat(new OpenCvSharp.Size(capture.Width, height), MatType.CV_8UC3);
                    whitePlaceholder.SetTo(new Scalar(255, 255, 255));
                    var roiRect = new Rect(capture.AxisX, capture.AxisY, capture.Width, height);
                    // Define the ROI on the base image
                    using var roi = new Mat(baseImage, roiRect);
                    whitePlaceholder.CopyTo(roi);
                }
            }

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
            Cv2.ImWrite(Path.Combine(targetDirectory, outputFolder, $"final_frames_{frameIndex:D4}.jpg"), blendedImage, jpegFormatCompressionParams);
            
            Console.WriteLine($"Saved frame {frameIndex}");
            frameIndex++;

            foreach (var channel in channels)
            {
                channel.Dispose();
            }

            if (frameIndex % 100 == 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        foreach (var video in videos)
        {
            video.Value.Dispose();
        }
        
        // Force GC after processing Image
        GC.Collect();
        GC.WaitForPendingFinalizers();
        
        // Check if image files exist
        if (Directory.GetFiles(Path.Combine(targetDirectory, outputFolder), "*", SearchOption.TopDirectoryOnly).Length <
            30) return;
        
        // Set FFmpeg executables path (if needed, optional for auto-download)
        FFmpeg.SetExecutablesPath("C:\\ffmpeg");
        
        // Custom FFmpeg command-line input as a string
        var inputPath = $"{Path.Combine(targetDirectory, outputFolder, "final_frames_%04d.jpg")}";
        var outputPathNativeVideo = $"{Path.Combine(targetDirectory, outputFolder, "output_hwaccel_auto_30fps.mp4")}";
        var outputPathOptimizedVideo = $"{Path.Combine(targetDirectory, outputFolder, "output_hwaccel_auto_60fps.webm")}";
        
        if(File.Exists(outputPathNativeVideo)) File.Delete(outputPathNativeVideo);
        if(File.Exists(outputPathOptimizedVideo)) File.Delete(outputPathOptimizedVideo);

        // Execute the custom FFmpeg command
        try
        {
            var customArgs =
                $"-hwaccel auto -r 30 -i \"{inputPath}\" -r 30 -filter:v scale=720:-1 -c:v ffv1 -pix_fmt yuv420p -colorspace bt709 \"{outputPathNativeVideo}\"";

            // Start the conversion
            var conversion = FFmpeg.Conversions.New().AddParameter(customArgs);
            AddConversionOutputInfo(conversion);
            var result = await conversion.Start();
            
            Console.WriteLine($"Conversion native completed successfully! - {result.Duration:hh\\:mm\\:ss}");

            customArgs =
                $"-hwaccel auto -i \"{outputPathNativeVideo}\" -vf \"minterpolate='mi_mode=mci:mc_mode=aobmc:vsbmc=1:fps=60'\" -c:v libvpx-vp9 -crf 20 \"{outputPathOptimizedVideo}\"";
            
            // Start the conversion
            conversion = FFmpeg.Conversions.New().AddParameter(customArgs);
            AddConversionOutputInfo(conversion);
            result = await conversion.Start();

            Console.WriteLine($"Conversion optimized completed successfully! - {result.Duration:hh\\:mm\\:ss}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// In case 2 videos have different framerate, this should sync the frames count to result the same duration
    /// </summary>
    private static IList<ShootLayoutDetail> ExtractFrameIndices(IList<ShootLayoutDetail> shootTimes,
        IDictionary<string, VideoCapture> videos)
    {
        if(shootTimes.Count == 0) return [];

        var result = new ShootLayoutDetail[shootTimes.Count];
        var minimumDuration = TimeSpan.MaxValue;

        foreach (var video in videos)
        {
            var detail = shootTimes.Where(x => x.VideoName == video.Key).OrderBy(x => x.MarkedTime).ToArray();
            var shootingDurations = new List<TimeSpan>(detail.Length + 2);

            for (var i = 1; i < detail.Length; i++)
            {
                shootingDurations.Add(detail[i].MarkedTime - detail[i - 1].MarkedTime);
            }
            
            shootingDurations.Add(StandardRecordDuration);
            shootingDurations.Add(minimumDuration);

            minimumDuration = shootingDurations.Min();
        }

        for(var i = 0; i < shootTimes.Count; i++)
        {
            if (string.IsNullOrEmpty(shootTimes[i].VideoName))
            {
                result[i] = shootTimes[i].Clone();
                continue;
            }
            
            var step = 1000.0 / videos[shootTimes[i].VideoName].Fps;
            var maxDurationByFrames = minimumDuration.TotalMilliseconds / 1000 * videos[shootTimes[i].VideoName].Fps;
            
            var frameEnd = shootTimes[i].MarkedTime.TotalMilliseconds/step;
            var frameStart = Math.Max(1, frameEnd - maxDurationByFrames);

            var shootTimeCopy = shootTimes[i].Clone();
            shootTimeCopy.FrameStart = Convert.ToInt32(frameStart);
            shootTimeCopy.FrameEnd = Convert.ToInt32(frameEnd);
            result[i] = shootTimeCopy;
        }

        return result;
    }
    
    private static int[][] ExtractFrameIndices(ICollection<TimeSpan> shootTime, int recordFps)
    {
        if(shootTime.Count == 0) return [];
        
        var result = new int[shootTime.Count][];
        var step = 1000.0 / recordFps;
        var maxDurationByFrames = 10 * recordFps;
        var currentIdx = 0;
        
        foreach (var targetTime in shootTime)
        {
            var targetFrame = targetTime.TotalMilliseconds/step;
            var startFrame = Math.Max(1, targetFrame - maxDurationByFrames);

            result[currentIdx] = [Convert.ToInt32(startFrame), Convert.ToInt32(targetFrame)];
            currentIdx++;
        }

        var shortestFrameCount = result.MinBy(x => x[1] - x[0])!;
        var minimumFrameDuration = shortestFrameCount[1] - shortestFrameCount[0];

        foreach (var range in result)
        {
            if (range[1] - range[0] < minimumFrameDuration) continue;
            range[0] = range[1] - minimumFrameDuration;
        }

        return result;
    }

    private static void AddConversionOutputInfo(IConversion? conversion)
    {
        if(conversion == null) return;
        
        // Subscribe to output data received event
        conversion.OnDataReceived += (sender, data) =>
        {
            if (!string.IsNullOrEmpty(data.Data))
            {
                Console.WriteLine($"FFmpeg Output: {data.Data}");
            }
        };

        // Subscribe to progress updates (optional)
        conversion.OnProgress += (sender, progress) =>
        {
            Console.WriteLine($"Progress: {progress.Percent}%");
        };
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
            using var themeImage = ImageProcessTemplate.MatToImage(imageTheme);
            using var finalImageBitmap = ImageProcessTemplate.MatToImage(finalImage);

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
    
    /// <param name="brightness">This act as stepping, for our use case, start from 3 and try</param>
    /// <param name="contrast">This act as multiplier, for our use case, start from 1.05 and try</param>
    public static void AdjustBrightnessAndContrast(double brightness, double contrast)
    {
        var targetDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(targetDirectory, "camera-view-image.png"); // Replace with your image path
        var outputPath = "adjust_brightness_contrast_image.png";
        // Load the image
        var image = Cv2.ImRead(inputPath);
        
        // Adjust brightness and contrast
        using var outputImage = new Mat();
        image.ConvertTo(outputImage, MatType.CV_8UC3, contrast, brightness);
        
        Cv2.ImWrite(Path.Combine(targetDirectory, outputPath), outputImage);
    }

    public static void DenoiseImage()
    {
        var targetDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(targetDirectory, "camera-view-image.png"); // Replace with your image path
        var outputPath = "denoise_image.jpg";
        // Load the image
        using var image = Cv2.ImRead(inputPath);
        
        using var result = new Mat();
        
        // Cv2.FastNlMeansDenoisingColored(image, result, 6, 6);
        Cv2.BilateralFilter(image, result, 9, 75, 75);

        Cv2.ImWrite(Path.Combine(targetDirectory, outputPath), result, new ImageEncodingParam(ImwriteFlags.JpegQuality, 100));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetRate">Normally, value between 1.005 - 1.1</param>
    public static void AdjustImageSaturation(double targetRate)
    {
        var targetDirectory = Directory.GetCurrentDirectory();
        var inputPath = Path.Combine(targetDirectory, "camera-view-image.png"); // Replace with your image path
        var outputPath = "saturation_adjusted_image.png";
        // Load the image
        using var image = Cv2.ImRead(inputPath);
        
        // Convert to HSV color space
        using var hsvImage = new Mat();
        Cv2.CvtColor(image, hsvImage, ColorConversionCodes.BGR2HSV);
        
        // Split into H, S, and V channels
        var hsvChannels = Cv2.Split(hsvImage);
        var hue = hsvChannels[0];
        var saturation = hsvChannels[1];
        var value = hsvChannels[2];
        
        // Apply vibrance adjustment on the saturation channel
        using var adjustedSaturation = AdjustVibrance(saturation, targetRate); // Increase vibrance by a factor of 1.2

        // Merge back the channels
        using var adjustedHSV = new Mat();
        Cv2.Merge([hue, adjustedSaturation, value], adjustedHSV);

        // Convert back to BGR color space
        using var adjustedImage = new Mat();
        Cv2.CvtColor(adjustedHSV, adjustedImage, ColorConversionCodes.HSV2BGR);
        
        Cv2.ImWrite(Path.Combine(targetDirectory, outputPath), adjustedImage);

        foreach (var channel in hsvChannels)
        {
            channel.Dispose();
        }
    }
    
    private static Mat AdjustVibrance(Mat saturation, double factor)
    {
        // Clone the saturation channel to modify it
        var adjusted = saturation.Clone();

        // Iterate through each pixel to adjust vibrance
        for (var y = 0; y < adjusted.Rows; y++)
        {
            for (var x = 0; x < adjusted.Cols; x++)
            {
                var satValue = adjusted.At<byte>(y, x);
                var adjustedValue = satValue * (1 + factor * (1 - (satValue / 255.0)));
                adjusted.At<byte>(y, x) = (byte)Math.Min(Math.Max(adjustedValue, 0), 255);
            }
        }

        return adjusted;
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

            using var targetImage = ImageProcessTemplate.MatToImage(frame);
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