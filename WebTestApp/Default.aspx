<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebTestApp._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to ASP.NET!
    </h2>
    <%--<p>
        To learn more about ASP.NET visit <a href="http://www.asp.net" title="ASP.NET Website">www.asp.net</a>.
    </p>
    <p>
        You can also find <a href="http://go.microsoft.com/fwlink/?LinkID=152368&amp;clcid=0x409"
            title="MSDN ASP.NET Docs">documentation on ASP.NET at MSDN</a>.
    </p>--%>

    <div style="margin-top:20px;margin-bottom:40px;">
        <rs:CalendarView ID="calTest" runat="server" Width="700px" BorderStyle="Solid" BorderColor="Silver" BorderWidth="1px">
            <DayHeaderStyle ForeColor="White" />
            <OtherMonthDateStyle ForeColor="Silver" />
            <CurrentMonthDateStyle ForeColor="Black" />
            <EventStyle ForeColor="Black" BorderColor="Gray" BorderStyle="Solid" BackColor="LemonChiffon" BorderWidth="1px" />
            <PrevNextStyle />
        </rs:CalendarView>
    </div>

    <%--<rs:Button ID="btnTest" runat="server" StyleTheme="Standard" ColorTheme="Blue" Text="Click Here" Enabled="false" />
    <rs:ModalPopup ID="modalTest" runat="server" HeaderText="Modal Test" ButtonConfig="Custom" TargetControlID="btnTest"
        OkTemplateControlID="btnOK" CancelTemplateControlID="btnCancel">
        <ContentTemplate>
            <p>This is a test of the new modal popup control.</p>
        </ContentTemplate>
        <CommandTemplate>
            <rs:Button ID="btnCancel" runat="server" StyleTheme="Standard" ColorTheme="Red" Text="Cancel" />
            <rs:Button ID="btnOK" runat="server" StyleTheme="Standard" ColorTheme="Green" Text="OK" />
        </CommandTemplate>
    </rs:ModalPopup>--%>

    <%--<asp:UpdatePanel ID="UpdatePanelTreeView" runat="server">
        <ContentTemplate>
            <rs:DirectoryListTreeView ID="treeView" runat="server" RootPath="c:\" RootType="PhysicalPath" ShowLines="true" ShowNodeImages="false" />
        </ContentTemplate>
    </asp:UpdatePanel>--%>

    <%--<rs:CustomFrame ID="customFrameTest" runat="server" Width="400px" MinimumHeight="300px" FrameTitle="Test Frame" ColorThemeTheme="blue">
    </rs:CustomFrame>--%>
    <asp:FileUpload ID="FileUpload1" runat="server" />
    <asp:Label ID="lblMessage" runat="server" />
</asp:Content>
