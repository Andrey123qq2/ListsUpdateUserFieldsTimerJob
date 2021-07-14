using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    public class ListConfigUpdateUserFields
    {
        public bool Enable;
        public string UserField;
        public Dictionary<string, string> AttributesFieldsMap;
        public string AdditionalCamlQuery;
        public bool ForceUpdate;
        public bool DisableForceUpdateAfterRun;
        public bool DisableForceUpdatePermissions;
        public string ForceUpdateCamlQuery;
        public string Notes;
        public string ConfModified;
    }
}