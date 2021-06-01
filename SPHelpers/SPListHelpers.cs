using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob.SPHelpers
{
    public static class SPListHelpers
    {
        private static readonly Dictionary<CAMLQueryType, string> _camlQueryTemplateToTypesMap = new Dictionary<CAMLQueryType, string> {
            { CAMLQueryType.User, @"<Where><Eq><FieldRef Name='{0}' LookupId='True' /><Value Type = 'User'>{1}</Value></Eq></Where>" },
            { CAMLQueryType.Text, @"<Where><Eq><FieldRef Name='{0}'/><Value Type = 'Text'>{1}</Value></Eq></Where>"}
        };
        public static void SaveFileToSPLib(SPWeb web, string libraryName, string fileFullPath)
        {
            bool replaceExistingFiles = true;
            if (!System.IO.File.Exists(fileFullPath))
                throw new FileNotFoundException("File not found.", fileFullPath);
            SPFolder reportLibrary = web.GetFolder(libraryName);
            string fileName = System.IO.Path.GetFileName(fileFullPath);
            FileStream fileStream = File.OpenRead(fileFullPath);
            reportLibrary.Files.Add(fileName, fileStream, replaceExistingFiles);
        }
        public static SPList GetSPList(string listUrl)
        {
            SPSite site = new SPSite(listUrl);
            SPWeb web = site.OpenWeb();
            SPList list = web.GetList(listUrl);

            return list;
        }
        public static SPList GetSPList(string webUrl, Guid listGUID)
        {
            SPList list;
            using (SPSite site = new SPSite(webUrl))
            using (SPWeb web = site.OpenWeb())
            {
                list = web.Lists[listGUID];
            }
            return list;
        }
        public static SPList GetSPList(string siteUrl, Guid webGUID, Guid listGUID)
        {
            SPList list;
            using (SPSite site = new SPSite(siteUrl))
            {
                var web = site.OpenWeb(webGUID);
                list = web.Lists[listGUID];
            }
            return list;
        }
        public static SPListItemCollection QueryItems(
            this SPList list, 
            string fieldInternalName, 
            string fieldValue, 
            CAMLQueryType mode = CAMLQueryType.Text
        )
        {
            string camlQueryTemplate = _camlQueryTemplateToTypesMap[mode];
            string camlQueryText = String.Format(camlQueryTemplate, fieldInternalName, fieldValue);
            SPQuery spQuery = new SPQuery
            {
                Query = camlQueryText
            };
            SPListItemCollection items = list.GetItems(spQuery);
            return items;
        }
        public static SPListItemCollection QueryItems(
            this SPList list,
            string queryFitlerString
        )
        {
            SPQuery spQuery = new SPQuery
            {
                Query = queryFitlerString
            };
            SPListItemCollection items = list.GetItems(spQuery);
            return items;
        }
    }
}