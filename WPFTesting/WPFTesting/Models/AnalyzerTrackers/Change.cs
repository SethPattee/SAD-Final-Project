using FactorySADEfficiencyOptimizer.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.Models.AnalyzerTrackers;

public record Change
{
	public int DayMade { get; set; }
	public ActionEnum Action { get; set; }
	public Product neededProduct { get; set; } = new Product();
	public EndpointUIValues? shipmentReceiver { get; set; }
}
