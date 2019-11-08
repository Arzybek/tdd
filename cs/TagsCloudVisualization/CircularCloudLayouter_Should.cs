using System;
using System.Windows;
using FluentAssertions;
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
            ccl.Layout[0].X.Should().Be(-rectSize.Width / 2);
            ccl.Layout[0].Y.Should().Be(rectSize.Height / 2);
            ccl.Layout[0].Top.Should().Be(rectSize.Height / 2);
            ccl.Layout[0].Right.Should().Be(rectSize.Width / 2);
        }

        [TestCase(1000)]
        [TestCase(300)]
        [TestCase(50)]
        public void CCL_PutsRectangles(int count)
        {
            var rectSize = new Size(50, 50);
            for (var i = 0; i < count; i++)
            {
                ccl.PutNextRectangle(rectSize);
            }

            count.Should().Be(ccl.Layout.Count);
        }

        [TestCase(0, 0, TestName = "Two zeros")]
        [TestCase(-10, -10, TestName = "Two negatives")]
        [TestCase(-10, 0, TestName = "Negative and zero")]
        [TestCase(10, 0, TestName = "Positive and zero")]
        [TestCase(-10, 10, TestName = "Negative and Positive")]
        [TestCase(10, -10, TestName = "Positive and Negative")]
        public void CCL_ThrowsArgumentException_With_NegativeOrZeroSize(int width, int height)
        {
            var rectSize = new Size(width, height);
            ccl.Invoking(y => y.PutNextRectangle(rectSize)).ShouldThrow<ArgumentException>();
        }

        private void CheckIntersection_EachWithEach()
        {
            for (var i = 0; i < ccl.Layout.Count; i++)
            {
                var rect = ccl.Layout[i];
                for (var j = i + 1; j < ccl.Layout.Count; j++)
                {
                    rect.IntersectsWith(ccl.Layout[j]).Should().Be(false);
                }
            }
        }

        [Test]
        public void CCL_PutManyRandomSizedRectangles_NotIntersects()
        {
            Random rnd = new Random();

            for (var i = 0; i < 30; i++)
            {
                var height = rnd.Next(20, 100);
                var width = rnd.Next(20, 100);
                var rectSize = new Size(height, width);
                ccl.PutNextRectangle(rectSize);
            }

            CheckIntersection_EachWithEach();
        }

        [Test]
        public void CCL_PutManyTwoSizedRectangles_NotIntersects()
        {
            var rectSize = new Size(50, 50);
            var rectSize2 = new Size(30, 30);

            for (var i = 0; i < 40; i++)
            {
                if (i % 2 == 0)
                    ccl.PutNextRectangle(rectSize);
                else ccl.PutNextRectangle(rectSize2);
            }

            CheckIntersection_EachWithEach();
        }

        [Test]
        public void CCL_PutManyOneSizedRectangles_NotIntersects()
        {
            var rectSize = new Size(50, 50);

            for (var i = 0; i < 30; i++)
            {
                ccl.PutNextRectangle(rectSize);
            }

            CheckIntersection_EachWithEach();
        }

        private double getOuterCircleRadius()
        {
            int maxX = 0, minX = 0, maxY = 0, minY = 0;
            for (var i = 0; i < ccl.Layout.Count; i++)
            {
                if (ccl.Layout[i].Y > maxY)
                    maxY = ccl.Layout[i].Y;
                if (ccl.Layout[i].Y - ccl.Layout[i].Height < minY)
                    minY = ccl.Layout[i].Y - ccl.Layout[i].Height;
                if (ccl.Layout[i].X + ccl.Layout[i].Width > maxX)
                    maxX = ccl.Layout[i].X + ccl.Layout[i].Width;
                if (ccl.Layout[i].X < minX)
                    minX = ccl.Layout[i].X;
            }

            int X = Math.Max(Math.Abs(maxX), Math.Abs(minX));
            int Y = Math.Max(Math.Abs(maxY), Math.Abs(minY));

            var radiusInnerCircle = Math.Max(X, Y);
            var radiusOuterCircle = radiusInnerCircle * Math.Sqrt(2);
            return radiusOuterCircle;
        }

        [TestCase(9)]
        [TestCase(13)]
        [TestCase(5)]
        public void CCL_Layout_CheckThickness(int count)
        {
            var rectSize = new Size(50, 50);
            for (var i = 0; i < count; i++)
            {
                ccl.PutNextRectangle(rectSize);
            }

            var radiusOuterCircle = getOuterCircleRadius();
            var circleSquare = Math.PI * radiusOuterCircle * radiusOuterCircle;
            var rectanglesSquare = 0;

            for (var i = 0; i < ccl.Layout.Count; i++)
            {
                rectanglesSquare += ccl.Layout[i].Width * ccl.Layout[i].Height;
            }

            var outerCircleSquare = rectanglesSquare * Math.PI / 2;
            var actualAcceptableSquare = circleSquare * 0.5;

            (actualAcceptableSquare <= outerCircleSquare).Should().BeTrue();
        }

        [TestCase(9)]
        [TestCase(13)]
        [TestCase(5)]
        public void CCL_Layout_CheckCircleForm(int count)
        {
            var rectSize = new Size(50, 50);
            for (var i = 0; i < count; i++)
            {
                ccl.PutNextRectangle(rectSize);
            }

            var radiusOuterCircle = getOuterCircleRadius();
            foreach (var rect in ccl.Layout)
            {
                var rectangleRadiusVectorLen = new Vector(rect.X - ccl.Center.X, rect.Y - ccl.Center.Y).Length;
                (rectangleRadiusVectorLen <= radiusOuterCircle).Should().BeTrue();
            }
        }
    }
}