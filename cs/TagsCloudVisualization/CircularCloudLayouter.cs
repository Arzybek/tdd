using System.Drawing;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter : MustInitialize<Point>, ICircularCloudLayouter
    {
        private Point _center;
        
        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            return Rectangle.Empty;
        }

        public CircularCloudLayouter(Point center) : base(center)
        {
            this._center = center;
        }
        
    }
}