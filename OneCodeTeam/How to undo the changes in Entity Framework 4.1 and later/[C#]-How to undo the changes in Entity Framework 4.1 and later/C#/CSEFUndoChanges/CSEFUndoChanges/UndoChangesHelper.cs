/****************************** Module Header ******************************\
Module Name:  UndoChangesHelper.cs
Project:      CSEFUndoChanges
Copyright (c) Microsoft Corporation.

This sample demonstrates how to undo the changes in Entity Framework.
This file contains the methods that undo the changes in different levels 
using DbContext or ObjectcContext.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Reflection;

namespace CSEFUndoChanges
{
    public static class UndoChangesHelper
    {
        #region UndoOperationsOfDbContext
        /// <summary>
        /// This method undoes the changes in Context level using DbContext.
        /// </summary>
        /// <param name="context">Undo the changes in this context</param>
        public static void UndoDbContext(this DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentException();
            }

            // Undo the changes of the all entries.
            foreach (DbEntityEntry entry in context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    // Under the covers, changing the state of an entity from 
                    // Modified to Unchanged first sets the values of all 
                    // properties to the original values that were read from 
                    // the database when it was queried, and then marks the 
                    // entity as Unchanged. This will also reject changes to 
                    // FK relationships since the original value of the FK 
                    // will be restored.
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    // If the EntityState is the Deleted, reload the date from the database.  
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// This method undoes the changes in DbEntities level using DbContext.
        /// </summary>
        /// <typeparam name="T">Type of the DbEntities</typeparam>
        /// <param name="context">Undo the changes in this context</param>
        public static void UndoDbEntities<T>(this DbContext context) where T : class
        {
            if (context == null)
            {
                throw new ArgumentException();
            }

            // Undo the changes of the T type entries.
            foreach (DbEntityEntry<T> entry in context.ChangeTracker.Entries<T>())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    default: break;

                }
            }
        }

        /// <summary>
        /// This method undoes the changes in DbEntity level using DbContext.
        /// </summary>
        /// <param name="context">Undo the changes in this context</param>
        /// <param name="entity">Undo the changes of the Entity</param>
        public static void UndoDbEntity(this DbContext context, object entity)
        {
            if (context == null||entity==null)
            {
                throw new ArgumentException();
            }

            // Get the entry of the entity, and then undo the changes.
            DbEntityEntry entry = context.Entry(entity);
            if (entry != null)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// This method undoes the changes in DbEntity Property level using DbContext.
        /// </summary>
        /// <param name="context">Undo the changes in this context</param>
        /// <param name="entity">Undo the changes in the Entity</param>
        /// <param name="propertyName">Undo the changes of the Property</param>
        public static void UndoDbEntityProperty(this DbContext context, object entity, string propertyName)
        {
            if (context == null || entity == null||propertyName==null)
            {
                throw new ArgumentException();
            }

            try
            {
                DbEntityEntry entry = context.Entry(entity);
                if (entry.State == EntityState.Added || entry.State == EntityState.Detached)
                {
                    return;
                }

                // Get and Set the Property value by the Property Name.
                object propertyValue = entry.OriginalValues.GetValue<object>(propertyName);
                entry.Property(propertyName).CurrentValue = entry.Property(propertyName).OriginalValue;
            }
            catch
            {
                throw;
            }
        } 
        #endregion


        #region UndoOperationsOfObjectContext
        /// <summary>
        /// This method undoes the changes in Context level using ObjectContext.
        /// </summary>
        /// <param name="context">Undo the changes in this context</param>
        public static void UndoObjectContext(this ObjectContext context)
        {
            if (context == null)
            {
                throw new ArgumentException();
            }

            // If the states of the entities are Modified or Deleted, refresh the date from the database.
            IEnumerable<object> collection = from e in context.ObjectStateManager.GetObjectStateEntries
                                                 (System.Data.EntityState.Modified | System.Data.EntityState.Deleted)
                                             select e.Entity;
            context.Refresh(RefreshMode.StoreWins, collection);

            // If the states of the entities are Added, detach these new entities.
            IEnumerable<object> AddedCollection = from e in context.ObjectStateManager.GetObjectStateEntries
                                                      (System.Data.EntityState.Added)
                                                  select e.Entity;
            foreach (object addedEntity in AddedCollection)
            {
                context.Detach(addedEntity);
            }
        }

        /// <summary>
        /// This method undoes the changes in ObjectEntities level using ObjectContext.
        /// </summary>
        /// <typeparam name="T">Type of the ObjectEntites</typeparam>
        /// <param name="context">Undo the changes in this context</param>
        /// <param name="objectSets">Undo the changes in the objectSets</param>
        public static void UndoObjectEntities<T>(this ObjectContext context, ObjectSet<T> objectSets) 
            where T : EntityObject
        {
            if (context == null||objectSets==null)
            {
                throw new ArgumentException();
            }

            IEnumerable<T> collection = from o in objectSets.AsEnumerable()
                                        where o.EntityState == EntityState.Modified || 
                                        o.EntityState == EntityState.Deleted
                                        select o;
            context.Refresh(RefreshMode.StoreWins, collection);

            IEnumerable<T> AddedCollection = (from e in context.ObjectStateManager.GetObjectStateEntries
                                                  (System.Data.EntityState.Added)
                                              select e.Entity).ToList().OfType<T>();
            foreach (T entity in AddedCollection)
            {
                context.Detach(entity);
            }
        }

        /// <summary>
        /// This method undoes the changes in ObjectEntity level using ObjectContext.
        /// </summary>
        /// <param name="context">Undo the changes in this context</param>
        /// <param name="entity">Undo the changes of the entity</param>
        public static void UndoObjectEntity(this ObjectContext context, EntityObject entity)
        {
            if (context == null || entity == null)
            {
                throw new ArgumentException();
            }

            if (entity.EntityState == EntityState.Modified || entity.EntityState == EntityState.Deleted)
            {
                context.Refresh(RefreshMode.StoreWins, entity);
            }
            else if (entity.EntityState == EntityState.Added)
            {
                context.Detach(entity);
            }
        }

        /// <summary>
        /// This method undoes the changes in ObjectEntity Property level using ObjectContext.
        /// </summary>
        /// <param name="context">Undo the changes in this context</param>
        /// <param name="entity">Undo the changes in the entity</param>
        /// <param name="propertyName">Undo the changes of the Property</param>
        public static void UndoObjectEntityProperty
            (this ObjectContext context, EntityObject entity, string propertyName)
        {
            if (context == null || entity == null||propertyName==null)
            {
                throw new ArgumentException();
            }

            try
            {
                // Get the entry from the entity, so we can get the original values. And then we use the 
                // reflection to set the property value of the entity.
                ObjectStateEntry entry = context.ObjectStateManager.GetObjectStateEntry(entity);
                if (entry.State != EntityState.Added && entry.State != EntityState.Detached)
                {
                    object propertyValue = entry.OriginalValues[propertyName];
                    PropertyInfo propertyInfo = entity.GetType().GetProperty(propertyName);
                    propertyInfo.SetValue(entity, propertyValue, null);

                }
            }
            catch
            {
                throw;
            }
        } 
        #endregion
    }


}
