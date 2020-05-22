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
using System.Windows.Documents;
using Microsoft.SceReader.Data;

namespace MsdnReader
{
    /// <summary>
    /// Special hyperlink used to represent links to images in msdn stories. This hyperlink may cause navigation
    /// to a code figure hosted on the web or to an image reference. The navigation behavior is different in each case, and hyperlink 
    /// has special properties to reflect this
    /// </summary>
    public class MsdnStoryImageHyperlink : Hyperlink
    {
        public MsdnStoryImageHyperlink(ImageReference imageReference, Story story)
            : base()
        {
            if (imageReference == null)
            {
                throw new ArgumentNullException("imageReference");
            }

            if (story == null)
            {
                throw new ArgumentNullException("story");
            }

            if (story.ImageReferenceCollection == null || !story.ImageReferenceCollection.Contains(imageReference))
            {
                throw new InvalidOperationException(MsdnReader.Resources.Strings.MsdnStoryImageHyperlinkInvalid);
            }

            _story = story;
            _imageReference = imageReference;
            _isImageReferenceLink = true;

            // Fake a navigate Uri, it cannot be null otherwise hyperlink will not navigate
            NavigateUri = new Uri(_story.WebLink);
        }

        public MsdnStoryImageHyperlink(ImageReference imageReference, Story story, Inline childInline, TextPointer insertionPosition)
            : base(childInline, insertionPosition)
        {
            if (imageReference == null)
            {
                throw new ArgumentNullException("imageReference");
            }

            if (story == null)
            {
                throw new ArgumentNullException("story");
            }

            if (story.ImageReferenceCollection == null || !story.ImageReferenceCollection.Contains(imageReference))
            {
                throw new InvalidOperationException(MsdnReader.Resources.Strings.MsdnStoryImageHyperlinkInvalid);
            }

            _story = story;
            _imageReference = imageReference;
            _isImageReferenceLink = true;

            // Fake a navigate Uri, it cannot be null otherwise hyperlink will not navigate
            NavigateUri = new Uri(_story.WebLink);
        }

        public MsdnStoryImageHyperlink(Uri navigateUri, Story story, Inline childInline, TextPointer insertionPosition) 
            : base(childInline, insertionPosition)
        {
            if (navigateUri == null)
            {
                throw new ArgumentException("navigateUri");
            }

            if (story == null)
            {
                throw new ArgumentNullException("story");
            }

            NavigateUri = navigateUri;
            _story = story;
            _imageReference = null;
            _isImageReferenceLink = false;
        }

        public MsdnStoryImageHyperlink(Uri navigateUri, Story story)
            : base()
        {
            if (navigateUri == null)
            {
                throw new ArgumentException("navigateUri");
            }

            if (story == null)
            {
                throw new ArgumentNullException("story");
            }

            NavigateUri = navigateUri;
            _story = story;
            _imageReference = null;
            _isImageReferenceLink = false;
        }

        public ImageReference ImageReference
        {
            get { return _imageReference; }
        }

        public Story Story
        {
            get { return _story; }
        }

        public bool IsImageReferenceLink
        {
            get { return _isImageReferenceLink; }
        }

        private Story _story;
        private ImageReference _imageReference;
        private bool _isImageReferenceLink;
    }
}