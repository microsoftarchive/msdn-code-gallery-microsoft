(function () {
    "use strict";

    var ui = WinJS.UI;

    ui.Pages.define("/pages/chat/chat.html", {

        itemInvoked: function (args) {
            var listContacts = document.body.querySelector(".contactslist").winControl;
            var itemsContacts = this.getItemsContact(args.detail.itemIndex);

            listContacts.itemDataSource = itemsContacts.dataSource;
        },

        contactInvoked: function (args) {
            WinJS.Navigation.navigate("/pages/table/table.html");
        },

        textInputkeyDown: function (args) {
            // Has the Enter key been pressed?
            if (args.keyCode === WinJS.Utilities.Key.enter) {
                var textinput = document.querySelector('.chatTextInput');

                // Add the text in the text input area immediately to the chat history area.
                var texttoadd = textinput.value;
                if (texttoadd !== "") {
                    // Clear the text input area.
                    textinput.value = "";

                    this.addTextToChat("Me: " + texttoadd);

                    var itemPage = this;

                    // Start a 3 second timer to have a simulated reply appear later.
                    var setupTimeoutFired = function () {
                        var sendername = "Unknown sender";
                        itemPage.addTextToChat(sendername + ": This is a sample reply.");
                    };

                    setTimeout(setupTimeoutFired, 3000);
                }
            }
        },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var listView = element.querySelector(".groupslist").winControl;
            var listContacts = element.querySelector(".contactslist").winControl;

            var itemsGroup = this.getItemsGroup();
            var itemsContacts = this.getItemsContact(0);

            listView.itemDataSource = itemsGroup.dataSource;
            listView.itemTemplate = element.querySelector(".itemtemplate");
            listView.addEventListener("iteminvoked", this.itemInvoked.bind(this));

            listContacts.itemDataSource = itemsContacts.dataSource;
            listContacts.itemTemplate = element.querySelector(".itemtemplate");
            listContacts.addEventListener("iteminvoked", this.contactInvoked.bind(this));

            var chatTextInput = element.querySelector('.chatTextInput');
            chatTextInput.addEventListener("keydown", this.textInputkeyDown.bind(this));
        },

        getItemsGroup: function () {
            var colors = ["red", "green", "blue"];
            var items = new WinJS.Binding.List();
            for (var i = 1; i <= 3; i++) {
                items.push({
                    key: i,
                    title: "Group " + i,
                    subtitle: "This is Group " + i,
                    borderAll: "2px solid rgba(255, 255, 255, 1.0)",
                    backgroundColor: colors[(i - 1) % colors.length],
                });
            }

            return items;
        },

        getItemsContact: function (index) {
            var colors = ["red", "green", "blue"];
            var items = new WinJS.Binding.List();
            for (var i = 1; i <= 3; i++) {
                items.push({
                    key: i,
                    title: 'Contact ' + i,
                    subtitle: 'This is Contact ' + (index + 1) + '.' + i,
                    borderAll: "2px solid rgba(255, 255, 255, 1.0)",
                    backgroundColor: colors[index],
                });
            }
            return items;
        },

        addTextToChat: function (texttoadd) {
            var textEcho = document.querySelector('.chatTextEchoContainer');
            if (textEcho) {
                // Add a div element containing the new text being echoed, beneath the element containing all the
                // echoed text. This leads to assistive technologies notifying users immediately of the update due
                // to the use of the aria-live and aria-relevant properties on the chatTextEchoContainer element.
                textEcho.insertAdjacentHTML('beforeEnd', "<div class=\"chatTextEcho\">" + texttoadd + "</div>");
            }
        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState) {
        }
    });
})();
