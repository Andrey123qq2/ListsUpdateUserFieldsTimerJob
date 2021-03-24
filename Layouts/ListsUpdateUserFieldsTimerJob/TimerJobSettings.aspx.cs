﻿using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using SPSCommon.SPJsonConf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using SPHelpers;
using System.Web.UI.WebControls;

namespace ListsUpdateUserFieldsTimerJob.Layouts.ListsUpdateUserFieldsTimerJob
{
    public partial class TimerJobSettings : LayoutsPageBase
    {
        private SPSite _currentSite;
        private SPJobDefinition _timerJob;
        private TimerJobConfig _TJConf;
        private List<string> _profilesAttributes;
        protected void Page_Load(object sender, EventArgs e)
        {
            InitParams();
            if (IsPostBack)
                return;
            //BindDataToAdditionalTable();
            BindDataToAttributesTable();
        }

        private void InitParams()
        {
            _currentSite = SPContext.Current.Site;
            _timerJob = _currentSite.WebApplication.JobDefinitions.FirstOrDefault(n => n.Name == CommonConstants.TIMER_JOB_NAME);
            _TJConf = PropertyBagConf<TimerJobConfig>.Get(_timerJob.Properties, CommonConstants.LIST_PROPERTY_JSON_CONF);
            _profilesAttributes = GetProfilesAttributes();
        }
        private List<string> GetProfilesAttributes()
        {
            SPServiceContext context = SPServiceContext.GetContext(_currentSite);
            ProfileSubtypeManager psm = ProfileSubtypeManager.Get(context);
            ProfileSubtype ps = psm.GetProfileSubtype(ProfileSubtypeManager.GetDefaultProfileName(ProfileType.User));
            ProfileSubtypePropertyManager pspm = ps.Properties;
            List<string> profilesAttributes = pspm.PropertiesWithSection
                .Cast<ProfileSubtypeProperty>()
                .Select(p => p.Name)
                .ToList();
            return profilesAttributes;
        }

        #region BindData to Page
        //private void BindDataToAdditionalTable()
        //{
        //    EnableCheckBox.Checked = !String.IsNullOrEmpty(_TJConf.SiteUrl);
        //}
        private void BindDataToAttributesTable()
        {
            AttributesTable.DataSource = GetDataForAttributesTable();
            AttributesTable.DataBind();
        }
        private DataTable GetDataForAttributesTable()
        {
            var fieldsDataTable = new DataTable();
            fieldsDataTable.Columns
                .AddRange(GetColumnsToFieldsDataTable().ToArray());
            GetRowsForFieldsDataTable()
                .ForEach(r => fieldsDataTable.Rows.Add(r.ToArray()));
            return fieldsDataTable;
        }
        private List<DataColumn> GetColumnsToFieldsDataTable()
        {
            var dataColumns = new List<DataColumn> {
                new DataColumn("AttributeName", typeof(string)),
                new DataColumn("AttributesOptInLists", typeof(bool))
            };
            return dataColumns;
        }
        private List<List<object>> GetRowsForFieldsDataTable()
        {
            var tableRows = new List<List<object>>();
            foreach (string attribute in _profilesAttributes)
            {
                List<object> dataRow = new List<object>();
                // Order should be same as in AttributesTable
                dataRow.Add(attribute);
                bool attributeInConf = _TJConf.AttributesOptInLists != null && _TJConf.AttributesOptInLists.Contains(attribute);
                dataRow.Add(attributeInConf);
                tableRows.Add(dataRow);
            };
            return tableRows;
        }
        #endregion

        #region SaveData From Page to PropertyBag
        //private void GetAdditionalParamsFromPageToTJConf()
        //{
        //    _TJConf.SiteUrl = EnableCheckBox.Checked ? _currentSite.Url : String.Empty;
        //}
        private void GetAttributesParamsFromPageToTJConf()
        {
            var attributesTableRows = AttributesTable.Rows;
            var attributesOptInLists = new List<string>();
            foreach (GridViewRow row in attributesTableRows)
            {
                var cellLabel = row.Cells[0];
                var attributeName = ((Label)(cellLabel.FindControl("AttributeLabel"))).Text;
                var cellControl = row.Cells[1];
                var attributeChecked = ((CheckBox)(cellControl.FindControl("AttributesOptInLists"))).Checked;
                if (attributeChecked)
                    attributesOptInLists.Add(attributeName);
            }
            _TJConf.AttributesOptInLists = attributesOptInLists;
        }

        private void SaveTJConfToPropertyBag()
        {
            PropertyBagConf<TimerJobConfig>.Set(_timerJob.Properties, CommonConstants.LIST_PROPERTY_JSON_CONF, _TJConf);
            _timerJob.Update();
        }
        #endregion
        protected void ButtonOK_EventHandler(object sender, EventArgs e)
        {
            //GetAdditionalParamsFromPageToTJConf();
            GetAttributesParamsFromPageToTJConf();
            SaveTJConfToPropertyBag();
            RedirectToPreviousPageBySource();
        }
        protected void ButtonCANCEL_EventHandler(object sender, EventArgs e)
        {
            RedirectToPreviousPageBySource();
        }

        //TODO: move to common lib
        private void RedirectToPreviousPageBySource()
        {
            string sourceUrl = Context.Request.QueryString["Source"];
            string previousUrl = String.IsNullOrEmpty(sourceUrl) ? SPContext.Current.Web.Url : sourceUrl;
            Response.Redirect(previousUrl);
        }
    }
}