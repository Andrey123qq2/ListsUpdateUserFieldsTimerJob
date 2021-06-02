using ListsUpdateUserFieldsTimerJob.SPHelpers;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob.Strategies
{
    class TimerJobReport : ISPListModifierStrategy
    {
        private SPListToModifyContext _listContext;
        private readonly string _reportWebUrl;
        private readonly string _reportLibraryName;
        private readonly string _reportFileFullPath;
        private string _listContextUserField;
        private DataTable _CSVReportTable = new DataTable();
        public TimerJobReport(string webUrl, string libraryName, string filePathTemplate)
        {
            _reportWebUrl = webUrl;
            _reportLibraryName = libraryName;
            _reportFileFullPath = String.Format(
                filePathTemplate, 
                DateTime.Now.ToString("yyyyMMdd-HHmmss")
            );
            _CSVReportTable.Columns.Add("Title");
            _CSVReportTable.Columns.Add("UserField");
            _CSVReportTable.Columns.Add("UserLogin");
            _CSVReportTable.Columns.Add("FieldsNewValues");
            _CSVReportTable.Columns.Add("ItemUrl");
        }
        public void Execute(SPListToModifyContext context)
        {
            if (context == null || !context.TJListConf.Enable)
                return;
            _listContext = context;
            _listContextUserField = _listContext.CurrentList.Fields.GetFieldByInternalName(_listContext.TJListConf.UserField).Title;
            AddRowsToCSVReportTable();
        }
        public void SaveReport()
        {
            ExportDataTableToCSV();
            SaveReportToSPLib();
        }
        private void SaveReportToSPLib()
        {
            SPListHelpers.SaveFileToSPLib(
                _listContext.CurrentList.ParentWeb.Site.OpenWeb(_reportWebUrl),
                _reportLibraryName,
                String.Format(_reportFileFullPath)
            );
        }

        private void ExportDataTableToCSV()
        {
            _CSVReportTable.ToCSV(_reportFileFullPath);
        }
        private void AddRowsToCSVReportTable()
        {
            _listContext.UsersItemsAndProfileChanges
                .ForEach(i => AddItemRowToCSVReportTable(i));
        }
        private void AddItemRowToCSVReportTable(UserItemsAndNewFieldsValues item)
        {
            string userLogin = item.UserLogin;
            string itemUrlBase = _listContext.CurrentList.ParentWeb.Site.Url + _listContext.CurrentList.DefaultDisplayFormUrl + "?ID=";
            string fieldsNewValuesString = FieldsNewValuesToString(item.FieldsNewValues);
            item.ListItems
                .Cast<SPListItem>()
                .ToList()
                .ForEach(i =>
                    {
                        string itemUrl = itemUrlBase + i.ID;
                        _CSVReportTable.Rows.Add(i.Title, _listContextUserField, userLogin, fieldsNewValuesString, itemUrl);
                    });
        }

        private string FieldsNewValuesToString(Dictionary<string, object> fieldsNewValues)
        {
            string stringsFormat = "{0}={1}";
            string[] fieldsNewValuesArray = fieldsNewValues
                .Select(p =>
                    String.Format(
                        stringsFormat,
                        p.Key,
                        p.Value?.ToString()
                    )
                )
                .ToArray();
            string profileChangesString = "\"" + string.Join(";", fieldsNewValuesArray) + "\"";
            return profileChangesString;
        }
    }
}