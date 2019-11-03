using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudVisualization
{
    public class Visualiser
    {
        public static void Main()
        {
            CircularCloudLayouter ccl = new CircularCloudLayouter(new Point(2500, 2500));
            var rectSize = new Size(500, 500);
            for (var i = 0; i < 50; i++)
            {
                ccl.PutNextRectangle(rectSize);
            }
            
            var bitmap = new Bitmap(5000, 5000);
            Graphics graphics = Graphics.FromImage(bitmap);
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(Color.Red);
            graphics.Clear(Color.White);
            graphics.FillRectangles(brush, ccl.field.ToArray());
            graphics.DrawRectangles(pen, ccl.field.ToArray());
            bitmap.Save("image.png", ImageFormat.Png);
        }
    }
}