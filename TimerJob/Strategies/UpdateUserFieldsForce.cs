using ListsUpdateUserFieldsTimerJob.SPHelpers;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ListsUpdateUserFieldsTimerJob.Strategies
{
    public class UpdateUserFieldsForce : ISPListModifierStrategy
    {
        private SPListToModifyContext _listContext;
        public void Execute(SPListToModifyContext context)
        {
            if (context == null || !context.TJListConf.Enable || !context.TJListConf.ForceUpdate)
                return;
            _listContext = context;
            if (_listContext.TJListConf.DisableForceUpdatePermissions)
                _listContext.DisableUpdatePermissions = true;
            var itemsForUpdate = GetListItemsForUpdate();
            var itemsForUpdateGroupedByUsers = GetListItemsGroupedByUsers(itemsForUpdate);
            _listContext.UsersItemsAndProfileChanges = GetUsersItemsAndProfileAttributes(itemsForUpdateGroupedByUsers);
            _listContext.UsersItemsAndProfileChanges.ForEach(i => UpdateItemByNewValues(i));
            ChangeForceUpdateInListConf();
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
            SPListItemCollection items;
            if (String.IsNullOrEmpty(_listContext.TJListConf.ForceUpdateCamlQuery))
                items = _listContext.CurrentList.Items;
            else
                items = _listContext.CurrentList.QueryItems("<Where>" + _listContext.TJListConf.ForceUpdateCamlQuery + "</Where>");
            return items;
        }
        private List<IGrouping<string, SPListItem>> GetListItemsGroupedByUsers(SPListItemCollection items)
        {
            var itemsGroupedByUsers = items
                .Cast<SPListItem>()
                .Where(i => i[_listContext.TJListConf.UserField] != null)
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
            {
                fieldNewValue = (profileValue != null) ? 
                    _listContext.CurrentList.ParentWeb.EnsureUser((string)profileValue) 
                    : null;
            }
            else if (listFieldTypeName.Contains("Lookup"))
                fieldNewValue = SPFieldHelpers.GetSPFieldLookupValueFromText(listField, (string)profileValue);
            else
                fieldNewValue = profileValue;
            return fieldNewValue;
        }
        private void ChangeForceUpdateInListConf()
        {
            if (!_listContext.TJListConf.DisableForceUpdateAfterRun)
                return;
            _listContext.TJListConf.ForceUpdate = false;
            PropertyBagConfHelper<ListConfigUpdateUserFields>.Set(
                _listContext.CurrentList.RootFolder.Properties, 
                CommonConstants.LIST_PROPERTY_JSON_CONF,
                _listContext.TJListConf
            );
            _listContext.CurrentList.Update();
        }
    }
}