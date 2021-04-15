using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob.Strategies
{
    public class UserItemsAndProfileChanges
    {
        public string UserLogin;
        public SPListItemCollection ListItems;
        public List<UserProfileChange> ProfileChanges;
        public Dictionary<string, object> FieldsNewValues;
    }
}
