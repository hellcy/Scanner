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
using System.Drawing.Printing;

namespace Scanner.Controllers
{
    public class ScannerController : BaseController
    {

        CoilMasters details = new CoilMasters();

        // used at WorkOrderLines to decode System.Drawing image
        private byte[] turnImageToByteArray(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }

        // used at Coil, change the lengths here for different products!
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

        /* 
        * This Action is to display all the Work Order Lines in a table under a Work Order, and will also display the Work Order Details.
        * Users can change the value of Lines and Work Order Details, and update the database by clicking the 'Update' button.
        * This Action can be accessed by click the 'Work Order' link in the top menu and then click any Sequence Number in the Work Order table.
        */
        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult WorkOrderLines(WorkOrder_Lines lines)
        {
            ViewBag.Title = "Work Order Lines";
            Session["CurrForm"] = "WorkOrderLines";

            string SeqNo = Request.QueryString["ID"];

            var sql = "select * from GRAM_SYD_LIVE.dbo.WORKSORD_HDR where SEQNO = '" + SeqNo + "'"; // to get all information of the SeqNo of a workOrderHeader
            var sql_2 = "select * from GRAM_SYD_LIVE.dbo.WORKSORD_LINES where HDR_SEQNO = '" + SeqNo + "'"; // to get all workOrderLines under the header SeqNo

            string new_TRANSDATE = "";
            string new_PRODDATE = "";
            string new_DUEDATE = "";
            string new_EXPIRY_DATE = "";
            string new_X_COMPLETION_DATE = "";
            string[] tmpArr;

            if (lines.workOrder_HDR.TRANSDATE != null)
            {
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
                " BILLCODE = " + ((lines.workOrder_HDR.BILLCODE == null) ? "null" : "'" + lines.workOrder_HDR.BILLCODE + "'") + ", " +
                //" PRODCODE = " + ((lines.workOrder_HDR.PRODCODE == null)? "null" : "'" + lines.workOrder_HDR.PRODCODE + "'") + ", " +
                //" BATCHCODE = " + ((lines.workOrder_HDR.BATCHCODE == null)? "null" : "'" + lines.workOrder_HDR.BATCHCODE + "'") + ", " +
                " TRANSDATE = " + ((lines.workOrder_HDR.TRANSDATE == null) ? "null" : "'" + new_TRANSDATE + "'") + ", " +
                " PRODDATE = " + ((lines.workOrder_HDR.PRODDATE == null) ? "null" : "'" + new_PRODDATE + "'") + ", " +
                " DUEDATE = " + ((lines.workOrder_HDR.DUEDATE == null) ? "null" : "'" + new_DUEDATE + "'") + ", " +
                " ORDSTATUS = " + ((lines.workOrder_HDR.ORDSTATUS == null) ? "null" : lines.workOrder_HDR.ORDSTATUS.ToString()) + ", " +
                " SALESORDNO = " + ((lines.workOrder_HDR.SALESORDNO == null) ? "null" : lines.workOrder_HDR.SALESORDNO.ToString()) + ", " +
                " NOTES = " + ((lines.workOrder_HDR.NOTES == null) ? "null" : "'" + lines.workOrder_HDR.NOTES + "'") + ", " +
                " PRODQTY = " + ((lines.workOrder_HDR.PRODQTY == null) ? "null" : lines.workOrder_HDR.PRODQTY.ToString()) + ", " +
                " ACTUALQTY = " + ((lines.workOrder_HDR.ACTUALQTY == null) ? "null" : lines.workOrder_HDR.ACTUALQTY.ToString()) + ", " +
                //" PRODLOCNO = " + ((lines.workOrder_HDR.PRODLOCNO == null)? "null" : lines.workOrder_HDR.PRODLOCNO.ToString()) + ", " +
                " REFERENCE = " + ((lines.workOrder_HDR.REFERENCE == null) ? "null" : "'" + lines.workOrder_HDR.REFERENCE + "'") + ", " +
                " STAFFNO = " + ((lines.workOrder_HDR.STAFFNO == null) ? "null" : lines.workOrder_HDR.STAFFNO.ToString()) + ", " +
                //" COMPONENTLOCNO = " + ((lines.workOrder_HDR.COMPONENTLOCNO == null)? "null" : lines.workOrder_HDR.COMPONENTLOCNO.ToString()) + ", " +
                " EXPIRY_DATE = " + ((lines.workOrder_HDR.EXPIRY_DATE == null) ? "null" : "'" + new_EXPIRY_DATE + "'") + ", " +
                " X_BR_ORDER = " + ((lines.workOrder_HDR.X_BR_ORDER == null) ? "null" : lines.workOrder_HDR.X_BR_ORDER.ToString()) + ", " +
                " X_BR_ACCNO = " + ((lines.workOrder_HDR.X_BR_ACCNO == null) ? "null" : "'" + lines.workOrder_HDR.X_BR_ACCNO + "'") + ", " +
                " X_BR_INVNO = " + ((lines.workOrder_HDR.X_BR_INVNO == null) ? "null" : "'" + lines.workOrder_HDR.X_BR_INVNO + "'") + ", " +
                //" X_BR = " + ((lines.workOrder_HDR.X_BR == null)? "null" : "'" + lines.workOrder_HDR.X_BR  + "'") + ", " +
                " X_CATEGORY = " + ((lines.workOrder_HDR.X_CATEGORY == null) ? "null" : "'" + lines.workOrder_HDR.X_CATEGORY + "'") + ", " +
                " X_COMPLETION_DATE = " + ((lines.workOrder_HDR.X_COMPLETION_DATE == null) ? "null" : "'" + new_X_COMPLETION_DATE + "'") + " " +
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
                lines.errMsg = "No Record Found: ";
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

            CodeQrBarcodeDraw QRcode = BarcodeDrawFactory.CodeQr; // to generate QR code
            //Code39BarcodeDraw barcode39 = BarcodeDrawFactory.Code39WithoutChecksum; // to generate barcode

            Image img = null;

            if (lines.workOrder_HDR.BILLCODE != null)
            {
                img = QRcode.Draw(lines.workOrder_HDR.BILLCODE, 50);
            }

            // QR code size: 150px for about 4cm
            // QR code maximum length: 120 characters
            byte[] imgBytes = turnImageToByteArray(img);
            string imgString = Convert.ToBase64String(imgBytes);
            ViewBag.QRcode = String.Format("<img src=\"data:image/bmp;base64,{0}\"/>", imgString);

            //Example for test purpose
            //img = QRcode.Draw("73M117A00+PL+SGR+4730+0.8+1025aaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbbbbbccccccccccccccccccccddddddddddddddddddddeeeeeeeeee", 50);
            //imgBytes = turnImageToByteArray(img);
            //string StringExample = Convert.ToBase64String(imgBytes);
            //ViewBag.QRCodeExample = String.Format("<img src=\"data:image/bmp;base64,{0}\", height=\"150px; \"/>", StringExample);

            return View(lines);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult WorkOrderHeaders()
        {
            WorkOrder_HDRs orders = new WorkOrder_HDRs();
            orders.statusSort = -1;
            return View(orders);
        }

        /*
         * This Action is to display all the Work Order Headers in a table
         * This Action can be accessed by clicking the 'Work Order' link in the top menu.
         */
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

            if (orders.sortCol != "SEQNO" && orders.sortCol != "BILLCODE" && orders.sortCol != "TRANSDATE" && orders.sortCol != "PRODDATE" && orders.sortCol != "DUEDATE"
                && orders.sortCol != "ORDSTATUS" && orders.sortCol != "SALESORDNO" && orders.sortCol != "NOTES" && orders.sortCol != "PRODQTY" && orders.sortCol != "ACTUALQTY"
                && orders.sortCol != "REFERENCE" && orders.sortCol != "STAFFNO" && orders.sortCol != "EXPIRY_DATE" && orders.sortCol != "X_BR_ORDER" && orders.sortCol != "X_BR_ACCNO"
                && orders.sortCol != "X_BR_INVNO" && orders.sortCol != "X_CATEGORY" && orders.sortCol != "X_COMPLETION_DATE")
            {
                orders.sortCol = "DefaultSort";
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


            // sideMenus = context.Database.SqlQuery<SideMenu>("GramOnline.dbo.proc_Y_GetSideMenu_v2").ToList<SideMenu>();
            var sql = "exec GramOnline.dbo.proc_Y_GetWorkOrders " +
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
                        var sql_category = "exec GramOnline.dbo.proc_Y_GetWorkOrderCatrgoryList";
                        orders.CategoryList = context.Database.SqlQuery<string>(sql_category).ToList<string>();
                    }
                }
            }
            catch (Exception e)
            {
                orders.totalPages = 0;
                orders.totalRows = 0;
                orders.workOrder_HDRs = null;
                orders.errMsg = e.Message.ToString().Replace("'", "\"");
                if (e.Message.Equals("The specified cast from a materialized 'System.String' type to the 'System.Int32' type is not valid."))
                {
                    orders.errMsg = "No Record Found.";
                }
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
        public ActionResult Coil()
        {
            CoilMasters details = new CoilMasters();
            details.CoilDetails = new List<CoilMaster>();
            return View(details);
        }


        /*
         * This Action is to allow users to scan the 'Upload Data' QR code and upload Coil ID to the database, it can also display details of a certain 
         * Coil in a table based on their ID.
         * This Action can be accessed by clicking the 'Coil' link in the top menu.
         */
        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult Coil(CoilMasters model)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings["GramLineConn"].ToString();

            string input;

            if (model.CoilDetails == null)
            {
                return View(model);
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

            //int splitAt = CoilQRcodeLength;
            int coilIDCount = 0;

            //for (int i = 0; i < input.Length; i = i + splitAt)
            //{
            //    coilIDCount++;
            //    if (input.Length - i >= splitAt)
            //        coilIDs.Add(input.Substring(i, CoilIDLength));
            //    else if (input.Length - i >= CoilIDLength)
            //        coilIDs.Add(input.Substring(i, CoilIDLength));
            //    else
            //        coilIDs.Add(input.Substring(i, ((input.Length - i))));
            //}
            string[] lines = input.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );
            coilIDCount = lines.Length - 1;

            for (int i = 0; i < coilIDCount; i++)
            {
                coilIDs.Add(lines[i].Substring(0, CoilIDLength));
            }

            for (int i = 0; i < coilIDCount; i++)
            {
                using (SqlConnection newCon = new SqlConnection(ConnStr))
                {
                    if (model.CoilDetails[0].Flag == "UPLOAD")
                    {
                        //model = (CoilMasters)Session["aaa"];
                        var date = DateTime.Now;
                        var UserName = ((Scanner.Models.User)Session["User"]).FirstName + " " + ((Scanner.Models.User)Session["User"]).LastName;
                        SqlCommand newCmd2 = new SqlCommand(("IF NOT EXISTS (SELECT * FROM GramOnline.dbo.TB_Y_X_COIL_TEST WHERE COILID = '" + coilIDs[i] + "') BEGIN INSERT INTO GramOnline.dbo.TB_Y_X_COIL_TEST (COILID, TYPE, COLOR, WEIGHT, GAUGE, WIDTH, DATE_INSERT, UserName) VALUES ('" + coilIDs[i] + "' , '" + model.CoilDetails[i + 1].TYPE + "' , '" + model.CoilDetails[i + 1].COLOR + "' , " + model.CoilDetails[i + 1].WEIGHT + " , " + model.CoilDetails[i + 1].GAUGE + " , " + model.CoilDetails[i + 1].WIDTH + " , GETDATE(), '" + UserName + "') END"), newCon);
                        newCon.Open();
                        SqlDataReader rdr2 = newCmd2.ExecuteReader();

                        ViewBag.Upload = "Data uploaded!";
                        newCon.Close();
                    }
                    else
                    {

                        CoilMaster modelDetail = new CoilMaster();
                        modelDetail.COILID = coilIDs[i];
                        modelDetail.Save = model.CoilDetails[0].Save;

                        string[] tokens = lines[i].Split('+');
                        modelDetail.COILID = tokens[0];
                        modelDetail.TYPE = tokens[1];
                        modelDetail.COLOR = tokens[2];
                        modelDetail.WEIGHT = Double.Parse(tokens[3]);
                        modelDetail.GAUGE = Double.Parse(tokens[4]);
                        modelDetail.WIDTH = Double.Parse(tokens[5]);
                        model.CoilDetails.Add(modelDetail);
                        //model.CoilDetails = new List<CoilMaster>();
                        //CoilMaster coilMaster = new CoilMaster();

                        //coilMaster.COILID = coilIDs[i];

                        //string[] tokens = lines[i].Split('+');
                        //coilMaster.COILID = tokens[0];
                        //coilMaster.TYPE = tokens[1];
                        //coilMaster.COLOR = tokens[2];
                        //coilMaster.WEIGHT = Double.Parse(tokens[3]);
                        //coilMaster.GAUGE = Double.Parse(tokens[4]);
                        //coilMaster.WIDTH = Double.Parse(tokens[5]);
                        //model.CoilDetails.Add(coilMaster);

                        //SqlCommand newCmd = new SqlCommand(("select * from GRAM_SYD_LIVE.dbo.X_COIL_MASTER where COILID = '" + coilIDs[i] + "'"), newCon);
                        //newCon.Open();
                        //SqlDataReader rdr = newCmd.ExecuteReader();
                        //if (rdr.HasRows) // If the sql command doesn't return any record, display a message
                        //{
                        //    rdr.Read();
                        //    if (!rdr.IsDBNull(0))
                        //        modelDetail.COILID = rdr.GetString(0);
                        //    if (!rdr.IsDBNull(1))
                        //        modelDetail.TYPE = rdr.GetString(1);
                        //    if (!rdr.IsDBNull(2))
                        //        modelDetail.COLOR = rdr.GetString(2);
                        //    if (!rdr.IsDBNull(3))
                        //        modelDetail.WEIGHT = rdr.GetDouble(3);
                        //    if (!rdr.IsDBNull(4))
                        //        modelDetail.GAUGE = rdr.GetDouble(4);
                        //    if (!rdr.IsDBNull(5))
                        //        modelDetail.WIDTH = rdr.GetDouble(5);
                        //    if (!rdr.IsDBNull(6))
                        //        modelDetail.ORDER = rdr.GetInt32(6);
                        //    if (!rdr.IsDBNull(7))
                        //        modelDetail.P_ORDER = rdr.GetString(7);
                        //    if (!rdr.IsDBNull(8))
                        //        modelDetail.MONTH_RECD = rdr.GetString(8);
                        //    if (!rdr.IsDBNull(9))
                        //        modelDetail.DATE_INWH = rdr.GetDateTime(9);
                        //    if (!rdr.IsDBNull(10))
                        //        modelDetail.DATE_TRANSFER = rdr.GetDateTime(10);
                        //    if (!rdr.IsDBNull(11))
                        //        modelDetail.LAST_STOCKTAKE_DATE = rdr.GetDateTime(11);
                        //    if (!rdr.IsDBNull(12))
                        //        modelDetail.STATUS = rdr.GetString(12);
                        //    if (!rdr.IsDBNull(13))
                        //        modelDetail.CLENGTH = rdr.GetInt32(13);
                        //}
                        //else
                        //{
                        //    if (coilIDs.Count == 1)
                        //        ViewBag.Error = "No information was found in the database.";
                        //}
                        //newCon.Close();
                        //model.CoilDetails.Add(modelDetail);
                    }
                }
            }
            Session["aaa"] = model.CoilDetails;
            return View(model);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult CoilSlit()
        {
            CoilSlits slits = new CoilSlits();
            return View(slits);
        }

        /*
         * This Action is to print new labels for the coil slit produced. When users scan the main coil, all details of that coil will be displayed.
         * User can choose to cut the coil into 2, 4, 6 or 8 pieces, after selected, user will see the information of their choice, including new coil slit ID.
         * User can click the 'PRINT' button to print the new labels for the coil slit, the information of that new coil slit will be added to the database. 
         * Or they can also 'BACK' to the previous page or 'CLEAR' the result and start over.
         * This Action can be accessed by clicking the 'Coil Slit' link in the top menu.
         */
        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult CoilSlit(CoilSlits slits)
        {
            ViewBag.Title = "Coil Slit";
            Session["CurrForm"] = "CoilSlit";

            slits.errMsg = "";

            if (slits.inputHidden != null)
            {
                //char[] delimiters = { ' ', '+' };
                //string[] inputArray = slits.input.Split(delimiters); // split the input string by using the delimiter '+'
                if (slits.inputHidden.Length < 9)
                {
                    slits.errMsg = "Wrong Coil ID.";
                    return View(slits);
                }

                string coilID = slits.inputHidden.Substring(0,9);

                int cover_width = 0;
                int base_width = 0;
                if (slits.slitWidth == 118)
                {
                    cover_width = 51;
                    base_width = 67;
                }
                if (slits.slitWidth == 150)
                {
                    cover_width = 67;
                    base_width = 83;
                }
                if (slits.slitWidth == 200)
                {
                    cover_width = 92;
                    base_width = 108;
                }

                CodeQrBarcodeDraw QRcode = BarcodeDrawFactory.CodeQr; // to generate QR code
                Code128BarcodeDraw barcode128 = BarcodeDrawFactory.Code128WithChecksum; // to generate barcode
                Image img_QRcode = null;
                Image img_Barcode = null;
                byte[] imgBytes;
                string imgString;

                var sql = "select * from GRAM_SYD_LIVE.dbo.X_COIL_MASTER where COILID = '" + coilID + "'";

                try
                {
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        slits.CoilDetails = context.Database.SqlQuery<CoilMaster>(sql).ToList<CoilMaster>();
                    }
                }
                catch (Exception e)
                {
                    slits.errMsg = "SQL Exception: " + e + ";";
                }

                if (slits.CoilDetails.Count == 0)
                {
                    slits.errMsg = "No information found in the database.";
                    return View(slits);
                }

                string M_color = "";
                string S_color = "";
                if (slits.CoilDetails[0].COLOR.Length == 4)
                {
                    M_color = slits.CoilDetails[0].COLOR.Substring(0, 2);
                    S_color = slits.CoilDetails[0].COLOR.Substring(2, 2);
                }

                switch (slits.CoilDetails[0].TYPE)
                {
                    case "RA":
                        slits.slitNumber = 4;
                        slits.slitWidth = 170;
                        cover_width = slits.slitWidth;
                        base_width = slits.slitWidth;
                        break;
                    case "SM":
                        break;
                    case "PO":
                        slits.slitNumber = 8;
                        slits.slitWidth = 135;
                        cover_width = slits.slitWidth;
                        base_width = slits.slitWidth;
                        break;
                    case "PL":
                        slits.slitNumber = 4;
                        slits.slitWidth = 255;
                        cover_width = slits.slitWidth;
                        base_width = slits.slitWidth;
                        break;
                    case "IS":
                        slits.slitNumber = 8;
                        slits.slitWidth = 116;
                        cover_width = slits.slitWidth;
                        base_width = slits.slitWidth;
                        break;
                    default:
                        slits.errMsg = "Type not exist.";
                        break;
                }

                if (slits.slitNumber > 0 && slits.CoilDetails != null)
                {
                    string[] slitIDs = new string[slits.slitNumber];
                    string[] slitLabels = new string[slits.slitNumber];
                    slits.QRcodes = new string[8];
                    slits.Barcodes = new string[8];
                    for (int i = 1; i < slitIDs.Length + 1; i++)
                    {
                        slitIDs[i - 1] = slits.CoilDetails[0].COILID + "_" + i;
                        slits.slits.Add(new CoilSlit());
                        slits.slits[i - 1].COIL_SLIT_ID = slitIDs[i - 1];
                        slits.slits[i - 1].TYPE = slits.CoilDetails[0].TYPE;
                        slits.slits[i - 1].COLOR = slits.CoilDetails[0].COLOR;
                        slits.slits[i - 1].M_COLOR = M_color;
                        slits.slits[i - 1].S_COLOR = S_color;
                        slits.slits[i - 1].WEIGHT = (int)(slits.CoilDetails[0].WEIGHT / slits.slitNumber);
                        slits.slits[i - 1].GAUGE = slits.CoilDetails[0].GAUGE;
                        slits.slits[i - 1].LENGTH = slits.CoilDetails[0].CLENGTH;
                        if (i % 2 != 0)
                        {
                            slits.slits[i - 1].WIDTH = cover_width;
                        }
                        else
                        {
                            slits.slits[i - 1].WIDTH = base_width;
                        }
                        slits.slits[i - 1].STATUS = 0; // new -> 0, used -> 1

                        slitLabels[i - 1] = slits.CoilDetails[0].COILID + "_" + i + "+" + slits.CoilDetails[0].TYPE + "+" + slits.CoilDetails[0].COLOR + "+" + (int)(slits.CoilDetails[0].WEIGHT / slits.slitNumber) + "+" + slits.CoilDetails[0].GAUGE + "+" + slits.slits[i - 1].WIDTH;
                        BarcodeMetrics barcodeMetrics = QRcode.GetDefaultMetrics(150);
                        barcodeMetrics.Scale = 3; //qrcode size
                        img_QRcode = QRcode.Draw(slitLabels[i - 1], barcodeMetrics);
                        imgBytes = turnImageToByteArray(img_QRcode);
                        imgString = Convert.ToBase64String(imgBytes);
                        slits.QRcodes[i - 1] = String.Format("<img src=\"data:image/png;base64,{0}\"/>",imgString);

                        img_Barcode = barcode128.Draw(slitLabels[i - 1], 100);
                        imgBytes = turnImageToByteArray(img_Barcode);
                        imgString = Convert.ToBase64String(imgBytes);
                        slits.Barcodes[i - 1] = String.Format("<img src=\"data:image/png;base64,{0}\"/>",imgString);
                    }
                    slits.CoilSlitIDs = slitIDs;
                    slits.CoilSlitLabels = slitLabels;

                    if (slits.printFlag == "print")
                    {
                        for (int i = 0; i < slits.CoilSlitIDs.Count; i++)
                        {
                            var coilID_sql = new SqlParameter("@coilID", slits.CoilDetails[0].COILID);
                            var coilSlitID_sql = new SqlParameter("@coilSlitID", slits.slits[i].COIL_SLIT_ID);
                            var type_sql = new SqlParameter("@type", slits.slits[i].TYPE);
                            var color_sql = new SqlParameter("@color", slits.slits[i].COLOR);
                            var m_color_sql = new SqlParameter("@m_color", slits.slits[i].M_COLOR);
                            var s_color_sql = new SqlParameter("@s_color", slits.slits[i].S_COLOR);
                            var weight_sql = new SqlParameter("@weight", slits.slits[i].WEIGHT);
                            var gauge_sql = new SqlParameter("@gauge", slits.slits[i].GAUGE);
                            var width_sql = new SqlParameter("@width", slits.slits[i].WIDTH);
                            var status_sql = new SqlParameter("@status", slits.slits[i].STATUS);
                            var userID_sql = new SqlParameter("@userID", ((Scanner.Models.User)Session["User"]).UserName);
                            var length_sql = new SqlParameter("@length", DBNull.Value);
                            if (slits.slits[i].LENGTH != null)
                            {
                                length_sql = new SqlParameter("@length", slits.slits[i].LENGTH);
                            }
                            else
                            {
                                length_sql = new SqlParameter("@length", DBNull.Value);
                            }

                            var sql_update = "exec GramOnline.dbo.proc_Y_AddCoilSlit " +
                                "@coilID, " +
                                "@coilSlitID, " +
                                "@type, " +
                                "@color, " +
                                "@m_color, " +
                                "@s_color, " +
                                "@weight, " +
                                "@gauge, " +
                                "@width, " +
                                "@status, " +
                                "@userID, " +
                                "@length ";

                            try
                            {
                                using (var context = new DbContext(Global.ConnStr))
                                {
                                    context.Database.ExecuteSqlCommand(sql_update,
                                        coilID_sql,
                                        coilSlitID_sql,
                                        type_sql,
                                        color_sql,
                                        m_color_sql,
                                        s_color_sql,
                                        weight_sql,
                                        gauge_sql,
                                        width_sql,
                                        status_sql,
                                        userID_sql,
                                        length_sql);
                                }
                            }
                            catch (Exception e)
                            {
                                slits.errMsg = "SQL Exception: " + e.Message + ";";
                            }
                        }
                    }
                }
            }
            return View(slits);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult CoilFind()
        {
            CoilSlits slit = new CoilSlits();
            return View();
        }

        /*
         This action is for looking up existing slited coils in the GramOnline.dbo.TB_Y_X_COIL_SLIT table, which contains the location of searched coils.
         The staff can use it to quicker find coils by their color or width or even search their particular IDs using keyword search.
             */
        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult CoilFind(CoilSlits slits)
        {
            ViewBag.Title = "Coil Lookup";
            Session["CurrForm"] = "CoilFind";

            if (string.IsNullOrEmpty(slits.sortCol))
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

                slits.sortCol = "DefaultSort";
                slits.sortColType = "Number";
                slits.rowsPerPage = 15;
                slits.pageNum = 1;
                slits.orderBy = "glyphicon glyphicon-arrow-up";
            }

            if (slits.sortCol != "COIL_SLIT_ID" && slits.sortCol != "TYPE" && slits.sortCol != "COLOR" && slits.sortCol != "WEIGHT" && slits.sortCol != "GAUGE"
                && slits.sortCol != "WIDTH" && slits.sortCol != "LENGTH" && slits.sortCol != "DATE_PRODUCED" && slits.sortCol != "DATE_USED" && slits.sortCol != "STATUS"
                && slits.sortCol != "USERID" && slits.sortCol != "SECTION" && slits.sortCol != "RACK" && slits.sortCol != "COLUMNS" && slits.sortCol != "ROW")
            {
                slits.sortCol = "DefaultSort";
            }

            if (String.IsNullOrEmpty(slits.whereStr))
            {
                slits.whereStr = "";
            }

            if (slits.whereStr.Replace(" ", "") == "")
            {
                slits.whereStr = "";
            }

            if (String.IsNullOrEmpty(slits.colorSort))
            {
                slits.colorSort = "";
            }

            var Role = new SqlParameter("@Role", ((Scanner.Models.User)Session["User"]).Role);
            var UserName = new SqlParameter("@UserName", ((Scanner.Models.User)Session["User"]).UserName);
            var pageNum = new SqlParameter("@pageNum", (slits.pageNum == 0) ? 1 : slits.pageNum);
            var rowsPerPage = new SqlParameter("@rowsPerPage", slits.rowsPerPage);
            var sortCol = new SqlParameter("@sortCol", slits.sortCol);
            var sortColType = new SqlParameter("@sortColType", slits.sortColType);
            var whereStr = new SqlParameter("@whereStr", slits.whereStr.ToString());
            var colorSort = new SqlParameter("@colorSort", slits.colorSort.ToString());
            var widthSort = new SqlParameter("@widthSort", slits.widthSort.ToString());

            var orderBy = (slits.orderBy == "glyphicon glyphicon-arrow-down") ?
                new SqlParameter("@orderBy", "desc") :
                new SqlParameter("@orderBy", "asc");


            var table = new SqlParameter("@table", "GramOnline.dbo.TB_Y_X_COIL_SLIT");
            var selStr = new SqlParameter("@selStr", "");

            var sql = "exec GramOnline.dbo.proc_Y_GetCoilSlits " +
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
                "@colorSort, " +
                "@widthSort ";

            var oldMsg = "";

            if (slits.errMsg == null)
                slits.errMsg = "";
            else
                oldMsg = slits.errMsg;

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    slits.slits = context.Database.SqlQuery<CoilSlit>(sql,
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
                       colorSort,
                       widthSort).ToList<CoilSlit>();
                    if (slits.ColorList == null)
                    {
                        var sql_color = "SELECT codes FROM GRAM_SYD_LIVE.dbo.X_COLOR_CHART order by codes asc";
                        slits.ColorList = context.Database.SqlQuery<string>(sql_color).ToList<string>();
                    }
                }
            }
            catch (Exception e)
            {
                slits.totalPages = 0;
                slits.totalRows = 0;
                slits.slits = null;
                slits.errMsg = e.Message.ToString().Replace("'", "\"");
                if (e.Message.Equals("The specified cast from a materialized 'System.String' type to the 'System.Int32' type is not valid."))
                {
                    slits.errMsg = "No Record Found.";
                }
            }

