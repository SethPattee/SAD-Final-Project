using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Models;
using WPFTesting.Shapes;

namespace FactorSADEfficiencyOptimizer.Models;

public record Change
{
    public int DayMade {  get; set; }
    public ActionEnum Action { get; set; }
    public Product neededProduct { get; set; } = new Product();
    public EndpointUIValues? shipmentReceiver { get; set; }
}
