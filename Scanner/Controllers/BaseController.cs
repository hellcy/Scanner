using System.Web.Mvc;
using Scanner.Models;
using System.Data;
using System;
using System.Data.Entity;
using System.Web.Configuration;
using System.Linq;
using System.Collections.Generic;

namespace Scanner.Controllers
{
    public class BaseController : Controller
    {
        public void loadNewTable(string currForm)
        {
            try
            {
                if (Session[currForm] == null)
                {
                    Session[currForm] = new DataTable();
                    Session[currForm + "Flags"] = new DataTable();
                    Session["fence"] = new Fence();

                    using (var context = new DbContext(Global.ConnStr))
                    {
                        object[] parameters = { currForm };
                        ((Fence)Session["fence"]).colours = context.Database.SqlQuery<string>("proc_GetColours {0}", parameters).ToList<string>();
                        ((Fence)Session["fence"]).standards = context.Database.SqlQuery<string>("proc_GetStandards {0}", parameters).ToList<string>();
                        ((Fence)Session["fence"]).isNorms = context.Database.SqlQuery<bool>("proc_GetIsNorms {0}", parameters).ToList<bool>();
                        ((Fence)Session["fence"]).Messages = context.Database.SqlQuery<string>("proc_GetMessages {0}", parameters).ToList<string>();
                        Session["forceBlock"] = context.Database.SqlQuery<string>("proc_GetForceBlock {0}", parameters).ToList<string>()[0];
                    }

                    ((Fence)Session["fence"]).standards.Insert(0, "COLOUR");
                    foreach (var heading in ((Fence)Session["fence"]).standards)
                    {
                        ((DataTable)Session[currForm]).Columns.Add(heading);
                        ((DataTable)Session[currForm + "Flags"]).Columns.Add(heading);
                    }

                    if (currForm != "Miscellaneous")
                    {
                        DataRow dr;
                        dr = ((DataTable)Session[currForm + "Flags"]).NewRow();
                        dr[0] = "";

                        int ni = 1;
                        foreach (var isNorm in ((Fence)Session["fence"]).isNorms)
                        {
                            dr[ni] = isNorm;
                            ni++;
                        }
                        ((DataTable)Session[currForm + "Flags"]).Rows.Add(dr);


                        dr = ((DataTable)Session[currForm + "Flags"]).NewRow();
                        dr[0] = "";

                        ni = 1;
                        foreach (var Message in ((Fence)Session["fence"]).Messages)
                        {
                            dr[ni] = Message;
                            ni++;
                        }

                        ((DataTable)Session[currForm + "Flags"]).Rows.Add(dr);

                        foreach (var colour in ((Fence)Session["fence"]).colours)
                        {
                            dr = ((DataTable)Session[currForm]).NewRow();
                            dr[0] = colour;
                            for (int i = 1; i < ((DataTable)Session[currForm]).Columns.Count; i++)
                            {
                                dr[i] = "";
                            }
                            ((DataTable)Session[currForm]).Rows.Add(dr);
                        }
                    }
                }
                else
                {
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        object[] parameters = { currForm };
                        Session["forceBlock"] = context.Database.SqlQuery<string>("proc_GetForceBlock {0}", parameters).ToList<string>()[0];
                    }
                }
            }
            catch (Exception e)
            {
                Session["fence"] = null;
                using (var context = new DbContext(Global.ConnStr))
                {
                    object[] parameters = {
                            WebConfigurationManager.AppSettings["GramAdminEmails"],
                            "Error: Can not load table"+currForm,
                            e.Message.Replace(Environment.NewLine,"<br>"),
                            "",
                            "",
                            ""
                        };
                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters);
                }
            }
        }

        public void setFromOrderForm(string orderForm, string fromForm, string C_X, string C_Y, string val)
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


        public void fillCurrTable(string currForm, int rowsCnt)
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

            if (Request.Form["uploadedFiles"] != null)
            {
                string nStr = "";
                if (Request.Form["uploadedFiles"].ToString() != "")
                {
                    string[] filesArr = Request.Form["uploadedFiles"].ToString().Split(';');
                    foreach (var s in filesArr)
                    {
                        if (s != "")
                        {
                            nStr += s.Split(',')[0] + ",";
                        }
                    }
                    Session["uploadedFiles"] = nStr.Substring(0, nStr.Length - 1);
                }

                if (Request.Form["UploadingNote"] != null)
                {
                    Session["UploadingNote"] = Request.Form["UploadingNote"].ToString().TrimStart().TrimEnd();
                }
            }


            if ((currForm != "CheckOut") &&
                (currForm != "Confirmation") &&
                (currForm != "OrderTrack") &&
                (currForm != "EnterMeterage") &&
                (currForm != "EnterNoOfPanels") &&
                (currForm != "Contact") &&
                (currForm != "Career") &&
                (currForm != "Branches") &&
                (currForm != "Category") &&
                (currForm != "Registration") &&
                (currForm != "PDFQuotation") &&
                (currForm.Substring(0, 2) != "C-"))
            {
                if (currForm != "SameColourItemOrdering")
                {

                    for (int i = 0; i < rowsCnt; i++)
                    {
                        if (i == ((DataTable)Session[currForm]).Rows.Count) //some addition rows added, and row count is more than original rows
                        {
                            DataRow ndr = ((DataTable)Session[currForm]).NewRow();
                            ((DataTable)Session[currForm]).Rows.Add(ndr);
                        }

                        for (int j = 0; j < ((DataTable)Session[currForm]).Columns.Count; j++)
                        {
                            ((DataTable)Session[currForm]).Rows[i][j] = Request.Form["cR" + i.ToString() + "C" + j.ToString()];

                            if (j > 0)
                            {
                                try
                                {
                                    setFromOrderForm(currForm, currForm, i.ToString(), j.ToString(), Request.Form["cR" + i.ToString() + "C" + j.ToString()].ToString());
                                }
                                catch (Exception e)
                                {
                                    setFromOrderForm(currForm, currForm, i.ToString(), j.ToString(), "");
                                }
                            }
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
                else
                { //SameColour
                    string[] forms = new string[] {
                        "GramLine",
                        "ColorLine",
                        "RailsPosts",
                        "Plinths",
                        "ExtChannelPost",
                        "SheetEdgeCover",
                        "Lattice",
                        "LatticeEdgeCover",
                        "GramSlat",
                        "PlasticCaps",
                        "AlumPostBall",
                        "PDLattice",
                        "WeldedGateStile",
                        "SmartStile",
                        "SmartStileDP",
                        "SquarePosts",
                        "Screws"
                    };

                    foreach (string form in forms)
                    {
                        for (int i = 0; i < ((DataTable)Session[form]).Rows.Count; i++)
                        {
                            if (((DataTable)Session[form]).Rows[i][0].ToString() == Request.Form[form + "_cR-1C0"])
                            {
                                for (int j = 0; j < ((DataTable)Session[form]).Columns.Count; j++)
                                {
                                    ((DataTable)Session[form]).Rows[i][j] = Request.Form[form + "_cR-1C" + j.ToString()];
                                    if (j > 0)
                                    {
                                        setFromOrderForm(form, "SameColourItemOrdering", i.ToString(), j.ToString(), Request.Form[form + "_cR-1C" + j.ToString()].ToString());
                                    }
                                }
                            }
                        }
                    }

                    if (Request.Form["allCuts"] != null)
                    {
                        if (Request.Form["allCuts"].ToString() != "")
                        {
                            Session["LatticeAllCuts"] = Request.Form["allCuts"].ToString();
                        }
                        else
                        {
                            Session["LatticeAllCuts"] = null;
                        }
                    }
                }

            }

            if ((currForm == "EnterMeterage") || (currForm == "EnterNoOfPanels"))
            {
                var sheetType = Request.Form["sheetType"].ToString();
                if (sheetType != "")
                {
                    var panelColour = Request.Form["panelColour"].ToString();
                    var frameColour = Request.Form["frameColour"].ToString();
                    var panelHeight = Request.Form["panelHeight"].ToString();
                    var postType = Request.Form["postType"].ToString();
                    var sheets = Request.Form["sheets"].ToString();
                    var posts = Request.Form["posts"].ToString();
                    var rail2370 = Request.Form["rail2370"].ToString();
                    var rail3100 = Request.Form["rail3100"].ToString();
                    var screws = Request.Form["screws"].ToString();

                    loadNewTable(sheetType);
                    loadNewTable("RailsPosts");
                    loadNewTable("Screws");

                    for (int i = 0; i < ((DataTable)Session[sheetType]).Rows.Count; i++)
                    {
                        if (panelColour == ((DataTable)Session[sheetType]).Rows[i][0].ToString())
                        {
                            for (int j = 0; j < ((DataTable)Session[sheetType]).Columns.Count; j++)
                            {
                                if (((DataTable)Session[sheetType]).Columns[j].ColumnName == panelHeight)
                                {
                                    var val = ((DataTable)Session[sheetType]).Rows[i][j].ToString();

                                    ((DataTable)Session[sheetType]).Rows[i][j] = (val == "") ? sheets :
                                     (Convert.ToInt32(val) + Convert.ToInt32(sheets)).ToString();


                                    if (j > 0)
                                    {
                                        setFromOrderForm(sheetType, currForm, i.ToString(), j.ToString(), ((DataTable)Session[sheetType]).Rows[i][j].ToString());
                                    }

                                }
                            }
                        }
                    }

                    var pHeight = (postType == "CHANNEL POST") ? Request.Form["chHeight"].ToString() : Request.Form["smHeight"].ToString();
                    pHeight = postType + " " + pHeight;

                    for (int i = 0; i < ((DataTable)Session["RailsPosts"]).Rows.Count; i++)
                    {
                        if (frameColour == ((DataTable)Session["RailsPosts"]).Rows[i][0].ToString())
                        {
                            for (int j = 0; j < ((DataTable)Session["RailsPosts"]).Columns.Count; j++)
                            {
                                var val = ((DataTable)Session["RailsPosts"]).Rows[i][j].ToString();

                                if ((((DataTable)Session["RailsPosts"]).Columns[j].ColumnName == "RAIL 2370")
                                    && (rail2370 != ""))
                                {
                                    ((DataTable)Session["RailsPosts"]).Rows[i][j] = (val == "") ? rail2370 :
                                     (Convert.ToInt32(val) + Convert.ToInt32(rail2370)).ToString();

                                    if (j > 0)
                                    {
                                        setFromOrderForm("RailsPosts", currForm, i.ToString(), j.ToString(), ((DataTable)Session["RailsPosts"]).Rows[i][j].ToString());
                                    }

                                }

                                if ((((DataTable)Session["RailsPosts"]).Columns[j].ColumnName == "RAIL 3100")
                                   && (rail3100 != ""))
                                {
                                    ((DataTable)Session["RailsPosts"]).Rows[i][j] = (val == "") ? rail3100 :
                                     (Convert.ToInt32(val) + Convert.ToInt32(rail3100)).ToString();

                                    if (j > 0)
                                    {
                                        setFromOrderForm("RailsPosts", currForm, i.ToString(), j.ToString(), ((DataTable)Session["RailsPosts"]).Rows[i][j].ToString());
                                    }

                                }

                                if (((DataTable)Session["RailsPosts"]).Columns[j].ColumnName == pHeight)
                                {


                                    ((DataTable)Session["RailsPosts"]).Rows[i][j] = (val == "") ? posts :
                                     (Convert.ToInt32(val) + Convert.ToInt32(posts)).ToString();

                                    if (j > 0)
                                    {
                                        setFromOrderForm("RailsPosts", currForm, i.ToString(), j.ToString(), ((DataTable)Session["RailsPosts"]).Rows[i][j].ToString());
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 0; i < ((DataTable)Session["Screws"]).Rows.Count; i++)
                    {
                        if (frameColour == "GALVANISED")
                        {
                            frameColour = "UNPAINTED";
                        }

                        if (frameColour == ((DataTable)Session["Screws"]).Rows[i][0].ToString())
                        {
                            var val = ((DataTable)Session["Screws"]).Rows[i]["C"].ToString();

                            if (((DataTable)Session["Screws"]).Rows[i]["D"].ToString() != "")
                            {
                                val = (Convert.ToInt32(val) + Convert.ToInt32(((DataTable)Session["Screws"]).Rows[i]["D"].ToString()) * 1000).ToString();
                            }

                            if (val != "")
                            {
                                screws = (Convert.ToInt32(val) + Convert.ToInt32(screws)).ToString();
                            }

                            if (Convert.ToInt32(screws) > 999)
                            {
                                ((DataTable)Session["Screws"]).Rows[i]["C"] = screws.Substring(1, 3).Replace("0", " ").Replace(" ", "0");
                                setFromOrderForm("Screws", currForm, i.ToString(), "3", ((DataTable)Session["Screws"]).Rows[i][3].ToString());
                                ((DataTable)Session["Screws"]).Rows[i]["D"] = screws.Substring(0, 1);
                                setFromOrderForm("Screws", currForm, i.ToString(), "4", ((DataTable)Session["Screws"]).Rows[i][4].ToString());
                            }
                            else
                            {
                                ((DataTable)Session["Screws"]).Rows[i]["C"] = screws;
                                setFromOrderForm("Screws", currForm, i.ToString(), "3", ((DataTable)Session["Screws"]).Rows[i][3].ToString());
                            }
                        }
                    }
                }
            }
        }

    }
}