/*

    This file has all of the controller logic for every view in the application.

*/


(function () {
  
    "use strict";

    var views = {};
    var isConnected = false;
    // many parts of the view contract are optional, this is just a default empty implementation.
    var doNothing = function () { };

    // gets called before any view is rendered.
    function beforeView() {
        isConnected = ConferenceApp.Util.isConnected();
    }

    // View objects have names, which are used by the navigation toolkit as descriptors.
    // They also have a title which is displayed at the top of the main template.
    // Hooks are a function that gets called after the template for the view is rendered.
    // beforeRender gets called on the view state before calling into the template.
    function createView(viewName, title, hooks, beforeRender) {
        var view = {};
        view.name = viewName;
        view.title = title;
        // templates are chosen via naming convention with the view name.
        view.template = '/html/' + viewName + '.html';
        view.hooks = hooks || doNothing;
        view.checkpoint = doNothing;
        view.relayout = doNothing;
        view.share = doNothing;
        view.playTo = doNothing;
        view.pending = [];
        view.beforeRender = beforeRender || doNothing;
        views[viewName] = view;
    }

    // This takes a date and returns an object useful for templating.
    function formatTime(date) {
        if (!date) {
            return null;
        }
        var mins = date.getMinutes();
        var ampm = 'AM';
        if(mins < 10) {
            mins = '0' + mins;
        }
        var hours = date.getHours();
        if (hours === 0) {
            hours = '12';
        }
        else if (hours >= 12) {
            ampm = 'PM';
            if (hours > 12) {
                hours = hours % 12;
            }
        }

        var time = hours + ':' + mins + ampm;
        var day = formatDay(date);
        var shortDay = formatShortDay(date); 
        var result = {
            hours: hours,
            minutes: mins,
            ampm: ampm,
            day: day,
            shortDay: shortDay,
            toString: function () { return time; },
            longString: function () { return day + ", " + time; }
        };
        return result;
    }

    var weekdayMap = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

    // Return a friendly description of the day of the week
    function formatDay(date) {
        if (date) {
            return weekdayMap[date.getDay()];
        }
        else {
            return null;
        }
    }

    // Return the first three characters of the day name.
    function formatShortDay(date) {
        return formatDay(date).substr(0, 3);
    }

    // Template helper to coerce an OData object's StartTime into something more templatable.
    function showStartTime() {
        return formatTime(this.item.StartTime);
    }

    // Template helper to coerce an OData object's EndTime into something more templatable.
    // Technically events don't have an EndTime, so we fake one using StartTime + Duration
    function showEndTime() {
        if (this.item.StartTime) {
            var duration = parseInt(this.item.Duration);
            var endTime = new Date(this.item.StartTime);
            endTime.setMinutes(this.item.StartTime.getMinutes() + duration);
            return formatTime(endTime);
        }
        return null;
        
    }

    // Format a speaker array into a comma separated string of their names
    function formatSpeakers(speakers) {
        return speakers.reduce(function(prev, current) {
            if(prev.length > 0) {
                prev += ', ';
            }
            return prev + current.FirstName + ' ' + current.LastName;
        },'');
    }

    // used to render ListView items of people
    function personItemRenderer(item) {
        msWriteProfilerMark("StartPersonItemRender");
        var itemOptions = {
            name: item.data.Name,
            company: item.data.Company,
            favorite: isPersonFavorite,
            item: item.data
        };
        if (isConnected) {
            itemOptions.portrait_uri = item.data.ImageUrl;
        }
        if (item.data.Type === 'Microsoft') {
            itemOptions.microsoft = true;
        }

        var template = $("<div>").html($.template('/html/peopleItem.html', itemOptions));
        template.find(".favoriteButton").listen('change', personFavoriteChangeHandler);
        msWriteProfilerMark("EndPersonItemRender");
        return template[0].firstElementChild;
        
    }

    // used to render ListView items of sessions.
    function sessionItemRenderer(item) {
        var currentItem = item.data;
        if ('item' in currentItem) {
            currentItem = currentItem.item;
        }

        var itemOptions = {
            startTime: formatTime(currentItem.StartTime),
            endTime: showEndTime,
            title: currentItem.Title,
            speaker: formatSpeakers(currentItem.Speakers.results),
            sessionCode: currentItem.SessionCode,
            room: currentItem.Room,
            favorite: isFavorite,
            item: currentItem,
            color: ConferenceApp.Api.getCategoryColor('session', currentItem.Track, false)
        };

        var template = $("<div>").html($.template('/html/sessionItem.html', itemOptions));
        template.find(".favoriteButton").listen('change', favoriteChangeHandler);
        return template[0].firstElementChild;
    }

    // used to render ListView items of events in the context of the Schedule view.
    function scheduleItemRenderer(item) {
        if ('SessionCode' in item.data) {
            return scheduleSessionItemRenderer(item);
        }
        else {
            return scheduleAgendaItemRenderer(item);
        }
    }

    // used to render ListView items of sessions in the context of the Schedule view.
    function scheduleSessionItemRenderer(item) {
        var itemOptions = {
            startTime: formatTime(item.data.StartTime),
            endTime: showEndTime,
            title: item.data.Title,
            speaker: formatSpeakers(item.data.Speakers.results),
            sessionCode: item.data.SessionCode,
            room: item.data.Room,
            favorite: isFavorite,
            item: item.data,
            color: ConferenceApp.Api.getCategoryColor('session', item.data.Track, true)
        };
        var template = $("<div>").html($.template('/html/scheduleItem.html', itemOptions));
        template.find(".favoriteButton").listen('change', favoriteChangeHandler);
        return template[0].firstElementChild;
    }

    // used to render ListView items of agenda items in the context of the Schedule view.
    function scheduleAgendaItemRenderer(item) {
        var itemOptions = {
            startTime: formatTime(item.data.StartTime),
            endTime: formatTime(item.data.EndTime),
            title: item.data.Title,
            location: item.data.Location,
            item: item.data,
            color: 'slashColorNeutral'
        };
        var template = $("<div>").html($.template('/html/agendaItem.html', itemOptions));
        return template[0].firstElementChild;
    }

    // A default group header renderer for the ListView.
    function groupRenderer(item) {
        return $("<div>").text(item.data.title).addClass('groupHeader')[0];
    }

    // Template helper, true if the session is a favorite.
    function isFavorite() {
        return ConferenceApp.Api.isSessionFavorite(this.sessionCode);
    }

    // Template helper, true if the attendee is a favorite.
    function isPersonFavorite() {
        return ConferenceApp.Api.isAttendeeFavorite(this.item.AttendeeId);
    }

    // Template helper, returns human readable text for accessibility about the favorite state.
    function favoriteText() {
        if (ConferenceApp.Api.isSessionFavorite(this.sessionCode)) {
            return 'This session is favorited.';
        }
        else {
            return 'This session is not favorited.';
        }
    }

    // Template helper, persists favorite actions on sessions.
    function favoriteChangeHandler(evt) {
        var sessionCode = $(this.parentNode).attr('data-sessionCode');
        if (this.checked) {
            ConferenceApp.Api.createSessionFavorite(sessionCode);
        }
        else {
            ConferenceApp.Api.deleteSessionFavorite(sessionCode);
        }
    }

    // Template helper, persists favorite actions on people.
    function personFavoriteChangeHandler(evt) {
        var attendeeId = $(this.parentNode).attr('data-attendeeid');
        if (this.checked) {
            ConferenceApp.Api.createAttendeeFavorite(attendeeId);
        }
        else {
            ConferenceApp.Api.deleteAttendeeFavorite(attendeeId);
        }
    }

    function initScheduleView(state) {

        function groupByKey(item) {
            return item.StartTime.getTime().toString();
        }

        function groupByData(item) {
            var date = item.StartTime;
            var time = formatTime(date);

            return {
                title: time.longString(),
                date: date,
                day: time.day,
                shortDay: time.shortDay,
                time: time
            };
        }


        // "Super Group By" is used because the groups of schedule items (day/time) are themselves grouped into days when you semantically zoom out.
        function superGroupByKey(item) {
            return item.date.getDay();
        }

        function superGroupByData(item) {
            return { title: item.day };
        }

        function zoomedRenderer(item) {
            var template = $("<div>").html($.template('/html/scheduleZoomItem.html', { time: item.data.time, day: item.data.shortDay }));
            return template[0].firstElementChild;
        }

        function updateDataSource() {
            filteredData = getFilteredData();
            grouped = (new WinJS.Binding.List(filteredData)).createGrouped(groupByKey, groupByData);
            superGrouped = grouped.groups.createGrouped(superGroupByKey, superGroupByData);

            listView.itemDataSource = grouped.dataSource;
            listView.groupDataSource = grouped.groups.dataSource;
            listView2.itemDataSource = superGrouped.dataSource;
            listView2.groupDataSource = superGrouped.groups.dataSource;
        }

        function getFilteredData() {
            var data = sortedData;
            if (state.day && state.day !== 'all') {
                data = data.filter(function (item) {
                    return (formatDay(item.StartTime) === state.day);
                });
            }
            if (state.favorite === 'my') {
                data = data.filter(function (item) {
                    var favorite = ConferenceApp.Api.isSessionFavorite(item.SessionCode);
                    var isAgendaItem = 'AgendaItemId' in item;
                    return favorite || isAgendaItem;
                });
            }
            return data;
        }

        function onFavoriteSelect() {
            var element = this;
            var value = element.getAttribute('data-value');
            state.favorite = value;
            $('#favoriteSelect .listOption').removeClass('listOptionActive');
            $('#favoriteSelect .listOption[data-value="' + value + '"]').addClass('listOptionActive');
            updateDataSource();

        }

        $('#daySelect').listen("change", function (evt) {
            var value = evt.target[evt.target.selectedIndex].value;
            state.day = value;
            state.dayIndex = evt.target.selectedIndex;
            updateDataSource();
        });

        var sortedData = ConferenceApp.Api.getScheduleItems().sortBy("SessionCode").sortBy("StartTime");
        var days = {};
        sortedData.forEach(function (item) {
            days[formatDay(item.StartTime)] = true;
        });
        Object.keys(days).forEach(function (item) {
            var dayName = item;
            $('<option>', { 'value': item }).text(item).appendTo('#daySelect');
        });

        ConferenceApp.Util.clickAndEnter('#favoriteSelect .listOption', onFavoriteSelect);
        ConferenceApp.Util.enablePressFeedback('#favoriteSelect .listOption');

        if (state.favorite) {
            $('#favoriteSelect .listOption').removeClass('listOptionActive');
            $('#favoriteSelect .listOption[data-value="' + state.favorite + '"]').addClass('listOptionActive');
        }
        if (state.dayIndex) {
            $('#daySelect')[0].selectedIndex = state.dayIndex;
        }

        var filteredData = getFilteredData();
        var grouped = (new WinJS.Binding.List(filteredData)).createGrouped(groupByKey, groupByData);
        var superGrouped = grouped.groups.createGrouped(superGroupByKey, superGroupByData);

        function itemInvoked(e) {
            var item = filteredData[e.detail.itemIndex];
            if (item.SessionCode) {
                checkpoint();
                ConferenceApp.Navigation.navigate('sessionDetail', { item: item });
            }
            else if (item.Link) {
                ConferenceApp.Util.launchUrl(item.Link);
            }
        }

        function checkpoint() {
            if (semanticZoom && !semanticZoom.zoomedOut && listView.indexOfFirstVisible >= 0) {
                state.listPosition = listView.indexOfFirstVisible;
            }
        }
        this.checkpoint = checkpoint;

        var layout = new WinJS.UI.GridLayout();
        var layout2 = new WinJS.UI.GridLayout();
        if (ConferenceApp.Util.isSnapped()) {
            layout = new WinJS.UI.ListLayout();
            layout2 = new WinJS.UI.ListLayout();
        }

        var listView = new WinJS.UI.ListView($('#detailListView')[0], { 
            itemDataSource: grouped.dataSource,
            oniteminvoked: itemInvoked,
            itemTemplate: WinJS.UI.simpleItemRenderer(scheduleItemRenderer),
            groupHeaderTemplate: WinJS.UI.simpleItemRenderer(groupRenderer),
            groupDataSource: grouped.groups.dataSource,
            layout: layout,
            selectionMode: 'none'
        });

        if (state.listPosition) {
            listView.indexOfFirstVisible = state.listPosition;
        }

        var listView2 = new WinJS.UI.ListView($('#jumpListView')[0], { 
            itemDataSource: superGrouped.dataSource,
            itemTemplate: WinJS.UI.simpleItemRenderer(zoomedRenderer),
            groupHeaderTemplate: WinJS.UI.simpleItemRenderer(groupRenderer),
            groupDataSource: superGrouped.groups.dataSource,
            layout: layout2,
            selectionMode: 'none'
        });

        var semanticZoom = new WinJS.UI.SemanticZoom($('#semanticZoom')[0]);

        this.relayout = function (newLayout) {
            if (newLayout === 'snapped') {
                listView.layout = new WinJS.UI.ListLayout();
                listView2.layout = new WinJS.UI.ListLayout();
                    
            }
            else {
                listView.layout = new WinJS.UI.GridLayout();
                listView2.layout = new WinJS.UI.GridLayout();
            }
            semanticZoom.forceLayout();
            listView.forceLayout();
            listView2.forceLayout();
        };


    }

    function beforeRenderSessionsDetailView(state) {
        if (state) {
            state.item = ConferenceApp.Db.all('sessions').filterBy('SessionCode', state.item.SessionCode)[0] || state.item;
            state.item.day = formatDay(state.item.StartTime);
            state.item.speakers = state.item.Speakers.results;
            state.item.startTime = showStartTime;
            state.item.endTime = showEndTime;
            state.roomStatus = ConferenceApp.Api.getRoomStatus(state.item.Room);
            state.color = ConferenceApp.Api.getCategoryColor('session', state.item.Track, true);
            state.notes = ConferenceApp.Api.getNotesForSession(state.item.SessionCode);
            state.video = ConferenceApp.Api.getSessionVideo(state.item.Files.results);
            state.slides = ConferenceApp.Api.getSessionSlides(state.item.Files.results);
            var sessions = ConferenceApp.Api.getRelatedSessions(state.item.SessionRelateds.results);
            state.relatedLinks = ConferenceApp.Api.getRelatedLinks(state.item.SessionLinks.results);
            state.relatedSessions = [];
            sessions.forEach(function (session) {
                var data = {
                    title: session.Title,
                    color: ConferenceApp.Api.getCategoryColor('session', session.Track, false),
                    favorite: ConferenceApp.Api.isSessionFavorite(session.SessionCode),
                    sessionCode: session.SessionCode
                };
                state.relatedSessions.push($.template('/html/sessionItemCollapsed.html', data));
            });
        }
        return;
    }

    function initSessionsDetailView(state) {

        if(!state.item) {
            ConferenceApp.Navigation.goBackOrGoHome();
        }

        this.share = function () {
            var data = new Windows.ApplicationModel.DataTransfer.DataPackage();
            var text = "#bldwin #" + state.item.SessionCode + " ";
            if (state.item.ShortURL) {
                text += state.item.ShortURL;
            }
            data.properties.title = state.item.Title;
            data.properties.description = state.item.Abstract;
            data.setText(text);

            return data;
        };

        $('.collapsedSessionItem').find('.favoriteButton').listen('change', favoriteChangeHandler);
        ConferenceApp.Util.enablePressFeedback('.collapsedSessionItem');
        ConferenceApp.Util.clickAndEnter('.collapsedSessionItem .title', function (evt) {
            var session = ConferenceApp.Api.getSession($(this.parentNode).attr('data-sessionCode'));
            if (session) {
                ConferenceApp.Navigation.navigate('sessionDetail', { item: session });
            }
        });

        $('#pageTitle').text(state.item.Title);

        ConferenceApp.Util.enablePressFeedback('.sessionSpeaker');
        ConferenceApp.Util.clickAndEnter('.sessionSpeaker', function () {
            var attendeeId = this.getAttribute('data-attendeeid');
            if (attendeeId) {
                attendeeId = parseInt(attendeeId, 10);
                var person = ConferenceApp.Api.getAttendee(attendeeId);
                if (person) {
                    ConferenceApp.Navigation.navigate('peopleDetail', { item: person });
                }
            }
        });

        if (state.video) {
            $('#sessionVideoButton').click(function () {
                ConferenceApp.Navigation.navigate('videoPlayer', { video: state.video, title: state.item.Title });
            });
        }

        if (state.item.Room) {
            $('#sessionMapButton').click(function () {
                ConferenceApp.Navigation.navigate('map', { selectedSession: state.item });
            });
        }

        function navigateToNotes() {
            ConferenceApp.Navigation.navigate('sessionNotes', { item: state.item });
        }
        $('#addNotes').click(navigateToNotes);

        ConferenceApp.Util.clickAndEnter('#notesText', navigateToNotes);
        
        var attrs = {
            'tabindex': 1,
            'type': 'checkbox',
            'aria-label': 'Favorite this session',
            'class': 'favoriteTitleButton'
        };
        if (ConferenceApp.Api.isSessionFavorite(state.item.SessionCode)) {
            attrs['checked'] = true;
        }
        $("<input>", attrs).appendTo('#pageTitle').listen('change', function(evt) {
            if (this.checked) {
                ConferenceApp.Api.createSessionFavorite(state.item.SessionCode);
            }
            else {
                ConferenceApp.Api.deleteSessionFavorite(state.item.SessionCode);
            }
        });
    }

    function initSessionsView(state) {

        var groupByKey = trackGroupByKey;
        var groupByData = trackGroupByData;

        function trackGroupByKey(item) {
            return item.Track;
        }

        function trackGroupByData(item) {
            return { title: item.Track };
        }

        function tagGroupByKey(item) {
            return item.tag;
        }

        function tagGroupByData(item) {
            return { title: item.tag };
        }

        function zoomedRenderer(item) {
            var container = $("<div>").addClass('zoomItem');
            $("<div>").text(item.data.title).addClass('zoomContent').appendTo(container);
            return container[0];
        }

        $('#groupingSelect')[0].addEventListener("change", function(evt) {
            var value = evt.target[evt.target.selectedIndex].value;
            state.grouping = value;
            state.groupingIndex = evt.target.selectedIndex;
            updateDataSource();
        });

        function updateDataSource() {
            filteredData = getFilteredData();
            grouped = (new WinJS.Binding.List(filteredData)).createGrouped(groupByKey, groupByData);

            listView.itemDataSource = grouped.dataSource;
            listView.groupDataSource = grouped.groups.dataSource;
            listView2.itemDataSource = grouped.groups.dataSource;
        }

        function onFavoriteSelect() {
            var element = this;
            var value = element.getAttribute('data-value');
            state.filter = value;
            $('#favoriteSelect .listOption').removeClass('listOptionActive');
            $('#favoriteSelect .listOption[data-value="' + value + '"]').addClass('listOptionActive');
            updateDataSource();
        }

        ConferenceApp.Util.clickAndEnter('#favoriteSelect .listOption', onFavoriteSelect);
        ConferenceApp.Util.enablePressFeedback('#favoriteSelect .listOption');
        
        var sortedData = ConferenceApp.Api.getSessions().sortBy("Track");

        var trackList = sortedData.groupBy("Track");
        Object.keys(trackList).forEach(function(item) {
            $('<option>', { value: item }).text(item).appendTo('#trackSelect');
        });

        function getFilteredData() {
            var data = sortedData;
            var filter = state.filter || 'all';
            var grouping = state.grouping || 'track';

            if(filter === 'favorite') {
                data = sortedData.filter(function (item) {
                    return ConferenceApp.Api.isSessionFavorite(item.SessionCode);
                });
            }

            if (grouping === 'tag') {
                var expandedData = ConferenceApp.Db.extendCollection([]);
                data.forEach(function (item) {
                    item.Tags.results.forEach(function (tag) {
                        expandedData.push({ tag: tag.TagValue, item: item });
                    });
                });
                expandedData.sortBy('tag');
                data = expandedData;
                groupByKey = tagGroupByKey;
                groupByData = tagGroupByData;
            }
            else {
                groupByKey = trackGroupByKey;
                groupByData = trackGroupByData;
            }

            return data;
        }

            
        var filteredData = getFilteredData();
        if(state.filter) {
            $('#favoriteSelect .listOption').removeClass('listOptionActive');
            $('#favoriteSelect .listOption[data-value="' + state.filter + '"]').addClass('listOptionActive');
        }
        if (state.groupingIndex) {
            $('#groupingSelect')[0].selectedIndex = state.groupingIndex;
        }
        var grouped = (new WinJS.Binding.List(filteredData)).createGrouped(groupByKey, groupByData);

        function itemInvoked(e) {
            var item = filteredData[e.detail.itemIndex];
            // when we're grouped by tag there is an extra wrapper.
            if ('item' in item) {
                item = item.item;
            }
            checkpoint();
            ConferenceApp.Navigation.navigate('sessionDetail', { item: item });
        }

        function checkpoint() {
            if (semanticZoom && !semanticZoom.zoomedOut && listView.indexOfFirstVisible >= 0) {
                state.listPosition = listView.indexOfFirstVisible;
            }
        }
        this.checkpoint = checkpoint;

        var layout = new WinJS.UI.GridLayout();
        var layout2 = new WinJS.UI.GridLayout();
        if (ConferenceApp.Util.isSnapped()) {
            layout = new WinJS.UI.ListLayout();
            layout2 = new WinJS.UI.ListLayout();
        }

        var listView = new WinJS.UI.ListView($('#detailListView')[0], { 
            itemDataSource: grouped.dataSource,
            oniteminvoked: itemInvoked,
            itemTemplate: WinJS.UI.simpleItemRenderer(sessionItemRenderer),
            groupHeaderTemplate: WinJS.UI.simpleItemRenderer(groupRenderer),
            groupDataSource: grouped.groups.dataSource,
            layout: layout,
            selectionMode: 'none'
        });


        if (state.listPosition) {
            listView.indexOfFirstVisible = state.listPosition;
        }

        var listView2 = new WinJS.UI.ListView($('#jumpListView')[0], { 
            itemDataSource: grouped.groups.dataSource,
            itemTemplate: WinJS.UI.simpleItemRenderer(zoomedRenderer),
            layout: layout2,
            selectionMode: 'none'
        });


        var semanticZoom = new WinJS.UI.SemanticZoom($('#semanticZoom')[0]);

        this.relayout = function (newLayout) {
            if (newLayout === 'snapped') {
                listView.layout = new WinJS.UI.ListLayout();
                listView2.layout = new WinJS.UI.ListLayout();
                    
            }
            else {
                listView.layout = new WinJS.UI.GridLayout();
                listView2.layout = new WinJS.UI.GridLayout();
            }
            semanticZoom.forceLayout();
            listView.forceLayout();
            listView2.forceLayout();
        };

    }

    function initPeopleView(state) {

        function groupByKey(item) {
            return item.Name[0].toUpperCase();
        }

        function groupByData(item) {
            return { title: item.Name[0].toUpperCase() };
        }

        function zoomedRenderer(item) {
            var container = $("<div>").addClass('zoomItem');
            $("<div>").text(item.data.title).addClass('zoomContent').appendTo(container);
            return container[0];
        }

        function updateDataSource() {
            filteredData = getFilteredData();

            grouped = (new WinJS.Binding.List(filteredData)).createGrouped(groupByKey, groupByData);

            listView.itemDataSource = grouped.dataSource;
            listView.groupDataSource = grouped.groups.dataSource;
            listView2.itemDataSource = grouped.groups.dataSource;
        }

        function getFilteredData() {
            var data = sortedData;
            if (state.favorite === 'favorite') {
                data = data.filter(function (item) {
                    return ConferenceApp.Api.isAttendeeFavorite(item.AttendeeId);
                });
            }
            else if (state.favorite === 'microsoft') {
                data = data.filterBy('Type', 'Microsoft');
            }
            return data;
        }

        function onFavoriteSelect() {
            var element = this;
            var value = element.getAttribute('data-value');
            state.favorite = value;
            $('#favoriteSelect .listOption').removeClass('listOptionActive');
            $('#favoriteSelect .listOption[data-value="' + value + '"]').addClass('listOptionActive');
            updateDataSource();

        }

        ConferenceApp.Util.clickAndEnter('#favoriteSelect .listOption', onFavoriteSelect);
        ConferenceApp.Util.enablePressFeedback('#favoriteSelect .listOption');

        var sortedData = ConferenceApp.Api.getAttendees().sortBy("Name");

        var filteredData = getFilteredData();
        if (state.favorite) {
            $('#favoriteSelect .listOption').removeClass('listOptionActive');
            $('#favoriteSelect .listOption[data-value="' + state.favorite + '"]').addClass('listOptionActive');
        }
        
        var grouped = (new WinJS.Binding.List(filteredData)).createGrouped(groupByKey, groupByData);

        function itemInvoked(e) {
            var item = filteredData[e.detail.itemIndex];
            checkpoint();
            ConferenceApp.Navigation.navigate('peopleDetail', { item: item });
        }

        function checkpoint() {
            if (semanticZoom && !semanticZoom.zoomedOut && listView.indexOfFirstVisible >= 0) {
                state.listPosition = listView.indexOfFirstVisible;
            }
        }
        this.checkpoint = checkpoint;

        var layout = new WinJS.UI.GridLayout();
        var layout2 = new WinJS.UI.GridLayout();
        if (ConferenceApp.Util.isSnapped()) {
            layout = new WinJS.UI.ListLayout();
            layout2 = new WinJS.UI.ListLayout();
        }

        var listView = new WinJS.UI.ListView($('#detailListView')[0], { 
            itemDataSource: grouped.dataSource,
            oniteminvoked: itemInvoked,
            itemTemplate: WinJS.UI.simpleItemRenderer(personItemRenderer),
            groupHeaderTemplate: WinJS.UI.simpleItemRenderer(groupRenderer),
            groupDataSource: grouped.groups.dataSource,
            layout: layout,
            selectionMode: 'none'
        });


        if (state.listPosition) {
            listView.indexOfFirstVisible = state.listPosition;
        }

        var listView2 = new WinJS.UI.ListView($('#jumpListView')[0], { 
            itemDataSource: grouped.groups.dataSource,
            itemTemplate: WinJS.UI.simpleItemRenderer(zoomedRenderer),
            selectionMode: 'none',
            layout: layout2
        });


        var semanticZoom = new WinJS.UI.SemanticZoom($('#semanticZoom')[0]);

        this.relayout = function (newLayout) {
            if (newLayout === 'snapped') {
                listView.layout = new WinJS.UI.ListLayout();
                listView2.layout = new WinJS.UI.ListLayout();
                    
            }
            else {
                listView.layout = new WinJS.UI.GridLayout();
                listView2.layout = new WinJS.UI.GridLayout();
            }
            semanticZoom.forceLayout();
            listView.forceLayout();
            listView2.forceLayout();
        };

        

    }

    function beforeRenderPeopleDetailView(state) {
        if (state.item) {
            state.item = ConferenceApp.Db.all('attendees').filterBy('AttendeeId', state.item.AttendeeId)[0] || state.item;
            state.companyUrl = ConferenceApp.Util.ensureUrl(state.item.CompanyUrl);
            state.blogUrl = ConferenceApp.Util.ensureUrl(state.item.BlogUrl);
            if (ConferenceApp.Util.isConnected()) {
                state.imageUrl = state.item.ImageUrl;
            }
            if(state.item.Type === 'Microsoft') {
                state.microsoft = true;
            }

            //filter out empty string
            var tech = state.item.Technologies.split(',').filter(function (item) { return item; });
            var lang = state.item.Languages.split(',').filter(function (item) { return item; });
            state.interestTags = tech.concat(lang);
            if (state.item.Interests || tech.length || lang.length) {
                state.hasInterests = true;
            }
            state.facebook = ConferenceApp.Util.ensureUrl(state.item.FacebookUrl);
            if (state.item.TwitterHandle) {
                state.twitter = "http://twitter.com/" + state.item.TwitterHandle;
            }
            if (state.item.Networking) {
                state.talkAbout = state.item.Networking.split(",");
                state.hasTalkAbout = true;
            }
            state.linkedIn = ConferenceApp.Util.ensureUrl(state.item.LinkedInUrl);
            state.speaker = ConferenceApp.Api.getSpeaker(state.item.AttendeeId);
        }
        
        if (state.speaker) {
            var sessions = ConferenceApp.Api.getSpeakerSessions(state.speaker);
            if (ConferenceApp.Util.isConnected() && state.speaker.PhotoUri) {
                state.imageUrl = state.speaker.PhotoUri;
            }
            if (sessions.length > 0) {
                state.hasSessions = true;
            }
            else {
                state.hasSessions = false;
            }
            state.sessions = [];
            sessions.forEach(function (session) {
                var data = {
                    title: session.Title,
                    color: ConferenceApp.Api.getCategoryColor('session', session.Track, false),
                    favorite: ConferenceApp.Api.isSessionFavorite(session.SessionCode),
                    sessionCode: session.SessionCode
                };
                state.sessions.push($.template('/html/sessionItemCollapsed.html', data));
            });
            state.content = $.template('/html/speakerDetail.html', state);
        }
        else {
            state.content = $.template('/html/attendeeDetail.html', state);
        }
    }

    function initPeopleDetailView(state) {
        $('#pageTitle').text(state.item.Name);

        if (state.speaker) {
            ConferenceApp.Util.enablePressFeedback('.collapsedSessionItem');
            $('.collapsedSessionItem').find('.favoriteButton').listen('change', favoriteChangeHandler);
            ConferenceApp.Util.clickAndEnter('.collapsedSessionItem .title', function (evt) {
                var session = ConferenceApp.Api.getSession($(this.parentNode).attr('data-sessionCode'));
                if (session) {
                    ConferenceApp.Navigation.navigate('sessionDetail', { item: session });
                }
            });
        }

        var attrs = {
            'tabindex': 0,
            'type': 'checkbox',
            'aria-label': 'Favorite this person',
            'class': 'favoriteTitleButton'
        };
        if (ConferenceApp.Api.isAttendeeFavorite(state.item.AttendeeId)) {
            attrs['checked'] = true;
        }
        $("<input>", attrs).appendTo('#pageTitle').listen('change', function (evt) {
            if (this.checked) {
                ConferenceApp.Api.createAttendeeFavorite(state.item.AttendeeId);
            }
            else {
                ConferenceApp.Api.deleteAttendeeFavorite(state.item.AttendeeId);
            }
        });
    }

    function initSearchResultsView(state) {

        function itemRenderer(item) {
            if (item.data.type === 'session') {
                return sessionItemRenderer({ data: item.data.item });
            }
            else if (item.data.type === 'person') {
                return personItemRenderer({ data: item.data.item });
            }
            else {
                throw new Error("Unexpected item type");
            }
        }

        function updateDataSource() {
            filteredData = getFilteredData();
            listView.itemDataSource = new WinJS.Binding.List(filteredData).dataSource;
        }

        function onFilterSelect() {
            var element = this;
            var value = element.getAttribute('data-value');
            state.filter = value;
            $('#filterSelect .listOption').removeClass('listOptionActive');
            $('#filterSelect .listOption[data-value="' + value + '"]').addClass('listOptionActive');
            updateDataSource();
        }

        function getFilteredData() {
            var data = results;
            var filter = state.filter || 'all';

            if (filter === 'sessions') {
                data = filteredData.filterBy('type', 'session');
            }
            else if (filter === 'people') {
                data = filteredData.filterBy('type', 'person');
            }

            return data;
        }

        ConferenceApp.Util.clickAndEnter('#filterSelect .listOption', onFilterSelect);
        ConferenceApp.Util.enablePressFeedback('#filterSelect .listOption');
        
        var results = ConferenceApp.Api.search(state.query);
        $('#userQuery').text(state.query);
        $('#allCount').text(results.length);
        $('#sessionCount').text(results.sessionCount);
        $('#personCount').text(results.personCount);

        var filteredData = getFilteredData();
        if (state.filter) {
            $('#filterSelect .listOption').removeClass('listOptionActive');
            $('#filterSelect .listOption[data-value="' + state.filter + '"]').addClass('listOptionActive');
        }

        function itemInvoked(e) {
            var item = filteredData[e.detail.itemIndex];

            checkpoint();
            
            if (item.type === 'session') {
                ConferenceApp.Navigation.navigate('sessionDetail', { item: item.item });
            }
            else if (item.type === 'person') {
                ConferenceApp.Navigation.navigate('peopleDetail', { item: item.item });
            }
        }

        function checkpoint() {
            if (listView.indexOfFirstVisible >= 0) {
                state.listPosition = listView.indexOfFirstVisible;
            }
        }
        this.checkpoint = checkpoint;

        var layout = new WinJS.UI.GridLayout();
        if (ConferenceApp.Util.isSnapped()) {
            layout = new WinJS.UI.ListLayout();
        }

        var listView = new WinJS.UI.ListView($('#searchResults')[0], {
            itemDataSource: new WinJS.Binding.List(filteredData).dataSource,
            oniteminvoked: itemInvoked,
            itemTemplate: WinJS.UI.simpleItemRenderer(itemRenderer),
            layout: layout,
            selectionMode: 'none'
        });


        if (state.listPosition) {
            listView.indexOfFirstVisible = state.listPosition;
        }

        this.relayout = function (newLayout) {
            if (newLayout === 'snapped') {
                listView.layout = new WinJS.UI.ListLayout();
            }
            else {
                listView.layout = new WinJS.UI.GridLayout();
            }

            listView.forceLayout();
        };
        
    }

    function initHomeView(state) {

        $('#pageTitle').text('');
        $('<img>', { src: '/images/build-logo.png', alt: 'Home' }).appendTo('#pageTitle');


        var featuredSessions = ConferenceApp.Api.getFeaturedSessions();

        function checkpoint() {
            if (featuredListView.indexOfFirstVisible >= 0) {
                state.featuredPosition = featuredListView.indexOfFirstVisible;
            }
        }
        this.checkpoint = checkpoint;
        

        function featuredItemRenderer(item) {
            var currentItem = item.data;
            if ('item' in currentItem) {
                currentItem = currentItem.item;
            }

            var itemOptions = {
                title: currentItem.Title,
                sessionCode: currentItem.SessionCode,
                item: currentItem
            };

            var template = $("<div>").html($.template('/html/homeItem.html', itemOptions));
            return template[0].firstElementChild;
        }

        function featuredItemInvoked(e) {
            var item = featuredSessions[e.detail.itemIndex];
            checkpoint();
            ConferenceApp.Navigation.navigate('sessionDetail', { item: item });
        }

        var layout = new WinJS.UI.GridLayout();
        if (ConferenceApp.Util.isSnapped()) {
            layout = new WinJS.UI.ListLayout();
        }

        var featuredListView = new WinJS.UI.ListView($('#featuredListView')[0], {
            itemDataSource: new WinJS.Binding.List(featuredSessions).dataSource,
            oniteminvoked: featuredItemInvoked,
            itemTemplate: WinJS.UI.simpleItemRenderer(featuredItemRenderer),
            layout: layout,
            selectionMode: 'none'
        });


        if (state.featuredPosition) {
            featuredListView.indexOfFirstVisible = state.featuredPosition;
        }


        this.relayout = function (newLayout) {
            if (newLayout === 'snapped') {
                featuredListView.layout = new WinJS.UI.ListLayout();
            }
            else {
                featuredListView.layout = new WinJS.UI.GridLayout();
            }
            featuredListView.forceLayout();
        };


        function startNewsTicker() {
            var active = true;
            var lastItemIndex = -1;
            var news;
            function setupNewsItem() {
                if (active) {
                    news = ConferenceApp.Api.getNews(true).sortBy("PublishTime");
                    if (news.length === 0) {
                        setTimeout(setupNewsItem, 10000);
                        return;
                    }
                    lastItemIndex = (lastItemIndex + 1) % news.length;
                    var nextItem = news[lastItemIndex];
                
                    $(".newsItem").remove();
                    $("<span>", { "class": "newsItem newsItemNew" }).text(nextItem.TickerText).appendTo("#newsContent");
                    setTimeout(showNewsItem, 1000);
                }
            }
            function showNewsItem() {
                if(active) {
                    $(".newsItem").removeClass("newsItemNew");
                    setTimeout(hideNewsItem, 6000);
                }
            }

            function hideNewsItem() {
                if(active) {
                    $(".newsItem").addClass("newsItemOld");
                    setTimeout(setupNewsItem, 2000);
                }
            }

            setTimeout(setupNewsItem, 1000);

            return new WinJS.Promise(function () { }, function () { active = false; });
        }

        this.pending.push(startNewsTicker());

    }

    function initVideoPlayerView(state) {
        if (!state.video) {
            ConferenceApp.Navigation.goBackOrGoHome();
        }
        var videoS = $('#sessionVideo');
        var video = videoS[0];

        // auto play on load and clean up when we navigate away
        video.play();
        this.pending.push(new WinJS.Promise(function(){}, function() {
            video.removeAttribute("src");
        }));
        
        videoS.listen('loadedmetadata', function () {
            if (state.videoTime) {
                video.currentTime = state.videoTime;
            }
        });
        
        videoS.click(function () {
            if (video.paused) {
                video.play();
            }
            else {
                video.pause();
            }
        });

        this.checkpoint = function () {
            if (video) {
                state.videoTime = video.currentTime;
            }
        };

        this.playTo = function () {
            if (video) {
                return video.msPlayToSource;
            }
            return null;
        };

        $('#pageTitle').text(state.title);
        $('#outerPage').addClass('darkTitle');
        $('#pageTitle').addClass('hideTextOverflow');
    }

    function beforeRenderSessionNotesView(state) {
        if (state.item) {
            state.notes = ConferenceApp.Api.getNotesForSession(state.item.SessionCode);
        }
    }

    function initSessionNotesView(state) {
        var lastRect;
        function resizeText(occludedRect) {
            lastRect = occludedRect;
            var pad = 120;
            if (ConferenceApp.Util.isSnapped()) {
                pad = 20;
            }
            else if (occludedRect > 0) {
                pad = 30;
            }
            $('#sessionNotes')[0].style.paddingBottom = (occludedRect.height + pad) + "px";
        }

        var inputPane = Windows.UI.ViewManagement.InputPane.getForCurrentView();
        resizeText(inputPane.occludedRect);

        function onResize(e) {
            resizeText(e.occludedRect);
        }

        this.relayout = function () {
            resizeText(lastRect);
        };

        inputPane.addEventListener("showing", onResize);
        inputPane.addEventListener("hiding", onResize);

        this.pending.push(new WinJS.Promise(function(){}, function() {
            inputPane.removeEventListener("showing", onResize);
            inputPane.removeEventListener("hiding", onResize);
        }));

        $('#sessionNotesText')[0].value = state.notes;
        $('#sessionNotesText')[0].focus();
        $('#pageTitle').addClass('hideTextOverflow');
        $('#pageTitle').text(state.item.Title);

        

        this.checkpoint = function () {
            var note = $('#sessionNotesText')[0].value;
            state.notes = note;
            ConferenceApp.Api.setNotesForSession(state.item.SessionCode, note);
        };
    }

    function beforeRenderTripReportView(state) {
        state.notes = ConferenceApp.Api.getTripReportNotes();
    }

    function initTripReportView(state) {
        function saveTripReport() {
            var picker = Windows.Storage.Pickers.FileSavePicker();
            picker.fileTypeChoices.insert("Text File", [".txt"]);
            picker.defaultFileExtension = ".txt";

            picker.pickSaveFileAsync().then(function (file) {
                return file.openAsync(Windows.Storage.FileAccessMode.readWrite);
            }).then(function (stream) {
                var outputStream = stream.getOutputStreamAt(0);
                var writer = new Windows.Storage.Streams.DataWriter(outputStream);
                var count = writer.writeString(state.notes);
                return writer.storeAsync().then(function () {
                    return outputStream.flushAsync();
                });
            });
        }
        $('#save').removeClass('hidden').click(saveTripReport);

        this.pending.push(new WinJS.Promise(function() {}, function() {
            var saveButton = $('#save').addClass('hidden')[0];
            if (saveButton) {
                saveButton.removeEventListener('click', saveTripReport, false);
            }
        }));
    }

    function beforeRenderMapView(state) {
        state.maps = ConferenceApp.Api.getMaps();
        if (state.selectedSession) {
            state.selectedSession = ConferenceApp.Db.all('sessions').filterBy('SessionCode', state.selectedSession.SessionCode)[0] || state.selectedSession;
        }
    }

    function initMapView(state) {

        var activeRoom;

        $("#close").click(function () {
            state.selectedSession = null;
            updateMap();
        });

        $("#moreButton").click(function () {
            ConferenceApp.Navigation.navigate('sessionDetail', { item: state.selectedSession });
        });

        function updateMap() {
            if (activeRoom) {
                activeRoom.setAttribute('fill', '#3D3F3D');
            }
            if (state.selectedSession) {
                $('#sessionInfo').removeClass('hidden');
                var roomStatus = ConferenceApp.Api.getRoomStatus(state.selectedSession.Room);
                if (roomStatus) {
                    $('#selectedRoomName').text(state.selectedSession.Room);
                    $('#selectedRoomStatus').text(roomStatus);
                }
                else {
                    $('#selectedRoomName').text(state.selectedSession.Room);
                }
                $('#selectedTitle').text(state.selectedSession.Title);
                var time = formatTime(state.selectedSession.StartTime);
                if (time) {
                    $('#selectedTime').text(time.longString());
                }
                for (var i = 1; i <= 4; i++) {
                    var level = 'level' + i;
                    var room = $('#' + level).find('#' + ConferenceApp.Util.makeMapId(state.selectedSession.Room));
                    if (room.length > 0) {
                        activeRoom = room[0];
                        room[0].setAttribute("fill", "#59CC0E");
                        state.floor = level;
                    }
                }
            }
            else {
                $('#sessionInfo').addClass('hidden');
            }
            state.floor = state.floor || "level1";
            $('#floorSelect .listOption').removeClass('listOptionActive');
            $('#floorSelect .listOption[data-value="' + state.floor + '"]').addClass('listOptionActive');
            $(".floorMap").addClass("hidden");
            $("#" + state.floor).removeClass("hidden");
            if (upcomingListView) {
                upcomingListView._onResize();
            }
        }

        function collapsedSessionItemRenderer(item) {
            var data = {
                title: item.data.Title,
                color: ConferenceApp.Api.getCategoryColor('session', item.data.Track, false),
                favorite: ConferenceApp.Api.isSessionFavorite(item.data.SessionCode),
                sessionCode: item.data.SessionCode
            };
            var template = $("<div>").html($.template('/html/sessionItemCollapsed.html', data));
            return template[0].firstElementChild;
        }

        function onFloorSelect() {
            var element = this;
            var value = element.getAttribute('data-value');
            state.floor = value;
            state.selectedSession = null;
            updateMap();
        }

        ConferenceApp.Util.clickAndEnter('#floorSelect .listOption', onFloorSelect);
        ConferenceApp.Util.enablePressFeedback('#floorSelect .listOption');

        updateMap();

        var upcomingData = ConferenceApp.Api.getUpcomingSessions();

        function checkpoint() {
            if (upcomingListView.indexOfFirstVisible >= 0) {
                state.upcomingListPosition = upcomingListView.indexOfFirstVisible;
            }
        }
        this.checkpoint = checkpoint;

        function upcomingItemInvoked(e) {
            state.selectedSession = upcomingData[e.detail.itemIndex];
            updateMap();
        }

        function upcomingItemRenderer(item) {
            var element = collapsedSessionItemRenderer(item);
            $(element).addClass('hideFavoriteButton');
            return element;
        }

        var layout = new WinJS.UI.GridLayout();
        if (ConferenceApp.Util.isSnapped()) {
            layout = new WinJS.UI.ListLayout();
        }

        var upcomingListView = new WinJS.UI.ListView($('#upcomingListView')[0], {
            itemDataSource: new WinJS.Binding.List(upcomingData).dataSource,
            oniteminvoked: upcomingItemInvoked,
            itemTemplate: WinJS.UI.simpleItemRenderer(upcomingItemRenderer),
            layout: layout,
            selectionMode: 'none'
        });


        if (state.upcomingListPosition) {
            upcomingListView.indexOfFirstVisible = state.upcomingListPosition;
        }

        this.relayout = function (newLayout) {
            if (newLayout === 'snapped') {
                upcomingListView.layout = new WinJS.UI.ListLayout();
            }
            else {
                upcomingListView.layout = new WinJS.UI.GridLayout();
            }
            upcomingListView._onResize();
        };

    }

    function initNewsView(state) {

        var newsData = ConferenceApp.Api.getNews().sortBy("PublishTime");

        function checkpoint() {
            if (newsListView.indexOfFirstVisible >= 0) {
                state.newsListPosition = newsListView.indexOfFirstVisible;
            }
        }

        this.checkpoint = checkpoint;

        function newsItemInvoked(e) {
            var item = newsData[e.detail.itemIndex];
            if (item.LinkUrl) {
                ConferenceApp.Util.launchUrl(item.LinkUrl);
            }
        }

        function newsItemRenderer(item) {
            var itemOptions = {
                title: item.data.TickerText,
                published: item.data.PublishTime.toString(),
                body: item.data.BodyText,
            };

            var template = $("<div>").html($.template('/html/newsItem.html', itemOptions));
            return template[0].firstElementChild;
        }

        var layout = new WinJS.UI.GridLayout();
        if (ConferenceApp.Util.isSnapped()) {
            layout = new WinJS.UI.ListLayout();
        }

        var newsListView = new WinJS.UI.ListView($('#newsListView')[0], {
            itemDataSource: new WinJS.Binding.List(newsData).dataSource,
            oniteminvoked: newsItemInvoked,
            itemTemplate: WinJS.UI.simpleItemRenderer(newsItemRenderer),
            layout: layout,
            selectionMode: 'none',
        });


        if (state.newsListPosition) {
            newsListView.indexOfFirstVisible = state.newsListPosition;
        }

        this.relayout = function (newLayout) {
            if (newLayout === 'snapped') {
                newsListView.layout = new WinJS.UI.ListLayout();
            }
            else {
                newsListView.layout = new WinJS.UI.GridLayout();
            }
            newsListView.forceLayout();
        };
        
    }

    function initNoConnectionView() {
        $("#signInButton").click(function () {
            $("#signInButton").addClass("hidden");
            $("#loadingBar").removeClass("hidden");
            ConferenceApp.Api.signIn().then(ConferenceApp.Api.loadInitialData).then(function () {
                ConferenceApp.Api.startSync();
                ConferenceApp.Navigation.navigate('home');
            }, function () {
                $("#signInButton").removeClass("hidden");
                $("#loadingBar").addClass("hidden");
            });
        });
    }

    // define all of the views with their appropriate callbacks.
    createView('home', 'Home', initHomeView);
    createView('schedule', 'Schedule', initScheduleView);
    createView('sessions', 'Sessions', initSessionsView);
    createView('people', 'People', initPeopleView);
    createView('map', 'Floorplan', initMapView, beforeRenderMapView);
    createView('sessionDetail', 'Sessions', initSessionsDetailView, beforeRenderSessionsDetailView);
    createView('peopleDetail', 'People', initPeopleDetailView, beforeRenderPeopleDetailView);
    createView('searchResults', 'Search Results', initSearchResultsView);
    createView('videoPlayer', 'Video Player', initVideoPlayerView);
    createView('sessionNotes', 'Session Notes', initSessionNotesView, beforeRenderSessionNotesView);
    createView('tripReport', 'Trip Report', initTripReportView, beforeRenderTripReportView);
    createView('news', 'News', initNewsView);
    createView('noConnection', 'Not Signed In', initNoConnectionView);

    WinJS.Namespace.define("ConferenceApp", {
        Views: views,
    });

    WinJS.Namespace.define("ConferenceApp.View", {
        beforeView: beforeView,
        formatTime: formatTime,
        formatSpeakers: formatSpeakers
    });
  
})();
