using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Models
{
    public class ComponentToProductTransformer
    {
        private ObservableCollection<Product> _components;
        private Product _product;
        private Guid _id;
        public ObservableCollection<Product> Components
        {
            get => _components ?? (new ObservableCollection<Product>());
            set => _components = value;
        }

        public Product ResultingProduct
        {
            get => _product ?? (new Product()
            {
                Quantity = 1,
                Price = 0,
                ProductName = "Duck"
            });
            set => _product = value;
        }
        public Guid Id
        {
            get => _id;
            private set => _id = value;
        }

        public ComponentToProductTransformer()
        {
            Id = Guid.NewGuid();
            _components = new ObservableCollection<Product>();
            _product = new Product();
        }

        //public ComponentToProductTransformer()
        //{
        //    _components = new List<Product>();
        //    _product = new Product();
        //}
    }
}
