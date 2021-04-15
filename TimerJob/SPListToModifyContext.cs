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
            UsersItemsAndProfileChanges = ProfilesChangesManager.ChangesGroupedByUser
                .Select(g =>
                    {
                        var profileChanges = g.ToList();
                        return new UserItemsAndProfileChanges
                        {
                            UserLogin = g.Key,
                            ListItems = GetUserItems(g.Key),
                            ProfileChanges = profileChanges,
                            FieldsNewValues = GetFieldsNewValuesMap(profileChanges)
                        };
                    }
                )
                .ToList();
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

        private SPListItemCollection GetUserItems(string userLogin)
        {
            string fieldName = TJListConf.UserField;
            string fieldInternalName = CurrentList.Fields.GetField(fieldName).InternalName;
            SPUser spUser = CurrentList.ParentWeb.EnsureUser(userLogin);
            SPListItemCollection items = CurrentList.QueryItems(fieldInternalName, spUser.ID.ToString(), CAMLQueryType.User);
            return items;
        }

        #region ProfileChanges processing methods
        private Dictionary<string, object> GetFieldsNewValuesMap(List<UserProfileChange> changedProperties)
        {
            Dictionary<string, object> fieldsNewValuesMap = changedProperties
                .Where(c => TJListConf.AttributesFieldsMap.ContainsKey(((UserProfileSingleValueChange)c).ProfileProperty.Name))
                .OrderByDescending(c => c.EventTime)
                .GroupBy(c => ((UserProfileSingleValueChange)c).ProfileProperty.Name)
                .Select(g => g.First())
                .ToDictionary(
                    c => TJListConf.AttributesFieldsMap[((UserProfileSingleValueChange)c).ProfileProperty.Name],
                    c => GetFieldValueFromProfileChange(c)
                );
            return fieldsNewValuesMap;
        }

        private object GetFieldValueFromProfileChange(UserProfileChange profileChange)
        {
            object fieldNewValue;
            string changedPropertyName = ((UserProfileSingleValueChange)profileChange).ProfileProperty.Name;
            string listFieldName = TJListConf.AttributesFieldsMap[changedPropertyName];
            SPField listField = CurrentList.Fields.GetField(listFieldName);
            string listFieldTypeName = listField.TypeAsString;
            var profileNewValue = ((UserProfileSingleValueChange)profileChange).NewValue;
            if (listFieldTypeName.Contains("User"))
            {
                fieldNewValue = CurrentList.ParentWeb.EnsureUser((string)profileNewValue);
            }
            else if (listFieldTypeName.Contains("Lookup"))
            {
                fieldNewValue = SPFieldHelpers.GetSPFieldLookupValueFromText(listField, (string)profileNewValue);
            }
            else
            {
                fieldNewValue = profileNewValue;
            }
            return fieldNewValue;
        }
        #endregion

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