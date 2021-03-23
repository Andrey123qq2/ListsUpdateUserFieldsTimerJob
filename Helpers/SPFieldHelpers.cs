using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ListsUpdateUserFieldsTimerJob
{
    class SPFieldHelpers
    {
        public static SPFieldLookupValue GetSPFieldLookupValueFromText(SPField listField, string lookupTitleValue)
        {
            string rootSiteUrl = listField.ParentList.ParentWeb.Site.Url;
            XElement fieldSchemaXml = XElement.Parse(listField.SchemaXml);
            string lookupListId = fieldSchemaXml.Attribute("List").Value;
            string lookupWebId = fieldSchemaXml.Attribute("WebId").Value;
            string lookupFieldName = fieldSchemaXml.Attribute("ShowField").Value;

            SPList lookupList = SPListHelpers.GetSPList(rootSiteUrl, new Guid(lookupWebId), new Guid(lookupListId));
            SPListItemCollection lookupItem = lookupList.QueryItems(lookupFieldName, lookupTitleValue);
            if (lookupItem.Count == 0)
                return new SPFieldLookupValue();
            var lookupValue = new SPFieldLookupValue(lookupItem[0].ID, lookupTitleValue);
            return lookupValue;
        }
    }
}
