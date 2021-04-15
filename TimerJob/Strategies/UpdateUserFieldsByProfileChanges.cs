﻿using Microsoft.Office.Server.UserProfiles;
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
            //Dictionary<string, object> fieldsNewValuesMap = GetFieldsNewValuesMap(item.ProfileChanges);
            //UpdateUserItems(item.ListItems, item.FieldsNewValues);
            item.ListItems
                .Cast<SPListItem>()
                .ToList()
                .ForEach(i => i.UpdateByNewValues(item.FieldsNewValues));
        }

        //#region ProfileChanges processing methods
        //private Dictionary<string, object> GetFieldsNewValuesMap(List<UserProfileChange> changedProperties)
        //{
        //    Dictionary<string, object> fieldsNewValuesMap = changedProperties
        //        .Where(c => _listContext.TJListConf.AttributesFieldsMap.ContainsKey(((UserProfileSingleValueChange)c).ProfileProperty.Name))
        //        .OrderByDescending(c => c.EventTime)
        //        .GroupBy(c => ((UserProfileSingleValueChange)c).ProfileProperty.Name)
        //        .Select(g => g.First())
        //        .ToDictionary(
        //            c => _listContext.TJListConf.AttributesFieldsMap[((UserProfileSingleValueChange)c).ProfileProperty.Name],
        //            c => GetFieldValueFromProfileChange(c)
        //        );
        //    return fieldsNewValuesMap;
        //}

        //private object GetFieldValueFromProfileChange(UserProfileChange profileChange)
        //{
        //    object fieldNewValue;
        //    string changedPropertyName = ((UserProfileSingleValueChange)profileChange).ProfileProperty.Name;
        //    string listFieldName = _listContext.TJListConf.AttributesFieldsMap[changedPropertyName];
        //    SPField listField = _listContext.CurrentList.Fields.GetField(listFieldName);
        //    string listFieldTypeName = listField.TypeAsString;
        //    var profileNewValue = ((UserProfileSingleValueChange)profileChange).NewValue;
        //    if (listFieldTypeName.Contains("User"))
        //    {
        //        fieldNewValue = _listContext.CurrentList.ParentWeb.EnsureUser((string)profileNewValue);
        //    }
        //    else if (listFieldTypeName.Contains("Lookup"))
        //    {
        //        fieldNewValue = SPFieldHelpers.GetSPFieldLookupValueFromText(listField, (string)profileNewValue);
        //    }
        //    else {
        //        fieldNewValue = profileNewValue;
        //    }
        //    return fieldNewValue;
        //}
        //#endregion

        //#region UserItems methods
        //private void UpdateUserItems(SPListItemCollection items, Dictionary<string, object> fieldsValuesMap)
        //{
        //    items.Cast<SPListItem>().ToList().ForEach(i =>
        //    {
        //        fieldsValuesMap
        //            .ToList()
        //            .ForEach(p =>
        //            {
        //                var fieldName = p.Key;
        //                try
        //                {
        //                    i[fieldName] = p.Value;
        //                }
        //                catch (Exception ex)
        //                {
        //                    var message = String.Format(CommonConstants.ERROR_MESSAGE_TEMPLATE, i.ParentList.ID, i.ID, ex.ToString());
        //                    SPLogger.WriteLog(SPLogger.Category.Unexpected, "Item FieldValue Error", message);
        //                    return;
        //                }
        //            });
        //        using (new DisableItemEvents())
        //        {
        //            i.SystemUpdate();
        //        }
        //    });
        //}

        
        //#endregion
    }
}