            if (slits.slits != null)
            {
                if (slits.slits.Count > 0)
                {
                    slits.totalPages = slits.slits[0].maxPages;
                    slits.totalRows = slits.slits[0].TotalRows;
                }
            }
            return View(slits);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult CoilMaster()
        {
            CoilMasters master = new CoilMasters();
            return View(master);
        }

        /*
         * This Action is to display all the coil details in a table. Users can click any 'Coil ID' and see the details of that coil.
         * This Action can be accessed by clicking the 'Coil Master' link in the top menu.
         */
        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult CoilMaster(CoilMasters master)
        {
            ViewBag.Title = "Coil Master Table";
            Session["CurrForm"] = "CoilMaster";

            if (string.IsNullOrEmpty(master.sortCol))
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

                master.sortCol = "DefaultSort";
                master.sortColType = "Number";
                master.rowsPerPage = 15;
                master.pageNum = 1;
                master.orderBy = "glyphicon glyphicon-arrow-up";
            }

            if (master.sortCol != "COILID" && master.sortCol != "TYPE" && master.sortCol != "COLOR" && master.sortCol != "WEIGHT" && master.sortCol != "GAUGE" &&
                master.sortCol != "WIDTH" && master.sortCol != "[ORDER]" && master.sortCol != "P_ORDER" && master.sortCol != "MONTH_RECD" && master.sortCol != "DATE_INWH" &&
                master.sortCol != "DATE_TRANSFER" && master.sortCol != "LAST_STOCKTAKE_DATE" && master.sortCol != "STATUS" && master.sortCol != "CLENGTH" && master.sortCol != "ZINCCOAT")
            {
                master.sortCol = "DefaultSort";
            }

