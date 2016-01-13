//  Copyright (c) 2008, Michael unfried
//  Email:  serbius3@gmail.com
//  All rights reserved.

//  Redistribution and use in source and binary forms, with or without modification, 
//  are permitted provided that the following conditions are met:

//  Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer. 
//  Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution. 

//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.

var currentVisibleDynamicMenuID = null;
function dynamicMenuControl_SetupPopupMenus(srcID, divID, providerKey) {
    if ($("input[id*='hdnDynamicMenuActiveMenuItem']").value == providerKey)
        $("[id*='" + divID + "']").show();

    $("span[id*='" + srcID + "']").click(function () {
        // Remove the 'active' class from all menu items.
        $("span.menuItem").removeClass('active');

        // Hide any existing sub menus.
        $("div.popoutSubMenu").hide('fast');

        // Get the parent MenuItem.
        var $menuItem = null;
        if ($(this).hasClass('menuItem')) {
            $menuItem = $(this);
        } else {
            $menuItem = $(this).parent("span.menuItem");
        }

        // Get the whole popout menu div.
        var $menuDiv = $menuItem.parent("div.PopoutMenu").parent("div.iconMenu");

        if (srcID != currentVisibleDynamicMenuID) {
            // If the current active menuItemID is not the same as the one just clicked, open it's sub menu.
            $menuItem.addClass('active');

            // Show the sub menu div.
            $("div[id*='" + divID + "']").show('fast');

            // Add the 'popoutMenuOen' class to the master menu.  This gives the main menu a min-height to prevent it from 'flickering' as the sub menus show/hide.
            $menuDiv.addClass('popoutMenuOpen');

            // Record the ID of the last menu item clicked on.
            currentVisibleDynamicMenuID = srcID;

            updateActiveMenuItem(providerKey, null);
        }
        else {
            // If it *is* the same, we're just going to leave all the sub menus closed, clear the 'current' menuItemID...
            currentVisibleDynamicMenuID = null;

            // ...and remove the 'open' CSS from the main menu to allow it to shrink back to its original size.
            $menuDiv.removeClass('popoutMenuOpen');

            updateActiveMenuItem(null, null);
        }
    });
}
function ReceiveServerData(arg, context) {
    //debugger;
}

$(document).ready(function () {
    $("span.menuItem:has(a[href])").click(function () {
        var $myAnchor = $(this).children("a[href]");
        if ($myAnchor && $myAnchor.length) {
            window.location = $myAnchor.attr('href');
        }
    });
});