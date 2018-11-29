using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using Scanner.Models;
using System.Text;
using System.Data.Entity;
using static Scanner.FilterConfig;
using System.Data;
using Zen.Barcode;
using System.Drawing;
using System.IO;

namespace Scanner.Controllers
{
    public class ScannerController : BaseController
    {

        CoilDetail details = new CoilDetail();

        // change the lengths here for different products!
        public const int CoilQRcodeLength = 33;
        public const int CoilIDLength = 9;

        // GET: /Scanner/ 

        [SessionExpire]
        [Authorize]
        public ActionResult Index()
        {

            return View();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult WorkOrderLines()
        {
            WorkOrder_Lines lines = new WorkOrder_Lines();
            return View(lines);
        }

        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult WorkOrderLines(WorkOrder_Lines lines)
        {
            ViewBag.Title = "Work Order Lines";
            Session["CurrForm"] = "WorkOrderLines";

               // < img src = "@Url.Barcode("123456", BarcodeSymbology.Code128, 30, 1, true)" />

            //BarcodeSymbology.Code39C
            //barcode.Symbology = KeepAutomation.Barcode.Symbology.Code39;
            //barcode.CodeToEncode = "'" + lines.workOrder_HDR.BILLCODE + "'";
            //barcode.generateBarcodeToImageFile("C:/Scanner/code39.png");

            string SeqNo = Request.QueryString["ID"];

            var sql = "select * from GRAM_SYD_LIVE.dbo.WORKSORD_HDR where SEQNO = '" + SeqNo + "'"; // to get all information of the SeqNo of a workOrderHeader
            var sql_2 = "select * from GRAM_SYD_LIVE.dbo.WORKSORD_LINES where HDR_SEQNO = '" + SeqNo + "'"; // to get all workOrderLines under the header SeqNo

            string new_TRANSDATE = "";
            string new_PRODDATE = "";
            string new_DUEDATE = "";
            string new_EXPIRY_DATE = "";
            string new_X_COMPLETION_DATE = "";
            string[] tmpArr;

            if (lines.workOrder_HDR.TRANSDATE != null) {
                tmpArr = lines.workOrder_HDR.TRANSDATE.ToString().Split(' ');
                new_TRANSDATE = tmpArr[0].Split('/')[2] + "/" + tmpArr[0].Split('/')[1] + "/" + tmpArr[0].Split('/')[0] + " " + tmpArr[1];
            }
            if (lines.workOrder_HDR.PRODDATE != null)
            {
                tmpArr = lines.workOrder_HDR.TRANSDATE.ToString().Split(' ');
                new_PRODDATE = tmpArr[0].Split('/')[2] + "/" + tmpArr[0].Split('/')[1] + "/" + tmpArr[0].Split('/')[0] + " " + tmpArr[1];
            }
            if (lines.workOrder_HDR.DUEDATE != null)
            {
                tmpArr = lines.workOrder_HDR.TRANSDATE.ToString().Split(' ');
                new_DUEDATE = tmpArr[0].Split('/')[2] + "/" + tmpArr[0].Split('/')[1] + "/" + tmpArr[0].Split('/')[0] + " " + tmpArr[1];
            }
            if (lines.workOrder_HDR.EXPIRY_DATE != null)
            {
                tmpArr = lines.workOrder_HDR.TRANSDATE.ToString().Split(' ');
                new_EXPIRY_DATE = tmpArr[0].Split('/')[2] + "/" + tmpArr[0].Split('/')[1] + "/" + tmpArr[0].Split('/')[0] + " " + tmpArr[1];
            }
            if (lines.workOrder_HDR.X_COMPLETION_DATE != null)
            {
                tmpArr = lines.workOrder_HDR.TRANSDATE.ToString().Split(' ');
                new_X_COMPLETION_DATE = tmpArr[0].Split('/')[2] + "/" + tmpArr[0].Split('/')[1] + "/" + tmpArr[0].Split('/')[0] + " " + tmpArr[1];
            }



            var sql_3 = "update GRAM_SYD_LIVE.dbo.WORKSORD_HDR set " +
                " BILLCODE = " + ((lines.workOrder_HDR.BILLCODE == null)? "null" : "'" + lines.workOrder_HDR.BILLCODE + "'") + ", " +
                //" PRODCODE = " + ((lines.workOrder_HDR.PRODCODE == null)? "null" : "'" + lines.workOrder_HDR.PRODCODE + "'") + ", " +
                //" BATCHCODE = " + ((lines.workOrder_HDR.BATCHCODE == null)? "null" : "'" + lines.workOrder_HDR.BATCHCODE + "'") + ", " +
                " TRANSDATE = " + ((lines.workOrder_HDR.TRANSDATE == null)? "null" : "'" + new_TRANSDATE + "'") + ", " +
                " PRODDATE = " + ((lines.workOrder_HDR.PRODDATE == null)? "null" : "'" + new_PRODDATE + "'") + ", " +
                " DUEDATE = " + ((lines.workOrder_HDR.DUEDATE == null)? "null" : "'" + new_DUEDATE + "'") + ", " +
                " ORDSTATUS = " + ((lines.workOrder_HDR.ORDSTATUS == null)? "null" : lines.workOrder_HDR.ORDSTATUS.ToString()) + ", " +
                " SALESORDNO = " + ((lines.workOrder_HDR.SALESORDNO == null)? "null" : lines.workOrder_HDR.SALESORDNO.ToString()) + ", " +
                " NOTES = " + ((lines.workOrder_HDR.NOTES == null)? "null" : "'" + lines.workOrder_HDR.NOTES + "'") + ", " +
                " PRODQTY = " + ((lines.workOrder_HDR.PRODQTY == null)? "null" : lines.workOrder_HDR.PRODQTY.ToString()) + ", " +
                " ACTUALQTY = " + ((lines.workOrder_HDR.ACTUALQTY == null)? "null" : lines.workOrder_HDR.ACTUALQTY.ToString()) + ", " +
                //" PRODLOCNO = " + ((lines.workOrder_HDR.PRODLOCNO == null)? "null" : lines.workOrder_HDR.PRODLOCNO.ToString()) + ", " +
                " REFERENCE = " + ((lines.workOrder_HDR.REFERENCE == null)? "null" : "'" + lines.workOrder_HDR.REFERENCE + "'") + ", " +
                " STAFFNO = " + ((lines.workOrder_HDR.STAFFNO == null)? "null" : lines.workOrder_HDR.STAFFNO.ToString()) + ", " +
                //" COMPONENTLOCNO = " + ((lines.workOrder_HDR.COMPONENTLOCNO == null)? "null" : lines.workOrder_HDR.COMPONENTLOCNO.ToString()) + ", " +
                " EXPIRY_DATE = " + ((lines.workOrder_HDR.EXPIRY_DATE == null)? "null" : "'" + new_EXPIRY_DATE + "'") + ", " +
                " X_BR_ORDER = " + ((lines.workOrder_HDR.X_BR_ORDER == null)? "null" : lines.workOrder_HDR.X_BR_ORDER.ToString()) + ", " +
                " X_BR_ACCNO = " + ((lines.workOrder_HDR.X_BR_ACCNO == null)? "null" : "'" + lines.workOrder_HDR.X_BR_ACCNO + "'") + ", " +
                " X_BR_INVNO = " + ((lines.workOrder_HDR.X_BR_INVNO == null)? "null" : "'" + lines.workOrder_HDR.X_BR_INVNO + "'") + ", " +
                //" X_BR = " + ((lines.workOrder_HDR.X_BR == null)? "null" : "'" + lines.workOrder_HDR.X_BR  + "'") + ", " +
                " X_CATEGORY = " + ((lines.workOrder_HDR.X_CATEGORY == null)? "null" : "'" + lines.workOrder_HDR.X_CATEGORY + "'") + ", " +
                " X_COMPLETION_DATE = " + ((lines.workOrder_HDR.X_COMPLETION_DATE == null)? "null" : "'" + new_X_COMPLETION_DATE + "'") + " " +
                "where SEQNO = " + lines.workOrder_HDR.SEQNO;

            WorkOrder_HDRs headerDetails = new WorkOrder_HDRs();

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    if (lines.updateFlag == "update") 
                    {
                        sql = "select * from GRAM_SYD_LIVE.dbo.WORKSORD_HDR where SEQNO = '" + lines.workOrder_HDR.SEQNO + "'";
                        sql_2 = "select * from GRAM_SYD_LIVE.dbo.WORKSORD_LINES where HDR_SEQNO = '" + lines.workOrder_HDR.SEQNO + "'";

                        if (lines.workOrder_Lines != null)
                        {
                            for (int i = 0; i < lines.workOrder_Lines.Count; i++)
                            {
                                var sql_4 = "update GRAM_SYD_LIVE.dbo.WORKSORD_LINES set " +
                                    //"HDR_SEQNO = " + ((lines.workOrder_Lines[i].HDR_SEQNO == null) ? "null" : lines.workOrder_Lines[i].HDR_SEQNO.ToString()) + ", " +
                                    //"STOCKCODE = " + ((lines.workOrder_Lines[i].STOCKCODE == null) ? "null" : "'" + lines.workOrder_Lines[i].STOCKCODE.ToString() + "'") + ", " +
                                    "DESCRIPTION = " + ((lines.workOrder_Lines[i].DESCRIPTION == null) ? "null" : "'" + lines.workOrder_Lines[i].DESCRIPTION + "'") + ", " +
                                    "QTYREQD = " + ((lines.workOrder_Lines[i].QTYREQD == null) ? "null" : lines.workOrder_Lines[i].QTYREQD.ToString()) + ", " +
                                    //"QTYUSED = " + ((lines.workOrder_Lines[i].QTYUSED == null) ? "null" : lines.workOrder_Lines[i].QTYUSED.ToString()) + ", " +
                                    //"BATCHCODE = " + ((lines.workOrder_Lines[i].BATCHCODE == null) ? "null" : "'" + lines.workOrder_Lines[i].BATCHCODE + "'") + ", " +
                                    "X_LENGTH = " + ((lines.workOrder_Lines[i].X_LENGTH == null) ? "null" : "'" + lines.workOrder_Lines[i].X_LENGTH + "'") + ", " +
                                    //"X_SOLINE = " + ((lines.workOrder_Lines[i].X_SOLINE == null) ? "null" : lines.workOrder_Lines[i].X_SOLINE.ToString()) + ", " +
                                    "X_NARRATIVE = " + ((lines.workOrder_Lines[i].X_NARRATIVE == null) ? "null" : lines.workOrder_Lines[i].X_NARRATIVE.ToString()) + ", " +
                                    //"X_LINESTATUS = " + ((lines.workOrder_Lines[i].X_LINESTATUS == null) ? "null" : lines.workOrder_Lines[i].X_LINESTATUS.ToString()) + ", " +
                                    "X_COLOR = " + ((lines.workOrder_Lines[i].X_COLOR == null) ? "null" : "'" + lines.workOrder_Lines[i].X_COLOR.ToString() + "'") + " " +
                                    "where SEQNO = " + lines.workOrder_Lines[i].SEQNO;
                                context.Database.ExecuteSqlCommand(sql_4);
                            }
                        }
                        context.Database.ExecuteSqlCommand(sql_3);
                    }
                    lines.workOrder_Lines = context.Database.SqlQuery<WorkOrder_Line>(sql_2).ToList<WorkOrder_Line>();
                    headerDetails.workOrder_HDRs = context.Database.SqlQuery<WorkOrder_HDR>(sql).ToList<WorkOrder_HDR>();
                }
            }
            catch (Exception e)
            {
                lines.errMsg = "No Record Found.";
            }

