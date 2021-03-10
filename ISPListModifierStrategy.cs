﻿using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    interface ISPListModifierStrategy
    {
        void UpdateItems(SPListToModifyContext context);
    }
}
