using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPHelpers
{
    static class SPWebApplicationExtensions
    {
        public static List<SPSite> GetSitesWithFeature(this SPWebApplication webApp, string featureName)
        {
            var sites = webApp.Sites
                .Where(s =>
                {
                    var feature = s.Features.FirstOrDefault(f => f.Definition.DisplayName == featureName);
                    return feature != null;
                }).ToList();
            return sites;
        }
    }
}