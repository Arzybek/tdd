﻿using System;
using System.Drawing;

namespace TagsCloudVisualization
{
    public class SpiralPointsGenerator
    {
        private double spiralCoeff = 1 / (2 * 3.14);
        private double angleStep = 3.14 / 8;
        private double angle = 0;
        
        public Point getNextSpiralPoint()
        {
            int X = (int) Math.Floor(spiralCoeff * angle * Math.Cos(angle));
            int Y = (int) Math.Floor(spiralCoeff * angle * Math.Sin(angle));
            var point = new Point(X, Y);
            angle += angleStep;
            return point;
        }
    }
}