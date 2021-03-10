using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    class TimerJob : SPJobDefinition
    {
        public TimerJob() : base() { }
        public TimerJob(string jobName, SPService service, SPServer server, SPJobLockType targetType) : base(jobName, service, server, targetType) { }
        public TimerJob(string jobName, SPWebApplication webApplication) : base(jobName, webApplication, null, SPJobLockType.ContentDatabase)
        {
            this.Title = jobName;
        }
        public override void Execute(Guid contentDbId)
        {
            try {
                List<SPListToModifyContext> listsToModifyContextes = SPListToModifyContext.Factory();
                listsToModifyContextes.ForEach(l => UpdateList(l));

            } catch (Exception ex) { }
        }

        private void UpdateList(SPListToModifyContext listContext)
        {
            listContext.SetStrategy(new SPListUserAttributesStrategy());
            listContext.UpdateListItems();
        }

    }
}
