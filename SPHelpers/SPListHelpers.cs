using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPHelpers
{
    static class SPListHelpers
    {
        private static readonly Dictionary<CAMLQueryType, string> _camlQueryTemplateToTypesMap = new Dictionary<CAMLQueryType, string> {
            { CAMLQueryType.User, @"<Where><Eq><FieldRef Name='{0}' LookupId='True' /><Value Type = 'User'>{1}</Value></Eq></Where>" },
            { CAMLQueryType.Text, @"<Where><Eq><FieldRef Name='{0}'/><Value Type = 'Text'>{1}</Value></Eq></Where>"}
        };
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
        public static List<SPList> GetListsWithJSONConf(string siteUrl, string confFilter)
        {
            var listsWithJSONConf = new List<SPList>();
            using (SPSite site = new SPSite(siteUrl))
            {
                site.AllWebs.Cast<SPWeb>().ToList().ForEach(w =>
                {
                    w.Lists.Cast<SPList>().ToList()
                    .Where(l => l.RootFolder.Properties.Contains(confFilter))
                    .ToList()
                    .ForEach(l => listsWithJSONConf.Add(l));
                });
            }
            return listsWithJSONConf;
        }
    }
}