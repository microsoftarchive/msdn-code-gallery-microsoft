using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Messaging;
using System.Windows.Shapes;

namespace SilverlightMessageConsumer
{
    /// <summary>
    /// This silverlight application sets up a message receiver, listening for 
    /// messages sent to a certain client ID. This ID is taken from the InitParameters
    /// collection. By way of example, it displays messages received and also filters
    /// products shown in a datagrid.
    /// </summary>
    public partial class MainPage : UserControl
    {
        internal LocalMessageReceiver myReceiver;
        private ObservableCollection<Product> myProducts;
        private PagedCollectionView myProductsView;

        public MainPage()
        {
            InitializeComponent();
            //Populate the datagrid
            myProducts = new ObservableCollection<Product>();
            myProducts.Add(new Product("Fork", "Tined item of cutlery", "$1.00"));
            myProducts.Add(new Product("Knife", "Item of cutlery for cutting", "$1.50"));
            myProducts.Add(new Product("Spoon", "Item of cutlery useful for soup", "$1.20"));
            myProducts.Add(new Product("Plate", "Flat item of crockery", "$2.50"));
            myProducts.Add(new Product("Bowl", "Deep item of crockery for soup or desserts", "$2.60"));
            myProducts.Add(new Product("Cup", "Small item of crockery for drinking from", "$2.10"));
            myProducts.Add(new Product("Saucer", "Item of cockery for catching spills", "$2.00"));
            myProducts.Add(new Product("Mug", "Large item of cockery for drinking from", "$2.90"));
            myProducts.Add(new Product("Salt Shaker", "Contains salt for seasoning", "$2.50"));
            myProducts.Add(new Product("Pepper Grinder", "Grinds pepper for seasoning", "$3.00"));
            //Use a PagedCollectionView so we can filter when a message arrives
            myProductsView = new PagedCollectionView(myProducts);
            this.dataGridProducts.ItemsSource = myProductsView;
        }

        internal void myReceiver_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //Display the message
            textBoxMessage.Text = e.Message;
            //Filter the datagrid
            if (textBoxMessage.Text.Length > 0)
            {
                myProductsView.Filter = null;
                myProductsView.Filter = new Predicate<object>(FilterProducts);
            }
            else
            {
                myProductsView.Filter = null;
            }
        }

        //This method tests whether a product should be displayed
        public bool FilterProducts(object p)
        {
            Product currentProduct = p as Product;
            if (currentProduct.Name.ToLower().StartsWith(this.textBoxMessage.Text.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    //This class defines Products to display in the DataGrid
    public class Product
    {
        private string name;
        private string description;
        private string price;

        public Product(string newName, string newDescription, string newPrice)
        {
            name = newName;
            description = newDescription;
            price = newPrice;
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        public string Price
        {
            get
            {
                return price;
            }
            set 
            {
                price = value;
            }
        }
    }
}
