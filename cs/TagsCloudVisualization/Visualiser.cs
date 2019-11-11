using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace TagsCloudVisualization
{
    public class Visualiser
    {
        public static Bitmap drawRectangles(CircularCloudLayouter ccl, int bitmapWidth=4000, int bitmapHeight=4000)
        {
            var bitmap = new Bitmap(bitmapWidth, bitmapHeight);
            var graphics = Graphics.FromImage(bitmap);
            var brush = new SolidBrush(Color.Black);
            var pen = new Pen(Color.Red, 10);
            graphics.Clear(Color.White);
            graphics.FillRectangles(brush, ccl.RectanglesList.ToArray());
            graphics.DrawRectangles(pen, ccl.RectanglesList.ToArray());
            return bitmap;
        }

        public static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                System.Console.WriteLine("Please enter name of file, count of rectangles and size (width, height)");
                System.Console.WriteLine("Usage: TagsCloudVisualization <file name> <count> <width> <height>");
                System.Console.WriteLine("Or use flag -l to enter name of file, and sizes separated by space (width, height)");
                System.Console.WriteLine("Usage: TagsCloudVisualization -l <file name> <width> <height> <width> <height>...");
                System.Console.WriteLine("Also you can use flag -r to generate random sizes, enter name of file, and count");
                System.Console.WriteLine("Usage: TagsCloudVisualization -r <file name> <count>");
                return;
            }
            if (args.Length == 4 && args[0]!="-l")
            {
                try
                {
                    string fileName = args[0];
                    int count = int.Parse(args[1]);
                    int width = int.Parse(args[2]);
                    int height = int.Parse(args[3]);
                    CircularCloudLayouter oneSizedCcl = new CircularCloudLayouter(new Point(2000, 2000));
                    for (var i = 0; i < count; i++)
                    {
                        oneSizedCcl.PutNextRectangle(new Size(width, height));
                    }

                    var bitmap = drawRectangles(oneSizedCcl);
                    bitmap.Save(fileName, ImageFormat.Png);
                    return;
                }
                catch (System.FormatException)
                {
                    System.Console.WriteLine("Please enter a numeric argument.");
                    System.Console.WriteLine("Usage: TagsCloudVisualization <file name> <count> <width> <height>");
                    return;
                }
            }
            if (args[0]=="-l")
            {
                try
                {
                    string fileName = args[1];
                    CircularCloudLayouter oneSizedCcl = new CircularCloudLayouter(new Point(2000, 2000));
                    for (var i = 2; i < args.Length-1; i+=2)
                    {
                        int width = int.Parse(args[i]);
                        int height = int.Parse(args[i+1]);
                        oneSizedCcl.PutNextRectangle(new Size(width, height));
                    }

                    var bitmap = drawRectangles(oneSizedCcl);
                    bitmap.Save(fileName, ImageFormat.Png);
                    return;
                }
                catch (Exception)
                {
                    System.Console.WriteLine("Usage: TagsCloudVisualization -l <file name> <width> <height> <width> <height>...");
                    return;
                }
            }
            if (args[0]=="-r")
            {
                try
                {
                    CircularCloudLayouter randomCcl = new CircularCloudLayouter(new Point(2000, 2000));
                    Random rnd = new Random();
                    string fileName = args[1];
                    int count = int.Parse(args[2]);
                    for (var i = 0; i < count; i++)
                    {
                        var height = rnd.Next(50, 200);
                        var width = rnd.Next(50, 200);
                        var rectSize = new Size(height, width);
                        randomCcl.PutNextRectangle(rectSize);
                    }

                    var bitmap = drawRectangles(randomCcl);
                    bitmap.Save(fileName, ImageFormat.Png);
                    return;
                }
                catch (Exception)
                {
                    System.Console.WriteLine("Usage: TagsCloudVisualization -r <file name> <count>");
                    return;
                }
            }
        }
    }
}