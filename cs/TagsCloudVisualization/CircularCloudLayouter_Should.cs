using System;
using System.Windows;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace TagsCloudVisualization
{
    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private CircularCloudLayouter ccl;

        [SetUp]
        public void SetUp()
        {
            ccl = new CircularCloudLayouter(new Point(0, 0));
        }

        [TestCase(1000, 1000)]
        [TestCase(300, 300)]
        [TestCase(50, 50)]
        public void CCL_ReturnsCentredRectangle_OnFirstPut(int width, int height)
        {
            var rectSize = new Size(width, height);
            ccl.PutNextRectangle(rectSize);
            ccl.Field[0].X.Should().Be(-rectSize.Width/2);
            ccl.Field[0].Y.Should().Be(rectSize.Height/2);
            ccl.Field[0].Top.Should().Be(rectSize.Height/2);
            ccl.Field[0].Right.Should().Be(rectSize.Width/2);
        }

        [Test]
        public void ManyRandomSizedRectangles_NotIntersects()
        {
            Random rnd = new Random();

            for (var i = 0; i < 30; i++)
            {
                var height = rnd.Next(20, 100);
                var width = rnd.Next(20, 100);
                var rectSize = new Size(height, width);
                ccl.PutNextRectangle(rectSize);
            }

            for (var i = 0; i < ccl.Field.Count; i++)
            {
                var rect = ccl.Field[i];
                for (var j = i + 1; j < ccl.Field.Count; j++)
                {
                    rect.IntersectsWith(ccl.Field[j]).Should().Be(false);
                }
            }
        }

        [Test]
        public void ManyTwoSizedRectangles_NotIntersects()
        {
            var rectSize = new Size(50, 50);
            var rectSize2 = new Size(30, 30);

            for (var i = 0; i < 30; i++)
            {
                if (i % 2 == 0)
                    ccl.PutNextRectangle(rectSize);
                else ccl.PutNextRectangle(rectSize2);
            }

            for (var i = 0; i < ccl.Field.Count; i++)
            {
                var rect = ccl.Field[i];
                for (var j = i + 1; j < ccl.Field.Count; j++)
                {
                    rect.IntersectsWith(ccl.Field[j]).Should().Be(false);
                }
            }
        }

        [Test]
        public void ManyOneSizedRectangles_NotIntersects()
        {
            var rectSize = new Size(50, 50);

            for (var i = 0; i < 30; i++)
            {
                ccl.PutNextRectangle(rectSize);
            }

            for (var i = 0; i < ccl.Field.Count; i++)
            {
                var rect = ccl.Field[i];
                for (var j = i + 1; j < ccl.Field.Count; j++)
                {
                    rect.IntersectsWith(ccl.Field[j]).Should().Be(false);
                }
            }
        }

        [TestCase(9)]
        [TestCase(13)]
        [TestCase(5)]
        public void CheckThickness_And_CircleForm(int count)
        {
            var rectSize = new Size(50, 50);
            for (var i = 0; i < count; i++)
            {
                ccl.PutNextRectangle(rectSize);
            }

            int maxX = 0, minX = 0, maxY = 0, minY = 0;
            for (var i = 0; i < ccl.Field.Count; i++)
            {
                if (ccl.Field[i].Y > maxY)
                    maxY = ccl.Field[i].Y;
                if (ccl.Field[i].Y - ccl.Field[i].Height < minY)
                    minY = ccl.Field[i].Y - ccl.Field[i].Height;
                if (ccl.Field[i].X + ccl.Field[i].Width > maxX)
                    maxX = ccl.Field[i].X + ccl.Field[i].Width;
                if (ccl.Field[i].X < minX)
                    minX = ccl.Field[i].X;
            }

            int X = Math.Max(Math.Abs(maxX), Math.Abs(minX));
            int Y = Math.Max(Math.Abs(maxY), Math.Abs(minY));

            var radiusInnerCircle = Math.Max(X, Y);
            var radiusOuterCircle = radiusInnerCircle * Math.Sqrt(2);
            var circleSquare = Math.PI * radiusOuterCircle * radiusOuterCircle;

            var rectanglesSquare = 0;
            for (var i = 0; i < ccl.Field.Count; i++)
            {
                rectanglesSquare += ccl.Field[i].Width * ccl.Field[i].Height;
            }

            var expCircleSquare = rectanglesSquare * Math.PI / 2;

            var actualMaxSquare = circleSquare * 0.5;
            (actualMaxSquare <= expCircleSquare).Should().BeTrue();
            
            foreach (var rect in ccl.Field)
            {
                var rectRadVectorLen = new Vector(rect.X - ccl.Center.X, rect.Y - ccl.Center.Y).Length;
                (rectRadVectorLen <= radiusOuterCircle).Should().BeTrue();
            }
        }
    }
}