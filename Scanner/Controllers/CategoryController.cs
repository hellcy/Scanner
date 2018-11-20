using System.Web.Mvc;
using static Scanner.FilterConfig;
using Scanner.Models;
using System.Collections.Generic;
using System.Data;
using System;

namespace Scanner.Controllers
{
    public class CategoryController : BaseController
    {
        [SessionExpire]
        //[Authorize]
        public ActionResult Index(string catId)
        {

            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if (!string.IsNullOrEmpty(Request.Form["rows"]))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            Session["CurrForm"] = "Category";
            var rqcatId = Request.QueryString["catId"].ToString();
            Session["MenuExpandings"] = ";" + rqcatId;
            if (rqcatId.Split('.').Length == 1)
            {
                IList<SideMenu> menus = ((List<SideMenu>)Session["SideMenu"]);
                foreach (var menu in menus)
                {
                    if (rqcatId == menu.SideMenuId)
                    {
                        return Redirect($"{Url.RouteUrl(new { controller = "Order", action = menu.lv1.Split('!')[1] })}");
                    }
                }
            }
            return View();
        }

        [SessionExpire]
        //[Authorize]
        public ActionResult SubCat(string catId)
        {
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if (!string.IsNullOrEmpty(Request.Form["rows"]))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            string[] strArr = catId.Split('.');
            Session["MenuExpandings"] = ";" + strArr[0] + ".0;" + catId;
            Session["CurrForm"] = "Category";
            return View();
        }

    }
}