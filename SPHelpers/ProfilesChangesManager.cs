using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;

namespace SPHelpers
{
    class ProfilesChangesManager : IDisposable
    {
        private static int _daysToCheck;
        private readonly UserProfileManager _profileManager;
        private readonly SPSite _site;
        public ProfilesChangesManager(string siteUrl, int daysToCheck)
        {
            _daysToCheck = daysToCheck;
            _site = new SPSite(siteUrl);
            SPServiceContext context = SPServiceContext.GetContext(_site);
            _profileManager = new UserProfileManager(context);
        }
        public void Dispose()
        {
            _site.Dispose();
        }
        public UserProfileChangeCollection GetChanges()
        {
            DateTime startDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(_daysToCheck));
            UserProfileChangeToken changeToken = new UserProfileChangeToken(startDate);
            UserProfileChangeQuery changeQuery = new UserProfileChangeQuery(false, true)
            {
                ChangeTokenStart = changeToken,
                SingleValueProperty = true
            };
            UserProfileChangeCollection changes = _profileManager.GetChanges(changeQuery);
            return changes;
        }
    }
}