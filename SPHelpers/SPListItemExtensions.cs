using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob.SPHelpers
{
    static class SPListItemExtensions
    {
        public static void UpdateByNewValues(this SPListItem item, Dictionary<string, object> fieldsValuesMap)
        {
            fieldsValuesMap
                .ToList()
                .ForEach(p =>
                {
                    var fieldName = p.Key;
                    try
                    {
                        item[fieldName] = p.Value;
                    }
                    catch (Exception ex)
                    {
                        var message = String.Format(CommonConstants.ERROR_MESSAGE_TEMPLATE, item.ParentList.ID, item.ID, ex.ToString());
                        SPLogger.WriteLog(SPLogger.Category.Unexpected, "Item FieldValue Error", message);
                        return;
                    }
                });
            using (new DisableItemEvents())
            {
                item.SystemUpdate();
            }
        }
    }
}
