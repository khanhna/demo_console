using OpenCvSharp;

namespace Demo_Console.Images;

public static class OpenCvExtensions
{
    public static Mat CropImageCenter(this Mat image, int widthRatio, int heightRatio)
    {
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
        var result = new Mat(image, cropRect);
        image.Dispose();
        return result;
    }
    
    public static Mat ResizeImage(this Mat image, int desiredWidth)
    {
        // Calculate the aspect ratio
        var aspectRatio = (double)image.Height / image.Width;

        // Calculate the new height to maintain the aspect ratio
        var newHeight = (int)(desiredWidth * aspectRatio);

        // Resize the image
        var resizedImage = new Mat();
        Cv2.Resize(image, resizedImage, new Size(desiredWidth, newHeight));
        image.Dispose();
        
        return resizedImage;
    }
    
    public static Mat FlipImage(this Mat image, FlipMode direction)
    {
        var result = new Mat();
        Cv2.Flip(image, result, direction);
        image.Dispose();
        
        return result;
    }
}