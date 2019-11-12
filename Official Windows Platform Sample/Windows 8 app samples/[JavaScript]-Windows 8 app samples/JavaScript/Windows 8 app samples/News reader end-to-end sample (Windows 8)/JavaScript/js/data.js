(function () {
    "use strict";

    // Object containing the classes that define the
    // data.
    var Classes = {

        // The Feed class.
        Feed: WinJS.Class.define(
            function (feedData) {
                this.category = feedData.Category;
                this.title = feedData.Name;
                this.description = feedData.Description;
                this.image = feedData.imgURL;
                this.url = feedData.URL;
            },
            {
                // Feed properties.
                category: null,
                title: null,
                description: null,
                image: null,
                url: null,

                // Feed articles.
                articles: [],

                // Status.
                _loading: false,
                _loaded: false,
                _refresh: false,

                // Loading promise.
                _loadingPromise: null,

                // Downloads the articles of the feed.
                _downloadArticlesAsync: function () {
                    this._loading = true;
                    var that = this;
                    return new WinJS.Promise(function (complete, error) {
                        var feedUri = new Windows.Foundation.Uri(that.url);
                        Windows.Web.Syndication.SyndicationClient().retrieveFeedAsync(feedUri).then(function (syndicationFeed) {

                            that.articles = [];

                            syndicationFeed.items.forEach(function (syndicationItem) {
                                var article = new Classes.Article(syndicationItem, that);
                                that.articles.push(article);
                            });

                            that._loading = false;
                            that._loaded = true;
                            complete(that.articles); // Propagate success up with the current feed.
                        }, function () {
                            that._loaded = false;
                            that._loading = false;
                            that._loadingPromise = null;
                            error(); // Propagate the feed retrieval error up.
                        });
                    });
                },

                // Updates the articles of the feed.
                refreshArticlesAsync: function () {
                    this._downloadArticlesAsync();
                },

                // Loads the articles of the feed.
                loadArticlesAsync: function () {

                    // Return the existing promise if a refresh is already in progress.
                    if (!this._loading && !this._loaded) {
                        this._loadingPromise = this._downloadArticlesAsync();
                    }

                    return this._loadingPromise;
                }
            },
            {

            }
        ),

        // The Article class.
        Article: WinJS.Class.define(
            function (syndicationItem, feed) {
                try {

                    // Get and format the body div
                    var bodyDiv = this.getFormattedBody(syndicationItem);

                    // Get the feed.
                    this.feed = feed;

                    // Get the title.
                    var titleText = syndicationItem.title ? syndicationItem.title.text : "";
                    this.title = this.getSanitized(titleText);

                    // Get the body.
                    this.body = bodyDiv.innerHTML;

                    // Get the image.
                    var imageEl = bodyDiv.querySelector("img");
                    this.imageSrc = (imageEl) ? imageEl.getAttribute("src") : null;

                } catch (e) {
                    // Ignore malformed article.
                }
            },
            {

                // Public members
                feed: null,
                title: null,
                body: null,
                imageSrc: null,
                imageBackground: null,

                // Gets the formatted body of the article.
                getFormattedBody: function (syndicationItem) {

                    var bodyContent = "";

                    // Get the content from the Syndication Item
                    if (syndicationItem.content) {
                        bodyContent = syndicationItem.content.text;
                    } else {
                        bodyContent = syndicationItem.summary.text;
                    }

                    // Sanitize content.
                    bodyContent = this.getSanitized(bodyContent);

                    // Replace unnessecary spaces and linebreakes.
                    bodyContent = bodyContent.replace(/(&nbsp;|<br>)+/, "<br>");

                    // Create the div.
                    var body = document.createElement("div");

                    MSApp.execUnsafeLocalFunction(function () {
                        body.innerHTML = bodyContent;
                    });

                    WinJS.Utilities.query("[style]", body)
                            .setAttribute("style", "");

                    WinJS.Utilities.query(".feedflare", body)
                        .removeClass("feedflare");

                    return body;
                },

                // Sanitizes the given string.
                getSanitized: function (string) {

                    // Replace special characters.
                    var element = document.createElement("textarea");
                    element.innerHTML = string.replace(/</g, "&lt;").replace(/>/g, "&gt;");
                    string = element.value;

                    // Trim whitespace.
                    string = string.replace(/^\s\s*/, '');
                    var ws = /\s/,
                        i = string.length;
                    while (ws.test(string.charAt(--i))) { };
                    string = string.slice(0, i + 1);

                    // Set toStaticHTML to cleanse from potential attacks (i.e. <script> tags).
                    string = toStaticHTML(string);

                    return string;
                }
            }
        )

    };

    // Handler object for the data collection.
    // Takes care of keeping track of and organizing the data.
    var Feeds = {

        // List of all available feeds listed in feedCollection.json.
        all: [],

        // List of the subscribed feeds.
        subscribed: [],

        // Subscribes to the given feed.
        subscribe: function (feed) {
            feed.loadArticlesAsync();
            Feeds.subscribed.push(feed);
        },

        // Unsubscribes from the given feed.
        unsubscribe: function (feed) {
            var index = Feeds.subscribed.indexOf(feed);
            Feeds.subscribed.splice(index, 1);
        }
    };

    // Public interface.
    WinJS.Namespace.define("Data", {
        Feed: Classes.Feed,

        allFeeds: Feeds.all,
        subFeeds: Feeds.subscribed,

        subscribe: Feeds.subscribe,
        unsubscribe: Feeds.unsubscribe
    });
})();
