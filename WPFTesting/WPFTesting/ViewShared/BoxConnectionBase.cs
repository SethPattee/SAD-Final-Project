using System.Windows.Shapes;
using YourNamespace;

namespace WPFTesting.ViewShared
{
    public class BoxConnectionBase
    {
        public SupplierElement ConnectedBox { get; set; }
        public Line ConnectionLine { get; set; }
    }
}