using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Linq;
using System.Configuration;
using SPWebPartsCommon;
using SPHelpers;

namespace ListsUpdateUserFieldsTimerJob.Layouts.ListsUpdateUserFieldsTimerJob
{
    public partial class ListConfiguration : LayoutsPageBase
    {
        private SPList _pageSPList;
        private ListConfigUpdateUserFields _TJListConf;
        private SPFieldCollection _listFields;
        private List<string> _profilesAttributes;
        private readonly string _currentUrl = HttpContext.Current.Request.RawUrl;
        protected void Page_Load(object sender, EventArgs e)
        {
            InitParams();
            if (IsPostBack)
                return;
            BindDataToAdditionalTable();
            BindDataToFieldsTable();
        }
       
        private void InitParams()
        {
            Guid listGuid = new Guid(Request.QueryString["List"]);
            _pageSPList = SPListHelpers.GetSPList(SPContext.Current.Web.Url, listGuid);
            _listFields = _pageSPList.Fields;
            _profilesAttributes = GetProfilesAttributes();
            _TJListConf = PropertyBagConfHelper<ListConfigUpdateUserFields>.Get(
                _pageSPList.RootFolder.Properties, 
                CommonConstants.LIST_PROPERTY_JSON_CONF
            );
        }
        private List<string> GetProfilesAttributes()
        {
            var tjConf = PropertyBagConfHelper<TimerJobConfig>.Get(
                _pageSPList.ParentWeb.Site.RootWeb.AllProperties, 
                CommonConstants.LIST_PROPERTY_JSON_CONF
            );
            List<string> profilesAttributes = tjConf.AttributesOptInLists;
            return profilesAttributes;
        }

        #region BindData to Page
        private void BindDataToAdditionalTable()
        {
            BindDataToUserField();
            BindDataToEnableCheckBox();
            BindDataToTimerJobUrl();
        }
        private void BindDataToUserField()
        {
            UserFieldDropDownList.DataSource = GetPersonFieldsForUserField();
            UserFieldDropDownList.DataBind();
            UserFieldDropDownList.SelectedValue = GetSelectedValueForUserField();
        }
        private List<string> GetPersonFieldsForUserField()
        {
            List<string> personFields = _listFields
                .Cast<SPField>().ToList()
                .Where(f => !f.Hidden && f.TypeAsString.Contains("User"))
                .Select(f => f.Title)
                .ToList();
            personFields.Add(String.Empty);
            personFields.Sort();
            return personFields;
        }
        private string GetSelectedValueForUserField()
        {
            string fieldTitleByListConf = null;
            try
            {
                fieldTitleByListConf = _pageSPList.Fields.GetFieldByInternalName(_TJListConf.UserField).Title;
            }
            catch (Exception ex)
            {
                var message = String.Format(CommonConstants.ERROR_MESSAGE_TEMPLATE, _pageSPList.ID, "", ex.ToString());
                SPLogger.WriteLog(SPLogger.Category.Unexpected, "List Config Error", message);
            }
            string selectedValueForUserField = String.IsNullOrEmpty(fieldTitleByListConf) ? String.Empty : fieldTitleByListConf;
            return selectedValueForUserField;
        }
        private void BindDataToEnableCheckBox()
        {
            EnableCheckBox.Checked = _TJListConf.Enable;
        }
        private void BindDataToTimerJobUrl()
        {
            TimerJobSettings.NavigateUrl = "/_layouts/15/ListsUpdateUserFieldsTimerJob/TimerJobSettings.aspx?Source=" + _currentUrl;
            TimerJobSettings.Text = "Common options";
        }
        private void BindDataToFieldsTable()
        {
            FieldsTable.DataSource = GetDataForFieldsTable();
            FieldsTable.DataBind();
        }
        private DataTable GetDataForFieldsTable()
        {
            var fieldsDataTable = new DataTable();
            AddColumnsToFieldsDataTable(fieldsDataTable);
            AddDataToFieldsDataTable(fieldsDataTable);
            return fieldsDataTable;
        }
        private void AddColumnsToFieldsDataTable(DataTable fieldsDataTable)
        {
            fieldsDataTable.Columns.Add(new DataColumn("FieldName", typeof(string)));
            fieldsDataTable.Columns.Add(new DataColumn("FieldInternalName", typeof(string)));
            fieldsDataTable.Columns.Add(new DataColumn("Attribute", typeof(string)));
            fieldsDataTable.Columns.Add(new DataColumn("AttributesList", typeof(Array)));
        }
        private void AddDataToFieldsDataTable(DataTable fieldsDataTable)
        {
            List<string> optionsAttributes = new List<string>
            {
                String.Empty
            };
            optionsAttributes.AddRange(_profilesAttributes);
            foreach (SPField field in _listFields)
            {
                if (field.ReadOnlyField || field.Hidden)
                    continue;
                List<object> dataRow = new List<object> { };
                // Order should be same as in AddColumnsToDataTable
                string fieldTitle = field.Title;
                string fieldInternalName = field.InternalName;
                var attributeForFieldFromConf = _TJListConf.AttributesFieldsMap
                    ?.FirstOrDefault(p => p.Value == fieldInternalName)
                    .Key;
                string selectAttributeForField = String.IsNullOrEmpty(attributeForFieldFromConf) ? String.Empty : attributeForFieldFromConf;
                Array optionsAttributesArray = optionsAttributes.Union(new List<string> { selectAttributeForField }).ToArray();
                dataRow.Add(fieldTitle);
                dataRow.Add(fieldInternalName);
                dataRow.Add(selectAttributeForField);
                dataRow.Add(optionsAttributesArray);
                fieldsDataTable.Rows.Add(dataRow.ToArray());
            };
        }
        #endregion

