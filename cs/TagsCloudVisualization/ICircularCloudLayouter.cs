using System.Drawing;

namespace TagsCloudVisualization
{
    public abstract class MustInitialize<T>
    {
        public MustInitialize(T parameters)
        {

        }
    }
    
    public interface ICircularCloudLayouter
    {
        Rectangle PutNextRectangle(Size rectangleSize);
    }
}