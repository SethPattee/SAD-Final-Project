using System.Windows.Shapes;
using YourNamespace;

namespace WPFTesting.ViewShared
{
    public class BoxConnectionBase
    {
        public DraggableBox ConnectedBox { get; set; }
        public Line ConnectionLine { get; set; }
    }
}