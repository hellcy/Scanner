using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Sockets;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Scanner.Models;
using static Scanner.FilterConfig;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Scanner.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.CurrIP = GetIPAddress();
            LoginUser loginUser = new LoginUser();
            Session.RemoveAll();
            Session["IP"] = ViewBag.CurrIP;
            return View(loginUser);
        }

        [HttpPost]
        public ActionResult Index(Models.LoginUser user)
        {
            ViewBag.isFront = false;
            ViewBag.CurrIP = GetIPAddress();
            Session["IP"] = ViewBag.CurrIP;

            if (string.IsNullOrEmpty(user.UserName))
            {
                ModelState.AddModelError("UserName", "* Login Email is null");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("Password", "* Password is null");
            }

            if (ModelState.IsValid)
            {
                Session["User"] = user.IsValid(user.UserName, user.Password);
                if (Session["User"] != null)
                {
                    user.RememberMe = false;
                    FormsAuthentication.SetAuthCookie(user.UserName + " " + user.Password, user.RememberMe);
                    return RedirectToAction("Index", "Scanner");
                }
                else
                {
                    ModelState.AddModelError("", "User is not existing!");
                }
            }
            return View(user);
        }

        [Authorize]
        public ActionResult Menu()
        {
            return View();
        }

        [Authorize]
        public ActionResult Despatch()
        {
            OutStandings outStandings = new OutStandings();
            return Despatch(outStandings);
        }


        [Authorize]
        [HttpPost]
        public ActionResult Despatch(OutStandings outStandings)
        {
            ViewBag.Title = "Despatch";
            Session["CurrForm"] = "Despatch";

            if (outStandings.CompanyList == null)
            {
                var sql = "exec dbo.proc_GetCompanyNameList";
                using (var context = new DbContext(Global.ConnStr))
                {
                    outStandings.CompanyList = context.Database.SqlQuery<string>(sql).ToList<string>();
                }
            }

            if (!string.IsNullOrEmpty(outStandings.Company))
            {
                bool exist = false;
                foreach (var company in outStandings.CompanyList)
                {
                    if (outStandings.Company == company)
                    {
                        exist = true;
                        break;
                    }
                }
                if (exist)
                {
                    if (string.IsNullOrEmpty(outStandings.sortCol))
                    {
                        outStandings.sortCol = "ORDER_NO";
                        outStandings.sortColType = "Number";
                        outStandings.rowsPerPage = 15;
                        outStandings.pageNum = 1;
                        outStandings.orderBy = "glyphicon glyphicon-arrow-down";
                    }

                    if (String.IsNullOrEmpty(outStandings.whereStr))
                    {
                        outStandings.whereStr = "";
                    }

                    var Company = new SqlParameter("@Company", outStandings.Company);
                    var pageNum = new SqlParameter("@pageNum", outStandings.pageNum);
                    var rowsPerPage = new SqlParameter("@rowsPerPage", outStandings.rowsPerPage);
                    var sortCol = new SqlParameter("@sortCol", outStandings.sortCol);
                    var sortColType = new SqlParameter("@sortColType", outStandings.sortColType);
                    var whereStr = new SqlParameter("@whereStr", outStandings.whereStr.ToString());

                    var orderBy = (outStandings.orderBy == "glyphicon glyphicon-arrow-down") ?
                        new SqlParameter("@orderBy", "desc") :
                        new SqlParameter("@orderBy", "asc");


                    var table = new SqlParameter("@table", "dbo.View_OutStandings");
                    var selStr = new SqlParameter("@selStr", "");

                    var sql = "exec dbo.proc_GetCompanyOutStandings " +
                        "@Company," +
                        "@pageNum, " +
                        "@rowsPerPage, " +
                        "@sortCol, " +
                        "@sortColType, " +
                        "@whereStr, " +
                        "@orderBy, " +
                        "@table, " +
                        "@selStr";

                    var oldMsg = "";

                    if (outStandings.errMsg == null)
                        outStandings.errMsg = "";
                    else
                        oldMsg = outStandings.errMsg;

                    try
                    {
                        using (var context = new DbContext(Global.ConnStr))
                        {
                            outStandings.outStandings = context.Database.SqlQuery<OutStanding>(sql,
                               Company,
                               pageNum,
                               rowsPerPage,
                               sortCol,
                               sortColType,
                               whereStr,
                               orderBy,
                               table,
                               selStr).ToList<OutStanding>();

                            sql = "proc_GetCompanyOutStandingDesp " +
                                "@Company";

                            Company = new SqlParameter("@Company", outStandings.Company);

                            outStandings.despatches = context.Database.SqlQuery<Despatch>(sql,
                              Company).ToList<Despatch>();

                            outStandings.totalWeight = 0;
                            foreach (Despatch d in outStandings.despatches)
                            {
                                d.SUB_WEIGHT = d.QTY * d.UNIT_WEIGHT;
                                outStandings.totalWeight += d.SUB_WEIGHT;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        outStandings.totalPages = 0;
                        outStandings.totalRows = 0;
                        outStandings.outStandings = null;
                        outStandings.errMsg = "No Record Found";
                    }


                    if (outStandings.outStandings != null)
                    {
                        if (outStandings.outStandings.Count > 0)
                        {
                            for (int i = 0; i < outStandings.outStandings.Count; i++)
                            {
                                outStandings.outStandings[i].WEIGHT = outStandings.outStandings[i].UNIT_WEIGHT * outStandings.outStandings[i].QTY;
                            }

                            outStandings.totalPages = outStandings.outStandings[0].maxPages;
                            outStandings.totalRows = outStandings.outStandings[0].TotalRows;
                        }
                    }

                    if (oldMsg != "")
                        outStandings.errMsg = oldMsg;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(outStandings.Company))
                {
                    outStandings.CompanyList = new List<String>();
                    outStandings.errMsg = "Error: Company is not existing";
                }
            }

            return View(outStandings);
        }


        [SessionExpire]
        [Authorize]
        public ActionResult BranchMap()
        {
            Session["CurrForm"] = "NranchMap";
            ViewBag.Message = "";

            return View();
        }




        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public ActionResult Logout()
        {
            using (var context = new DbContext(Global.ConnStr))
            {
                try
                {
                    var UserName = new SqlParameter("@UserName", ((User)Session["User"]).UserName);
                    var sql = "exec dbo.proc_Logout @UserName";
                    context.Database.ExecuteSqlCommand(sql, UserName);
                }
                catch (Exception e)
                {
                }
            }

            //FormsAuthentication.SignOut();                 

            Session.RemoveAll();
            return RedirectToAction("Index", "Home");

            //return View("OpenExo");
        }


    }
}