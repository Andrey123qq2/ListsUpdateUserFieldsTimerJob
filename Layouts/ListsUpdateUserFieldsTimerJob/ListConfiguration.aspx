﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
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
    <script type = "text/javascript">
        function ShowHideForceUpdateCamlQuery() {
            var forceUpdateCheckBox = document.querySelector("[id$=ForceUpdate]");
            var forceUpdateCamlQueryTableRow = document.querySelector("[id$=ForceUpdateCamlQueryTableRow]");
            var disableForceUpdateCheckBoxRow = document.querySelector("[id$=DisableForceUpdateAfterRunTableRow]");
            var disableForceUpdatePermissionsCheckBoxRow = document.querySelector("[id$=DisableForceUpdatePermissionsTableRow]");
            if (forceUpdateCheckBox.checked) {
                forceUpdateCamlQueryTableRow.style.display = '';
                disableForceUpdateCheckBoxRow.style.display = '';
                disableForceUpdatePermissionsCheckBoxRow.style.display = '';
            } else {
                forceUpdateCamlQueryTableRow.style.display = 'none';
                disableForceUpdateCheckBoxRow.style.display = 'none';
                disableForceUpdatePermissionsCheckBoxRow.style.display = 'none';
            }
        }
    </script>
    <asp:Table ID="AdditionalParamsTable" runat="server" HorizontalAlign="Left" CssClass="ms-viewheadertr" style="margin-bottom:20px;margin-top:20px;" >
        <asp:TableRow ID="TableRow1" runat="server" BackColor="White">
            <asp:TableCell Width="200">Enable</asp:TableCell>
            <asp:TableCell>
                <asp:CheckBox ID="EnableCheckBox" runat="server"></asp:CheckBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server" BackColor="White" >
            <asp:TableCell>UserField</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="UserFieldDropDownList" runat="server"></asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow3" runat="server" BackColor="White" >
            <asp:TableCell>Additional CamlQuery</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="AdditionalCamlQuery" runat="server" Width="380" TextMode="MultiLine" Height="140"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow5" runat="server" BackColor="White" >
            <asp:TableCell>Force update items</asp:TableCell>
            <asp:TableCell>
                <asp:CheckBox ID="ForceUpdate" runat="server"></asp:CheckBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="DisableForceUpdatePermissionsTableRow" runat="server" BackColor="White" >
            <asp:TableCell>Disable force update permissions</asp:TableCell>
            <asp:TableCell>
                <asp:CheckBox ID="DisableForceUpdatePermissions" runat="server"></asp:CheckBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="DisableForceUpdateAfterRunTableRow" runat="server" BackColor="White" >
            <asp:TableCell>Disable force update after run</asp:TableCell>
            <asp:TableCell>
                <asp:CheckBox ID="DisableForceUpdateAfterRun" runat="server"></asp:CheckBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="ForceUpdateCamlQueryTableRow" runat="server" BackColor="White" >
            <asp:TableCell>Force update CamlQuery</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="ForceUpdateCamlQuery" runat="server" Width="380" TextMode="MultiLine" Height="140"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow7" runat="server" BackColor="White" >
            <asp:TableCell>TimerJob Settings</asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="TimerJobSettings" runat="server"></asp:HyperLink>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow8" runat="server" BackColor="White" >
            <asp:TableCell>Notes</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="Notes" runat="server" Width="380" TextMode="MultiLine" Height="140"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <SharePoint:SPGridView ID="FieldsTable" runat="server" AutoGenerateColumns="false">
        <RowStyle BackColor="#f6f7f8" Height="30px" HorizontalAlign="Left" />
        <AlternatingRowStyle BackColor="White" ForeColor="#000" Height="30px" HorizontalAlign="Left" />
        <HeaderStyle Font-Bold="true" HorizontalAlign="Left" CssClass="ms-viewheadertr" />
        <Columns>
            <asp:TemplateField HeaderText="Field" HeaderStyle-Width="250px">
                <ItemTemplate>
                    <asp:Label ID="FieldLabel" runat="server" Text='<%# Eval("FieldName") %>' data-fieldInternalName='<%# Eval("FieldInternalName") %>'></asp:Label>
                </ItemTemplate> 
            </asp:TemplateField> 
            <asp:TemplateField HeaderText="Attribute" HeaderStyle-Width="250px">
                <ItemTemplate>
                    <asp:DropDownList ID="DropDownList1" runat="server" SelectedValue='<%# Eval("Attribute") %>' DataSource='<%# Eval("AttributesList") %>' ></asp:DropDownList>
                </ItemTemplate> 
            </asp:TemplateField> 
        </Columns>
    </SharePoint:SPGridView>
    <asp:Button ID="ButtonOK" runat="server" Text="OK" OnClick="ButtonOK_EventHandler"/>
    <asp:Button ID="ButtonCANCEL" runat="server" Text="Cancel" OnClick="ButtonCANCEL_EventHandler"/>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
TimerJob: update user attribute fields
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
TimerJob: update user attribute fields
</asp:Content>
