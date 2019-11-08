using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter : MustInitialize<Point>, ICircularCloudLayouter
    {
        public Point Center
        {
            get;
        }

        private List<Rectangle> layout = new List<Rectangle>();
        public List<Rectangle> Layout
        {
            get { return layout; }
        }

        private double spiralCoeff = 1 / (2 * 3.14);
        private double angleStep = 3.14 / 8;
        private double angle = 0;
        
        public CircularCloudLayouter(Point center) : base(center)
        {
            this.Center = center;
        }
        
        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Height <= 0 || rectangleSize.Width <= 0)
            {
                throw new ArgumentException("Width and Height should be positive numbers");
            }

            while (true)
            {
                var point = getNextSpiralPoint();
                var top = Center.Y + point.Y + rectangleSize.Height / 2;
                var left = Center.X + point.X - rectangleSize.Width / 2;
                var rect = new Rectangle(new Point(left, top),
                    rectangleSize); 

                if (CheckIntersection(rect))
                    continue;
                rect = moveToCenter(rect);
                layout.Add(rect);
                return rect;
            }
        }

        private Rectangle moveToCenter(Rectangle rect)
        {
            var zero = new Vector(0, 0);
            var rectCenterX = Center.X - (rect.X + rect.Width / 2);
            var rectCenterY = Center.Y - (rect.Y - rect.Height / 2);
            var vector = new Vector(rectCenterX, rectCenterY);
            var tmp = new double[2] {rect.X, rect.Y};
            Rectangle tmpRect;

            if (!vector.Equals(zero))
                vector.Normalize();
            if (!vector.Equals(zero))
            {
                while (true)
                {
                    tmpRect = new Rectangle((int) tmp[0], (int) tmp[1], rect.Width, rect.Height);
                    if (CheckIntersection(tmpRect))
                        break;
                    tmp[0] += vector.X;
                    tmp[1] += vector.Y;
                }
            }

            var lastX = (int) (tmp[0] - vector.X);
            var lastY = (int) (tmp[1] - vector.Y);
            return new Rectangle(lastX, lastY, rect.Width, rect.Height);
        }

        private bool CheckIntersection(Rectangle getRect)
        {
            return layout.Any(getRect.IntersectsWith);
        }

        private Point getNextSpiralPoint()
        {
            int X = (int) Math.Floor(spiralCoeff * angle * Math.Cos(angle));
            int Y = (int) Math.Floor(spiralCoeff * angle * Math.Sin(angle));
            var point = new Point(X, Y);
            angle += angleStep;
            return point;
        }
    }
}