            lines.workOrder_HDR.SEQNO = headerDetails.workOrder_HDRs[0].SEQNO;
            lines.workOrder_HDR.BILLCODE = headerDetails.workOrder_HDRs[0].BILLCODE;
            lines.workOrder_HDR.PRODCODE = headerDetails.workOrder_HDRs[0].PRODCODE;
            lines.workOrder_HDR.BATCHCODE = headerDetails.workOrder_HDRs[0].BATCHCODE;
            lines.workOrder_HDR.TRANSDATE = headerDetails.workOrder_HDRs[0].TRANSDATE;
            lines.workOrder_HDR.PRODDATE = headerDetails.workOrder_HDRs[0].PRODDATE;
            lines.workOrder_HDR.DUEDATE = headerDetails.workOrder_HDRs[0].DUEDATE;
            lines.workOrder_HDR.ORDSTATUS = headerDetails.workOrder_HDRs[0].ORDSTATUS;
            lines.workOrder_HDR.SALESORDNO = headerDetails.workOrder_HDRs[0].SALESORDNO;
            lines.workOrder_HDR.NOTES = headerDetails.workOrder_HDRs[0].NOTES;
            lines.workOrder_HDR.PRODQTY = headerDetails.workOrder_HDRs[0].PRODQTY;
            lines.workOrder_HDR.ACTUALQTY = headerDetails.workOrder_HDRs[0].ACTUALQTY;
            lines.workOrder_HDR.PRODLOCNO = headerDetails.workOrder_HDRs[0].PRODLOCNO;
            lines.workOrder_HDR.REFERENCE = headerDetails.workOrder_HDRs[0].REFERENCE;
            lines.workOrder_HDR.STAFFNO = headerDetails.workOrder_HDRs[0].STAFFNO;
            lines.workOrder_HDR.COMPONENTLOCNO = headerDetails.workOrder_HDRs[0].COMPONENTLOCNO;
            lines.workOrder_HDR.EXPIRY_DATE = headerDetails.workOrder_HDRs[0].EXPIRY_DATE;
            lines.workOrder_HDR.X_BR_ORDER = headerDetails.workOrder_HDRs[0].X_BR_ORDER;
            lines.workOrder_HDR.X_BR_ACCNO = headerDetails.workOrder_HDRs[0].X_BR_ACCNO;
            lines.workOrder_HDR.X_BR_INVNO = headerDetails.workOrder_HDRs[0].X_BR_INVNO;
            lines.workOrder_HDR.X_BR = headerDetails.workOrder_HDRs[0].X_BR;
            lines.workOrder_HDR.X_CATEGORY = headerDetails.workOrder_HDRs[0].X_CATEGORY;
            lines.workOrder_HDR.X_COMPLETION_DATE = headerDetails.workOrder_HDRs[0].X_COMPLETION_DATE;


