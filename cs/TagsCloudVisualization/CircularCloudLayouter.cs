using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter : MustInitialize<Point>, ICircularCloudLayouter
    {
        private Point _center;

        public Point Center
        {
            get { return _center; }
        }

        private List<Rectangle> field = new List<Rectangle>();

        public List<Rectangle> Field
        {
            get { return field; }
        }

        private double spiralCoeff = 1 / (2 * 3.14);
        private double angleStep = 3.14 / 8;
        private double angle = 0;

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Height <= 0 || rectangleSize.Width <= 0)
            {
                throw new ArgumentException("Width and Height should be positive numbers");
            }

            //var i = 0;
            while (true)
            {
                //i++;
                var point = getNextSpiralPoint();
                var top = _center.Y + point.Y + rectangleSize.Height / 2;
                var left = _center.X + point.X - rectangleSize.Width / 2;
                var rect = new Rectangle(new Point(left, top),
                    rectangleSize); // point - левый верхний угол, size - ширина/высота

                var flag = CheckIntersection(rect);
                if (flag)
                    continue;
                rect = moveToCenter(rect);
                field.Add(rect);
                //TestContext.WriteLine("It taked {0} iterations", i);
                return rect;
            }
        }

        public CircularCloudLayouter(Point center) : base(center)
        {
            this._center = center;
        }

        private Rectangle moveToCenter(Rectangle rect)
        {
            var zero = new Vector(0, 0);
            var rectCenterX = _center.X - (rect.X + rect.Width / 2);
            var rectCenterY = _center.Y - (rect.Y - rect.Height / 2);
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
            foreach (var rectangle in field)
            {
                if (getRect.IntersectsWith(rectangle))
                    return true;
            }

            return false;
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