using FactorySADEfficiencyOptimizer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace FactorySADEfficiencyOptimizer.UIComponent;

/// <summary>
/// Interaction logic for CommonProductWindow.xaml
/// </summary>
public partial class CommonProductWindow : Window, INotifyPropertyChanged
{
	public CommonProductWindow()
	{
		InitializeComponent();
		DataContext = ProductCatalog.Products.Values;
	}
	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged(string name)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
	//public void DeleteProductButton_Click(object sender, RoutedEventArgs e) //THis worked... but it producted too many bugs. Chose to remove the delete option.
	//{
	//	if (sender is Button button)
	//	{
	//		GeneralProduct prod = (GeneralProduct)button.DataContext;

	//		ProductCatalog.Products = ProductCatalog.Products.Where(p => p.Value.ProductName != prod.ProductName).ToDictionary();
	//		OnPropertyChanged(nameof(ProductCatalog.Products));
	//		DataContext = "";
	//		DataContext = ProductCatalog.Products.Values;//force refresh
	//	}
	//}
	public void AddNewProduct_Click(object sender, RoutedEventArgs e)
	{
		GeneralProduct newProduct = new GeneralProduct() { ProductName = "NEW PRODUCT" };
		ProductCatalog.Products[Guid.NewGuid()] = newProduct;
        var collectionView = (CollectionView)CollectionViewSource.GetDefaultView(ProductCatalog.Products.Values);
        collectionView.Refresh();

        // Scroll to the newly added product
        ProductList.SelectedItem = newProduct;
        ProductList.ScrollIntoView(newProduct);
    }
}
