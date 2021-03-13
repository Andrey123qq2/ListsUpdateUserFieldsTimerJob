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
    class SPListToModifyContext : IDisposable, ISPListContext
    {
        public SPList CurrentList { get; }
        public ConfListUserChanges ERConf { get; }
        private SPWeb _web;
        private ISPListModifierStrategy _modifierStrategy;
        
        public SPListToModifyContext(SPList list, string confPopertyName)
        {
            CurrentList = list;
            ERConf = SPJsonConf<ConfListUserChanges>.Get(list, confPopertyName);
        }

        public void SetStrategy(ISPListModifierStrategy strategy)
        {
            _modifierStrategy = strategy;
        }

        public void UpdateListItems()
        {
            _modifierStrategy.UpdateItems(this);
        }

        public void Dispose()
        {
            _web.Dispose();
        }

        public static List<SPListToModifyContext> Factory()
        {
            string confName = CommonConfig.LIST_PROPERTY_JSON_CONF;
            List<SPListToModifyContext> listsToChange = Util.GetListsWithJSONConf(confName)
                .Select(l => new SPListToModifyContext(l, confName))
                .ToList();
            return listsToChange;
        }
    }
}
