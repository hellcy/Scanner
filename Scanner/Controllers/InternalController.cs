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

namespace GramEngineering.Controllers
{
    public class InternalController : Controller
    {
        public ActionResult Index()
        {
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                //  var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    var rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                    fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
                }
            }

            Session["CurrForm"] = "Category";
            return View();
        }

        //[SessionExpire]
        //[Authorize]
        //[HttpPost]
        //public void Upload(string id)
        //{
        //    for (int i = 0; i < Request.Files.Count; i++)
        //    {
        //        var file = Request.Files[i];

        //        var fileName = Path.GetFileName(file.FileName);

        //        var path = Path.Combine(Server.MapPath("~/Junk/"), fileName);
        //        file.SaveAs(path);
        //    }
        //}

        [SessionExpire]
        [Authorize]
        public ActionResult Uploads()
        {
            if (Session["uploadGuid"] == null)
            {
                Session["uploadGuid"] = Guid.NewGuid().ToString();
            }

            ViewBag.ExistingFiles = "";
            ViewBag.uploadGuid = "";
            if (Session["uploadGuid"] != null)
            {
                ViewBag.ExistingFiles = "";
                ViewBag.uploadGuid = Session["uploadGuid"].ToString();

                DirectoryInfo dir = null;

                if (Session["uploadGuid"].ToString().IndexOf("GRAM_INTERNAL") > -1)
                {
                    dir = new System.IO.DirectoryInfo(string.Format("{0}" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")));
                }
                else
                {
                    dir = new System.IO.DirectoryInfo(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")));
                }

                try
                {
                    FileInfo[] fileInformations = dir.GetFiles();
                    FileInfo fi;
                    for (int i = 0; i < fileInformations.Length; i++)
                    {
                        fi = fileInformations[i];
                        ViewBag.ExistingFiles += fi.Name + "," + fileSize(fi.Length) + ";";
                    }
                }
                catch (Exception e)
                {
                    ViewBag.ExistingFiles = "";
                }
            }
            else
            {
                ViewBag.ExistingFiles = "";
                ViewBag.uploadGuid = "";
            }

            //ViewBag.path = string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\"));
            //ViewBag.path2 = System.Web.Hosting.HostingEnvironment.MapPath(@"\");
            return View();
        }

        private string fileSize(long len)
        {
            string size = "";

            if (len >= 1024)
            {
                double kbs = Convert.ToDouble(len / 1024);

                if (kbs >= 1024)
                {
                    size = Convert.ToInt32(kbs / 1024).ToString() + " MB";
                }
                else
                {
                    size = Math.Round(kbs, 2).ToString() + " KB";
                }
            }
            else
            {
                size = len.ToString() + " B";
            }

            return size;
        }


        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult SaveUploadedFile()
        {
            bool isSavedSuccessfully = true;
            string fName = "";

            //if (Session["uploadGuid"] == null) {
            //    Session["uploadGuid"] = User.Identity.Name.Split(" ") + "-"+   Guid.NewGuid().ToString();
            //}

            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName; // Guid.NewGuid().ToString(); 
                    if (file != null && file.ContentLength > 0)
                    {

                        var originalDirectory = new System.IO.DirectoryInfo(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")));
                        string pathString = originalDirectory.ToString();
                        var fileName1 = System.IO.Path.GetFileName(file.FileName);
                        bool isExists = System.IO.Directory.Exists(pathString);
                        if (!isExists)
                            System.IO.Directory.CreateDirectory(pathString);
                        var path = string.Format("{0}\\{1}", pathString, fName);
                        file.SaveAs(path);
                    }
                }
            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }

            if (isSavedSuccessfully)
                return Json(new { Message = fName });
            else
                return Json(new { Message = "Error in saving file" });
        }



        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult Folders(FolderFile folderFile)
        {

            var role = ((Scanner.Models.User)Session["User"]).Role;
            //string path = Server.MapPath(@"~/GRAM_INTERNAL");
            //DirectoryInfo dir = new DirectoryInfo(path);

            if (role.IndexOf("Gram") > -1)
            {
                string MainFolder = ConfigurationManager.AppSettings["MainFolder"].ToString();
                DirectoryInfo dir = new DirectoryInfo(@MainFolder);
                IList<string> folderAndFiles = new List<string>();
                IList<string> names = new List<string>();
                int idx = 0;
                folderAndFiles.Add("<ul>");
                AddDirectories(ref folderAndFiles, ref names, ref idx, dir);
                folderAndFiles.Add("</ul>");

                FolderFiles ff = new FolderFiles();
                ff.folderAndFiles = folderAndFiles;
                ff.names = names;
                ViewBag.Title = "Folders";
                ViewBag.ff = ff;

                return View(folderFile);
            }
            else
            {
                return Redirect($"{Url.RouteUrl(new { controller = "Home", action = "Logout" })}");
            }
        }

        private void AddDirectories(ref IList<string> folderAndFiles, ref IList<string> names, ref int idx, DirectoryInfo dir)
        {
            string path = "";
            if (dir.GetDirectories().Count() > 0)
            {
                folderAndFiles.Add("<li id='df_" + idx.ToString() + "'><a id='af_" + idx.ToString() + "' onclick='openDir(this.id)' data='" + dir.FullName.Replace(@"C:\", @"\").Replace(@"\", "/") + "'>" + dir.Name + "</a><ul>");
                names.Add(dir.Name);
                idx++;
                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    AddDirectories(ref folderAndFiles, ref names, ref idx, d);
                }


                if (dir.GetFiles().Count() > 0)
                {
                    foreach (FileInfo f in dir.GetFiles())
                    {
                        // folderAndFiles.Add("<li><a href='#' onclick='$(\"#byClick\").val(\"1\");window.open(\""+@f.FullName+"\")'>" + f.Name + "</a></li>");
                        // folderAndFiles.Add("<li><a href='file:/" + @f.FullName + "'>" + f.Name + "</a></li>");
                        path = f.FullName.Replace(@"C:\", @"\GramEngineering\").Replace(@"\", "/");
                        folderAndFiles.Add("<li id='df_" + idx.ToString() + "'><a id='af_" + idx.ToString() + "' onclick='openFile(this.id)' data='" + path + "'>" + f.Name + "</a></li>");
                        names.Add(f.Name);
                        idx++;
                    }
                }

                folderAndFiles.Add("</ul></li>");
            }
            else
            {

                folderAndFiles.Add("<li id='df_" + idx.ToString() + "'><a id='af_" + idx.ToString() + "'  onclick='openDir(this.id)' data='" + dir.FullName.Replace(@"C:\", @"\").Replace(@"\", "/") + "'>" + dir.Name + "</a><ul>");
                names.Add(dir.Name);
                idx++;
                if (dir.GetFiles().Count() > 0)
                {
                    foreach (FileInfo f in dir.GetFiles())
                    {
                        // folderAndFiles.Add("<li><a href='file:/"+@f.FullName+"'>" + f.Name + "</a></li>");

                        // folderAndFiles.Add("<li><a onclick='openFile(\""+ @f.FullName.Replace(@"\", "/") + "\")'>" + f.Name + "</a></li>");
                        path = f.FullName.Replace(@"C:\", @"\GramEngineering\").Replace(@"\", "/");
                        folderAndFiles.Add("<li id='df_" + idx.ToString() + "'><a id='af_" + idx.ToString() + "' onclick='openFile(this)' data='" + path + "'>" + f.Name + "</a></li>");
                        names.Add(f.Name);
                        idx++;

                    }
                }
                folderAndFiles.Add("</ul></li>");
                var aa = folderAndFiles.Count();
            }
        }

        public void fillCurrTable(string currForm, int rowsCnt)
        {
            if ((Session["newOrder"] != null) && (Request.Form["Company"] != null))
            {
                ((NewOrder)Session["newOrder"]).Head.Mobile = Request.Form["Mobile"].ToString();
                ((NewOrder)Session["newOrder"]).Head.Company = Request.Form["Company"].ToString();
                ((NewOrder)Session["newOrder"]).Head.OrderBy = Request.Form["Orderby"].ToString();
                ((NewOrder)Session["newOrder"]).Head.Email = Request.Form["Email"].ToString();
                ((NewOrder)Session["newOrder"]).Head.CustomerOrderNo = Request.Form["CustomerOrderNo"].ToString();
                ((NewOrder)Session["newOrder"]).Head.RequestForDelivery = Convert.ToBoolean(Request.Form["RequestForDelivery"]);
                ((NewOrder)Session["newOrder"]).Head.Reference = Request.Form["Reference"].ToString();
                ((NewOrder)Session["newOrder"]).Head.DispatchDate = Request.Form["DispatchDate"].ToString();
                ((NewOrder)Session["newOrder"]).Head.PickupBy = Request.Form["PickupBy"].ToString();
                ((NewOrder)Session["newOrder"]).Head.DriverLic = (Request.Form["DriverLic"] != null) ? Request.Form["DriverLic"].ToString() : "";
                ((NewOrder)Session["newOrder"]).Head.ABN = (Request.Form["ABN"] != null) ? Request.Form["ABN"].ToString() : "";
                ((NewOrder)Session["newOrder"]).Head.Address = (Request.Form["Address"] != null) ? Request.Form["Address"].ToString() : "";
                ((NewOrder)Session["newOrder"]).Head.City = (Request.Form["City"] != null) ? Request.Form["City"].ToString() : "";
                ((NewOrder)Session["newOrder"]).Head.State = (Request.Form["State"] != null) ? Request.Form["State"].ToString() : "";
                ((NewOrder)Session["newOrder"]).Head.Postcode = (Request.Form["Postcode"] != null) ? Request.Form["Postcode"].ToString() : "";
                ((NewOrder)Session["newOrder"]).Head.Country = (Request.Form["Country"] != null) ? Request.Form["Country"].ToString() : "";
            }

            if ((currForm != "CheckOut") &&
                (currForm != "Confirmation") &&
                (currForm != "QuotationTrack") &&
                (currForm != "OrderTrack") &&
                (currForm != "EnterMeterage") &&
                (currForm != "EnterNoOfPanels") &&
                (currForm != "Contact") &&
                (currForm != "Category") &&
                (currForm != "Registration") &&
                (currForm.Substring(0, 2) != "C-"))
            {
                for (int i = 0; i < rowsCnt; i++)
                {
                    if (i == ((DataTable)Session[currForm]).Rows.Count)
                    {
                        DataRow ndr = ((DataTable)Session[currForm]).NewRow();
                        ((DataTable)Session[currForm]).Rows.Add(ndr);
                    }

                    for (int j = 0; j < ((DataTable)Session[currForm]).Columns.Count; j++)
                    {
                        ((DataTable)Session[currForm]).Rows[i][j] = Request.Form["cR" + i.ToString() + "C" + j.ToString()];
                    }
                }

                if (Request.Form["allCuts"] != null)
                {
                    if (Request.Form["allCuts"].ToString() != "")
                    {
                        Session[currForm + "AllCuts"] = Request.Form["allCuts"].ToString();
                    }
                    else
                    {
                        Session[currForm + "AllCuts"] = null;
                    }
                }
            }
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
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            if (Request.QueryString["ini"] != null)
            {
                return Redirect($"{Url.RouteUrl(new { controller = "Home", action = "Contact" })}");
            }

            Session["CurrForm"] = "Contact";
            ViewBag.Message = "";

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

                string body = "";
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


                message.Subject = "Online Request: " + contact.Subject;
                message.Body = body;
                message.IsBodyHtml = true;
                smtp.Send(message);

                ViewBag.Message = "Your Request Details has been sent and we will contact you as soon as possible. Thanks.";
            }


            return View(contact);
        }
    }
}