using ListsUpdateUserFieldsTimerJob.SPHelpers;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ListsUpdateUserFieldsTimerJob.Strategies
{
    public class UpdateUserFieldsForce : ISPListModifierStrategy
    {
        private SPListToModifyContext _listContext;
        private readonly string _camlQueryTemplate = @"<Where><Geq><FieldRef Name = Created/> <Value Type = 'DateTime'><Today OffsetDays={0}/></Value></Geq></Where>";
        public void Execute(SPListToModifyContext context)
        {
            if (context == null || !context.TJListConf.Enable || context.TJListConf.FilterCreatedLastDays == 0)
                return;
            _listContext = context;
            var itemsForUpdate = GetListItemsForUpdate();
            var itemsForUpdateGroupedByUsers = GetListItemsGroupedByUsers(itemsForUpdate);
            _listContext.UsersItemsAndProfileChanges = GetUsersItemsAndProfileAttributes(itemsForUpdateGroupedByUsers);
            _listContext.UsersItemsAndProfileChanges.ForEach(i => UpdateItemByNewValues(i));
        }
        private void UpdateItemByNewValues(UserItemsAndNewFieldsValues item)
        {
            item.ListItems
                .Cast<SPListItem>()
                .ToList()
                .ForEach(i => i.UpdateByNewValues(item.FieldsNewValues));
        }
        private SPListItemCollection GetListItemsForUpdate()
        {
            string camlQueryString = String.Format(_camlQueryTemplate, -1 * _listContext.TJListConf.FilterCreatedLastDays);
            SPListItemCollection items = _listContext.CurrentList.QueryItems(camlQueryString);
            return items;
        }
        private List<IGrouping<string, SPListItem>> GetListItemsGroupedByUsers(SPListItemCollection items)
        {
            var itemsGroupedByUsers = items
                .Cast<SPListItem>()
                .GroupBy(i =>
                    {
                        string userFieldValueString = i[_listContext.TJListConf.UserField].ToString();
                        SPFieldUserValue userFieldValue = new SPFieldUserValue(_listContext.CurrentList.ParentWeb, userFieldValueString);
                        return userFieldValue.User.LoginName;
                    }
                )
                .ToList();
            return itemsGroupedByUsers;
        }
        private List<UserItemsAndNewFieldsValues> GetUsersItemsAndProfileAttributes(List<IGrouping<string, SPListItem>> listItemsGroupedByUsers)
        {
            var usersItemsAndProfileAttributes = listItemsGroupedByUsers
                .Select(g =>
                    {
                        string userLogin = g.Key;
                        return new UserItemsAndNewFieldsValues
                        {
                            UserLogin = userLogin,
                            ListItems = g.ToList(),
                            FieldsNewValues = GetFieldsNewValuesMapFromUserProfile(userLogin)
                        };
                    }
                )
                .ToList();
            return usersItemsAndProfileAttributes;
        }
        private Dictionary<string, object> GetFieldsNewValuesMapFromUserProfile(string userLogin)
        {
            var userProfile = _listContext.ProfilesChangesManager.GetUserProfile(userLogin);
            Dictionary<string, object> fieldsNewValuesMap = _listContext.TJListConf.AttributesFieldsMap
                .ToDictionary(
                    a => a.Value,
                    a => GetFieldValueFromProfile(userProfile, a.Key, a.Value)
                );
            return fieldsNewValuesMap;
        }
        private object GetFieldValueFromProfile(UserProfile profile, string attributeName, string listFieldName)
        {
            object fieldNewValue;
            var profileValue = profile[attributeName].Value;
            SPField listField = _listContext.CurrentList.Fields.GetField(listFieldName);
            string listFieldTypeName = listField.TypeAsString;
            if (listFieldTypeName.Contains("User"))
                fieldNewValue = _listContext.CurrentList.ParentWeb.EnsureUser((string)profileValue);
            else if (listFieldTypeName.Contains("Lookup"))
                fieldNewValue = SPFieldHelpers.GetSPFieldLookupValueFromText(listField, (string)profileValue);
            else
                fieldNewValue = profileValue;
            return fieldNewValue;
        }
    }
}