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

            string SeqNo = Request.QueryString["ID"];

            var sql = "select * from dbo.WORKSORD_HDR where SEQNO = '" + SeqNo + "'"; // to get all information of the SeqNo of a workOrderHeader
            var sql_2 = "select * from dbo.WORKSORD_LINES where HDR_SEQNO = '" + SeqNo + "'"; // to get all workOrderLines under the header SeqNo
            var sql_3 = "delete from dbo.WORKSORD_LINES where HDR_SEQNO = '" + lines.workOrder_HDR.SEQNO + "'"; // delete old data from lines table
            var sql_4 = "delete from dbo.WORKSORD_HDR where SEQNO = '" + lines.workOrder_HDR.SEQNO + "'"; // delete old data from headers table

            var sql_6 = "insert into dbo.WORKSORD_HDR (SEQNO, BILLCODE, PRODCODE, BATCHCODE, TRANSDATE, PRODDATE, DUEDATE, ORDSTATUS, SALESORDNO, NOTES, PRODQTY, ACTUALQTY, PRODLOCNO, REFERENCE, STAFFNO, COMPONENTLOCNO, EXPIRY_DATE, X_BR_ORDER, X_BR_ACCNO, X_BR_INVNO, X_BR) values('" +
                lines.workOrder_HDR.SEQNO + "', '" +
                lines.workOrder_HDR.BILLCODE + "', '" +
                lines.workOrder_HDR.PRODCODE + "', '" +
                lines.workOrder_HDR.BATCHCODE + "', '" +
                lines.workOrder_HDR.TRANSDATE + "', '" +
                lines.workOrder_HDR.PRODDATE + "', '" +
                lines.workOrder_HDR.DUEDATE + "', '" +
                lines.workOrder_HDR.ORDSTATUS + "', '" +
                lines.workOrder_HDR.SALESORDNO + "', '" +
                lines.workOrder_HDR.NOTES + "', '" +
                lines.workOrder_HDR.PRODQTY + "', '" +
                lines.workOrder_HDR.ACTUALQTY + "', '" +
                lines.workOrder_HDR.PRODLOCNO + "', '" +
                lines.workOrder_HDR.REFERENCE + "', '" +
                lines.workOrder_HDR.STAFFNO + "', '" +
                lines.workOrder_HDR.COMPONENTLOCNO + "', '" +
                lines.workOrder_HDR.EXPIRY_DATE + "', '" +
                lines.workOrder_HDR.X_BR_ORDER + "', '" +
                lines.workOrder_HDR.X_BR_ACCNO + "', '" +
                lines.workOrder_HDR.X_BR_INVNO + "', '" +
                lines.workOrder_HDR.X_BR + "')";

            WorkOrder_HDRs headerDetails = new WorkOrder_HDRs();

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    if (lines.updateFlag == "update")
                    {
                        sql = "select * from dbo.WORKSORD_HDR where SEQNO = '" + lines.workOrder_HDR.SEQNO + "'";
                        sql_2 = "select * from dbo.WORKSORD_LINES where HDR_SEQNO = '" + lines.workOrder_HDR.SEQNO + "'";

                        context.Database.ExecuteSqlCommand(sql_3);
                        context.Database.ExecuteSqlCommand(sql_4);
                        if (lines.workOrder_Lines != null)
                        {
                            for (int i = 0; i < lines.workOrder_Lines.Count; i++)
                            {
                                var sql_5 = "insert into dbo.WORKSORD_LINES (SEQNO, HDR_SEQNO, STOCKCODE, DESCRIPTION, QTYREQD, QTYUSED, BATCHCODE, X_LENGTH, X_COLOR) values('" +
                                    lines.workOrder_Lines[i].SEQNO + "', '" +
                                    lines.workOrder_Lines[i].HDR_SEQNO + "', '" +
                                    lines.workOrder_Lines[i].STOCKCODE + "', '" +
                                    lines.workOrder_Lines[i].DESCRIPTION + "', '" +
                                    lines.workOrder_Lines[i].QTYREQD + "', '" +
                                    lines.workOrder_Lines[i].QTYUSED + "', '" +
                                    lines.workOrder_Lines[i].BATCHCODE + "', '" +
                                    lines.workOrder_Lines[i].X_LENGTH + "', '" +
                                    lines.workOrder_Lines[i].X_COLOR + "')";
                                context.Database.ExecuteSqlCommand(sql_5);
                            }
                        }
                        context.Database.ExecuteSqlCommand(sql_6);
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

            return View(lines);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult WorkOrderHeaders()
        {
            WorkOrder_HDRs orders = new WorkOrder_HDRs();
            return View(orders);
        }

        [SessionExpire]
        [HttpPost]
        [Authorize]
        public ActionResult WorkOrderHeaders(WorkOrder_HDRs orders)
        {
            ViewBag.Title = "Work Order Headers";
            Session["CurrForm"] = "WorkOrderHeaders";

            var sql = "exec dbo.proc_GetWorkOrders ";

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    orders.workOrder_HDRs = context.Database.SqlQuery<WorkOrder_HDR>(sql).ToList<WorkOrder_HDR>();
                }
            }
            catch (Exception e)
            {
                orders.errMsg = "No Record Found.";
            }

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
                        //SqlCommand newCmd2 = new SqlCommand(("IF NOT EXISTS (SELECT * FROM X_COIL_TEST WHERE COILID = '" + coilIDs[i] + "') BEGIN INSERT INTO X_COIL_TEST SELECT * FROM X_COIL_MASTER WHERE COILID = ('" + coilIDs[i] + "') END"), newCon);
                        var date = DateTime.Now;
                        SqlCommand newCmd2 = new SqlCommand(("IF NOT EXISTS (SELECT * FROM X_COIL_TEST WHERE COILID = '" + coilIDs[i] + "') BEGIN INSERT INTO X_COIL_TEST (COILID, DATE_INSERT) VALUES ('" + coilIDs[i] + "', GETDATE()) END"), newCon);
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
                        SqlCommand newCmd = new SqlCommand(("select * from X_COIL_MASTER where COILID = '" + coilIDs[i] + "'"), newCon);
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
