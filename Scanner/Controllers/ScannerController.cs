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

namespace Scanner.Controllers
{
    public class ScannerController : Controller
    {

        CoilDetail details = new CoilDetail();

        // change the lengths here for different products!
        public const int CoilQRcodeLength = 33;
        public const int CoilIDLength = 9;

        // GET: /Scanner/ 

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult WorkOrderLines()
        {
            WorkOrder_Lines lines = new WorkOrder_Lines();
            return View(lines);
        }

        [HttpPost]
        public ActionResult WorkOrderLines(WorkOrder_Lines lines)
        {

            return View(lines);
        }

        public ActionResult WorkOrder()
        {
            WorkOrder_HDRs orders = new WorkOrder_HDRs();
            return View(orders);
        }

        [HttpPost]
        public ActionResult WorkOrder(WorkOrder_HDRs orders)
        {
            ViewBag.Title = "Work Order";
            Session["CurrForm"] = "WorkOrder";

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

        public ActionResult Product2()
        {
            CoilDetail details = new CoilDetail();
            return View(details);
        }

        public ActionResult Product3()
        {
            CoilDetail details = new CoilDetail();
            return View(details);
        }

        public ActionResult Coil()
        {
            CoilDetail details = new CoilDetail();
            return View(details);
        }

        [HttpPost]
        public ActionResult Coil(CoilDetail model)
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
