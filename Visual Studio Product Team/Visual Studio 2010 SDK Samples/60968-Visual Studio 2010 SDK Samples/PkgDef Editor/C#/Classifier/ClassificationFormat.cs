//***************************************************************************
// Copyright © 2010 Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//***************************************************************************
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace PkgDefLanguage
{
    #region Format definition
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "pkgdef.token")]
    [Name("pkgdefToken")]
    [UserVisible(true)]
    [Order(Before = Priority.High, After = Priority.Default)]
    internal sealed class PkgDefTokenFormat : ClassificationFormatDefinition
    {
        public PkgDefTokenFormat()
        {
            this.DisplayName = "PkgDef Token";
            this.ForegroundColor = Colors.Blue;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "pkgdef.guid")]
    [Name("pkgdefGuid")]
    [UserVisible(true)]
    [Order(After = Priority.Default, Before = "pkgdefToken")]
    internal sealed class PkgDefGuidFormat : ClassificationFormatDefinition
    {
        public PkgDefGuidFormat()
        {
            this.DisplayName = "PkgDef Guid";
            this.ForegroundColor = Colors.Purple;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "pkgdef.key")]
    [Name("pkgdefValueKey")]
    [UserVisible(true)]
    [Order(After = Priority.Default, Before = "pkgdefGuid")]
    internal sealed class PkgDefKeyFormat : ClassificationFormatDefinition
    {
        public PkgDefKeyFormat()
        {
            this.DisplayName = "PkgDef Key";
            this.ForegroundColor = Colors.IndianRed;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "pkgdef.value.name")]
    [Name("pkgdefValueName")]
    [UserVisible(true)]
    [Order(After = Priority.Default, Before = "pkgdefGuid")]
    internal sealed class PkgDefNameStringFormat : ClassificationFormatDefinition
    {
        public PkgDefNameStringFormat()
        {
            this.DisplayName = "PkgDef Value Name";
            this.ForegroundColor = Colors.IndianRed;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "pkgdef.value.string")]
    [Name("pkgdefValueString")]
    [UserVisible(true)]
    [Order(After = Priority.Default, Before = "pkgdefGuid")]
    internal sealed class PkgDefValueStringFormat : ClassificationFormatDefinition
    {
        public PkgDefValueStringFormat()
        {
            this.DisplayName = "PkgDef Value String";
            this.ForegroundColor = Colors.Red;
        }
    }

    #endregion //Format definition
}
