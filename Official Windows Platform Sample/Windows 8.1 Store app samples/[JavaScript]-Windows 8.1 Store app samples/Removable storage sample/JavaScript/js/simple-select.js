//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var _itemList = null;
    var _onItemSelected = null;
    var _title = null;

    var showItemSelector = function (itemList, onItemSelected) {
        _itemList = itemList;
        _onItemSelected = onItemSelected;

        var selector = document.getElementById("simpleSelect");
        while (selector.childNodes.length > 0) {
            selector.removeChild(selector.childNodes[0]);
        }

        var itemSelector = document.createElement("div");
        var control = new ItemSelect(itemSelector);
        selector.appendChild(itemSelector);
    };

    // Control that populates and runs the item selector

    var ItemSelect = WinJS.UI.Pages.define("/html/simple-select.html", {
        ready: function (element, options) {
            var that = this;
            var selectElement = WinJS.Utilities.query("#simpleItemSelect", element);
            this._selectElement = selectElement[0];

            var size = _itemList.length;
            for (var i = 0; i < size; i++) {
                that._addItem(_itemList[i], i);
            }

            selectElement.listen("click", function (clickEvent) {
                var select = clickEvent.target;
                var index = 0;
                if (select.selectedIndex !== undefined) {
                    // Select was clicked
                    index = select.options[select.selectedIndex].value;
                    select.parentNode.style.display = "none";
                } else {
                    // Item was clicked
                    index = select.value;
                    select.parentNode.parentNode.style.display = "none";
                }
                _onItemSelected(_itemList[index]);
            });
            selectElement[0].size = (size > 5 ? 5 : size);
        },

        _addItem: function (item, index) {
            var option = document.createElement("option");
            option.text = item.name;
            option.value = String(index);
            this._selectElement.appendChild(option);
        },
    });

    WinJS.Namespace.define("SdkSample", {
        showItemSelector: showItemSelector,
    });

})();
