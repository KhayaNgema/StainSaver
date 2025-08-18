using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

namespace StainSaver.Services
{
    public class BarcodeService
    {
        private readonly FileUploadService _fileUploadService;
        public BarcodeService(FileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        // Returns tuple with saved file path and generated barcode image bytes
        public async Task<(string filePath, byte[] imageBytes)> GenerateAndSaveBarcodeAsync(string barcodeValue, string fileName)
        {
            if (string.IsNullOrWhiteSpace(barcodeValue))
                throw new ArgumentException("barcodeValue cannot be null or empty.", nameof(barcodeValue));

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 80,
                    Width = 300,
                    Margin = 10
                }
            };

            var pixelData = writer.Write(barcodeValue);

            using var barcodeBitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
            var bitmapData = barcodeBitmap.LockBits(
                new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb);

            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                barcodeBitmap.UnlockBits(bitmapData);
            }

            int labelHeight = 30;
            int totalHeight = barcodeBitmap.Height + labelHeight;

            using var finalBitmap = new Bitmap(barcodeBitmap.Width, totalHeight);

            using (var graphics = Graphics.FromImage(finalBitmap))
            {
                graphics.Clear(Color.White);
                graphics.DrawImage(barcodeBitmap, 0, 0);

                using var font = new Font("Arial", 14, FontStyle.Regular);
                var textSize = graphics.MeasureString(barcodeValue, font);
                float xCenter = (finalBitmap.Width - textSize.Width) / 2;
                float yText = barcodeBitmap.Height + 5;

                using var brush = new SolidBrush(Color.Black);
                graphics.DrawString(barcodeValue, font, brush, xCenter, yText);
            }

            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                finalBitmap.Save(ms, ImageFormat.Png);
                imageBytes = ms.ToArray();
            }

            var uploadedFilePath = await _fileUploadService.SaveFileAsync(imageBytes, fileName);

            return (uploadedFilePath, imageBytes);
        }
    }
}
