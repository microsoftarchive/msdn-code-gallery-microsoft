// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker.Fakes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Objects;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Implementation of IObjectSet based on in-memory data
    /// </summary>
    /// <typeparam name="TEntity">Type of data to be stored in set</typeparam>
    public sealed class FakeObjectSet<TEntity> : IObjectSet<TEntity> where TEntity : class
    {
        /// <summary>
        /// The underlying data of this set
        /// </summary>
        private HashSet<TEntity> data;

        /// <summary>
        /// IQueryable version of underlying data
        /// </summary>
        private IQueryable query;

        /// <summary>
        /// Initializes a new instance of the FakeObjectSet class.
        /// The instance contains no data.
        /// </summary>
        public FakeObjectSet()
        {
            this.data = new HashSet<TEntity>();
            this.query = this.data.AsQueryable();
        }

        /// <summary>
        /// Initializes a new instance of the FakeObjectSet class.
        /// The instance contains the supplied data data.
        /// </summary>
        /// <param name="testData">Data to be included in set</param>
        public FakeObjectSet(IEnumerable<TEntity> testData)
        {
            if (testData == null)
            {
                throw new ArgumentNullException("testData");
            }

            this.data = new HashSet<TEntity>(testData);
            this.query = this.data.AsQueryable();
        }

        /// <summary>
        /// Gets the type of elements in this set
        /// </summary>
        Type IQueryable.ElementType
        {
            get { return this.query.ElementType; }
        }

        /// <summary>
        /// Gets the expression tree for this set
        /// </summary>
        Expression IQueryable.Expression
        {
            get { return this.query.Expression; }
        }

        /// <summary>
        /// Gets the query provider for this set
        /// </summary>
        IQueryProvider IQueryable.Provider
        {
            get { return this.query.Provider; }
        }

        /// <summary>
        /// Adds a new item to this set
        /// </summary>
        /// <param name="entity">The item to add</param>
        public void AddObject(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.data.Add(entity);
        }

        /// <summary>
        /// Deletes a new item from this set
        /// </summary>
        /// <param name="entity">The item to delete</param>
        public void DeleteObject(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.data.Remove(entity);
        }

        /// <summary>
        /// Attaches a new item to this set
        /// </summary>
        /// <param name="entity">The item to attach</param>
        public void Attach(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.data.Add(entity);
        }

        /// <summary>
        /// Detaches a new item from this set
        /// </summary>
        /// <param name="entity">The item to detach</param>
        public void Detach(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.data.Remove(entity);
        }

        /// <summary>
        /// Gets an enumerator for this set
        /// </summary>
        /// <returns>Returns an enumerator for all items in this set</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        /// <summary>
        /// Gets a typed enumerator for this set
        /// </summary>
        /// <returns>Returns an enumerator for all items in this set</returns>
        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }
    }
}