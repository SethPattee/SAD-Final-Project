using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorSADEfficiencyOptimizer.Models;

public record Issue
{
    public int DayFound;

    public StatusEnum Severity;
    public Change Solution { get; set; } = new Change();
}