        #region SaveData From Page to SPList
        private void GetAdditionalParamsFromPageToERConf()
        {
            string userFieldInternalName = _pageSPList.Fields.GetField(UserFieldDropDownList.SelectedValue).InternalName;
            _TJListConf.UserField = userFieldInternalName;
            _TJListConf.Enable = EnableCheckBox.Checked;
        }
        private void GetFieldsParamsFromPageToERConf()
        {
            var fieldsTableRows = FieldsTable.Rows;
            Dictionary<string, string> attributesFieldsMap = new Dictionary<string, string>();
            foreach (GridViewRow row in fieldsTableRows)
            {
                var fieldTitleCell = row.Cells[0];
                string fieldName = ((Label)(fieldTitleCell.FindControl("FieldLabel"))).Text;
                string fieldInternalName = ((Label)(fieldTitleCell.FindControl("FieldLabel"))).Attributes["data-fieldInternalName"].ToString();
                var attributeCell = row.Cells[1];
                string attributeForField = ((DropDownList)(attributeCell.FindControl("DropDownList1"))).SelectedValue;
                if (String.IsNullOrEmpty(attributeForField))
                    continue;
                attributesFieldsMap.Add(attributeForField, fieldInternalName);
            }
            _TJListConf.AttributesFieldsMap = attributesFieldsMap;
        }

        private void SaveERConfToListPropertyBag()
        {
            PropertyBagConfHelper<ListConfigUpdateUserFields>.Set(_pageSPList.RootFolder.Properties, CommonConstants.LIST_PROPERTY_JSON_CONF, _TJListConf);
            _pageSPList.Update();
        }
        #endregion
        protected void ButtonOK_EventHandler(object sender, EventArgs e)
        {
            GetAdditionalParamsFromPageToERConf();
            GetFieldsParamsFromPageToERConf();
            SaveERConfToListPropertyBag();
            RedirectToParentPage();
        }
        protected void ButtonCANCEL_EventHandler(object sender, EventArgs e)
        {
            RedirectToParentPage();
        }

        //TODO: move to common lib
        private void RedirectToParentPage()
        {
            string listSettingsUrl = Regex.Replace(
                HttpContext.Current.Request.RawUrl, 
                "ListsUpdateUserFieldsTimerJob/ListConfiguration", "listedit", RegexOptions.IgnoreCase
            );
            Response.Redirect(listSettingsUrl);
        }
    }
}
