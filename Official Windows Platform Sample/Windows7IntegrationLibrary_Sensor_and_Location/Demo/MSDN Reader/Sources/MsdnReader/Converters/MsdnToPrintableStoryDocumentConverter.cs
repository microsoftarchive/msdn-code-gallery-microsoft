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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Xml.XPath;
using Microsoft.SceReader.Data;
using System.Windows.Documents;
using Microsoft.SceReader.Controls;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Threading;
using Microsoft.SceReader.Data.Feed;
using Microsoft.SceReader.Converters;

namespace MsdnReader
{
    class MsdnToPrintableStoryDocumentConverter : MsdnMagazineDocumentToFlowDocumentConverter
    {

        #region Constructors

        public MsdnToPrintableStoryDocumentConverter()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// ConvertAsync - produces formatted PrintableStoryFlowDocument and starts off async fetches for images sources of images inserted in
        /// the document
        /// </summary>
        public virtual void ConvertAsync(XPathDocument document, Story story, FlowDocumentStyleProvider styleProvider, object userState)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            XPathNavigator navigator = document.CreateNavigator();
            XPathNavigator msdn = XmlHelper.GetChildNavigator(navigator, "content");
            Story = story;

            // Get web figures for links pointing to code in browser, so these will work in a printed XPS document.
            // Images that open the  viewer window cannot work in this context
            GetWebFigures(msdn, story);

            GetFlowDocumentFromNavigatorAsync(msdn, story, styleProvider, userState);
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Event raised on successful completion, an error, or a cancellation of conversion
        /// </summary>
        public event EventHandler<ConversionCompletedEventArgs> ConversionCompleted;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates formatted FlowDocument
        /// </summary>
        protected virtual void GetFlowDocumentFromNavigatorAsync(XPathNavigator navigator, Story story, FlowDocumentStyleProvider styleProvider, object userState)
        {
            PrintableStoryFlowDocument flowDocument = new PrintableStoryFlowDocument();
            MsdnStoryToFlowDocumentConverter.ApplyStyle(flowDocument, GetFlowDocumentStyle(styleProvider));

            // Headline does not require async fetches, so the base class method can be used
            CreateHeadline(flowDocument, navigator, story, styleProvider);

            CreateBodyAsync(flowDocument, navigator, story, styleProvider, userState);
        }

        /// <summary>
        /// CreateBodyAsync calls CreateBody to produce the FlowDocument, then begins async download of image sources.
        /// If download is pending, or if there are no image sources in the document, it raises completed event
        /// </summary>
        protected virtual void CreateBodyAsync(PrintableStoryFlowDocument flowDocument, XPathNavigator navigator, Story story, FlowDocumentStyleProvider styleProvider, object userState)
        {
            if (!IsImageDownloadPending)
            {
                CreateBody(flowDocument, navigator, story, styleProvider);

                // Check if image source fetches are needed
                if (_pendingImageRequestStates != null && _pendingImageRequestStates.Count > 0)
                {
                    // Request image sources. Store the FlowDocument as our user state and exit. Completed event is raised when image fetches complete
                    _targetDocument = flowDocument;
                    _dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(GetImageSources), userState);
                    return;
                }
            }

