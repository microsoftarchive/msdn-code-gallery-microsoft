/*

    This file contains all the calls that we make to the OData backend as well as interacting
    with any data we have previously retrieved.

*/


(function () {

    "use strict";

    var server = "http://data.buildwindows.com";
    var sessionFeedUrl = server + "/buildservice.svc/Sessions?$expand=Speakers,Files,Tags,SessionRelateds,SessionLinks";
    var sessionFavoritesFeedUrl = server + "/buildservice.svc/SessionFavorites";
    var deleteSessionFavoritesUrl = server + "/Favorite/Session/";
    var speakerFeedUrl = server + "/buildservice.svc/Speakers?$expand=Sessions&$select=*,Sessions/SessionCode";
    var attendeeFeedUrl = server + "/buildservice.svc/Attendees";
    var attendeeFavoritesFeedUrl = server + "/buildservice.svc/AttendeeFavorites";
    var deleteAttendeeFavoritesUrl = server + "/Favorite/Attendee/";
    var agendaItemsFeedUrl = server + "/buildservice.svc/AgendaItems";
    var newsFeedUrl = server + "/buildservice.svc/News";
    var roomStatusUrl = server + "/buildservice.svc/RoomStatus";
    var mapRoot = "http://build.blob.core.windows.net/media/Default/map/";
    var authServiceHost = "buildwindows.com";
    var authToken = "";
    var syncIntervalId = null;
    var showAllUpcomingFlag = false;

    function isSignedIn() {
        /// <returns type="Boolean"></returns>
        if (authToken) {
            return true;
        }
        else {
            return false;
        }
    }
  
    function signOut() {
        /// <summary>Clears the authToken and signs the user out, if able to do so. 
        ///     If they are signed in with a Microsoft account or have connected one, 
        ///     this isn't going to do anything.</summary>
        authToken = "";
        var clientAuth = new Windows.Security.Authentication.OnlineId.OnlineIdAuthenticator();
        if (clientAuth.canSignOut) {
            return clientAuth.signOutUserAsync();
        }
        else {
            return WinJS.Promise.wrap(true);
        }
    }

    function canSignOut() {
        /// <summary>Checks if the user can sign out, see comment in signOut.</summary>
        var clientAuth = new Windows.Security.Authentication.OnlineId.OnlineIdAuthenticator();
        return clientAuth.canSignOut;
    }

    
    function getFeed(feedUrl) {
        /// <summary>getFeed fetches an OData feed from the server. 
        /// It is smart enough to follow "__next" links in OData responses and compile all of the results.</summary>
        return new WinJS.Promise(function (c, e, p) {
            var results = [];
            
            function helper(url) {
                var options = {
                    url: url,
                    headers: { 'Accept': 'application/json', 'Authorization': authToken, 'X-Secret':'BuildAppSecret'},
                };
                WinJS.xhr(options).then(function (req) {
                    var json = JSON.parse(req.responseText);
                    results = results.concat(json.d.results);
                    if (json.d.__next) {
                        helper(json.d.__next);
                    }
                    else {
                        c({ results: results });
                    }
                }, e, p);
            }

            helper(feedUrl);
        });
    }


    function postFeed(feedUrl, dataObj) {
        /// <summary>Posts the given JSON data to an OData feed.</summary>
        var options = {
            url: feedUrl,
            headers: { 'Accept': 'application/json', 'Authorization': authToken, 'Content-Type': 'application/json', 'X-Secret': 'BuildAppSecret' },
            type: 'POST',
            data: JSON.stringify(dataObj)
        };

        return WinJS.xhr(options);
    }

    function signIn() {
        /// <summary>Gets a Live auth token using the user's Microsoft account.</summary>
        var clientAuth = new Windows.Security.Authentication.OnlineId.OnlineIdAuthenticator();
        var target = Windows.Security.Authentication.OnlineId.OnlineIdServiceTicketRequest(authServiceHost, "JWT");
        return clientAuth.authenticateUserAsync(target).then(
            function (userIdentity) {
                authToken = userIdentity.tickets[0].value;
                return authToken;
            }
        );
    }

    function deleteFeed(feedUrl) {
        /// <summary>Calls DELETE on the given OData feed. Used for removing server objects e.g. favorites</summary>
        var options = {
            url: feedUrl,
            headers: { 'Accept': 'application/json', 'Authorization': authToken, 'X-Secret': 'BuildAppSecret' },
            type: 'DELETE'
        };

        return WinJS.xhr(options);
    }

    function loadInitialData() {
        /// <summary>Retrieves conference data from the remove server and caches it locally.</summary>
        ConferenceApp.Util.updateLoadingMessage("Caching data from feeds");
        return loadSessions().then(loadSessionFavorites).then(loadSpeakers).then(loadAttendees).then(loadAttendeeFavorites).then(loadAgendaItems).then(loadNews).then(loadRoomStatus).then(loadMaps).then(ConferenceApp.State.saveAll);
    }

    function loadSessions() {
        /// <summary>Retrieves all the session data if it has not already been retrieved.</summary>
        ConferenceApp.Util.updateLoadingMessage("Loading session data");
        if (ConferenceApp.Db.hasCollection('sessions')) {
            return WinJS.Promise.wrap(false);
        }
        else {
            return getFeed(sessionFeedUrl).then(function(data) {
                data.results.forEach(function(item) {
                    item.StartTime = ConferenceApp.Util.parseODataDate(item.StartTime);
                });
                ConferenceApp.Db.createCollection('sessions', data.results);
                ConferenceApp.State.local.sessionFeedLastUpdated = new Date();
            });
        }
    }

    function syncSessions() {
        /// <summary>Updates the session data store, filtering on the LastUpdated property from when we last synced. </summary>
        var lastUpdated = ConferenceApp.State.local.sessionFeedLastUpdated;
        if (lastUpdated) {
            var feedUrl = sessionFeedUrl + '&$filter=' + encodeURIComponent("LastUpdated ge datetime'" + lastUpdated.toISOString() + "'");
            return getFeed(feedUrl).then(function (data) {
                data.results.forEach(function (item) {
                    item.StartTime = ConferenceApp.Util.parseODataDate(item.StartTime);
                    ConferenceApp.Db.replaceItem('sessions', item, 'SessionCode');
                });
                ConferenceApp.State.local.sessionFeedLastUpdated = new Date();
                ConferenceApp.State.saveAll();
            });
        }
        else {
            return loadSessions();
        }
    }

    function getSession(sessionCode) {
        /// <summary>Fetches the JSON object for a particular session out of the cache.</summary>
        var results = getSessions().filterBy("SessionCode", sessionCode);
        if (results.length > 0) {
            return results[0];
        }
        else {
            return null;
        }
    }

    function getSessions() {
        /// <summary>Returns all of the cached session objects.</summary>
        return ConferenceApp.Db.all('sessions');
    }

    function getUpcomingSessions(includeAgendaItems) {
        /// <summary>Gets the "upcoming" items for a user. 
        ///     This means grab all their favorite sessions and show ones that have a StartTime greater than ten minutes ago
        ///     and less than 2 hours from now.</summary>
        /// <param name="includeAgendaItems" type="Boolean">If true, includes agenda items as well as sessions</param>
        var data;
        if (includeAgendaItems) {
            data = getScheduleItems();
        }
        else {
            data = getSessions();
        }
        var sortedData = data.sortBy("SessionCode").sortBy("StartTime");
        sortedData = sortedData.filter(function (item) {
            var isFavorite = isSessionFavorite(item.SessionCode);
            var isAgendaItem = 'AgendaItemId' in item;
            return isFavorite || isAgendaItem;
        });

        if (showAllUpcomingFlag) {
            return sortedData;
        }

        var start = new Date();
        start.setMinutes(start.getMinutes() -10);
        var end = new Date();
        end.setHours(end.getHours() + 2);
        sortedData = sortedData.filter(function (item) {
            if (item.StartTime >= start && item.StartTime <= end) {
                return true;
            }
            else {
                return false;
            }
        });
        return sortedData;
    }

    function getPopularSessions() {
        /// <summary>Get the most popular 20 sessions</summary>
        var results = ConferenceApp.Db.all('sessions').sortBy("Popularity").reverse();
        return results.slice(0, 20);
    }

    function getFeaturedSessions() {
        /// <summary>Get the "featured" sessions. This is what replaced popular sessions in the post-conference experience.</summary>
        var featured = ["BPS-1004", "BPS-1005", "BPS-1006", "KEY-0001", "KEY-0002"];
        var results = ConferenceApp.Db.all('sessions');
        results = results.filter(function (item) {
            return featured.indexOf(item["SessionCode"]) !== -1;
        });
        return ConferenceApp.Db.extendCollection(results).sortBy("StartTime");
    }

    function getRelatedSessions(sessionCodeList) {
        /// <summary>Given an array of related session codes, fetch the JSON data for each of those sessions.</summary>
        var results = [];
        sessionCodeList.forEach(function (item) {
            var related = getSessions().filterBy("SessionCode", item.RelatedSession)[0];
            if (related) {
                results.push(related);
            }
        });
        return results;
    }

    function getSpeakerSessions(speaker) {
        /// <summary>Get all of the session objects for a particular speaker.</summary>
        var sessions = speaker.Sessions.results;
        var results = [];
        sessions.forEach(function (session) {
            var fullSession = ConferenceApp.Db.all('sessions').filterBy('SessionCode', session.SessionCode)[0];
            if (fullSession) {
                results.push(fullSession);
            }
        });
        return results;
    }

    function getSessionVideo(fileList) {
        /// <summary>Given the file list from a session object, try to find the right video URL (MP4 high) so we can play it</summary>
        var bestMatch = null;
        for(var i = 0; i < fileList.length; i++) {
            if (fileList[i].MimeType === "video/mp4") {
                if (!bestMatch || fileList[i].Type === 'MP4 High') {
                    bestMatch = fileList[i].FileUri;
                }
            }
        }
        return bestMatch;
    }

    function getSessionSlides(fileList) {
        /// <summary>Given the file list from a session object, return the slide URL.</summary>
        for (var i = 0; i < fileList.length; i++) {
            if (fileList[i].MimeType === "application/vnd.ms-powerpoint") {
                return fileList[i].FileUri;
            }
        }
        return null;
    }

    function getRelatedLinks(linkList) {
        /// <summary>Reformat an array of links from session data so they are grouped by type.</summary>
        var links = ConferenceApp.Db.extendCollection(linkList).groupBy("Type", true);
        var results = [];
        for (var type in links) {
            results.push({ Type: type, Links: links[type] });
        }
        return results;
    }

    function loadSessionFavorites(refresh) {
        /// <summary>Load session favorites, if they have not been loaded already.</summary>
        /// <param name="refresh" type="Boolean">If true, always reload session favorites, even if already loaded.</param>
        ConferenceApp.Util.updateLoadingMessage("Loading session favorites");
        if (ConferenceApp.Db.hasCollection('sessionFavorites') && !refresh) {
            return WinJS.Promise.wrap(false);
        }
        else {
            return getFeed(sessionFavoritesFeedUrl).then(function (data) {
                var sessionList = {};
                data.results.forEach(function (item) {
                    sessionList[item.SessionCode] = item.TimeStamp;
                    });
                ConferenceApp.Db.createCollection('sessionFavorites', [sessionList]);
            });
        }
    }

    function refreshSessionFavorites() {
        /// <summary>Equivalent to calling loadSessionFavorites(true).</summary>
        return loadSessionFavorites(true).then(ConferenceApp.State.saveAll);
    }

    function getSessionFavorites() {
        /// <summary>Returns the list of all favorited SessionCodes as an object with one property per favorited session.</summary>
        var favorites = ConferenceApp.Db.all('sessionFavorites');
        if (favorites.length > 0) {
            return favorites[0];
        }
        else {
            var sessionList = {};
            ConferenceApp.Db.createCollection('sessionFavorites', [sessionList]);
            return sessionList;
        }
    }

    function isSessionFavorite(sessionCode) {
        /// <summary>True if the user has favorited the session with the given session code.</summary>
        return sessionCode in getSessionFavorites();
    }

    function syncSessionFavorites() {
        /// <summary>Sync favorites with the server. Push any local changes before retrieving remote updates.</summary>
        var pending = ConferenceApp.Db.all('pendingSessionFavorites');

        if (pending.length === 0) {
            loadSessionFavorites(true);
        }

        pending.forEach(function (item) {
            if (item['Operation'] === 'ADD') {
                createSessionFavoriteOnServer(item['SessionCode']).then(function () {
                    ConferenceApp.Db.deleteItem('pendingSessionFavorites', item);
                    return ConferenceApp.State.saveAll();
                });
            }
            else if (item['Operation'] === 'DELETE') {
                deleteSessionFavoriteOnServer(item['SessionCode']).then(function() {
                    ConferenceApp.Db.deleteItem('pendingSessionFavorites', item);
                    return ConferenceApp.State.saveAll();
                });
            }
        });
        
    }

    function createSessionFavorite(sessionCode) {
        /// <summary>Lazily add this session as a favorite during the next sync.</summary>
        if (sessionCode) {
            getSessionFavorites()[sessionCode] = true;
            var existingOp = ConferenceApp.Db.all('pendingSessionFavorites').filterBy('SessionCode', sessionCode)[0];
            if (existingOp) {
                if (existingOp['Operation'] !== 'ADD') {
                    ConferenceApp.Db.deleteItem('pendingSessionFavorites', existingOp);
                }
            }
            else {
                ConferenceApp.Db.put('pendingSessionFavorites', { 'SessionCode': sessionCode, 'Operation': 'ADD' });
            }
        }
        return ConferenceApp.State.saveAll();
    }

    function createSessionFavoriteOnServer(sessionCode) {
        /// <summary>Send the favorite add request to the server.</summary>
        return postFeed(sessionFavoritesFeedUrl, { 'SessionCode': sessionCode }).then(
            null,
            function (xhr) {
                if (xhr.status === 500) {
                    // favorite already exists, ignore request.
                    return true;
                }
                throw xhr;
            }
        );
    }

    function deleteSessionFavorite(sessionCode) {
        /// <summary>Lazily remove a session favorite during the next sync with the server.</summary>
        if (sessionCode) {
            delete getSessionFavorites()[sessionCode];
            var existingOp = ConferenceApp.Db.all('pendingSessionFavorites').filterBy('SessionCode', sessionCode)[0];
            if (existingOp) {
                if (existingOp['Operation'] !== 'DELETE') {
                    ConferenceApp.Db.deleteItem('pendingSessionFavorites', existingOp);
                }
            }
            else {
                ConferenceApp.Db.put('pendingSessionFavorites', { 'SessionCode': sessionCode, 'Operation': 'DELETE' });
            }
        }
        return ConferenceApp.State.saveAll();
    }

    function deleteSessionFavoriteOnServer(sessionCode) {
        /// <summary>Remove the favorite from the server.</summary>
        return deleteFeed(deleteSessionFavoritesUrl + sessionCode).then(
            null, 
            function (xhr) {
                if (xhr.status === 404) {
                    // favorite already deleted, ignore request.
                    return true;
                }
                throw xhr;
            }
        );
    }

    function loadSpeakers(refresh) {
        /// <summary>Retrieve the list of speakers from the server if we haven't done so already.</summary>
        ConferenceApp.Util.updateLoadingMessage("Loading speakers");
        if (ConferenceApp.Db.hasCollection('speakers') && !refresh) {
            return WinJS.Promise.wrap(false);
        }
        else {
            return getFeed(speakerFeedUrl).then(function(data) {
                ConferenceApp.Db.createCollection('speakers', data.results);
            });
        }
    }

    function getSpeakers() {
        /// <summary>Return all cached speakers.</summary>
        return ConferenceApp.Db.all('speakers');
    }

    function getSpeaker(attendeeId) {
        /// <summary>Return a particular speaker, by their AttendeeId</summary>
        return getSpeakers().filterBy('AttendeeId', attendeeId)[0];
    }

    function loadAttendees(refresh) {
        /// <summary>Load attendees from the server, if we have not already done so.</summary>
        /// <param name="refresh" type="Boolean">If true, force a refresh and reload from server even if we have cached data.</param>
        ConferenceApp.Util.updateLoadingMessage("Loading attendee directory");
        if (ConferenceApp.Db.hasCollection('attendees') && !refresh) {
            return WinJS.Promise.wrap(false);
        }
        else {
            return getFeed(attendeeFeedUrl).then(function (data) {
                ConferenceApp.Db.createCollection('attendees', data.results);
            });
        }
    }

    function getAttendees() {
        /// <summary>Return all cached attendees.</summary>
        return ConferenceApp.Db.all('attendees');
    }

    function getAttendee(attendeeId) {
        /// <summary>Return a particular attendee.</summary>
        var matches = getAttendees().filterBy('AttendeeId', attendeeId);
        if (matches.length > 0) {
            return matches[0];
        }
        else {
            return null;
        }
    }

    function syncPeople() {
        /// <summary>Forces a refresh of both speakers and attendees.</summary>
        loadSpeakers(true);
        loadAttendees(true);
    }

    function loadNews(refresh) {
        /// <summary>Retrieve news items if we have not already done so.</summary>
        /// <param name="refresh" type="Boolean">If true, ignore cached news and re-retrieve anyway.</param>
        ConferenceApp.Util.updateLoadingMessage("Retrieving news");
        if (ConferenceApp.Db.hasCollection('news') && !refresh) {
            return WinJS.Promise.wrap(false);
        }
        else {
            return getFeed(newsFeedUrl).then(function (data) {
                data.results.forEach(function (item) {
                    item.PublishTime = ConferenceApp.Util.parseODataDate(item.PublishTime);
                });
                ConferenceApp.Db.createCollection('news', data.results);
            });
        }
    }

    function getNews(onlyTicker) {
        /// <summary>Retrieves cached news items</summary>
        /// <param name="onlyTicket" type="Boolean">Return only news items that are supposed to show up in the ticker.</param>
        var news = ConferenceApp.Db.all('news');

        if (onlyTicker) {
            news = news.filterBy("ShowInTickerFlag", true);
        }

        return news;
    }

    function syncNews() {
        /// <summary>Force a refresh of the news items.</summary>
        loadNews(true);
    }

    function loadRoomStatus(refresh) {
        /// <summary>Sync room data with the server, if we haven't already.</summary>
        /// <param name="refresh" type="Boolean">If true, refresh room data even if there is cached data.</param>
        ConferenceApp.Util.updateLoadingMessage("Retrieving room status");
        if (ConferenceApp.Db.hasCollection('roomStatus') && !refresh) {
            return WinJS.Promise.wrap(false);
        }
        else {
            return getFeed(roomStatusUrl).then(function (data) {
                var roomStatus = {};
                data.results.forEach(function (item) {
                    roomStatus[item.Room] = item.Status;
                });
                ConferenceApp.Db.createCollection('roomStatus', [roomStatus]);
            });
        }
    }

    function getRoomStatus(roomId) {
        /// <summary>Get the status for a particular room. 
        /// Possible values are "Filling" and "Full".
        /// If there is no data for the room, return empty string.</summary>
        var roomStatus = ConferenceApp.Db.all('roomStatus')[0];
        if (roomStatus) {
            if (roomId in roomStatus) {
                if (roomStatus[roomId] === 1) {
                    return "Filling";
                }
                else if (roomStatus[roomId] === 2) {
                    return "Full";
                }
            }
        }
        // room is fine, nothing to report
        return "";
    }

    function syncRoomStatus() {
        /// <summary>Force a refresh of the room status data.</summary>
        loadRoomStatus(true);
    }

    function loadAttendeeFavorites(refresh) {
        /// <summary>Retrieve people favorites from server.</summary>
        ConferenceApp.Util.updateLoadingMessage("Populating contacts");
        if (ConferenceApp.Db.hasCollection('attendeeFavorites') && !refresh) {
            return WinJS.Promise.wrap(false);
        }
        else {
            return getFeed(attendeeFavoritesFeedUrl).then(function (data) {
                var attendeeList = {};
                data.results.forEach(function (item) {
                    attendeeList[item.AttendeeId] = item.TimeStamp;
                });
                ConferenceApp.Db.createCollection('attendeeFavorites', [attendeeList]);
            });
        }
    }

    function refreshAttendeeFavorites() {
        /// <summary>Refresh people favorites from server.</summary>
        return loadAttendeeFavorites(true).then(ConferenceApp.State.saveAll);
    }

    function getAttendeeFavorites() {
        /// <summary>Get all cached people favorites.</summary>
        var favorites = ConferenceApp.Db.all('attendeeFavorites');
        if (favorites.length > 0) {
            return favorites[0];
        }
        else {
            var attendeeList = {};
            ConferenceApp.Db.createCollection('attendeeFavorites', [attendeeList]);
            return attendeeList;
        }
    }

    function isAttendeeFavorite(attendeeId) {
        /// <summary>True when given attendeeId is a favorite.</summary>
        return attendeeId in getAttendeeFavorites();
    }

    function syncAttendeeFavorites() {
        /// <summary>Rationalize pending people favorite changes with the server.</summary>
        var pending = ConferenceApp.Db.all('pendingAttendeeFavorites');

        if (pending.length === 0) {
            refreshAttendeeFavorites();
        }

        pending.forEach(function (item) {
            if (item['Operation'] === 'ADD') {
                createAttendeeFavoriteOnServer(item['AttendeeId']).then(function () {
                    ConferenceApp.Db.deleteItem('pendingAttendeeFavorites', item);
                    return ConferenceApp.State.saveAll();
                });
            }
            else if (item['Operation'] === 'DELETE') {
                deleteAttendeeFavoriteOnServer(item['AttendeeId']).then(function () {
                    ConferenceApp.Db.deleteItem('pendingAttendeeFavorites', item);
                    return ConferenceApp.State.saveAll();
                });
            }
        });

    }

    function createAttendeeFavorite(attendeeId) {
        /// <summary>Lazily add a people favorite.</summary>
        if (attendeeId) {
            getAttendeeFavorites()[attendeeId] = true;
            var existingOp = ConferenceApp.Db.all('pendingAttendeeFavorites').filterBy('AttendeeId', attendeeId)[0];
            if (existingOp) {
                if (existingOp['Operation'] !== 'ADD') {
                    ConferenceApp.Db.deleteItem('pendingAttendeeFavorites', existingOp);
                }
            }
            else {
                ConferenceApp.Db.put('pendingAttendeeFavorites', { 'AttendeeId': attendeeId, 'Operation': 'ADD' });
            }
        }
        return ConferenceApp.State.saveAll();
    }

    function createAttendeeFavoriteOnServer(attendeeId) {
        /// <summary>Send the people favorite request to the server.</summary>
        return postFeed(attendeeFavoritesFeedUrl, { 'AttendeeId': attendeeId }).then(
            null,
            function (xhr) {
            if (xhr.status === 500) {
                // favorite already exists, ignore request.
                return true;
            }
            throw xhr;
        }
        );
    }

    function deleteAttendeeFavorite(attendeeId) {
        /// <summary>Lazily delete a favorite.</summary>
        if (attendeeId) {
            delete getAttendeeFavorites()[attendeeId];
            var existingOp = ConferenceApp.Db.all('pendingAttendeeFavorites').filterBy('AttendeeId', attendeeId)[0];
            if (existingOp) {
                if (existingOp['Operation'] !== 'DELETE') {
                    ConferenceApp.Db.deleteItem('pendingAttendeeFavorites', existingOp);
                }
            }
            else {
                ConferenceApp.Db.put('pendingAttendeeFavorites', { 'AttendeeId': attendeeId, 'Operation': 'DELETE' });
            }
        }
        return ConferenceApp.State.saveAll();
    }

    function deleteAttendeeFavoriteOnServer(attendeeId) {
        /// <summary>Send the delete request to the server.</summary>

        return deleteFeed(deleteAttendeeFavoritesUrl + attendeeId).then(
            null,
            function (xhr) {
            if (xhr.status === 404) {
                // favorite already deleted, ignore request.
                return true;
            }
            throw xhr;
        }
        );
    }

    function loadAgendaItems(refresh) {
        /// <summary>Get agenda items from the server if not already cached.</summary>
        /// <param name="refresh" type="Boolean">If true, discard cache.</param>
        ConferenceApp.Util.updateLoadingMessage("Retrieving agenda items");
        if (ConferenceApp.Db.hasCollection('agendaItems') && !refresh) {
            return WinJS.Promise.wrap(false);
        }
        else {
            return getFeed(agendaItemsFeedUrl).then(function (data) {
                data.results.forEach(function (item) {
                    item.StartTime = ConferenceApp.Util.parseODataDate(item.StartTime);
                    item.EndTime = ConferenceApp.Util.parseODataDate(item.EndTime);
                });
                var filtered = data.results.filter(function (item) {
                    if (item.StartTime.getDay() === 1) {
                        return false;
                    }
                    return true;
                });
                ConferenceApp.Db.createCollection('agendaItems', filtered);
            });
        }
    }

    function getScheduleItems() {
        /// <summary>Return all agenda items that do not have sub items (should display on schedule)</summary>
        var sessions = getSessions().filter(function (item) {
            if (item.StartTime) {
                return true;
            }
            return false;
        });
        var agendaItems = ConferenceApp.Db.all('agendaItems').filterBy('HasSubItems', false);
        return ConferenceApp.Db.extendCollection(sessions.concat(agendaItems));
    }

    function syncAgendaItems() {
        /// <summary>Refresh cache of agenda items.</summary>
        loadAgendaItems(true);
    }

    function stopSync() {
        /// <summary>Stop syncing data with the server.</summary>
        if (syncIntervalId !== null) {
            clearInterval(syncIntervalId);
            syncIntervalId = null;
        }
    }
    
    // this function keeps track of a list of when different feeds need updating.
    // it is done this way instead of just using setInterval since PLM can end up clearing intervals
    // and it is possible for the longer intervals to never be hit.
    function startSync() {

        if (syncIntervalId !== null) {
            return;
        }

        var timerCallbacks = {};
        
        function checkTimers() {
            var now = new Date();
            var dirty = false;
            for (var timerName in ConferenceApp.State.local.timers) {
                var timer = ConferenceApp.State.local.timers[timerName];
                if (now > timer.done) {
                    setImmediate(timerCallbacks[timerName]);
                    var newDone = new Date();
                    newDone.setSeconds(newDone.getSeconds() + timer.length);
                    timer.done = newDone;
                    dirty = true;
                }

            }
            if (dirty) {
                setImmediate(ConferenceApp.State.saveAll);
            }
        }

        // length is in seconds
        function setTimer(timerName, callback, lengthInSeconds) {
            timerCallbacks[timerName] = callback;
            if (!ConferenceApp.State.local.timers[timerName]) {
                var done = new Date();
                done.setSeconds(done.getSeconds() + lengthInSeconds);
                var timer = { done: done, length: lengthInSeconds, name: timerName };
                ConferenceApp.State.local.timers[timerName] = timer;
            }
        }

        if (!ConferenceApp.State.local.timers) {
            ConferenceApp.State.local.timers = {};
        }

        // sync favorites up every minute
        setTimer('sessionFavorites', syncSessionFavorites, 60);
        // sync attendee favorites every minute
        setTimer('attendeeFavorites', syncAttendeeFavorites, 60);
        // sync session data every 10 minutes
        setTimer('sessions', syncSessions, 600);
        // sync agenda items (non-sessions) every 10 minutes
        setTimer('agendaItems', syncAgendaItems, 600);
        // sync people (attendees / speakers) every half hour
        setTimer('people', syncPeople, 1800);
        // sync news articles every 5 minutes
        setTimer('news', syncNews, 300);
        // sync room status every 2 minutes
        setTimer('roomStatus', syncRoomStatus, 120);

        ConferenceApp.State.saveAll();
        // every ten seconds check if we have to sync something.
        syncIntervalId = setInterval(checkTimers, 10000);
        
    }

    function search(query) {
        /// <summary>Search local caches for a given query. This searchs both people and sessions.</summary>
        var personConfig = [{ field: 'Name', weight: 5 }, { field: 'TwitterHandle', weight: 1 }, { field: 'Technologies', weight: 1 }, { field: 'State', weight: 1 }, { field: 'Languages', weight: 1 }, { field: 'Company', weight: 5 }, { field: 'Country', weight: 1 }, { field: 'City', weight: 1 }, {field:'Industry', weight:1}];
        var sessionConfig = [{ field: 'Abstract', weight: 1 }, { field: 'SessionCode', weight: 10 }, { field: 'Room', weight: 1 }, { field: 'Title', weight: 5 }];
        var results = [];

        var people = ConferenceApp.Db.search('attendees', personConfig, query, 'person');
        results = results.concat(people);

        var sessions = ConferenceApp.Db.search('sessions', sessionConfig, query, 'session');
        results = results.concat(sessions);

        results.sort(function (a, b) {
            return b.score - a.score;
        });
        
        results.personCount = people.length;
        results.sessionCount = sessions.length;
        return ConferenceApp.Db.extendCollection(results);
    }

    // For each type/value assign a color in order (1-5) and remember the choice.
    // Some categories have pre-defined colors.
    function getCategoryColor(type, value, isSlash) {
        var colorNumber = 1;
        var knownColors = {
            'apps': 1,
            'platform': 2,
            'tools': 3,
            'hardware': 4,
            'server+cloud': 5,
            'phone': 6
        };

        value = value.toLowerCase();
        if (value in knownColors) {
            colorNumber = knownColors[value];
        }
        else {
            var numColors = 5;
            if (!ConferenceApp.State.local.savedColors) {
                ConferenceApp.State.local.savedColors = {};
            }
            var colorStore = ConferenceApp.State.local.savedColors;
            if (!colorStore[type]) {
                colorStore[type] = { values: {}, lastColor: 0 };
            }
            colorNumber = colorStore[type].values[value];
            if (!colorNumber) {
                colorNumber = (colorStore[type].lastColor + 1) % (numColors + 1);
                if (colorNumber === 0) {
                    colorNumber = 1;
                }
                colorStore[type].lastColor = colorNumber;
                colorStore[type].values[value] = colorNumber;
            }
        }

        var prefix = 'itemCategoryColor';
        if (isSlash) {
            prefix = 'slashColor';
        }
        
        return prefix + colorNumber;
    }

    function getAllNotes() {
        /// <summary>Retrieve all saved session notes from local cache.</summary>
        var favorites = ConferenceApp.Db.all('sessionNotes');
        if (favorites.length > 0) {
            return favorites[0];
        }
        else {
            var sessionNotes = {};
            ConferenceApp.Db.createCollection('sessionNotes', [sessionNotes]);
            return sessionNotes;
        }
    }

    function getNotesForSession(sessionCode) {
        /// <summary>Retrieve saved session nodes for a particular session.</summary>
        var note = getAllNotes()[sessionCode];
        note = note || '';
        return note;
    }

    function setNotesForSession(sessionCode, noteData) {
        /// <summary>Update the notes for a session</summary>
        if (ConferenceApp.Util.trim(noteData).length === 0) {
            delete getAllNotes()[sessionCode];
        }
        else {
            getAllNotes()[sessionCode] = noteData;
        }
    }

    function getTripReportNotes() {
        /// <summary>Format all session notes as a Trip Report for exporting.</summary>
        var notes = getAllNotes();
        var notesString = [];
        var sep = "\r\n\r\n";
        Object.keys(notes).forEach(function (sessionCode) {
            var session = ConferenceApp.Db.all('sessions').filterBy('SessionCode', sessionCode)[0];
            notesString.push(sessionCode + "  ");
            if (session) {
                notesString.push(session.Title);
                notesString.push("\r\n");
                notesString.push(ConferenceApp.View.formatTime(session.StartTime).longString());
                notesString.push("\r\n");
                notesString.push(ConferenceApp.View.formatSpeakers(session.Speakers.results));
            }
            notesString.push(sep);
            notesString.push(notes[sessionCode]);
            notesString.push(sep);
            notesString.push(sep);
        });

        if (notesString.length === 0) {
            notesString.push('To create a trip report, first take notes by visiting a session detail page.');
        }

        return notesString.join('');
    }

    function loadMap(mapId) {
        /// <summary>Retrieve a floormap from the server.</summary>
        var options = {
            url: mapRoot + mapId + ".svg",
        };

        return WinJS.xhr(options).then(function (xhr) {
            ConferenceApp.Db.all('maps')[0][mapId] = xhr.responseText;
        });
    }

    function loadMaps() {
        /// <summary>Retrieve all floormaps from the server.</summary>
        ConferenceApp.Util.updateLoadingMessage("Caching maps");
        if (ConferenceApp.Db.hasCollection('maps')) {
            return WinJS.Promise.wrap(false);
        }
        else {
            ConferenceApp.Db.createCollection('maps', [{}]);
            var maps = {
                'level1': loadMap('level1'),
                'level2': loadMap('level2'),
                'level3': loadMap('level3'),
                'legend': loadMap('legend'),
                'marriott' : loadMap('marriott')
            };
            return WinJS.Promise.join(maps);
        }
    }

    function getMaps() {
        /// <summary>Return cached maps.</summary>
        return ConferenceApp.Db.all('maps')[0];
    }

    function showAllUpcoming(enable) {
        /// <summary>If enable is true, show all favorited sessions as "upcoming".</summary>
        if (enable) {
            showAllUpcomingFlag = true;
        }
        else {
            showAllUpcomingFlag = false;
        }
    }

    function clearUserData() {
        /// <summary>Remove all user-specific cached data.</summary>
        ConferenceApp.Db.deleteCollection('sessionFavorites');
        ConferenceApp.Db.deleteCollection('pendingSessionFavorites');
        ConferenceApp.Db.deleteCollection('attendeeFavorites');
        ConferenceApp.Db.deleteCollection('pendingAttendeeFavorites');
    }

    
    WinJS.Namespace.define("ConferenceApp.Api", {
        getSessions: getSessions,
        getSession: getSession,
        getUpcomingSessions: getUpcomingSessions,
        getPopularSessions: getPopularSessions,
        getSessionVideo: getSessionVideo,
        getSessionSlides: getSessionSlides,
        getSpeakers: getSpeakers,
        getSpeaker: getSpeaker,
        getSpeakerSessions: getSpeakerSessions,
        getRelatedSessions: getRelatedSessions,
        getAttendees: getAttendees,
        getAttendee: getAttendee,
        getScheduleItems: getScheduleItems,
        loadInitialData: loadInitialData,
        isSessionFavorite: isSessionFavorite,
        createSessionFavorite: createSessionFavorite,
        deleteSessionFavorite: deleteSessionFavorite,
        isAttendeeFavorite: isAttendeeFavorite,
        createAttendeeFavorite: createAttendeeFavorite,
        deleteAttendeeFavorite: deleteAttendeeFavorite,
        startSync: startSync,
        stopSync: stopSync,
        search: search,
        signIn: signIn,
        signOut: signOut,
        canSignOut: canSignOut,
        isSignedIn: isSignedIn,
        getCategoryColor: getCategoryColor,
        getNotesForSession: getNotesForSession,
        setNotesForSession: setNotesForSession,
        getTripReportNotes: getTripReportNotes,
        getRoomStatus: getRoomStatus,
        getNews: getNews,
        getRelatedLinks: getRelatedLinks,
        getMaps: getMaps,
        clearUserData: clearUserData,
        showAllUpcoming: showAllUpcoming,
        getFeaturedSessions: getFeaturedSessions
    });

})();
