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
        //private PropertyInfo[] ERConfProperties;
        private string CurrentUrl;
        protected void Page_Load(object sender, EventArgs e)
        {
            InitParams();
            if (IsPostBack)
                return;
            BindData();
        }
        private void InitParams()
        {
            Guid listGuid = new Guid(Request.QueryString["List"]);
            _pageSPList = Util.GetSPList(listGuid);
            _listFields = _pageSPList.Fields;
            _profilesAttributes = GetProfilesAttributes(); //new ProfilesChangesManager().GetProfilesAttributes();
            _ERConf = SPJsonConf<ConfListUserChanges>.Get(_pageSPList, CommonConfig.LIST_PROPERTY_JSON_CONF);
            CurrentUrl = HttpContext.Current.Request.RawUrl;
            //ERConfProperties = ERConf.GetType().GetProperties();
        }

        private List<string> GetProfilesAttributes()
        {
            SPWeb web = SPControl.GetContextWeb(HttpContext.Current);
            AppSettingsSection appSettings = WebPartsHelper.GetWebAppSettings(web);
            string profilesAttributesInSettings = appSettings.Settings["ProfilesAttributes"].Value;
            List<string> profilesAttributes = profilesAttributesInSettings.Split(',').ToList();
            return profilesAttributes;
        }
        private void BindData()
        {
            BindDataToAdditionalTable();
            FieldsTable.DataSource = GetDataForFieldsTable();
            FieldsTable.DataBind();
        }

        private void BindDataToAdditionalTable()
        {
            BindDataToUserField();
        }
        private void BindDataToUserField()
        {
            List<string> personFields = _listFields.Cast<SPField>().ToList()
                .Where(f => !f.Hidden && f.TypeAsString.Contains("User"))
                .Select(f => f.Title)
                .ToList();
            personFields.Add(String.Empty);
            personFields.Sort();
            UserFieldDropDownList.DataSource = personFields;
            UserFieldDropDownList.DataBind();
            UserFieldDropDownList.SelectedValue = String.Empty;
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
            List<string> optionsAttributes = new List<string>();
            optionsAttributes.Add(String.Empty); //"-- select --"
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
                //string attributeForField = String.Empty;
                //if (_ERConf.AttributesFieldsMap != null)
                //    attributeForField = _ERConf.AttributesFieldsMap.Where(m => m.Value == fieldTitle).Select(m => m.Value).First();
                string attributeForField = String.Empty; //"Department"; //
                dataRow.Add(attributeForField);
                dataRow.Add(optionsAttributes.ToArray());
                fieldsDataTable.Rows.Add(dataRow.ToArray());
            };
        }

        //private void GetFieldsParamsFromPageToConf()
        //{
        //    var fieldsTableRows = FieldsTable.Rows;
        //    var headerCount = FieldsTable.HeaderRow.Cells.Count;

        //    for (int i = 1; i < headerCount; i++)
        //    {
        //        List<string> valueList = new List<string> { };
        //        string ctrId = "";
        //        foreach (GridViewRow row in fieldsTableRows)
        //        {
        //            var cellLabel = row.Cells[0];
        //            var fieldName = ((Label)(cellLabel.FindControl("FieldLabel"))).Text;

        //            var cell = row.Cells[i];
        //            var cellControls = cell.Controls;
        //            foreach (var ctr in cellControls)
        //            {
        //                if (ctr is CheckBox box)
        //                {
        //                    ctrId = box.ID;
        //                    if (box.Checked)
        //                    {
        //                        valueList.Add(fieldName);
        //                    }
        //                }
        //            }
        //        }
        //        ERConf.GetType().GetProperty(ctrId)?.SetValue(ERConf, valueList);
        //    }
        //}
        protected void ButtonOK_EventHandler(object sender, EventArgs e)
        {
            //GetAdditionalParamsFromPageToConf();
            //GetFieldsParamsFromPageToConf();
            //SPJsonConf<ConfListUserChanges>.Set(PageSPList, CommonConfigNotif.LIST_PROPERTY_JSON_CONF, ERConf);

            RedirectToParentPage();
        }
        protected void ButtonCANCEL_EventHandler(object sender, EventArgs e)
        {
            RedirectToParentPage();
        }

        //TODO: move to common lib
        private void RedirectToParentPage()
        {
            string listSettingsUrl = Regex.Replace(CurrentUrl, "ERListsSettings/Notifications", "listedit", RegexOptions.IgnoreCase);
            Response.Redirect(listSettingsUrl);
        }
    }
}
