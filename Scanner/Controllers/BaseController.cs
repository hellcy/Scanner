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
                (currForm != "WorkOrderHeaders") &&
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