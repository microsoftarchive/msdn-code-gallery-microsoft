// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserInterface.Gateways;
using UserInterface.AdventureWorksService;


namespace UserInterface
{
    /// <summary>
    /// Interaction logic for ProductList.xaml
    /// </summary>
    public partial class ProductList : Window
    {

        ProductGateway gateway;


	/// <summary>
        /// Lauches the entry form on startup
        /// </summary>
        public ProductList()
        {
            InitializeComponent();
            gateway = new ProductGateway();
            ProductsListView.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(ProductsListView_MouseDoubleClick);     
        }

	/// <summary>
        /// Bind results of gateway.GetCategories() to the Product Category combo box at the top of the form.
        /// </summary>
        private void BindCategories()
        {
            CategoryComboBox.ItemsSource = gateway.GetCategories();
            CategoryComboBox.SelectedIndex = 0;
        }

	/// <summary>
        /// Binds results of gateway.GetProducts(string ProductName, ProductCategory p) to the ListView control.
        /// </summary>
        private void BindProducts()
        {
            if (CategoryComboBox.SelectedIndex > -1)
            {
                ProductsListView.ItemsSource = gateway.GetProducts(NameTextBox.Text, CategoryComboBox.SelectedItem as ProductCategory);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindCategories();            
        }

        private void ProductsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Product p = ProductsListView.SelectedItem as Product;
            ProductView window = new ProductView(gateway);
            window.Closed += new EventHandler(window_Closed);
            window.UpdateProduct(p);
            window.Show();
        }

	/// <summary>
        /// Call BindProducts() when Search button is clicked.
        /// </summary>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            BindProducts();
        }

        private void btnNewProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductView window = new ProductView(gateway);
            window.Closed += new EventHandler(window_Closed);
            window.Show();
        }

	/// <summary>
        /// Call gateway.DeleteProduct() to initiate delete of selected product, if product cannot be deleted gateway.DeleteProduct 
	/// does not return null and response is shown to user via MessageBox.
        /// </summary>
        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            Product p = ProductsListView.SelectedItem as Product;
            if (p != null)
            {                
                string returned = gateway.DeleteProduct(p);
                if (returned != null)
                {
                    MessageBox.Show(returned);
                }
                BindProducts();
            }
        }

	/// <summary>
        /// Refresh List when new category is selected
        /// </summary>
        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BindProducts();
        } 

        void window_Closed(object sender, EventArgs e)
        {
            BindCategories();           
        }


    }
}