            if (String.IsNullOrEmpty(master.whereStr))
            {
                master.whereStr = "";
            }

            if (master.whereStr.Replace(" ", "") == "")
            {
                master.whereStr = "";
            }

            var Role = new SqlParameter("@Role", ((Scanner.Models.User)Session["User"]).Role);
            var UserName = new SqlParameter("@UserName", ((Scanner.Models.User)Session["User"]).UserName);
            var pageNum = new SqlParameter("@pageNum", (master.pageNum == 0) ? 1 : master.pageNum);
            var rowsPerPage = new SqlParameter("@rowsPerPage", master.rowsPerPage);
            var sortCol = new SqlParameter("@sortCol", master.sortCol);
            var sortColType = new SqlParameter("@sortColType", master.sortColType);
            var whereStr = new SqlParameter("@whereStr", master.whereStr.ToString());

            var orderBy = (master.orderBy == "glyphicon glyphicon-arrow-down") ?
                new SqlParameter("@orderBy", "desc") :
                new SqlParameter("@orderBy", "asc");


            var table = new SqlParameter("@table", "GRAM_SYD_LIVE.dbo.X_COIL_MASTER");
            var selStr = new SqlParameter("@selStr", "");


            // sideMenus = context.Database.SqlQuery<SideMenu>("GramOnline.dbo.proc_Y_GetSideMenu_v2").ToList<SideMenu>();
            var sql = "exec GramOnline.dbo.proc_Y_GetCoilMasters " +
                "@Role," +
                "@UserName, " +
                "@pageNum, " +
                "@rowsPerPage, " +
                "@sortCol, " +
                "@sortColType, " +
                "@whereStr, " +
                "@orderBy, " +
                "@table, " +
                "@selStr ";

