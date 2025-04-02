using FactorySADEfficiencyOptimizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FactorySADEfficiencyOptimizer.UIComponent.EventArguments
{
    public class SendEndpointDetailsEventArgs : EventArgs
    {
        public EndpointNode? EndpointModel;
    }
}
