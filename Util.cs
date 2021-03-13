using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    class Util
    {
        public static List<SPList> GetListsWithJSONConf(string confFilter)
        {
            throw new NotImplementedException();
        }

        //TODO: move to common lib
        public static SPList GetSPList(Guid listGUID)
        {
            SPList list;

            using (SPSite site = new SPSite(SPContext.Current.Web.Url))
            using (SPWeb web = site.OpenWeb())
            {
                list = web.Lists[listGUID];
            }

            return list;
        }

    }
}
