using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    class TimerJobConfig
    {
        public List<string> AttributesOptInLists = new List<string>();
        public string SPReportWebUrl;
        public string SPReportLibraryName;
        public string SPReportFilePathTemplate;
    }
}