            Code39BarcodeDraw barcode39 = BarcodeDrawFactory.Code39WithoutChecksum;

            Image img = null;

            if (lines.workOrder_HDR.BILLCODE != null)
            {
                img = barcode39.Draw(lines.workOrder_HDR.BILLCODE, 50);
            }

            byte[] imgBytes = turnImageToByteArray(img);
            string imgString = Convert.ToBase64String(imgBytes);
            ViewBag.Barcode = String.Format("<img src=\"data:image/png;base64,{0}\"/>", imgString);
           
            return View(lines);
        }

        private byte[] turnImageToByteArray(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }

        [SessionExpire]
        [Authorize]
        public ActionResult WorkOrderHeaders()
        {
            WorkOrder_HDRs orders = new WorkOrder_HDRs();
            orders.statusSort = -1;
            return View(orders);
        }

        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult WorkOrderHeaders(WorkOrder_HDRs orders)
        {
            ViewBag.Title = "Work Order Headers";
            Session["CurrForm"] = "WorkOrderHeaders";

            if (string.IsNullOrEmpty(orders.sortCol))
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

                orders.sortCol = "DefaultSort";
                orders.sortColType = "Number";
                orders.rowsPerPage = 15;
                orders.pageNum = 1;
                orders.orderBy = "glyphicon glyphicon-arrow-up";
            }

