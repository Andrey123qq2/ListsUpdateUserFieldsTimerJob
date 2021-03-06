<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimerJobSettings.aspx.cs" Inherits="ListsUpdateUserFieldsTimerJob.Layouts.ListsUpdateUserFieldsTimerJob.TimerJobSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:Table ID="AdditionalParamsTable" runat="server" HorizontalAlign="Left" CssClass="ms-viewheadertr" style="margin-bottom:20px;margin-top:20px;" >
        <asp:TableRow ID="TableRow1" runat="server" BackColor="White" >
            <asp:TableCell>SPReport WebUrl (site relative)</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="SPReportWebUrl" runat="server" Width="300px"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server" BackColor="White" >
            <asp:TableCell>SPReport LibraryName</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="SPReportLibraryName" runat="server" Width="300px"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server" BackColor="White" >
            <asp:TableCell>SPReport FilePathTemplate</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="SPReportFilePathTemplate" runat="server" Width="300px"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow4" runat="server" BackColor="White" >
            <asp:TableCell>Site all lists configs</asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="AllConfigs" runat="server" Text="AllConfigs" NavigateUrl="SiteAllConfigs.aspx"></asp:HyperLink>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <SharePoint:SPGridView ID="AttributesTable" runat="server" AutoGenerateColumns="false" style="margin-top:20px;">
        <RowStyle BackColor="#f6f7f8" Height="30px" HorizontalAlign="Left" />
        <AlternatingRowStyle BackColor="White" ForeColor="#000" Height="30px" HorizontalAlign="Left" />
        <HeaderStyle Font-Bold="true" HorizontalAlign="Left" CssClass="ms-viewheadertr" />
        <Columns>
            <asp:TemplateField HeaderText="Attribute" HeaderStyle-Width="250px">
                <ItemTemplate>
                    <asp:Label ID="AttributeLabel" runat="server" Text='<%# Eval("AttributeName") %>'></asp:Label>
                </ItemTemplate> 
            </asp:TemplateField> 
            <asp:TemplateField HeaderText="AttributesOptInLists" ControlStyle-Width="100">
                <ItemTemplate>
                    <asp:CheckBox ID="AttributesOptInLists" runat="server" AutoPostBack="false" Checked='<%# Eval("AttributesOptInLists") %>'/>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </SharePoint:SPGridView>
    <asp:Button ID="ButtonOK" runat="server" Text="OK" OnClick="ButtonOK_EventHandler"/>
    <asp:Button ID="ButtonCANCEL" runat="server" Text="Cancel" OnClick="ButtonCANCEL_EventHandler"/>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
TimerJob Common Settings
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
TimerJob Common Settings
</asp:Content>
