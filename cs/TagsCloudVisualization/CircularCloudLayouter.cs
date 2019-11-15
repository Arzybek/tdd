﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using FluentAssertions;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter : MustInitialize<Point>, ICircularCloudLayouter
    {
        public Point Center { get; internal set; }

        public ReadOnlyCollection<Rectangle> RectanglesList
        {
            get { return layout.AsReadOnly(); }
        }
        private List<Rectangle> layout = new List<Rectangle>();
        
        private SpiralPointsGenerator spiralPointsGenerator = new SpiralPointsGenerator();

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
                var point = spiralPointsGenerator.GetNextSpiralPoint();
                var top = Center.Y + point.Y + rectangleSize.Height / 2;
                var left = Center.X + point.X - rectangleSize.Width / 2;
                var rect = new Rectangle(new Point(left, top),
                    rectangleSize);

                if (CheckIntersection(rect))
                    continue;
                rect = MoveToCenter(rect);
                layout.Add(rect);
                return rect;
            }
        }

        private Rectangle MoveToCenter(Rectangle rect)
        {
            var zeroVector = new Vector(0, 0);
            var rectCenterX = Center.X - (rect.X + rect.Width / 2);
            var rectCenterY = Center.Y - (rect.Y - rect.Height / 2);
            var rectRadiusVector = new Vector(rectCenterX, rectCenterY);
            double possibleXafterMove = rect.X;
            double possibleYafterMove = rect.Y;
            Rectangle tmpRect;

            if (!rectRadiusVector.Equals(zeroVector))
            {
                rectRadiusVector.Normalize();
                while (true)
                {
                    possibleXafterMove += rectRadiusVector.X;
                    possibleYafterMove += rectRadiusVector.Y;
                    tmpRect = new Rectangle((int) possibleXafterMove, (int) possibleYafterMove, rect.Width, rect.Height);
                    if (CheckIntersection(tmpRect))
                    {
                        possibleXafterMove -= rectRadiusVector.X;
                        possibleYafterMove -= rectRadiusVector.Y;
                        break;
                    }
                }
            }
            
            return new Rectangle((int) possibleXafterMove, (int) possibleYafterMove, rect.Width, rect.Height);
        }

        private bool CheckIntersection(Rectangle rect)
        {
            return layout.Any(rect.IntersectsWith);
        }
    }
}