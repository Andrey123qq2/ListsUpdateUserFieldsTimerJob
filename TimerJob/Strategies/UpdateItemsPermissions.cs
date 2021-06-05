using ListsUpdatePermissions;
using ListsUpdatePermissions.SPHelpers;
using ListsUpdateUserFieldsTimerJob.SPHelpers;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob.Strategies
{
    class UpdateItemsPermissions : ISPListModifierStrategy
    {
        private SPListToModifyContext _listContext;
        ERConfPermissions _listPermConf;
        List<SPListItem> _allItemsToProcess;
        public void Execute(SPListToModifyContext context)
        {
            if (context == null || !context.TJListConf.Enable || context.DisableUpdatePermissions)
                return;
            _listContext = context;
            _listPermConf = SPJsonConf<ERConfPermissions>.Get(_listContext.CurrentList, CommonConstants.LIST_PROPERTY_PERM_JSON_CONF);
            _allItemsToProcess = _listContext.UsersItemsAndProfileChanges
                .SelectMany(i => i.ListItems.Cast<SPListItem>().ToList())
                .ToList();
            _allItemsToProcess.ForEach(i => UpdatePermissions(i));
        }
        private void UpdatePermissions(SPListItem item)
        {
            try
            {
                var listItemUpdatePermissions = new ListItemUpdatePermissions(item, _listPermConf);
                listItemUpdatePermissions.UpdatePermissions();
            }
            catch (Exception ex)
            {
                var message = String.Format(CommonConstants.ERROR_MESSAGE_TEMPLATE, item.ParentList.ID, item.ID, ex.ToString());
                SPLogger.WriteLog(SPLogger.Category.Unexpected, "UpdatePermissions Error", message);
                return;
            }
        }
    }
}
