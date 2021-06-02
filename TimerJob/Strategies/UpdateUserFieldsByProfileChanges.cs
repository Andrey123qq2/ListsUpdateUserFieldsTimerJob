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
        private readonly string _camlQueryTemplateForUserField = @"<Eq><FieldRef Name='{0}' LookupId='True' /><Value Type = 'User'>{1}</Value></Eq>";
        public void Execute(SPListToModifyContext context)
        {
            if (context == null || !context.TJListConf.Enable || context.TJListConf.FilterCreatedLastDays > 0)
                return;
            _listContext = context;
            _listContext.UsersItemsAndProfileChanges = GetUsersItemsAndProfileChanges();
            _listContext.UsersItemsAndProfileChanges.ForEach(i => UpdateUserItemsByChanges(i));
        }

        private void UpdateUserItemsByChanges(UserItemsAndNewFieldsValues item)
        {
            item.ListItems
                .Cast<SPListItem>()
                .ToList()
                .ForEach(i => i.UpdateByNewValues(item.FieldsNewValues));
        }

        private List<UserItemsAndNewFieldsValues> GetUsersItemsAndProfileChanges() 
        {
            var usersItemsAndProfileChanges = _listContext.ProfilesChangesManager.ChangesGroupedByUser
                    .Select(g =>
                        {
                            var profileChanges = g.ToList();
                            return new UserItemsAndNewFieldsValues
                            {
                                UserLogin = g.Key,
                                ListItems = GetUserItems(g.Key),
                                ProfileChanges = profileChanges,
                                FieldsNewValues = GetFieldsNewValuesMap(profileChanges)
                            };
                        }
                    )
                    .Where(i => i.FieldsNewValues.Count > 0)
                    .ToList();
            return usersItemsAndProfileChanges;
        }
        private List<SPListItem> GetUserItems(string userLogin)
        {
            string camlQueryString = GetCamlQueryFilterString(userLogin);
            SPListItemCollection items = _listContext.CurrentList.QueryItems(camlQueryString);
            return items.Cast<SPListItem>().ToList();
        }
        private string GetCamlQueryFilterString(string userLogin)
        {
            string camlQueryFilterString;
            SPUser spUser = _listContext.CurrentList.ParentWeb.EnsureUser(userLogin);
            string camlQueryForUserField = String.Format(_camlQueryTemplateForUserField, _listContext.TJListConf.UserField, spUser.ID.ToString());
            if (!String.IsNullOrEmpty(_listContext.TJListConf.AdditionalCamlQuery))
                camlQueryFilterString = "<Where><And>" + camlQueryForUserField + _listContext.TJListConf.AdditionalCamlQuery + "</And></Where>";
            else
                camlQueryFilterString = "<Where>" + camlQueryForUserField + "</Where>";
            return camlQueryFilterString;
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
                fieldNewValue = _listContext.CurrentList.ParentWeb.EnsureUser((string)profileNewValue);
            else if (listFieldTypeName.Contains("Lookup"))
                fieldNewValue = SPFieldHelpers.GetSPFieldLookupValueFromText(listField, (string)profileNewValue);
            else
                fieldNewValue = profileNewValue;
            return fieldNewValue;
        }
        private Dictionary<string, object> GetFieldsNewValuesMap(List<UserProfileChange> changedProperties)
        {
            Dictionary<string, object> fieldsNewValuesMap = changedProperties
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
    }
}