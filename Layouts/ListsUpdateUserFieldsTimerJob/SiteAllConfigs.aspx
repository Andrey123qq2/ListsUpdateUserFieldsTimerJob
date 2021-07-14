<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SiteAllConfigs.aspx.cs" Inherits="ListsUpdateUserFieldsTimerJob.Layouts.ListsUpdateUserFieldsTimerJob.SiteAllConfigs" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <br/>
    <asp:UpdatePanel ID="ListsConfigsUpdatePanel" runat="server">
        <ContentTemplate>
            <div id="divloadingImage" visible="true" runat="server">
                <br />
                <asp:Image ID="imgLoading" runat="server" ImageUrl="/_layouts/Images/gears_anv4.gif" />
                <br />
            </div>
            <SharePoint:SPGridView ID="ListsConfigsTable" runat="server" AutoGenerateColumns="false" Visible="false">
                <RowStyle BackColor="#f6f7f8" Height="30px" HorizontalAlign="Left" />
                <AlternatingRowStyle BackColor="White" ForeColor="#000" Height="30px" HorizontalAlign="Left" />
                <HeaderStyle Font-Bold="true" HorizontalAlign="Left" CssClass="ms-viewheadertr" />
                <Columns>
                    <asp:TemplateField HeaderText="SP List" HeaderStyle-Width="200px">
                        <ItemTemplate>
                            <asp:HyperLink ID="ListLink" runat="server" ControlStyle-CssClass="table-links" Text='<%# Eval("ListTitle") %>' 
                                NavigateUrl='<%# Eval("ListUrl") %>'/>
                            </ItemTemplate> 
                    </asp:TemplateField> 
                    <asp:TemplateField HeaderText="Config" HeaderStyle-Width="200px">
                        <ItemTemplate>
                            <asp:HyperLink ID="ConfLink" runat="server" ControlStyle-CssClass="table-links" Text='<%# Eval("ConfTitle") %>' 
                                NavigateUrl='<%# Eval("ConfUrl") %>'/>
                        </ItemTemplate> 
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Enabled" HeaderStyle-Width="200px">
                        <ItemTemplate>
                            <asp:Label ID="Enabled" runat="server" Text='<%# Eval("ConfEnabled") %>'></asp:Label>
                        </ItemTemplate> 
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Modified" HeaderStyle-Width="200px">
                        <ItemTemplate>
                            <asp:Label ID="ModifiedDate" runat="server" Text='<%# Eval("ConfModifiedDate") %>'></asp:Label>
                        </ItemTemplate> 
                    </asp:TemplateField>
                </Columns>
            </SharePoint:SPGridView>
            <div id="divWebPartError" visible="false" runat="server" style="color:red; font-weight: bold">
                <br />
                An error has occurred, please contact administrator.
                <br />
            </div>
            <asp:Timer ID="ListsConfigsTimer" Interval="1" runat="server" OnTick="Timer1_Tick" Enabled ="true"></asp:Timer>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Site all configs
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    <a id="ListConfigsLink" runat="server">Settings</a>
    <span>
        <span style="height:16px;width:16px;position:relative;display:inline-block;overflow:hidden;">
            <img src="/_layouts/15/images/spcommon.png?rev=43#ThemeKey=spcommon" alt=":" style="position:absolute;left:-109px !important;top:-232px !important;"/>
        </span>
    </span>
    <span>site all configs</span>

</asp:Content>
