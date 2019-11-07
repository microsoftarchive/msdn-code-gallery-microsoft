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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Geofencing4SqSampleTasks
{
    public enum TraceLevel
    {
        Error = 0,
        Warn,
        Info,
        Debug
    }

    // Simple logging class that appends to a file
    sealed class Logger
    {
        private static StorageFile _storageFile;
        private static string _filename;
        private static Logger _log;
        private static SemaphoreSlim _semaphoreSlim;

        public static void Initialize(string filename)
        {
            Debug.Assert(_log == null);
            _log = new Logger(filename);
            _semaphoreSlim = new SemaphoreSlim(1);
        }

        public Logger(string filename)
        {
            Logger._filename = filename;
        }

        public static string FormatException(Exception ex)
        {
            return ex.ToString() + "\n" + ex.StackTrace;
        }

        public static string FormatLatLong(double latitude, double longitude)
        {
            return String.Format("({0:F6}, {1:F6})", latitude, longitude);
        }

        // Note: this returns async void instead of async Task for convenience
        // and because this is fire-and-forget
        public static async void Trace(TraceLevel level, string message)
        {
            await _semaphoreSlim.WaitAsync();

            if (null == _storageFile)
            {
                _storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(_filename, CreationCollisionOption.OpenIfExists);
            }

            try
            {
                await FileIO.AppendTextAsync(_storageFile, String.Format("{0:yyyy-MM-dd HH\\:mm\\:ss\\:ffff}\t[{1}]\t{2}\n", DateTime.Now, level, message));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Failed to log to '{0}': {1}", _storageFile.Path, e.Message);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
