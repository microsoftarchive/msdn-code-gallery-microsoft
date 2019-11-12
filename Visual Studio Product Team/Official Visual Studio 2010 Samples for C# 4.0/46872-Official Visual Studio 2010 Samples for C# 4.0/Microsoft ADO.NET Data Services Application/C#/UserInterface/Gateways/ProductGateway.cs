// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Configuration;
using UserInterface.AdventureWorksService;
using System.Data.Services.Client;

namespace UserInterface.Gateways
{

    public class ProductGateway : IProductGateway
    {

        /// <summary>
        /// DataServiceContext object representing the runtime context for the data service.
        /// </summary>
        AdventureWorksLTEntities context;

        /// <summary>
        /// URI representing the service entry point
        /// </summary>
        Uri serviceUri;

        /// <summary>
        /// Initialize DataServiceContext
        /// </summary>
        public ProductGateway()
        {
            serviceUri = new Uri("http://localhost:50000/AdventureWorks.svc");
            context = new AdventureWorksLTEntities(serviceUri);
            context.MergeOption = MergeOption.OverwriteChanges;
        }

        /// <summary>
        /// If no product name is specified return all products with the specified categoryId, otherwise return only products with the specified categoryId and product name.
        /// </summary>
 	/// <param name="productName">The product name used to query Products</param>
        /// <param name="category">The category used to query Products</param>
        public IList<Product> GetProducts(string productName, ProductCategory category)
        {
            IEnumerable<Product> query;
            
            int categoryId = category.ProductCategoryID;
            if (!String.IsNullOrEmpty(productName))
            {
                query = from p in context.Products.Expand("ProductCategory")
                        where p.ProductCategory.ProductCategoryID == categoryId && p.Name == productName
                        select p;
            }
            else
            {
                query = from p in context.Products.Expand("ProductCategory")
                        where p.ProductCategory.ProductCategoryID == categoryId
                        select p;
            }

            try
            {
                List<Product> productSet = query.ToList();
                return productSet;
            }catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Return all Product Categories
        /// </summary>
        public IList<ProductCategory> GetCategories()
        {
            return context.ProductCategories.ToList();
        }

        /// <summary>
        /// Try to delete the specified product, if product cannot be deleted, return error message "Product Cannot be Deleted"
        /// </summary>
	/// <param name="product">The product to be deleted</param>
        public string DeleteProduct(Product product)
        {
            context.DeleteObject(product);
            
            try
            {
                context.SaveChanges();
            }
            catch (DataServiceRequestException)
            {
                return "Product Cannot be Deleted";
            }
            return null;
        }

 	/// <summary>
        /// This method assumes that all fields have been changed and updates the entire entity, including the association to ProductCategory.
        /// Changes are sent to the server using SaveChangesOptions.Batch so that all operations are sent in a single HTTP request.
	/// </summary>
	/// <param name="product">The product to be Updated</param>
        public void UpdateProduct(Product product)
        {
            ProductCategory newCategory = product.ProductCategory;
            context.SetLink(product, "ProductCategory", newCategory);
            context.UpdateObject(product);
            context.SaveChanges(SaveChangesOptions.Batch);
        }

 	/// <summary>
        /// Add new product object to the DataServiceContext, and associat the object with an existing ProductCategory.
        /// Changes are sent to the server using SaveChangesOptions.Batch so that all operations are sent in a single HTTP request.
	/// </summary>
	/// <param name="product">The product to be Added</param>
        public void AddProduct(Product product)
        {
            product.rowguid = Guid.NewGuid();
            context.AddObject("Products", product);
            context.SetLink(product, "ProductCategory", product.ProductCategory);
            context.SaveChanges(SaveChangesOptions.Batch);
        }

    }
}
