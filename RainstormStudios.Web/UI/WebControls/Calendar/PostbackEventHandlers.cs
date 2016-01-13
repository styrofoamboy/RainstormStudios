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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Web.UI.WebControls.Calendar
{
    public delegate void ReminderEventHandler(object sender, ReminderEventArgs e);
    public class ReminderEventArgs : EventArgs
    {
        public object
            ProviderReminderKey { get; private set; }
        public CalendarEvent
            ParentEvent { get; private set; }
        public DateTime
            ParentEventTime { get; private set; }

        public ReminderEventArgs(object provRemindKey, CalendarEvent parentEvent, DateTime eventDatetime)
        {
            this.ProviderReminderKey = provRemindKey;
            this.ParentEvent = parentEvent;
            this.ParentEventTime = eventDatetime;
        }
    }
    public delegate void DayRenderEventHandler(object sender, DayRenderEventArgs e);
    public class DayRenderEventArgs : EventArgs
    {
        public DateTime
            Date { get; private set; }
        public CalendarEventCollection
            Events { get; private set; }

        public DayRenderEventArgs(DateTime date, CalendarEventCollection events)
        {
            this.Date = date;
            this.Events = events;
        }
        public DayRenderEventArgs(DateTime date, CalendarEvent[] events)
            : this(date, new CalendarEventCollection(events))
        { }
    }
    public delegate void EventRenderEventHandler(object sender, EventRenderEventArgs e);
    public class EventRenderEventArgs : EventArgs
    {
        public DateTime
            Date { get; private set; }
        public CalendarEvent
            RenderedEvent { get; private set; }

        public EventRenderEventArgs(DateTime date, CalendarEvent calEvent)
        {
            this.Date = date;
            this.RenderedEvent = calEvent;
        }
    }
    public delegate void DayClickedEventHandler(object sender, DayClickedEventArgs e);
    public class DayClickedEventArgs : EventArgs
    {
        public DateTime
            Date { get; private set; }
        public CalendarEventCollection
            Events { get; private set; }

        public DayClickedEventArgs(DateTime date, CalendarEventCollection events)
        {
            this.Date = date;
            this.Events = events;
        }
        public DayClickedEventArgs(DateTime date, CalendarEvent[] events)
            : this(date, new CalendarEventCollection(events))
        { }
    }
    public delegate void EventClickedEventHandler(object sender, EventClickedEventArgs e);
    public class EventClickedEventArgs : EventArgs
    {
        public DateTime
            Date { get; private set; }
        public CalendarEvent
            ClickedEvent { get; private set; }

        public EventClickedEventArgs(DateTime date, CalendarEvent calEvent)
        {
            this.Date = date;
            this.ClickedEvent = calEvent;
        }
    }
}
