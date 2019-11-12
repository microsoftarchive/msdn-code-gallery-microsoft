(function () {
    "use strict";

    // All the items from the feed collection.
    // To be used by a list view that's meant to show all
    // the available feeds, categorized by the group they
    // belong to.
    // (Meant for the Subscriptions page)
    var groupedFeedItems = new WinJS.Binding.List().createGrouped(
        function groupKeySelector(item) { return item.group.key; },
        function groupDataSelector(item) { return item.group; }
    );

    // Sets up the list view for the subscriptions page.
    function setupFeedCollectionListView() {

        Data.allFeeds.forEach(function (feed) {
            var feedItem = getFeedItem(feed);
            groupedFeedItems.push(feedItem);
        });
    }

    // Given an instance of the Feed class, returns a simplified object
    // containing the properties that have to be rendered.
    function getFeedItem(feed) {
        return {
            group: { key: feed.category, title: feed.category },
            title: feed.title,
            description: feed.description,
            image: feed.image,
            url: feed.url
        };
    }

    // Given a list of feeds and the number of articles to display
    // per feed, returns a populated grouped binding list that contains
    // the articles of the feeds that the user has subscribed to.
    function getGroupedArticleItems(feeds, numOfItems) {

        var groupedItems = new WinJS.Binding.List().createGrouped(
            function groupKeySelector(item) { return item.group.key; },
            function groupDataSelector(item) { return item.group; }
        );

        for (var i = 0, feedsLen = feeds.length; i < feedsLen; i++) {
            feeds[i].loadArticlesAsync().done(function (articles) {
                var articleGroup = { key: articles[0].feed.url, title: articles[0].feed.title, largeTileTitle: null };
                var articlesLen = (numOfItems && numOfItems < articles.length) ? numOfItems : articles.length;
                for (var j = 0; j < articlesLen; j++) {
                    var article = articles[j];
                    var item = getArticleItem(article, articleGroup);
                    groupedItems.push(item);

                    if (j === 0) {
                        articleGroup.largeTileTitle = item.title;
                    }
                }
            });
        }

        return groupedItems;
    }

    // Given an instance of the Article class and an article group object, 
    // returns an article item that's meant for use by the list views.
    function getArticleItem(article, articleGroup) {
        return {
            group: articleGroup,
            title: article.title,
            body: article.body,
            imageSrc: article.imageSrc,
            imageBackground: null,
            tileType: "normal-text-tile"
        };
    }

    // Renders an item given an itemPromise.
    // (Meant for the article rendering pages -- currently
    // news.html and feed.html)
    function renderArticleTile(itemPromise) {
        return itemPromise.then(function (item) {
            var article = item.data;
            var div = document.createElement("div");
            var textArticleTemplate = document.getElementById("textArticleTemplate").winControl;

            if (article.group.largeTileTitle === article.title) {
                article.tileType = "large-text-tile";
            }

            textArticleTemplate.render(article).then(function (textArticleElement) {

                div.appendChild(textArticleElement);

                var imageIsValid = (article.imageSrc) ?
                    article.imageSrc.search(/.png$/) !== -1 ||
                    article.imageSrc.search(/.gif$/) !== -1 ||
                    article.imageSrc.search(/.jpg$/) !== -1 ||
                    article.imageSrc.search(/.bmp$/) !== -1 : false;

                if (imageIsValid) {

                    getImageSizeAsync(article.imageSrc).then(function (size) {
                        var hPass = size.height > 250;
                        var wPass = size.width > 250;
                        if (wPass && hPass) {

                            if (article.group.largeTileTitle === article.title) {
                                article.tileType = "large-image-tile";
                            } else {
                                article.tileType = "normal-image-tile";
                            }

                            article.imageBackground = "url('" + article.imageSrc + "') center center";

                            var imageArticleTemplateEl = document.getElementById("imageArticleTemplate");
                            if (!imageArticleTemplateEl) { 
                                return; // If imageArticleTemplateEl is empty, the user must be in another page by now.
                            }


                            var imageArticleTemplate = imageArticleTemplateEl.winControl;
                            imageArticleTemplate.render(article).then(function (imageArticleElement) {
                                WinJS.Utilities.empty(div);
                                div.appendChild(imageArticleElement);

                                // If height < minHeight, scale width down to minHeight.
                                // Else, scale smallerOf(width, height) down to minHeight.
                                var e = div.querySelector(".image");
                                if (!hPass) {
                                    WinJS.Utilities.addClass(e, "small-image");
                                } else {
                                    WinJS.Utilities.addClass(e, "large-image");
                                }
                            });
                        }
                    });
                }
            });

            return div;
        });
    }

    // Given the URL to an image, returns an object
    // containing the width and height of the image.
    function getImageSizeAsync(src) {
        return new WinJS.Promise(function (c, e) {
            var img = new Image();
            img.src = src;

            img.onload = function () {
                c({
                    width: img.width,
                    height: img.height
                });
            };

            img.onerror = function () {
                e();
            };
        });
    }

    // Given a feed item index of the groupedFeedItems list,
    // returns the corresponding instance of the Feed class.
    function getFeedObject(index) {
        var url = Render.groupedFeedItems.getAt(index).url;
        var feeds = Data.allFeeds;
        for (var i = 0, len = feeds.length; i < len; i++) {
            if (feeds[i].url === url) {
                return feeds[i];
            }
        }

        return null;
    }

    // Public interface.
    WinJS.Namespace.define("Render", {

        groupedFeedItems: groupedFeedItems,
        setupFeedCollectionListView: setupFeedCollectionListView,

        getFeedObject: getFeedObject,
        getGroupedArticleItems: getGroupedArticleItems,

        renderArticleTile: renderArticleTile,
        selectionIndices: []
    });
})();
