using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListsUpdateUserFieldsTimerJob
{
    class ListConfigUpdateUserFields
    {
        public bool Enable;
        public string UserField;
        public Dictionary<string, string> AttributesFieldsMap;
    }
}