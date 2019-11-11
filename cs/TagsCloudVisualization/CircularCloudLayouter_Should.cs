using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
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
        
        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                var bitmap = Visualiser.drawRectangles(ccl, 1000, 1000);
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                    "TearDown.png");
                bitmap.Save(path, ImageFormat.Png);
                TestContext.WriteLine("Tag cloud visualization saved to file {0}", path);
                return;
            }
        }

        [TestCase(1000, 1000)]
        [TestCase(300, 300)]
        [TestCase(50, 50)]
        public void ReturnCentredRectangle_OnFirstPut(int width, int height)
        {
            var rectSize = new Size(width, height);
            ccl.PutNextRectangle(rectSize);
            var expectedRect = new Rectangle(new Point(-width / 2, height / 2), rectSize);
            ccl.RectanglesList[0].ShouldBeEquivalentTo(expectedRect);
        }
        
        [Test]
        public void TearDown_With_ImageAndMessage()
        {
            ccl = new CircularCloudLayouter(new Point(500,500));
            var rectSize = new Size(50, 50);
            for (var i = 0; i < 30; i++)
            {
                ccl.PutNextRectangle(rectSize);
            }
            true.Should().BeFalse();
        }

        [TestCase(1000)]
        [TestCase(300)]
        [TestCase(50)]
        public void PutRectangles(int count)
        {
            var rectSize = new Size(50, 50);
            for (var i = 0; i < count; i++)
            {
                ccl.PutNextRectangle(rectSize);
            }

            ccl.RectanglesList.Should().HaveCount(count);
        }

        [TestCase(0, 0, TestName = "Two zeros")]
        [TestCase(-10, -10, TestName = "Two negatives")]
        [TestCase(-10, 0, TestName = "Negative and zero")]
        [TestCase(10, 0, TestName = "Positive and zero")]
        [TestCase(-10, 10, TestName = "Negative and Positive")]
        [TestCase(10, -10, TestName = "Positive and Negative")]
        public void ThrowArgumentException_With_NegativeOrZeroSize(int width, int height)
        {
            var rectSize = new Size(width, height);
            ccl.Invoking(y => y.PutNextRectangle(rectSize)).ShouldThrow<ArgumentException>();
        }

        private void CheckNotIntersection_EachWithEach()
        {
            for (var i = 0; i < ccl.RectanglesList.Count; i++)
            {
                var rect = ccl.RectanglesList[i];
                for (var j = i + 1; j < ccl.RectanglesList.Count; j++)
                {
                    rect.IntersectsWith(ccl.RectanglesList[j]).Should().Be(false);
                }
            }
        }

        [Test]
        public void PutManyRandomSizedRectangles_NotIntersects()
        {
            Random rnd = new Random();

            for (var i = 0; i < 30; i++)
            {
                var height = rnd.Next(20, 100);
                var width = rnd.Next(20, 100);
                var rectSize = new Size(height, width);
                ccl.PutNextRectangle(rectSize);
            }

            CheckNotIntersection_EachWithEach();
        }

        [Test]
        public void PutManyTwoSizedRectangles_NotIntersects()
        {
            var rectSize = new Size(50, 50);
            var rectSize2 = new Size(30, 30);

            for (var i = 0; i < 40; i++)
            {
                if (i % 2 == 0)
                    ccl.PutNextRectangle(rectSize);
                else ccl.PutNextRectangle(rectSize2);
            }

            CheckNotIntersection_EachWithEach();
        }

        [Test]
        public void PutManyOneSizedRectangles_NotIntersects()
        {
            initializeCclWithOneSizedRectangles(30);

            CheckNotIntersection_EachWithEach();
        }

        private Tuple<int, int, int, int> getMinAndMaxCoordinatesByAxis()
        {
            int maxX = ccl.Center.X, minX = ccl.Center.X, maxY = ccl.Center.Y, minY = ccl.Center.Y;
            foreach (var rect in ccl.RectanglesList)
            {
                if (rect.Y > maxY)
                    maxY = rect.Y;
                if (rect.Y - rect.Height < minY)
                    minY = rect.Y - rect.Height;
                if (rect.X + rect.Width > maxX)
                    maxX = rect.X + rect.Width;
                if (rect.X < minX)
                    minX = rect.X;
            }

            return Tuple.Create(minX, maxX, minY, maxY);
        }

        private double getOuterCircleRadius()
        {
            var minsAndMaxes = getMinAndMaxCoordinatesByAxis();
            var minX = minsAndMaxes.Item1;
            var maxX = minsAndMaxes.Item2;
            var minY = minsAndMaxes.Item3;
            var maxY = minsAndMaxes.Item4;

            int X = Math.Max(Math.Abs(maxX), Math.Abs(minX));
            int Y = Math.Max(Math.Abs(maxY), Math.Abs(minY));

            var radiusInnerCircle = Math.Max(X, Y);
            var radiusOuterCircle = radiusInnerCircle * Math.Sqrt(2);
            return radiusOuterCircle;
        }

        private void initializeCclWithOneSizedRectangles(int count, int width = 50, int height = 50)
        {
            var rectSize = new Size(50, 50);
            for (var i = 0; i < count; i++)
            {
                ccl.PutNextRectangle(rectSize);
            }
        }

        [TestCase(9)]
        [TestCase(16)]
        [TestCase(25)]
        public void Layout_CheckThickness(int count)
        {
            initializeCclWithOneSizedRectangles(count);

            var radiusOuterCircle = getOuterCircleRadius();
            var circleSquare = Math.PI * radiusOuterCircle * radiusOuterCircle;
            var rectanglesSquare = 0;

            for (var i = 0; i < ccl.RectanglesList.Count; i++)
            {
                rectanglesSquare += ccl.RectanglesList[i].Width * ccl.RectanglesList[i].Height;
            }

            var outerCircleSquare = rectanglesSquare * Math.PI / 2;
            var actualAcceptableSquare = circleSquare * 0.45;

            Assert.True(actualAcceptableSquare <= outerCircleSquare);
        }

        [TestCase(9)]
        [TestCase(16)]
        [TestCase(25)]
        public void Layout_CheckCircleForm(int count)
        {
            initializeCclWithOneSizedRectangles(count);
            var minsAndMaxes = getMinAndMaxCoordinatesByAxis();
            var minX = minsAndMaxes.Item1;
            var maxX = minsAndMaxes.Item2;
            var minY = minsAndMaxes.Item3;
            var maxY = minsAndMaxes.Item4;

            var maxRadVectLeftTop = ccl.RectanglesList.Where(rectangle => rectangle.X < 0 && rectangle.Y > 0)
                .Max(rectangle => new Vector(rectangle.X, rectangle.Y).Length);
            var maxRadVectRightBot = ccl.RectanglesList.Where(rectangle => rectangle.X > 0 && rectangle.Y < 0)
                .Max(rectangle => new Vector(rectangle.X + rectangle.Width, rectangle.Y-rectangle.Height).Length);
            var maxRadVectRightTop = ccl.RectanglesList.Where(rectangle => rectangle.X > 0 && rectangle.Y > 0)
                .Max(rectangle => new Vector(rectangle.X + rectangle.Width, rectangle.Y).Length);
            var maxRadVectLeftBot = ccl.RectanglesList.Where(rectangle => rectangle.X < 0 && rectangle.Y < 0)
                .Max(rectangle => new Vector(rectangle.X, rectangle.Y - rectangle.Height).Length);

            Assert.AreEqual(Math.Abs(minX), Math.Abs(maxX), 25);
            Assert.AreEqual(Math.Abs(minY), Math.Abs(maxY), 25);
            Assert.AreEqual(maxRadVectLeftBot, maxRadVectRightTop, 25);
            Assert.AreEqual(maxRadVectRightBot, maxRadVectLeftTop, 25);
            var xDiameter = Math.Abs(minX) + Math.Abs(maxX);
            var yDiameter = Math.Abs(minY) + Math.Abs(maxY);
            var leftToRightDiameter = maxRadVectLeftTop + maxRadVectRightBot;
            var rightToLeftDiameter = maxRadVectRightTop + maxRadVectLeftBot;
            Assert.AreEqual(xDiameter,yDiameter,25);
            Assert.AreEqual(leftToRightDiameter,rightToLeftDiameter,25);
        }
    }
}