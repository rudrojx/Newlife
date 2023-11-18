using Newlife.Filters;
using System.Web;
using System.Web.Mvc;

namespace Newlife
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new HomeFilter());
        }
    }
}
