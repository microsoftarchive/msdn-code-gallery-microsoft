//***************************************************************************
// Copyright © 2010 Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//***************************************************************************
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace PkgDefLanguage
{
    /// <summary>
    /// Define the content types and classification types
    /// </summary>
    internal static class PkgDefClassificationDefinition
    {
        #region Content Type and File Extension Definitions

        [Export]
        [Name("pkgdef")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition pkgdefContentTypeDefinition = null;

        [Export]
        [FileExtension(".pkgdef")]
        [ContentType("pkgdef")]
        internal static FileExtensionToContentTypeDefinition pkgdefFileExtensionDefinition = null;

        [Export]
        [FileExtension(".pkgundef")]
        [ContentType("pkgdef")]
        internal static FileExtensionToContentTypeDefinition pkgundefFileExtensionDefinition = null;

        #endregion

        #region Type definition

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("pkgdef.key")]
        internal static ClassificationTypeDefinition keyClassificationDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("pkgdef.value")]
        internal static ClassificationTypeDefinition valueClassificationDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("pkgdef.value.name")]
        [BaseDefinition("pkgdef.value")]
        internal static ClassificationTypeDefinition valueNameClassificationDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("pkgdef.value.name.default")]
        [BaseDefinition("pkgdef.value")]
        internal static ClassificationTypeDefinition valueNameDefaultClassificationDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("pkgdef.value.string")]
        [BaseDefinition("pkgdef.value")]
        internal static ClassificationTypeDefinition valueStringClassificationDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("pkgdef.value.number")]
        [BaseDefinition("pkgdef.value")]
        internal static ClassificationTypeDefinition valueNumberClassificationDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("pkgdef.token")]
        internal static ClassificationTypeDefinition tokenClassificationDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("pkgdef.guid")]
        internal static ClassificationTypeDefinition guidClassificationDefinition = null;

        #endregion
    }
}
