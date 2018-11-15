using System.Web;
using System.Web.Mvc;

namespace Scanner
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public class SessionExpireAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = HttpContext.Current;
                // check  sessions here
                if (HttpContext.Current.Session["User"] == null)
                {
                    filterContext.Result = new RedirectResult("~");
                    return;
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
