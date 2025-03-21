﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactorySADEfficiencyOptimizer.Models;

public static class ProductCatalog
{
	public static Dictionary<Guid, GeneralProduct> Products { get; set; } = new () { };

}
public class GeneralProduct // : INotifyPropertyChanged
{
	public string ProductName { get; set; } = "";

	//public event PropertyChangedEventHandler? PropertyChanged;
	//protected void OnPropertyChanged(string propertyName)
	//{
	//	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	//}
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
