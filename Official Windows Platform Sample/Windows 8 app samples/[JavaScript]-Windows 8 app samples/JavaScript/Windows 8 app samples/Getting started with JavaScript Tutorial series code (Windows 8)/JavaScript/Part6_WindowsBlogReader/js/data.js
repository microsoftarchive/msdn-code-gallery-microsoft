(function () {
    "use strict";

    // Set up array variables
    var dataPromises = [];
    var blogs;

    // Create a data binding for our ListView
    var blogPosts = new WinJS.Binding.List();

    // Process the blog feeds
    function getFeeds() {
        // Create an object for each feed in the blogs array
        // Get the content for each feed in the blogs array
        // Return when all asynchronous operations are complete

        // Create an object for each feed in the blogs array
        blogs = [
        {
            key: "blog1",
            url: 'http://blogs.windows.com/skydrive/b/skydrive/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog2",
            url: 'http://blogs.windows.com/windows/b/windowsexperience/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog3",
            url: 'http://blogs.windows.com/windows/b/extremewindows/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog4",
            url: 'http://blogs.windows.com/windows/b/business/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog5",
            url: 'http://blogs.windows.com/windows/b/bloggingwindows/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog6",
            url: 'http://blogs.windows.com/windows/b/windowssecurity/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog7",
            url: 'http://blogs.windows.com/windows/b/springboard/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog8",
            url: 'http://blogs.windows.com/windows/b/windowshomeserver/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog9",
            url: 'http://blogs.windows.com/windows_live/b/developer/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog10",
            url: 'http://blogs.windows.com/ie/b/ie/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog11",
            url: 'http://blogs.windows.com/windows_phone/b/wpdev/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        },
        {
            key: "blog12",
            url: 'http://blogs.windows.com/windows_phone/b/wmdev/atom.aspx',
            title: 'tbd', updated: 'tbd',
            acquireSyndication: acquireSyndication, dataPromise: null
        }];

        // Get the content for each feed in the blogs array
        blogs.forEach(function (feed) {
            feed.dataPromise = feed.acquireSyndication(feed.url);
            dataPromises.push(feed.dataPromise);
        });

        // Return when all asynchronous operations are complete
        return WinJS.Promise.join(dataPromises).then(function () {
            return blogs;
        });


    }

    function acquireSyndication(url) {

        // Call xhr for the URL to get results asynchronously
        return WinJS.xhr(
            {
                url: url,
                headers: { "If-Modified-Since": "Mon, 27 Mar 1972 00:00:00 GMT" }

            });

    }

    function getBlogPosts() {
        // Walk the results to retrieve the blog posts
        getFeeds().then(function () {
            // Process each blog
            blogs.forEach(function (feed) {
                feed.dataPromise.then(function (articlesResponse) {

                    var articleSyndication = articlesResponse.responseXML;

                    if (articleSyndication) {
                        // Get the blog title 
                        feed.title = articleSyndication.querySelector("feed > title").textContent;

                        // Use the date of the latest post as the last updated date
                        var published = articleSyndication.querySelector("feed > entry > published").textContent;

                        // Convert the date for display
                        var date = new Date(published);
                        var dateFmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
                           "month.abbreviated day year.full");
                        var blogDate = dateFmt.format(date);
                        feed.updated = "Last updated " + blogDate;

                        // Get the blog posts
                        getItemsFromXml(articleSyndication, blogPosts, feed);
                    }
                    else {

                        // There was an error loading the blog. 
                        feed.title = "Error loading blog";
                        feed.updated = "Error";
                        blogPosts.push({
                            group: feed,
                            key: "Error loading blog",
                            title: feed.url,
                            author: "Unknown",
                            month: "?",
                            day: "?",
                            year: "?",
                            content: "Unable to load the blog at " + feed.url
                        });

                    }
                });
            });
        });

        return blogPosts;

    }

    function getItemsFromXml(articleSyndication, bPosts, feed) {

        // Get the info for each blog post
        var posts = articleSyndication.querySelectorAll("entry");

        // Process each blog post
        for (var postIndex = 0; postIndex < posts.length; postIndex++) {
            var post = posts[postIndex];

            // Get the title, author, and date published
            var postTitle = post.querySelector("title").textContent;
            var postAuthor = post.querySelector("author > name").textContent;
            var postPublished = post.querySelector("published").textContent;

            // Convert the date for display
            var postDate = new Date(postPublished);
            var monthFmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
                "month.abbreviated");
            var dayFmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
                "day");
            var yearFmt = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(
                "year.full");
            var blogPostMonth = monthFmt.format(postDate);
            var blogPostDay = dayFmt.format(postDate);
            var blogPostYear = yearFmt.format(postDate);

            // Process the content so it displays nicely
            var staticContent = toStaticHTML(post.querySelector("content").textContent);

            // Store the post info we care about in the array
            bPosts.push({
                group: feed,
                key: feed.title,
                title: postTitle,
                author: postAuthor,
                month: blogPostMonth.toUpperCase(),
                day: blogPostDay,
                year: blogPostYear,
                content: staticContent
            });
        }

    }


    var list = getBlogPosts();

    var groupedItems = list.createGrouped(
        function groupKeySelector(item) { return item.group.key; },
        function groupDataSelector(item) { return item.group; }
    );



    WinJS.Namespace.define("Data", {
        items: groupedItems,
        groups: groupedItems.groups,
        getItemReference: getItemReference,
        getItemsFromGroup: getItemsFromGroup,
        resolveGroupReference: resolveGroupReference,
        resolveItemReference: resolveItemReference
    });

    // Get a reference for an item, using the group key and item title as a
    // unique reference to the item that can be easily serialized.
    function getItemReference(item) {
        return [item.group.key, item.title];
    }

    // This function returns a WinJS.Binding.List containing only the items
    // that belong to the provided group.
    function getItemsFromGroup(group) {
        return list.createFiltered(function (item) { return item.group.key === group.key; });
    }

    // Get the unique group corresponding to the provided group key.
    function resolveGroupReference(key) {
        for (var i = 0; i < groupedItems.groups.length; i++) {
            if (groupedItems.groups.getAt(i).key === key) {
                return groupedItems.groups.getAt(i);
            }
        }
    }

    // Get a unique item from the provided string array, which should contain a
    // group key and an item title.
    function resolveItemReference(reference) {
        for (var i = 0; i < groupedItems.length; i++) {
            var item = groupedItems.getAt(i);
            if (item.group.key === reference[0] && item.title === reference[1]) {
                return item;
            }
        }
    }


})();

