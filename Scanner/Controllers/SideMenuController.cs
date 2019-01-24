﻿using System;
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
using Zen.Barcode;
using System.Drawing;
using System.Data.OleDb;
using OfficeOpenXml;
using CsvHelper;
using System.Configuration;

namespace Scanner.Controllers
{
    public class SideMenuController : BaseController
    {
        CoilMasters details = new CoilMasters();

        // used at WorkOrderLines to decode System.Drawing image
        private byte[] turnImageToByteArray(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }

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

                }
            }
            Session["CurrForm"] = "Home";
            return View();
        }


        public ActionResult PrintOrder()
        {
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
        public ActionResult CoilMaster()
        {
            CoilMasters master = new CoilMasters();
            return View(master);
        }

        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult CoilMaster(CoilMasters master)
        {
            ViewBag.Title = "Coil Master Table";
            Session["CurrForm"] = "Option_1";

            // read .xlsx or .csv file
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];

                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName) && master.openFileFlag == "open")
                {
                    string fileName = file.FileName;
                    master.excelFileName = fileName;
                    string extension = System.IO.Path.GetExtension(fileName).ToLower();
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    if (extension.Equals(".xlsx"))
                    {
                        var package = new ExcelPackage(file.InputStream);

                        ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

                        for (int i = workSheet.Dimension.Start.Row;
                                 i <= workSheet.Dimension.End.Row;
                                 i++)
                        {
                            master.excelRecords.Add(new List<string>());
                            object firstColumnCell = null;
                            for (int j = workSheet.Dimension.Start.Column;
                                     j <= workSheet.Dimension.End.Column;
                                     j++)
                            {
                                firstColumnCell = workSheet.Cells[i, 1].Value;
                                object cellValue = workSheet.Cells[i, j].Value;
                                master.excelRecords[i - 1].Add(Convert.ToString(cellValue));
                            }
                            master.excelCoilIDs.Add(Convert.ToString(firstColumnCell));
                        }
                    }
                    else if (extension.Equals(".csv"))
                    {
                        Guid gid = Guid.NewGuid();
                        file.SaveAs(Server.MapPath(@"~\TmpFiles\" + gid.ToString() + ".csv"));

                        string[] lineArr;
                        if ((System.IO.File.Exists(Server.MapPath(@"~\TmpFiles\" + gid.ToString() + ".csv"))) == true)
                        {
                            foreach (string line in System.IO.File.ReadLines(Server.MapPath(@"~\TmpFiles\" + gid.ToString() + ".csv")))
                            {
                                lineArr = line.Split(',');
                                master.excelRecords.Add(lineArr.ToList());
                                master.excelCoilIDs.Add(lineArr[0]);
                            }
                            System.IO.File.Delete(Server.MapPath(@"~\TmpFiles\" + gid.ToString() + ".csv"));
                        }
                    }

                    Session["readExcelFile"] = master;
                    return RedirectToAction("CoilReadFile");

                }
            }

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
                master.sortCol != "DATE_TRANSFER" && master.sortCol != "LAST_STOCKTAKE_DATE" && master.sortCol != "STATUS" && master.sortCol != "CLENGTH" && master.sortCol != "ZINCCOAT" &&
                master.sortCol != "LOCATION" && master.sortCol != "INV_NO" && master.sortCol != "INV_DATE" && master.sortCol != "UNITPRICE")
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
            string new_INV_DATE = "";
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
                            if (coil.CoilDetails[0].INV_DATE != null)
                            {
                                tmpArr = coil.CoilDetails[0].INV_DATE.ToString().Split(' ');
                                new_INV_DATE = tmpArr[0].Split('/')[2] + "/" + tmpArr[0].Split('/')[1] + "/" + tmpArr[0].Split('/')[0] + " " + tmpArr[1];
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
                                "ZINCCOAT = " + ((coil.CoilDetails[0].ZINCCOAT == null) ? "null" : "'" + coil.CoilDetails[0].ZINCCOAT + "'") + ", " +
                                "LOCATION = " + ((coil.CoilDetails[0].LOCATION == null) ? "null" : "'" + coil.CoilDetails[0].LOCATION + "'") + ", " +
                                "INV_NO = " + ((coil.CoilDetails[0].INV_NO == null) ? "null" : "'" + coil.CoilDetails[0].INV_NO + "'") + ", " +
                                "INV_DATE = " + ((coil.CoilDetails[0].INV_DATE == null) ? "null" : "'" + new_INV_DATE + "'") + ", " +
                                "UNITPRICE = " + ((coil.CoilDetails[0].UNITPRICE == null) ? "null" : coil.CoilDetails[0].UNITPRICE.ToString()) +
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

        [SessionExpire]
        [Authorize]
        public ActionResult CoilReadFile()
        {
            CoilMasters master = new CoilMasters();

            if (Session["readExcelFile"] != null) {
                master = (CoilMasters) Session["readExcelFile"];
                //Session["readExcelFile"] = null;
            }

            string ConnStr = ConfigurationManager.ConnectionStrings["GramLineConn"].ToString();
            using (SqlConnection newCon = new SqlConnection(ConnStr))
            {
                string notFound;
                for (int i = 0; i < master.excelCoilIDs.Count; i++)
                {
                    var excelCoilIDs = new SqlParameter("@excelCoilIDs_sql", master.excelCoilIDs[i]);
                    SqlCommand sqlCommand = new SqlCommand("select count(COILID) FROM GRAM_SYD_LIVE.dbo.X_COIL_MASTER where COILID = '" + master.excelCoilIDs[i] + "'", newCon);
                    newCon.Open();

                    notFound = Convert.ToString(sqlCommand.ExecuteScalar());
                    newCon.Close();

                    master.excelRecords[i].Add(notFound);
                }
            }
            
            return View(master);
        }

        /*

             */
        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult CoilReadFile(CoilMasters master)
        {
            if (master.updateFlag == "update")
            {
                if (Session["readExcelFile"] != null)
                {
                    master = (CoilMasters)Session["readExcelFile"];
                    //Session["readExcelFile"] = null;
                }
                for (int i = 0; i < master.excelCoilIDs.Count; i++)
                {
                    var excelCoilIDs = new SqlParameter("@excelCoilIDs_sql", master.excelCoilIDs[i]);

                    var excel_sql = "update GRAM_SYD_LIVE.dbo.X_COIL_MASTER set status = 'D' where COILID = '" + master.excelCoilIDs[i] + "'";

                    try
                    {
                        using (var context = new DbContext(Global.ConnStr))
                        {
                            context.Database.ExecuteSqlCommand(excel_sql);
                        }
                        master.message = "Database updated!";
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
                }
            }
            return View(master);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult CoilSlit()
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
        public ActionResult CoilSlit(CoilSlits slits)
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