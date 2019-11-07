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
using System.Windows.Shapes;
using UserInterface.AdventureWorksService;
using UserInterface.Gateways;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ProductView : Window
    {


        ProductGateway gateway;


	/// <summary>
        /// Lauches the entry form on startup
        /// </summary>
        public ProductView(ProductGateway gateway)
        {
            InitializeComponent();
            this.gateway = gateway;
        }

        /// <summary>
        /// If true then ProductView window is being used to create/add a new product.
        /// If false then ProductView window is being used to edit an existing product.
        /// </summary>
        private bool _FormCreateMode = true;

        private bool FormCreateMode
        {
            get
            {
                return _FormCreateMode;
            }
            set
            {
                _FormCreateMode = value;
            }
        }

        /// <summary>
        /// Product instance being edited or created.
        /// </summary>
        private Product product { get; set; }


        public void UpdateProduct(Product product)
        {
            this.product = gateway.GetProducts(product.Name, product.ProductCategory)[0];
            FormCreateMode = false;
            this.Title = "Edit " + product.Name;
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindCategories();
            if (FormCreateMode)
            {
                product = new Product();
            }
            BindProduct();
        }

        /// <summary>
        /// Bind properties of the product instance being updated/created to the corresponding TextBox.
        /// </summary>
        private void BindProduct()
        {
            txtProductNumber.DataContext = product;
            txtName.DataContext = product;
            txtListPrice.DataContext = product;
            txtColor.DataContext = product;
            CategoryComboBoxProductDetail.DataContext = product;
            txtModifiedDate.DataContext = product;
            txtSellStartDate.DataContext = product;
            txtStandardCost.DataContext = product;
        }

        /// <summary>
        /// Query for list of Categories and bind to ComboBox
        /// </summary>
        private void BindCategories()
        {
            CategoryComboBoxProductDetail.ItemsSource = gateway.GetCategories();
            CategoryComboBoxProductDetail.SelectedIndex = 0;
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (FormCreateMode)
            {
                product.ProductCategory = (ProductCategory)CategoryComboBoxProductDetail.SelectedItem;
                gateway.AddProduct(product);
            }
            else
            {
                product.ProductCategory = (ProductCategory)CategoryComboBoxProductDetail.SelectedItem;
                gateway.UpdateProduct(product);
            }
            this.Close();
        }
    }
}
