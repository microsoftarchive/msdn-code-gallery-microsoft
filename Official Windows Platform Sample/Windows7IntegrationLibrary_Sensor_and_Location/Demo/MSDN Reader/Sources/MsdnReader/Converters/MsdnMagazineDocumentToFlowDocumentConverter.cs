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
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Xml.XPath;
using Microsoft.SceReader.Converters;
using Microsoft.SceReader.Data;
using Microsoft.SceReader;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace MsdnReader
{
    class MsdnMagazineDocumentToFlowDocumentConverter : StoryDocumentConverter
    {
        #region Public Methods

        public virtual FlowDocument Convert(XPathDocument document, Story story)
        {
            XPathNavigator navigator = document.CreateNavigator();
            XPathNavigator msdn = GetMsdnNavigator(navigator);
            Story = story;

            // Before conversion, pre-populate the list of web and ImageReference figures
            GetWebFigures(msdn, story);
            GetImageReferenceFigures(msdn, story);

            MsdnFlowDocumentStyleProvider styleProvider = ServiceProvider.ConverterManager.FlowDocumentStyleProvider as MsdnFlowDocumentStyleProvider;
            return GetFlowDocumentFromNavigator(msdn, story, styleProvider);
        }

        public static Uri GetMagazineArticleWebFigureUri(Story story)
        {
            Uri uri = null;
            if (story != null)
            {
                Uri.TryCreate(String.Concat(story.WebLink, _webFigureUriSuffix), UriKind.Absolute, out uri);
            }
            return uri;
        }

        protected virtual XPathNavigator GetMsdnNavigator(XPathNavigator documentNavigator)
        {
            return XmlHelper.GetChildNavigator(documentNavigator, "content");
        }

        /// <summary>
        /// Creates headline and byline paragraphs
        /// </summary>
        protected override void CreateHeadline(FlowDocument flowDocument, XPathNavigator nitfNavigator, Story story, FlowDocumentStyleProvider styleProvider)
        {
            XPathNavigator headerNavigator = GetHeaderNavigator(nitfNavigator);
            MsdnFlowDocumentStyleProvider msdnStyleProvider = styleProvider as MsdnFlowDocumentStyleProvider;

            CreateHeadlineParagraph(flowDocument, headerNavigator, story, msdnStyleProvider);
            CreateAuthorIssueParagraph(flowDocument, headerNavigator, nitfNavigator, story, msdnStyleProvider);
            CreateLinkToSampleCodeParagraph(flowDocument, headerNavigator, story, msdnStyleProvider);
            //CreateBylineDateParagraph(flowDocument, headerNavigator, story, styleProvider);
        }

        /// <summary>
        /// Creates document body
        /// </summary>
        protected override void CreateBody(FlowDocument flowDocument, XPathNavigator navigator, Story story, FlowDocumentStyleProvider styleProvider)
        {
            XPathNavigator bodyNavigator = GetBodyNavigator(navigator);
            XPathNodeIterator bodyParagraphIterator = GetBodyParagraphIterator(bodyNavigator);
            if (bodyParagraphIterator != null)
            {
                // Iterate through sections
                while (bodyParagraphIterator.MoveNext())
                {
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
            AddWebFigureLinkToBody(flowDocument, navigator, story, styleProvider);
            AddAuthorAndBioToBody(flowDocument, navigator, story, styleProvider);
            AddCopyrightNoticeToBody(flowDocument, navigator, story, styleProvider);
        }

        protected void AddWebFigureLinkToBody(FlowDocument flowDocument, XPathNavigator navigator, Story story, FlowDocumentStyleProvider styleProvider)
        {
            MsdnStoryImageHyperlink hyperlink = GetLinkToWebFigures(story, navigator, styleProvider);

            if (hyperlink != null)
            {
                Paragraph webFigureDownloadPara = new Paragraph();
                ApplyStyle(webFigureDownloadPara, GetBodyTextParagraphStyle(styleProvider, 0));
                webFigureDownloadPara.Inlines.Add(hyperlink);
                flowDocument.Blocks.Add(webFigureDownloadPara);
            }
        }

        protected MsdnStoryImageHyperlink GetLinkToWebFigures(Story story, XPathNavigator navigator, FlowDocumentStyleProvider styleProvider)
        {
            MsdnStoryImageHyperlink hyperlink = null;
            XPathNavigator figuresNavigator = navigator.SelectSingleNode("Figures");
            if (figuresNavigator != null)
            {
                // Figures exist, add link
                hyperlink = new MsdnStoryImageHyperlink(GetMagazineArticleWebFigureUri(story), story);
                hyperlink.Inlines.Add(new Run("See all code figures for this article"));
                hyperlink.ToolTip = "View Code Figures In Browser";
                hyperlink.RequestNavigate += ((MsdnViewManager)ServiceProvider.ViewManager).OnImageHyperlinkRequestNavigate;
            }
            return hyperlink;
        }

        protected virtual void AddAuthorAndBioToBody(FlowDocument flowDocument, XPathNavigator navigator, Story story, FlowDocumentStyleProvider styleProvider)
        {
            XPathNavigator headerNavigator = GetHeaderNavigator(navigator);
            MsdnFlowDocumentStyleProvider msdnStyleProvider = styleProvider as MsdnFlowDocumentStyleProvider;
            CreateBioTextParagraph(flowDocument, headerNavigator, story, msdnStyleProvider);
        }

        protected virtual void AddCopyrightNoticeToBody(FlowDocument flowDocument, XPathNavigator navigator, Story story, FlowDocumentStyleProvider styleProvider)
        {
            MsdnFlowDocumentStyleProvider msdnStyleProvider = styleProvider as MsdnFlowDocumentStyleProvider;
            CreateCopyrightNoticeParagraph(flowDocument, msdnStyleProvider);
        }

        protected virtual Paragraph CreateCodeParagraph(XPathNavigator codeNavigator, Story story, FlowDocumentStyleProvider styleProvider)
        {
            Paragraph paragraph = new Paragraph(new Run());
            // Navigate paragraph using a duplicate navigator. Step inside paragraph's first child before beginning navigation
            XPathNavigator parsingNavigator = codeNavigator.CreateNavigator();
            if (parsingNavigator.NodeType == XPathNodeType.Element && parsingNavigator.Name.ToLower(CultureInfo.InvariantCulture) == "code" && parsingNavigator.MoveToFirstChild())
            {
                ParseContent(parsingNavigator.CreateNavigator(), paragraph.ContentStart, styleProvider);
            }
            MsdnStoryToFlowDocumentConverter.ApplyStyle(paragraph, GetArticleCodeStyle(styleProvider));
            return paragraph;
        }

        /// <summary>
        /// Creates a body text paragraph from body navigator. Paragraph number is passed in in case different styles 
        /// are desired for the first paragraph, etc.
        /// </summary>
        protected override Paragraph CreateBodyTextParagraph(XPathNavigator paragraphNavigator, Story story, FlowDocumentStyleProvider styleProvider, int paragraphNumber)
        {
            Paragraph paragraph = new Paragraph(new Run());

            // Navigate paragraph using a duplicate navigator. Step inside paragraph's first child before beginning navigation
            XPathNavigator parsingNavigator = paragraphNavigator.CreateNavigator();
            if (parsingNavigator.NodeType == XPathNodeType.Element && parsingNavigator.Name.ToLower(CultureInfo.InvariantCulture) == "para" && parsingNavigator.MoveToFirstChild())
            {
                ParseContent(parsingNavigator.CreateNavigator(), paragraph.ContentStart, styleProvider);
            }
            ApplyStyle(paragraph, GetBodyTextParagraphStyle(styleProvider, paragraphNumber));
            return paragraph;
        }

        /// <summary>
        /// Creates title for sections
        /// </summary>
        protected virtual Paragraph CreateSectionTitleParagraph(XPathNavigator titleNavigator, Story story, FlowDocumentStyleProvider styleProvider)
        {
            Paragraph paragraph = new Paragraph(new Run());

            // Navigate paragraph using a duplicate navigator. Step inside paragraph's first child before beginning navigation
            XPathNavigator parsingNavigator = titleNavigator.CreateNavigator();
            if (parsingNavigator.NodeType == XPathNodeType.Element && parsingNavigator.Name.ToLower(CultureInfo.InvariantCulture) == "title" && parsingNavigator.MoveToFirstChild())
            {
                ParseContent(parsingNavigator.CreateNavigator(), paragraph.ContentStart, styleProvider);
            }
            MsdnStoryToFlowDocumentConverter.ApplyStyle(paragraph, GetSectionTitleParagraphStyle(styleProvider));
            return paragraph;
        }

        protected virtual void CreateAuthorIssueParagraph(FlowDocument flowDocument, XPathNavigator headlineNavigator, XPathNavigator nitfNavigator, Story story, MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            Paragraph authorIssuePara = new Paragraph();
            ApplyStyle(authorIssuePara, GetAuthorParagraphStyle(msdnStyleProvider));

            Figure authorIssueFigure = new Figure();
            ApplyStyle(authorIssueFigure, GetAuthorIssueFigureStyle(msdnStyleProvider));
            authorIssuePara.Inlines.Add(authorIssueFigure);

            String authorText = GetAuthorText(headlineNavigator);
            Paragraph authorTextPara = new Paragraph(new Run(authorText));
            authorIssueFigure.Blocks.Add(authorTextPara);

            String issueText = GetIssueText(nitfNavigator);
            Paragraph issueTextPara = new Paragraph(new Run(issueText));
            ApplyStyle(issueTextPara, GetIssueParagraphStyle(msdnStyleProvider));
            authorIssueFigure.Blocks.Add(issueTextPara);

            flowDocument.Blocks.Add(authorIssuePara);    
        }

        /// <summary>
        /// Override for CreateStyledInline checks for Fig tags to create special hyperlinks for images
        /// </summary>
        /// <param name="navigator"></param>
        /// <param name="textPointer"></param>
        /// <param name="styleProvider"></param>
        /// <returns></returns>
        protected override Inline CreateStyledInline(XPathNavigator navigator, TextPointer textPointer, FlowDocumentStyleProvider styleProvider)
        {
            Inline inline = null;
            if (textPointer != null)
            {
                if (navigator.NodeType == XPathNodeType.Element)
                {
                    switch (navigator.Name.ToLowerInvariant())
                    {
                        case "fig":
                            inline = CreateImageHyperlink(navigator, textPointer, styleProvider);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (inline == null)
            {
                inline = base.CreateStyledInline(navigator, textPointer, styleProvider);
            }

            return inline;
        }

        /// <summary>
        /// Creates a an MsdnStoryImageHyperlink element with a link to the image Uri/ImageReference
        /// </summary>
        protected MsdnStoryImageHyperlink CreateImageHyperlink(XPathNavigator navigator, TextPointer textPointer, FlowDocumentStyleProvider styleProvider)
        {
            MsdnStoryImageHyperlink hyperlink = null;
            if (navigator != null)
            {
                // If it's a figure containing code/ javascript, it will be referenced using the "ref" attribute
                Run run = new Run(XmlHelper.GetNavigatorText(navigator));
                string webFigureRef = navigator.GetAttribute("ref", String.Empty);
                if (!String.IsNullOrEmpty(webFigureRef) && _webFigureStore.ContainsKey(webFigureRef))
                {
                    // 'ref' attribute found, try to match it with a web figure Uri
                    Uri uri = _webFigureStore[webFigureRef];
                    hyperlink = new MsdnStoryImageHyperlink(uri, Story, run, textPointer);
                    hyperlink.ToolTip = "View Code In Browser";
                }
                else
                {
                    // No attribute matched. Try to match figure text to an image caption from an Image element in the doc
                    string text = XmlHelper.GetNavigatorText(navigator);
                    if (!String.IsNullOrEmpty(text) && _imageReferenceFigureStore.ContainsKey(text))
                    {
                        ImageReference imageReference = _imageReferenceFigureStore[text];
                        hyperlink = new MsdnStoryImageHyperlink(imageReference, Story, run, textPointer);
                        hyperlink.ToolTip = "View Image";
                    }
                }

                if (hyperlink != null)
                {
                    ApplyStyle(hyperlink, GetHyperlinkStyle(styleProvider));
                    hyperlink.RequestNavigate += ((MsdnViewManager)ServiceProvider.ViewManager).OnImageHyperlinkRequestNavigate;
                }
            }

            return hyperlink;        
        }

        protected virtual void CreateLinkToSampleCodeParagraph(FlowDocument flowDocument, XPathNavigator headlineNavigator, Story story, MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            Hyperlink download = GetDownloadHyperlink(headlineNavigator);

            if (download != null)
            {
                Paragraph samplePara = new Paragraph();
                ApplyStyle(samplePara, GetLinkToSampleCodeParagraphStyle(msdnStyleProvider));

                Figure sampleFigure = new Figure();
                ApplyStyle(sampleFigure, GetLinkToSampleCodeFigureStyle(msdnStyleProvider));
                samplePara.Inlines.Add(sampleFigure);

                Paragraph sampleTextPara = new Paragraph();
                sampleTextPara.Inlines.Add(download);
                sampleFigure.Blocks.Add(sampleTextPara);

                flowDocument.Blocks.Add(samplePara);
            }
        }

        protected virtual void CreateAuthorTextParagraph(FlowDocument flowDocument, XPathNavigator headlineNavigator, Story story, MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            Paragraph authorPara = new Paragraph();
            ApplyStyle(authorPara, GetAuthorParagraphStyle(msdnStyleProvider));

            String authorText = GetAuthorText(headlineNavigator);
            authorPara.Inlines.Add(new Run(authorText));

            flowDocument.Blocks.Add(authorPara);
        }

        protected virtual void CreateBioTextParagraph(FlowDocument flowDocument, XPathNavigator headlineNavigator, Story story, MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            Paragraph bioPara = new Paragraph();
            ApplyStyle(bioPara, GetBioParagraphStyle(msdnStyleProvider));

            String bioText = GetAuthorText(headlineNavigator) + GetBioText(headlineNavigator);
            bioPara.Inlines.Add(new Run(bioText));

            flowDocument.Blocks.Add(bioPara);
        }

        protected virtual void CreateCopyrightNoticeParagraph(FlowDocument flowDocument, MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            Paragraph copyrightPara = new Paragraph();
            ApplyStyle(copyrightPara, GetCopyrightParagraphStyle(msdnStyleProvider));
            copyrightPara.Inlines.Add(new Run(Resources.Strings.MagazineConverterCopyright));
            flowDocument.Blocks.Add(copyrightPara);
        }

        #endregion

        #region Navigator getters

        /// <summary>
        /// Gets header navigator from MSDN navigator
        /// </summary>
        protected override XPathNavigator GetHeaderNavigator(XPathNavigator nitfNavigator)
        {
            return XmlHelper.GetChildNavigator(nitfNavigator, "body/Mast");
        }

        /// <summary>
        /// Gets the main body navigator from the MSDN document navigator
        /// </summary>
        protected override XPathNavigator GetBodyNavigator(XPathNavigator nitfNavigator)
        {
            return XmlHelper.GetChildNavigator(nitfNavigator, "body/Text");
        }

        /// <summary>
        /// Gets paragraph iterator from body navigator - for MSDN articles, body paragraphs correspond to the Section elements
        /// which contain a title and a number of inner text paragraphs
        /// </summary>
        protected override XPathNodeIterator GetBodyParagraphIterator(XPathNavigator bodyNavigator)
        {
            XPathNodeIterator paragraphIterator = null;
            if (bodyNavigator != null)
            {
                paragraphIterator = bodyNavigator.SelectChildren("Section", String.Empty);
            }
            return paragraphIterator;
        }

        /// <summary>
        /// Gets the navigator with the section title
        /// </summary>
        protected virtual XPathNavigator GetSectionTitleNavigator(XPathNavigator bodyParagraphNavigator)
        {
            return XmlHelper.GetChildNavigator(bodyParagraphNavigator, "Title");
        }

        /// <summary>
        /// Gets inner body paragraphs - Paragraph and Title nodes - from main body paragraphs, or sections
        /// </summary>
        protected virtual XPathNodeIterator GetInnerBodyParagraphIterator(XPathNavigator bodyParagraphNavigator)
        {
            XPathNodeIterator innerParagraphIterator = null;
            if (bodyParagraphNavigator != null)
            {
                innerParagraphIterator = bodyParagraphNavigator.Select("Para");
            }
            return innerParagraphIterator;
        }

        #endregion

        #region Text Getters

        protected override string GetHeadlineText(XPathNavigator headerNavigator)
        {
            string headlineText = XmlHelper.GetChildValueFromNavigator(headerNavigator, "Title", typeof(String)) as string;
            return headlineText;
        }

        protected string GetAuthorText(XPathNavigator headerNavigator)
        {
            string headlineText = XmlHelper.GetChildValueFromNavigator(headerNavigator, "Author/Name", typeof(String)) as string;
            return headlineText;
        }

        protected string GetIssueText(XPathNavigator headerNavigator)
        {
            string headlineText = headerNavigator.GetAttribute("issue", String.Empty);
            return headlineText;
        }


        protected Hyperlink GetDownloadHyperlink(XPathNavigator headerNavigator)
        {
            Hyperlink download = null; 
            XPathNavigator downloadNavigator = XmlHelper.GetChildNavigator(headerNavigator, "Download");
            if (downloadNavigator != null)
            {
                download = new Hyperlink(new Run("Get the sample code for this article"));
                string downloadRef = downloadNavigator.GetAttribute("ref", String.Empty);
                Uri navigateUri = null;
                if (Uri.TryCreate(downloadRef, UriKind.Absolute, out navigateUri))
                {
                    download.NavigateUri = navigateUri;
                }
            }
            return download;
        }

        protected string GetBioText(XPathNavigator headerNavigator)
        {
            string headlineText = XmlHelper.GetChildValueFromNavigator(headerNavigator, "Author/Bio", typeof(String)) as string;
            return headlineText;
        }

        #endregion

        #region Style Getters

        protected virtual Style GetSectionTitleParagraphStyle(FlowDocumentStyleProvider styleProvider)
        {
            if (styleProvider != null && styleProvider is MsdnFlowDocumentStyleProvider)
            {
                return ((MsdnFlowDocumentStyleProvider)styleProvider).ArticleSectionTitleParagraphStyle;
            }
            return null;
        }

        protected virtual Style GetArticleCodeStyle(FlowDocumentStyleProvider styleProvider)
        {
            if (styleProvider != null && styleProvider is MsdnFlowDocumentStyleProvider)
            {
                return ((MsdnFlowDocumentStyleProvider)styleProvider).ArticleCodeStyle;
            }
            return null;
        }

        protected virtual Style GetAuthorParagraphStyle(MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            if (msdnStyleProvider != null)
            {
                return msdnStyleProvider.AuthorParagraphStyle;
            }
            return null;
        }

        protected virtual Style GetAuthorIssueFigureStyle(MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            if (msdnStyleProvider != null)
            {
                return msdnStyleProvider.AuthorIssueFigureStyle;
            }
            return null;
        }

        protected virtual Style GetIssueParagraphStyle(MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            if (msdnStyleProvider != null)
            {
                return msdnStyleProvider.IssueParagraphStyle;
            }
            return null;
        }

        protected virtual Style GetLinkToSampleCodeParagraphStyle(MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            if (msdnStyleProvider != null)
            {
                return msdnStyleProvider.LinkToSampleCodeParagraphStyle;
            }
            return null;
        }

        protected virtual Style GetLinkToSampleCodeFigureStyle(MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            if (msdnStyleProvider != null)
            {
                return msdnStyleProvider.LinkToSampleCodeFigureStyle;
            }
            return null;
        }

        protected virtual Style GetBioParagraphStyle(MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            if (msdnStyleProvider != null)
            {
                return msdnStyleProvider.BioParagraphStyle;
            }
            return null;
        }

        protected virtual Style GetCopyrightParagraphStyle(MsdnFlowDocumentStyleProvider msdnStyleProvider)
        {
            if (msdnStyleProvider != null)
            {
                return msdnStyleProvider.CopyrightParagraphStyle;
            }
            return null;
        }

        /// <summary>
        /// Populate list of figures that will appear in Story's ImageReference collection
        /// </summary>
        /// <param name="msdn"></param>
        /// <param name="story"></param>
        protected void GetImageReferenceFigures(XPathNavigator msdn, Story story)
        {
            // Image content is nested inside body navigator
            XPathNavigator bodyNavigator = GetBodyNavigator(msdn);
            if (bodyNavigator != null)
            {
                XPathNodeIterator imageIterator = bodyNavigator.SelectDescendants("Image", String.Empty, false);
                if (imageIterator != null)
                {
                    while (imageIterator.MoveNext())
                    {
                        XPathNavigator image = imageIterator.Current;
                        string src = image.GetAttribute("src", String.Empty);
                        string caption = image.GetAttribute("caption", String.Empty);
                        if (story.ImageReferenceCollection != null)
                        {
                            // Match src to image reference
                            foreach (ImageReference imageReference in story.ImageReferenceCollection)
                            {
                                if (imageReference.ImageDataCollection != null)
                                {
                                    foreach (ImageData imageData in imageReference.ImageDataCollection)
                                    {
                                        string source = Path.GetFileName(imageData.Source.ToString());
                                        if (source == src)
                                        {
                                            // Match found. In Msdn these types of image figures are typically
                                            // referenced by caption, so use caption as key. If something else is used as a reference, this won't work
                                            if (!_imageReferenceFigureStore.ContainsKey(caption))
                                            {
                                                _imageReferenceFigureStore.Add(caption, imageReference);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Populate list of web figures
        /// </summary>
        /// <param name="msdn"></param>
        protected void GetWebFigures(XPathNavigator msdn, Story story)
        {
            XPathNavigator figuresNavigator = msdn.SelectSingleNode("Figures");
            if (figuresNavigator != null)
            {
                XPathNodeIterator figuresIterator = figuresNavigator.SelectChildren("Figure", String.Empty);
                if (figuresIterator != null)
                {
                    while (figuresIterator.MoveNext())
                    {
                        XPathNavigator figure = figuresIterator.Current;
                        string figureRef = figure.GetAttribute("ref", String.Empty);
                        int figureRefNumber = -1;
                        if (int.TryParse(figureRef, out figureRefNumber))
                        {
                            // Create web Uri for this Figure
                            string suffix = String.Concat(_webFigureUriSuffix, "#fig", figureRefNumber.ToString(CultureInfo.InvariantCulture));
                            Uri figureUri = null;
                            if (Uri.TryCreate(String.Concat(story.WebLink, suffix), UriKind.Absolute, out figureUri))
                            {
                                // Valid figure Uri generated, add it to the store of figure Uris. It will
                                // be referenced by a reference string which can be used to index it
                                figureRef = String.Concat("fig", figureRefNumber.ToString(CultureInfo.InvariantCulture));
                                if (!_webFigureStore.ContainsKey(figureRef))
                                {
                                    _webFigureStore.Add(figureRef, figureUri);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Properties

        protected Story Story
        {
            get { return _story; }
            set { _story = value; }
        }

        #endregion

        #region Fields

        /// <summary>
        /// Links to figures on the web, indexed by figure references
        /// </summary>
        private Dictionary<string, Uri> _webFigureStore = new Dictionary<string, Uri>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Links to figures that are part of the Story's ImageReferenceCollection
        /// </summary>
        private Dictionary<string, ImageReference> _imageReferenceFigureStore = new Dictionary<string, ImageReference>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Suffix to concatenate with Story's web link and figure reference to generate figure link
        /// </summary>
        private const string _webFigureUriSuffix = "/default.aspx?loc=&fig=true";

        /// <summary>
        /// Store the story being converted, this converter needs to maintain state during the conversion process
        /// </summary>
        private Story _story;

        #endregion
    }
}