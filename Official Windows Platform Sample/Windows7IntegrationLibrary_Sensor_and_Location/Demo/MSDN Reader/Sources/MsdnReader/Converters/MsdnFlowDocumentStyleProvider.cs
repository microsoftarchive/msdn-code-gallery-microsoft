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
using Microsoft.SceReader.Data;
using System.Windows;
using System.Windows.Documents;

namespace MsdnReader
{
    [StyleTypedProperty(Property = "ArticleSectionTitleParagraphStyle", StyleTargetType = typeof(Paragraph))]
    [StyleTypedProperty(Property = "CopyrightParagraphStyle", StyleTargetType = typeof(Paragraph))]
    public class MsdnFlowDocumentStyleProvider : FlowDocumentStyleProvider
    {
        /// <summary>
        /// DependencyProperty for <see cref="ArticleSectionTitleParagraphStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty ArticleSectionTitleParagraphStyleProperty =
                DependencyProperty.Register(
                        "ArticleSectionTitleParagraphStyle",
                        typeof(Style),
                        typeof(MsdnFlowDocumentStyleProvider),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Style for article section title paragraph. Target type should be Paragraph.
        /// </summary>
        public Style ArticleSectionTitleParagraphStyle
        {
            get { return (Style)GetValue(ArticleSectionTitleParagraphStyleProperty); }
            set { SetValue(ArticleSectionTitleParagraphStyleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="ArticleCodeStyleProperty"/> property.
        /// </summary>
        public static readonly DependencyProperty ArticleCodeStyleProperty =
                DependencyProperty.Register(
                        "ArticleCodeStyle",
                        typeof(Style),
                        typeof(MsdnFlowDocumentStyleProvider),
                        new FrameworkPropertyMetadata(null));


        public Style ArticleCodeStyle
        {
            get { return (Style)GetValue(ArticleCodeStyleProperty); }
            set { SetValue(ArticleCodeStyleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="AuthorFigureContainerParagraphStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty AuthorParagraphStyleProperty =
                DependencyProperty.Register(
                        "AuthorParagraphStyle",
                        typeof(Style),
                        typeof(MsdnFlowDocumentStyleProvider),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Style for author figure paragraph. Target type should be Paragraph.
        /// </summary>
        public Style AuthorParagraphStyle
        {
            get { return (Style)GetValue(AuthorParagraphStyleProperty); }
            set { SetValue(AuthorParagraphStyleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="AuthorIssueFigureStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty AuthorIssueFigureStyleProperty =
                DependencyProperty.Register(
                        "AuthorIssueFigureStyle",
                        typeof(Style),
                        typeof(MsdnFlowDocumentStyleProvider),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Style for author & issue figure. Target type should be Figure
        /// </summary>
        public Style AuthorIssueFigureStyle
        {
            get { return (Style)GetValue(AuthorIssueFigureStyleProperty); }
            set { SetValue(AuthorIssueFigureStyleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="IssueParagraphStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty IssueParagraphStyleProperty =
                DependencyProperty.Register(
                        "IssueParagraphStyle",
                        typeof(Style),
                        typeof(MsdnFlowDocumentStyleProvider),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Style for issue figure paragraph. Target type should be Paragraph.
        /// </summary>
        public Style IssueParagraphStyle
        {
            get { return (Style)GetValue(IssueParagraphStyleProperty); }
            set { SetValue(IssueParagraphStyleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="LinkToSampleCodeParagraphStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LinkToSampleCodeParagraphStyleProperty =
                DependencyProperty.Register(
                        "LinkToSampleCodeParagraphStyle",
                        typeof(Style),
                        typeof(MsdnFlowDocumentStyleProvider),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Style for LinkToSampleCode figure paragraph. Target type should be Paragraph.
        /// </summary>
        public Style LinkToSampleCodeParagraphStyle
        {
            get { return (Style)GetValue(LinkToSampleCodeParagraphStyleProperty); }
            set { SetValue(LinkToSampleCodeParagraphStyleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="LinkToSampleCodeFigureStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty LinkToSampleCodeFigureStyleProperty =
                DependencyProperty.Register(
                        "LinkToSampleCodeFigureStyle",
                        typeof(Style),
                        typeof(MsdnFlowDocumentStyleProvider),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Style for LinkToSampleCode. Target type should be Figure
        /// </summary>
        public Style LinkToSampleCodeFigureStyle
        {
            get { return (Style)GetValue(LinkToSampleCodeFigureStyleProperty); }
            set { SetValue(LinkToSampleCodeFigureStyleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="BioParagraphStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty BioParagraphStyleProperty =
                DependencyProperty.Register(
                        "BioParagraphStyle",
                        typeof(Style),
                        typeof(MsdnFlowDocumentStyleProvider),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Style for Bio figure paragraph. Target type should be Paragraph.
        /// </summary>
        public Style BioParagraphStyle
        {
            get { return (Style)GetValue(BioParagraphStyleProperty); }
            set { SetValue(BioParagraphStyleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="BioFigureStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty BioFigureStyleProperty =
                DependencyProperty.Register(
                        "BioFigureStyle",
                        typeof(Style),
                        typeof(MsdnFlowDocumentStyleProvider),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Style for Bio. Target type should be Figure
        /// </summary>
        public Style BioFigureStyle
        {
            get { return (Style)GetValue(BioFigureStyleProperty); }
            set { SetValue(BioFigureStyleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="CopyrightParagraphStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty CopyrightParagraphStyleProperty =
                DependencyProperty.Register(
                        "CopyrightParagraphStyle",
                        typeof(Style),
                        typeof(MsdnFlowDocumentStyleProvider),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Style for copyright notice para
        /// </summary>
        public Style CopyrightParagraphStyle
        {
            get { return (Style)GetValue(CopyrightParagraphStyleProperty); }
            set { SetValue(CopyrightParagraphStyleProperty, value); }
        }
    }
}