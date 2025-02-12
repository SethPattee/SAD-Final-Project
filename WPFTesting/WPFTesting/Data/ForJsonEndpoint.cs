using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FactorySADEfficiencyOptimizer.Models;
using FactorySADEfficiencyOptimizer.Shapes;

namespace FactorySADEfficiencyOptimizer.Data;

public class ForJsonEndpoint
{
    public EndpointNode Supplier { get; set; }
    public int XPos { get; set; }
    public int YPos { get; set; }
    public ForJsonEndpoint()
    {
        Supplier = new EndpointNode();
        XPos = 10;
        YPos = 10;
    }
    public ForJsonEndpoint(EndpointUIValues s)
    {
        Supplier = (EndpointNode)s.supplier;
        XPos = s.Position.X;
        YPos = s.Position.Y;
    }
}
public class FromJsonEndpoint
{
    private EndpointUIValues _e = new EndpointUIValues();

    public FromJsonEndpoint(ForJsonEndpoint e)
    {
        _e.supplier = e.Supplier;
        _e.Position = new System.Drawing.Point() { X = e.XPos, Y = e.YPos };
    }
    public EndpointUIValues Supplier { get { return _e; } }
}
