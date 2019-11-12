/*
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
*/
(function () {
    "use strict";

    var controlGroups = [
        {
            groupKey: "commands",
            label: WinJS.Resources.getString("commandsCategoryTitle").value,
            objectNames: "",
            shortDesc: WinJS.Resources.getString("commandsCategoryShortDesc").value,
            fullDesc: WinJS.Resources.getString("commandsCategoryfullDesc").value,
            previewImage: "/images/appbar.png",
            icon: WinJS.UI.AppBarIcon.page
        },
        {
            groupKey: "buttons",
            label: WinJS.Resources.getString("buttonsCategoryTitle").value,
            objectNames: "",
            shortDesc: WinJS.Resources.getString("buttonsCategoryShortDesc").value,
            fullDesc: WinJS.Resources.getString("buttonsCategoryfullDesc").value,
            previewImage: "/images/button.png",
            icon: WinJS.UI.AppBarIcon.page
        },
        {
            groupKey: "collections",
            label: WinJS.Resources.getString("collectionsCategoryTitle").value,
            objectNames: "",
            shortDesc: WinJS.Resources.getString("collectionsCategoryShortDesc").value,
            fullDesc: WinJS.Resources.getString("collectionsCategoryfullDesc").value,
            previewImage: "/images/ListView2.png",
            icon: WinJS.UI.AppBarIcon.page
        },
        {
            groupKey: "layout",
            label: WinJS.Resources.getString("layoutsCategoryTitle").value,
            objectNames: "",
            shortDesc: WinJS.Resources.getString("layoutsCategoryShortDesc").value,
            fullDesc: WinJS.Resources.getString("layoutsCategoryfullDesc").value,
            previewImage: "/images/flexbox.png",
            icon: WinJS.UI.AppBarIcon.page
        },
        {
            groupKey: "menu",
            label: WinJS.Resources.getString("flyoutsCategoryTitle").value,
            objectNames: "",
            shortDesc: WinJS.Resources.getString("flyoutsCategoryShortDesc").value,
            fullDesc: WinJS.Resources.getString("flyoutsCategoryfullDesc").value,
            previewImage: "/images/Flyout.png",
            icon: WinJS.UI.AppBarIcon.page
        },
        {
            groupKey: "navigation",
            label: WinJS.Resources.getString("navigationCategoryTitle").value,
            objectNames: "",
            shortDesc: WinJS.Resources.getString("navigationCategoryShortDesc").value,
            fullDesc: WinJS.Resources.getString("navigationCategoryfullDesc").value,
            previewImage: "/images/navbar.png",
            icon: WinJS.UI.AppBarIcon.page
        },
        {
            groupKey: "progress",
            label: WinJS.Resources.getString("progressCategoryTitle").value,
            objectNames: "",
            shortDesc: WinJS.Resources.getString("progressCategoryShortDesc").value,
            fullDesc: WinJS.Resources.getString("progressCategoryfullDesc").value,
            previewImage: "/images/ProgressBar.png",
            icon: WinJS.UI.AppBarIcon.page
        },
        {
            groupKey: "search",
            label: WinJS.Resources.getString("searchCategoryTitle").value,
            objectNames: "",
            shortDesc: WinJS.Resources.getString("searchCategoryShortDesc").value,
            fullDesc: WinJS.Resources.getString("searchCategoryfullDesc").value,
            previewImage: "/images/searchbox.png",
            icon: WinJS.UI.AppBarIcon.page
        },
        {
            groupKey: "selection",
            label: WinJS.Resources.getString("selectionCategoryTitle").value,
            objectNames: "",
            shortDesc: WinJS.Resources.getString("selectionCategoryShortDesc").value,
            fullDesc: WinJS.Resources.getString("selectionCategoryfullDesc").value,
            previewImage: "/images/Checkbox.png",
            icon: WinJS.UI.AppBarIcon.page
        }

    ];

    var controlGroupsList = new WinJS.Binding.List(controlGroups);

    var controlGroupsIndex = {

        commands: controlGroups[0],
        buttons: controlGroups[1],
        collections: controlGroups[2],
        layouts: controlGroups[3],
        menu: controlGroups[4],
        navigation: controlGroups[5],
        progress: controlGroups[6],
        search: controlGroups[7],
        selection: controlGroups[8]
    };



    var controlsData = [
        {
            groups: [controlGroupsIndex.commands, controlGroupsIndex.menu],
            title: WinJS.Resources.getString("AppBarTitle").value,
            controlName: WinJS.Resources.getString("AppBarTitle").value,
            target: "/controlPages/appbar.html",
            previewImage: "/images/appbar.png",
            desc: WinJS.Resources.getString("AppBarDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                    [
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br229670.aspx", linkText: "AppBar"},
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh700497.aspx", linkText: "AppBarCommand" },
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh770557.aspx", linkText: "AppBarIcon" },
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465309.aspx", linkText: "Quickstart: Adding an app bar with commands" },
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh780658.aspx", linkText: "Quickstart: Adding an app bar with custom content" },
                        { link: "http://go.microsoft.com/fwlink/p/?linkid=252593", linkText: "HTML AppBar control sample" }
                    ]
                )
            
        },
        {
            groups: [controlGroupsIndex.buttons, controlGroupsIndex.navigation],
            altGroup: "navigation",
            title: WinJS.Resources.getString("BackButtonTitle").value,
            controlName: WinJS.Resources.getString("BackButtonControlName").value,
            target: "/controlPages/backbutton.html",
            previewImage: "/images/backbutton.png",
            desc: WinJS.Resources.getString("BackButtonDescription").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn255082.aspx", linkText: "BackButton" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465493.aspx", linkText: "Quickstart: Adding WinJS controls and styles" },
                    { link: "http://go.microsoft.com/fwlink/?LinkID=227744", linkText: "Navigation and navigation history sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.buttons],
            title: WinJS.Resources.getString("ButtonTitle").value,
            controlName: "button",
            target: "/controlPages/buttonControl.html",
            previewImage: "/images/button.png",
            desc: WinJS.Resources.getString("ButtonControlDescription").value,
            documentation: new WinJS.Binding.List(
                    [
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh453017.aspx", linkText: "button" },
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465482.aspx", linkText: "Quickstart: Adding a button" },
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/jj835822.aspx", linkText: "How to style buttons" }
                    ]
                )
        },
        {
            groups: [controlGroupsIndex.menu],
            title: WinJS.Resources.getString("ContextMenuTitle").value,
            controlName: WinJS.Resources.getString("ContextMenuTitle").value,
            target: "/controlPages/contextmenu.html",
            previewImage: "/images/contextMenu.png",
            desc: WinJS.Resources.getString("ContextMenuDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/library/windows/apps/br208693", linkText: "PopupMenu" },
                    { link: "http://msdn.microsoft.com/library/windows/apps/br242166", linkText: "UICommand" },
                    { link: "http://go.microsoft.com/fwlink/p/?linkid=234891", linkText: "Context menu sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.selection],
            title: WinJS.Resources.getString("CheckboxTitle").value,
            controlName: WinJS.Resources.getString("CheckboxControlName").value,
            target: "/controlPages/checkboxes.html",
            previewImage: "/images/checkbox.png",
            desc: WinJS.Resources.getString("CheckboxDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh466132.aspx", linkText: "input type='checkbox'" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/jj849979.aspx", linkText: "Quickstart: Adding checkbox controls" },
                    { link: "http://go.microsoft.com/fwlink/?LinkID=231508", linkText: "HTML essential controls sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.selection],
            title: WinJS.Resources.getString("DatePickerTitle").value,
            controlName: WinJS.Resources.getString("DatePickerControlName").value,
            target: "/controlPages/datepicker.html",
            previewImage: "/images/DatePicker.png",
            desc: WinJS.Resources.getString("DatePickerDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br211681.aspx", linkText: "DatePicker" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465480.aspx", linkText: "Quickstart: Adding a DatePicker" },
                    { link: "http://go.microsoft.com/fwlink/?LinkID=231644", linkText: "HTML DatePIcker and TimePicker controls sample" }
                ]
            )
        },


        {
            groups: [controlGroupsIndex.selection],
            title: WinJS.Resources.getString("FileUploadTitle").value,
            controlName: WinJS.Resources.getString("FileUploadControlName").value,
            target: "/controlPages/fileupload.html",
            previewImage: "images/fileupload.png",
            desc: WinJS.Resources.getString("FileUploadDescription").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh466145.aspx", linkText: "input type='file'" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/Hh700372.aspx", linkText: "Quickstart: Uploading a file" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.layouts],
            title: WinJS.Resources.getString("FlexboxTitle").value,
            controlName: WinJS.Resources.getString("FlexboxControlName").value,
            target: "/controlPages/flexboxLayout.html",
            previewImage: "images/flexbox.png",
            desc: WinJS.Resources.getString("FlexboxDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/ie/ms530751.aspx", linkText: "display" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh673531.aspx", linkText: "Flexbox overview" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.collections],
            title: WinJS.Resources.getString("FlipViewTitle").value,
            controlName: WinJS.Resources.getString("FlipViewControlName").value,
            target: "/controlPages/flipview.html",
            previewImage: "/images/FlipView.png",
            desc: WinJS.Resources.getString("FlipViewDescription").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br211711.aspx", linkText: "FlipView" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh700774.aspx", linkText: "List" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br229723.aspx", linkText: "Template" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465425.aspx", linkText: "Quickstart: Adding a FlipView" },
                    { link: "http://go.microsoft.com/fwlink/p/?linkid=242387", linkText: "HTML FlipView control sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.menu],
            title: WinJS.Resources.getString("FlyoutTitle").value,
            controlName: WinJS.Resources.getString("FlyoutControlName").value,
            target: "/controlPages/flyoutcontrol.html",
            previewImage: "images/Flyout.png",
            desc: WinJS.Resources.getString("FlyoutDescription").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br211726.aspx", linkText: "Flyout" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465354.aspx", linkText: "Quickstart: Adding a Flyout" },
                    { link: "http://go.microsoft.com/fwlink/p/?linkid=252630", linkText: "HTML Flyout control sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.layouts],
            title: WinJS.Resources.getString("GridLayoutTitle").value,
            controlName: WinJS.Resources.getString("GridLayoutControlName").value,
            target: "/controlPages/gridLayout.html",
            previewImage: "images/grid.png",
            desc: WinJS.Resources.getString("GridLayoutDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/ie/ms530751.aspx", linkText: "display" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh673533.aspx", linkText: "Grid layout overview" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.navigation, controlGroupsIndex.layouts],
            title: WinJS.Resources.getString("HubTitle").value,
            controlName: WinJS.Resources.getString("HubControlName").value,
            target: "/controlPages/hub.html",
            previewImage: "/images/Hub.png",
            desc: WinJS.Resources.getString("HubDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn255137.aspx", linkText: "Hub" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn255127.aspx", linkText: "HubSection" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn412707.aspx", linkText: "Quickstart: Using a hub control for layout and navigation" },
                    { link: "http://go.microsoft.com/fwlink/p/?LinkID=389437", linkText: "HTML Hub control sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.collections, controlGroupsIndex.selection],
            altGroup: "selection",
            title: WinJS.Resources.getString("ItemContainerTitle").value,
            controlName: WinJS.Resources.getString("ItemContainerControlName").value,
            target: "/controlPages/itemContainer.html",
            previewImage: "/images/itemContainer.png",
            desc: WinJS.Resources.getString("ItemContainerDescription").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn255188.aspx", linkText: "ItemContainer" },
                    { link: "http://go.microsoft.com/fwlink/?LinkID=310068", linkText: "HTML ItemContainer control sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.collections],
            title: WinJS.Resources.getString("ListViewTitle").value,
            controlName: WinJS.Resources.getString("ListViewControlName").value,
            target: "/controlPages/listview.html",
            previewImage: "/images/ListView.png",
            desc: WinJS.Resources.getString("ListViewDescription").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br211837.aspx", linkText: "ListView" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh700774.aspx", linkText: "List" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br229723.aspx", linkText: "Template" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465496.aspx", linkText: "Quickstart: Add a ListView" },
                    { link: "http://go.microsoft.com/fwlink/p/?linkid=231554", linkText: "HTML ListView essentials sample" },
                    { link: "http://go.microsoft.com/fwlink/p/?linkid=231499", linkText: "HTML ListView item templates sample" },
                    { link: "http://go.microsoft.com/fwlink/p/?linkid=231563", linkText: "HTML ListView grouping and SemanticZoom sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.menu],
            title: WinJS.Resources.getString("MenuTitle").value,
            controlName: WinJS.Resources.getString("MenuControlName").value,
            target: "/controlPages/menucontrols.html",
            previewImage: "/images/MenuFlyout.png",
            desc: WinJS.Resources.getString("MenuDescription").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh700921.aspx", linkText: "Menu" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh700879.aspx", linkText: "MenuCommand" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh921961.aspx", linkText: "Quickstart: Adding a header menu" },
                    { link: "http://go.microsoft.com/fwlink/p/?linkid=252630", linkText: "HTML Flyout control sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.layouts],
            title: WinJS.Resources.getString("MultiColumnTitle").value,
            controlName: WinJS.Resources.getString("MultiColumnControlName").value,
            target: "/controlPages/multiColumnLayout.html",
            previewImage: "images/multicolumnlayout.png",
            desc: WinJS.Resources.getString("MultiColumnDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh772211.aspx", linkText: "column-width" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh772195.aspx", linkText: "column-count" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh673534.aspx", linkText: "Multi-column layout overview" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.navigation, controlGroupsIndex.commands, controlGroupsIndex.menu],
            altGroup: "commands",
            title: WinJS.Resources.getString("NavBarTitle").value,
            controlName: WinJS.Resources.getString("NavBarControlName").value,
            target: "/controlPages/navbar.html",
            previewImage: "/images/navbar.png",
            desc: WinJS.Resources.getString("NavBarDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn301893.aspx", linkText: "NavBar" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn301875.aspx", linkText: "NavBarContainer" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn301859.aspx", linkText: "NavBarCommand" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn376409.aspx", linkText: "Quickstart: Adding a nav bar" },
                    { link: "http://go.microsoft.com/fwlink/p/?LinkID=304147", linkText: "HTML NavBar control sample" },
                    { link: "http://go.microsoft.com/fwlink/p/?LinkID=389436", linkText: "Navigation and navigation history sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.progress],
            title: WinJS.Resources.getString("ProgressTitle").value,
            controlName: WinJS.Resources.getString("ProgressControlName").value,
            target: "/controlPages/progressControls.html",
            previewImage: "images/ProgressBar.png",
            desc: WinJS.Resources.getString("ProgressDescription").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh441310.aspx", linkText: "progress" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465487.aspx", linkText: "Quickstart: Adding progress controls" },
                    { link: "http://go.microsoft.com/fwlink/?LinkID=231508", linkText: "HTML essential controls sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.selection, controlGroupsIndex.buttons],
            altGroup: "buttons",
            title: WinJS.Resources.getString("RadioButtonTitle").value,
            controlName: WinJS.Resources.getString("RadioButtonControlName").value,
            target: "/controlPages/radiobuttons.html",
            previewImage: "/images/RadioButton.png",
            desc: WinJS.Resources.getString("RadioButtonDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh466176.aspx", linkText: "input type='radio'" },
                    { link: "http://go.microsoft.com/fwlink/?LinkID=231508", linkText: "HTML essential controls sample" }
                ]
            )
        },
        {
            // TODO: Create an actual group
            groups: [controlGroupsIndex.selection],
            title: WinJS.Resources.getString("RangeTitle").value,
            controlName: WinJS.Resources.getString("RangeControlName").value,
            target: "/controlPages/range.html",
            previewImage: "/images/Slider.png",
            desc: WinJS.Resources.getString("RangeDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh466182.aspx", linkText: "input type='range'" },
                    { link: "http://go.microsoft.com/fwlink/?LinkID=231508", linkText: "HTML essential controls sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.layouts],
            title: WinJS.Resources.getString("RegionLayoutTitle").value,
            controlName: WinJS.Resources.getString("RegionLayoutControlName").value,
            target: "/controlPages/regionLayout.html",
            previewImage: "images/regionlayout.png",
            desc: WinJS.Resources.getString("RegionLayoutDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh771899.aspx", linkText: "-ms-flow-into" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh771897.aspx", linkText: "-ms-flow-from" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh673537.aspx", linkText: "Regions overview" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.collections],
            title: WinJS.Resources.getString("RepeaterTitle").value,
            controlName: WinJS.Resources.getString("RepeaterControlName").value,
            target: "/controlPages/repeaterControl.html",
            previewImage: "/images/repeater.png",
            desc: WinJS.Resources.getString("RepeaterDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn301916.aspx", linkText: "Repeater" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh700774.aspx", linkText: "List" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br229723.aspx", linkText: "Template" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn465805.aspx", linkText: "Quickstart: Adding repeater controls" },
                    { link: "http://go.microsoft.com/fwlink/p/?LinkId=324079", linkText: "HTML Repeater control sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.search, controlGroupsIndex.navigation],
            altGroup: "navigation",
            title: WinJS.Resources.getString("SearchTitle").value,
            controlName: WinJS.Resources.getString("SearchControlName").value,
            target: "/controlPages/searchControl.html",
            previewImage: "/images/searchbox.png",
            desc: WinJS.Resources.getString("SearchDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn301949.aspx", linkText: "SearchBox" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465238.aspx", linkText: "Quickstart: Adding search to an app" },
                    { link: "http://go.microsoft.com/fwlink/p/?LinkID=310082", linkText: "SearchBox control sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.selection],
            title: WinJS.Resources.getString("SelectTitle").value,
            controlName: WinJS.Resources.getString("SelectControlName").value,
            target: "/controlPages/selectControls.html",
            previewImage: "/images/ComboBox.png",
            desc: WinJS.Resources.getString("SelectDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/library/windows/apps/hh466252", linkText: "select" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465485.aspx", linkText: "Quickstart: Adding select controls" },
                    { link: "http://go.microsoft.com/fwlink/?LinkID=231508", linkText: "HTML essential controls sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.selection, controlGroupsIndex.collections],
            title: WinJS.Resources.getString("SemanticZoomTitle").value,
            controlName: WinJS.Resources.getString("SemanticZoomControlName").value,
            target: "/controlPages/semanticzoom.html",
            previewImage: "/images/SemanticZoom.png",
            desc: WinJS.Resources.getString("SemanticZoomDescription").value,
            documentation: new WinJS.Binding.List(
                [
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br229690.aspx", linkText: "SemanticZoom" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br211837.aspx", linkText: "ListView" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh700774.aspx", linkText: "List" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br229723.aspx", linkText: "Template" },
                    { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465492.aspx", linkText: "Quickstart: Adding SemanticZoom controls" },
                    { link: "http://go.microsoft.com/fwlink/p/?linkid=231563", linkText: "HTML ListView grouping and SemanticZoom sample" }
                ]
            )
        },
        {
            groups: [controlGroupsIndex.buttons],
            title: WinJS.Resources.getString("FormButtonTitle").value,
            controlName: WinJS.Resources.getString("FormButtonControlName").value,
            target: "/controlPages/formButtons.html",
            previewImage: "/images/resetsubmitbuttons.png",
            desc: WinJS.Resources.getString("FormButtonDescription").value,
            documentation: new WinJS.Binding.List(
                    [
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh453017.aspx", linkText: "button" },
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465482.aspx", linkText: "Quickstart: Adding a button" },
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/jj835822.aspx", linkText: "How to style buttons" }
                    ]
                )
        },
        {
            groups: [controlGroupsIndex.selection],
            title: WinJS.Resources.getString("TimePickerTitle").value,
            controlName: WinJS.Resources.getString("TimePickerControlName").value,
            target: "/controlPages/timepicker.html",
            previewImage: "/images/TimePicker.png",
            desc: WinJS.Resources.getString("TimePickerDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                    [
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/br229736.aspx", linkText: "TimePicker" },
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/hh465483.aspx", linkText: "Quickstart: Adding a TimePicker" },
                        { link: "http://go.microsoft.com/fwlink/?LinkID=231644", linkText: "HTML DatePIcker and TimePicker controls sample" }
                    ]
                )
        },
        {
            groups: [controlGroupsIndex.navigation],
            title: WinJS.Resources.getString("WebViewTitle").value,
            controlName: WinJS.Resources.getString("WebViewControlName").value,
            target: "/controlPages/webViewControl.html",
            previewImage: "/images/webview.png",
            desc: WinJS.Resources.getString("WebViewDescriptionShort").value,
            documentation: new WinJS.Binding.List(
                    [
                        { link: "http://msdn.microsoft.com/en-us/library/windows/apps/dn301831.aspx", linkText: "x-ms-webview" },
                        { link: "http://go.microsoft.com/fwlink/?LinkId=306259", linkText: "HTML WebView sample" }
                    ]
                )
        },

    ];


    var controlsList = new WinJS.Binding.List(controlsData);

    var groupedControlsList = controlsList.createGrouped(
        function groupKeySelector(item) { return item.groups[0].groupKey; },
        function groupDataSelector(item) { return item.group; }
    );


    function createFilteredControlList(groupName) {

        return controlsList.createFiltered(function (item) {

            var found = false; 
            for (i = 0; i < item.groups.length && !found; i++)
            {
                found = (item.groups[i].groupKey === groupName);
            }
            return found; 

        });

    }

    // Iterate through the controls and populate the object
    // name list of each group; 
    var currentGroup, currentItem;
    for (var i = 0; i < controlsData.length; i++) {
        currentItem = controlsData[i];

        for (var j = 0; j < currentItem.groups.length; j++) {
            currentGroup = currentItem.groups[j];
                        
            if (currentGroup.objectNames.length > 1) {
                currentGroup.objectNames = currentGroup.objectNames + ", ";
            }
            currentGroup.objectNames = currentGroup.objectNames + currentItem.title;

        }
    }

    WinJS.Namespace.define("ControlsData",
    {
        controlsList: controlsList,
        groupedControlsList : groupedControlsList,
        controlGroupsList: controlGroupsList,
        createFilteredControlList: createFilteredControlList
    });



})();