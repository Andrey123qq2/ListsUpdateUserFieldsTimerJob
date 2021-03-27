using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPHelpers;

namespace ListsUpdateUserFieldsTimerJob
{
    public class UpdateUserFieldsByProfileChanges : ISPListModifierStrategy
    {
        private List<IGrouping<string, UserProfileChange>> _changesGroupedByUsers;
        private SPListToModifyContext _listContext;
        public UpdateUserFieldsByProfileChanges(SPSite site)
        {
            var profilesChangesManager = new ProfilesChangesManager(
                site,
                CommonConstants.CHANGE_MANAGER_DAYS_TO_CHECK
            );
            _changesGroupedByUsers = profilesChangesManager.GetAddModifyChangesGroupedByUser();
        }
        public void UpdateItems(SPListToModifyContext context)
        {
            if (context == null || !context.TJListConf.Enable)
                return;
            _listContext = context;
            _changesGroupedByUsers.ForEach(g => UpdateUserItemsByChanges(g));
        }

        private void UpdateUserItemsByChanges(IGrouping<string, UserProfileChange> changedProperties)
        {
            string userLogin = changedProperties.Key;
            SPListItemCollection userItems = GetUserItems(userLogin);
            Dictionary<string, object> changedAttributes = GetFieldsNewValuesMap(changedProperties);
            UpdateUserItems(userItems, changedAttributes);
        }

        #region ProfileChanges processing methods
        private Dictionary<string, object> GetFieldsNewValuesMap(IGrouping<string, UserProfileChange> changedProperties)
        {
            Dictionary<string, object> fieldsNewValuesMap = changedProperties.ToList()
                .Where(c => _listContext.TJListConf.AttributesFieldsMap.ContainsKey(((UserProfileSingleValueChange)c).ProfileProperty.Name))
                .OrderByDescending(c => c.EventTime)
                .GroupBy(c => ((UserProfileSingleValueChange)c).ProfileProperty.Name)
                .Select(g => g.First())
                .ToDictionary(
                    c => _listContext.TJListConf.AttributesFieldsMap[((UserProfileSingleValueChange)c).ProfileProperty.Name],
                    c => GetFieldValueFromProfileChange(c)
                );
            return fieldsNewValuesMap;
        }

        private object GetFieldValueFromProfileChange(UserProfileChange profileChange)
        {
            object fieldNewValue;
            string changedPropertyName = ((UserProfileSingleValueChange)profileChange).ProfileProperty.Name;
            string listFieldName = _listContext.TJListConf.AttributesFieldsMap[changedPropertyName];
            SPField listField = _listContext.CurrentList.Fields.GetField(listFieldName);
            string listFieldTypeName = listField.TypeAsString;
            var profileNewValue = ((UserProfileSingleValueChange)profileChange).NewValue;
            if (listFieldTypeName.Contains("User"))
            {
                fieldNewValue = _listContext.CurrentList.ParentWeb.EnsureUser((string)profileNewValue);
            }
            else if (listFieldTypeName.Contains("Lookup"))
            {
                fieldNewValue = SPFieldHelpers.GetSPFieldLookupValueFromText(listField, (string)profileNewValue);
            }
            else {
                fieldNewValue = profileNewValue;
            }
            return fieldNewValue;
        }
        #endregion

        #region UserItems methods
        private void UpdateUserItems(SPListItemCollection items, Dictionary<string, object> changedAttributes)
        {
            items.Cast<SPListItem>().ToList().ForEach(i =>
            {
                changedAttributes
                    .ToList()
                    .ForEach(p =>
                    {
                        var fieldForAttribute = p.Key;
                        try
                        {
                            i[fieldForAttribute] = p.Value;
                        }
                        catch (Exception ex)
                        {
                            var message = String.Format(CommonConstants.ERROR_MESSAGE_TEMPLATE, i.ParentList.ID, i.ID, ex.ToString());
                            SPLogger.WriteLog(SPLogger.Category.Unexpected, "Item FieldValue Error", message);
                            return;
                        }
                    });
                using (new DisableItemEvents())
                {
                    i.SystemUpdate();
                }
            });
        }

        private SPListItemCollection GetUserItems(string userLogin)
        {
            string fieldName = _listContext.TJListConf.UserField;
            string fieldInternalName = _listContext.CurrentList.Fields.GetField(fieldName).InternalName;
            SPUser spUser = _listContext.CurrentList.ParentWeb.EnsureUser(userLogin);
            SPListItemCollection items = _listContext.CurrentList.QueryItems(fieldInternalName, spUser.ID.ToString(), CAMLQueryType.User);
            return items;
        }
        #endregion
    }
}