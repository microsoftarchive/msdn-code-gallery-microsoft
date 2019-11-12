using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Windows.Phone.SocialInformation;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Phone.SocialInformation.OnlineMedia;
using System.Collections.Generic;
using Microsoft.Phone.Info;

namespace SpecialAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private OnlineMediaManager manager;

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
                Debugger.Log(1, "exception", "UnhandledException e=" + e.ToString());
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

            this.manager = await OnlineMediaManager.RequestMediaManagerAsync();

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
                            case SocialOperationType.DownloadAlbumItems:
                                await ProcessOperation(socialOperation as DownloadAlbumItemsOperation);
                                break;

                            case SocialOperationType.DownloadAlbumCover:
                                await ProcessOperation(socialOperation as DownloadAlbumCoverOperation);
                                break;

                            case SocialOperationType.DownloadImage:
                                // Improve performance by downloading the image binaries in parallel.
                                // The app is limitted to a maximum of 8 simultaneous network requests.
                                // Optimally, the maximum number of parallel operations is between 4-8.
                                // Throttle to 4 parallel image downloads.
                                if (inprogressOperations.Count >= 4)
                                {
                                    Task completed = await Task.WhenAny(inprogressOperations);
                                    inprogressOperations.Remove(completed);
                                }

                                // Don't wait, download in parallel 
                                inprogressOperations.Add(ProcessOperation(socialOperation as DownloadImageOperation));
                                break;

                            default:
                                // This should never happen
                                await ProcessOperation(socialOperation);
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

                // wait for all the operations to complete
                if (inprogressOperations.Count > 0)
                {
                    await Task.WhenAll(inprogressOperations);
                    inprogressOperations.Clear();
                }
            }

            NotifyComplete();
        }

        /// <summary>
        /// Because the app has registered to support "Photos" extension the agent will be invoked
        /// and requested to download the album items when the user taps on our tile 
        /// in the photos hub. This method handles the operation by synchronizing the local
        /// store with our web server (Bing Images).
        /// </summary>
        /// <param name="operation">The operation that we need to perform</param>
        private async Task ProcessOperation(DownloadAlbumItemsOperation operation)
        {
            try
            {
                Logger.Log("Agent", "DownloadAlbumItemsOperation(" + operation.ParentAlbumId + ")");

                if (string.IsNullOrEmpty(operation.ParentAlbumId))
                {
                    // An empty ParentAlbumId means that we need to populate the root album
                    OnlinePictureAlbum rootAlbum = await manager.GetRootPictureAlbumAsync();

                    // Let's add the top level albums
                    // Simulate server request to get the top level albums
                    // (note: this code sample builds the list statically)
                    List<BingApi.Album> topLevelAlbums = BingApi.GetAlbums();
                    foreach (BingApi.Album newAlbum in topLevelAlbums)
                    {
                        OnlinePictureAlbum album = await manager.GetPictureAlbumByRemoteIdAsync(newAlbum.id);
                        if (null == album)
                        {
                            album = manager.CreatePictureAlbum(newAlbum.id, rootAlbum.Id);
                            album.Title = newAlbum.title;

                            // The albums that have the RequiresAuthentication flag set will 
                            // trigger a DownloadImageOperation every time the Photos app needs
                            // to display an image that hasn't already been downloaded.
                            album.RequiresAuthentication = newAlbum.authRequired;

                            await manager.SavePictureAlbumAsync(album);
                        }
                    }
                }
                else
                {
                    // ParentAlbumId is the remote id of the album that we need to populate
                    OnlinePictureAlbum album = await manager.GetPictureAlbumByRemoteIdAsync(operation.ParentAlbumId);

                    // Make a network call to get the most recent VersionStamp
                    string newVersionStamp = await BingApi.GetVersionStampAsync(operation.ParentAlbumId);

                    // Is up to the app to use the VersionStamp on the album to best suit its needs.
                    // The OS doesn't use this value. The app can use it to determine if the album is
                    // up to date. Only trigger a full sync if the album is not up to date.
                    if (album.VersionStamp != newVersionStamp)
                    {                      
                        // Get all the pictures that are currently in the album
                        OnlineMediaItemQueryResult query = album.GetContentsQuery();
                        IList<IOnlineMediaItem> oldPictures = await query.GetItemsAsync();

                        // Mark all current pictures for deletion
                        Dictionary<string, OnlinePicture> photosToDelete = new Dictionary<string, OnlinePicture>();
                        foreach (IOnlineMediaItem item in oldPictures)
                        {
                            OnlinePicture onlinePicture = item as OnlinePicture;
                            if (null != onlinePicture)
                            {
                                photosToDelete.Add(onlinePicture.RemoteId, onlinePicture);
                            }
                        }

                        // Contact server to download the metadata for all the items in the album
                        List<BingApi.Photo> newPhotos = await BingApi.GetAllPhotosMetadataAsync(operation.ParentAlbumId);

                        // Fill with new photos
                        List<OnlinePicture> photosToSave = new List<OnlinePicture>();
                        // Save a reference to the most recent photo among the new photos to use it as the cover for the album
                        OnlinePicture mostRecent = null;
                        foreach (BingApi.Photo newPhoto in newPhotos)
                        {
                            OnlinePicture onlinePicture = await manager.GetPictureByRemoteIdAsync(newPhoto.id);
                            if (null == onlinePicture)
                            {
                                OnlinePicture localPhoto = manager.CreatePicture(newPhoto.id, album.Id);
                                localPhoto.Title = newPhoto.title;
                                localPhoto.CreationTime = newPhoto.timestamp;
                                localPhoto.ThumbnailSmallUrl = new Uri(newPhoto.smallThumbnailUrl); // required
                                localPhoto.ThumbnailLargeUrl = new Uri(newPhoto.largeThumbnailUrl); // required
                                //localPhoto.ContentUrl = new Uri(newPhoto.fullSizeUrl); // optional
                                photosToSave.Add(localPhoto);

                                // Check if the current photo is the most recent one
                                if (null == mostRecent || (localPhoto.CreationTime > mostRecent.CreationTime))
                                {
                                    mostRecent = localPhoto;
                                }                                

                                if (photosToSave.Count > 500)
                                {
                                    // Save the photos in one bulk transaction, note the transaction can't be larger than 1000,
                                    // but we break it into 500 for good measure
                                    await manager.SaveMediaItemsAsync(photosToSave);

                                    photosToSave.Clear();
                                }
                            }
                            else
                            {
                                // The photo is still on the server. Keep it.
                                photosToDelete.Remove(newPhoto.id);
                            }
                        }
                        
                        // Most server APIs will provide a distinct cover image
                        // Here we use the most recent photo update the cover image
                        if (null != mostRecent &&
                            album.CoverImageUrl != mostRecent.ThumbnailSmallUrl)
                        {
                            await album.SetCoverImageAsync(null);
                            album.CoverImageUrl = mostRecent.ThumbnailSmallUrl;
                        }

                        // Update last modified time
                        album.VersionStamp = newVersionStamp;

                        // Calling SavePictureAlbumAsync will flicker the entire album page.
                        // It can produce a poor user experience if called everytime 
                        // we receive a newDownloadAlbumItemsOperation.
                        // For that reason, we only call this when the album has been updated.
                        // So, we always check if the VersionStamp has been updated.
                        await manager.SavePictureAlbumAsync(album);

                        // Save all the photos in one bulk transaction:
                        if (photosToSave.Count > 0)
                        {
                            await manager.SaveMediaItemsAsync(photosToSave);
                        }

                        // Purge old photos
                        if (null != photosToDelete)
                        {
                            foreach (OnlinePicture photo in photosToDelete.Values)
                            {
                                await manager.DeleteMediaItemAsync(photo);
                            }
                        }
                        
                    }
                }
                Logger.Log("Agent", "Precessing DownloadAlbumItemsOperation for Id=" + operation.ParentAlbumId);
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

        /// <summary>
        /// Because the app has registered to support "Photos" extension the agent will be invoked
        /// and requested to download the album cover on a per need basis (e.g.: the user is 
        /// browsing the album covers). We only recevie this operation it the album cover url is missing or 
        /// the image binary is missing and the album is configured to require authentication 
        /// (i.e. : we have explicitly set the property RequiresAuthentication = true)
        /// </summary>
        /// <param name="operation">The operation that we need to perform</param>
        private async Task ProcessOperation(DownloadAlbumCoverOperation operation)
        {
            Logger.Log("Agent", "DownloadAlbumCoverOperation(" + operation.AlbumId + ")");

            try
            {
                OnlinePictureAlbum album = await manager.GetPictureAlbumByRemoteIdAsync(operation.AlbumId);
                if ((null != album.CoverImageUrl) &&
                    (!string.IsNullOrEmpty(album.CoverImageUrl.AbsolutePath)))
                {
                    await album.SetCoverImageAsync(await Helpers.MakeStreamFromUrl(album.CoverImageUrl));
                    await manager.SavePictureAlbumAsync(album);
                }
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

        /// <summary>
        /// Because the app has registered to support "Photos" extension the agent will be invoked
        /// and requested to download each image binary on a per need basis (e.g.: the user 
        /// has tapped on the photo). We only receive this operation for the albums that require 
        /// authentication (i.e. : we have explicitly set the property RequiresAuthentication = true)
        /// </summary>
        /// <param name="operation">The operation that we need to perform</param>
        private async Task ProcessOperation(DownloadImageOperation operation)
        {
            Logger.Log("Agent", "DownloadImageOperation(" + operation.PhotoId + ", " + operation.DesiredImageType + ")");

            try
            {
                OnlinePicture image = await manager.GetPictureByRemoteIdAsync(operation.PhotoId);

                // Download according to the desired image type

                if (operation.DesiredImageType == ImageType.SmallThumbnail)
                {
                    await image.SetThumbnailSmallAsync(await Helpers.MakeStreamFromUrl(image.ThumbnailSmallUrl));
                }
                else if (operation.DesiredImageType == ImageType.LargeThumbnail)
                {
                    await image.SetThumbnailLargeAsync(await Helpers.MakeStreamFromUrl(image.ThumbnailLargeUrl));
                }
                else if (operation.DesiredImageType == ImageType.FullSize)
                {
                    await image.SetContentAsync(await Helpers.MakeStreamFromUrl(image.ContentUrl));
                }

                await manager.SaveMediaItemAsync(image);

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

        private async Task ProcessOperation(ISocialOperation operation)
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
