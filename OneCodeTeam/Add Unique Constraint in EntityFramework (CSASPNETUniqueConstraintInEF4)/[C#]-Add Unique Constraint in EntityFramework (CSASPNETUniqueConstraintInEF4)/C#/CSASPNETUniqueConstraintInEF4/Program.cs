/****************************** Module Header ******************************\
* Module Name:    Program.cs
* Project:        CSASPNETUniqueConstraintInEF4
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to add unique constraint in Entity Framework. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace CSASPNETUniqueConstraintInEF4
{
    ///<summary>
    ///A unique attribute
    ///</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UniqueKeyAttribute : Attribute
    {
    }

    /// <summary>
    ///  Try to analyze the ModelEntity by reflecting them one by one and fetch the properties
    ///  with "unique" attribute and then call Sql statement to create database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyDbGenerator<T> : IDatabaseInitializer<T> where T : DbContext
    {
        public void InitializeDatabase(T context)
        {
            context.Database.Delete();
            context.Database.Create();

            // Fetch all the parent class's public properties
            var fatherPropertyNames = typeof(DbContext).GetProperties().Select(pi => pi.Name).ToList();

            // Loop each dbset's T
            foreach (PropertyInfo item in typeof(T).GetProperties().Where(p => fatherPropertyNames.IndexOf(p.Name) < 0).Select(p => p))
            {
                // Fetch the type of "T"
                Type entityModelType = item.PropertyType.GetGenericArguments()[0];
                var allfieldNames = from prop in entityModelType.GetProperties()
                                    where prop.GetCustomAttributes(typeof(UniqueKeyAttribute), true).Count() > 0
                                    select prop.Name;
                foreach (string s in allfieldNames)
                {
                    context.Database.ExecuteSqlCommand("alter table " + entityModelType.Name + " add unique(" + s + ")");
                }
            }
        }
    }

    /// <summary>
    /// Category Class
    /// </summary>
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [UniqueKey] // Unique Key
        public int IdentifyCode { get; set; }
        public string Name { get; set; }
    }

    public class DbG : DbContext
    {
        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but 
        /// before the model has been locked down and used to initialize the context. 
        /// </summary>
        /// <param name="modelBuilder">It is used to map CLR classes to a database schema. </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //  Disable the default PluralizingTableNameConvention
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Category> Categories { get; set; }
    }

    /// <summary>
    /// Main
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // Get or set the database initialization strategy.
            Database.SetInitializer<DbG>(new MyDbGenerator<DbG>());

            using (DbG g = new DbG())
            {
                g.Categories.Add(new Category { Id = 1, Name = "Category" });
                g.Categories.Add(new Category { Id = 2, IdentifyCode = 2, Name = "Category" });

                // Save all changes
                g.SaveChanges();

                Console.WriteLine("OK");
            }
        }

    }
}
