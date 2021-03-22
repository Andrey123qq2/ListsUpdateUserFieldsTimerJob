using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPSCommon.SPJsonConf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Linq;
using System.Configuration;
using SPWebPartsCommon;

namespace ListsUpdateUserFieldsTimerJob.Layouts.ListsUpdateUserFieldsTimerJob
{
    public partial class ListConfiguration : LayoutsPageBase
    {
        private SPList _pageSPList;
        private ConfListUserChanges _ERConf;
        private SPFieldCollection _listFields;
        private List<string> _profilesAttributes;
        private string CurrentUrl;
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
            _pageSPList = Util.GetSPList(listGuid);
            _listFields = _pageSPList.Fields;
            _profilesAttributes = GetProfilesAttributes();
            _ERConf = SPJsonConf<ConfListUserChanges>.Get(_pageSPList, CommonConfig.LIST_PROPERTY_JSON_CONF);
            CurrentUrl = HttpContext.Current.Request.RawUrl;
        }
        private List<string> GetProfilesAttributes()
        {
            SPWeb web = SPControl.GetContextWeb(HttpContext.Current);
            AppSettingsSection appSettings = WebPartsHelper.GetWebAppSettings(web);
            string profilesAttributesInSettings = appSettings.Settings["ProfilesAttributes"].Value;
            List<string> profilesAttributes = profilesAttributesInSettings.Split(',').ToList();
            return profilesAttributes;
        }

        #region BindData to Page
        private void BindDataToAdditionalTable()
        {
            BindDataToUserField();
            BindDataToEnableCheckBox();
        }
        private void BindDataToUserField()
        {
            List<string> personFields = _listFields
                .Cast<SPField>().ToList()
                .Where(f => !f.Hidden && f.TypeAsString.Contains("User"))
                .Select(f => f.Title)
                .ToList();
            personFields.Add(String.Empty);
            personFields.Sort();
            UserFieldDropDownList.DataSource = personFields;
            UserFieldDropDownList.DataBind();
            UserFieldDropDownList.SelectedValue = String.IsNullOrEmpty(_ERConf.UserField) ? String.Empty : _ERConf.UserField;
        }
        private void BindDataToEnableCheckBox()
        {
            EnableCheckBox.Checked = _ERConf.Enable;
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
                // data for columns FieldName, Attribute, AttributesList
                string fieldTitle = field.Title;
                dataRow.Add(fieldTitle);
                var ERConfattributeForField = _ERConf.AttributesFieldsMap
                    ?.FirstOrDefault(p => p.Key == fieldTitle)
                    .Value;
                string attributeForField = String.IsNullOrEmpty(ERConfattributeForField) ? String.Empty : ERConfattributeForField;
                dataRow.Add(attributeForField);
                dataRow.Add(optionsAttributes.ToArray());
                fieldsDataTable.Rows.Add(dataRow.ToArray());
            };
        }
        #endregion

        #region SaveData From Page to SPList
        private void GetAdditionalParamsFromPageToERConf()
        {
            _ERConf.UserField = UserFieldDropDownList.SelectedValue;
            _ERConf.Enable = EnableCheckBox.Checked;
        }
        private void GetFieldsParamsFromPageToERConf()
        {
            var fieldsTableRows = FieldsTable.Rows;
            Dictionary<string, string> attributesFieldsMap = new Dictionary<string, string>();

            foreach (GridViewRow row in fieldsTableRows)
            {
                var fieldTitleCell = row.Cells[0];
                string fieldName = ((Label)(fieldTitleCell.FindControl("FieldLabel"))).Text;
                var attributeCell = row.Cells[1];
                string attributeForField = ((DropDownList)(attributeCell.FindControl("DropDownList1"))).SelectedValue;
                if (String.IsNullOrEmpty(attributeForField))
                    continue;
                attributesFieldsMap.Add(fieldName, attributeForField);
            }
            _ERConf.AttributesFieldsMap = attributesFieldsMap;
        }

        private void SaveERConfToListPropertyBag()
        {
            SPJsonConf<ConfListUserChanges>.Set(_pageSPList, CommonConfig.LIST_PROPERTY_JSON_CONF, _ERConf);
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
            string listSettingsUrl = Regex.Replace(CurrentUrl, "ListsUpdateUserFieldsTimerJob/ListConfiguration", "listedit", RegexOptions.IgnoreCase);
            Response.Redirect(listSettingsUrl);
        }
    }
}
