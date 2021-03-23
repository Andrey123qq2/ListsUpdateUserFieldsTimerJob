using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using SPSCommon.SPJsonConf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    class SPListToModifyContext : ISPListContext
    {
        public SPList CurrentList { get; }
        public ListConfigUpdateUserFields ERConf { get; }
        private ISPListModifierStrategy _modifierStrategy;
        
        public SPListToModifyContext(SPList list, string confPopertyName)
        {
            CurrentList = list;
            ERConf = SPJsonConf<ListConfigUpdateUserFields>.Get(list, confPopertyName);
        }

        public void SetStrategy(ISPListModifierStrategy strategy)
        {
            _modifierStrategy = strategy;
        }

        public void UpdateListItems()
        {
            _modifierStrategy.UpdateItems(this);
        }

        public static List<SPListToModifyContext> Factory()
        {
            List<SPListToModifyContext> listsToChange = Util.GetListsWithJSONConf(CommonConstants.SITE_URL, CommonConstants.LIST_PROPERTY_JSON_CONF)
                .Select(l => new SPListToModifyContext(l, CommonConstants.LIST_PROPERTY_JSON_CONF))
                .ToList();
            return listsToChange;
        }
    }
}
