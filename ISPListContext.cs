using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    interface ISPListContext
    {
        SPList CurrentList { get; }
        ConfListUserChanges ERConf { get; }
        //UserProfileChangeCollection ProfilesChanges { get; }
    }
}
