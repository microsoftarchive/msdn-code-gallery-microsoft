//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {

    var currentPage = 0;
    var searchClass = '.fullRegion';

    // This array points to HTML files that represent individual page templates. 
    // The templates are instantiated in the order specified by the array below - page[0] will be the 
    // first template instantiated, page[1] will be the second page, and so on. To add or remove templates 
    // from this order, simply add or remove entries in the array. If more pages need to be generated than 
    // there are entries in the array, the page at the last entry is used repeatedly.
    var pageTemplates = [
        '/html/page1.html',
        '/html/page2.html',
        '/html/page3.html',
        '/html/pageDefault.html'
    ];

    function ready(element, options) {

        // This block causes the layout function to fire when the content in the source iFrame is loaded
        var sourceIFrame = element.querySelector('#sourceIFrame');
        sourceIFrame.addEventListener('load', createPages);

        // This block adds keystroke listeners to the source iframe and scrolling element for keyboard navigation.
        // Note that due to the eventing model of CSS Regions, keystroke listeners are necessary on both the iframe (to catch
        // keydown events on Regions) and on the scrolling element (to catch keydown events on non-Region elements within a page).
        // The handleKeystrokes function is purely for demonstration purposes; for a more complete look at how to handle element 
        // scrolling with gradual animations, please see the Input: Pan/scroll and zoom Windows 8 SDK sample 
        // (http://go.microsoft.com/fwlink/?LinkId=254904)
        sourceIFrame.contentDocument.addEventListener('keydown', handleKeystrokes);
        var scrollingElement = element.querySelector('#pageContainer');
        scrollingElement.addEventListener('keydown', handleKeystrokes);
        scrollingElement.focus();
        
        // This block, along with the callback functions below, allow us to detect when the app has
        // been resized, which may necessitate the creation or removal of additional pages. We also use matchMedia
        // calls to track when the view state has changed, which in turn controls the set of elements we examine when
        // determining whether to add or subtract pages. Specifically, in snapped mode, we deal with a smaller set of 
        // regions that control the layout and resize functions. In all other view states, we deal with the full 
        // set of regions.
        window.addEventListener("resize", resizePages);
        window.matchMedia('(-ms-view-state: snapped)').addListener(setSnappedRegions);
        window.matchMedia('(-ms-view-state: filled)').addListener(setOtherRegions);
        window.matchMedia('(-ms-view-state: fullscreen-portrait)').addListener(setOtherRegions);
        window.matchMedia('(-ms-view-state: fullscreen-landscape)').addListener(setOtherRegions);

    }

    function setSnappedRegions(mql) {
        if (mql.matches) {
            searchClass = '.snappedRegion';
        }
    }

    function setOtherRegions(mql) {
        if (mql.matches) {
            searchClass = '.fullRegion';
        }
    }



    // createPages is a simple recursive function that first determines what page is being instantiated 
    // (first page, second, etc.) and clones the appropriate template from the pages array declared above. 
    // Once the template has been cloned, it is placed in the flexbox element which contains all instantiated 
    // pages. The function then determines if the newly instantiated page has fully displayed the content 
    // stream - if not (if there is overflow, and therefore more pages to create) then layout is called again, 
    // and the process begins all over. 
    function createPages() {

        var targetPage;

        if (currentPage > (pageTemplates.length - 1)) {
            targetPage = pageTemplates[pageTemplates.length - 1];
        } else {
            targetPage = pageTemplates[currentPage];
        }
              
        // This block clones the desired template, and places the newly cloned template in the flexbox 
        // element which contains all instantiated pages. The setImmediate function improves performnace
        // when creating a large number of pages.
        var flexboxElement = document.getElementById('pageContainer');
        WinJS.UI.Fragments.render(targetPage, flexboxElement).done(function () {
            setImmediate(function () {
                currentPage += 1;
                // Here we simply find the last instantiated page, then the last region on the page,
                // to find out if content is overflowing the page. If so, we call layout again, and the
                // process continues until all content has been laid out.
                var /*@override*/ flexboxElement = document.getElementById('pageContainer');
                var pages = flexboxElement.querySelectorAll('.page');
                var lastPage = pages[pages.length - 1];
                var regions = lastPage.querySelectorAll(searchClass);
                var lastRegion = regions[regions.length - 1];
                // msRegionOverflow has 3 different values - overflow indicates that there is still
                // content that the region was not able to display, empty means that no content from the
                // steam was left to place in this particular region
                if (lastRegion.msRegionOverflow === 'overflow') {
                    createPages();
                }
            });
          });
    }
    
    // This function gets called if the document has been resized, which could mean one of two things 
    // - the window (and therefore, the pages) have grown larger, which means more content will fit into 
    // each page and fewer pages are needed, or the window has grown smaller, and more pages are needed. 
    // The function examines the last page in the document, and determines if the last region in the 
    // document has overflow, in which case more pages are needed - this is done by calling createPages 
    // again. If this is not the case, the first region in the page is examined to find out if 
    // it is empty. If it is, the page is not needed (because it is not displaying any content) so we 
    // remove it, and continue to remove pages as long as there is no content in the first region. 
    function resizePages() {
        var flexboxElement = document.getElementById('pageContainer');
        var pages = flexboxElement.querySelectorAll('.page');
        var lastPage = pages[pages.length-1];
        
        var regions = lastPage.querySelectorAll(searchClass);
        var lastRegion = regions[regions.length-1];

        if (lastRegion.msRegionOverflow === 'overflow') {
            createPages();
        }
        else {
            var firstRegion = regions[0];
            while (firstRegion.msRegionOverflow === 'empty')
            {
                flexboxElement.removeChild(lastPage);
                currentPage = currentPage - 1;
                lastPage = pages[currentPage-1];
                regions = lastPage.querySelectorAll(searchClass);
                firstRegion = regions[0];
            }
        }
    }

    // This is a simple function to handle keyboard navigation between pages. 
    function handleKeystrokes(eventObject) {

        var Key = WinJS.Utilities.Key;
        var scrollingElement = document.getElementById('pageContainer');
        var scrollDistance = window.getComputedStyle(scrollingElement).getPropertyValue('width');
        var scrollValue = scrollDistance.substring(0, scrollDistance.indexOf('px'));

        switch (eventObject.keyCode) {
            case Key.leftArrow:
                scrollingElement.scrollLeft = scrollingElement.scrollLeft - parseInt(scrollValue);
                break;
            case Key.rightArrow:
                scrollingElement.scrollLeft = scrollingElement.scrollLeft + parseInt(scrollValue);
                break;
            default:
                return;
        }

        eventObject.preventDefault();
        eventObject.stopPropagation();

    }

    WinJS.UI.Pages.define('/html/regionTemplatePage.html', {
        ready: ready
    });

})();