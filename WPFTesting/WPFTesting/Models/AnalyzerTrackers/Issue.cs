using FactorSADEfficiencyOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.Models.AnalyzerTrackers;

public record Issue
{
	public int DayFound;

	public StatusEnum Severity;
	public Change Solution { get; set; } = new Change();
}
