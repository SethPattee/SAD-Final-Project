using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models
{
    public class Supplier : INotifyPropertyChanged
    {
        private string _name;
        public Guid Id { get; set; }
        public string Name {

            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(Name);
            }
        
        }
        public List<Product> Products { get; set; } = new List<Product>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
