using ListsUpdateUserFieldsTimerJob.SPHelpers;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ListsUpdateUserFieldsTimerJob.Layouts.ListsUpdateUserFieldsTimerJob
{
    public partial class SiteAllConfigs : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                BindDataToListsConfigsTable();
            }
            catch (Exception exception)
            {
                HandleException();
            }
            finally
            {
                divloadingImage.Visible = false;
                ListsConfigsTable.Visible = true;
                ListsConfigsTimer.Enabled = false;
            }
        }

        private void BindDataToListsConfigsTable()
        {
            SPSite site = new SPSite(SPContext.Current.Web.Url);
            var listsWithConf = site.GetListsWithJSONConf(CommonConstants.LIST_PROPERTY_JSON_CONF);
            var arrayForGridView = GetArrayForGridView(listsWithConf);
            ListsConfigsTable.DataSource = arrayForGridView;
            ListsConfigsTable.DataBind();

        }
        public Array GetArrayForGridView(List<SPList> lists)
        {
            Array arrayForGridView = lists
                .Select(list =>
                    {
                        var conf = PropertyBagConfHelper<ListConfigUpdateUserFields>.Get(
                            list.RootFolder.Properties,
                            CommonConstants.LIST_PROPERTY_JSON_CONF
                        );
                        return new
                        {
                            ListTitle = list.Title,
                            ListUrl = list.DefaultViewUrl,
                            ConfModifiedDate = conf.ConfModified,
                            ConfEnabled = conf.Enable,
                            ConfTitle = "TimerJob Config",
                            ConfUrl = String.Format(
                                list.ParentWeb.Url + "/_layouts/15/ListsUpdateUserFieldsTimerJob/ConfigEdit.aspx?List={0}",
                                "{" + list.ID.ToString() + "}")
                        };
                    }
                )
                .ToArray();
            return arrayForGridView;
        }
        protected void HandleException()
        {
            divWebPartError.Visible = true;
        }

    }
}
