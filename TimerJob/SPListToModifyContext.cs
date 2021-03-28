using ListsUpdateUserFieldsTimerJob.SPHelpers;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using SPHelpers;
using SPSCommon.SPJsonConf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    public class SPListToModifyContext : ISPListContext
    {
        public SPList CurrentList { get; }
        public ListConfigUpdateUserFields TJListConf { get; }
        private static ISPListModifierStrategy _modifierStrategy;
        
        public SPListToModifyContext(SPList list, string confPopertyName)
        {
            CurrentList = list;
            TJListConf = PropertyBagConfHelper<ListConfigUpdateUserFields>.Get(list.RootFolder.Properties, confPopertyName);
        }

        public static void SetStrategy(ISPListModifierStrategy strategy)
        {
            _modifierStrategy = strategy;
        }

        public void UpdateListItems()
        {
            if (_modifierStrategy == null)
                return;
            _modifierStrategy.UpdateItems(this);
        }

        public static List<SPListToModifyContext> Factory(SPSite site)
        {
            List<SPListToModifyContext> listsToChange = site.GetListsWithJSONConf(CommonConstants.LIST_PROPERTY_JSON_CONF)
                .Select(l => new SPListToModifyContext(l, CommonConstants.LIST_PROPERTY_JSON_CONF))
                .ToList();
            return listsToChange;
        }
    }
}