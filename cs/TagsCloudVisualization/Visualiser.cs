using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudVisualization
{
    public class Visualiser
    {
        public static void Main()
        {
            CircularCloudLayouter ccl = new CircularCloudLayouter(new Point(2000, 2000));
                //Random
                
            Random rnd = new Random();
            
            for (var i = 0; i < 500; i++)
            {
                var height = rnd.Next(50, 150);
                var width = rnd.Next(50, 150);
                var rectSize = new Size(height, width);
                ccl.PutNextRectangle(rectSize);
            }

            //Two sized
            
//            var rectSize = new Size(500, 300);
//            var rectSize2 = new Size(500, 500);
//            
//            for (var i = 0; i < 30; i++)
//            {
//                if (i % 2 == 0)
//                    ccl.PutNextRectangle(rectSize);
//                else ccl.PutNextRectangle(rectSize2);
//            }

            //One sized
            
//            var rectSize = new Size(100, 100);
//            
//            for (var i = 0; i < 1000; i++)
//            {
//                ccl.PutNextRectangle(rectSize);
//            }

            var bitmap = new Bitmap(4000, 4000);
            Graphics graphics = Graphics.FromImage(bitmap);
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(Color.Red);
            graphics.Clear(Color.White);
            graphics.FillRectangles(brush, ccl.field.ToArray());
            graphics.DrawRectangles(pen, ccl.field.ToArray());
            bitmap.Save("one_sized_many_with.png", ImageFormat.Png);
        }
    }
}