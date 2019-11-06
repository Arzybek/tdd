using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudVisualization
{
    public class Visualiser
    {
        public static Bitmap drawRectangles(CircularCloudLayouter ccl)
        {
            var bitmap = new Bitmap(4000, 4000);
            Graphics graphics = Graphics.FromImage(bitmap);
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(Color.Red, 10);
            graphics.Clear(Color.White);
            graphics.FillRectangles(brush, ccl.Field.ToArray());
            graphics.DrawRectangles(pen, ccl.Field.ToArray());
            return bitmap;
        }

        public static void Main()
        {
            CircularCloudLayouter randomCcl = new CircularCloudLayouter(new Point(2000, 2000));
            Random rnd = new Random();
            for (var i = 0; i < 500; i++)
            {
                var height = rnd.Next(50, 150);
                var width = rnd.Next(50, 150);
                var rectSize = new Size(height, width);
                randomCcl.PutNextRectangle(rectSize);
            }

            CircularCloudLayouter twoSizedCcl = new CircularCloudLayouter(new Point(2000, 2000));
            for (var i = 0; i < 30; i++)
            {
                if (i % 2 == 0)
                    twoSizedCcl.PutNextRectangle(new Size(500, 300));
                else twoSizedCcl.PutNextRectangle(new Size(500, 500));
            }

            //One sized
            CircularCloudLayouter oneSizedCcl = new CircularCloudLayouter(new Point(2000, 2000));
            for (var i = 0; i < 30; i++)
            {
                oneSizedCcl.PutNextRectangle(new Size(250, 250));
            }

            CircularCloudLayouter testCcl = new CircularCloudLayouter(new Point(2000, 2000));
            for (var i = 0; i < 9; i++)
            {
                testCcl.PutNextRectangle(new Size(300, 300));
            }

            var bitmap = drawRectangles(testCcl);
            bitmap.Save("test.png", ImageFormat.Png);
        }
    }
}