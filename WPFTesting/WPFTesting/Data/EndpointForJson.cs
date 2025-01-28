using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Models;
using WPFTesting.Shapes;

namespace WPFTesting.Data
{
    public class EndpointForJson
    {
        public IVendor ep_node;
        public int Xpos;
        public int Ypos;

        public EndpointForJson()
        {
            ep_node = new EndpointNode();
            Xpos = 0;
            Ypos = 0;
        }

        public EndpointForJson(EndpointUIValues e)
        {
            ep_node = e.supplier;
            Xpos = e.Position.X;
            Ypos = e.Position.Y;
        }
    }
}
