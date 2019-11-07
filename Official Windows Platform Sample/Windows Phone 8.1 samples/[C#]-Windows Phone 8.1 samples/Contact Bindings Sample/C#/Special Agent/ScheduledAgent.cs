using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Shared_Library;
using Windows.Phone.SocialInformation.OnlineMedia;
using Windows.Phone.PersonalInformation;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Phone.SocialInformation;
using Microsoft.Phone.Info;
using Windows.Storage.Streams;

namespace Special_Agent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private ContactBindingManager contactBindingManager;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        async protected override void OnInvoke(ScheduledTask task)
        {
            Logger.Log("Agent", "- - - - - - - - - - - - -");
            Logger.Log("Agent", "Agent invoked -> " + task.Name);

            this.contactBindingManager = await ContactBindings.GetAppContactBindingManagerAsync(); 

            // Use the name of the task to differentiate between the ExtensilityTaskAgent 
            // and the ScheduledTaskAgent
            if (task.Name == "ExtensibilityTaskAgent")
            {
                List<Task> inprogressOperations = new List<Task>();

                OperationQueue operationQueue = await SocialManager.GetOperationQueueAsync();
                ISocialOperation socialOperation = await operationQueue.GetNextOperationAsync();

                while (null != socialOperation)
                {
                    Logger.Log("Agent", "Dequeued an operation of type " + socialOperation.Type.ToString());

                    try
                    {
                        switch (socialOperation.Type)
                        {
                            case SocialOperationType.DownloadRichConnectData:
                                await ProcessOperationAsync(socialOperation as DownloadRichConnectDataOperation);
                                break;

                            default:
                                // This should never happen
                                HandleUnknownOperation(socialOperation);
                                break;
                        }

                        // The agent can only use up to 20 MB
                        // Logging the memory usage information for debugging purposes
                        Logger.Log("Agent", string.Format("Completed operation {0}, memusage: {1}kb/{2}kb",
                           socialOperation.ToString(),
                           (int)((long)DeviceExtendedProperties.GetValue("ApplicationCurrentMemoryUsage")) / 1024,
                           (int)((long)DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage")) / 1024));


                        // This can block for up to 1 minute. Don't expect to run instantly every time.
                        socialOperation = await operationQueue.GetNextOperationAsync();
                    }
                    catch (Exception e)
                    {
                        Helpers.HandleException(e);
                    }
                }

                Logger.Log("Agent", "No more operations in the queue");
            }

            NotifyComplete();
        }

        /// <summary>
        /// Because the app writes contact bindings the agent will be invoked on a per need basis 
        /// (e.g.: the user opens a contact an swipes to the "Connect" pivot). On invocation, the agent is
        /// requested to write the ConnectTile information.
        /// </summary>
        /// <param name="operation">The operation that we need to perform</param>
        private async Task ProcessOperationAsync(DownloadRichConnectDataOperation operation)
        {

            Logger.Log("Agent", "DownloadRichConnectDataOperation Ids=(" +  string.Join(",", operation.Ids) + ")");

            try
            {
                await Helpers.ParallelForEach(operation.Ids, async (string remoteId) =>
                    {
                        Logger.Log("Agent", "Start sync for id = " + remoteId);
                        ContactBinding binding = await contactBindingManager.GetContactBindingByRemoteIdAsync(remoteId);
                        ServerApi.TileData tileData = await ServerApi.GetTileDataFromWebServiceAsync(remoteId);
                        binding.TileData = new ConnectTileData();
                        binding.TileData.Title = tileData.Title;
                        foreach(IRandomAccessStream stream in tileData.Images)
                        {
                            ConnectTileImage image = new ConnectTileImage();
                            await image.SetImageAsync(stream);
                            binding.TileData.Images.Add(image);
                        }                        
                        await contactBindingManager.SaveContactBindingAsync(binding);
                        Logger.Log("Agent", "Finish sync for id = " + remoteId);
                    });
            }
            catch (Exception e)
            {
                Helpers.HandleException(e);
            }
            finally
            {
                Logger.Log("Agent", "DownloadRichConnectDataOperation Ids=(" + string.Join(",", operation.Ids) + ") - completed");
                operation.SafeNotifyCompletion();
            }
        }

        private void HandleUnknownOperation(ISocialOperation operation)
        {
            try
            {
                Logger.Log("Agent", "Don't know how to handle the operation " + operation.Type.ToString());
            }
            catch (Exception e)
            {
                Helpers.HandleException(e);
            }
            finally
            {
                operation.SafeNotifyCompletion();
            }
        }
    }
}
