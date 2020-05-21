// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

namespace MsdnReader
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Data;
    using System.Data.SqlServerCe;
    using System.Threading;

    using Microsoft.SubscriptionCenter.Sync;
    using Microsoft.SceReader;
    using Microsoft.SceReader.Data;
    using Microsoft.SceReader.View;

    /// <summary>
    /// Manage and persist the Read state of stories a user has read across newsreader sessions.
    /// </summary>
    public class ReadStatePersistence
    {
        /// <summary>SQLce table name to persist isRead state of this news reader.</summary>
        private static readonly string stateTableName = "MsdnReader_ReadStateTable";

        /// <summary>SQLce table name to persist edition names.</summary>
        private static readonly string editionsTableName = "MsdnReader_EditionsTable";

        /// <summary>A Logger that provides interface for event logging.</summary>
        private readonly Logger logger;

        /// <summary> The local cache database file. </summary>
        private SqlCeConnection localCacheDatabase;

        /// <summary> Sync cache helper to manage common feed database. </summary>
        private readonly SyncCacheHelper syncCacheHelper;

        /// <summary> View manager of this news reader application. </summary>
        private readonly ViewManager viewManager;

        /// <summary> Precompiled SQL INSERT command to add new story guid to persistence. </summary>
        private SqlCeCommand insertCommand;

        /// <summary> Precompiled SQL SELECT command to retrieve if a story guid is marked as read in persistence. </summary>
        private SqlCeCommand selectCommand;

        /// <summary> Precompiled SQL SELECT command to delete all stories of one edition. </summary>
        private SqlCeCommand deleteStoriesCommand;

        /// <summary> Precompiled SQL INSERT command to add new edition guid to persistence. </summary>
        private SqlCeCommand insertEditionCommand;

        /// <summary> Precompiled SQL SELECT command to retrieve edition guid and ID. </summary>
        private SqlCeCommand selectEditionCommand;

        /// <summary> Precompiled SQL DELET command to delete edition guid and ID. </summary>
        private SqlCeCommand deleteEditionCommand;

        /// <summary> True if instance is initialized and ready. </summary>
        private bool initialized;

        /// <summary> synchronize SQL connection access if used from multiple threads. </summary>
        private readonly object databaseSyncRoot = new object();

        /// <summary>
        /// Initialize read state persistence for given ViewManager.
        /// </summary>
        /// <param name="localCacheFolder">directory for cache database</param>
        /// <param name="databaseName">database name</param>
        /// <param name="logger">A Logger that provides interface for event logging. Can be null if no logging is required.</param>
        /// <param name="viewManager">View manager of this news reader</param>
        public ReadStatePersistence(string localCacheFolder, string databaseName, Logger logger, ViewManager viewManager)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException("databaseName");
            }

            this.logger = logger;
            this.syncCacheHelper = SyncCacheHelper.EnsureCacheDatabaseIsCreated(localCacheFolder, databaseName, "");
            this.viewManager = viewManager;

            this.localCacheDatabase = new SqlCeConnection();
            this.localCacheDatabase.ConnectionString = SyncCacheHelper.GetConnectionString(localCacheFolder, databaseName, "");
            this.localCacheDatabase.Open();

            PrepareDataTable();
        }

        /// <summary>
        /// Initiate loading persisted stories Read state from the database; actual work happens on a background thread.
        /// </summary>
        public void Load()
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException();
            }
            // defer actual work on background thread
            ThreadPool.QueueUserWorkItem(LoadCallback);
        }

        /// <summary>
        /// Initiate saving stories Read state to the database
        /// </summary>
        /// <param name="runInBackground">if true, save story state on a background worker thread. </param>
        public void Save(bool runInBackground)
        {
            if (!this.initialized)
            {
                throw new InvalidOperationException();
            }
            if (runInBackground)
            {
                // defer actual work on background thread
                ThreadPool.QueueUserWorkItem(SaveCallback);
            }
            else
            {
                // run on caller's thread
                SaveCallback(null);
            }
        }

        /// <summary>
        /// Close persistence.
        /// </summary>
        public void Close()
        {
            if (this.initialized)
            {
                this.localCacheDatabase.Close();
                this.initialized = false;
            }
        }

        /// <summary>
        /// Loads persisted stories Read state from the database; can run on a worker thread or UI thread.
        /// </summary>
        /// <param name="state">not used</param>
        private void LoadCallback(object state)
        {
            foreach (DataFeedContent edition in this.viewManager.Editions)
            {
                lock (this.databaseSyncRoot)
                {
                    int editionID = GetEditionID(edition.Guid);
                    foreach (Story story in edition.Stories)
                    {
                        this.selectCommand.Parameters["@editionID"].Value = editionID;
                        this.selectCommand.Parameters["@storyGuid"].Value = story.Guid;
                        SqlCeDataReader reader = this.selectCommand.ExecuteReader();

                        if (!reader.Read())
                        {
                            continue;
                        }
                        story.Read = true;
                    }
                }
            }
        }

        /// <summary>
        /// Saves stories Read state to the database; expected to run on a worker thread.
        /// </summary>
        /// <param name="state">not used</param>
        private void SaveCallback(object state)
        {
            lock (this.databaseSyncRoot)
            {
                // clear previous stories and editions:
                this.deleteStoriesCommand.ExecuteNonQuery();
                this.deleteEditionCommand.ExecuteNonQuery();

                // enumerate all stories and save Read state for stories read by user
                foreach (DataFeedContent edition in this.viewManager.Editions)
                {
                    int editionID = GetEditionID(edition.Guid);
                    foreach (Story story in edition.Stories)
                    {
                        if (!story.Read)
                        {
                            continue;
                        }

                        this.insertCommand.Parameters["@storyGuid"].Value = story.Guid;
                        this.insertCommand.Parameters["@editionID"].Value = editionID;
                        this.insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Returns editionID of give EditionGUID; will create a new entry into the editions table if unknown
        /// </summary>
        /// <param name="editionGuid">GUID of edition</param>
        /// <returns>Returns editionID of given EditionGUID</returns>
        private int GetEditionID(string editionGuid)
        {
            int editionID = -1;
            lock (this.databaseSyncRoot)
            {
                this.selectEditionCommand.Parameters["@editionGuid"].Value = editionGuid;
                SqlCeDataReader reader = this.selectEditionCommand.ExecuteReader();

                if (!reader.Read())
                {
                    // edition unknown, create new entry:
                    this.insertEditionCommand.Parameters["@editionGuid"].Value = editionGuid;
                    this.insertEditionCommand.ExecuteNonQuery();
                    // re-read to get newly created editionID
                    reader = this.selectEditionCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        editionID = reader.GetInt32(reader.GetOrdinal("EditionID"));
                    }
                }
                else
                {
                    editionID = reader.GetInt32(reader.GetOrdinal("EditionID"));
                }
            }
            return editionID;
        }
        /// <summary>
        /// Create table schemata and precompile SQL commands.
        /// </summary>
        private void PrepareDataTable()
        {
            if (this.initialized)
            {
                return;
            }
                
            if (!this.syncCacheHelper.IsCacheTableRegistered(ReadStatePersistence.stateTableName))
            {
                CreateSchema();
            }

            if (this.insertCommand == null)
            {
                // initialize and precompile commonly used SQL commands
                this.insertCommand = this.localCacheDatabase.CreateCommand();
                this.insertCommand.CommandText = "INSERT"
                                               + " INTO " + ReadStatePersistence.stateTableName
                                               + "     (StoryGuid, EditionID)"
                                               + " VALUES         (@storyGuid,  @editionID)";
                this.insertCommand.Parameters.Add("@storyGuid", SqlDbType.NVarChar, 256);
                this.insertCommand.Parameters.Add("@editionID", SqlDbType.Int);
                this.insertCommand.Prepare();

                this.selectCommand = this.localCacheDatabase.CreateCommand();
                this.selectCommand.CommandText = "SELECT StoryGuid"
                                               + " FROM " + ReadStatePersistence.stateTableName
                                               + " WHERE StoryGuid = @storyGuid"
                                               + "  AND EditionID = @editionID";
                this.selectCommand.Parameters.Add("@storyGuid", SqlDbType.NVarChar, 256);
                this.selectCommand.Parameters.Add("@editionID", SqlDbType.Int);
                this.selectCommand.Prepare();

                this.deleteStoriesCommand = this.localCacheDatabase.CreateCommand();
                this.deleteStoriesCommand.CommandText = "DELETE " + ReadStatePersistence.stateTableName;
                this.deleteStoriesCommand.Prepare();

                this.insertEditionCommand = this.localCacheDatabase.CreateCommand();
                this.insertEditionCommand.CommandText = "INSERT"
                                                      + " INTO " + ReadStatePersistence.editionsTableName
                                                      + "     (EditionGuid)"
                                                      + " VALUES  (@editionGuid)";
                this.insertEditionCommand.Parameters.Add("@editionGuid", SqlDbType.NVarChar, 256);
                this.insertEditionCommand.Prepare();

                this.selectEditionCommand = this.localCacheDatabase.CreateCommand();
                this.selectEditionCommand.CommandText = "SELECT EditionID, EditionGuid"
                                               + " FROM " + ReadStatePersistence.editionsTableName
                                               + " WHERE EditionGuid = @editionGuid";
                this.selectEditionCommand.Parameters.Add("@editionGuid", SqlDbType.NVarChar, 256);
                this.selectEditionCommand.Prepare();

                this.deleteEditionCommand = this.localCacheDatabase.CreateCommand();
                this.deleteEditionCommand.CommandText = "DELETE " + ReadStatePersistence.editionsTableName;
                this.deleteEditionCommand.Prepare();
            }
            this.initialized = true;
        }

        /// <summary>
        /// Creates data schema for read state persistence.
        /// </summary>
        private void CreateSchema()
        {
            // create database schema:
            SqlCeCommand cmd = this.localCacheDatabase.CreateCommand();
            cmd.CommandText =
                  "CREATE TABLE " + ReadStatePersistence.stateTableName + "  ("
                + "  StoryGuid nvarchar(256),"
                + "  EditionID int"
                + ")";
            cmd.ExecuteNonQuery();
            this.syncCacheHelper.RegisterCacheTable(ReadStatePersistence.stateTableName);

            cmd = this.localCacheDatabase.CreateCommand();
            cmd.CommandText =
                  "CREATE TABLE " + ReadStatePersistence.editionsTableName + "  ("
                + "  EditionID int IDENTITY PRIMARY KEY,"
                + "  EditionGuid nvarchar(256) UNIQUE"
                + ")";
            cmd.ExecuteNonQuery();
            this.syncCacheHelper.RegisterCacheTable(ReadStatePersistence.editionsTableName);
        }
    }
}