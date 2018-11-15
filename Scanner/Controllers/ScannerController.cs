using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Configuration;
using Scanner.Models;
using System.Text;

namespace Scanner.Controllers
{
    public class ScannerController : Controller
    {
        CoilDetail details = new CoilDetail();

        // GET: /Scanner/ 

        public ActionResult Index()
        {
            return View(details);
        }

        [HttpPost]
        public ActionResult Index(CoilDetail model)
        {
            string ConnStr = ConfigurationManager.ConnectionStrings["GramLineConn"].ToString();

            string input;

            if (model.CoilDetails[0].Flag == "UPLOAD")
                input = model.CoilDetails[0].Save2;
            else
                input = model.CoilDetails[0].Input;

            if (input == null)
            {
                return View(model);
            }

            List<string> coilIDs = new List<string>();

            int splitAt = 33; // change 9 with the size of strings you want.
            int coilIDCount = 0;
            for (int i = 0; i < input.Length; i = i + splitAt)
            {
                coilIDCount++;
                if (input.Length - i >= splitAt)
                    coilIDs.Add(input.Substring(i, splitAt - 24));
                else if (input.Length - i >= 9)
                    coilIDs.Add(input.Substring(i, 9));
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
