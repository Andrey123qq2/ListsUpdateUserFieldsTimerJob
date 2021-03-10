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
        }
        public void UpdateUserFieldsInItems(string user, Dictionary<string, string> fieldsAndValues)
        {
            List<SPListItem> userItems = GetUserItems(user, _listContext.ERConf.UserField);
            foreach (var item in userItems)
                UpdateItem(item, fieldsAndValues);
        }

        private List<SPListItem> GetUserItemsByField(string user, string field)
        {
            string fieldInternalName = _listContext.CurrentList.Fields.GetField(field).InternalName;
            string camlQueryTemplate = @"<Query><Where><Eq><FieldRef Name='{0}'/><Value Type='Text'>{1}</Value></Eq></Where></Query>";

        }

        private List<IGrouping<string, UserProfileChange>> GetGroupedByUserChanges()
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
