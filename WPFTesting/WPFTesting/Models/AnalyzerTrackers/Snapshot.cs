using FactorSADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.Models.AnalyzerTrackers;

public class Snapshot
{
	public List<EndpointUIValues> Endpoints { get; set; } = new List<EndpointUIValues>();
	public List<Shipment> Shipments { get; set; } = new List<Shipment>();
	public List<SupplierUIValues> Suppliers { get; set; } = new List<SupplierUIValues>();
	public List<ProductionTarget> Targets { get; set; } = new List<ProductionTarget>();
	public List<Product> ComponentsUsed { get; set; } = new List<Product>();
	public double todaysProfit { get; set; } = 0;
	public double todaysSpending { get; set; } = 0;
}
