using System.ComponentModel;
using WPFTesting.Models;

namespace WPFTesting.Shapes
{
    public interface IVendorUIValues
    {
        int XPosition { get; set; }
        int YPosition { get; set; }
        IVendor supplier { get; set; }
        int XPosition1 { get; set; }
    }
}