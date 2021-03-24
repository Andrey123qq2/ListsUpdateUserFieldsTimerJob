using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPHelpers
{
    static class Util
    {
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
