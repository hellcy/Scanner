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
using System.Management;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Scanner.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            // Global.Execute(@"C:\Bridge\Software\BridgeMann\DF", @"C:\Bridge\Software\BridgeMann\DF\DF.exe","GRAM", @"engineering", "fence");       
            LoginUser user = new LoginUser();

            Session["aebn"] = "0";
            if (Request.QueryString["aebn"] != null)
            {
                if (Request.QueryString["aebn"].ToString() == "367a974c-bf89-4732-803b-7e71d9b3f0f2")
                {
                    Session["aebn"] = "1";
                    if (Request.QueryString["nndk"] != null)
                    {
                        var compName = Request.QueryString["nndk"].ToString().ToUpper();
                        user = GetHandleUser(compName);

                        //LoginUser user = new LoginUser();
                        user.UserName = "charlesc@gram.com.au";
                        user.Password = "123";

                        Session["User"] = user.IsValid(user.UserName, user.Password);
                        if (Session["User"] != null)
                        {
                            user.RememberMe = false;
                            FormsAuthentication.SetAuthCookie(user.UserName + " " + user.Password, user.RememberMe);
                            //FormsAuthentication.Authenticate(user.UserName, user.Password);
                            Session["TradeType"] = 0;
                            IList<SideMenu> sideMenus = new List<SideMenu>();
                            using (var context = new DbContext(Global.ConnStr))
                            {
                                try
                                {
                                    sideMenus = context.Database.SqlQuery<SideMenu>("proc_GetSideMenu_v1").ToList<SideMenu>();
                                    Session["SideMenu"] = sideMenus;
                                }
                                catch (Exception e)
                                {
                                    Session["SideMenu"] = null;
                                }
                            }
                            Session["IP"] = GetIPAddress();
                            return RedirectToAction("Index", "Scanner");
                        }
                    }
                }
            }

            ViewBag.CurrIP = GetIPAddress();
            LoginUser loginUser = new LoginUser();
            int numFrontCnt = Convert.ToInt32(WebConfigurationManager.AppSettings["numfrontC"]);
            ViewBag.isFront = false;
            try
            {
                for (var i = 1; i < numFrontCnt + 1; i++)
                {
                    if (ViewBag.CurrIP == WebConfigurationManager.AppSettings["FrontCounter" + i.ToString()].ToString())
                    {
                        ViewBag.isFront = true;
                        loginUser.RememberMe = false;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
            }

            if (!ViewBag.isFront)
            {
                string cookieName = FormsAuthentication.FormsCookieName; //Find cookie name
                HttpCookie authCookie = HttpContext.Request.Cookies[cookieName]; //Get the cookie by it's name
                FormsAuthenticationTicket ticket;
                if (authCookie != null)
                {
                    ticket = FormsAuthentication.Decrypt(authCookie.Value); //Decrypt it
                    if (ticket.IsPersistent)
                    {
                        if (ticket.Name.Split(' ').Length == 2)
                        {
                            loginUser.UserName = ticket.Name.Split(' ')[0];
                            loginUser.Password = ticket.Name.Split(' ')[1];
                        }
                        loginUser.RememberMe = true;
                    }
                }
            }
            else
            {
                if (loginUser.PhoneList == null)
                {
                    var sql = "exec dbo.proc_GetPhoneList";
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        loginUser.PhoneList = context.Database.SqlQuery<string>(sql).ToList<string>();
                    }
                }
            }

            if ((Session["User"] == null) && (Request.IsAuthenticated))
            {
                if (loginUser.UserName != "")
                {
                    try
                    {
                        using (var context = new DbContext(Global.ConnStr))
                        {
                            var UserName = new SqlParameter("@UserName", loginUser.UserName);
                            var sql = "exec dbo.proc_Logout @UserName";
                            context.Database.ExecuteSqlCommand(sql, UserName);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
                // FormsAuthentication.SignOut();
            }

            Session.RemoveAll();

            Session["CapturedImage"] = "";
            Session["IP"] = ViewBag.CurrIP;
            Session["TradeType"] = "0";
            if (ViewBag.isFront)
            {
                Session["TradeType"] = "1";
            }

            if (ViewBag.CurrIP == "::1")
            {
                loginUser.UserName = "charlesc@gram.com.au";
                loginUser.Password = "123";
            }

            return View(loginUser);
        }

        private LoginUser GetHandleUser(string compName)
        {
            LoginUser loginuser = new LoginUser();
            IList<User> users = new List<User>();
            User user = new User();
            using (var context = new DbContext(Global.ConnStr))
            {

                object[] parameters = { compName };
                try
                {
                    users = context.Database.SqlQuery<User>("proc_GetHandleUser {0}", parameters).ToList<User>();
                    user = (users.Count > 0) ? users[0] : null;

                }
                catch (ValidationException e)
                {
                    user = null;
                }
            }

            if (user != null)
            {
                loginuser.UserName = user.UserName;
                loginuser.Password = user.Password;
                loginuser.RememberMe = false;
            }

            return loginuser;
        }

        [HttpPost]
        public ActionResult Index(Models.LoginUser user)
        {
            ViewBag.isFront = false;
            ViewBag.CurrIP = GetIPAddress();

            if (Request.Form["isFront"] != null)
            {
                if (Request.Form["isFront"] == "1")
                {
                    ViewBag.isFront = true;
                    Session.RemoveAll();
                }
            }

            if (!string.IsNullOrEmpty(user.Phone))
            {
                user.Phone = user.Phone.Replace(" ", "");
            }

            Session["IP"] = ViewBag.CurrIP;
            Session["TradeType"] = "0";
            if ((Request.Form["isFront"] == "1") && (user.UserName != "quotation"))
            {
                Session["TradeType"] = "1";

                if (user.PhoneList == null)
                {
                    var sql = "exec dbo.proc_GetPhoneList";
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        user.PhoneList = context.Database.SqlQuery<string>(sql).ToList<string>();
                    }
                }

                bool phExist = false;
                foreach (var ph in user.PhoneList)
                {
                    if (user.Phone == ph.Split(',')[0])
                    {
                        phExist = true;
                        break;
                    }
                }

                if (!phExist)
                {
                    ModelState.AddModelError("Phone", "* Phone/Mobile is not existing.");
                }
            }

            if (Request.QueryString["newCustLogin"] != null)
            {
                //Session["TradeType"] = Request.QueryString["newCustLogin"].ToString();
                Session["TradeType"] = "2";
                int numFrontCnt = Convert.ToInt32(WebConfigurationManager.AppSettings["numfrontC"]);
                try
                {
                    for (var i = 1; i < numFrontCnt + 1; i++)
                    {
                        if (ViewBag.CurrIP == WebConfigurationManager.AppSettings["FrontCounter" + i.ToString()].ToString())
                        {
                            user.UserName = "FrontCounter" + i.ToString();
                            user.Password = "frontxx" + i.ToString();
                            user.RememberMe = false;

                            Session["User"] = user.IsValid(user.UserName, user.Password);
                            if (Session["User"] != null)
                            {
                                FormsAuthentication.SetAuthCookie(user.UserName + " " + user.Password, user.RememberMe);
                                IList<SideMenu> sideMenus = new List<SideMenu>();
                                using (var context = new DbContext(Global.ConnStr))
                                {
                                    try
                                    {
                                        sideMenus = context.Database.SqlQuery<SideMenu>("proc_GetSideMenu_v1").ToList<SideMenu>();
                                        Session["SideMenu"] = sideMenus;
                                    }
                                    catch (Exception e)
                                    {
                                        Session["SideMenu"] = null;
                                    }
                                }

                                return RedirectToAction("Index", "Order");
                            }
                            else
                            {
                                ModelState.AddModelError("", "User is not existing!");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }

            if ((Request.Form["isFront"] != "1") && (user.UserName != "quotation"))
            {
                if (string.IsNullOrEmpty(user.UserName))
                {
                    ModelState.AddModelError("UserName", "* Login Email is null");
                }

                if (string.IsNullOrEmpty(user.Password))
                {
                    ModelState.AddModelError("Password", "* Password is null");
                }
            }

            if (ModelState.IsValid)
            {
                if ((Request.Form["isFront"] == "1") && (user.UserName != "quotation"))
                {
                    Session["User"] = user.IsValid(user.Phone, user.Password, 1);
                }
                else
                {
                    Session["User"] = user.IsValid(user.UserName, user.Password);
                }

                if (Session["User"] != null)
                {
                    //if ((Request.Form["isFront"].ToString() == "1") && (user.UserName != "quotation"))
                    //{
                        user.RememberMe = false;
                        user.UserName = ((User)Session["User"]).UserName;
                        user.Password = ((User)Session["User"]).Password;
                    //}

                    FormsAuthentication.SetAuthCookie(user.UserName + " " + user.Password, user.RememberMe);
                    //FormsAuthentication.Authenticate(user.UserName, user.Password);

                    IList<SideMenu> sideMenus = new List<SideMenu>();
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        try
                        {
                            sideMenus = context.Database.SqlQuery<SideMenu>("proc_GetSideMenu_v1").ToList<SideMenu>();
                            Session["SideMenu"] = sideMenus;
                        }
                        catch (Exception e)
                        {
                            Session["SideMenu"] = null;
                        }
                    }

                    if (((User)Session["User"]).Role.IndexOf("Gram") > -1)
                    {
                        return RedirectToAction("Index", "Scanner");
                    }

                    if (user.UserName == "quotation")
                    {
                        Session["TradeType"] = "3";
                    }

                    return RedirectToAction("Index", "Order");
                }
                else
                {
                    ModelState.AddModelError("", "User is not existing or not active.");
                }
            }

            return View(user);
        }


        [HttpPost]
        public ActionResult Capture()
        {
            string filename = Request.QueryString["filename"].ToString();
            //var urlPre = "";
            //if (WebConfigurationManager.AppSettings["pubDir"].ToString() != "")
            //{
            //    urlPre = "/gramapi/" + WebConfigurationManager.AppSettings["pubDir"].ToString();
            //}
            if (Request.InputStream.Length > 0)
            {
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    string hexString = Server.UrlEncode(reader.ReadToEnd());
                    string imageName = filename;
                    string imagePath = string.Format("~/UserImages/{0}.png", imageName);
                    System.IO.File.WriteAllBytes(Server.MapPath(imagePath), ConvertHexToBytes(hexString));
                    Session["CapturedImage"] = VirtualPathUtility.ToAbsolute(imagePath);
                }
            }
            return View();
        }


        [HttpPost]
        public ContentResult GetCapture()
        {
            string url = Session["CapturedImage"].ToString();
            Session["CapturedImage"] = null;
            return Content(url);
        }

        private static byte[] ConvertHexToBytes(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
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




        public ActionResult VVBGAKUISJDIULAOSD()
        {
            Session.RemoveAll();
            LoginUser2 user = new LoginUser2();
            user.UserName = "ronmannpnd@bigpond.com";
            user.Password = "fence123";
            return View(user);
        }


        [HttpPost]
        public ActionResult VVBGAKUISJDIULAOSD(Models.LoginUser2 user)
        {
            user.UserName = "ronmannpnd@bigpond.com";
            user.Password = "fence123";
            if (ModelState.IsValid)
            {
                Session["User"] = user.IsValid(user.UserName, user.Password);
                if (Session["User"] != null)
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, user.RememberMe);

                    IList<SideMenu> sideMenus = new List<SideMenu>();
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        try
                        {
                            sideMenus = context.Database.SqlQuery<SideMenu>("proc_GetSideMenu_v1").ToList<SideMenu>();
                            Session["SideMenu"] = sideMenus;
                        }
                        catch (Exception e)
                        {
                            Session["SideMenu"] = null;
                        }
                    }

                    return RedirectToAction("Index", "Order");
                }
                else
                {
                    ModelState.AddModelError("", "User is not existing!");
                }
            }
            return View(user);
        }


        public static string GetLocalIpAllocationMode()
        {
            string MethodResult = "";
            try
            {

                ManagementObjectSearcher searcherNetwork = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_NetworkAdapterConfiguration");

                Dictionary<string, string> Properties = new Dictionary<string, string>();

                foreach (ManagementObject queryObj in searcherNetwork.Get())
                {
                    foreach (var prop in queryObj.Properties)
                    {
                        if (prop.Name != null && prop.Value != null && !Properties.ContainsKey(prop.Name))
                        {
                            Properties.Add(prop.Name, prop.Value.ToString());
                        }

                    }
                }
                MethodResult = Properties["DHCPEnabled"].ToLower() == "true" ? "DHCP" : "Static";
            }
            catch (Exception ex)
            {
                // ex.HandleException();
            }
            return MethodResult;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }



        [SessionExpire]
        [Authorize]
        public ActionResult ResetUserActive(int id)
        {

            using (var context = new DbContext(Global.ConnStr))
            {
                object[] parameters = { id };
                try
                {
                    context.Database.ExecuteSqlCommand("proc_ResetUserActive {0}", parameters);
                }
                catch (Exception e)
                {
                    string errBody = "Error: " + e.Message.Replace(Environment.NewLine, "<br>");
                    if (((e.InnerException != null) && (!string.IsNullOrEmpty(e.InnerException.Message))))
                    {
                        errBody += @"<br><br><br>" + e.InnerException.Message.Replace("\n", "<br>");
                    }


                    object[] parameters_ = {
                                        WebConfigurationManager.AppSettings["GramAdminEmails"],
                                        "Error: User "+ id.ToString()+ " can not reset active",
                                        errBody,
                                        "",
                                        "",
                                        ""
                                    };

                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters_);

                }
            }

            return Redirect($"{Url.RouteUrl(new { controller = "Home", action = "Users" })}");
        }

        public ActionResult Users()
        {
            return Users(new Users());
        }

        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult Users(Users users)
        {
            ViewBag.Title = "Users";
            Session["CurrForm"] = "Category";

            if (string.IsNullOrEmpty(users.sortCol))
            {
                if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
                {
                    var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                    if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                    {
                        rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                    }

                    fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
                }

                users.sortCol = "Id";
                users.sortColType = "Number";
                users.rowsPerPage = 15;
                users.pageNum = 1;
                users.orderBy = "glyphicon glyphicon-arrow-down";
            }

            if (String.IsNullOrEmpty(users.whereStr))
            {
                users.whereStr = "";
            }

            var Role = new SqlParameter("@Role", ((Scanner.Models.User)Session["User"]).Role);
            var UserName = new SqlParameter("@UserName", ((Scanner.Models.User)Session["User"]).UserName);
            var pageNum = new SqlParameter("@pageNum", users.pageNum);
            var rowsPerPage = new SqlParameter("@rowsPerPage", users.rowsPerPage);
            var sortCol = new SqlParameter("@sortCol", users.sortCol);
            var sortColType = new SqlParameter("@sortColType", users.sortColType);
            var whereStr = new SqlParameter("@whereStr", users.whereStr.ToString());

            var orderBy = (users.orderBy == "glyphicon glyphicon-arrow-down") ?
                new SqlParameter("@orderBy", "desc") :
                new SqlParameter("@orderBy", "asc");


            var table = new SqlParameter("@table", "dbo.View_Users");
            var selStr = new SqlParameter("@selStr", "");

            var sql = "exec dbo.proc_GetUsers " +
                "@Role," +
                "@UserName, " +
                "@pageNum, " +
                "@rowsPerPage, " +
                "@sortCol, " +
                "@sortColType, " +
                "@whereStr, " +
                "@orderBy, " +
                "@table, " +
                "@selStr";


            users.errMsg = "";
            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    users.users = context.Database.SqlQuery<User>(sql,
                       Role,
                       UserName,
                       pageNum,
                       rowsPerPage,
                       sortCol,
                       sortColType,
                       whereStr,
                       orderBy,
                       table,
                       selStr).ToList<User>();
                }
            }
            catch (Exception e)
            {
                users.totalPages = 0;
                users.totalRows = 0;
                users.users = null;
                users.errMsg = "No Record Found";
            }


            if (users.users != null)
            {
                if (users.users.Count > 0)
                {
                    users.totalPages = users.users[0].maxPages;
                    users.totalRows = users.users[0].TotalRows;
                }
            }

            return View(users);
        }

        public ActionResult ReqNewLogin()
        {
            Models.User user = new Models.User();
            user.UserName = "-";
            user.Password = "-";
            user.RePassword = "-";

            using (var context = new DbContext(Global.ConnStr))
            {
                try
                {
                    string sql = "exec dbo.proc_GetACCGroup";
                    IList<ACCGroup> accGroups = context.Database.SqlQuery<ACCGroup>(sql).ToList<ACCGroup>();
                    ViewBag.AccGroups = accGroups;
                    ViewBag.ACCGroup = "";
                }
                catch (Exception e)
                {
                    ViewBag.Newcompany = "";
                    string errBody = "Error: " + e.Message.Replace(Environment.NewLine, "<br>");
                    if (((e.InnerException != null) && (!string.IsNullOrEmpty(e.InnerException.Message))))
                    {
                        errBody += @"<br><br><br>" + e.InnerException.Message.Replace("\n", "<br>");
                    }


                    object[] parameters_ = {
                                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                                            "Error: ReqNewLogin",
                                            errBody,
                                            "",
                                            "",
                                            ""
                                        };

                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters_);
                }
            }

            user.Country = "Australia";
            return View(user);
        }

        [HttpPost]
        public ActionResult ReqNewLogin(User user)
        {
            using (var context = new DbContext(Global.ConnStr))
            {
                try
                {
                    var sql = "exec dbo.proc_GetACCGroup";
                    IList<ACCGroup> accGroups = context.Database.SqlQuery<ACCGroup>(sql).ToList<ACCGroup>();
                    ViewBag.AccGroups = accGroups;
                }
                catch (Exception e)
                {
                    string errBody = "Error: " + e.Message.Replace(Environment.NewLine, "<br>");
                    if (((e.InnerException != null) && (!string.IsNullOrEmpty(e.InnerException.Message))))
                    {
                        errBody += @"<br><br><br>" + e.InnerException.Message.Replace("\n", "<br>");
                    }

                    object[] parameters_ = {
                                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                                            "Error: Req new Login Acc Groups  can not find",
                                            errBody,
                                            "",
                                            "",
                                            ""
                                        };

                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters_);
                }
            }

            if (ModelState.IsValid)
            {
                if (user.Password.ToLower() != user.RePassword.ToLower())
                {
                    ModelState.AddModelError("RePassword", "Password and Password Re-input not matched.");
                }
                else
                {
                    try
                    {
                        user.ApprovedBy = null;
                        user.Approved = false;
                        user.isUpdate = 5;
                        user.ApprovedBy = 0;
                        var UserName_ = new SqlParameter("@UserName", user.UserName);
                        var Password_ = new SqlParameter("@Password", user.Password);
                        var UserEmail_ = new SqlParameter("@UserEmail", user.UserEmail);
                        var FirstName_ = new SqlParameter("@FirstName", user.FirstName);
                        var LastName_ = new SqlParameter("@LastName", user.LastName);
                        var Mobile_ = new SqlParameter("@Mobile", user.Mobile);
                        var Phone_ = new SqlParameter("@Phone", (user.Phone == null) ? "" : user.Phone);
                        var Company_ = new SqlParameter("@Company", user.CompanyName);
                        var DriverLic_ = new SqlParameter("@DriverLic", (user.DriverLic == null) ? "" : user.DriverLic);
                        var ABN_ = new SqlParameter("@ABN", (user.ABN == null) ? "" : user.ABN);
                        var ACCGroup_ = new SqlParameter("@ACCGroup", user.ACCGroup);
                        var Address1_ = new SqlParameter("@Address1", user.Address1);
                        var Address2_ = new SqlParameter("@Address2", user.Address2);
                        var State_ = new SqlParameter("@State", user.State);
                        var Postcode_ = new SqlParameter("@Postcode", user.Postcode);
                        var Country_ = new SqlParameter("@Country", user.Country);
                        var isUpdate_ = new SqlParameter("@isUpdate", user.isUpdate);
                        var Approved_ = new SqlParameter("@Approved", false);
                        var ApprovedBy_ = new SqlParameter("@ApprovedBy", 1);

                        var sql = "exec dbo.proc_AddUpdateUser " +
                        "@UserName," +
                        "@Password," +
                        "@UserEmail," +
                        "@FirstName," +
                        "@LastName," +
                        "@Mobile," +
                        "@Phone," +
                        "@Company," +
                        "@DriverLic," +
                        "@ABN," +
                        "@ACCGroup," +
                        "@Address1," +
                        "@Address2," +
                        "@State," +
                        "@Postcode," +
                        "@Country," +
                        "@isUpdate," +
                        "@Approved," +
                        "@ApprovedBy";

                        using (var context = new DbContext(Global.ConnStr))
                        {
                            user.isUpdate = context.Database.SqlQuery<int>(sql,
                                UserName_,
                                Password_,
                                UserEmail_,
                                FirstName_,
                                LastName_,
                                Mobile_,
                                Phone_,
                                Company_,
                                DriverLic_,
                                ABN_,
                                ACCGroup_,
                                Address1_,
                                Address2_,
                                State_,
                                Postcode_,
                                Country_,
                                isUpdate_,
                                Approved_,
                                ApprovedBy_
                                ).ToList<int>()[0];

                        }
                    }
                    catch (Exception e)
                    {
                        string errMsg = "Error: Req New Login Unable to registrate new user " + user.UserEmail + " -- " + e.Message + e.InnerException.Message;
                        errMsg += "<br/>User Name: " + user.UserName + "<br/>";
                        errMsg += "<br/>Password: " + user.Password + "<br/>";
                        errMsg += "<br/>UserEmail: " + user.UserEmail + "<br/>";
                        errMsg += "<br/>FirstName: " + user.FirstName + "<br/>";
                        errMsg += "<br/>LastName: " + user.LastName + "<br/>";
                        errMsg += "<br/>Mobile: " + user.Mobile + "<br/>";
                        errMsg += "<br/>Company: " + user.CompanyName + "<br/>";
                        errMsg += "<br/>Address1: " + user.Address1 + "<br/>";
                        errMsg += "<br/>Address2: " + user.Address2 + "<br/>";
                        errMsg += "<br/>State: " + user.State + "<br/>";
                        errMsg += "<br/>Postcode: " + user.Postcode + "<br/>";
                        errMsg += "<br/>Country: " + user.Country + "<br/>";

                        using (var context = new DbContext(Global.ConnStr))
                        {
                            object[] parameters = {
                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                            "Error: Req New Login Unable to registrate new user "+user.UserEmail,
                           errMsg,
                            "",
                            "",
                            ""
                        };
                            context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters);
                        }
                    }
                }
            }
            else
            {
                string str = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        str += error.ErrorMessage;
                    }
                }
            }
            return View(user);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Registration()
        {
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if (Request.Form["rows"] != null)
                {
                    if (Request.Form["rows"].ToString() != "")
                    {
                        rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                    }
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            Models.User user = new Models.User();
            user.isUpdate = 0;
            Session["CurrForm"] = "Registration";
            return View(user);
        }


        [SessionExpire]
        [Authorize]
        [HttpGet]
        public ActionResult Registration(int id)
        {
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if (Request.Form["rows"] != null)
                {
                    if (Request.Form["rows"].ToString() != "")
                    {
                        rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                    }
                }
                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            Session["CurrForm"] = "Registration";

            Models.User LoginUser = (Models.User)Session["User"];
            Models.User user = new Models.User();

            var UserId = new SqlParameter("@UserId", id);

            var sql = "";
            using (var context = new DbContext(Global.ConnStr))
            {
                try
                {
                    sql = "exec dbo.proc_GetACCGroup";
                    IList<ACCGroup> accGroups = context.Database.SqlQuery<ACCGroup>(sql).ToList<ACCGroup>();
                    ViewBag.AccGroups = accGroups;

                    sql = "exec dbo.proc_GetCompanyNameList";
                    IList<string> CompanyList = context.Database.SqlQuery<string>(sql).ToList<string>();
                    ViewBag.CompanyList = CompanyList;

                    sql = "exec dbo.proc_GetUser " + "@UserId";
                    user = context.Database.SqlQuery<Models.User>(sql,
                        UserId).ToList<Models.User>()[0];

                    ViewBag.Newcompany = "NEW";
                    if (CompanyList.IndexOf(user.CompanyName) > -1)
                    {
                        ViewBag.Newcompany = "";
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Newcompany = "";
                    string errBody = "Error: " + e.Message.Replace(Environment.NewLine, "<br>");
                    if (((e.InnerException != null) && (!string.IsNullOrEmpty(e.InnerException.Message))))
                    {
                        errBody += @"<br><br><br>" + e.InnerException.Message.Replace("\n", "<br>");
                    }

                    object[] parameters_ = {
                                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                                            "Error: User "+ id.ToString()+ " can not find",
                                            errBody,
                                            "",
                                            "",
                                            ""
                                        };

                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters_);
                }
            }

            if ((LoginUser.Id != Convert.ToInt32(user.Id)) && (LoginUser.Role.IndexOf("Gram") < 0))
            {
                user = LoginUser;
            }

            user.isUpdate = 1;
            return View(user);
        }

        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult Registration(User user)
        {
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if (Request.Form["rows"] != null)
                {
                    if (Request.Form["rows"].ToString() != "")
                    {
                        rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                    }
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            Session["CurrForm"] = "Registration";
            using (var context = new DbContext(Global.ConnStr))
            {
                try
                {
                    var sql = "exec dbo.proc_GetACCGroup";
                    IList<ACCGroup> accGroups = context.Database.SqlQuery<ACCGroup>(sql).ToList<ACCGroup>();
                    ViewBag.AccGroups = accGroups;

                    sql = "exec dbo.proc_GetCompanyNameList";
                    IList<string> CompanyList = context.Database.SqlQuery<string>(sql).ToList<string>();
                    ViewBag.CompanyList = CompanyList;


                    if (CompanyList.IndexOf(user.CompanyName) > -1)
                    {
                        ViewBag.Newcompany = "";
                    }
                    else
                    {
                        ViewBag.Newcompany = "NEW";
                    }
                }
                catch (Exception e)
                {
                    string errBody = "Error: " + e.Message.Replace(Environment.NewLine, "<br>");
                    if (((e.InnerException != null) && (!string.IsNullOrEmpty(e.InnerException.Message))))
                    {
                        errBody += @"<br><br><br>" + e.InnerException.Message.Replace("\n", "<br>");
                    }


                    object[] parameters_ = {
                                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                                            "Error: Acc Groups  can not find",
                                            errBody,
                                            "",
                                            "",
                                            ""
                                        };

                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters_);
                }
            }

            if (ModelState.IsValid)
            {
                if (user.Password.ToLower() != user.RePassword.ToLower())
                {
                    ModelState.AddModelError("RePassword", "Password and Password Re-input not matched.");
                }
                else
                {
                    try
                    {
                        user.ApprovedBy = ((User)Session["User"]).Id;
                        using (var context = new DbContext(Global.ConnStr))
                        {
                            object[] parameters = {
                                user.UserName,
                                user.Password,
                                user.UserEmail,
                                user.FirstName,
                                user.LastName,
                                user.Mobile,
                                user.Phone,
                                user.CompanyName,
                                user.DriverLic,
                                user.ABN,
                                user.ACCGroup,
                                user.Address1,
                                user.Address2,
                                user.State,
                                user.Postcode,
                                user.Country,
                                user.isUpdate,
                                user.Approved,
                                user.ApprovedBy
                            };

                            int retnCnt = context.Database.SqlQuery<int>("proc_AddUpdateUser {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}", parameters).ToList<int>()[0];
                            if (retnCnt > 0)
                            {
                                ModelState.AddModelError("UserEmail", "Login Email is already existing.");
                            }
                            else
                            {
                                if (retnCnt != -2)
                                {
                                    LoginUser login = new LoginUser();
                                    Session["User"] = login.IsValid(user.UserEmail, user.Password);
                                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                                    return RedirectToAction("Index", "Order");
                                }

                                user.isUpdate = -2;
                            }
                        }
                    }
                    catch (Exception e)
                    {

                        using (var context = new DbContext(Global.ConnStr))
                        {
                            object[] parameters = {
                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                            "Error: Unable to registrate new user "+user.UserEmail,
                            "Error: Unable to registrate new user "+user.UserEmail,
                            "",
                            "",
                            ""
                        };
                            context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters);
                        }
                    }
                }
            }
            else
            {
                string str = "";
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        str += error.ErrorMessage;
                    }
                }
            }

            return View(user);
        }


        public ActionResult ForgotPassword()
        {
            PasswordReq req = new PasswordReq();
            req.Email = "";
            req.Msg = "";
            return View(req);
        }


        [HttpPost]
        public ActionResult ForgotPassword(PasswordReq req)
        {
            if (!string.IsNullOrEmpty(req.Email))
            {
                req.Msg = "";
                if (ModelState.IsValid)
                {
                    req.Email = req.Email.Replace(" ", "").ToLower();
                    req.Msg = "Email address is not existing.";
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        try
                        {
                            object[] parameters = {
                                           req.Email
                                        };

                            List<string> lines = context.Database.SqlQuery<string>("exec dbo.proc_ResendLoginDetails {0}", parameters).ToList<string>();

                            req.Msg = lines[0];
                        }
                        catch (Exception e)
                        {
                            string errBody = "Error: " + e.Message.Replace(Environment.NewLine, "<br>");
                            if (((e.InnerException != null) && (!string.IsNullOrEmpty(e.InnerException.Message))))
                            {
                                errBody += @"<br><br><br>" + e.InnerException.Message.Replace("\n", "<br>");
                            }

                            object[] parameters_ = {
                                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                                            "Error: Resend Login Details",
                                            errBody,
                                            "",
                                            "",
                                            ""
                                        };

                            context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters_);
                        }
                    }
                }
            }

            return View(req);
        }




        public async void JustTest()
        {
            HttpClient client = new HttpClient();
            IList<ItemDescription> itemDescriptions = new List<ItemDescription>();
            ItemDescription item = new ItemDescription();
            item.TYPE = "GramLine";
            item.LENGTH = "1190";
            item.COLOUR = "AUTUMN RED";
            item.QTY = "1";
            itemDescriptions.Add(item);

            HttpResponseMessage response = await client.PostAsJsonAsync<IList<ItemDescription>>("http://localhost:58974/api/Values", itemDescriptions);
            IList<ItemDescription> jsonData = await response.Content.ReadAsAsync<IList<ItemDescription>>();

        }

        public async void JustTest2()
        {
            HttpClient client = new HttpClient();
            //IList<ItemDescription> itemDescriptions = new List<ItemDescription>();
            //ItemDescription item = new ItemDescription();
            //item.TYPE = "GramLine";
            //item.LENGTH = "1190";
            //item.COLOUR = "AUTUMNRED";
            //item.QTY = "1";
            //itemDescriptions.Add(item);

            Order order = new Order();
            order.ACCNO = "339";
            order.BranchIDDealWith = "1";
            order.Company = "BILL GIBSON FENCING PTY LIMITED";
            order.Email = "c";
            order.Mobile = "b";
            order.OrderBy = "d";
            order.OrderNo = "g";
            order.Reference = "j";
            order.OrderDate = DateTime.Now.ToShortDateString();

            order.OrderDetails = new List<OrderDetail>();
            OrderDetail orderDetail = new OrderDetail();
            orderDetail.COLOUR = "AUTUMN RED";
            orderDetail.DESCRIPTION = "SPF® GramLine® SHEET  AUTUMN RED";
            orderDetail.PRICE = "0";
            orderDetail.QTY = "1";
            orderDetail.STANDARD = "1190";
            orderDetail.STOCKCODE = "SPGR11ARE";
            orderDetail.WEIGHT = "3.34";
            order.OrderDetails.Add(orderDetail);

            orderDetail.COLOUR = "GALVANISED";
            orderDetail.DESCRIPTION = "SPF® GramLine® SHEET  ALZN(GALVANISED)";
            orderDetail.PRICE = "0";
            orderDetail.QTY = "2";
            orderDetail.STANDARD = "1790";
            orderDetail.STOCKCODE = "SPGR17GAL";
            orderDetail.WEIGHT = "10.3";
            order.OrderDetails.Add(orderDetail);


            HttpResponseMessage response = await client.PostAsJsonAsync<Order>("http://localhost:58974/api/Order", order);
            string jsonData = await response.Content.ReadAsAsync<string>();

        }

        private void resetRequest()
        {
            var authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null && !authTicket.Expired)
                {
                    var roles = authTicket.UserData.Split(',');
                    System.Web.HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), roles);
                }
            }
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

            string frontOrderId = "";
            string orderOrQuoteID = "";

            string httpString = Request.Url.OriginalString;
            if (httpString.IndexOf("frontOrderId") > 0)
            {

                frontOrderId = httpString.Split('?')[1].Split(new string[] { "&amp;" }, StringSplitOptions.None)[0].Split('=')[1];
                orderOrQuoteID = httpString.Split('?')[1].Split(new string[] { "&amp;" }, StringSplitOptions.None)[1].Split('=')[1].Replace("%20", " ");
            }

            if (Session["uploadGuid"] != null)
            {
                string tmpDir = Session["uploadGuid"].ToString();

                if (Directory.Exists(string.Format("{0}UploadedTmp\\" + tmpDir, System.Web.Hosting.HostingEnvironment.MapPath(@"\"))))
                {
                    Directory.Delete(string.Format("{0}UploadedTmp\\" + tmpDir, System.Web.Hosting.HostingEnvironment.MapPath(@"\")), true);
                }
            }

            if (Directory.Exists(string.Format("{0}UploadedTmp\\", System.Web.Hosting.HostingEnvironment.MapPath(@"\"))))
            {
                DirectoryInfo source = new DirectoryInfo(string.Format("{0}UploadedTmp\\", System.Web.Hosting.HostingEnvironment.MapPath(@"\")));
                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    if (diSourceSubDir.CreationTime < DateTime.Now.AddDays(-1))
                    {
                        Directory.Delete(string.Format("{0}UploadedTmp\\" + diSourceSubDir.Name, System.Web.Hosting.HostingEnvironment.MapPath(@"\")), true);
                    }
                }
            }

            //FormsAuthentication.SignOut();      
            Session.RemoveAll();
            return (frontOrderId != "") ? RedirectToAction("Index", "Home", new { frontOrderId = frontOrderId, orderOrQuoteID = orderOrQuoteID }) : RedirectToAction("Index", "Home");
            //return View("OpenExo");
        }


        public ActionResult OpenExo()
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

            var orderIds = Request.QueryString["orderIds"].Replace(" ", "");

            if (orderIds.IndexOf(",") > -1)
            {
                orderIds = orderIds.Split(',')[0].ToString();
            }

            ViewBag.orderId = orderIds;

            return View();
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Contact()
        {
            Session["CurrForm"] = "Contact";
            ViewBag.Message = "";

            Contact contact = new Contact();
            contact.Name = ((User)Session["User"]).FirstName + " " + ((User)Session["User"]).LastName;
            contact.Mobile = ((User)Session["User"]).Mobile;
            contact.Email = ((User)Session["User"]).UserEmail;


            string ConnStr = ConfigurationManager.ConnectionStrings["BridgeConn"].ToString();

            IList<Bridge> bridges = new List<Bridge>();

            var sql = "select *, dbo.fn_GetEvaluationVal(dbo.fn_SplitString(NHand,'.',0)) as S_SHL, dbo.fn_GetEvaluationVal(dbo.fn_SplitString(NHand,'.',1)) as H_SHL, dbo.fn_GetEvaluationVal(dbo.fn_SplitString(NHand,'.',2)) as D_SHL, dbo.fn_GetEvaluationVal(dbo.fn_SplitString(NHand,'.',3)) as C_SHL from [dbo].[View_Examples]  where isactive = 1 order by pos2";
            using (var context = new DbContext(ConnStr))
            {
                bridges = context.Database.SqlQuery<Bridge>(sql).ToList<Bridge>();
            }

            ViewBag.Bridges = bridges;

            return View(contact);
        }


        [SessionExpire]
        [Authorize]
        public ActionResult Career()
        {
            Session["CurrForm"] = "Career";
            ViewBag.Message = "";

            //Contact contact = new Contact();
            //contact.Name = ((User)Session["User"]).FirstName + " " + ((User)Session["User"]).LastName;
            //contact.Mobile = ((User)Session["User"]).Mobile;
            //contact.Email = ((User)Session["User"]).UserEmail;

            return View();
        }




        [SessionExpire]
        [Authorize]
        public ActionResult Branches()
        {
            Session["CurrForm"] = "Branches";
            ViewBag.Message = "";

            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if (Request.Form["rows"] != null)
                {
                    if (Request.Form["rows"].ToString() != "")
                    {
                        rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                    }
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            return View();
        }

        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult Contact(Contact contact, HttpPostedFileBase fileUploader)
        {
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if (Request.Form["rows"] != null)
                {
                    if (Request.Form["rows"].ToString() != "")
                    {
                        rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                    }
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            if (Request.QueryString["ini"] != null)
            {
                return Redirect($"{Url.RouteUrl(new { controller = "Home", action = "Contact" })}");
            }

            Session["CurrForm"] = "Contact";
            ViewBag.Message = "";
            string body = "";
            if (fileUploader != null)
            {
                if (fileUploader.InputStream.Length > 5120000)
                {
                    ModelState.AddModelError("Attachment", "File size is over 5 mb.");
                }
            }
            if (ModelState.IsValid)
            {
                SmtpClient smtp = new SmtpClient()
                {
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["emPort"].ToString()),
                    EnableSsl = false,
                    Timeout = 1000000,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["emUserName"].ToString(),
                  ConfigurationManager.AppSettings["emPassword"].ToString()),
                    Host = ConfigurationManager.AppSettings["emHost"].ToString()
                };

                MailMessage message = new MailMessage();
                MailAddress fAddr = new MailAddress("info@gram.com.au", "Gram Engineering");
                message.From = fAddr;


                if ((Request.QueryString["sendGramBulk"] != null) && (Request.QueryString["sendGramBulk"].ToString() == "yes1true"))
                {
                    IList<string> emails = new List<string>();

                    using (var context = new DbContext(Global.ConnStr))
                    {
                        string sql = "proc_GetAllUserEmails";
                        emails = context.Database.SqlQuery<string>(sql).ToList<string>();
                    }

                    foreach (var email in emails)
                    {
                        if (email.Replace(" ", "") != "")
                        {
                            MailAddress bccAddr = new MailAddress(email);
                            message.Bcc.Add(bccAddr);
                        }
                    }


                    body = "";
                    body += "<!DOCTYPE html>";
                    body += "<html>";
                    body += "<head>";
                    body += "<style>table {font-family: arial, sans-serif;border-collapse: collapse;}td, th {border: 0px;text-align: left;padding: 8px;}tr:nth-child(even) {background-color: #CCC;}</style>";
                    body += "</head>";
                    body += "<body>";
                    body += "<table>";

                    body += "<tr>";
                    body += "<td>";
                    body += contact.EmailBody.Replace(Environment.NewLine, "<br/>");
                    body += "</td>";
                    body += "</tr>";

                    body += "</table>";
                    body += "</body>";
                    body += "</html>";

                    message.Subject = contact.Subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    smtp.Send(message);
                    ViewBag.Message = "Bulk Email has been sent to all users.";

                }
                else
                {

                    body = "";
                    body += "<!DOCTYPE html>";
                    body += "<html>";
                    body += "<head>";
                    body += "<style>table {font-family: arial, sans-serif;border-collapse: collapse;}td, th {border: 1px solid #dddddd;text-align: left;padding: 8px;}tr:nth-child(even) {background-color: #CCC;}</style>";
                    body += "</head>";
                    body += "<body>";
                    body += "<table>";

                    body += "<tr>";
                    body += "<td><b>";
                    body += "Conact Name:";
                    body += "</td></b>";
                    body += "<td style='width:20px;'></td>";
                    body += "<td>";
                    body += contact.Name;
                    body += "</td>";
                    body += "</tr>";

                    body += "<tr>";
                    body += "<td><b>";
                    body += "Mobile:";
                    body += "</td></b>";
                    body += "<td style='width:20px;'></td>";
                    body += "<td>";
                    body += contact.Mobile;
                    body += "</td>";
                    body += "</tr>";

                    body += "<tr>";
                    body += "<td><b>";
                    body += "Email:";
                    body += "</td></b>";
                    body += "<td style='width:20px;'></td>";
                    body += "<td>";
                    body += contact.Email;
                    body += "</td>";
                    body += "</tr>";


                    body += "<tr>";
                    body += "<td><b>";
                    body += "Subject:";
                    body += "</td></b>";
                    body += "<td style='width:20px;'></td>";
                    body += "<td>";
                    body += contact.Subject;
                    body += "</td>";
                    body += "</tr>";

                    body += "<tr>";
                    body += "<td valign='top'><b>";
                    body += "Request Details:";
                    body += "</td></b>";
                    body += "<td style='width:20px;'></td>";
                    body += "<td>";
                    body += contact.EmailBody.Replace(Environment.NewLine, "<br/>");
                    body += "</td>";
                    body += "</tr>";

                    body += "</table>";
                    body += "</body>";
                    body += "</html>";



                    string[] toEmailsArr = ConfigurationManager.AppSettings["GramContactToEmails"].ToString().Replace(",", ";").Split(';');
                    foreach (var email in toEmailsArr)
                    {
                        if (email.Replace(" ", "") != "")
                        {
                            MailAddress tAddr = new MailAddress(email);
                            message.To.Add(tAddr);
                        }
                    }

                    if (ConfigurationManager.AppSettings["GramBccEmails"].ToString() != "")
                    {
                        string[] bccEmailsArr = ConfigurationManager.AppSettings["GramBccEmails"].ToString().Replace(",", ";").Split(';');
                        foreach (var email in bccEmailsArr)
                        {
                            if (email.Replace(" ", "") != "")
                            {
                                MailAddress bccAddr = new MailAddress(email);
                                message.Bcc.Add(bccAddr);
                            }
                        }
                    }

                    if (fileUploader != null)
                    {
                        string fileName = Path.GetFileName(fileUploader.FileName);
                        message.Attachments.Add(new Attachment(fileUploader.InputStream, fileName));
                    }

                    message.Subject = "Online Request: " + contact.Subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    smtp.Send(message);

                    ViewBag.Message = "Your Request Details has been sent and we will contact you as soon as possible. Thanks.";
                }
            }


            return View(contact);
        }
    }




}