using System.Web.Mvc;
using static Scanner.FilterConfig;
using Scanner.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace Scanner.Controllers
{
    public class ReturnController : Controller
    {
        [SessionExpire]
        [Authorize]
        public ActionResult Index()
        {
            ReturnOrder rtnOrder = new ReturnOrder();

            using (var context = new DbContext(Global.ConnStr))
            {
                try
                {
                    rtnOrder.TypeCodes = context.Database.SqlQuery<string>("proc_GetRtnTypeCodes").ToList<string>();
                }
                catch (Exception e)
                {
                }
            }

            return View(rtnOrder);
        }
    }
}