            if (String.IsNullOrEmpty(orders.whereStr))
            {
                orders.whereStr = "";
            }

            if (orders.whereStr.Replace(" ", "") == "")
            {
                orders.whereStr = "";
            }

            if (String.IsNullOrEmpty(orders.categorySort))
            {
                orders.categorySort = "";
            }

            var Role = new SqlParameter("@Role", ((Scanner.Models.User)Session["User"]).Role);
            var UserName = new SqlParameter("@UserName", ((Scanner.Models.User)Session["User"]).UserName);
            var pageNum = new SqlParameter("@pageNum", (orders.pageNum == 0) ? 1 : orders.pageNum);
            var rowsPerPage = new SqlParameter("@rowsPerPage", orders.rowsPerPage);
            var sortCol = new SqlParameter("@sortCol", orders.sortCol);
            var sortColType = new SqlParameter("@sortColType", orders.sortColType);
            var whereStr = new SqlParameter("@whereStr", orders.whereStr.ToString());
            var categorySort = new SqlParameter("@categorySort", orders.categorySort.ToString());
            var statusSort = new SqlParameter("@statusSort", orders.statusSort);

            var orderBy = (orders.orderBy == "glyphicon glyphicon-arrow-down") ?
                new SqlParameter("@orderBy", "desc") :
                new SqlParameter("@orderBy", "asc");