            // Fallback: either there are no image sources to fetch, or downloads are pending. Raise completed event
            ConversionCompletedEventArgs args = new ConversionCompletedEventArgs(userState, flowDocument);
            _dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(RaiseConversionCompleted), args);
        }

    
        protected override void CreateBody(FlowDocument flowDocument, XPathNavigator navigator, Story story, FlowDocumentStyleProvider styleProvider)
        {
            XPathNavigator bodyNavigator = GetBodyNavigator(navigator);
            XPathNodeIterator bodyParagraphIterator = GetBodyParagraphIterator(bodyNavigator);
            List<ImageData> storyImageData = null;
            List<ImageReference> storyImages = GetStoryImages(story, out storyImageData);
            int imageCounter = 0;
            int imageSpacing = CalculateImageSpacing(bodyParagraphIterator, storyImages);

            if (bodyParagraphIterator != null)
            {
                // Iterate through sections
                while (bodyParagraphIterator.MoveNext())
                {

                    if (bodyParagraphIterator.CurrentPosition == 1 && imageSpacing > 0)
                    {
                        // Always put an image first if there is one
                        if (imageCounter < storyImages.Count)
                        {
                            Paragraph imageParagraph = CreateImageParagraph(story, storyImages[imageCounter], storyImageData[imageCounter], styleProvider);
                            imageCounter++;
                            flowDocument.Blocks.Add(imageParagraph);
                        }
                    }
                    else if (bodyParagraphIterator.CurrentPosition > 0 && imageSpacing > 0
                             && (bodyParagraphIterator.CurrentPosition % imageSpacing == 0))
                    {
                        if (imageCounter < storyImages.Count)
                        {
                            Paragraph imageParagraph = CreateImageParagraph(story, storyImages[imageCounter], storyImageData[imageCounter], styleProvider);
                            imageCounter++;
                            flowDocument.Blocks.Add(imageParagraph);
                        }
                    }

                    
                    // Get title of section
                    XPathNavigator titleNavigator = GetSectionTitleNavigator(bodyParagraphIterator.Current);
                    if (titleNavigator != null)
                    {
                        Paragraph paragraph = CreateSectionTitleParagraph(titleNavigator, story, styleProvider);
                        flowDocument.Blocks.Add(paragraph);
                    }

                    XPathNodeIterator innerBodyParagraphIterator = GetInnerBodyParagraphIterator(bodyParagraphIterator.Current);
                    if (innerBodyParagraphIterator != null)
                    {
                        while (innerBodyParagraphIterator.MoveNext())
                        {
                            Paragraph paragraph = null;
                            XPathNavigator codeNavigator = innerBodyParagraphIterator.Current.SelectSingleNode("Code");
                            if (codeNavigator == null)
                            {
                                paragraph = CreateBodyTextParagraph(innerBodyParagraphIterator.Current, story, styleProvider, innerBodyParagraphIterator.CurrentPosition);
                            }
                            else
                            {
                                paragraph = CreateCodeParagraph(codeNavigator, story, styleProvider);
                            }
                            flowDocument.Blocks.Add(paragraph);
                        }
                    }
                }
            }

            AddAuthorAndBioToBody(flowDocument, navigator, story, styleProvider);
            AddCopyrightNoticeToBody(flowDocument, navigator, story, styleProvider);
        }

        /// <summary>
        /// Override to get FlowDocumentStyle that gets style for print documents
        /// </summary>
        protected override Style GetFlowDocumentStyle(FlowDocumentStyleProvider styleProvider)
        {
            Style style = null;
            if (styleProvider != null)
            {
                style = styleProvider.ArticlePrintDocumentStyle;
            }
            if (style == null)
            {
                // No available style for print documents. Default to value returned by base
                style = base.GetFlowDocumentStyle(styleProvider);
            }
            return style;
        }

        /// <summary>
        /// Creates a paragraph with a PrintableStoryImageControl to display images, and adds the control to the list of pending image requests
        /// </summary>
        protected virtual Paragraph CreateImageParagraph(Story story, ImageReference imageReference, ImageData imageData, FlowDocumentStyleProvider styleProvider)
        {
            PrintableStoryImageControl imageControl = new PrintableStoryImageControl();
            imageControl.ImageReference = imageReference;
            imageControl.ImageData = imageData;
            imageControl.Story = story;
            MsdnStoryToFlowDocumentConverter.ApplyStyle(imageControl, GetImageControlStyle(styleProvider));

            // Add the image control to the list of pending image request states
            if (_pendingImageRequestStates == null)
            {
                _pendingImageRequestStates = new List<PrintableStoryImageControl>();
            }
            _pendingImageRequestStates.Add(imageControl);

            HeightAligner heightAligner = CreateHeightAligner(imageControl, WhitespaceDistribution.Split, styleProvider);
            Figure imageFigure = new Figure(new BlockUIContainer(heightAligner));
            // Image figure may not be styled since we need to set its content and anchoring
            if (imageData.Width < 600)
            {
                imageFigure.Width = new FigureLength(1, FigureUnitType.Column);
                imageFigure.VerticalAnchor = FigureVerticalAnchor.ContentBottom;
                imageFigure.HorizontalAnchor = FigureHorizontalAnchor.ColumnCenter;
            }
            else
            {
                imageFigure.Width = new FigureLength(2, FigureUnitType.Column);
                imageFigure.VerticalAnchor = FigureVerticalAnchor.ContentBottom;
                imageFigure.HorizontalAnchor = FigureHorizontalAnchor.ContentLeft;
            }

            Paragraph imageParagraph = new Paragraph(imageFigure);
            MsdnStoryToFlowDocumentConverter.ApplyStyle(imageParagraph, GetImageContainerStyle(styleProvider));
            return imageParagraph;
        }

        /// <summary>
        /// Selects images that can be displayed in the printed story. Thumbnail images are not displayed to avoid overscaling
        /// </summary>
        protected virtual List<ImageReference> GetStoryImages(Story story, out List<ImageData> storyImageData)
        {
            List<ImageReference> storyImages = new List<ImageReference>();
            storyImageData = new List<ImageData>();
            if (story != null)
            {
                if (story.ImageReferenceCollection != null)
                {
                    foreach (ImageReference imageReference in story.ImageReferenceCollection)
                    {
                        ImageData imageData = imageReference.WidestImageData;
                        if (imageData == ImageData.Empty)
                        {
                            imageData = imageReference.WidestLandscapeImageData;
                            if (imageData == ImageData.Empty)
                            {
                                imageData = imageReference.LargestImageData;
                            }
                        }

                        if (imageData.Format == ImageDataFormat.Thumbnail)
                        {
                            imageData = ImageData.Empty;
                        }

                        if (imageData != null && imageData != ImageData.Empty)
                        {
                            storyImages.Add(imageReference);
                            storyImageData.Add(imageData);
                        }
                    }
                }
            }
            return storyImages;
        }

        /// <summary>
        /// Calculates image spacing uniformly between paragraphs
        /// </summary>
        protected virtual int CalculateImageSpacing(XPathNodeIterator bodyParagraphIterator, List<ImageReference> storyImages)
        {
            int spacing = 0;
            if (bodyParagraphIterator != null && storyImages != null && bodyParagraphIterator.Count > 0 &&
                storyImages.Count > 0)
            {
                spacing = Math.Max(1, (int)(bodyParagraphIterator.Count / storyImages.Count));
            }
            return spacing;
        }

        #endregion

        #region Async Handlers

        /// <summary>
        /// Gets image sources
        /// </summary>
        protected virtual void OnGetImageSources(object userState)
        {
            _requestedUserState = userState;
            if (_pendingImageRequestStates != null && _pendingImageRequestStates.Count > 0)
            {
                foreach (PrintableStoryImageControl imageControl in _pendingImageRequestStates)
                {
                    // Start image download request. Target document is used as user state
                    imageControl.GetImageSourceCompleted += GetImageSourceCompleted;
                    imageControl.GetImageSourceAsync(_targetDocument);
                }
            }
            else
            {
                // Stored _userState should be the target FlowDocument
                ConversionCompletedEventArgs args = new ConversionCompletedEventArgs(userState, _targetDocument);
                _dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(RaiseConversionCompleted), args);
                _requestedUserState = null;
            }
        }

        /// <summary>
        /// When a request for image source completes, remove the requesting ImageControl from the list of pending request states.
        /// If all pending requests have been completed, raise conversion completed event
        /// </summary>
        protected virtual void OnGetImageSourceCompleted(object sender, GetImageSourceCompletedEventArgs e)
        {
            if (sender is PrintableStoryImageControl)
            {
                PrintableStoryImageControl imageControl = (PrintableStoryImageControl)sender;
                if (_pendingImageRequestStates.Contains(imageControl))
                {
                    _pendingImageRequestStates.Remove(imageControl);
                }
                imageControl.GetImageSourceCompleted -= GetImageSourceCompleted;
            }

            if (_pendingImageRequestStates.Count == 0)
            {
                // _userState is the target document
                ConversionCompletedEventArgs args = new ConversionCompletedEventArgs(_requestedUserState, _targetDocument);
                _dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(RaiseConversionCompleted), args);
                _requestedUserState = null;
            }
        }

        /// <summary>
        /// Raise conversion completed event
        /// </summary>
        protected virtual void OnConversionCompleted(ConversionCompletedEventArgs e)
        {
            if (ConversionCompleted != null)
            {
                ConversionCompleted(this, e);
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// True if image downloads are currently outstanding
        /// </summary>
        protected bool IsImageDownloadPending
        {
            get { return _isImageDowloadPending; }
            set { _isImageDowloadPending = value; }
        }

        #endregion

        #region Protected Style Getters

        /// <summary>
        /// Gets style for PrintableStoryImageControl that will display images in the printed document
        /// </summary>
        protected virtual Style GetImageControlStyle(FlowDocumentStyleProvider styleProvider)
        {
            if (styleProvider != null)
            {
                return styleProvider.ArticleImagePrintStyle;
            }
            return null;
        }

        /// <summary>
        /// Gets style for paragraph containing image controls
        /// </summary>
        protected virtual Style GetImageContainerStyle(FlowDocumentStyleProvider styleProvider)
        {
            if (styleProvider != null)
            {
                return styleProvider.ArticleFigureContainerParagraphStyle;
            }
            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Dispatcher callback to update image sources
        /// </summary>
        private object GetImageSources(object arg)
        {
            IsImageDownloadPending = false;
            OnGetImageSources(arg);
            return null;
        }

        /// <summary>
        /// Handler for PrintableStoryImageControl's GetImageSourceCompleted event. 
        /// </summary>
        private void GetImageSourceCompleted(object sender, GetImageSourceCompletedEventArgs e)
        {
            if (e.UserState == _targetDocument)
            {
                OnGetImageSourceCompleted(sender, e);
            }
        }

        /// <summary>
        /// Raises ConversionCompleted event
        /// </summary>
        private object RaiseConversionCompleted(object arg)
        {
            ConversionCompletedEventArgs args = arg as ConversionCompletedEventArgs;
            OnConversionCompleted(args);
            return null;
        }

        #endregion
        #region Private Fields

        /// <summary>
        /// User state passed by callers of async requests
        /// </summary>
        private object _requestedUserState;

        /// <summary>
        /// Target document produced by the converter
        /// </summary>
        private PrintableStoryFlowDocument _targetDocument;

        /// <summary>
        /// List of pending image request states
        /// </summary>
        private IList<PrintableStoryImageControl> _pendingImageRequestStates = new List<PrintableStoryImageControl>();

        /// <summary>
        /// True if outstanding image requests exist
        /// </summary>
        private bool _isImageDowloadPending;

        private Dispatcher _dispatcher;

        #endregion
    }
    
}