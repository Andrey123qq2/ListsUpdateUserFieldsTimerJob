using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob.Strategies
{
    public class UserItemsAndNewFieldsValues
    {
        public string UserLogin;
        public List<SPListItem> ListItems;
        public List<UserProfileChange> ProfileChanges;
        public Dictionary<string, object> FieldsNewValues;
    }
}
