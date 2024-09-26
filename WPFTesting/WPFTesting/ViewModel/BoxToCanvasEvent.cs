using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourNamespace;

namespace WPFTesting.ViewModel
{
    public class BoxToCanvasEvent
    {
        public event EventHandler<DraggableBox> BoxClicked;

        protected virtual void OnBoxClicked(DraggableBox d)
        {
            BoxClicked?.Invoke(this, d);
        }
    }
}
