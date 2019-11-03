using System;
using System.Drawing;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace TagsCloudVisualization
{
    [TestFixture]
    public class CircularCloudLayouter_should
    {
        private CircularCloudLayouter ccl;

        [SetUp]
        public void SetUp()
        {
            ccl = new CircularCloudLayouter(new Point(0, 0));
        }

        [Test]
        public void returnsCentredRectangle_OnFirstPut()
        {
            var rectSize = new Size(500, 500);
            ccl.PutNextRectangle(rectSize);
            ccl.field[0].X.Should().Be(-250);
            ccl.field[0].Y.Should().Be(250);
            ccl.field[0].Top.Should().Be(250);
            ccl.field[0].Right.Should().Be(250);
        }

        [Test]
        public void NotIntersects_WithTwoRectangles()
        {
            var rectSize = new Size(500, 500);
            ccl.PutNextRectangle(rectSize);
            ccl.PutNextRectangle(rectSize);
            ccl.field[0].IntersectsWith(ccl.field[1]).Should().Be(false);
        }
    }
}