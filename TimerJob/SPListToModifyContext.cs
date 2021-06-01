using ListsUpdateUserFieldsTimerJob.SPHelpers;
using ListsUpdateUserFieldsTimerJob.Strategies;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    public class SPListToModifyContext // : ISPListContext
    {
        public SPList CurrentList { get; }
        public ListConfigUpdateUserFields TJListConf { get; }
        public List<UserItemsAndProfileChanges> UsersItemsAndProfileChanges;

        public UserProfileManagerWrapper ProfilesChangesManager;
        private ISPListModifierStrategy _modifierStrategy;
        
        public SPListToModifyContext(SPList list, string confPopertyName, UserProfileManagerWrapper profilesChangesManager)
        {
            CurrentList = list;
            TJListConf = PropertyBagConfHelper<ListConfigUpdateUserFields>.Get(list.RootFolder.Properties, confPopertyName);
            ProfilesChangesManager = profilesChangesManager;
        }

        public void SetStrategy(ISPListModifierStrategy strategy)
        {
            _modifierStrategy = strategy;
        }
        public void ExecuteStrategy()
        {
            if (_modifierStrategy == null)
                return;
            _modifierStrategy.Execute(this);
        }
        public static List<SPListToModifyContext> Factory(SPSite site)
        {
            var profilesChangesManager = new UserProfileManagerWrapper(site, CommonConstants.CHANGE_MANAGER_DAYS_TO_CHECK);
            List<SPListToModifyContext> listsToChange = site.GetListsWithJSONConf(CommonConstants.LIST_PROPERTY_JSON_CONF)
                .Select(l => new SPListToModifyContext(l, CommonConstants.LIST_PROPERTY_JSON_CONF, profilesChangesManager))
                .ToList();
            return listsToChange;
        }
    }
}