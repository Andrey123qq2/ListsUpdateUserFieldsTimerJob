<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListConfiguration.aspx.cs" Inherits="ListsUpdateUserFieldsTimerJob.Layouts.ListsUpdateUserFieldsTimerJob.ListConfiguration" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:Table ID="AdditionalParamsTable" runat="server" HorizontalAlign="Left" CssClass="ms-viewheadertr">
        <asp:TableHeaderRow runat="server" ForeColor="Snow" BackColor="OliveDrab" Font-Bold="true" >
            <asp:TableHeaderCell>Parameter</asp:TableHeaderCell>
            <asp:TableHeaderCell>Value</asp:TableHeaderCell>
        </asp:TableHeaderRow>
        <asp:TableRow ID="TableRow1" runat="server" BackColor="White" >
            <asp:TableCell>UserField</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="UserFieldDropDownList" runat="server"></asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br/>
    <SharePoint:SPGridView ID="FieldsTable" runat="server" AutoGenerateColumns="false">
        <RowStyle BackColor="#f6f7f8" Height="30px" HorizontalAlign="Left" />
        <AlternatingRowStyle BackColor="White" ForeColor="#000" Height="30px" HorizontalAlign="Left" />
        <HeaderStyle Font-Bold="true" HorizontalAlign="Left" CssClass="ms-viewheadertr" />
        <HeaderStyle />
        <Columns>
            <asp:TemplateField HeaderText="Field" HeaderStyle-Width="250px">
                <ItemTemplate>
                    <asp:Label ID="FieldLabel" runat="server" Text='<%# Eval("FieldName") %>'></asp:Label>
                </ItemTemplate> 
            </asp:TemplateField> 
            <asp:TemplateField HeaderText="Attribute" HeaderStyle-Width="250px">
                <ItemTemplate>
                    <asp:DropDownList ID="DropDownList1" runat="server"></asp:DropDownList>
                </ItemTemplate> 
            </asp:TemplateField> 
        </Columns>
    </SharePoint:SPGridView>
    <asp:Button ID="ButtonOK" runat="server" Text="OK" OnClick="ButtonOK_EventHandler"/>
    <asp:Button ID="ButtonCANCEL" runat="server" Text="Cancel" OnClick="ButtonCANCEL_EventHandler"/>
</asp:Content>


<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Application Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
My Application Page
</asp:Content>
