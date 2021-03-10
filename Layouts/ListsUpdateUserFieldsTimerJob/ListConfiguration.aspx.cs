using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPSCommon.SPJsonConf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace ListsUpdateUserFieldsTimerJob.Layouts.ListsUpdateUserFieldsTimerJob
{
    public partial class ListConfiguration : LayoutsPageBase
    {
        private SPList PageSPList;
        private ConfListUserChanges ERConf;
        private SPFieldCollection ListFields;
        private PropertyInfo[] ERConfProperties;
        private string CurrentUrl;
        private string MailTemplatesUrl;
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
            PageSPList = GetSPList(listGuid);
            ListFields = PageSPList.Fields;
            ERConf = SPJsonConf<ConfListUserChanges>.Get(PageSPList, CommonConfigNotif.LIST_PROPERTY_JSON_CONF);
            ERConfProperties = ERConf.GetType().GetProperties();
            CurrentUrl = HttpContext.Current.Request.RawUrl;
        }
        private void BindData()
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
            fieldsDataTable.Columns.Add(new DataColumn("Attribute", typeof(bool)));

            foreach (var prop in ERConfProperties)
            {
                if (prop.PropertyType != typeof(List<string>))
                    continue;

                fieldsDataTable.Columns.Add(new DataColumn(prop.Name, typeof(bool))); // change to dropdown
            }

        }
        private void AddDataToFieldsDataTable(DataTable fieldsDataTable)
        {
            foreach (SPField field in ListFields)
            {
                if (field.ReadOnlyField || field.Hidden)
                    continue;

                List<object> dataRow = new List<object> { };
                string fieldTitle = field.Title;
                // Order should be same as in AddColumnsToDataTable
                // data for column FieldName
                dataRow.Add(fieldTitle);
                // data for column MailTemplatesUrl
                //attributes = GetProfilesAttributes(); // get as static singleton
                dataRow.Add(attributes);
                // data for column UserField

                fieldsDataTable.Rows.Add(dataRow.ToArray());
            };
        }

        private void GetFieldsParamsFromPageToConf()
        {
            var fieldsTableRows = FieldsTable.Rows;
            var headerCount = FieldsTable.HeaderRow.Cells.Count;

            for (int i = 1; i < headerCount; i++)
            {
                List<string> valueList = new List<string> { };
                string ctrId = "";
                foreach (GridViewRow row in fieldsTableRows)
                {
                    var cellLabel = row.Cells[0];
                    var fieldName = ((Label)(cellLabel.FindControl("FieldLabel"))).Text;

                    var cell = row.Cells[i];
                    var cellControls = cell.Controls;
                    foreach (var ctr in cellControls)
                    {
                        if (ctr is CheckBox box)
                        {
                            ctrId = box.ID;
                            if (box.Checked)
                            {
                                valueList.Add(fieldName);
                            }
                        }
                    }
                }
                ERConf.GetType().GetProperty(ctrId)?.SetValue(ERConf, valueList);
            }
        }
        protected void ButtonOK_EventHandler(object sender, EventArgs e)
        {
            GetAdditionalParamsFromPageToConf();
            GetFieldsParamsFromPageToConf();
            SPJsonConf<ConfListUserChanges>.Set(PageSPList, CommonConfigNotif.LIST_PROPERTY_JSON_CONF, ERConf);

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
