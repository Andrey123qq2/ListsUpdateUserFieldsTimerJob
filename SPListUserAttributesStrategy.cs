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
            var allChangesGrouped = GetChangesGroupedByUser();
            allChangesGrouped.ForEach(g =>
            {
                string userLogin = g.Key;
                string fieldName = _listContext.ERConf.UserField;
                string fieldInternalName = _listContext.CurrentList.Fields.GetField(fieldName).InternalName;
                SPUser spUser = _listContext.CurrentList.ParentWeb.EnsureUser(userLogin);
                var userItems = GetItemsByUserField(spUser.ID, fieldInternalName);
                var changesForUser = GetActualChangedPropertiesValuesForUser(g);
                userItems.Cast<SPListItem>().ToList().ForEach(i =>
                {
                    UpdateItems(i, changesForUser);
                });
            });
        }

        private Dictionary<string, object> GetActualChangedPropertiesValuesForUser(IGrouping<string, UserProfileChange>)
        {
            Dictionary<string, object> changes = new Dictionary<string, object>();
            g.ToList().ForEach(c =>
            {
                var changedProperty = ((UserProfileSingleValueChange)c).ProfileProperty.Name;
                if (_listContext.ERConf.AttributesFieldsMap.ContainsKey(changedProperty))
                {
                    actualChangedPropertiesValues.Add(changedProperty, ((UserProfileSingleValueChange)c).NewValue);
                }
            });
            return changes;
        }
        public void UpdateUserFieldsInItems(string user, Dictionary<string, string> fieldsAndValues)
        {
            List<SPListItem> userItems = GetUserItems(user, _listContext.ERConf.UserField);
            foreach (var item in userItems)
                UpdateItem(item, fieldsAndValues);
        }

        private void UpdateUserItems(SPListItemCollection items, UserProfileChange profileChanges)
        {
            items.Cast<SPListItem>().ToList().ForEach(i =>
            {

            });
        }

        private UserProfileChangeCollection GetChangesForListFields(UserProfileChange profileChanges)
        {
            profileChanges.
        }

        private SPListItemCollection GetItemsByUserField(int userID, string fieldInternalName)
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
//        AccountName = "string" in parent object

//ChangeType = [Modify, Add] - Microsoft.Office.Server.UserProfiles.ChangeTypes

//NewValue = object - string or DateTime, etc

//ObjectType = SingleValueProperty - Microsoft.Office.Server.UserProfiles.ObjectTypes

//ProfileProperty.Name = "string"


//((UserProfileSingleValueChange) allChanges[0]).NewValue

// var allChangesGrouped = profileManager.GetChanges().Cast<UserProfileChange>()
//                    .Where(c => c.ChangeType == ChangeTypes.Add || c.ChangeType == ChangeTypes.Modify)
//                    .ToList().OrderBy(c=>c.AccountName).GroupBy(p=>p.AccountName);

//        allChangesGrouped.ToList()[198].ToList()[0]
    }
}
