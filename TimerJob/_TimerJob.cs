using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using ListsUpdateUserFieldsTimerJob.SPHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListsUpdateUserFieldsTimerJob.Strategies;

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
        protected override bool HasAdditionalUpdateAccess()
        {
            return true;
        }
        public override void Execute(Guid contentDbId)
        {
            try
            {
                this.WebApplication.GetSitesWithFeature(CommonConstants.TJOB_SITE_FEATURE_NAME)
                    .ForEach(s => ProcessSite(s));
            } catch (Exception ex) {
                throw new Exception("Custom TimerJob exception: " + ex.Message);
            }
        }
        private void ProcessSite(SPSite site)
        {
            var siteTJConf = PropertyBagConfHelper<TimerJobConfig>.Get(
                site.RootWeb.AllProperties,
                CommonConstants.TJOB_PROPERTY_JSON_CONF
            );
            List<SPListToModifyContext> listsToModifyContextes = SPListToModifyContext.Factory(site);
            ProcessListsByStrategies(listsToModifyContextes, siteTJConf);
        }
        private void ProcessListsByStrategies(List<SPListToModifyContext> listsContextes, TimerJobConfig tjConf)
        {
            UpdateUserFieldsByProfileChanges strategy1 = new UpdateUserFieldsByProfileChanges();
            UpdateItemsPermissions strategy2 = new UpdateItemsPermissions();
            TimerJobReport strategy3 = new TimerJobReport(tjConf.SPReportWebUrl, tjConf.SPReportLibraryName, tjConf.SPReportFilePathTemplate);
            //TODO: AsParallel().ForAll
            listsContextes.ForEach(c => {
                c.SetStrategy(strategy1);
                c.ExecuteStrategy();
                c.SetStrategy(strategy2);
                c.ExecuteStrategy();
                c.SetStrategy(strategy3);
                c.ExecuteStrategy();
            });
            strategy3.SaveReport();
        }
    }
}
