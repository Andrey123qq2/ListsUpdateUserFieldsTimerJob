using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListsUpdateUserFieldsTimerJob.SPHelpers;

namespace ListsUpdateUserFieldsTimerJob.Strategies
{
    public class UpdateUserFieldsByProfileChanges : ISPListModifierStrategy
    {
        private SPListToModifyContext _listContext;
        public void Execute(SPListToModifyContext context)
        {
            if (context == null || !context.TJListConf.Enable)
                return;
            _listContext = context;
            _listContext.UsersItemsAndProfileChanges.ForEach(i => UpdateUserItemsByChanges(i));
        }

        private void UpdateUserItemsByChanges(UserItemsAndProfileChanges item)
        {
            item.ListItems
                .Cast<SPListItem>()
                .ToList()
                .ForEach(i => i.UpdateByNewValues(item.FieldsNewValues));
        }
    }
}