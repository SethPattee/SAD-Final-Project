using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.Models;

public static class ProductCatalog
{
	public static Dictionary<Guid, GeneralProduct> Products { get; set; } = new () { };

}
public class GeneralProduct : INotifyPropertyChanged
{
	private string _productName = "";
	public string ProductName {

		get => _productName;
		set
		{
			_productName = value;
			OnPropertyChanged(nameof(ProductName));
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	protected void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
public static class ProductCatalogNames
{
	public static List<String> ProductNames 
	{ 
		get
		{
			var list = new List<String>();
			foreach (GeneralProduct product in ProductCatalog.Products.Values)
			{
				list.Add(product.ProductName);
			}
			return list;
		}
	}
}
