using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace WebDataEntry.Web.Application
{
    public static class ImageExtension
    {

		// Orientations.
		public const int ExifOrientationID = 0x112; // 274
		public enum ExifOrientations
		{
			Unknown = 0,
			TopLeft = 1,
			TopRight = 2,
			BottomRight = 3,
			BottomLeft = 4,
			LeftTop = 5,
			RightTop = 6,
			RightBottom = 7,
			LeftBottom = 8,
		}

		private static ExifOrientations GetExifOrientation(this Image image)
		{
			// Return the orientation value.
			try
			{
				return (ExifOrientations)image.GetPropertyItem(ExifOrientationID).Value[0];
			}
			catch
			{
				return ExifOrientations.Unknown;
			}
		}

		public static bool IsPortrait(this Image image)
		{
			
			var exifOrientation = image.GetExifOrientation();
			switch (exifOrientation)
			{
				case ExifOrientations.LeftBottom:
					return true;
				case ExifOrientations.Unknown:
					return image.Width < image.Height;
				default:
					return false;
			}
		}
		
		public delegate Size GetSizingRightFunc(Size maxSize, Size imageSize);

		public static Size CalculateNewSize(in Size maxSize, Size originalImageSize, GetSizingRightFunc getSizingRightFunc)
        {
			var newMaxSize = getSizingRightFunc(maxSize, originalImageSize);

			float nPercentW = ((float)newMaxSize.Width / (float)originalImageSize.Width);
			float nPercentH = ((float)newMaxSize.Height / (float)originalImageSize.Height);
			float nPercent = Math.Min(nPercentH, nPercentW);

			var destWidth = (int)(originalImageSize.Width * nPercent);
			var destHeight = (int)(originalImageSize.Height * nPercent);

			return new Size(destWidth, destHeight);
		}

		public static Image ResizeImage(this Image imgToResize, Size maxSize)
		{
			ExifRotate(imgToResize);
			var newSize = CalculateNewSize(maxSize, imgToResize.Size, GetSizingRight);

			var bitmap = new Bitmap(newSize.Width, newSize.Height);
			using (var graphics = Graphics.FromImage(bitmap))
			{
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.DrawImage(imgToResize, 0, 0, newSize.Width, newSize.Height);
			}

			return bitmap;
		}

		private static Size GetSizingRight(Size maxSize, Size imageSize)
		{
			var originalMaxSize = maxSize;

			if (imageSize.Width > imageSize.Height)
			{
				// landscape
				if (originalMaxSize.Width < originalMaxSize.Height)
				{
					maxSize.Width = originalMaxSize.Height;
					maxSize.Height = originalMaxSize.Width;
				}
			}
			else
			{
				// portrait
				if (originalMaxSize.Width > originalMaxSize.Height)
				{
					maxSize.Width = originalMaxSize.Height;
					maxSize.Height = originalMaxSize.Width;
				}
			}

			return maxSize;
		}

		public static void SaveJPeg(this Image imgToSave, string filePath, int compression)
        {
            var eps = new EncoderParameters(1);
            eps.Param[0] = new EncoderParameter(Encoder.Quality, compression);
            var ici = GetEncoderInfo("image/jpeg");
            imgToSave.Save(filePath, ici, eps);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            var encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

		
		public static void ExifRotate(this Image img)
		{
			if (!img.PropertyIdList.Contains(ExifOrientationID))
				return;

			var prop = img.GetPropertyItem(ExifOrientationID);
			int val = BitConverter.ToUInt16(prop.Value, 0);
			var rot = RotateFlipType.RotateNoneFlipNone;

			if (val == 3 || val == 4)
				rot = RotateFlipType.Rotate180FlipNone;
			else if (val == 5 || val == 6)
				rot = RotateFlipType.Rotate90FlipNone;
			else if (val == 7 || val == 8)
				rot = RotateFlipType.Rotate270FlipNone;

			if (val == 2 || val == 4 || val == 5 || val == 7)
				rot |= RotateFlipType.RotateNoneFlipX;

			if (rot != RotateFlipType.RotateNoneFlipNone)
				img.RotateFlip(rot);
		}

	}


}