<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CalendarProto.aspx.cs" Inherits="WebTestApp.CalendarProto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Calendar Control Prototype</title>

    <style type="text/css">
        .ITCEventCalendar
        {
            font-family: Arial Verdana Sans-Serif;
        }
        .ITCEventCalendar A
        {
            text-decoration: none;
        }
        .ITCEventCalendar TR.controls
        {
            border: none;
            border-bottom: 1px solid black;
        }
        .ITCEventCalendar TR.controls TD
        {
            border: none;
            font-weight: bold;
            vertical-align: bottom;
        }
        .ITCEventCalendar TR.controls TD.prev
        {
            text-align: left;
            padding-left: 6px;
        }
        .ITCEventCalendar TR.controls TD.next
        {
            text-align: right;
            padding-right: 6px;
        }
        .ITCEventCalendar TR TH
        {
            font-weight: bold;
            border-bottom: 1px solid gray;
            border-right: 1px solid gray;
        }
        .ITCEventCalendar TR
        {
            border-right: 1px solid black;
            border-left: 1px solid black;
        }
        .ITCEventCalendar TR.dayRow TD
        {
            border-right: 1px solid silver;
            border-top: 1px solid silver;
        }
        .ITCEventCalendar TR.dayRow > TD
        {
            border-left: 1px solid silver;
        }
        .ITCEventCalendar TR.dayRow > TD ~ TD
        {
            border-left: none;
        }
        .ITCEventCalendar TR.dayRow TD DIV
        {
            min-height: 50px;
            vertical-align: top;
        }
        .ITCEventCalendary TR.dayRow TD DIV IMG
        {
            margin: 4px;
        }
        .ITCEventCalendar TD .date
        {
            font-size: 8pt;
            vertical-align: top;
            float: right;
        }
        .ITCEventCalendar TD .date.prevMonth
        {
            color: Silver;
        }
        .ITCEventCalendar TD .date.nextMonth
        {
            color: Silver;
        }
        .ITCEventCalendar TR.eventRow TD.empty
        {
            border-right: 1px solid silver;
            
        }
        .ITCEventCalendar TR.eventRow TD.event
        {
            border: 1px solid gray;
            background-color: #f5f3c7;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        

        <table id="tblEvents" width="600px" cellpadding="2" cellspacing="0" border="0" class="ITCEventCalendar">
            <colgroup>
                <col style="width:14%" />
                <col style="width:14%" />
                <col style="width:14%" />
                <col style="width:14%" />
                <col style="width:14%" />
                <col style="width:14%" />
                <col style="width:14%" />
            </colgroup>
            <tr class="controls">
                <td class="prev">
                    <asp:LinkButton ID="lnkPrev" runat="server" Text="&lt;&lt;" />
                </td>
                <td colspan="5"></td>
                <td class="next">
                    <asp:LinkButton ID="lnkNext" runat="server" Text="&gt;&gt;" />
                </td>
            </tr>
            <tr>
                <th>Sun</th>
                <th>Mon</th>
                <th>Tue</th>
                <th>Wed</th>
                <th>Thu</th>
                <th>Fri</th>
                <th>Sat</th>
            </tr>
            <tr>
                <td><div class="day first">
                    <span class="date otherMon">26</span>
                </div></td>
                <td><div class="day">
                    <span class="date otherMon">27</span>
                </div></td>
                <td><div class="day">
                    <span class="date otherMon">28</span>
                </div></td>
                <td><div class="day">
                    <span class="date otherMon">29</span>
                </div></td>
                <td><div class="day">
                    <span class="date otherMon">30</span>
                </div></td>
                <td><div class="day">
                    <span class="date otherMon">31</span>
                </div></td>
                <td><div class="day last">
                    <span class="date">1</span>
                </div></td>
            </tr>
            <tr>
                <td><div class="day first">
                    <span class="date">2</span>
                </div></td>
                <td><div class="day">
                    <span class="date">3</span>
                </div></td>
                <td><div class="day">
                    <span class="date">4</span>
                    <img src="images/folder.gif" alt="" />
                </div></td>
                <td><div class="day">
                    <span class="date">5</span>
                </div></td>
                <td><div class="day">
                    <span class="date">6</span>
                </div></td>
                <td><div class="day">
                    <span class="date">7</span>
                </div></td>
                <td><div class="day last">
                    <span class="date">8</span>
                </div></td>
            </tr>
            <tr>
                <td><div class="day first">
                    <span class="date">9</span>
                </div></td>
                <td><div class="day">
                    <span class="date">10</span>
                </div></td>
                <td><div class="day">
                    <span class="date">11</span>
                </div></td>
                <td><div class="day">
                    <span class="date">12</span>
                </div></td>
                <td><div class="day">
                    <span class="date">13</span>
                </div></td>
                <td><div class="day">
                    <span class="date">14</span>
                </div></td>
                <td><div class="day last">
                    <span class="date">15</span>
                </div></td>
            </tr>
            <tr>
                <td><div class="day first">
                    <span class="date">16</span>
                </div></td>
                <td><div class="day">
                    <span class="date">17</span>
                </div></td>
                <td><div class="day">
                    <span class="date">18</span>
                </div></td>
                <td><div class="day">
                    <span class="date">19</span>
                </div></td>
                <td><div class="day">
                    <span class="date">20</span>
                </div></td>
                <td><div class="day">
                    <span class="date">21</span>
                </div></td>
                <td><div class="day last">
                    <span class="date">22</span>
                </div></td>
            </tr>
            <tr>
                <td><div class="day first">
                    <span class="date">23</span>
                </div></td>
                <td><div class="day">
                    <span class="date">24</span>
                </div></td>
                <td><div class="day">
                    <span class="date">25</span>
                </div></td>
                <td><div class="day">
                    <span class="date">26</span>
                </div></td>
                <td><div class="day">
                    <span class="date">27</span>
                </div></td>
                <td><div class="day">
                    <span class="date">28</span>
                </div></td>
                <td><div class="day last">
                    <span class="date">29</span>
                </div></td>
            </tr>
            <tr>
                <td><div class="day first">
                    <span class="date">30</span>
                </div></td>
                <td><div class="day">
                    <span class="date otherMon">1</span>
                </div></td>
                <td><div class="day">
                    <span class="date otherMon">2</span>
                </div></td>
                <td><div class="day">
                    <span class="date otherMon">3</span>
                </div></td>
                <td><div class="day">
                    <span class="date otherMon">4</span>
                </div></td>
                <td><div class="day">
                    <span class="date otherMon">5</span>
                </div></td>
                <td><div class="day last">
                    <span class="date otherMon">6</span>
                </div></td>
            </tr>
        </table>


    </div>
    </form>
</body>
</html>
