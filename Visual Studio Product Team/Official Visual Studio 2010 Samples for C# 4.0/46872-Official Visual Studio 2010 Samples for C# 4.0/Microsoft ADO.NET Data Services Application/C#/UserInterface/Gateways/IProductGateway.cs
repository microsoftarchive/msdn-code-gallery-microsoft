// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserInterface.AdventureWorksService;


namespace UserInterface.Gateways
{
    public interface IProductGateway
    {
        IList<Product> GetProducts(string productName, ProductCategory category);
        IList<ProductCategory> GetCategories();
        string DeleteProduct(Product product);
        void UpdateProduct(Product product);
        void AddProduct(Product product);
    }
}
