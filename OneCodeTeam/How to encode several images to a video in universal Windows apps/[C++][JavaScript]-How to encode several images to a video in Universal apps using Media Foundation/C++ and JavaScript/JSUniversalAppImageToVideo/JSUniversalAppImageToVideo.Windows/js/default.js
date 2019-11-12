// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=392286
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var g_videoWidth = 640;
    var g_videoHeight = 480;
    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            window.items = new WinJS.Binding.List();
            args.setPromise(WinJS.UI.processAll().then(function ready() {
                document.getElementById("imagesBtn").addEventListener("click", openImages, false);
                document.getElementById("encodeBtn").addEventListener("click", encode, false);
                document.getElementById("imagesListView").addEventListener("iteminvoked", removeImage);
            }));

            
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    // Print message
    function displayInfo(text) {       
        document.getElementById("error").innerText = text;
        document.getElementById("error").style.color = "red";
        
        
    }
 
    var g_imageFiles = new Array();
    var g_videoFile;
    var g_picture;
    function openImages()
    {
        if (items.length != 0)
        {
            items.splice(0, items.length);
        }
        if (g_imageFiles.length != 0)
        {
            g_imageFiles.splice(0, g_imageFiles.length);
        }
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.viewMode = Windows.Storage.Pickers.PickerViewMode.thumbnail;
        openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.picturesLibrary;
        openPicker.fileTypeFilter.append(".jpg");
        openPicker.fileTypeFilter.append(".png");
        openPicker.fileTypeFilter.append(".bmp");
        openPicker.pickMultipleFilesAsync().then(function (files) {
            if(files.size > 0)
            {                
                files.forEach(function (file) {
                    g_imageFiles.push(file);
                    file.openAsync(Windows.Storage.FileAccessMode.read).then(function (stream) {                        
                        items.push({ picture: URL.createObjectURL(stream) });                        
                    });
                });                
            }
        });
    }

    function encode()
    {
        if (g_imageFiles.length == 0)
        {
            displayInfo("You must select one image at least.");
            return;
        }
        var savePicker = new Windows.Storage.Pickers.FileSavePicker();
        savePicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.videosLibrary;
        savePicker.suggestedFileName = "output";
        savePicker.defaultFileExtension = ".mp4";
        savePicker.fileTypeChoices.insert("MP4 file", [".mp4"]);
        var promise = savePicker.pickSaveFileAsync().then(function (videoFile) {
            if (videoFile) {
                g_videoFile = videoFile;
                return videoFile.openAsync(Windows.Storage.FileAccessMode.readWrite);
            }
        }).then(function (stream) {
            if (stream)
            {
                g_picture = new EncodeImages.PictureWriter(stream, g_videoWidth, g_videoHeight);
            }            
        });

        var imageWidth, imageHeight;

        var promiseArray = g_imageFiles.map(function (file) {
            promise = promise.then(function () {
                
                return file.properties.getImagePropertiesAsync().then(function (props) {
                    if (g_picture) {
                        imageWidth = props.width;
                        imageHeight = props.height;
                        return file.openAsync(Windows.Storage.FileAccessMode.read);
                    }
                    
                }).then(function (stream) {
                    if (stream)
                    {
                        return Windows.Graphics.Imaging.BitmapDecoder.createAsync(stream);
                    }
                    
                }).then(function (decoder) {
                    if (decoder)
                    {
                        // Transform the image size.
                        var scaleOfWidth = g_videoWidth / imageWidth;
                        var scaleOfHeight = g_videoHeight / imageHeight;
                        var scale = scaleOfWidth > scaleOfHeight ? scaleOfHeight : scaleOfWidth;
                        imageWidth *= scale;
                        imageHeight *= scale;
                        var transform = new Windows.Graphics.Imaging.BitmapTransform();
                        transform.scaledWidth = imageWidth;
                        transform.scaledHeight = imageHeight;
                        return decoder.getPixelDataAsync(
                            Windows.Graphics.Imaging.BitmapPixelFormat.bgra8,
                            Windows.Graphics.Imaging.BitmapAlphaMode.straight,
                            transform,
                            Windows.Graphics.Imaging.ExifOrientationMode.respectExifOrientation,
                            Windows.Graphics.Imaging.ColorManagementMode.colorManageToSRgb);
                    }
                    
                }).then(function (provider) {
                    if (provider)
                    {
                        var data = provider.detachPixelData();
                        for (var i = 0; i < 10; ++i) {
                            g_picture.addFrame(data, imageWidth, imageHeight);
                        }
                    }
                });
            });
            return promise;
            
        });
        WinJS.Promise.join(promiseArray).then(function () {
            if (g_picture)
            {
                g_picture.finalize();
                g_picture = null;
                displayInfo("The image files are encoded successfully.");
                var myVideo = document.getElementById("videoElement");
                myVideo.src = URL.createObjectURL(g_videoFile);
            }
            
        });        
    }

    function removeImage(eventInfo)
    {
        g_imageFiles.splice(eventInfo.detail.itemIndex, 1);
        items.splice(eventInfo.detail.itemIndex, 1);
        
    }
    app.start();
})();