            var table = new SqlParameter("@table", "GRAM_SYD_LIVE.dbo.WORKSORD_HDR");
            var selStr = new SqlParameter("@selStr", "");


           // sideMenus = context.Database.SqlQuery<SideMenu>("GramOnline.dbo.proc_GetSideMenu_v2").ToList<SideMenu>();
            var sql = "exec GramOnline.dbo.proc_GetWorkOrders " +
                "@Role," +
                "@UserName, " +
                "@pageNum, " +
                "@rowsPerPage, " +
                "@sortCol, " +
                "@sortColType, " +
                "@whereStr, " +
                "@orderBy, " +
                "@table, " +
                "@selStr, " +
                "@categorySort, " +
                "@statusSort";

            var oldMsg = "";

            if (orders.errMsg == null)
                orders.errMsg = "";
            else
                oldMsg = orders.errMsg;

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    orders.workOrder_HDRs = context.Database.SqlQuery<WorkOrder_HDR>(sql,
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
                       categorySort,
                       statusSort).ToList<WorkOrder_HDR>();
                    if (orders.CategoryList == null)
                    {
                        var sql_category = "exec GramOnline.dbo.proc_GetWorkOrderCatrgoryList";
                        orders.CategoryList = context.Database.SqlQuery<string>(sql_category).ToList<string>();
                    }
                }
            }
            catch (Exception e)
            {
                orders.totalPages = 0;
                orders.totalRows = 0;
                orders.workOrder_HDRs = null;
                orders.errMsg = "No Record Found";
            }

            if (orders.workOrder_HDRs != null)
            {
                if (orders.workOrder_HDRs.Count > 0)
                {
                    orders.totalPages = orders.workOrder_HDRs[0].maxPages;
                    orders.totalRows = orders.workOrder_HDRs[0].TotalRows;
                }
            }

            if (oldMsg != "")
                orders.errMsg = oldMsg;

            return View(orders);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Product2()
        {
            CoilDetail details = new CoilDetail();
            return View(details);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Product3()
        {
            CoilDetail details = new CoilDetail();
            return View(details);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Coil()
        {
            CoilDetail details = new CoilDetail();
            return View(details);
        }

        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult Coil(CoilDetail model)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings["GramLineConn"].ToString();

            string input;

            if (model.CoilDetails == null)
            {
                return View(model);
            }

            if (model.CoilDetails.Count > 1)
            {
                var test = model.CoilDetails[1].Gauge;
            }
            if (model.CoilDetails[0].Flag == "UPLOAD")
                input = model.CoilDetails[0].SaveTable;
            else
                input = model.CoilDetails[0].Input;

            if (input == null)
            {
                return View(model);
            }

            List<string> coilIDs = new List<string>();

            int splitAt = CoilQRcodeLength; // change 9 with the size of strings you want.
            int coilIDCount = 0;
            for (int i = 0; i < input.Length; i = i + splitAt)
            {
                coilIDCount++;
                if (input.Length - i >= splitAt)
                    coilIDs.Add(input.Substring(i, CoilIDLength));
                else if (input.Length - i >= CoilIDLength)
                    coilIDs.Add(input.Substring(i, CoilIDLength));
                else
                    coilIDs.Add(input.Substring(i, ((input.Length - i))));
            }

            for (int i = 0; i < coilIDCount; i++)
            {
                using (SqlConnection newCon = new SqlConnection(ConnStr))
                {
                    if (model.CoilDetails[0].Flag == "UPLOAD")
                    {
                        var date = DateTime.Now;
                        var UserName = ((Scanner.Models.User)Session["User"]).FirstName + " " + ((Scanner.Models.User)Session["User"]).LastName;
                        SqlCommand newCmd2 = new SqlCommand(("IF NOT EXISTS (SELECT * FROM GramOnline.dbo.X_COIL_TEST WHERE COILID = '" + coilIDs[i] + "') BEGIN INSERT INTO GramOnline.dbo.X_COIL_TEST (COILID, DATE_INSERT, UserName) VALUES ('" + coilIDs[i] + "', GETDATE(), '" + UserName + "') END"), newCon);
                        newCon.Open();
                        SqlDataReader rdr2 = newCmd2.ExecuteReader();

                        ViewBag.Upload = "Data uploaded!";
                        newCon.Close();
                    }
                    else
                    {
                        CoilModel modelDetail = new CoilModel();
                        modelDetail.ID = coilIDs[i];
                        modelDetail.Save = model.CoilDetails[0].Save;
                        SqlCommand newCmd = new SqlCommand(("select * from GRAM_SYD_LIVE.dbo.X_COIL_MASTER where COILID = '" + coilIDs[i] + "'"), newCon);
                        newCon.Open();
                        SqlDataReader rdr = newCmd.ExecuteReader();
                        if (rdr.HasRows) // If the sql command doesn't return any record, display a message
                        {
                            rdr.Read();
                            if (!rdr.IsDBNull(0))
                                modelDetail.ID = rdr.GetString(0);
                            if (!rdr.IsDBNull(1))
                                modelDetail.Type = rdr.GetString(1);
                            if (!rdr.IsDBNull(2))
                                modelDetail.Color = rdr.GetString(2);
                            if (!rdr.IsDBNull(3))
                                modelDetail.Weight = rdr.GetDouble(3);
                            if (!rdr.IsDBNull(4))
                                modelDetail.Gauge = rdr.GetDouble(4);
                            if (!rdr.IsDBNull(5))
                                modelDetail.Width = rdr.GetDouble(5);
                            if (!rdr.IsDBNull(6))
                                modelDetail.Order = rdr.GetInt32(6);
                            if (!rdr.IsDBNull(7))
                                modelDetail.P_order = rdr.GetString(7);
                            if (!rdr.IsDBNull(8))
                                modelDetail.Month_recd = rdr.GetString(8);
                            if (!rdr.IsDBNull(9))
                                modelDetail.Date_inwh = rdr.GetDateTime(9);
                            if (!rdr.IsDBNull(10))
                                modelDetail.Date_transfer = rdr.GetDateTime(10);
                            if (!rdr.IsDBNull(11))
                                modelDetail.Last_stocktake_date = rdr.GetDateTime(11);
                            if (!rdr.IsDBNull(12))
                                modelDetail.Status = rdr.GetString(12);
                            if (!rdr.IsDBNull(13))
                                modelDetail.Clength = rdr.GetInt32(13);
                        }
                        else
                        {
                            if (coilIDs.Count == 1)
                                ViewBag.Error = "No information was found in the database.";
                        }
                        newCon.Close();
                        model.CoilDetails.Add(modelDetail);
                    }
                }
            }
            return View(model);
        }
    }
}
