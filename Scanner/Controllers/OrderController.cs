using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using Scanner.Models;
using static Scanner.FilterConfig;
using System.IO;

namespace Scanner.Controllers
{
    public class OrderController : BaseController
    {
        [SessionExpire]
        [Authorize]
        public ActionResult Index()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }

            Session["UseSessionMenuExpandings"] = null;


            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }
                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }
            else
            {
                if ((Session["newOrder"] != null) && (Request.Form["Company"] != null))
                {
                    ((NewOrder)Session["newOrder"]).Head.Mobile = Request.Form["Mobile"].ToString();
                    ((NewOrder)Session["newOrder"]).Head.Company = Request.Form["Company"].ToString();
                    ((NewOrder)Session["newOrder"]).Head.OrderBy = Request.Form["Orderby"].ToString();
                    ((NewOrder)Session["newOrder"]).Head.Email = Request.Form["Email"].ToString();
                    ((NewOrder)Session["newOrder"]).Head.CustomerOrderNo = Request.Form["CustomerOrderNo"].ToString();
                    ((NewOrder)Session["newOrder"]).Head.PickupBy = Request.Form["PickupBy"].ToString();
                    ((NewOrder)Session["newOrder"]).Head.DispatchDate = Request.Form["DispatchDate"].ToString();
                    ((NewOrder)Session["newOrder"]).Head.RequestForDelivery = (!string.IsNullOrEmpty(((NewOrder)Session["newOrder"]).Head.DispatchDate));
                    ((NewOrder)Session["newOrder"]).Head.Reference = Request.Form["Reference"].ToString();
                    ((NewOrder)Session["newOrder"]).Head.DriverLic = (Request.Form["DriverLic"] != null) ? Request.Form["DriverLic"].ToString() : "";
                    ((NewOrder)Session["newOrder"]).Head.ABN = (Request.Form["ABN"] != null) ? Request.Form["ABN"].ToString() : "";
                    ((NewOrder)Session["newOrder"]).Head.Address = (Request.Form["Address"] != null) ? Request.Form["Address"].ToString() : "";
                    ((NewOrder)Session["newOrder"]).Head.City = (Request.Form["City"] != null) ? Request.Form["City"].ToString() : "";
                    ((NewOrder)Session["newOrder"]).Head.State = (Request.Form["State"] != null) ? Request.Form["State"].ToString() : "";
                    ((NewOrder)Session["newOrder"]).Head.Postcode = (Request.Form["Postcode"] != null) ? Request.Form["Postcode"].ToString() : "";
                    ((NewOrder)Session["newOrder"]).Head.Country = (Request.Form["Country"] != null) ? Request.Form["Country"].ToString() : "";
                }
            }
            Session["CurrForm"] = "Home";
            return View();
        }


        public ActionResult PrintOrder()
        {
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult NewOrder()
        {
            Scanner.Models.User user = ((Scanner.Models.User)Session["User"]);
            IList<SideMenu> menu = ((List<SideMenu>)Session["SideMenu"]);

            Session.RemoveAll();
            Session["User"] = user;
            Session["SideMenu"] = menu;

            int numFrontCnt = Convert.ToInt32(WebConfigurationManager.AppSettings["numfrontC"]);
            var isFront = false;
            try
            {
                for (var i = 1; i < numFrontCnt + 1; i++)
                {
                    if (Environment.MachineName == WebConfigurationManager.AppSettings["FrontCounter" + i.ToString()].ToString())
                    {
                        isFront = true;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
            }

            Session["TradeType"] = "0";
            if (isFront)
            {
                Session["TradeType"] = "1";
            }
            // return View("Index");
            return Redirect($"{Url.RouteUrl(new { controller = "Order", action = "Index" })}");
        }

        [SessionExpire]
        [Authorize]
        public ActionResult SameColourItemOrdering()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;

            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }
                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SAME COLOUR ITEM ORDERING";
            //ViewBag.Title = "GRAMLINE";
            Session["CurrForm"] = "SameColourItemOrdering";
            loadNewTable("GramLine");
            loadNewTable("ColorLine");
            loadNewTable("RailsPosts");
            loadNewTable("Plinths");
            loadNewTable("ExtChannelPost");
            loadNewTable("SheetEdgeCover");
            loadNewTable("Lattice");
            loadNewTable("LatticeEdgeCover");
            loadNewTable("GramSlat");
            loadNewTable("PlasticCaps");
            loadNewTable("AlumPostBall");
            loadNewTable("PDLattice");
            loadNewTable("WeldedGateStile");
            loadNewTable("SmartStile");
            loadNewTable("SmartStileDP");
            loadNewTable("SquarePosts");
            loadNewTable("Screws");

            updateMenuClickCount("SameColourItemOrdering");
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult GramLine()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;

            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }
                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SPF® GramLine® SHEET";
            //ViewBag.Title = "GRAMLINE";
            Session["CurrForm"] = "GramLine";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        private void updateMenuClickCount(string formName)
        {
            string sql = "update [dbo].[TB_SIDEMENU_v1] set ClickCount = ClickCount+1 where [dbo].[fn_GetSplitedItem](lv1,'!',1)='" + formName + "' or [dbo].[fn_GetSplitedItem](lv2,'!',1)='" + formName + "' or [dbo].[fn_GetSplitedItem](lv3,'!',1)='" + formName + "'";
            using (var context = new DbContext(Global.ConnStr))
            {
                context.Database.ExecuteSqlCommand(sql);
            }
        }

        [SessionExpire]
        [Authorize]
        public ActionResult ColorLine()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SPF® ColorLine® SHEET";
            //ViewBag.Title = "COLOUR LINE";
            Session["CurrForm"] = "ColorLine";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult RailsPosts()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }
            ViewBag.Title = "SPF® RAIL / SPF® SLIMLINE CHANNEL POST / SPF® SmartPost®";

            //ViewBag.Title = "RAILS & POSTS";
            Session["CurrForm"] = "RailsPosts";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult SquarePosts()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SQUARE POSTS";
            Session["CurrForm"] = "SquarePosts";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult Lattice()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }
            ViewBag.Title = "GramLat® 3D Lattice™";

            //ViewBag.Title = "LATTICE";
            Session["CurrForm"] = "Lattice";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Plinths()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;

            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SPF® SteelPlinth";
            // ViewBag.Title = "PLINTHS";
            Session["CurrForm"] = "Plinths";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult SmartSlatAng()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }
            ViewBag.Title = "SMARTSLAT ANGLESTRIP & FLAT COVER STRIP";
            Session["CurrForm"] = "SmartSlatAng";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult SmartSlat()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }
            ViewBag.Title = "SMARTSLAT";
            Session["CurrForm"] = "SmartSlat";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult Screws()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SCREWS";
            Session["CurrForm"] = "Screws";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [SessionExpire]
        [Authorize]
        public ActionResult SheetEdgeCover()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SPF® SHEET EDGE COVER";
            // ViewBag.Title = "SHEET EDGE COVER ";
            Session["CurrForm"] = "SheetEdgeCover";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult SmartStile()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SMART STILE";
            Session["CurrForm"] = "SmartStile";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult SmartStileDP()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SMART STILE DOUBLE PUNCH";
            Session["CurrForm"] = "SmartStileDP";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult WeldedGateStile()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "WELDED GATE STILE";
            Session["CurrForm"] = "WeldedGateStile";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult ExtChannelPost()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "EXTENSION CHANNEL POST";
            Session["CurrForm"] = "ExtChannelPost";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult ExtSmartPost()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "EXTENSION SmartPost®";
            Session["CurrForm"] = "ExtSmartPost";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult PDLattice()
        {

            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;

            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "PUNCHED DIAMOND LATTICE";
            Session["CurrForm"] = "PDLattice";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult GramSlat()
        {

            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;

            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SPF GRAMSLATS SHADOWLINE";
            Session["CurrForm"] = "GramSlat";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [SessionExpire]
        [Authorize]
        public ActionResult SmartSlatLBSO()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }
            ViewBag.Title = "SMARTSLAT STRAIGHT/OFFSET LOUVRE BRACKET SET";

            Session["CurrForm"] = "SmartSlatLBSO";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [SessionExpire]
        [Authorize]
        public ActionResult WallBracket()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }
            ViewBag.Title = "RETAINING WALL BRACKET";

            Session["CurrForm"] = "WallBracket";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult SmartStileIncLDP300()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "D/PUNCHED SLIM SMTSTILE 300 LATTICE";
            Session["CurrForm"] = "SmartStileIncLDP300";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult SmartStileIncLDP450()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "D/PUNCHED SLIM SMTSTILE 450 LATTICE";
            Session["CurrForm"] = "SmartStileIncLDP450";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult SmartStileIncLDP600()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "D/PUNCHED SLIM SMTSTILE 600 LATTICE";
            Session["CurrForm"] = "SmartStileIncLDP600";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult SmartStileIncLSP300()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SLIM SMARTSTILE INCL 300 LATTICE";
            Session["CurrForm"] = "SmartStileIncLSP300";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult SmartStileIncLSP450()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SLIM SMARTSTILE INCL 450 LATTICE";
            Session["CurrForm"] = "SmartStileIncLSP450";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult SmartStileIncLSP600()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SLIM SMARTSTILE INCL 600 LATTICE";
            Session["CurrForm"] = "SmartStileIncLSP600";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult WeldedGateStileL300()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "WELDED STILE INCL 300 LATTICE";
            Session["CurrForm"] = "WeldedGateStileL300";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult WeldedGateStileL450()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "WELDED STILE INCL 450 LATTICE";
            Session["CurrForm"] = "WeldedGateStileL450";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult WeldedGateStileL600()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "WELDED STILE INCL 600 LATTICE";
            Session["CurrForm"] = "WeldedGateStileL600";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult SmartSlatSpacer()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SMARTSLAT INSTALLATION GAP SPACER 600MM";
            Session["CurrForm"] = "SmartSlatSpacer";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [SessionExpire]
        [Authorize]
        public ActionResult SmartSlatITRF()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SMARTSLAT INSTALLATION TOOL ROUND & FLAT ";
            Session["CurrForm"] = "SmartSlatITRF";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult PlasticCaps()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "PLASTIC CAP";
            Session["CurrForm"] = "PlasticCaps";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [SessionExpire]
        [Authorize]
        public ActionResult AlumPostBall()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "ALUMINIUM POST BALL";
            Session["CurrForm"] = "AlumPostBall";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult StainlessSteelFittings()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "STAINLESS STEEL FITTINGS";
            Session["CurrForm"] = "StainlessSteelFittings";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult ZincFittings()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "ZINC FITTINGS";
            Session["CurrForm"] = "ZincFittings";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult CapsPlastic()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "PLASTIC CAPS";
            Session["CurrForm"] = "CapsPlastic";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult CapsMetal()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "METAL CAPS";
            Session["CurrForm"] = "CapsMetal";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Cement()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "CEMENT";
            Session["CurrForm"] = "Cement";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult Fasteners()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }


            using (var context = new DbContext(Global.ConnStr))
            {
                object[] parameters = { "GramLine" };
                ViewBag.colours = context.Database.SqlQuery<string>("proc_GetColours {0}", parameters).ToList<string>();
            }

            ViewBag.Title = "FASTENERS";
            Session["CurrForm"] = "Fasteners";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult Inserts()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }


            ViewBag.Title = "INSERTS";
            Session["CurrForm"] = "Inserts";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [SessionExpire]
        [Authorize]
        public ActionResult Downee()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "DOWNEE";
            Session["CurrForm"] = "Downee";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [Authorize]
        public ActionResult SprayPaint()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SPARY PAINT";
            Session["CurrForm"] = "SprayPaint";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [SessionExpire]
        [Authorize]
        public ActionResult Roofing()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "ROOFING";
            Session["CurrForm"] = "Roofing";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult Hinges()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "HINGES";
            Session["CurrForm"] = "Hinges";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Locks()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "LOCKS";
            Session["CurrForm"] = "Locks";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Paint()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "PAINT";
            Session["CurrForm"] = "Paint";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult Tools()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "TOOLS";
            Session["CurrForm"] = "Tools";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult SlidingGate()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SLIDING GATE";
            Session["CurrForm"] = "SlidingGate";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult LatticeEdgeCover()
        {
            if (Session["UseSessionMenuExpandings"] == null)
            {
                Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            }
            Session["UseSessionMenuExpandings"] = null;
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SPF®GramLat® 3D Lattice™ EDGE COVER";
            Session["CurrForm"] = "LatticeEdgeCover";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [SessionExpire]
        [Authorize]
        public ActionResult PoolFencing()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "ALUMINIUM FLAT TOP POOL FENCING";
            Session["CurrForm"] = "PoolFencing";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [SessionExpire]
        [Authorize]
        public ActionResult SecFencing()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "PREGAL SECURITY FENCING";
            Session["CurrForm"] = "SecFencing";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }



        [SessionExpire]
        [Authorize]
        public ActionResult SpecialOrder()
        {
            if (Session["uploadGuid"] == null)
            {
                Session["uploadGuid"] = Guid.NewGuid().ToString();
            }

            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }

            ViewBag.Title = "SPECIAL ORDER";
            Session["CurrForm"] = "SpecialOrder";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }




        [SessionExpire]
        [Authorize]
        public ActionResult Miscellaneous()
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "f"))
            {
                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
            }
            Session["CurrForm"] = "Miscellaneous";
            loadNewTable(Session["CurrForm"].ToString());
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }


        [SessionExpire]
        [Authorize]
        public ActionResult QuotationTrack()
        {
            OrderHeads ohead = new OrderHeads();
            return QuotationTrack(ohead);
        }


        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult QuotationTrack(OrderHeads orderHs)
        {
            ViewBag.Title = "Quotation Track";
            Session["CurrForm"] = "QuotationTrack";

            if (string.IsNullOrEmpty(orderHs.sortCol))
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

                orderHs.sortCol = "DefaultSort";
                orderHs.sortColType = "String";
                orderHs.rowsPerPage = 15;
                orderHs.pageNum = 1;
                orderHs.orderBy = "glyphicon glyphicon-arrow-up";
            }

            if (String.IsNullOrEmpty(orderHs.whereStr))
            {
                orderHs.whereStr = "";
            }

            if (orderHs.whereStr.Replace(" ", "") == "")
            {
                orderHs.whereStr = "";
            }


            var Role = new SqlParameter("@Role", ((Scanner.Models.User)Session["User"]).Role);
            var UserName = new SqlParameter("@UserName", ((Scanner.Models.User)Session["User"]).UserName);
            var pageNum = new SqlParameter("@pageNum", (orderHs.pageNum == 0) ? 1 : orderHs.pageNum);
            var rowsPerPage = new SqlParameter("@rowsPerPage", orderHs.rowsPerPage);
            var sortCol = new SqlParameter("@sortCol", orderHs.sortCol);
            var sortColType = new SqlParameter("@sortColType", orderHs.sortColType);
            var whereStr = new SqlParameter("@whereStr", orderHs.whereStr.ToString());
            var flag = new SqlParameter("@flag", 2);

            var orderBy = (orderHs.orderBy == "glyphicon glyphicon-arrow-down") ?
                new SqlParameter("@orderBy", "desc") :
                new SqlParameter("@orderBy", "asc");


            var table = new SqlParameter("@table", "dbo.View_Orders");
            var selStr = new SqlParameter("@selStr", "");

            var sql = "exec dbo.proc_GetOrders_v1 " +
                "@Role," +
                "@UserName, " +
                "@pageNum, " +
                "@rowsPerPage, " +
                "@sortCol, " +
                "@sortColType, " +
                "@whereStr, " +
                "@orderBy, " +
                "@table, " +
                "@selStr," +
                "@flag";

            var oldMsg = "";

            if (orderHs.errMsg == null)
                orderHs.errMsg = "";
            else
                oldMsg = orderHs.errMsg;

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    orderHs.orderHeads = context.Database.SqlQuery<OrderHead2>(sql,
                       Role,
                       UserName,
                       pageNum,
                       rowsPerPage,
                       sortCol,
                       sortColType,
                       whereStr,
                       orderBy,
                       table,
                       selStr,
                       flag).ToList<OrderHead2>();
                }
            }
            catch (Exception e)
            {
                orderHs.totalPages = 0;
                orderHs.totalRows = 0;
                orderHs.orderHeads = null;
                orderHs.errMsg = "No Record Found";
            }

            if (orderHs.orderHeads != null)
            {
                if (orderHs.orderHeads.Count > 0)
                {
                    orderHs.totalPages = orderHs.orderHeads[0].maxPages;
                    orderHs.totalRows = orderHs.orderHeads[0].TotalRows;
                }
            }

            if (oldMsg != "")
                orderHs.errMsg = oldMsg;

            return View(orderHs);
        }


        [SessionExpire]
        [Authorize]
        public ActionResult OrderTrack()
        {
            OrderHeads ohead = new OrderHeads();
            return OrderTrack(ohead);
        }


        [SessionExpire]
        [Authorize]
        [HttpPost]
        public ActionResult OrderTrack(OrderHeads orderHs)
        {
            ViewBag.Title = "Order Track";
            Session["CurrForm"] = "OrderTrack";

            if (string.IsNullOrEmpty(orderHs.sortCol))
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

                orderHs.sortCol = "DefaultSort";
                orderHs.sortColType = "String";
                orderHs.rowsPerPage = 15;
                orderHs.pageNum = 1;
                orderHs.orderBy = "glyphicon glyphicon-arrow-up";
            }

            if (String.IsNullOrEmpty(orderHs.whereStr))
            {
                orderHs.whereStr = "";
            }

            if (orderHs.whereStr.Replace(" ", "") == "")
            {
                orderHs.whereStr = "";
            }


            var Role = new SqlParameter("@Role", ((Scanner.Models.User)Session["User"]).Role);
            var UserName = new SqlParameter("@UserName", ((Scanner.Models.User)Session["User"]).UserName);
            var pageNum = new SqlParameter("@pageNum", (orderHs.pageNum == 0) ? 1 : orderHs.pageNum);
            var rowsPerPage = new SqlParameter("@rowsPerPage", orderHs.rowsPerPage);
            var sortCol = new SqlParameter("@sortCol", orderHs.sortCol);
            var sortColType = new SqlParameter("@sortColType", orderHs.sortColType);
            var whereStr = new SqlParameter("@whereStr", orderHs.whereStr.ToString());
            var flag = new SqlParameter("@flag", 1);

            var orderBy = (orderHs.orderBy == "glyphicon glyphicon-arrow-down") ?
                new SqlParameter("@orderBy", "desc") :
                new SqlParameter("@orderBy", "asc");


            var table = new SqlParameter("@table", "dbo.View_Orders");
            var selStr = new SqlParameter("@selStr", "");

            var sql = "exec dbo.proc_GetOrders_v1 " +
                "@Role," +
                "@UserName, " +
                "@pageNum, " +
                "@rowsPerPage, " +
                "@sortCol, " +
                "@sortColType, " +
                "@whereStr, " +
                "@orderBy, " +
                "@table, " +
                "@selStr," +
                "@flag";

            var oldMsg = "";

            if (orderHs.errMsg == null)
                orderHs.errMsg = "";
            else
                oldMsg = orderHs.errMsg;

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    orderHs.orderHeads = context.Database.SqlQuery<OrderHead2>(sql,
                       Role,
                       UserName,
                       pageNum,
                       rowsPerPage,
                       sortCol,
                       sortColType,
                       whereStr,
                       orderBy,
                       table,
                       selStr,
                       flag).ToList<OrderHead2>();
                }
            }
            catch (Exception e)
            {
                orderHs.totalPages = 0;
                orderHs.totalRows = 0;
                orderHs.orderHeads = null;
                orderHs.errMsg = "No Record Found";
            }

            if (orderHs.orderHeads != null)
            {
                if (orderHs.orderHeads.Count > 0)
                {
                    orderHs.totalPages = orderHs.orderHeads[0].maxPages;
                    orderHs.totalRows = orderHs.orderHeads[0].TotalRows;
                }
            }

            if (oldMsg != "")
                orderHs.errMsg = oldMsg;

            return View(orderHs);
        }


        public ActionResult OrderListBoard(OrderHeads orderHs)
        {
            ViewBag.Title = "Order List Board";
            Session["CurrForm"] = "OrderListBoard";

            if (string.IsNullOrEmpty(orderHs.sortCol))
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

                orderHs.sortCol = "DefaultSort";
                orderHs.sortColType = "String";
                orderHs.rowsPerPage = 15;
                orderHs.pageNum = 1;
                orderHs.orderBy = "glyphicon glyphicon-arrow-up";
            }

            if (String.IsNullOrEmpty(orderHs.whereStr))
            {
                orderHs.whereStr = "";
            }

            if (orderHs.whereStr.Replace(" ", "") == "")
            {
                orderHs.whereStr = "";
            }


            var Role = new SqlParameter("@Role", ((Scanner.Models.User)Session["User"]).Role);
            var UserName = new SqlParameter("@UserName", ((Scanner.Models.User)Session["User"]).UserName);
            var pageNum = new SqlParameter("@pageNum", (orderHs.pageNum == 0) ? 1 : orderHs.pageNum);
            var rowsPerPage = new SqlParameter("@rowsPerPage", orderHs.rowsPerPage);
            var sortCol = new SqlParameter("@sortCol", orderHs.sortCol);
            var sortColType = new SqlParameter("@sortColType", orderHs.sortColType);
            var whereStr = new SqlParameter("@whereStr", orderHs.whereStr.ToString());
            var flag = new SqlParameter("@flag", 3);

            var orderBy = (orderHs.orderBy == "glyphicon glyphicon-arrow-down") ?
                new SqlParameter("@orderBy", "desc") :
                new SqlParameter("@orderBy", "asc");


            var table = new SqlParameter("@table", "dbo.View_Orders");
            var selStr = new SqlParameter("@selStr", "");

            var sql = "exec dbo.proc_GetOrders_v1 " +
                "@Role," +
                "@UserName, " +
                "@pageNum, " +
                "@rowsPerPage, " +
                "@sortCol, " +
                "@sortColType, " +
                "@whereStr, " +
                "@orderBy, " +
                "@table, " +
                "@selStr," +
                "@flag";

            var oldMsg = "";

            if (orderHs.errMsg == null)
                orderHs.errMsg = "";
            else
                oldMsg = orderHs.errMsg;

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    orderHs.orderHeads = context.Database.SqlQuery<OrderHead2>(sql,
                       Role,
                       UserName,
                       pageNum,
                       rowsPerPage,
                       sortCol,
                       sortColType,
                       whereStr,
                       orderBy,
                       table,
                       selStr,
                       flag).ToList<OrderHead2>();
                }
            }
            catch (Exception e)
            {
                orderHs.totalPages = 0;
                orderHs.totalRows = 0;
                orderHs.orderHeads = null;
                orderHs.errMsg = "No Record Found";
            }

            if (orderHs.orderHeads != null)
            {
                if (orderHs.orderHeads.Count > 0)
                {
                    orderHs.totalPages = orderHs.orderHeads[0].maxPages;
                    orderHs.totalRows = orderHs.orderHeads[0].TotalRows;
                }
            }

            if (oldMsg != "")
                orderHs.errMsg = oldMsg;

            return View(orderHs);
        }



        [SessionExpire]
        [Authorize]
        public ActionResult CancelOrder(int id)
        {
            var errMsg = "";
            using (var context = new DbContext(Global.ConnStr))
            {
                object[] parameters = { id,
                    ((Scanner.Models.User)Session["User"]).UserName
                };
                try
                {
                    errMsg = context.Database.SqlQuery<string>("proc_CancelOrder {0},{1}", parameters).ToList<string>()[0];

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
                                        "Error: Order "+ id.ToString()+ " can not cancel",
                                        errBody,
                                        "",
                                        "",
                                        ""
                                    };

                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters_);

                }
            }

            var outputMessage = "";
            if (WebConfigurationManager.AppSettings["pubDir"] == "")
            {
                outputMessage = "<html><body><form action = '/Order/OrderTrack' id='frmMain' name='frmMain' method = 'post'><input type='hidden' name='errMsg' value='" + errMsg + "' /></form><script type='text/javascript'>document.frmMain.submit();</script></body></html >";
            }
            else
            {
                outputMessage = "<html><body><form action = '/" + WebConfigurationManager.AppSettings["pubDir"] + "/Order/OrderTrack' id='frmMain' name='frmMain' method = 'post'><input type='hidden' name='errMsg' value='" + errMsg + "' /></form><script type='text/javascript'>document.frmMain.submit();</script></body></html >";
            }
            return Content(outputMessage);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult ReloadOrder(int id)
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            Session["ReloadId"] = id;
            Session["UploadingNote"] = "";

            NewOrder rOrder = new NewOrder();
            using (var context = new DbContext(Global.ConnStr))
            {
                object[] parameters = { id };
                try
                {
                    rOrder.Head = context.Database.SqlQuery<OrderHead>("proc_GetOrderHead2_v1 {0}", parameters).ToList<OrderHead>()[0];

                    if (!string.IsNullOrEmpty(rOrder.Head.ServerSEQNO))
                    {
                        Session["ReloadId"] = null;
                    }

                    rOrder.OrderDetails = context.Database.SqlQuery<NewOrderDetail>("proc_GetOrderDetails {0}", parameters).ToList<NewOrderDetail>();

                    int idx = 0;
                    foreach (NewOrderDetail od in rOrder.OrderDetails)
                    {
                        if ((od.ITEMTYPE == "SpecialOrder") && (od.STOCKCODE == "MISC") && ((rOrder.Head.Status.IndexOf("Saved") > -1) || (rOrder.Head.Status.IndexOf("Ready To Proceed") > -1)))
                        {
                            Session["uploadedFiles"] = od.DESCRIPTION.Replace("MISC ITEM (", "").Replace(")", "");

                            if (Session["uploadGuid"] != null)
                            {
                                if (Directory.Exists(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\"))))
                                {
                                    Directory.Delete(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")), true);
                                }
                                Session["uploadGuid"] = null;
                            }

                            if (Directory.Exists(string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + rOrder.Head.Id.ToString(), Server.MapPath(@"\"))))
                            {
                                Session["uploadGuid"] = Guid.NewGuid().ToString();
                                Directory.CreateDirectory(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")));

                                Directory.GetFiles(string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + rOrder.Head.Id.ToString(), Server.MapPath(@"\")))
                                .ToList()
                                .ForEach(f => System.IO.File.Copy(f, string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")) + f.Substring(f.LastIndexOf("\\"))));

                                //Directory.Move(string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + rOrder.Head.Id.ToString(), Server.MapPath(@"\")), string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")));
                            }
                        }

                        //if ((od.ITEMTYPE == "SpecialOrder") && (od.STOCKCODE == "MISC") && (rOrder.Head.Status.IndexOf("Saved") < 0)) {
                        //    if (Directory.Exists(string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + rOrder.Head.Id.ToString(), Server.MapPath(@"\"))))
                        //    {
                        //        Session["uploadedFiles"] = od.DESCRIPTION.Replace("MISC ITEM (", "").Replace(")", "");
                        //        Session["uploadGuid"] = "GRAM_INTERNAL\\Uploaded\\"+ rOrder.Head.Id.ToString()+"\\";                                
                        //    }
                        //}

                        od.Id = idx;
                        od.Idx = idx;
                        idx++;
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
                                        "Error: Order "+ id.ToString()+ " can not be loaded",
                                        errBody,
                                        "",
                                        "",
                                        ""
                                    };

                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters_);
                    rOrder.Head = new OrderHead();
                    rOrder.Head.Message = "";

                    Session["ErrorMsg"] = "Loading Order Error has been has been occured.<br/> And we will get this issue fixed as soon as possible.<br/> Sorry for your inconvenience.";
                    return Redirect($"{Url.RouteUrl(new { controller = "Order", action = "OrderTrack" })}");
                    //  throw new Exception("Can not reload order", e);
                }
            }


            var role = ((Scanner.Models.User)Session["User"]).Role;
            var trial = ((Scanner.Models.User)Session["User"]).isTrial;

            var letPop = false;

            if (!string.IsNullOrEmpty(rOrder.Head.ServerSEQNO))
            {
                letPop = false;
            }
            else
            {
                if (rOrder.Head.Status == "2. Saved")
                {
                    letPop = true;
                }
                else
                {
                    if ((role.IndexOf("Gram") > -1) || (!trial))
                    {
                        letPop = true;
                    }
                }
            }

            if (letPop)
            {
                var forms = getforms();
                foreach (var form in forms)
                {
                    Session.Remove(form);
                    Session.Remove(form + "AllCuts");
                }

                rOrder.Head.TotalPrice = 0;
                rOrder.Head.TotalWeight = 0;

                NewOrderDetail preOrderDetail = new NewOrderDetail();
                preOrderDetail.STOCKCODE = "";
                int TotalQty = 0;
                bool ini = true;
                int cnt = 0;
                var addedSmartSlat = "";
                var addedSmartSlatAng = "";
                var addedSmartSlatLBSO = "";
                bool found;

                foreach (var orderDetail in rOrder.OrderDetails)
                {
                    cnt++;
                    orderDetail.needAdvice = false;
                    if (orderDetail.DESCRIPTION.IndexOf("needs adv") > -1)
                    {
                        orderDetail.needAdvice = true;
                    }

                    if (orderDetail.Cuts > 0)
                    {
                        if (Session[orderDetail.ITEMTYPE + "AllCuts"] == null)
                            Session[orderDetail.ITEMTYPE + "AllCuts"] = "";

                        Session[orderDetail.ITEMTYPE + "AllCuts"] += orderDetail.COLOUR + "|~|" + orderDetail.HEADING + "|~|" + orderDetail.Cuts + "|~|" + orderDetail.CutDesc + "|_|";
                    }

                    if (orderDetail.Cuts == -1)
                    {
                        Session["UploadingNote"] = orderDetail.CutDesc;
                    }

                    //orderDetail.PRICE = "";
                    //orderDetail.WEIGHT = "";
                    if (Session[orderDetail.ITEMTYPE] == null)
                    {
                        loadNewTable(orderDetail.ITEMTYPE);
                    }

                    int C_x = orderDetail.C_X ?? 0;
                    int C_y = orderDetail.C_Y ?? 0;

                    if (orderDetail.nextRelColorId != null)
                    {
                        if (Convert.ToInt32(orderDetail.nextRelColorId) > -1)
                        {
                            ((DataTable)Session[orderDetail.ITEMTYPE]).Rows[C_y][C_x + 1] = orderDetail.nextRelColorId;
                        }
                    }

                    if (ini)
                    {
                        preOrderDetail = orderDetail;
                        TotalQty = Convert.ToInt32(orderDetail.QTY);
                        ini = false;
                    }
                    else
                    {
                        bool populate = true;

                        if (orderDetail.ITEMTYPE != "Screws")
                        {
                            if (preOrderDetail.STOCKCODE == orderDetail.STOCKCODE)
                            {
                                if ((orderDetail.ITEMTYPE != "SmartSlat") &&
                                    (orderDetail.ITEMTYPE != "SmartSlatAng") &&
                                    (orderDetail.ITEMTYPE != "SmartSlatLBSO"))
                                {
                                    TotalQty += Convert.ToInt32(orderDetail.QTY);
                                    populate = false;
                                }
                                else
                                {
                                    if (preOrderDetail.LENGTH == orderDetail.LENGTH)
                                    {
                                        populate = false;
                                        TotalQty += Convert.ToInt32(orderDetail.QTY);
                                    }
                                }
                            }
                        }

                        if (populate)
                        {
                            if ((orderDetail.ITEMTYPE == "SmartSlat") &&
                            (addedSmartSlat.IndexOf(";" + orderDetail.COLOUR + orderDetail.HEADING) > -1))
                            {
                                for (int i = 0; i < ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.Count; i++)
                                {
                                    if (orderDetail.COLOUR == ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0].ToString())
                                    {
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0] = "!#" + ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0];
                                        DataRow nRow = ((DataTable)Session[preOrderDetail.ITEMTYPE]).NewRow();
                                        nRow[0] = orderDetail.COLOUR;
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.InsertAt(nRow, i + 1);
                                        break;
                                    }
                                }
                                addedSmartSlat = "";
                            }

                            if ((orderDetail.ITEMTYPE == "SmartSlatAng") &&
                            (addedSmartSlatAng.IndexOf(";" + orderDetail.COLOUR + orderDetail.HEADING) > -1))
                            {
                                for (int i = 0; i < ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.Count; i++)
                                {
                                    if (orderDetail.COLOUR == ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0].ToString())
                                    {
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0] = "!#" + ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0];
                                        DataRow nRow = ((DataTable)Session[preOrderDetail.ITEMTYPE]).NewRow();
                                        nRow[0] = orderDetail.COLOUR;
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.InsertAt(nRow, i + 1);
                                        break;
                                    }
                                }
                                addedSmartSlatAng = "";
                            }


                            if ((orderDetail.ITEMTYPE == "SmartSlatLBSO") &&
                          (addedSmartSlatLBSO.IndexOf(";" + orderDetail.COLOUR + orderDetail.HEADING) > -1))
                            {
                                for (int i = 0; i < ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.Count; i++)
                                {
                                    if (orderDetail.COLOUR == ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0].ToString())
                                    {
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0] = "!#" + ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0];
                                        DataRow nRow = ((DataTable)Session[preOrderDetail.ITEMTYPE]).NewRow();
                                        nRow[0] = orderDetail.COLOUR;
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.InsertAt(nRow, i + 1);
                                        break;
                                    }
                                }
                                addedSmartSlatLBSO = "";
                            }


                            found = false;
                            string tmpTxt = "";
                            for (int i = 0; i < ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.Count; i++)
                            {
                                tmpTxt = ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0].ToString();

                                if ((preOrderDetail.ITEMTYPE == "SpecialOrder") && (tmpTxt.IndexOf("MISC") > -1))
                                {
                                    tmpTxt = "MISC!^!MISC ITEM (" + Session["uploadedFiles"].ToString() + ")";
                                }

                                if (preOrderDetail.COLOUR == tmpTxt)
                                {
                                    found = true;
                                    var colFound = false;
                                    for (int j = 1; j < ((DataTable)Session[preOrderDetail.ITEMTYPE]).Columns.Count; j++)
                                    {
                                        if (((DataTable)Session[preOrderDetail.ITEMTYPE]).Columns[j].ColumnName == preOrderDetail.HEADING)
                                        {
                                            colFound = true;
                                            if (preOrderDetail.ITEMTYPE == "Roofing")
                                                ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][1] = preOrderDetail.LENGTH;


                                            if (preOrderDetail.ITEMTYPE == "Screws")
                                            {

                                                if ((preOrderDetail.LENGTH == "121420") && (TotalQty > 499))
                                                {
                                                    TotalQty = TotalQty / 500;
                                                }
                                                else
                                                {
                                                    if (TotalQty > 999)
                                                    {
                                                        TotalQty = TotalQty / 1000;
                                                    }
                                                }
                                            }


                                            ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][j] = TotalQty.ToString();


                                            if (preOrderDetail.ITEMTYPE == "SmartSlat")
                                            {
                                                addedSmartSlat += ";" + orderDetail.COLOUR + preOrderDetail.HEADING;
                                                if (preOrderDetail.HEADING.IndexOf("ec") < 0)
                                                {
                                                    ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][j + 1] = preOrderDetail.STANDARD.Split(' ')[1].ToString();
                                                    j++;
                                                }
                                            }

                                            if (preOrderDetail.ITEMTYPE == "SmartSlatAng")
                                            {
                                                addedSmartSlatAng += ";" + orderDetail.COLOUR + preOrderDetail.HEADING;
                                                preOrderDetail.LENGTH = preOrderDetail.STANDARD;
                                                ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][j + 1] = preOrderDetail.LENGTH;
                                                preOrderDetail.STANDARD = preOrderDetail.LENGTH + "|" + ((DataTable)Session["SmartSlatAng"]).Columns[j].ColumnName.Replace(Environment.NewLine + "QTY", "").Replace("ANGLE ", "").Replace("FLAT ", "");
                                                j++;
                                            }


                                            if (preOrderDetail.ITEMTYPE == "SmartSlatLBSO")
                                            {
                                                addedSmartSlatLBSO += ";" + orderDetail.COLOUR + preOrderDetail.HEADING;
                                                preOrderDetail.LENGTH = preOrderDetail.STANDARD;
                                                ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][j + 1] = preOrderDetail.LENGTH;
                                                preOrderDetail.STANDARD = preOrderDetail.LENGTH + "|" + ((DataTable)Session["SmartSlatLBSO"]).Columns[j].ColumnName.Replace(Environment.NewLine + "QTY", "").Replace("ANGLE ", "").Replace("FLAT ", "");
                                                j++;
                                            }


                                            if (preOrderDetail.ITEMTYPE == "Fasteners")
                                            {
                                                ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][j - 1] = (string.IsNullOrEmpty(preOrderDetail.SUNDRYCOLOUR)) ? "" : preOrderDetail.SUNDRYCOLOUR;
                                                j++;
                                            }

                                            break;
                                        }
                                    }

                                    if (!colFound)
                                    {
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Columns.Add(preOrderDetail.HEADING);
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE + "Flags"]).Columns.Add(preOrderDetail.HEADING);
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE + "Flags"]).Rows[0][preOrderDetail.HEADING] = "FALSE";
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE + "Flags"]).Rows[1][preOrderDetail.HEADING] = "Made to Order. Lead Time 10-15 Working Days.";
                                        found = false;
                                        i--;
                                    }
                                }

                                if (found)
                                    break;
                            }

                            preOrderDetail = orderDetail;
                            TotalQty = Convert.ToInt32(orderDetail.QTY);
                        }
                    }

                }

                //the last one
                if ((preOrderDetail.ITEMTYPE == "SmartSlat") &&
                           (addedSmartSlat.IndexOf(";" + preOrderDetail.COLOUR + preOrderDetail.HEADING) > -1))
                {
                    for (int i = 0; i < ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.Count; i++)
                    {
                        if (preOrderDetail.COLOUR == ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0].ToString())
                        {
                            ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0] = "!#" + ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0];
                            DataRow nRow = ((DataTable)Session[preOrderDetail.ITEMTYPE]).NewRow();
                            nRow[0] = preOrderDetail.COLOUR;
                            ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.InsertAt(nRow, i + 1);
                            break;
                        }
                    }
                    addedSmartSlat = "";
                }


                if ((preOrderDetail.ITEMTYPE == "SmartSlatAng") &&
                          (addedSmartSlatAng.IndexOf(";" + preOrderDetail.COLOUR + preOrderDetail.HEADING) > -1))
                {
                    for (int i = 0; i < ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.Count; i++)
                    {
                        if (preOrderDetail.COLOUR == ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0].ToString())
                        {
                            ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0] = "!#" + ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0];
                            DataRow nRow = ((DataTable)Session[preOrderDetail.ITEMTYPE]).NewRow();
                            nRow[0] = preOrderDetail.COLOUR;
                            ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.InsertAt(nRow, i + 1);
                            break;
                        }
                    }
                    addedSmartSlatAng = "";
                }


                if ((preOrderDetail.ITEMTYPE == "SmartSlatLBSO") &&
                     (addedSmartSlatLBSO.IndexOf(";" + preOrderDetail.COLOUR + preOrderDetail.HEADING) > -1))
                {
                    for (int i = 0; i < ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.Count; i++)
                    {
                        if (preOrderDetail.COLOUR == ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0].ToString())
                        {
                            ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0] = "!#" + ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0];
                            DataRow nRow = ((DataTable)Session[preOrderDetail.ITEMTYPE]).NewRow();
                            nRow[0] = preOrderDetail.COLOUR;
                            ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.InsertAt(nRow, i + 1);
                            break;
                        }
                    }
                    addedSmartSlatLBSO = "";
                }


                found = false;
                if (!string.IsNullOrEmpty(preOrderDetail.ITEMTYPE))
                {

                    found = false;
                    string tmpTxt = "";
                    for (int i = 0; i < ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows.Count; i++)
                    {
                        tmpTxt = ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][0].ToString();

                        if ((preOrderDetail.ITEMTYPE == "SpecialOrder") && (tmpTxt.IndexOf("MISC") > -1))
                        {
                            tmpTxt = "MISC!^!MISC ITEM (" + Session["uploadedFiles"].ToString() + ")";
                        }


                        if (preOrderDetail.COLOUR == tmpTxt)
                        {
                            found = true;
                            var colFound = false;
                            for (int j = 1; j < ((DataTable)Session[preOrderDetail.ITEMTYPE]).Columns.Count; j++)
                            {
                                if (((DataTable)Session[preOrderDetail.ITEMTYPE]).Columns[j].ColumnName == preOrderDetail.HEADING)
                                {
                                    colFound = true;
                                    if (preOrderDetail.ITEMTYPE == "Roofing")
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][1] = preOrderDetail.LENGTH;


                                    if (preOrderDetail.ITEMTYPE == "Screws")
                                    {
                                        if ((preOrderDetail.LENGTH == "121420") && (TotalQty > 499))
                                        {
                                            TotalQty = TotalQty / 500;
                                        }
                                        else
                                        {
                                            if (TotalQty > 999)
                                            {
                                                TotalQty = TotalQty / 1000;
                                            }
                                        }
                                    }

                                    ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][j] = TotalQty.ToString();

                                    if (preOrderDetail.ITEMTYPE == "SmartSlat")
                                    {
                                        if (preOrderDetail.HEADING.IndexOf("ec") < 0)
                                        {
                                            ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][j + 1] = preOrderDetail.STANDARD.Split(' ')[1].ToString();
                                            j++;
                                        }
                                    }

                                    if (preOrderDetail.ITEMTYPE == "SmartSlatAng")
                                    {
                                        preOrderDetail.LENGTH = preOrderDetail.STANDARD;
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][j + 1] = preOrderDetail.LENGTH;
                                        preOrderDetail.STANDARD = preOrderDetail.LENGTH + "|" + ((DataTable)Session["SmartSlatAng"]).Columns[j].ColumnName.Replace(Environment.NewLine + "QTY", "").Replace("ANGLE ", "").Replace("FLAT ", "");
                                        j++;
                                    }

                                    if (preOrderDetail.ITEMTYPE == "SmartSlatLBSO")
                                    {
                                        preOrderDetail.LENGTH = preOrderDetail.STANDARD;
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][j + 1] = preOrderDetail.LENGTH;
                                        preOrderDetail.STANDARD = preOrderDetail.LENGTH + "|" + ((DataTable)Session["SmartSlatLBSO"]).Columns[j].ColumnName.Replace(Environment.NewLine + "QTY", "").Replace("ANGLE ", "").Replace("FLAT ", "");
                                        j++;
                                    }


                                    if (preOrderDetail.ITEMTYPE == "Fasteners")
                                    {
                                        ((DataTable)Session[preOrderDetail.ITEMTYPE]).Rows[i][j - 1] = (string.IsNullOrEmpty(preOrderDetail.SUNDRYCOLOUR)) ? "" : preOrderDetail.SUNDRYCOLOUR;
                                        j++;
                                    }

                                    break;
                                }
                            }

                            if (!colFound)
                            {
                                ((DataTable)Session[preOrderDetail.ITEMTYPE]).Columns.Add(preOrderDetail.HEADING);
                                ((DataTable)Session[preOrderDetail.ITEMTYPE + "Flags"]).Columns.Add(preOrderDetail.HEADING);
                                ((DataTable)Session[preOrderDetail.ITEMTYPE + "Flags"]).Rows[0][preOrderDetail.HEADING] = "FALSE";
                                ((DataTable)Session[preOrderDetail.ITEMTYPE + "Flags"]).Rows[1][preOrderDetail.HEADING] = "Made to Order. Lead Time 10-15 Working Days.";
                                found = false;
                                i--;
                            }
                        }

                        if (found)
                            break;
                    }
                }

                if (Session["SmartSlat"] != null)
                {
                    for (int i = 0; i < ((DataTable)Session["SmartSlat"]).Rows.Count; i++)
                    {
                        ((DataTable)Session["SmartSlat"]).Rows[i][0] = ((DataTable)Session["SmartSlat"]).Rows[i][0].ToString().Replace("!#", "");
                    }
                }

                if (Session["SmartSlatAng"] != null)
                {
                    for (int i = 0; i < ((DataTable)Session["SmartSlatAng"]).Rows.Count; i++)
                    {
                        ((DataTable)Session["SmartSlatAng"]).Rows[i][0] = ((DataTable)Session["SmartSlatAng"]).Rows[i][0].ToString().Replace("!#", "");
                    }
                }

                if (Session["SmartSlatLBSO"] != null)
                {
                    for (int i = 0; i < ((DataTable)Session["SmartSlatLBSO"]).Rows.Count; i++)
                    {
                        ((DataTable)Session["SmartSlatLBSO"]).Rows[i][0] = ((DataTable)Session["SmartSlatLBSO"]).Rows[i][0].ToString().Replace("!#", "");
                    }
                }


                Session["JustViewPrint"] = null;
            }
            else
            {
                Session["JustViewPrint"] = 1;
            }

            Session["newOrder"] = rOrder;
            Session["CurrForm"] = "CheckOut";
            ViewBag.Title = "CHECK OUT";

            if (rOrder.Head.CompanyList == null)
            {
                var sql = "exec dbo.proc_GetCompanyNameList";
                using (var context = new DbContext(Global.ConnStr))
                {
                    rOrder.Head.CompanyList = context.Database.SqlQuery<string>(sql).ToList<string>();
                }
            }

            rOrder.Head.OrderBy = rOrder.Head.ContactName;
            return View("CheckOut", rOrder.Head);
        }

        [SessionExpire]
        public ActionResult SmartSlatQuotation()
        {
            updateMenuClickCount("SmartSlatQuotation");
            return View();
        }

        [SessionExpire]
        public ActionResult EnterMeterage()
        {
            ViewBag.Title = "Enter Meterage";
            Session["CurrForm"] = "EnterMeterage";
            IList<string> colours = new List<string>();
            IList<string> heights = new List<string>();
            IList<string> chHeights = new List<string>();
            IList<string> smHeights = new List<string>();

            var sql = "select Colour from [dbo].[TB_COLOUR] where ItemTypeId = 1 order by pos";
            var sql2 = "select Standard + case when IsNorm = 0 then ' *' else '' end as Standard from [dbo].[TB_STANDARD] where ItemTypeId = 1 order by pos";
            var sql3 = "select ActStandard + case when IsNorm = 0 then ' *' else '' end as ActStandard from [dbo].[TB_STANDARD] where ItemTypeId = 10 and Standard like '%CHANNEL POST%' order by pos";
            var sql4 = "select ActStandard + case when IsNorm = 0 then ' *' else '' end as ActStandard from [dbo].[TB_STANDARD] where ItemTypeId = 10 and Standard like '%SMART POST%' order by pos";
            using (var context = new DbContext(Global.ConnStr))
            {
                colours = context.Database.SqlQuery<string>(sql).ToList<string>();
                heights = context.Database.SqlQuery<string>(sql2).ToList<string>();
                chHeights = context.Database.SqlQuery<string>(sql3).ToList<string>();
                smHeights = context.Database.SqlQuery<string>(sql4).ToList<string>();
            }

            fillCurrTable(Request.Form["frmName"].ToString(), -1);

            ViewBag.Colours = colours;
            ViewBag.Heights = heights;
            ViewBag.chHeights = chHeights;
            ViewBag.smHeights = smHeights;

            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        public ActionResult EnterNoOfPanels()
        {
            ViewBag.Title = "Enter No Of Panels";
            Session["CurrForm"] = "EnterNoOfPanels";
            IList<string> colours = new List<string>();
            IList<string> heights = new List<string>();
            IList<string> chHeights = new List<string>();
            IList<string> smHeights = new List<string>();

            var sql = "select Colour from [dbo].[TB_COLOUR] where ItemTypeId = 1 order by pos";
            var sql2 = "select Standard + case when IsNorm = 0 then ' *' else '' end as Standard from [dbo].[TB_STANDARD] where ItemTypeId = 1 order by pos";
            var sql3 = "select ActStandard + case when IsNorm = 0 then ' *' else '' end as ActStandard from [dbo].[TB_STANDARD] where ItemTypeId = 10 and Standard like '%CHANNEL POST%' order by pos";
            var sql4 = "select ActStandard + case when IsNorm = 0 then ' *' else '' end as ActStandard from [dbo].[TB_STANDARD] where ItemTypeId = 10 and Standard like '%SMART POST%' order by pos";
            using (var context = new DbContext(Global.ConnStr))
            {
                colours = context.Database.SqlQuery<string>(sql).ToList<string>();
                heights = context.Database.SqlQuery<string>(sql2).ToList<string>();
                chHeights = context.Database.SqlQuery<string>(sql3).ToList<string>();
                smHeights = context.Database.SqlQuery<string>(sql4).ToList<string>();
            }


            fillCurrTable(Request.Form["frmName"].ToString(), -1);

            ViewBag.Colours = colours;
            ViewBag.Heights = heights;
            ViewBag.chHeights = chHeights;
            ViewBag.smHeights = smHeights;
            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        private void CheckAnddeleteUpload()
        {

            bool delete = true;
            if ((Session["newOrder"] != null) && (Session["uploadGuid"] != null))
            {
                if (((NewOrder)Session["newOrder"]).OrderDetails != null)
                {
                    foreach (NewOrderDetail nd in ((NewOrder)Session["newOrder"]).OrderDetails)
                    {
                        if (nd.ITEMTYPE == "SpecialOrder")
                        {
                            delete = false;
                        }
                    }
                }
            }

            if (delete)
            {
                if (Session["uploadGuid"] != null)
                {
                    if (Directory.Exists(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\"))))
                    {
                        Directory.Delete(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")), true);
                    }
                }
                Session["uploadGuid"] = null;
            }
        }


        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult PDFQuotation(Quote1 quote1)
        {
            quote1.ADDRESS = "lll";
            string aa = quote1.ADDRESS;
            string bb = quote1.PHONE;



            return View(quote1);
        }


        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult CheckOut(OrderHead newOrderh)
        {
            var currOrderStatus = "";
            if (Request.Form["currOrderStatus"] != null)
            {
                currOrderStatus = Request.Form["currOrderStatus"].ToString();
            }

            if ((currOrderStatus == "4. Submitted") || (currOrderStatus == "5. Custom"))
            {
                return Redirect($"{Url.RouteUrl(new { controller = "Order", action = "OrderTrack" })}");
            }

            //if (newOrderh.ACCNO != "") {
            //    newOrderh.ACCNO = 
            //}

            //if (newOrderh.ACCNO == "") {

            //}

            //var CompanyName = new SqlParameter("@CompanyName", newOrderh.Company);

            //var sql = "exec dbo.proc_GetAccno " +
            //    "@CompanyName";

            //using (var context = new DbContext(Global.ConnStr))
            //{
            //    var data = context.Database.SqlQuery<decimal>(sql,
            //       CompanyName,
            //        ContactName,
            //        CustMobile,
            //        Email,
            //        UserName,
            //        AccNo,
            //        PickUp,
            //        dispatchDate,
            //        PoNum,
            //        Reference,
            //        TotalWeight,
            //        TotalPrice,
            //        ReloadId,
            //        StatusId,
            //        PickupBy,
            //        Role,
            //        pareantOrderNo,
            //        DriverLic,
            //        Address,
            //        City,
            //        State,
            //        Postcode,
            //        Country,
            //        ABN,
            //        TradeType,
            //        SplitOrder,
            //        IP,
            //        Comment,
            //        needAdvice_,
            //        needGetIsQuote).ToList<decimal>()[0];
            //    orderNo = data.ToString();
            //}            

            //var sql = "exec dbo.proc_GetCompanyNameList";
            //using (var context = new DbContext(Global.ConnStr))
            //{
            //    newOrderh.CompanyList = context.Database.SqlQuery<string>(sql).ToList<string>();
            //}


            ViewBag.maxLoad = WebConfigurationManager.AppSettings["maxLoad"];
            var role = ((Scanner.Models.User)Session["User"]).Role;
            var orderNo = "-1";
            newOrderh.SplitOrder = false;
            if (role.IndexOf("Gram") > -1)
            {
                if (newOrderh.CompanyList == null)
                {
                    var sql = "exec dbo.proc_GetCompanyNameList";
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        newOrderh.CompanyList = context.Database.SqlQuery<string>(sql).ToList<string>();
                    }
                }
            }

            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ?
                Request.Form["MenuExpandings"].ToString() : "";

            if (newOrderh != null)
            {
                if (!string.IsNullOrEmpty(newOrderh.Company))
                {
                    newOrderh.Company = newOrderh.Company.ToUpper();
                }
                newOrderh.Message = "";
            }

            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString() == "c"))
            {
                CheckAnddeleteUpload();
                return View(newOrderh);
            }


            if ((Request.Form["splitOrder"] != null) && (Request.Form["splitOrder"].ToString() == "1"))
            {
                newOrderh.SplitOrder = true;
            }


            if ((Request.Form["actReq"] != null) && (Request.Form["actReq"].ToString().IndexOf("splitOrder") > -1))
            {
                newOrderh.SplitOrder = true;
            }


            if ((Session["CurrForm"].ToString() == "OrderTrack") ||
                (Session["CurrForm"].ToString() == "QuotationTrack") ||
                 (Session["CurrForm"].ToString() == "Category") ||
                 (Session["CurrForm"].ToString() == "Registration"))
            {

                if (Session["newOrder"] == null)
                {
                    Session["newOrder"] = new NewOrder();
                    newOrderh.Mobile = ((User)Session["User"]).Mobile;
                    newOrderh.Company = ((User)Session["User"]).CompanyName;
                    newOrderh.OrderBy = ((User)Session["User"]).FirstName + " " + ((User)Session["User"]).LastName;
                    newOrderh.Email = ((User)Session["User"]).UserEmail;

                    newOrderh.Address = ((User)Session["User"]).Address1;
                    newOrderh.City = ((User)Session["User"]).Address2;
                    newOrderh.State = ((User)Session["User"]).State;
                    newOrderh.Postcode = ((User)Session["User"]).Postcode;
                    newOrderh.Country = ((User)Session["User"]).Country;

                }
                else
                {
                    newOrderh = ((NewOrder)Session["newOrder"]).Head;
                }

                ModelState.Remove("Company");
                ModelState.Remove("Mobile");
                ModelState.Remove("OrderBy");
                ModelState.Remove("Email");
                ModelState.Remove("CustomerOrderNo");

                fillNewOrderDetails();

                CheckAnddeleteUpload();
                return View(newOrderh);
            }


            if ((Session["CurrForm"].ToString() != "CheckOut")
                && (Session["CurrForm"].ToString() != "Confirmation"))
            {
                if (Session["newOrder"] == null)
                {
                    Session["newOrder"] = new NewOrder();

                    if ((Session["TradeType"].ToString() == "2") || (Session["TradeType"].ToString() == "3"))
                    {

                        newOrderh.Mobile = "";
                        newOrderh.Company = "";
                        newOrderh.OrderBy = "";
                        newOrderh.Email = "";
                        newOrderh.DriverLic = "";
                        newOrderh.ABN = "";
                        newOrderh.Address = "";
                        newOrderh.City = "";
                        newOrderh.State = "";
                        newOrderh.Postcode = "";
                        newOrderh.Country = "";
                        newOrderh.UserName = "";
                        newOrderh.ReadyToSubmitDate = "";

                    }
                    else
                    {
                        newOrderh.Mobile = ((User)Session["User"]).Mobile;
                        newOrderh.Company = ((User)Session["User"]).CompanyName;
                        newOrderh.OrderBy = ((User)Session["User"]).FirstName + " " + ((User)Session["User"]).LastName;
                        newOrderh.Email = ((User)Session["User"]).UserEmail;
                        newOrderh.Address = ((User)Session["User"]).Address1;
                        newOrderh.City = ((User)Session["User"]).Address2;
                        newOrderh.State = ((User)Session["User"]).State;
                        newOrderh.Postcode = ((User)Session["User"]).Postcode;
                        newOrderh.Country = ((User)Session["User"]).Country;
                        newOrderh.UserName = ((User)Session["User"]).UserName;
                        newOrderh.ReadyToSubmitDate = "";
                    }
                }
                else
                {
                    if (((NewOrder)Session["newOrder"]).Head == null)
                    {
                        if ((Session["TradeType"].ToString() == "2") || (Session["TradeType"].ToString() == "3"))
                        {
                            newOrderh.Mobile = "";
                            newOrderh.Company = "";
                            newOrderh.OrderBy = "";
                            newOrderh.Email = "";
                            newOrderh.DriverLic = "";
                            newOrderh.ABN = "";
                            newOrderh.Address = "";
                            newOrderh.City = "";
                            newOrderh.State = "";
                            newOrderh.Postcode = "";
                            newOrderh.Country = "";
                        }
                        else
                        {
                            newOrderh.Mobile = ((User)Session["User"]).Mobile;
                            newOrderh.Company = ((User)Session["User"]).CompanyName;
                            newOrderh.OrderBy = ((User)Session["User"]).FirstName + " " + ((User)Session["User"]).LastName;
                            newOrderh.Email = ((User)Session["User"]).UserEmail;
                            newOrderh.Address = ((User)Session["User"]).Address1;
                            newOrderh.City = ((User)Session["User"]).Address2;
                            newOrderh.State = ((User)Session["User"]).State;
                            newOrderh.Postcode = ((User)Session["User"]).Postcode;
                            newOrderh.Country = ((User)Session["User"]).Country;
                        }
                        newOrderh.UserName = "";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(((NewOrder)Session["newOrder"]).Head.OrderBy))
                        {
                            if (Session["TradeType"].ToString() == "0")
                            {
                                newOrderh.Mobile = ((User)Session["User"]).Mobile;
                                newOrderh.Company = ((User)Session["User"]).CompanyName;
                                newOrderh.OrderBy = ((User)Session["User"]).FirstName + " " + ((User)Session["User"]).LastName;
                                newOrderh.Email = ((User)Session["User"]).UserEmail;
                            }
                        }
                        else
                        {
                            newOrderh.Mobile = ((NewOrder)Session["newOrder"]).Head.Mobile;
                            newOrderh.Company = ((NewOrder)Session["newOrder"]).Head.Company;
                            newOrderh.OrderBy = ((NewOrder)Session["newOrder"]).Head.OrderBy;
                            newOrderh.Email = ((NewOrder)Session["newOrder"]).Head.Email;
                        }

                        newOrderh.CustomerOrderNo = ((NewOrder)Session["newOrder"]).Head.CustomerOrderNo;
                        newOrderh.RequestForDelivery = ((NewOrder)Session["newOrder"]).Head.RequestForDelivery;
                        newOrderh.Reference = ((NewOrder)Session["newOrder"]).Head.Reference;
                        newOrderh.DispatchDate = ((NewOrder)Session["newOrder"]).Head.DispatchDate;
                        newOrderh.PickupBy = ((NewOrder)Session["newOrder"]).Head.PickupBy;

                        /*************************************/
                        newOrderh.UserName = ((NewOrder)Session["newOrder"]).Head.UserName;
                        newOrderh.ReadyToSubmitDate = ((NewOrder)Session["newOrder"]).Head.ReadyToSubmitDate;
                        newOrderh.OrderDate = ((NewOrder)Session["newOrder"]).Head.OrderDate;
                        /*************************************/

                        if (Session["TradeType"].ToString() != "0")
                        {
                            newOrderh.DriverLic = ((NewOrder)Session["newOrder"]).Head.DriverLic;
                            newOrderh.ABN = ((NewOrder)Session["newOrder"]).Head.ABN;
                        }

                        newOrderh.Address = ((NewOrder)Session["newOrder"]).Head.Address;
                        newOrderh.City = ((NewOrder)Session["newOrder"]).Head.City;
                        newOrderh.State = ((NewOrder)Session["newOrder"]).Head.State;
                        newOrderh.Postcode = ((NewOrder)Session["newOrder"]).Head.Postcode;
                        newOrderh.Country = ((NewOrder)Session["newOrder"]).Head.Country;
                    }
                }




                if (Request.Form["frmName"] != null)
                {
                    var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                    if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                    {
                        rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                    }

                    fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);
                    fillNewOrderDetails();
                    ((NewOrder)Session["newOrder"]).Head = new OrderHead();
                    ((NewOrder)Session["newOrder"]).Head.ACCNO = ((User)Session["User"]).ACCNO.ToString();
                    fillPrice(((NewOrder)Session["newOrder"]));
                }
                Session["CurrForm"] = "CheckOut";
                ViewBag.Title = "CHECK OUT";
                ModelState.Clear();
            }
            else
            {
                if ((((NewOrder)Session["newOrder"]) == null) ||
                    (((NewOrder)Session["newOrder"]).OrderDetails == null) ||
                    (((NewOrder)Session["newOrder"]).OrderDetails.Count < 1))
                {
                    ModelState.AddModelError("noOrder", "Error: No order has been placed.");
                }
                else
                {
                    ResetOrderDetails(); // for Over weight checking
                }

                if (newOrderh.RequestForDelivery)
                {

                    if (string.IsNullOrEmpty(newOrderh.DispatchDate))
                    {
                        ModelState.AddModelError("DispatchDate", "* Request Date is null.");
                    }


                    if ((newOrderh.Address != null) && (newOrderh.Address.Length > 30))
                    {
                        ModelState.AddModelError("Address", "* Address Input (Max 30 letters).");
                    }


                    if ((newOrderh.City != null) && (newOrderh.City.Length > 30))
                    {
                        ModelState.AddModelError("City", "* Suburb/City Input (Max 30 letters).");
                    }


                    if ((newOrderh.State != null) && (newOrderh.State.Length > 30))
                    {
                        ModelState.AddModelError("State", "* State Input (Max 30 letters).");
                    }


                    if ((newOrderh.Postcode != null) && (newOrderh.Postcode.Length > 30))
                    {
                        ModelState.AddModelError("Postcode", "* Postcode Input (Max 30 letters).");
                    }


                    if ((newOrderh.Country != null) && (newOrderh.Country.Length > 30))
                    {
                        ModelState.AddModelError("Country", "* Country Input (Max 30 letters).");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(newOrderh.PickupBy))
                    {
                        ModelState.AddModelError("DispatchDate", "*  Pickup By Input(Max 20 letters).");
                    }
                    else
                    {
                        if (newOrderh.PickupBy.Length > 20)
                        {
                            ModelState.AddModelError("DispatchDate", "*  Pickup By Input(Max 20 letters).");
                        }
                    }
                }


                //if ((Session["TradeType"].ToString() == "1") && (string.IsNullOrEmpty(newOrderh.DriverLic)))
                //{
                //    ModelState.AddModelError("DriverLic", "*  Driver License is null.");
                //}

                //if ((Session["TradeType"].ToString() == "2") && (string.IsNullOrEmpty(newOrderh.DriverLic)))
                //{
                //    ModelState.AddModelError("DriverLic", "*  Driver License is null.");
                //}

                //if ((Session["TradeType"].ToString() == "2") && (string.IsNullOrEmpty(newOrderh.ABN)))
                //{
                //    ModelState.AddModelError("ABN", "*  ABN is null.");
                //}


                if ((role.IndexOf("Gram") < 0) && (Request.Form["compConfirm"] != null))
                {
                    var compConfirm = Request.Form["compConfirm"];

                    if (compConfirm.ToString().ToLower() != "true")
                    {
                        ModelState.AddModelError("Company", "* Click check box to confirm.");
                    }
                }

                //if (role.IndexOf("Gram") < 0) {
                //    var compConfirm = Request.Form["compConfirm"];

                //    if (compConfirm.ToString().ToLower() != "true") {
                //        ModelState.AddModelError("Company", "* Click check box to confirm.");
                //    }
                //}

                ModelState.Remove("SplitOrder");
                if (ModelState.IsValid)
                {
                    ((NewOrder)Session["newOrder"]).Head = newOrderh;
                    ((NewOrder)Session["newOrder"]).Head.ACCNO = ((User)Session["User"]).ACCNO.ToString();

                    var actReq = "";
                    if (Request.Form["actReq"] != null)
                    {
                        if (Request.Form["actReq"].ToString() == "save")
                            actReq = "save";

                        if (Request.Form["actReq"].ToString() == "quotation")
                            actReq = "quotation";

                        if (Request.Form["actReq"].ToString() == "trial")
                            actReq = "trial";

                    }


                    if (((actReq != "") && (actReq == "save" || actReq == "quotation" || actReq == "trial"))
                        && ((currOrderStatus != "4. Submitted") && (currOrderStatus != "5. Custom")))
                    {
                        ((NewOrder)Session["newOrder"]).Head = newOrderh;
                        ((NewOrder)Session["newOrder"]).Head.ACCNO = ((User)Session["User"]).ACCNO.ToString();
                        string WrongItem = "";
                        try
                        {
                            fillPrice(((NewOrder)Session["newOrder"]));
                            ViewBag.needRemove = false;

                            if (newOrderh.TotalWeight > Convert.ToDouble(ViewBag.maxLoad))
                            {
                                //ModelState.AddModelError("", "*  Max Truck load is " + ViewBag.maxLoad + "kg. Please remove none urgent items.");
                                CheckAnddeleteUpload();
                                ViewBag.needRemove = true;
                                return View(newOrderh);
                            }

                            var CustCompName = new SqlParameter("@CustCompName", newOrderh.Company);
                            var ContactName = new SqlParameter("@ContactName", newOrderh.OrderBy);
                            var CustMobile = new SqlParameter("@CustMobile", newOrderh.Mobile);
                            var Email = new SqlParameter("@Email", newOrderh.Email);
                            var UserName = new SqlParameter("@UserName", ((User)Session["User"]).UserName.ToString());
                            var AccNo = new SqlParameter("@AccNo", ((User)Session["User"]).ACCNO.ToString());
                            var PickUp = (newOrderh.RequestForDelivery == true) ? new SqlParameter("@PickUp", "0") : new SqlParameter("@PickUp", "1");
                            var newDate = (!string.IsNullOrEmpty(newOrderh.DispatchDate)) ? newOrderh.DispatchDate.Split('/')[1] + "/" + newOrderh.DispatchDate.Split('/')[0] + "/" + newOrderh.DispatchDate.Split('/')[2] : "";
                            var dispatchDate = new SqlParameter("@DispatchDate", newDate);
                            var PoNum = new SqlParameter("@PoNum", (newOrderh.CustomerOrderNo == null) ? "" : newOrderh.CustomerOrderNo);
                            var Reference = new SqlParameter("@Reference", (newOrderh.Reference == null) ? "" : newOrderh.Reference);
                            var TotalWeight = new SqlParameter("@TotalWeight", newOrderh.TotalWeight);
                            var TotalPrice = new SqlParameter("@TotalPrice", newOrderh.TotalPrice);
                            var ReloadId = (Session["ReloadId"] == null) ? new SqlParameter("@ReloadId", "") : new SqlParameter("@ReloadId", Session["ReloadId"].ToString());
                            var Role = new SqlParameter("@Role", ((User)Session["User"]).Role);

                            int statusId = 0;
                            if (actReq == "quotation")
                                statusId = 9;

                            if (actReq == "trial")
                                statusId = 3;

                            Session["isQuote"] = "0";
                            if ((((User)Session["User"]).UserName.ToString() == "quotation") || (actReq == "quotation"))
                            {
                                // newOrderh.Message = orderNo + ";Quotation;Your Quotation has been Submitted Successfully.<br/>Quotation Id: " + orderNo + ".|Would you like to print a copy?";
                                CheckAnddeleteUpload();
                                Session["isQuote"] = "1";
                                //  return Redirect($"{Url.RouteUrl(new { controller = "Order", action = "Confirmation" })}");
                            }


                            var StatusId = new SqlParameter("@StatusId", statusId.ToString());

                            var pareantOrderNo = new SqlParameter("@pareantOrderNo", "-1");

                            var PickupBy = (string.IsNullOrEmpty(newOrderh.PickupBy)) ? new SqlParameter("@PickupBy", "") : new SqlParameter("@PickupBy", newOrderh.PickupBy);

                            var DriverLic = new SqlParameter("@DriverLic", (newOrderh.DriverLic == null) ? "" : newOrderh.DriverLic);
                            var ABN = new SqlParameter("@ABN", (newOrderh.ABN == null) ? "" : newOrderh.ABN);

                            var Address = new SqlParameter("@Address", (newOrderh.Address == null) ? "" : newOrderh.Address);
                            var City = new SqlParameter("@City", (newOrderh.City == null) ? "" : newOrderh.City);
                            var State = new SqlParameter("@State", (newOrderh.State == null) ? "" : newOrderh.State);
                            var Postcode = new SqlParameter("@Postcode", (newOrderh.Postcode == null) ? "" : newOrderh.Postcode);
                            var Country = new SqlParameter("@Country", (newOrderh.Country == null) ? "" : newOrderh.Country);


                            var TradeType = new SqlParameter("@TradeType", (Session["TradeType"] == null) ? 0 : Convert.ToInt32(Session["TradeType"]));

                            var SplitOrder = new SqlParameter("@SplitOrder", newOrderh.SplitOrder);
                            var IP = new SqlParameter("@IP", (Session["IP"] != null) ? Session["IP"].ToString() : "");
                            var Comment = new SqlParameter("@Comment", (newOrderh.Comment == null) ? "" : newOrderh.Comment);
                            var needAdvice_ = new SqlParameter("@needAdvice", "0");
                            var needGetIsQuote = new SqlParameter("@needGetIsQuote", "-1");


                            //var ORDERNO = new SqlParameter("@ORDERNO", SqlDbType.Int);
                            //ORDERNO.Direction = ParameterDirection.Output;

                            //check out first proc_AddOrderHead
                            var sql = "exec dbo.proc_AddOrderHead_v1 " +
                                "@CustCompName," +
                                "@ContactName," +
                                "@CustMobile," +
                                "@Email," +
                                "@UserName," +
                                "@AccNo," +
                                "@PickUp," +
                                "@DispatchDate," +
                                "@PoNum," +
                                "@Reference," +
                                "@TotalWeight," +
                                "@TotalPrice," +
                                "@ReloadId," +
                                "@StatusId," +
                                "@PickupBy," +
                                "@Role," +
                                "@pareantOrderNo," +
                                "@DriverLic," +
                                "@ABN," +
                                "@Address," +
                                "@city," +
                                "@State," +
                                "@postcode," +
                                "@Country," +
                                "@TradeType," +
                                "@SplitOrder," +
                                "@IP," +
                                "@Comment," +
                                "@needAdvice," +
                                "@needGetIsQuote";

                            using (var context = new DbContext(Global.ConnStr))
                            {
                                var data = context.Database.SqlQuery<decimal>(sql,
                                    CustCompName,
                                    ContactName,
                                    CustMobile,
                                    Email,
                                    UserName,
                                    AccNo,
                                    PickUp,
                                    dispatchDate,
                                    PoNum,
                                    Reference,
                                    TotalWeight,
                                    TotalPrice,
                                    ReloadId,
                                    StatusId,
                                    PickupBy,
                                    Role,
                                    pareantOrderNo,
                                    DriverLic,
                                    Address,
                                    City,
                                    State,
                                    Postcode,
                                    Country,
                                    ABN,
                                    TradeType,
                                    SplitOrder,
                                    IP,
                                    Comment,
                                    needAdvice_,
                                    needGetIsQuote).ToList<decimal>()[0];
                                orderNo = data.ToString();
                            }

                            if (Convert.ToInt32(orderNo) < 0)
                            {
                                if (Convert.ToInt32(orderNo) == -1)
                                {
                                    Session["errorMessage"] = "This Order (Track Id: " + Session["ReloadId"].ToString() + ") has been cancelled already.";
                                }
                                else
                                {
                                    Session["errorMessage"] = "This Order seems submitted already,<br/> with Order NO: " + orderNo.Substring(1, orderNo.Length - 1) + ".<br/>If need to force to submit it again,<br/>please change the Referenece or PONum on the Check Out page, and submit it again.";
                                }

                                CheckAnddeleteUpload();
                                return Redirect($"{Url.RouteUrl(new { controller = "Order", action = "Index" })}");
                                //return View(newOrderh);
                            }
                            else
                            {
                                Session["ReloadId"] = orderNo;
                            }

                            var order = ((NewOrder)Session["newOrder"]);


                            using (var context = new DbContext(Global.ConnStr))
                            {
                                foreach (NewOrderDetail orderDetail in order.OrderDetails)
                                {
                                    if ((orderDetail.ITEMTYPE == "SpecialOrder") &&
                                (orderDetail.DESCRIPTION.IndexOf("MISC ITEM") > -1) &&
                                (Session["uploadGuid"] != null))
                                    {
                                        if (Session["UploadingNote"].ToString() != "")
                                        {
                                            orderDetail.Cuts = -1;
                                            orderDetail.CutDesc = Session["UploadingNote"].ToString();
                                        }

                                        if (Directory.Exists(string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + orderNo, Server.MapPath(@"\"))))
                                        {
                                            Directory.Delete(string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + orderNo, Server.MapPath(@"\")), true);
                                        }


                                        if (Directory.Exists(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\"))))
                                        {
                                            Directory.Move(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")), string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + orderNo, Server.MapPath(@"\")));
                                        }
                                    }

                                    WrongItem = orderDetail.DESCRIPTION;
                                    object[] parameters = {
                                        orderNo,
                                        orderDetail.ITEMTYPE,
                                        orderDetail.COLOUR,
                                        orderDetail.STANDARD,
                                        orderDetail.STOCKCODE,
                                        orderDetail.DESCRIPTION,
                                        orderDetail.WEIGHT,
                                        orderDetail.PRICE,
                                        orderDetail.QTY,
                                        orderDetail.PQTY,
                                        orderDetail.DISCOUNTRATE,
                                        orderDetail.BASEPRICE,
                                        orderDetail.SUNDRYCOLOUR,
                                        orderDetail.isNormal,
                                        orderDetail.C_X,
                                        orderDetail.C_Y,
                                        orderDetail.Cuts,
                                        orderDetail.CutDesc,
                                        orderDetail.nextRelColorId,
                                        orderDetail.menuId
                                    };

                                    //check out first proc_AddOrderDetail
                                    context.Database.ExecuteSqlCommand("proc_AddOrderDetail_v1 {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}", parameters);
                                }

                                if (Request.Form["actReq"].ToString() == "trial")
                                {
                                    object[] parameters = {
                                            orderNo,
                                            CustCompName.Value,
                                            UserName.Value
                                        };
                                    context.Database.ExecuteSqlCommand("proc_SendStiky {0},{1},{2}", parameters);
                                }
                            }

                            string status = "Saved";
                            if (actReq == "trial")
                                status = "Ready to Proceed";

                            if ((((User)Session["User"]).UserName.ToString() == "quotation") || (actReq == "quotation"))
                            {
                                newOrderh.Message = orderNo + ";Quotation;Your Quotation has been Submitted Successfully.<br/>Quotation Id: " + orderNo + ".|Would you like to print a copy?";

                                if (((User)Session["User"]).UserName.ToString() == "quotation")
                                {

                                    using (var context = new DbContext(Global.ConnStr))
                                    {
                                        object[] parameters = {
                                            orderNo,
                                            ((User)Session["User"]).ACCNO.ToString(),
                                            newOrderh.Email,
                                            WebConfigurationManager.AppSettings["GramBccEmails"],
                                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                                            ""
                                        };
                                        context.Database.ExecuteSqlCommand("proc_SendOrderNotification_v1 {0},{1},{2},{3},{4},{5}", parameters);
                                    }
                                }
                                //CheckAnddeleteUpload();
                                //return Redirect($"{Url.RouteUrl(new { controller = "Order", action = "Confirmation" })}");
                            }
                            else
                            {
                                if (status == "Ready to Proceed")
                                {
                                    newOrderh.Message = orderNo + ";" + status + ";Thank you for your order.<br/>Your Order Request has been Successfully Submitted.<br/>Order Track Id: " + orderNo + ".|Would you like to print a copy?";
                                }
                                else
                                {
                                    newOrderh.Message = orderNo + ";" + status + ";Your Order Request has been Successfully Saved.<br/>Order Track Id: " + orderNo + ".|Would you like to print a copy?";
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            using (var context = new DbContext(Global.ConnStr))
                            {
                                string errBody = @"Company: " + newOrderh.Company + "<br>Order By: " + newOrderh.OrderBy + "<br>Mobile " + newOrderh.Mobile + "<br><br><br>" +
                                    WrongItem + "<br><br>" +
                                    e.Message.Replace(Environment.NewLine, "<br>");
                                if (((e.InnerException != null) && (!string.IsNullOrEmpty(e.InnerException.Message))))
                                {
                                    errBody += @"<br><br><br>" + e.InnerException.Message.Replace("\n", "<br>");
                                }

                                object[] parameters = {
                                        WebConfigurationManager.AppSettings["GramAdminEmails"],
                                        "Error: Order "+ orderNo+ " (Saved, Quotation, Ready to Submit)",
                                        errBody,
                                        "",
                                        "",
                                        ""
                                    };
                                context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters);
                                newOrderh.Message = "Error:<br/>Your Order Request has been placed unsuccessfully.<br/>An error message has been sent to our IT department already.<br/>And we will get this issue fixed as soon as possible.<br/>Sorry for your inconvenience.";
                            }
                        }
                    }
                    else
                    {
                        if (Request.Form["actReq"].ToString() == "splitOrder")
                        {
                            ((NewOrder)Session["newOrder"]).SplitOrder = true;
                        }
                        else
                        {
                            ((NewOrder)Session["newOrder"]).SplitOrder = false;
                        }

                        CheckAnddeleteUpload();
                        return Redirect($"{Url.RouteUrl(new { controller = "Order", action = "Confirmation" })}");
                    }
                }
            }

            if (Convert.ToInt32(orderNo) > 0)
            {
                newOrderh.PrintId1 = orderNo;
            }
            CheckAnddeleteUpload();

            if (string.IsNullOrEmpty(newOrderh.SubmittedBy))
            {
                newOrderh.SubmittedBy = newOrderh.OrderBy;
            }

            if (string.IsNullOrEmpty(newOrderh.OrderDate))
            {
                newOrderh.OrderDate = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
            }

            if (string.IsNullOrEmpty(newOrderh.SubmitDate))
            {
                newOrderh.SubmitDate = newOrderh.OrderDate;
            }

            ((NewOrder)Session["newOrder"]).Head = newOrderh;
            return View(newOrderh);
        }

        private void ResetOrderDetails()
        {
            var ReMainDetails = new List<NewOrderDetail>();
            for (int i = 0; i < ((NewOrder)Session["newOrder"]).OrderDetails.Count - 1; i++)
            {
                if (Request.Form["rmItmChk" + i.ToString()] != null)
                {
                    ReMainDetails.Add(((NewOrder)Session["newOrder"]).OrderDetails[i]);
                    ((NewOrder)Session["newOrder"]).OrderDetails.Remove(((NewOrder)Session["newOrder"]).OrderDetails[i]);
                    i--;
                }
            }
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Confirmation()
        {
            //  Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            Session["CurrForm"] = "Confirmation";

            //if ((Session["CurrForm"].ToString() != "Confirmation"))
            //{
            //    Session["CurrForm"] = "Confirmation";
            //}
            //else {
            //}

            if (((NewOrder)Session["newOrder"]) != null)
            {
                ((NewOrder)Session["newOrder"]).Head.Message = "";
            }


            return View(((NewOrder)Session["newOrder"]).Head);
        }

        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult Confirmation(OrderHead newOrderh)
        {
            Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            Session["CurrForm"] = "Confirmation";


            string currDateTime = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
            if (string.IsNullOrEmpty(newOrderh.OrderDate))
            {
                newOrderh.OrderDate = currDateTime;
            }


            if (string.IsNullOrEmpty(newOrderh.SubmitDate))
            {
                newOrderh.SubmitDate = currDateTime;
            }


            if (string.IsNullOrEmpty(newOrderh.LastUpdate))
            {
                newOrderh.LastUpdate = currDateTime;
            }

            ((NewOrder)Session["newOrder"]).Head = newOrderh;
            ((NewOrder)Session["newOrder"]).Head.Message = "";
            ((NewOrder)Session["newOrder"]).Head.PrintId1 = "";
            ((NewOrder)Session["newOrder"]).Head.PrintId2 = "";

            var role = ((Scanner.Models.User)Session["User"]).Role;

            ((NewOrder)Session["newOrder"]).Head.ACCNO = ((User)Session["User"]).ACCNO.ToString();

            if (role.IndexOf("Gram") > -1)
            {
                var COMPNAME = new SqlParameter("@CompanyName", newOrderh.Company);
                var sql = "exec dbo.proc_GetCompanyAccno @CompanyName";
                var nACCNO = "";
                using (var context = new DbContext(Global.ConnStr))
                {
                    var nACCNOs = context.Database.SqlQuery<string>(sql, COMPNAME).ToList<string>();
                    if (nACCNOs != null)
                    {
                        if (nACCNOs.Count > 0)
                        {
                            nACCNO = nACCNOs[0];
                        }
                    }

                    if (!string.IsNullOrEmpty(nACCNO))
                    {
                        ((NewOrder)Session["newOrder"]).Head.ACCNO = nACCNO;
                    }
                }
            }

            var orderNo = "-1";
            string serverRefNo = "";
            try
            {
                var order = ((NewOrder)Session["newOrder"]);


                bool isAllUnnormal = true;

                foreach (var itm in order.OrderDetails)
                {
                    if (itm.isNormal)
                    {
                        isAllUnnormal = false;
                        break;
                    }
                }

                fillPrice(((NewOrder)Session["newOrder"]));
                var CustCompName = new SqlParameter("@CustCompName", newOrderh.Company);
                var ContactName = new SqlParameter("@ContactName", newOrderh.OrderBy);
                var CustMobile = new SqlParameter("@CustMobile", newOrderh.Mobile);
                var Email = new SqlParameter("@Email", newOrderh.Email);
                var UserName = new SqlParameter("@UserName", ((User)Session["User"]).UserName.ToString());
                var AccNo = new SqlParameter("@AccNo", ((User)Session["User"]).ACCNO.ToString());
                var PickUp = (newOrderh.RequestForDelivery == true) ? new SqlParameter("@PickUp", "0") : new SqlParameter("@PickUp", "1");
                var newDate = (!string.IsNullOrEmpty(newOrderh.DispatchDate)) ? newOrderh.DispatchDate.Split('/')[1] + "/" + newOrderh.DispatchDate.Split('/')[0] + "/" + newOrderh.DispatchDate.Split('/')[2] : "";
                var dispatchDate = new SqlParameter("@DispatchDate", newDate);
                var PoNum = new SqlParameter("@PoNum", (newOrderh.CustomerOrderNo == null) ? "" : newOrderh.CustomerOrderNo);
                var Reference = new SqlParameter("@Reference", (newOrderh.Reference == null) ? "" : newOrderh.Reference);
                var TotalWeight = new SqlParameter("@TotalWeight", newOrderh.TotalWeight);
                var TotalPrice = new SqlParameter("@TotalPrice", newOrderh.TotalPrice);
                var ReloadId = (Session["ReloadId"] == null) ? new SqlParameter("@ReloadId", "") : new SqlParameter("@ReloadId", Session["ReloadId"].ToString());
                var StatusId = (isAllUnnormal) ? new SqlParameter("@StatusId", "2") : new SqlParameter("@StatusId", "1");
                var PickupBy = (string.IsNullOrEmpty(newOrderh.PickupBy)) ? new SqlParameter("@PickupBy", "") : new SqlParameter("@PickupBy", newOrderh.PickupBy);
                var Role = new SqlParameter("@Role", ((User)Session["User"]).Role);
                var pareantOrderNo = new SqlParameter("@pareantOrderNo", "-1");
                var DriverLic = new SqlParameter("@DriverLic", (newOrderh.DriverLic == null) ? "" : newOrderh.DriverLic);
                var ABN = new SqlParameter("@ABN", (newOrderh.ABN == null) ? "" : newOrderh.ABN);
                var Address = new SqlParameter("@Address", (newOrderh.Address == null) ? "" : newOrderh.Address);
                var City = new SqlParameter("@City", (newOrderh.City == null) ? "" : newOrderh.City);
                var State = new SqlParameter("@State", (newOrderh.State == null) ? "" : newOrderh.State);
                var Postcode = new SqlParameter("@Postcode", (newOrderh.Postcode == null) ? "" : newOrderh.Postcode);
                var Country = new SqlParameter("@Country", (newOrderh.Country == null) ? "" : newOrderh.Country);
                var TradeType = new SqlParameter("@TradeType", (Session["TradeType"] == null) ? 0 : Convert.ToInt32(Session["TradeType"]));
                var SplitOrder = new SqlParameter("@SplitOrder", false);
                var IP = new SqlParameter("@IP", (Session["IP"] != null) ? Session["IP"].ToString() : "");
                var Comment = new SqlParameter("@Comment", (newOrderh.Comment == null) ? "" : newOrderh.Comment);
                var needAdvice = new SqlParameter("@needAdvice", (Session["needAdvice"] == null) ? "0" : Session["needAdvice"].ToString());
                var needGetIsQuote = new SqlParameter("@needGetIsQuote", "-1");


                if (Session["isQuote"] != null)
                {
                    if (Session["isQuote"].ToString() == "1")
                    {
                        needGetIsQuote = new SqlParameter("@needGetIsQuote", "-2");
                    }
                }



                //var ORDERNO = new SqlParameter("@ORDERNO", SqlDbType.Int);
                //ORDERNO.Direction = ParameterDirection.Output;               
                //confirm first proc_AddOrderHead
                var sql = "exec dbo.proc_AddOrderHead_v1 " +
                    "@CustCompName," +
                    "@ContactName," +
                    "@CustMobile," +
                    "@Email," +
                    "@UserName," +
                    "@AccNo," +
                    "@PickUp," +
                    "@DispatchDate," +
                    "@PoNum," +
                    "@Reference," +
                    "@TotalWeight," +
                    "@TotalPrice," +
                    "@ReloadId," +
                    "@StatusId," +
                    "@PickupBy," +
                    "@Role," +
                    "@pareantOrderNo," +
                    "@DriverLic," +
                    "@ABN," +
                    "@Address," +
                    "@City," +
                    "@State," +
                    "@Postcode," +
                    "@Country," +
                    "@TradeType," +
                    "@SplitOrder," +
                    "@IP," +
                    "@Comment," +
                    "@needAdvice," +
                    "@needGetIsQuote";

                using (var context = new DbContext(Global.ConnStr))
                {
                    var data = context.Database.SqlQuery<decimal>(sql,
                        CustCompName,
                        ContactName,
                        CustMobile,
                        Email,
                        UserName,
                        AccNo,
                        PickUp,
                        dispatchDate,
                        PoNum,
                        Reference,
                        TotalWeight,
                        TotalPrice,
                        ReloadId,
                        StatusId,
                        PickupBy,
                        Role,
                        pareantOrderNo,
                        DriverLic,
                        ABN,
                        Address,
                        City,
                        State,
                        Postcode,
                        Country,
                        TradeType,
                        SplitOrder,
                        IP,
                        Comment,
                        needAdvice,
                        needGetIsQuote).ToList<decimal>()[0];
                    orderNo = data.ToString();
                }


                if (Session["HandledBy"] != null)
                {
                    if (Session["HandledBy"].ToString() != "")
                    {
                        sql = "update dbo.TB_ORDER set UserId =" + Session["HandledBy"].ToString() + " where Id=" + orderNo;
                        using (var context = new DbContext(Global.ConnStr))
                        {
                            context.Database.ExecuteSqlCommand(sql);
                        }
                    }
                }

                if (Convert.ToInt32(orderNo) < 0)
                {
                    if (Convert.ToInt32(orderNo) == -1)
                    {
                        Session["errorMessage"] = "Error:<br/>This Order (Track Id: " + Session["ReloadId"].ToString() + ") has been cancelled already.";
                    }
                    else
                    {
                        Session["errorMessage"] = "This Order seems submitted already,<br/> with Order NO: " + orderNo.Substring(1, orderNo.Length - 1) + ".<br/>If need to force to submit it again,<br/>please change the Referenece or PONum on the Check Out page, and submit it again.";
                    }

                    CheckAnddeleteUpload();
                    Session["HandledBy"] = null;
                    return Redirect($"{Url.RouteUrl(new { controller = "Order", action = "Index" })}");

                    //return View(((NewOrder)Session["newOrder"]).Head);
                }

                if (!order.SplitOrder)
                {
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        foreach (NewOrderDetail orderDetail in order.OrderDetails)
                        {

                            if ((orderDetail.ITEMTYPE == "SpecialOrder") &&
                                 (orderDetail.DESCRIPTION.IndexOf("MISC ITEM") > -1) &&
                                 (Session["uploadGuid"] != null))
                            {

                                if (Session["UploadingNote"].ToString() != "")
                                {
                                    orderDetail.Cuts = -1;
                                    orderDetail.CutDesc = Session["UploadingNote"].ToString();
                                }

                                if (Directory.Exists(string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + orderNo, Server.MapPath(@"\"))))
                                {
                                    Directory.Delete(string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + orderNo, Server.MapPath(@"\")), true);
                                }


                                if (Directory.Exists(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\"))))
                                {
                                    Directory.Move(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")), string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + orderNo, Server.MapPath(@"\")));
                                }
                            }

                            if (orderDetail.needAdvice == false)
                            {
                                object[] parameters = {
                                    orderNo,
                                    orderDetail.ITEMTYPE,
                                    orderDetail.COLOUR,
                                    orderDetail.STANDARD,
                                    orderDetail.STOCKCODE,
                                    orderDetail.DESCRIPTION,
                                    orderDetail.WEIGHT,
                                    orderDetail.PRICE,
                                    orderDetail.QTY,
                                    orderDetail.PQTY,
                                    orderDetail.DISCOUNTRATE,
                                    orderDetail.BASEPRICE,
                                    orderDetail.SUNDRYCOLOUR,
                                    orderDetail.isNormal,
                                    orderDetail.C_X,
                                    orderDetail.C_Y,
                                    orderDetail.Cuts,
                                    orderDetail.CutDesc,
                                    orderDetail.nextRelColorId,
                                    orderDetail.menuId
                                };

                                //confirm first proc_AddOrderDetail All items
                                context.Database.ExecuteSqlCommand("proc_AddOrderDetail_v1 {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}", parameters);
                            }
                        }
                    }

                    using (var context = new DbContext(Global.ConnStr))
                    {
                        object[] parameters = {
                                Convert.ToInt32(orderNo)
                        };

                        sql = "exec dbo.X_PROC_RECREATE_SALESORDER_v1 {0}";
                        var data = context.Database.SqlQuery<int>(sql,
                            parameters).ToList<int>()[0];
                        serverRefNo = data.ToString();
                    }

                    ((NewOrder)Session["newOrder"]).Head.PrintId1 = "Track Id: " + orderNo + ", Order Id: " + serverRefNo;

                    using (var context = new DbContext(Global.ConnStr))
                    {
                        object[] parameters = {
                            orderNo,
                            ((User)Session["User"]).ACCNO.ToString(),
                            newOrderh.Email,
                            WebConfigurationManager.AppSettings["GramBccEmails"],
                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                            ""
                        };
                        context.Database.ExecuteSqlCommand("proc_SendOrderNotification_v1 {0},{1},{2},{3},{4},{5}", parameters);
                        newOrderh.Message = "Your Order Request has been placed Successfully. Your Reference No is: " + serverRefNo + " and email Notification has been sent. One of our representives will contact you as soon as possible.<br/>Thank you very much.<br/><br/>Do you want to print a copy?";
                    }
                }
                else //it is split order
                {
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        foreach (NewOrderDetail orderDetail in order.OrderDetails)
                        {
                            if ((orderDetail.isNormal) && (orderDetail.needAdvice == false))
                            {
                                object[] parameters = {
                                    orderNo,
                                    orderDetail.ITEMTYPE,
                                    orderDetail.COLOUR,
                                    orderDetail.STANDARD,
                                    orderDetail.STOCKCODE,
                                    orderDetail.DESCRIPTION,
                                    orderDetail.WEIGHT,
                                    orderDetail.PRICE,
                                    orderDetail.QTY,
                                    orderDetail.PQTY,
                                    orderDetail.DISCOUNTRATE,
                                    orderDetail.BASEPRICE,
                                    orderDetail.SUNDRYCOLOUR,
                                    orderDetail.isNormal,
                                    orderDetail.C_X,
                                    orderDetail.C_Y,
                                    orderDetail.Cuts,
                                    orderDetail.CutDesc,
                                    orderDetail.nextRelColorId,
                                    orderDetail.menuId
                                };
                                //confirm second proc_AddOrderDetail normal and not to be advice (TBA) items
                                context.Database.ExecuteSqlCommand("proc_AddOrderDetail_v1 {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}", parameters);
                            }
                        }
                    }

                    using (var context = new DbContext(Global.ConnStr))
                    {
                        object[] parameters = {
                            Convert.ToInt32(orderNo)
                    };


                        sql = "exec dbo.X_PROC_RECREATE_SALESORDER_v1 {0}";
                        var data = context.Database.SqlQuery<int>(sql,
                            parameters).ToList<int>()[0];
                        serverRefNo = data.ToString();
                    }

                   ((NewOrder)Session["newOrder"]).Head.PrintId1 = "Track Id: " + orderNo + ", Order Id: " + serverRefNo;

                    using (var context = new DbContext(Global.ConnStr))
                    {
                        object[] parameters = {
                        orderNo,
                        ((User)Session["User"]).ACCNO.ToString(),
                        newOrderh.Email,
                        WebConfigurationManager.AppSettings["GramBccEmails"],
                        WebConfigurationManager.AppSettings["GramAdminEmails"],
                        ""
                    };
                        context.Database.ExecuteSqlCommand("proc_SendOrderNotification_v1 {0},{1},{2},{3},{4},{5}", parameters);
                        newOrderh.Message = "Your Order Request has been placed Successfully. Your Reference No is: " + serverRefNo + " and email Notification has been sent. One of our representives will contact you as soon as possible.<br/>Thank you very much.";
                    }

                    var CustCompName_ = new SqlParameter("@CustCompName", newOrderh.Company);
                    var ContactName_ = new SqlParameter("@ContactName", newOrderh.OrderBy);
                    var CustMobile_ = new SqlParameter("@CustMobile", newOrderh.Mobile);
                    var Email_ = new SqlParameter("@Email", newOrderh.Email);
                    var UserName_ = new SqlParameter("@UserName", ((User)Session["User"]).UserName.ToString());
                    var AccNo_ = new SqlParameter("@AccNo", ((User)Session["User"]).ACCNO.ToString());
                    var PickUp_ = (newOrderh.RequestForDelivery == true) ? new SqlParameter("@PickUp", "0") : new SqlParameter("@PickUp", "1");
                    var newDate_ = (!string.IsNullOrEmpty(newOrderh.DispatchDate)) ? newOrderh.DispatchDate.Split('/')[1] + "/" + newOrderh.DispatchDate.Split('/')[0] + "/" + newOrderh.DispatchDate.Split('/')[2] : "";
                    var dispatchDate_ = new SqlParameter("@DispatchDate", newDate_);
                    var PoNum_ = new SqlParameter("@PoNum", (newOrderh.CustomerOrderNo == null) ? "" : newOrderh.CustomerOrderNo);
                    var Reference_ = new SqlParameter("@Reference", (newOrderh.Reference == null) ? "" : newOrderh.Reference);
                    var TotalWeight_ = new SqlParameter("@TotalWeight", newOrderh.TotalWeight);
                    var TotalPrice_ = new SqlParameter("@TotalPrice", newOrderh.TotalPrice);
                    var ReloadId_ = new SqlParameter("@ReloadId", "");
                    var StatusId_ = new SqlParameter("@StatusId", "2");
                    var PickupBy_ = (string.IsNullOrEmpty(newOrderh.PickupBy)) ? new SqlParameter("@PickupBy", "") : new SqlParameter("@PickupBy", newOrderh.PickupBy);
                    var Role_ = new SqlParameter("@Role", ((User)Session["User"]).Role);
                    var pareantOrderNo_ = new SqlParameter("@pareantOrderNo", orderNo);
                    var DriverLic_ = new SqlParameter("@DriverLic", (newOrderh.DriverLic == null) ? "" : newOrderh.DriverLic);
                    var ABN_ = new SqlParameter("@ABN", (newOrderh.ABN == null) ? "" : newOrderh.ABN);

                    var Address_ = new SqlParameter("@Address", (newOrderh.Address == null) ? "" : newOrderh.Address);
                    var City_ = new SqlParameter("@City", (newOrderh.City == null) ? "" : newOrderh.City);
                    var State_ = new SqlParameter("@State", (newOrderh.State == null) ? "" : newOrderh.State);
                    var Postcode_ = new SqlParameter("@Postcode", (newOrderh.Postcode == null) ? "" : newOrderh.Postcode);
                    var Country_ = new SqlParameter("@Country", (newOrderh.Country == null) ? "" : newOrderh.Country);
                    var TradeType_ = new SqlParameter("@TradeType", (Session["TradeType"] == null) ? 0 : Convert.ToInt32(Session["TradeType"]));
                    var SplitOrder_ = new SqlParameter("@SplitOrder", false);
                    var IP_ = new SqlParameter("@IP", (Session["IP"] != null) ? Session["IP"].ToString() : "");
                    var Comment_ = new SqlParameter("@Comment", (newOrderh.Comment == null) ? "" : newOrderh.Comment);
                    var needAdvice_ = new SqlParameter("@needAdvice", "0");
                    var needGetIsQuote_ = new SqlParameter("@needGetIsQuote", orderNo);


                    //var ORDERNO = new SqlParameter("@ORDERNO", SqlDbType.Int);
                    //ORDERNO.Direction = ParameterDirection.Output;
                    //confirm second proc_AddOrderHead un normal and to be advice (TBA) items

                    sql = "exec dbo.proc_AddOrderHead_v1 " +
                        "@CustCompName," +
                        "@ContactName," +
                        "@CustMobile," +
                        "@Email," +
                        "@UserName," +
                        "@AccNo," +
                        "@PickUp," +
                        "@DispatchDate," +
                        "@PoNum," +
                        "@Reference," +
                        "@TotalWeight," +
                        "@TotalPrice," +
                        "@ReloadId," +
                        "@StatusId," +
                        "@PickupBy," +
                        "@Role," +
                        "@pareantOrderNo," +
                        "@DriverLic," +
                        "@ABN," +
                        "@Address," +
                        "@City," +
                        "@State," +
                        "@Postcode," +
                        "@Country," +
                        "@TradeType," +
                        "@SplitOrder," +
                        "@IP," +
                        "@Comment," +
                        "@needAdvice," +
                        "@needGetIsQuote";


                    string firstOrderId = orderNo.ToString();
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        var data = context.Database.SqlQuery<decimal>(sql,
                            CustCompName_,
                            ContactName_,
                            CustMobile_,
                            Email_,
                            UserName_,
                            AccNo_,
                            PickUp_,
                            dispatchDate_,
                            PoNum_,
                            Reference_,
                            TotalWeight_,
                            TotalPrice_,
                            ReloadId_,
                            StatusId_,
                            PickupBy_,
                            Role_,
                            pareantOrderNo_,
                            DriverLic_,
                            ABN_,
                            Address_,
                            City_,
                            State_,
                            Postcode_,
                            Country_,
                            TradeType_,
                            SplitOrder_,
                            IP_,
                            Comment_,
                            needAdvice_,
                            needGetIsQuote_).ToList<decimal>()[0];
                        orderNo = data.ToString();

                    }

                    if (Session["HandledBy"] != null)
                    {
                        if (Session["HandledBy"].ToString() != "")
                        {
                            sql = "update dbo.TB_ORDER set UserId =" + Session["HandledBy"].ToString() + " where Id=" + orderNo;
                            using (var context = new DbContext(Global.ConnStr))
                            {
                                context.Database.ExecuteSqlCommand(sql);
                            }
                        }
                    }


                    if (Convert.ToInt32(orderNo) < 0)
                    {
                        if (Convert.ToInt32(orderNo) == -1)
                        {
                            Session["errorMessage"] = "Error:<br/>This Order (Track Id: " + Session["ReloadId"].ToString() + ") has been cancelled already.";
                        }
                        else
                        {
                            Session["errorMessage"] = "This Order seems submitted already,<br/> with Order NO: " + orderNo.Substring(1, orderNo.Length - 1) + ".<br/>If need to force to submit it again,<br/>please change the Referenece or PONum on the Check Out page, and submit it again.";
                        }

                        CheckAnddeleteUpload();
                        Session["HandledBy"] = null;
                        return Redirect($"{Url.RouteUrl(new { controller = "Order", action = "Index" })}");
                        //return View(((NewOrder)Session["newOrder"]).Head);
                    }


                    using (var context = new DbContext(Global.ConnStr))
                    {
                        foreach (NewOrderDetail orderDetail in order.OrderDetails)
                        {
                            if (!orderDetail.isNormal)
                            {
                                if ((orderDetail.ITEMTYPE == "SpecialOrder") &&
                                  (orderDetail.DESCRIPTION.IndexOf("MISC ITEM") > -1) &&
                                  (Session["uploadGuid"] != null))
                                {

                                    if (Session["UploadingNote"].ToString() != "")
                                    {
                                        orderDetail.Cuts = -1;
                                        orderDetail.CutDesc = Session["UploadingNote"].ToString();
                                    }

                                    if (Directory.Exists(string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + orderNo, Server.MapPath(@"\"))))
                                    {
                                        Directory.Delete(string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + orderNo, Server.MapPath(@"\")), true);
                                    }


                                    if (Directory.Exists(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\"))))
                                    {
                                        Directory.Move(string.Format("{0}UploadedTmp\\" + Session["uploadGuid"].ToString(), Server.MapPath(@"\")), string.Format("{0}GRAM_INTERNAL\\Uploaded\\" + orderNo, Server.MapPath(@"\")));
                                    }
                                }


                                object[] parameters = {
                                    orderNo,
                                    orderDetail.ITEMTYPE,
                                    orderDetail.COLOUR,
                                    orderDetail.STANDARD,
                                    orderDetail.STOCKCODE,
                                    orderDetail.DESCRIPTION,
                                    orderDetail.WEIGHT,
                                    orderDetail.PRICE,
                                    orderDetail.QTY,
                                    orderDetail.PQTY,
                                    orderDetail.DISCOUNTRATE,
                                    orderDetail.BASEPRICE,
                                    orderDetail.SUNDRYCOLOUR,
                                    orderDetail.isNormal,
                                    orderDetail.C_X,
                                    orderDetail.C_Y,
                                    orderDetail.Cuts,
                                    orderDetail.CutDesc,
                                    orderDetail.nextRelColorId,
                                    orderDetail.menuId

                                };


                                //confirm third proc_AddOrderDetail un normal and to be advice (TBA) items
                                context.Database.ExecuteSqlCommand("proc_AddOrderDetail_v1 {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}", parameters);
                            }
                        }
                    }

                    using (var context = new DbContext(Global.ConnStr))
                    {

                        object[] parameters = {
                            Convert.ToInt32(orderNo)
                        };

                        sql = "exec dbo.X_PROC_RECREATE_SALESORDER_v1 {0}";
                        var data = context.Database.SqlQuery<int>(sql,
                            parameters).ToList<int>()[0];
                        serverRefNo = data.ToString();
                    }

                    ((NewOrder)Session["newOrder"]).Head.PrintId2 = "Track Id: " + orderNo + ", Order Id: " + serverRefNo;
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        object[] parameters = {
                            orderNo,
                            ((User)Session["User"]).ACCNO.ToString(),
                            newOrderh.Email,
                            WebConfigurationManager.AppSettings["GramBccEmails"],
                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                            ""
                            };
                        context.Database.ExecuteSqlCommand("proc_SendOrderNotification_v1 {0},{1},{2},{3},{4},{5}", parameters);

                        if (newOrderh.Message != "")
                            newOrderh.Message += "<br/>";

                        newOrderh.Message += "<br/><br/>Your Custom Order Request has been placed Successfully. Your Reference No is: " + serverRefNo + " and email Notification has been sent. One of our representives will contact you as soon as possible.<br/>Thank you very much.<br/><br/>Do you want to print a copy?";
                    }
                }
            }
            catch (Exception e)
            {
                Session["HandledBy"] = null;
                using (var context = new DbContext(Global.ConnStr))
                {
                    string errBody = @"Company: " + newOrderh.Company + "<br>Order By: " + newOrderh.OrderBy + "<br>Mobile " + newOrderh.Mobile + "<br><br><br>" + e.Message.Replace(Environment.NewLine, "<br>");
                    if (((e.InnerException != null) && (!string.IsNullOrEmpty(e.InnerException.Message))))
                    {
                        errBody += @"<br><br><br>" + e.InnerException.Message.Replace("\n", "<br>");
                    }


                    object[] parameters = {
                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                            "Error: Order "+ orderNo+ " (Submitted)",
                            errBody,
                            "",
                            "",
                            ""
                        };
                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters);
                    newOrderh.Message = "Error:<br/>Your Order Request has been placed unsuccessfully.<br/>An error message has been sent to our IT department already.<br/>And we will get this issue fixed as soon as possible.<br/>Sorry for your inconvenience.";
                }
            }


            if (string.IsNullOrEmpty(((NewOrder)Session["newOrder"]).Head.SubmittedBy))
            {
                ((NewOrder)Session["newOrder"]).Head.SubmittedBy = newOrderh.OrderBy;
            }


            if (string.IsNullOrEmpty(((NewOrder)Session["newOrder"]).Head.SubmitDate))
            {
                ((NewOrder)Session["newOrder"]).Head.SubmitDate = currDateTime;
            }

            if (string.IsNullOrEmpty(((NewOrder)Session["newOrder"]).Head.HandleBy))
            {
                if (string.IsNullOrEmpty(newOrderh.UserName))
                {
                    ((NewOrder)Session["newOrder"]).Head.HandleBy = ((Scanner.Models.User)Session["User"]).FirstName + " " + ((Scanner.Models.User)Session["User"]).LastName;
                }
                else
                {
                    if (newOrderh.UserName.ToLower() != ((Scanner.Models.User)Session["User"]).UserName.ToLower())
                    {
                        ((NewOrder)Session["newOrder"]).Head.HandleBy = ((Scanner.Models.User)Session["User"]).FirstName + " " + ((Scanner.Models.User)Session["User"]).LastName;
                    }
                }
            }

            if (string.IsNullOrEmpty(((NewOrder)Session["newOrder"]).Head.ReadyToSubmitDate))
            {

                if (string.IsNullOrEmpty(newOrderh.UserName))
                {
                    ((NewOrder)Session["newOrder"]).Head.ReadyToSubmitDate = ((NewOrder)Session["newOrder"]).Head.SubmitDate;
                }
                else
                {
                    if (newOrderh.UserName.ToLower() != ((Scanner.Models.User)Session["User"]).UserName.ToLower())
                    {
                        ((NewOrder)Session["newOrder"]).Head.ReadyToSubmitDate = ((NewOrder)Session["newOrder"]).Head.OrderDate;
                    }
                    else
                    {
                        ((NewOrder)Session["newOrder"]).Head.ReadyToSubmitDate = ((NewOrder)Session["newOrder"]).Head.SubmitDate;
                    }
                }
            }

            Session["HandledBy"] = null;
            return View(((NewOrder)Session["newOrder"]).Head);
        }


        private void fillPrice(NewOrder order)
        {
            double totalWeight = 0;
            double totalPrice;
            totalPrice = 0;

            if (string.IsNullOrEmpty(order.Head.ACCNO)) order.Head.ACCNO = "438";

            foreach (var orderDetail in order.OrderDetails)
            {
                var STOCKCODE = new SqlParameter("@STOCKCODE", orderDetail.STOCKCODE);
                var ACCNO = new SqlParameter("@ACCNO", order.Head.ACCNO);
                var QTY = new SqlParameter("@QTY", orderDetail.QTY);
                var TRANSDATE = new SqlParameter("@TRANSDATE", DateTime.Now);
                var BEST_FLAG = new SqlParameter("@BEST_FLAG", 'Y');
                var SPECIAL_LEN = new SqlParameter("@SPECIAL_LEN", -1);

                if (orderDetail.STOCKCODE.Length > 5)
                {
                    if (orderDetail.STOCKCODE[4].ToString() + orderDetail.STOCKCODE[5].ToString() == "SL")
                    {
                        SPECIAL_LEN = new SqlParameter("@SPECIAL_LEN", orderDetail.LENGTH);
                    }
                }

                var JOBNO = new SqlParameter("@JOBNO", SqlDbType.Float);
                JOBNO.Direction = ParameterDirection.Output;

                var MASTER_JOBNO = new SqlParameter("@MASTER_JOBNO", SqlDbType.Float);
                MASTER_JOBNO.Direction = ParameterDirection.Output;

                var BEST_INT1 = new SqlParameter("@BEST_INT1", SqlDbType.Float);
                BEST_INT1.Direction = ParameterDirection.Output;

                var BEST_INT2 = new SqlParameter("@BEST_INT2", SqlDbType.Float);
                BEST_INT2.Direction = ParameterDirection.Output;

                var BEST_VAR1 = new SqlParameter("@BEST_VAR1", SqlDbType.VarChar, 50);
                BEST_VAR1.Direction = ParameterDirection.Output;

                var BEST_PRICE = new SqlParameter("@BEST_PRICE", SqlDbType.Float);
                BEST_PRICE.Direction = ParameterDirection.Output;

                var DISCOUNT_AMOUNT = new SqlParameter("@DISCOUNT_AMOUNT", SqlDbType.Float);
                DISCOUNT_AMOUNT.Direction = ParameterDirection.Output;

                var IS_SPECIAL_PRICE = new SqlParameter("@IS_SPECIAL_PRICE", SqlDbType.Char, 1);
                IS_SPECIAL_PRICE.Direction = ParameterDirection.Output;

                var POLICY_HDR = new SqlParameter("@POLICY_HDR", SqlDbType.Int);
                POLICY_HDR.Direction = ParameterDirection.Output;

                var FREIGHT_FREE = new SqlParameter("@FREIGHT_FREE", SqlDbType.Char, 1);
                FREIGHT_FREE.Direction = ParameterDirection.Output;

                var FIXEDPOLICY = new SqlParameter("@FIXEDPOLICY", SqlDbType.Char, 1);
                FIXEDPOLICY.Direction = ParameterDirection.Output;

                var DISCOUNTRATE = new SqlParameter("@DISCOUNTRATE", SqlDbType.Float);
                DISCOUNTRATE.Direction = ParameterDirection.Output;

                var BASEPRICE = new SqlParameter("@BASEPRICE", SqlDbType.Float);
                BASEPRICE.Direction = ParameterDirection.Output;
                var currentServer = WebConfigurationManager.AppSettings["SydneyServer"];

                var sql = "exec " + currentServer + ".dbo.X_BEST_PRICE_V1 " +
                    "@STOCKCODE," +
                    "@ACCNO,@QTY," +
                    "@TRANSDATE," +
                    "@BEST_FLAG," +
                    "@SPECIAL_LEN,@JOBNO," +
                    "@MASTER_JOBNO," +
                    "@BEST_INT1," +
                    "@BEST_INT2," +
                    "@BEST_VAR1," +
                    "@BEST_PRICE OUT," +
                    "@DISCOUNT_AMOUNT OUT," +
                    "@IS_SPECIAL_PRICE OUT," +
                    "@POLICY_HDR OUT," +
                    "@FREIGHT_FREE OUT," +
                    "@FIXEDPOLICY OUT," +
                    "@DISCOUNTRATE OUT," +
                    "@BASEPRICE OUT";

                using (var context = new DbContext(Global.ConnStr))
                {
                    var data = context.Database.SqlQuery<Price>(sql,
                        STOCKCODE,
                        ACCNO,
                        QTY,
                        TRANSDATE,
                        BEST_FLAG,
                        SPECIAL_LEN,
                        JOBNO,
                        MASTER_JOBNO,
                        BEST_INT1,
                        BEST_INT2,
                        BEST_VAR1,
                        BEST_PRICE,
                        DISCOUNT_AMOUNT,
                        IS_SPECIAL_PRICE,
                        POLICY_HDR,
                        FREIGHT_FREE,
                        FIXEDPOLICY,
                        DISCOUNTRATE,
                        BASEPRICE);

                    var item = data.Single<Price>();

                    if (item.DISCOUNT_AMOUNT == null)
                        item.DISCOUNT_AMOUNT = 0;

                    if (item.POLICY_HDR == null)
                    {
                        if (((Scanner.Models.User)Session["User"]).Role == "Trade Account")
                            item.POLICY_HDR = 119;
                        else
                            item.POLICY_HDR = 0;
                    }

                    if (string.IsNullOrEmpty(item.FIXEDPOLICY))
                        item.FIXEDPOLICY = "Y";

                    orderDetail.PRICE = item.BEST_PRICE.ToString();
                    orderDetail.DISCOUNTRATE = item.DISCOUNTRATE.ToString();
                    orderDetail.BASEPRICE = item.BASEPRICE.ToString();

                    totalPrice += item.BEST_PRICE * Convert.ToDouble(orderDetail.QTY);
                    totalWeight += Convert.ToDouble(orderDetail.WEIGHT) * Convert.ToDouble(orderDetail.QTY);
                }
            }

            order.Head.TotalWeight = Convert.ToDouble(totalWeight);
            order.Head.TotalPrice = totalPrice;
        }


        private void fillNewOrderDetails()
        {
            string currForm = Session["CurrForm"].ToString();
            //if (currForm == "EnterMeterage" || currForm == "EnterNoOfPanels") {

            //    if (Request.Form["sheetType"].ToString() == "")
            //    {
            //        currForm = "Category";
            //    }
            //    else
            //    {
            //        currForm = Request.Form["sheetType"].ToString();
            //    }
            //}

            //  DataTable GramLineFlags = (DataTable)Session["GramLineFlags"];
            //if (((Session[currForm] != null) && (((DataTable)Session[currForm]).Rows.Count > 0)))
            //{
            ((NewOrder)Session["newOrder"]).OrderDetails = CreateNewOrderDetail();
            StockDescription description = new StockDescription();

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    IList<NewOrderDetail> orderDetails = new List<NewOrderDetail>();
                    string roofLength = "";
                    decimal idx = 0;
                    foreach (var nOrderDetail in ((NewOrder)Session["newOrder"]).OrderDetails)
                    {
                        //  var nOrderDetail = Global.newOrder.OrderDetails[i];                            
                        if (string.IsNullOrEmpty(nOrderDetail.DESCRIPTION)) nOrderDetail.DESCRIPTION = "";
                        nOrderDetail.ACCNO = ((User)Session["User"]).ACCNO.ToString();
                        nOrderDetail.CutDesc = "";
                        nOrderDetail.Cuts = 0;
                        if (Session[nOrderDetail.ITEMTYPE + "AllCuts"] != null)
                        {
                            string[] itemCusts = Session[nOrderDetail.ITEMTYPE + "AllCuts"].ToString().Split(new string[] { "|_|" }, StringSplitOptions.None);
                            foreach (string itemCut in itemCusts)
                            {
                                if (itemCut != "")
                                {
                                    string[] itemCutDet = itemCut.Split(new string[] { "|~|" }, StringSplitOptions.None);
                                    if ((itemCutDet[0] == nOrderDetail.COLOUR) && (itemCutDet[1] == nOrderDetail.STANDARD))
                                    {
                                        nOrderDetail.Cuts = Convert.ToInt32(itemCutDet[2]);
                                        nOrderDetail.CutDesc = itemCutDet[3];
                                        break;
                                    }
                                }
                            }
                        }

                        nOrderDetail.Idx = idx;

                        if (nOrderDetail.ITEMTYPE == "SmartSlat")
                        {
                            if ((nOrderDetail.STANDARD.IndexOf("bq") > -1) || (nOrderDetail.STANDARD.IndexOf("cq") > -1))
                            {
                                nOrderDetail.LENGTH = nOrderDetail.STANDARD.Split(' ')[1].ToString();
                            }
                        }

                        if (nOrderDetail.ITEMTYPE == "Roofing")
                        {
                            if (roofLength == "")
                            {
                                roofLength = nOrderDetail.QTY;
                                continue;
                            }
                            else
                            {
                                nOrderDetail.LENGTH = roofLength;
                                roofLength = "";
                            }
                        }

                        NewOrderDetail nDr = new NewOrderDetail();
                        description = getItemDescirption(nOrderDetail.ITEMTYPE, nOrderDetail.LENGTH, nOrderDetail.STANDARD, nOrderDetail.COLOUR);

                        if (!string.IsNullOrEmpty(description.MFGQTY))
                        {
                            if (Convert.ToInt32(description.MFGQTY) > 0)
                            {
                                if (Convert.ToInt32(nOrderDetail.QTY) >= Convert.ToInt32(description.MFGQTY))
                                {
                                    nOrderDetail.isNormal = false;
                                }
                            }
                        }

                        if ((nOrderDetail.ITEMTYPE == "PoolFencing") || (nOrderDetail.ITEMTYPE == "SecFencing"))
                        {
                            if (description.Description.IndexOf("MM") > -1)
                            {
                                string[] strAr = description.Description.Split(new string[] { "MM" }, StringSplitOptions.None)[0].Split(' ');
                                nOrderDetail.LENGTH = strAr[strAr.Length - 1].Replace("X", "").Replace("x", "");
                            }
                        }

                        nOrderDetail.needAdvice = false;
                        if (description.Description.IndexOf("needs adv") > -1)
                        {
                            nOrderDetail.needAdvice = true;
                            nOrderDetail.isNormal = false;
                        }

                        if (description.Description.IndexOf("needs adv") > -1)
                        {
                            nOrderDetail.STOCKCODE = "MISC";
                            nOrderDetail.DESCRIPTION = description.Description;
                            nOrderDetail.ALERT = description.Alert;
                            orderDetails.Add(nOrderDetail);
                        }
                        else
                        {
                            nOrderDetail.DESCRIPTION = (description.Description.IndexOf("(BAR:") > -1) ? description.Description + " " + nOrderDetail.DESCRIPTION : description.Description;
                            nOrderDetail.ALERT = description.Alert;
                            nOrderDetail.STOCKCODE = description.ItemCode;

                            if ((nOrderDetail.ITEMTYPE == "Screws") && ((nOrderDetail.LENGTH == "B")
                                || (nOrderDetail.LENGTH == "D")
                                || (nOrderDetail.LENGTH == "F")))
                            {
                                if (nOrderDetail.LENGTH == "F")
                                {
                                    nOrderDetail.QTY = (Convert.ToInt32(nOrderDetail.QTY) * 500).ToString();
                                }
                                else
                                {
                                    nOrderDetail.QTY = (Convert.ToInt32(nOrderDetail.QTY) * 1000).ToString();
                                }
                            }

                            string originalQty = nOrderDetail.QTY;
                            nOrderDetail.WEIGHT = description.Weight;
                            nOrderDetail.PQTY = description.PQTY;
                            double rQty = Convert.ToDouble(originalQty) % Convert.ToDouble(nOrderDetail.PQTY);

                            if (rQty == 0)
                            {
                                nDr.ITEMTYPE = nOrderDetail.ITEMTYPE;
                                nDr.COLOUR = nOrderDetail.COLOUR;
                                nDr.STOCKCODE = nOrderDetail.STOCKCODE;
                                nDr.DESCRIPTION = nOrderDetail.DESCRIPTION;
                                nDr.SUNDRYCOLOUR = nOrderDetail.SUNDRYCOLOUR;
                                nDr.ALERT = nOrderDetail.ALERT;
                                nDr.LENGTH = nOrderDetail.LENGTH;
                                nDr.nextRelColorId = nOrderDetail.nextRelColorId;

                                if (nOrderDetail.ITEMTYPE != "SmartSlat")
                                    nDr.STANDARD = nOrderDetail.LENGTH;

                                if ((nOrderDetail.ITEMTYPE == "SmartSlatAng") ||
                                (nOrderDetail.ITEMTYPE == "SmartSlat") ||
                                (nOrderDetail.ITEMTYPE == "SmartSlatSpacer") ||
                                (nOrderDetail.ITEMTYPE == "SmartSlatLBSO")
                                )
                                {
                                    nDr.STANDARD = nOrderDetail.STANDARD;
                                }

                                if (nOrderDetail.ITEMTYPE == "Lattice")
                                {
                                    nDr.LENGTH = nOrderDetail.STANDARD.ToLower().Split('x')[1];
                                }

                                nDr.WEIGHT = description.Weight;
                                nDr.QTY = nOrderDetail.QTY;
                                nDr.PQTY = nOrderDetail.PQTY;

                                nDr.isNormal = nOrderDetail.isNormal;
                                nDr.needAdvice = nOrderDetail.needAdvice;
                                nDr.C_X = nOrderDetail.C_X;
                                nDr.C_Y = nOrderDetail.C_Y;
                                nDr.Idx = nOrderDetail.Idx;
                                nDr.Cuts = nOrderDetail.Cuts;
                                nDr.CutDesc = nOrderDetail.CutDesc;
                                nDr.menuId = nOrderDetail.menuId;
                                orderDetails.Add(nDr);
                            }
                            else
                            {
                                if (Convert.ToDouble(nOrderDetail.QTY) - rQty > 0)
                                {
                                    NewOrderDetail nDr2 = new NewOrderDetail();
                                    nDr2.ITEMTYPE = nOrderDetail.ITEMTYPE;
                                    nDr2.COLOUR = nOrderDetail.COLOUR;
                                    nDr2.STOCKCODE = nOrderDetail.STOCKCODE;
                                    nDr2.DESCRIPTION = nOrderDetail.DESCRIPTION;
                                    nDr2.SUNDRYCOLOUR = nOrderDetail.SUNDRYCOLOUR;
                                    nDr2.ALERT = nOrderDetail.ALERT;
                                    nDr2.LENGTH = nOrderDetail.LENGTH;
                                    nDr2.nextRelColorId = nOrderDetail.nextRelColorId;

                                    if (nOrderDetail.ITEMTYPE != "SmartSlat")
                                        nDr2.STANDARD = nOrderDetail.LENGTH;

                                    if ((nOrderDetail.ITEMTYPE == "SmartSlatAng") ||
                                    (nOrderDetail.ITEMTYPE == "SmartSlat") ||
                                    (nOrderDetail.ITEMTYPE == "SmartSlatSpacer") ||
                                    (nOrderDetail.ITEMTYPE == "SmartSlatLBSO"))
                                    {
                                        nDr2.STANDARD = nOrderDetail.STANDARD;
                                    }

                                    if (nOrderDetail.ITEMTYPE == "Lattice")
                                    {
                                        nDr2.LENGTH = nOrderDetail.STANDARD.ToLower().Split('x')[1];
                                    }

                                    nDr2.WEIGHT = description.Weight;
                                    nDr2.QTY = (Convert.ToDouble(nOrderDetail.QTY) - rQty).ToString();
                                    nDr2.PQTY = nOrderDetail.PQTY;
                                    nDr2.isNormal = nOrderDetail.isNormal;
                                    nDr2.needAdvice = nOrderDetail.needAdvice;
                                    nDr2.C_X = nOrderDetail.C_X;
                                    nDr2.C_Y = nOrderDetail.C_Y;
                                    nDr2.Idx = nOrderDetail.Idx;
                                    nDr2.Cuts = 0;
                                    nDr2.CutDesc = "";
                                    nDr2.menuId = nOrderDetail.menuId;
                                    orderDetails.Add(nDr2);
                                }

                                NewOrderDetail nDr3 = new NewOrderDetail();
                                nDr3.ITEMTYPE = nOrderDetail.ITEMTYPE;
                                nDr3.COLOUR = nOrderDetail.COLOUR;
                                nDr3.STOCKCODE = nOrderDetail.STOCKCODE;
                                nDr3.DESCRIPTION = nOrderDetail.DESCRIPTION;
                                nDr3.SUNDRYCOLOUR = nOrderDetail.SUNDRYCOLOUR;
                                nDr3.ALERT = nOrderDetail.ALERT;
                                nDr3.LENGTH = nOrderDetail.LENGTH;
                                nDr3.nextRelColorId = nOrderDetail.nextRelColorId;

                                if (nOrderDetail.ITEMTYPE != "SmartSlat")
                                    nDr3.STANDARD = nOrderDetail.LENGTH;

                                if ((nOrderDetail.ITEMTYPE == "SmartSlatAng")
                                || (nOrderDetail.ITEMTYPE == "SmartSlat") ||
                                (nOrderDetail.ITEMTYPE == "SmartSlatSpacer") ||
                                (nOrderDetail.ITEMTYPE == "SmartSlatLBSO"))
                                {
                                    nDr3.STANDARD = nOrderDetail.STANDARD;
                                }

                                if (nOrderDetail.ITEMTYPE == "Lattice")
                                {
                                    nDr3.LENGTH = nOrderDetail.STANDARD.ToLower().Split('x')[1];
                                }

                                if (Convert.ToInt32(nOrderDetail.QTY) > Convert.ToInt32(nOrderDetail.PQTY))
                                    nDr3.Idx = nOrderDetail.Idx + Convert.ToDecimal(0.1);
                                else
                                    nDr3.Idx = nOrderDetail.Idx;

                                nDr3.WEIGHT = description.Weight;
                                nDr3.QTY = rQty.ToString();
                                nDr3.PQTY = nOrderDetail.PQTY;
                                nDr3.isNormal = nOrderDetail.isNormal;
                                nDr3.needAdvice = nOrderDetail.needAdvice;
                                nDr3.C_X = nOrderDetail.C_X;
                                nDr3.C_Y = nOrderDetail.C_Y;
                                nDr3.Cuts = nOrderDetail.Cuts;
                                nDr3.CutDesc = nOrderDetail.CutDesc;
                                nDr3.menuId = nOrderDetail.menuId;
                                orderDetails.Add(nDr3);
                            }
                        }
                        idx++;
                    }
                    ((NewOrder)Session["newOrder"]).OrderDetails = orderDetails;
                }
            }
            catch (Exception e)
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    object[] parameters = {
                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                            "Error: Can not load item description "+currForm,
                            e.Message.Replace(Environment.NewLine,"<br>"),
                            "",
                            "",
                            ""
                        };
                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters);
                }
            }
            //}
            //else {
            //    ((NewOrder)Session["newOrder"]).OrderDetails = CreateNewOrderDetail();
            //}
        }

        private StockDescription getItemDescirption(string type, string length, string standard, string colour)
        {
            StockDescription description = new StockDescription();
            IList<StockDescription> descriptions = new List<StockDescription>();

            using (var context = new DbContext(Global.ConnStr))
            {
                var len = length;
                var stand = standard;

                if (type == "Lattice")
                {
                    len = length.Split('x')[0];
                    stand = length.Split('x')[1];
                }

                object[] parameters = { type, len, stand, colour };
                descriptions = context.Database.SqlQuery<StockDescription>("proc_GetItemDescription {0},{1},{2},{3}", parameters).ToList<StockDescription>();
            }

            if (descriptions.Count > 0)
            {
                description = descriptions[0];

                if ((type == "SpecialOrder") && colour.IndexOf("MISC ITEM") > -1)
                {
                    if (Session["uploadedFiles"] != null)
                    {
                        if (Session["uploadedFiles"].ToString() != "")
                        {
                            description.Description = description.Description + " (" + Session["uploadedFiles"].ToString() + ")";
                        }
                    }
                }
            }
            else
                description.Description = "Item " + type + " " + length + " " + colour + " needs advice.";

            return description;
        }



        public void setFromOrderForm2(string orderForm, string fromForm, string C_X, string C_Y, string val)
        {
            if (Session["FromFormClick"] == null)
            {
                Session["FromFormClick"] = new List<string>();
            }

            List<string> FromFormClick = (List<string>)Session["FromFormClick"];
            string[] tmpArr;
            if (FromFormClick.Count > 0)
            {
                foreach (string item in FromFormClick)
                {
                    tmpArr = item.Split(',');
                    if ((tmpArr[0] == orderForm) && (tmpArr[2] == C_X) && (tmpArr[3] == C_Y))
                    {
                        FromFormClick.Remove(item);
                        break;
                    }
                }
            }

            if (val != "")
            {
                FromFormClick.Add(orderForm + "," + fromForm + "," + C_X + "," + C_Y);
            }
        }


        [SessionExpire]
        [HttpPost]
        public ActionResult AddNewLength()
        {
            //Session["MenuExpandings"] = (Request.Form["MenuExpandings"] != null) ? Request.Form["MenuExpandings"].ToString() : "";
            Session["UseSessionMenuExpandings"] = 1;
            string length = Request.Form["newLength"].ToString();
            string colour = Request.Form["newColour"].ToString();
            string qty = Request.Form["newQty"].ToString();
            string currForm = Session["CurrForm"].ToString();
            bool exist = false;


            foreach (DataColumn col in ((DataTable)Session[currForm]).Columns)
            {
                if (col.ColumnName == length)
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                ((DataTable)Session[currForm]).Columns.Add(length);

                var rowsCnt = (Session[Request.Form["frmName"].ToString()] != null) ? ((DataTable)Session[Request.Form["frmName"].ToString()]).Rows.Count : 0;
                if ((Request.Form["rows"] != null) && (Request.Form["rows"].ToString() != ""))
                {
                    rowsCnt = Convert.ToInt32(Request.Form["rows"]);
                }

                fillCurrTable(Request.Form["frmName"].ToString(), rowsCnt);

                ((DataTable)Session[currForm + "Flags"]).Columns.Add(length);
                ((DataTable)Session[currForm + "Flags"]).Rows[0][length] = "FALSE";
                ((DataTable)Session[currForm + "Flags"]).Rows[1][length] = "Made to Order. Lead Time 10-15 Working Days.";

                for (int i = 0; i < ((DataTable)Session[currForm]).Rows.Count; i++)
                {
                    if (((DataTable)Session[currForm]).Rows[i][0].ToString() == colour)
                    {
                        ((DataTable)Session[currForm]).Rows[i][((DataTable)Session[currForm]).Columns.Count - 1] = qty;
                        setFromOrderForm2(currForm, currForm, i.ToString(), (((DataTable)Session[currForm]).Columns.Count - 1).ToString(), qty.ToString());
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < ((DataTable)Session[currForm]).Rows.Count; i++)
                {
                    if (((DataTable)Session[currForm]).Rows[i][0].ToString() == colour)
                    {
                        for (int j = 1; j < ((DataTable)Session[currForm]).Columns.Count; j++)
                        {
                            if (((DataTable)Session[currForm]).Columns[j].ColumnName == length)
                            {
                                ((DataTable)Session[currForm]).Rows[i][j] = qty;
                                setFromOrderForm2(currForm, currForm, i.ToString(), j.ToString(), qty.ToString());
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            return Redirect($"{Url.RouteUrl(new { controller = "Order", action = currForm })}");
        }

        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult AddMisItem(HttpPostedFileBase file)
        {

            string currForm = Session["CurrForm"].ToString();
            DataRow dr = ((DataTable)Session[currForm]).NewRow();
            dr["COLOUR"] = Request.Form["colours"].ToString();
            dr["ITEM"] = Request.Form["item"].ToString();
            //dr["FILE"] = file.p

            dr["FILE"] = Request.Form["fileUpload"].ToString();


            dr["QTY"] = Request.Form["qty"].ToString();
            ((DataTable)Session[currForm]).Rows.Add(dr);


            return Redirect($"{Url.RouteUrl(new { controller = "Order", action = currForm })}");
        }

        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult DelMisItem()
        {
            string currForm = Session["CurrForm"].ToString();
            int rowId = Convert.ToInt32(Request.Form["delRowNo"].ToString());
            ((DataTable)Session[currForm]).Rows.RemoveAt(rowId);
            return Redirect($"{Url.RouteUrl(new { controller = "Order", action = currForm })}");
        }

        public IList<NewOrderDetail> CreateNewOrderDetail()
        {
            IList<NewOrderDetail> nOrderDetails = new List<NewOrderDetail>();
            NewOrderDetail nOrderDetail;

            var forms = getforms();
            DataTable Flags;
            decimal idx = 0;
            foreach (var form in forms)
            {
                nOrderDetail = new NewOrderDetail();
                if ((Session[form] != null) && (((DataTable)Session[form]).Rows.Count > 0))
                {
                    Flags = (DataTable)Session[form + "Flags"];
                    switch (form)
                    {
                        case "SmartSlat":
                            for (int i = 0; i < ((DataTable)Session[form]).Rows.Count; i++)
                            {
                                for (int j = 1; j < ((DataTable)Session[form]).Columns.Count; j++)
                                {
                                    if (((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString() != "")
                                    {
                                        nOrderDetail = new NewOrderDetail();
                                        nOrderDetail.ITEMTYPE = form;
                                        nOrderDetail.LENGTH = ((DataTable)Session[form]).Columns[j].ColumnName;
                                        nOrderDetail.STANDARD = ((DataTable)Session[form]).Columns[j].ColumnName;
                                        nOrderDetail.COLOUR = ((DataTable)Session[form]).Rows[i]["COLOUR"].ToString();
                                        nOrderDetail.SUNDRYCOLOUR = "";
                                        nOrderDetail.QTY = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString();


                                        nOrderDetail.isNormal = Convert.ToBoolean(Flags.Rows[0][j]);


                                        if ((nOrderDetail.LENGTH.IndexOf("bq") > -1) || (nOrderDetail.LENGTH.IndexOf("cq") > -1))
                                        {
                                            nOrderDetail.STANDARD += " " + ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName.Replace("bq", "bs").Replace("cq", "cs")].ToString();
                                            j++;
                                        }

                                        nOrderDetail.Idx = idx;
                                        idx = idx + 1;

                                        nOrderDetail.C_Y = i;
                                        nOrderDetail.C_X = j;
                                        nOrderDetail.nextRelColorId = "-1";
                                        AddMenuId(ref nOrderDetail, form);
                                        nOrderDetails.Add(nOrderDetail);
                                    }
                                }
                            }
                            break;
                        case "SmartSlatAng":
                            for (int i = 0; i < ((DataTable)Session[form]).Rows.Count; i++)
                            {
                                for (int j = 1; j < ((DataTable)Session[form]).Columns.Count; j++)
                                {
                                    if (((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString() != "")
                                    {
                                        nOrderDetail = new NewOrderDetail();
                                        nOrderDetail.ITEMTYPE = form;
                                        nOrderDetail.LENGTH = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j + 1].ColumnName].ToString();
                                        nOrderDetail.STANDARD = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j + 1].ColumnName].ToString() + "|" + ((DataTable)Session[form]).Columns[j].ColumnName.Replace(Environment.NewLine + "QTY", "").Replace("ANGLE ", "").Replace("FLAT ", "");
                                        nOrderDetail.COLOUR = ((DataTable)Session[form]).Rows[i]["COLOUR"].ToString();
                                        nOrderDetail.SUNDRYCOLOUR = "";
                                        nOrderDetail.QTY = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString();
                                        nOrderDetail.isNormal = Convert.ToBoolean(Flags.Rows[0][j]);
                                        j++;
                                        nOrderDetail.Idx = idx;
                                        idx = idx + 1;
                                        nOrderDetail.C_Y = i;
                                        nOrderDetail.C_X = j;
                                        nOrderDetail.nextRelColorId = "-1";
                                        AddMenuId(ref nOrderDetail, form);
                                        nOrderDetails.Add(nOrderDetail);
                                    }
                                }
                            }
                            break;
                        case "SmartSlatLBSO":
                            for (int i = 0; i < ((DataTable)Session[form]).Rows.Count; i++)
                            {
                                for (int j = 1; j < ((DataTable)Session[form]).Columns.Count; j++)
                                {
                                    if (((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString() != "")
                                    {
                                        nOrderDetail = new NewOrderDetail();
                                        nOrderDetail.ITEMTYPE = form;
                                        nOrderDetail.LENGTH = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j + 1].ColumnName].ToString();
                                        nOrderDetail.STANDARD = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j + 1].ColumnName].ToString() + "|" + ((DataTable)Session[form]).Columns[j].ColumnName.Replace(Environment.NewLine + "QTY", "").Replace("ANGLE ", "").Replace("FLAT ", "");
                                        nOrderDetail.COLOUR = ((DataTable)Session[form]).Rows[i]["COLOUR"].ToString();
                                        nOrderDetail.SUNDRYCOLOUR = "";
                                        nOrderDetail.QTY = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString();
                                        nOrderDetail.isNormal = Convert.ToBoolean(Flags.Rows[0][j]);
                                        j++;
                                        nOrderDetail.Idx = idx;
                                        idx = idx + 1;
                                        nOrderDetail.C_Y = i;
                                        nOrderDetail.C_X = j;
                                        nOrderDetail.nextRelColorId = "-1";
                                        AddMenuId(ref nOrderDetail, form);
                                        nOrderDetails.Add(nOrderDetail);
                                    }
                                }
                            }
                            break;
                        case "Fasteners":
                            for (int i = 0; i < ((DataTable)Session[form]).Rows.Count; i++)
                            {
                                for (int j = 2; j < ((DataTable)Session[form]).Columns.Count; j++)
                                {
                                    if (((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString() != "")
                                    {
                                        string length = ((DataTable)Session[form]).Columns[j].ColumnName;

                                        nOrderDetail = new NewOrderDetail();
                                        nOrderDetail.ITEMTYPE = form;
                                        nOrderDetail.LENGTH = length;
                                        nOrderDetail.STANDARD = ((DataTable)Session[form]).Columns[j].ColumnName;
                                        nOrderDetail.COLOUR = ((DataTable)Session[form]).Rows[i]["COLOUR"].ToString();
                                        nOrderDetail.SUNDRYCOLOUR = "";
                                        if (((DataTable)Session[form]).Rows[i][1].ToString() != "")
                                        {
                                            nOrderDetail.SUNDRYCOLOUR = ((DataTable)Session[form]).Rows[i][1].ToString();
                                        }
                                        nOrderDetail.QTY = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString();
                                        nOrderDetail.isNormal = Convert.ToBoolean(Flags.Rows[0][j]);
                                        nOrderDetail.Idx = idx;
                                        idx = idx + 1;
                                        nOrderDetail.C_Y = i;
                                        nOrderDetail.C_X = j;
                                        nOrderDetail.nextRelColorId = "-1";
                                        AddMenuId(ref nOrderDetail, form);
                                        nOrderDetails.Add(nOrderDetail);
                                    }
                                }
                            }
                            break;
                        case "GramSlat":
                            string[] guideColorStrArr = new string[] {
                                "0,BIRCH,BIR",
                                "1,BLACK,BLA",
                                "2,COKE,COK",
                                "3,GREY,GGR",
                                "4,MERINO,MER",
                                "5,MIST GREEN,MGR",
                                "6,PRIMROSE,PRI",
                                "7,SLATE GREY,SGR"
                            };

                            GuideColors guideColors = new GuideColors();
                            guideColors.guideColors = new List<GuideColor>();

                            foreach (var s in guideColorStrArr)
                            {
                                GuideColor gc = new GuideColor();
                                string[] arr = s.Split(',');
                                gc.Id = Convert.ToInt32(arr[0]);
                                gc.Colour = arr[1];
                                gc.Code = arr[2];
                                guideColors.guideColors.Add(gc);
                            }

                            for (int i = 0; i < ((DataTable)Session[form]).Rows.Count; i++)
                            {
                                for (int j = 1; j < ((DataTable)Session[form]).Columns.Count; j++)
                                {
                                    var val = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString();
                                    if (j % 2 == 1)
                                    {
                                        if (val != "")
                                        {
                                            nOrderDetail = new NewOrderDetail();
                                            string length = ((DataTable)Session[form]).Columns[j].ColumnName;
                                            nOrderDetail.ITEMTYPE = form;
                                            nOrderDetail.LENGTH = length;
                                            nOrderDetail.STANDARD = ((DataTable)Session[form]).Columns[j].ColumnName;
                                            nOrderDetail.COLOUR = ((DataTable)Session[form]).Rows[i]["COLOUR"].ToString();
                                            nOrderDetail.SUNDRYCOLOUR = "";
                                            nOrderDetail.QTY = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString();
                                            nOrderDetail.isNormal = Convert.ToBoolean(Flags.Rows[0][j]);
                                            nOrderDetail.Idx = idx;

                                            idx = idx + 1;
                                            nOrderDetail.C_Y = i;
                                            nOrderDetail.C_X = j;
                                            nOrderDetail.nextRelColorId = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j + 1].ColumnName].ToString();
                                            AddMenuId(ref nOrderDetail, form);
                                            nOrderDetails.Add(nOrderDetail);
                                        }
                                    }

                                    if (j % 2 == 0)
                                    {
                                        if (val != "")
                                        {
                                            var barColor = "";

                                            foreach (var c in guideColors.guideColors)
                                            {
                                                if (c.Id.ToString() == val)
                                                {
                                                    barColor = c.Colour;
                                                    break;
                                                }
                                            }

                                            nOrderDetails[nOrderDetails.Count - 1].DESCRIPTION = "(BAR: " + barColor + ")";
                                        }
                                    }
                                }
                            }

                            break;
                        default:
                            for (int i = 0; i < ((DataTable)Session[form]).Rows.Count; i++)
                            {
                                for (int j = 1; j < ((DataTable)Session[form]).Columns.Count; j++)
                                {
                                    if (((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString() != "")
                                    {
                                        nOrderDetail = new NewOrderDetail();
                                        string length = ((DataTable)Session[form]).Columns[j].ColumnName;
                                        nOrderDetail.ITEMTYPE = form;
                                        nOrderDetail.LENGTH = length;
                                        nOrderDetail.STANDARD = ((DataTable)Session[form]).Columns[j].ColumnName;
                                        nOrderDetail.COLOUR = ((DataTable)Session[form]).Rows[i]["COLOUR"].ToString();
                                        nOrderDetail.SUNDRYCOLOUR = "";
                                        nOrderDetail.QTY = ((DataTable)Session[form]).Rows[i][((DataTable)Session[form]).Columns[j].ColumnName].ToString();
                                        nOrderDetail.isNormal = Convert.ToBoolean(Flags.Rows[0][j]);

                                        if ((form == "GramLine" || form == "ColorLine") && (length == "1190") && (Convert.ToInt32(nOrderDetail.QTY) <= 20))
                                        {
                                            nOrderDetail.isNormal = true;
                                        }

                                        nOrderDetail.Idx = idx;
                                        idx = idx + 1;
                                        nOrderDetail.C_Y = i;
                                        nOrderDetail.C_X = j;
                                        nOrderDetail.nextRelColorId = "-1";
                                        AddMenuId(ref nOrderDetail, form);
                                        nOrderDetails.Add(nOrderDetail);


                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return nOrderDetails;
        }


        private void AddMenuId(ref NewOrderDetail nOrderDetail, string form)
        {
            string[] strArr;
            string lv1;
            string lv2;
            string lv3;
            if ((Session["FromFormClick"] != null) && (Session["SideMenu"] != null) && (nOrderDetail != null))
            {
                foreach (string item in (List<string>)Session["FromFormClick"])
                {
                    lv1 = "";
                    lv2 = "";
                    lv3 = "";
                    strArr = item.Split(',');
                    if ((strArr[0] == form) &&
                        (nOrderDetail.C_X.ToString() == strArr[3]) &&
                        (nOrderDetail.C_Y.ToString() == strArr[2]))
                    {
                        foreach (SideMenu menuItem in (List<SideMenu>)Session["SideMenu"])
                        {
                            nOrderDetail.menuId = "-1";
                            if (menuItem.lv1.IndexOf("!") > -1)
                            {
                                lv1 = menuItem.lv1.Split('!')[1];
                            }

                            if (menuItem.lv2.IndexOf("!") > -1)
                            {
                                lv2 = menuItem.lv2.Split('!')[1];
                            }

                            if (menuItem.lv3.IndexOf("!") > -1)
                            {
                                lv3 = menuItem.lv3.Split('!')[1];
                            }

                            if ((lv1 == strArr[1]) ||
                                (lv2 == strArr[1]) || (lv3 == strArr[1]))
                            {

                                nOrderDetail.menuId = menuItem.Id.ToString();
                                break;
                            }
                        }
                    }
                }
            }
        }

        public string[] getforms()
        {
            string[] forms = new string[] {
                "GramLine",
                "ColorLine",
                "RailsPosts",
                "ExtChannelPost",
                "ExtSmartPost",
                "Plinths",
                "Lattice",
                "LatticeEdgeCover",
                "PDLattice",
                "GramSlat",
                "SmartSlat",
                "SmartSlatAng",
                "AlumPostBall",
                "CapsMetal",
                "CapsPlastic",
                "Cement",
                "Downee",
                "Fasteners",
                "Hinges",
                "Inserts",
                "Locks",
                "PoolFencing",
                "SecFencing",
                "SpecialOrder",
                "Miscellaneous",
                "Paint",
                "PlasticCaps",
                "Roofing",
                "Screws",
                "SprayPaint",
                "SheetEdgeCover",
                "SlidingGate",
                "SmartStile",
                "SmartStileDP",
                "SmartStileIncLDP300",
                "SmartStileIncLDP450",
                "SmartStileIncLDP600",
                "SmartStileIncLSP300",
                "SmartStileIncLSP450",
                "SmartStileIncLSP600",
                "SmartSlatLBSO",
                "SmartSlatITRF",
                "SmartSlatSpacer",
                "SquarePosts",
                "StainlessSteelFittings",
                "Tools",
                "WallBracket",
                "WeldedGateStile",
                "WeldedGateStileL300",
                "WeldedGateStileL450",
                "WeldedGateStileL600",
                "ZincFittings"
            };
            return forms;


        }

        [SessionExpire]
        [Authorize]
        public ActionResult Option_1()
        {
            ViewBag.Title = "Enter Meterage";
            Session["CurrForm"] = "Option_1";
            IList<string> colours = new List<string>();
            IList<string> heights = new List<string>();
            IList<string> chHeights = new List<string>();
            IList<string> smHeights = new List<string>();

            var sql = "select Colour from [dbo].[TB_COLOUR] where ItemTypeId = 1 order by pos";
            var sql2 = "select Standard + case when IsNorm = 0 then ' *' else '' end as Standard from [dbo].[TB_STANDARD] where ItemTypeId = 1 order by pos";
            var sql3 = "select ActStandard + case when IsNorm = 0 then ' *' else '' end as ActStandard from [dbo].[TB_STANDARD] where ItemTypeId = 10 and Standard like '%CHANNEL POST%' order by pos";
            var sql4 = "select ActStandard + case when IsNorm = 0 then ' *' else '' end as ActStandard from [dbo].[TB_STANDARD] where ItemTypeId = 10 and Standard like '%SMART POST%' order by pos";
            using (var context = new DbContext(Global.ConnStr))
            {
                colours = context.Database.SqlQuery<string>(sql).ToList<string>();
                heights = context.Database.SqlQuery<string>(sql2).ToList<string>();
                chHeights = context.Database.SqlQuery<string>(sql3).ToList<string>();
                smHeights = context.Database.SqlQuery<string>(sql4).ToList<string>();
            }

            //fillCurrTable(Request.Form["frmName"].ToString(), -1);

            ViewBag.Colours = colours;
            ViewBag.Heights = heights;
            ViewBag.chHeights = chHeights;
            ViewBag.smHeights = smHeights;

            updateMenuClickCount(Session["CurrForm"].ToString());
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Option_2()
        {
            ViewBag.Title = "Option_2";
            Session["CurrForm"] = "Option_2";
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Option_3()
        {
            ViewBag.Title = "Option_3";
            Session["CurrForm"] = "Option_3";
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Option_4()
        {
            ViewBag.Title = "Option_4";
            Session["CurrForm"] = "Option_4";
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Option_5()
        {
            ViewBag.Title = "Option_5";
            Session["CurrForm"] = "Option_5";
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Option_6()
        {
            ViewBag.Title = "Option_6";
            Session["CurrForm"] = "Option_6";
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Option_7()
        {
            ViewBag.Title = "Option_7";
            Session["CurrForm"] = "Option_7";
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Option_8()
        {
            ViewBag.Title = "Option_8";
            Session["CurrForm"] = "Option_8";
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Option_9()
        {
            ViewBag.Title = "Option_9";
            Session["CurrForm"] = "Option_9";
            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Option_10_1()
        {
            ViewBag.Title = "Option_10_1";
            Session["CurrForm"] = "Option_10_1";
            return View();
        }
        [SessionExpire]
        [Authorize]
        public ActionResult Option_10_2()
        {
            ViewBag.Title = "Option_10_2";
            Session["CurrForm"] = "Option_10_2";
            return View();
        }
        [SessionExpire]
        [Authorize]
        public ActionResult Option_10_3()
        {
            ViewBag.Title = "Option_10_3";
            Session["CurrForm"] = "Option_10_3";
            return View();
        }

    }
}