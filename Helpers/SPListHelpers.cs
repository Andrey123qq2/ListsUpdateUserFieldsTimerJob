using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    static class SPListHelpers
    {
        private static Dictionary<CAMLQueryType, string> _camlQueryTemplateToTypesMap = new Dictionary<CAMLQueryType, string> {
            { CAMLQueryType.User, @"<Where><Eq><FieldRef Name='{0}' LookupId='True' /><Value Type = 'User'>{1}</Value></Eq></Where>" },
            { CAMLQueryType.Text, @"<Where><Eq><FieldRef Name='{0}'/><Value Type = 'Text'>{1}</Value></Eq></Where>"}
        };
        //TODO: move to common lib
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
    }
}
