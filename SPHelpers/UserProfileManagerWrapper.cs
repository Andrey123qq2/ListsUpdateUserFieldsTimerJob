using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;

namespace ListsUpdateUserFieldsTimerJob.SPHelpers
{
    public class UserProfileManagerWrapper // : IDisposable
    {
        public List<IGrouping<string, UserProfileChange>> ChangesGroupedByUser {get; }
        private readonly UserProfileManager _profileManager;
        private readonly SPSite _site;
        public UserProfileManagerWrapper(SPSite site, int daysToCheckUserProfilesChanges = 2)
        {
            _site = site;
            SPServiceContext context = SPServiceContext.GetContext(_site);
            _profileManager = new UserProfileManager(context);
            ChangesGroupedByUser = GetAddModifyChangesGroupedByUser(daysToCheckUserProfilesChanges);
        }
        //public void Dispose()
        //{
        //    _site.Dispose();
        //}
        public UserProfile GetUserProfile(string userLogin)
        {
            return _profileManager.GetUserProfile(userLogin);
        }
        public UserProfileChangeCollection GetChanges(int daysToCheck)
        {
            DateTime startDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(daysToCheck));
            UserProfileChangeToken changeToken = new UserProfileChangeToken(startDate);
            UserProfileChangeQuery changeQuery = new UserProfileChangeQuery(false, true)
            {
                ChangeTokenStart = changeToken,
                SingleValueProperty = true,
                UpdateMetadata = false
            };
            UserProfileChangeCollection changes = _profileManager.GetChanges(changeQuery);
            return changes;
        }
        public List<IGrouping<string, UserProfileChange>> GetAddModifyChangesGroupedByUser(int daysToCheck)
        {
            var groupedByUserChanges = GetChanges(daysToCheck).Cast<UserProfileChange>()
                .Where(c => {
                    return c.ChangeType == ChangeTypes.Add || c.ChangeType == ChangeTypes.Modify;
                })
                .GroupBy(p => p.AccountName)
                .ToList();
            return groupedByUserChanges;
        }
    }
}