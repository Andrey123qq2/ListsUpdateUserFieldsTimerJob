using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;

namespace ListsUpdateUserFieldsTimerJob
{
    class ProfilesChangesManager : IDisposable
    {
        public static int DaysToCheck = 3;
        //public UserProfileChangeCollection RecentChanges { get; private set; }
        private UserProfileManager _profileManager;
        private SPSite _site;
        //private static readonly ProfilesChangesManager _instance = new ProfilesChangesManager();
        //public static ProfilesChangesManager Instance => _instance;
        public ProfilesChangesManager()
        {
            _site = new SPSite("https://dev-info.deps.kiev.ua");
            SPServiceContext context = SPServiceContext.GetContext(_site);
            _profileManager = new UserProfileManager(context);
        }
        public void Dispose()
        {
            _site.Dispose();
        }
        public UserProfileChangeCollection GetChanges()
        {
            DateTime startDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(DaysToCheck));
            UserProfileChangeToken changeToken = new UserProfileChangeToken(startDate);
            UserProfileChangeQuery changeQuery = new UserProfileChangeQuery(false, true)
            {
                ChangeTokenStart = changeToken,
                SingleValueProperty = true,
            };
            UserProfileChangeCollection changes = _profileManager.GetChanges(changeQuery);
            return changes;
        }

        //public List<string> GetProfilesAttributes()
        //{
        //    SPServiceContext serviceContext = SPServiceContext.GetContext(_site);
        //    ProfileSubtypeManager profileSubtypeMgr = ProfileSubtypeManager.Get(serviceContext);
        //    ProfileSubtype profileSubtype = profileSubtypeMgr.GetProfileSubtype(ProfileSubtypeManager.GetDefaultProfileName(ProfileType.User));
        //    ProfileSubtypePropertyManager profileSubtypePropertyMgr = profileSubtype.Properties;
        //    List<string> profileProperties = profileSubtypePropertyMgr.PropertiesWithSection
        //        .ToArray().ToList()
        //        .Select(p => ((ProfileSubtypeProperty)p).Name)
        //        .ToList();
        //    return profileProperties;
        //}
    }
}
