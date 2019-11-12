//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using System.IO;
using Windows.ApplicationModel;

class SuspensionManager
{
    static private Dictionary<string, object> sessionState_ = new Dictionary<string, object>();
    static private List<Type> knownTypes_ = new List<Type>();
    private const string filename = "_sessionState.xml";

    // Provides access to the currect session state
    static public Dictionary<string, object> SessionState
    {
        get { return sessionState_; }
    }

    // Allows custom types to be added to the list of types that can be serialized
    static public List<Type> KnownTypes
    {
        get { return knownTypes_; }
    }

    // Save the current session state
    static async public Task SaveAsync()
    {
        // Get the output stream for the SessionState file.
        StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
        using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
        {
            // Serialize the Session State.
            DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), knownTypes_);
            serializer.WriteObject(transaction.Stream.AsStreamForWrite(), sessionState_);
            await transaction.CommitAsync();
        }
    }

    // Restore the saved sesison state
    static async public Task RestoreAsync()
    {
        // Get the input stream for the SessionState file.
        try
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(filename);
            if (file == null) return;
            using (IInputStream inStream = await file.OpenSequentialReadAsync())
            {
                // Deserialize the Session State.
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), knownTypes_);
                sessionState_ = (Dictionary<string, object>)serializer.ReadObject(inStream.AsStreamForRead());
            }
        }
        catch (Exception)
        {
            // Restoring state is best-effort.  If it fails, the app will just come up with a new session.
        }
    }
}
