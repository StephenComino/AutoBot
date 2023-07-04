using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using OpenCvSharp.Extensions;
using System.IO;
using Microsoft.VisualBasic.ApplicationServices;
using static OpenCvSharp.LineIterator;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;

namespace AutoBot.Services
{
    /// <summary>
    /// THis class is responsible for Capturing a ScreenShot and adding it to the List
    /// </summary>
    public class ScreenCaptureService : IDisposable
    {
        private Mat _imageToCompare;
        private PeriodicTimer _timer;
        private CancellationTokenSource cts;
        private EventBus _bus;
        public Mat ImageToCompare { get => _imageToCompare; set => _imageToCompare = value; }

        public Bitmap Image { get; set; }
        public ScreenCaptureService(EventBus bus)
        {
            var time = new TimeSpan(0, 0, 4);
            _timer = new PeriodicTimer(time);
            cts = new CancellationTokenSource();
            img_display = new();
            result = new();
            init_match = new();
            _bus = bus;
            StartTimer();

        }

        public void Callback()
        {
            Console.Write("Hi");
        }
        public async void StartTimer()
        {
            while (await _timer.WaitForNextTickAsync(cts.Token))
            {
                StartScreenCapture();
                _bus.Publish(this, new TestEvent());
            }
        }
        public void StartScreenCapture()
        {
            var screenshotMatrix = GetScreenShot();
            var file = File.ReadAllBytes(@"C:\Users\SC106206\OneDrive - Aristocrat Gaming\Desktop\New2.png");

            // Create memory stream
            using (MemoryStream stream = new MemoryStream(file))
            using (Bitmap bitmap = new Bitmap(stream))
            {
                ImageToCompare = BitmapConverter.ToMat(bitmap);
                ImageMatch(screenshotMatrix);
            }
        }

        private Mat img_display;
        private Mat result;
        private Mat init_match;
        public void ImageMatch(Mat image)
        {
            img_display = new();
            result = new();
            init_match = new();
            image.CopyTo(img_display);

            int result_cols = image.Cols - image.Cols + 1;
            int result_rows = image.Rows - image.Rows + 1;

            Cv2.CvtColor(ImageToCompare, init_match, ColorConversionCodes.RGB2RGBA);

            result.Create(result_rows, result_cols, MatType.CV_8UC4);
            Cv2.MatchTemplate(image, init_match, result, TemplateMatchModes.SqDiff);
            Cv2.Normalize(result, result, 0, 1, NormTypes.MinMax, -1);

            Cv2.MinMaxLoc(result, out OpenCvSharp.Point minVal, out OpenCvSharp.Point maxVal);
 
            Cv2.Rectangle(img_display, minVal, new OpenCvSharp.Point(minVal.X + init_match.Cols, minVal.Y + init_match.Rows), new Scalar(254,254,254,254), 5, LineTypes.AntiAlias, 0);


            for (int y = 0; y < img_display.Rows; ++y)
                for (int x = 0; x < img_display.Cols; ++x)
                {
                    img_display.At<Vec4b>(y, x)[3] = 0;
                    // if pixel is white
                        if (((y >= minVal.Y) && (y <= (minVal.Y + init_match.Rows)) && (x >= minVal.X) && (x <= (minVal.X + init_match.Cols))))
                        {
                            // set alpha to zero:
                            img_display.At<Vec4b>(y, x)[3] = 255;
                            if ((img_display.At<Vec4b>(y, x)[0] != 254) && img_display.At<Vec4b>(y, x)[1] != 254 && img_display.At<Vec4b>(y, x)[2] != 254)
                            {
                                img_display.At<Vec4b>(y, x)[3] = 0;
                            }

                        }
                }

            Image = img_display.ToBitmap();
        }

        private Mat GetScreenShot()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                //bitmap.Save("C://test.jpg", ImageFormat.Jpeg);
                return BitmapConverter.ToMat(bitmap);
            }
        }

        public void Dispose()
        {
            cts.Cancel();
        }
    }
}
