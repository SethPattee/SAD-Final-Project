using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Models;

namespace FactorSADEfficiencyOptimizer.Models
{
    public record ProductionTarget
    {
        public Product? ProductTarget {  get; set; }
        public float TargetQuantity { get; set; }
        public int DueDate { get; set; }
        public float CurrentAmount { get; set; }
        public StatusEnum Status {  get; set; }
        public bool IsTargetEnabled { get; set; }
    }
}