            var oldMsg = "";

            if (master.errMsg == null)
                master.errMsg = "";
            else
                oldMsg = master.errMsg;

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    master.CoilDetails = context.Database.SqlQuery<CoilMaster>(sql,
                       Role,
                       UserName,
                       pageNum,
                       rowsPerPage,
                       sortCol,
                       sortColType,
                       whereStr,
                       orderBy,
                       table,
                       selStr).ToList<CoilMaster>();
                }
            }
            catch (Exception e)
            {
                master.totalPages = 0;
                master.totalRows = 0;
                master.CoilDetails = null;
                master.errMsg = e.Message.ToString().Replace("'", "\"");
                if (e.Message.Equals("The specified cast from a materialized 'System.String' type to the 'System.Int32' type is not valid."))
                {
                    master.errMsg = "No Record Found.";
                }
            }

            if (master.CoilDetails != null)
            {
                if (master.CoilDetails.Count > 0)
                {
                    master.totalPages = master.CoilDetails[0].maxPages;
                    master.totalRows = master.CoilDetails[0].TotalRows;
                }
            }

            if (oldMsg != "")
                master.errMsg = oldMsg;

            return View(master);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult CoilMasterDetails()
        {
            CoilMasters coil = new CoilMasters();
            return View(coil);
        }

        /*
         * This Action is to display the details of one certain coil. Users can change the value of the details and update the database by clicking the 'Update' button.
         * This Action can be accessed by clicking the 'Coil Master' link in the top menu, followed by clicking any 'Coil ID'.
         */
        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult CoilMasterDetails(CoilMasters coil)
        {
            ViewBag.Title = "Coil Master Details";
            Session["CurrForm"] = "CoilMasterDetails";
            string CoilID = Request.QueryString["ID"];

            var sql = "select * from GRAM_SYD_LIVE.dbo.X_COIL_MASTER where COILID = '" + CoilID + "';"; // to get all information from X_COIL_MASTER table where CoilID matches

            string new_DATE_INWH = "";
            string new_DATE_TRANSFER = "";
            string new_LAST_STOCKTAKE_DATE = "";
            string[] tmpArr;

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    if (coil.CoilDetails != null) // first enter the page the coil.CoilDetails is null.
                    {
                        if (coil.CoilDetails[0].updateFlag == "update")
                        {

                            if (coil.CoilDetails[0].DATE_INWH != null)
                            {
                                tmpArr = coil.CoilDetails[0].DATE_INWH.ToString().Split(' ');
                                new_DATE_INWH = tmpArr[0].Split('/')[2] + "/" + tmpArr[0].Split('/')[1] + "/" + tmpArr[0].Split('/')[0] + " " + tmpArr[1];
                            }
                            if (coil.CoilDetails[0].DATE_TRANSFER != null)
                            {
                                tmpArr = coil.CoilDetails[0].DATE_TRANSFER.ToString().Split(' ');
                                new_DATE_TRANSFER = tmpArr[0].Split('/')[2] + "/" + tmpArr[0].Split('/')[1] + "/" + tmpArr[0].Split('/')[0] + " " + tmpArr[1];
                            }
                            if (coil.CoilDetails[0].LAST_STOCKTAKE_DATE != null)
                            {
                                tmpArr = coil.CoilDetails[0].LAST_STOCKTAKE_DATE.ToString().Split(' ');
                                new_LAST_STOCKTAKE_DATE = tmpArr[0].Split('/')[2] + "/" + tmpArr[0].Split('/')[1] + "/" + tmpArr[0].Split('/')[0] + " " + tmpArr[1];
                            }

                            var sql_update = "update GRAM_SYD_LIVE.dbo.X_COIL_MASTER set " +
                                "TYPE = " + ((coil.CoilDetails[0].TYPE == null) ? "null" : "'" + coil.CoilDetails[0].TYPE + "'") + ", " +
                                "COLOR = " + ((coil.CoilDetails[0].COLOR == null) ? "null" : "'" + coil.CoilDetails[0].COLOR + "'") + ", " +
                                "WEIGHT = " + ((coil.CoilDetails[0].WEIGHT == null) ? "null" : coil.CoilDetails[0].WEIGHT.ToString()) + ", " +
                                "GAUGE = " + ((coil.CoilDetails[0].GAUGE == null) ? "null" : coil.CoilDetails[0].GAUGE.ToString()) + ", " +
                                "WIDTH = " + ((coil.CoilDetails[0].WIDTH == null) ? "null" : coil.CoilDetails[0].WIDTH.ToString()) + ", " +
                                "[ORDER] = " + ((coil.CoilDetails[0].ORDER == null) ? "null" : coil.CoilDetails[0].ORDER.ToString()) + ", " +
                                "P_ORDER = " + ((coil.CoilDetails[0].P_ORDER == null) ? "null" : "'" + coil.CoilDetails[0].P_ORDER + "'") + ", " +
                                "MONTH_RECD = " + ((coil.CoilDetails[0].MONTH_RECD == null) ? "null" : "'" + coil.CoilDetails[0].MONTH_RECD + "'") + ", " +
                                "DATE_INWH = " + ((coil.CoilDetails[0].DATE_INWH == null) ? "null" : "'" + new_DATE_INWH + "'") + ", " +
                                "DATE_TRANSFER = " + ((coil.CoilDetails[0].DATE_TRANSFER == null) ? "null" : "'" + new_DATE_TRANSFER + "'") + ", " +
                                "LAST_STOCKTAKE_DATE = " + ((coil.CoilDetails[0].LAST_STOCKTAKE_DATE == null) ? "null" : "'" + new_LAST_STOCKTAKE_DATE + "'") + ", " +
                                "STATUS = " + ((coil.CoilDetails[0].STATUS == null) ? "null" : "'" + coil.CoilDetails[0].STATUS + "'") + ", " +
                                "CLENGTH = " + ((coil.CoilDetails[0].CLENGTH == null) ? "null" : coil.CoilDetails[0].CLENGTH.ToString()) + ", " +
                                "ZINCCOAT = " + ((coil.CoilDetails[0].ZINCCOAT == null) ? "null" : "'" + coil.CoilDetails[0].ZINCCOAT + "'") +
                                " where COILID = " + "'" + coil.CoilDetails[0].COILID + "';";

                            sql = "select * from GRAM_SYD_LIVE.dbo.X_COIL_MASTER where COILID = '" + coil.CoilDetails[0].COILID + "';";

                            context.Database.ExecuteSqlCommand(sql_update);
                        }
                    }
                    coil.CoilDetails = context.Database.SqlQuery<CoilMaster>(sql).ToList<CoilMaster>();
                }
            }
            catch (Exception e)
            {
                coil.errMsg = "No Record Found: ";
            }

            return View(coil);
        }
    }
}
