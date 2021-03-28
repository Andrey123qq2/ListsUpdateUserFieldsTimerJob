using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob.SPHelpers
{
    static class SPSiteHelpers
    {
        public static List<SPList> GetListsWithJSONConf(this SPSite site, string confFilter)
        {
            var listsWithJSONConf = new List<SPList>();
            site.AllWebs.Cast<SPWeb>().ToList().ForEach(w =>
            {
                w.Lists.Cast<SPList>().ToList()
                .Where(l => l.RootFolder.Properties.Contains(confFilter))
                .ToList()
                .ForEach(l => listsWithJSONConf.Add(l));
            });
            return listsWithJSONConf;
        }
    }
}
