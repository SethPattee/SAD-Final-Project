using FactorySADEfficiencyOptimizer.UIComponent.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FactorySADEfficiencyOptimizer.UIComponent
{
    /// <summary>
    /// Interaction logic for ConfirmCloseScheduledShipmentDialogue.xaml
    /// </summary>
    public partial class ConfirmCloseScheduledShipmentDialogue : Window
    {
        public ConfirmCloseScheduledShipmentDialogue()
        {
            InitializeComponent();
        }

        public EventHandler? DialogueConfirmed;
        public void OnYesInput_Click(object sender, RoutedEventArgs e)
        {
            SendResponse(sender, true);
        }

        private void SendResponse(object sender, bool userChoiceFlag)
        {
            var choice = new CloseScheduledShipmentsEventArgs()
            {
                UserSaveChoice = userChoiceFlag
            };
            DialogueConfirmed?.Invoke(sender, choice);
            Close();
        }

        public void OnNoInput_Click(object sender, RoutedEventArgs e)
        {
            SendResponse(sender, false);
        }
    }
}
