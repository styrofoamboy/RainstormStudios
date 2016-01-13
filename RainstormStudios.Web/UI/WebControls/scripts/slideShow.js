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

function initSlideshow(ssID, delay, start_frame, fadeDur) {
    var lis = $("[id$='" + ssID + "']").children("li");

    for (i = 0; i < lis.length; i++) {
        if (i != 0) {
            lis[i].style.display = 'none';
        }
    }
    end_frame = lis.length - 1;

    start_slideshow(start_frame, end_frame, delay, lis, fadeDur);
    $("[id$='" + ssID + "']").css('visibility', 'visible');
}

function start_slideshow(start_frame, end_frame, delay, lis, fadeDur) {
    setTimeout(advanceSlideshow(start_frame, start_frame, end_frame, delay, lis, fadeDur), delay);
}

function advanceSlideshow(frame, start_frame, end_frame, delay, lis, fadeDur) {
    return (function () {
        //lis = $("#slide-images").children("li");
        $(lis[frame]).fadeToggle(fadeDur);
        if (frame == end_frame) { frame = start_frame; } else { frame++; }
        lisAppear = lis[frame];
        $(lisAppear).fadeToggle(fadeDur);
        setTimeout(advanceSlideshow(frame, start_frame, end_frame, delay, lis, fadeDur), delay);
    })
}