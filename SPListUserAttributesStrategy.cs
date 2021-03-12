using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    class SPListUserAttributesStrategy : ISPListModifierStrategy
    {
        private static readonly UserProfileChangeCollection _profilesChanges = new ProfilesChangesManager().GetChanges();
        private SPListToModifyContext _listContext;
        public void UpdateItems(SPListToModifyContext context)
        {
            _listContext = context;
            var changesGroupedByUsers = GetChangesGroupedByUser();
            changesGroupedByUsers.ForEach(g => UpdateUserItemsByChanges(g));
        }

        private void UpdateUserItemsByChanges(IGrouping<string, UserProfileChange> changedProperties)
        {
            string userLogin = changedProperties.Key;
            SPListItemCollection userItems = GetUserItems(userLogin);
            Dictionary<string, object> changedAttributes = GetUserChangesByListFields(changedProperties);
            UpdateUserItems(userItems, changedAttributes);
        }

        private Dictionary<string, object> GetUserChangesByListFields(IGrouping<string, UserProfileChange> changedProperties)
        {
            Dictionary<string, object> actualChanges = new Dictionary<string, object>();
            changedProperties.ToList().ForEach(c =>
            {
                var changedProperty = ((UserProfileSingleValueChange)c).ProfileProperty.Name;
                if (_listContext.ERConf.AttributesFieldsMap.ContainsKey(changedProperty))
                    actualChanges.Add(changedProperty, ((UserProfileSingleValueChange)c).NewValue);
            });
            return actualChanges;
        }

        private void UpdateUserItems(SPListItemCollection items, Dictionary<string, object> changedAttributes)
        {
            items.Cast<SPListItem>().ToList().ForEach(i =>
            {
                changedAttributes.ToList().ForEach(p =>
                {
                    i[p.Key] = p.Value;
                });
                i.SystemUpdate();
            });
        }

        private SPListItemCollection GetUserItems(string userLogin)
        {
            string fieldName = _listContext.ERConf.UserField;
            string fieldInternalName = _listContext.CurrentList.Fields.GetField(fieldName).InternalName;
            SPUser spUser = _listContext.CurrentList.ParentWeb.EnsureUser(userLogin);
            SPListItemCollection items = QueryUserItems(spUser.ID, fieldInternalName);
            return items;
        }

        private SPListItemCollection QueryUserItems(int userID, string fieldInternalName)
        {
            string camlQueryTemplate = @"<Where><Eq><FieldRef Name='{0}' LookupId='True' /><Value Type = 'User'>{1}</Value></Eq></Where>";
            string camlQueryText = String.Format(camlQueryTemplate, fieldInternalName, userID);
            SPQuery spQuery = new SPQuery
            {
                Query = camlQueryText
            };
            SPListItemCollection items = _listContext.CurrentList.GetItems(spQuery);
            return items;
        }

        private List<IGrouping<string, UserProfileChange>> GetChangesGroupedByUser()
        {
            var groupedByUserChanges = _profilesChanges.Cast<UserProfileChange>()
                .Where(c => c.ChangeType == ChangeTypes.Add || c.ChangeType == ChangeTypes.Modify)
                .OrderBy(c => c.AccountName)
                .GroupBy(p => p.AccountName)
                .ToList();
            return groupedByUserChanges;
        }
    }
}
