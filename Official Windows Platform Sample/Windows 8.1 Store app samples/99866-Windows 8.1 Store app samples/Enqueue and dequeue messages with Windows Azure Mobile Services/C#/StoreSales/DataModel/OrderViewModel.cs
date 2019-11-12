//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

namespace StoreSales
{
    using System.Collections.Generic;

    public class OrderViewModel : BindableBase
    {
        private IEnumerable<int> amountSource;
        private int selectedAmount = -1;
        private IEnumerable<string> productsSource;
        private string selectedProduct = string.Empty;
        private string customer = string.Empty;

        public OrderViewModel(IEnumerable<string> productsSource, IEnumerable<int> amountSource)
        {
            this.productsSource = productsSource;
            this.amountSource = amountSource;
        }

        public IEnumerable<string> ProductsSource
        {
            get { return this.productsSource; }
        }

        public string SelectedProduct
        {
            get { return this.selectedProduct; }

            set { this.SetProperty(ref this.selectedProduct, value); }
        }

        public IEnumerable<int> AmountsSource
        {
            get { return this.amountSource; }
        }

        public int SelectedAmount
        {
            get { return this.selectedAmount; }

            set { this.SetProperty(ref this.selectedAmount, value); }
        }

        public string Customer
        {
            get { return this.customer; }

            set { this.SetProperty(ref this.customer, value); }
        }

        public void CleanUp()
        {
            this.Customer = string.Empty;
            this.SelectedProduct = string.Empty;
            this.SelectedAmount = -1;
        }
    }
}
