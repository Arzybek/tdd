using System;
using System.Drawing;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

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

        [Test]
        public void CCL_ReturnsCentredRectangle_OnFirstPut()
        {
            var rectSize = new Size(500, 500);
            ccl.PutNextRectangle(rectSize);
            ccl.field[0].X.Should().Be(-250);
            ccl.field[0].Y.Should().Be(250);
            ccl.field[0].Top.Should().Be(250);
            ccl.field[0].Right.Should().Be(250);
        }
        
        [Test]
        public void ManyRandomSizedRectangles_NotIntersects()
        {
            Random rnd = new Random();
            
            for (var i = 0; i < 30; i++)
            {
                var height = rnd.Next(1, 100);
                var width = rnd.Next(1, 100);
                var rectSize = new Size(height, width);
                ccl.PutNextRectangle(rectSize);
            }

            for (var i = 0; i < ccl.field.Count; i++)
            {
                var rect = ccl.field[i];
                for (var j = i+1; j < ccl.field.Count; j++)
                {
                    rect.IntersectsWith(ccl.field[j]).Should().Be(false);
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

            for (var i = 0; i < ccl.field.Count; i++)
            {
                var rect = ccl.field[i];
                for (var j = i+1; j < ccl.field.Count; j++)
                {
                    rect.IntersectsWith(ccl.field[j]).Should().Be(false);
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

            for (var i = 0; i < ccl.field.Count; i++)
            {
                var rect = ccl.field[i];
                for (var j = i+1; j < ccl.field.Count; j++)
                {
                    rect.IntersectsWith(ccl.field[j]).Should().Be(false);
                }
            }
        }
    }
}