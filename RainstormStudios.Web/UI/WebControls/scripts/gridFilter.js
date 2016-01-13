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

var $cover = null;

function initGridFilterSearchIndicator(targetID, myClientID, coverBgClr, coverOpacity, imgLoaderUrl) {
    //debugger;
    var $tgt = $("[id$='" + targetID + "']");
    var pos = $tgt.extend({
        width: $tgt.outerWidth(),
        height: $tgt.outerHeight()
    }, $tgt.position());

    $("a[id$='btnFilter_" + myClientID + "']").click(function () {
        showGridFilterSearchIndicator($tgt, this, pos, coverBgClr, coverOpacity, imgLoaderUrl);
    });
    $("a[id$='btnClear_" + myClientID + "']").click(function () {
        showGridFilterSearchIndicator($tgt, this, pos, coverBgClr, coverOpacity, imgLoaderUrl);
    });
}
function showGridFilterSearchIndicator(target, src, pos, coverBgClr, coverOpacity, imgLoaderUrl) {
    //debugger;
    $cover = $('<div>', {
        id: 'overlay',
        css: {
            position: 'absolute',
            top: pos.top,
            left: pos.left,
            width: pos.width,
            height: pos.height,
            backgroundColor: coverBgClr,
            opacity: coverOpacity,
            zIndex: 10000
        }
    });
    $('<img', {
        src: imgLoaderUrl,
        alt: 'Loading. Please wait...',
        css: {
            position: 'absolute',
            top: pos.top, //+ ((pos.height / 2) - (32 / 2)),
            left: pos.left, //+ ((pos.width / 2) - (32 / 2))
            zIndex: 10001
        }
    }).appendTo($cover);
    $cover.appendTo(target)
}
function removeCover() {
    $cover.hide('fast');
    $cover.remove();
}