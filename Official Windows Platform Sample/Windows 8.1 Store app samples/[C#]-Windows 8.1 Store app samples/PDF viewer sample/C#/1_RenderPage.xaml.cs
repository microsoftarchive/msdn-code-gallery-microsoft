//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using SDKTemplate;
using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls.Primitives;
using System.Threading.Tasks;
using Windows.Data.Pdf;

namespace PDFAPI
{    
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private PdfDocument _pdfDocument;   

        enum RENDEROPTIONS
        {
            NORMAL,
            ZOOM,
            PORTION
        }

        uint PDF_PAGE_INDEX = 0; //first page
        uint ZOOM_FACTOR = 3; //300% zoom
        Rect PDF_PORTION_RECT = new Rect(100, 100, 300, 400); //portion of a page
        string PDFFILENAME = "Assets\\Windows_7_Product_Guide.pdf"; //Pdf file

        public Scenario1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// This function would display the Image 
        /// </summary>
        public async Task DisplayImageFileAsync(StorageFile file)
        {            
            // Display the image in the UI.
            BitmapImage src = new BitmapImage();
            src.SetSource(await file.OpenAsync(FileAccessMode.Read));
            Image1.Source = src;           
        }
        
        /// <summary>
        /// This renders PDF page with render options 
        /// Rendering a pdf page requires following 3 steps
        ///     1. PdfDocument.LoadFromFileAsync(pdfFile) which returns pdfDocument
        ///     2. pdfDocument.GetPage(pageIndex) 
        ///     3. pdfPage.RenderToStreamAsync(stream) or pdfPage.RenderToStreamAsync(stream,pdfPageRenderOptions)
        /// </summary>
             
        private async Task RenderPDFPage(string pdfFileName,RENDEROPTIONS renderOptions)
        {
            try
            {         
                 StorageFile pdfFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(pdfFileName);
                 //Load Pdf File
                 
                 _pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile); ;

                if (_pdfDocument != null && _pdfDocument.PageCount > 0)
                {
                    //Get Pdf page
                    var pdfPage = _pdfDocument.GetPage(PDF_PAGE_INDEX);                    

                    if (pdfPage != null)
                    {
                        // next, generate a bitmap of the page
                        StorageFolder tempFolder = ApplicationData.Current.TemporaryFolder;
                                         
                        StorageFile jpgFile = await tempFolder.CreateFileAsync(Guid.NewGuid().ToString() + ".png", CreationCollisionOption.ReplaceExisting);
                        
                        if (jpgFile != null)
                        {
                            IRandomAccessStream randomStream = await jpgFile.OpenAsync(FileAccessMode.ReadWrite);
                            
                            PdfPageRenderOptions pdfPageRenderOptions = new PdfPageRenderOptions();
                            switch (renderOptions)
                            {
                                case RENDEROPTIONS.NORMAL:
                                    //Render Pdf page with default options
                                    await pdfPage.RenderToStreamAsync(randomStream);                                    
                                    break;
                                case RENDEROPTIONS.ZOOM:
                                    //set PDFPageRenderOptions.DestinationWidth or DestinationHeight with expected zoom value
                                    Size pdfPageSize = pdfPage.Size;
                                    pdfPageRenderOptions.DestinationHeight = (uint)pdfPageSize.Height * ZOOM_FACTOR;
                                    //Render pdf page at a zoom level by passing pdfpageRenderOptions with DestinationLength set to the zoomed in length 
                                    await pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);
                                    break;
                                case RENDEROPTIONS.PORTION:
                                    //Set PDFPageRenderOptions.SourceRect to render portion of a page
                                    pdfPageRenderOptions.SourceRect = PDF_PORTION_RECT;                                                                     
                                    //Render portion of a page
                                    await pdfPage.RenderToStreamAsync(randomStream, pdfPageRenderOptions);
                                    break;
                            }
                            await randomStream.FlushAsync();
                            
                            randomStream.Dispose();
                            pdfPage.Dispose();

                            await DisplayImageFileAsync(jpgFile);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                rootPage.NotifyUser("Error: " + err.Message, NotifyType.ErrorMessage);

            }
        }
        /// <summary>
        /// This is the click handler for the 'RenderPage' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  async void RenderPage_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                rootPage.NotifyUser("Rendering page...", NotifyType.StatusMessage);
               
                await RenderPDFPage(PDFFILENAME, RENDEROPTIONS.NORMAL); 
               
                rootPage.NotifyUser("Rendered page ", NotifyType.StatusMessage);
            }
            catch (Exception err)
            {
                rootPage.NotifyUser("Error: " + err.Message, NotifyType.ErrorMessage);                
            }
        }
        /// <summary>
        /// This is the click handler for the 'RenderPageZoom' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  async void RenderPageZoom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rootPage.NotifyUser("Rendering page at zoom level...", NotifyType.StatusMessage);

                await RenderPDFPage(PDFFILENAME, RENDEROPTIONS.ZOOM);

                rootPage.NotifyUser("Rendered page at zoom level", NotifyType.StatusMessage);
            }
            catch (Exception err)
            {
                rootPage.NotifyUser("Error: " + err.Message, NotifyType.ErrorMessage);             
            }                
        }
        /// <summary>
        /// This is the click handler for the 'RenderPagePortion' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RenderPagePortion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rootPage.NotifyUser("Rendering portion of a page...", NotifyType.StatusMessage);

                await RenderPDFPage(PDFFILENAME, RENDEROPTIONS.PORTION);

                rootPage.NotifyUser("Rendered portion of a page ", NotifyType.StatusMessage);
            }
            catch (Exception err)
            {
                rootPage.NotifyUser("Error: " + err.Message, NotifyType.ErrorMessage);
            }                       
         }     
      
    }
}
