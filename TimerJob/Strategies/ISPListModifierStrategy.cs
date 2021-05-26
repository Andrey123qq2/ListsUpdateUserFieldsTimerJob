using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob.Strategies
{
    public interface ISPListModifierStrategy
    {
        public void Execute(SPListToModifyContext context);
    }
}