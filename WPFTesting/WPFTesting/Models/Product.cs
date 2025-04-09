using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.Models
{
    public class Product : INotifyPropertyChanged
    {
        private float _quantity;

        private Decimal _price;
        private Guid _catalogueKey = Guid.NewGuid();
        private float _recenlytAddedQuantity = 0;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public float Quantity
        {
            get => _quantity; 
            set
            {
                if (value > _quantity)
                {
                    _recenlytAddedQuantity = value - _quantity;
                }
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }
        public float RecentlyAddedQuantity
        {
            get
            {
                var toReturn = _recenlytAddedQuantity;
                _recenlytAddedQuantity = 0;
                return toReturn;
            }
        }
        public string ProductName
        {
            get {
				if (ProductCatalog.Products.TryGetValue(_catalogueKey, out GeneralProduct? genProd))
                {
                    return genProd.ProductName;
                }
                else
                {
                    return "No Name";
                }
            } 
            set
            {
                foreach (Guid Key in ProductCatalog.Products.Keys)
                {
                    if (ProductCatalog.Products.TryGetValue(Key, out GeneralProduct? Prod))
                    {
                        if (value == Prod.ProductName)
                        {
                            _catalogueKey = Key;
                            OnPropertyChanged(nameof(ProductName));
                            break;
                        }
                    }               
                }
				ProductCatalog.Products[_catalogueKey] = new GeneralProduct() { ProductName = value};
                OnPropertyChanged(nameof(ProductName));
            }
        }
        public List<string> CatalogNames
        {
            get => ProductCatalogNames.ProductNames;
        }
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        public Product ShallowCopy()
        {
            Product p = new Product();
            p.ProductName = this.ProductName;
            p.Price = this.Price;
            p.Quantity = this.Quantity;
            return p;
        }
    }
}
