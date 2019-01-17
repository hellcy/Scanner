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
        public ActionResult Coil()
        {
            CoilMasters details = new CoilMasters();
            details.CoilDetails = new List<CoilMaster>();
            return View(details);
        }


        /*
         * This Action is to allow users to scan the 'Upload Data' QR code and upload Coil IDs to the database.
         * When the new raw materials (coils) arrive, the staff needs to scan all new coils and upload them to the database.
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

            int coilIDCount = 0;

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
                        var date = DateTime.Now;
                        var UserName = ((Scanner.Models.User)Session["User"]).FirstName + " " + ((Scanner.Models.User)Session["User"]).LastName;
                        SqlCommand newCmd2 = new SqlCommand(("IF NOT EXISTS (SELECT * FROM GramOnline.dbo.TB_Y_X_COIL_TEST WHERE COILID = '" + coilIDs[i] + "') BEGIN INSERT INTO GramOnline.dbo.TB_Y_X_COIL_TEST (COILID, TYPE, COLOR, WEIGHT, GAUGE, WIDTH, DATE_INSERT, STATUS, UserName) VALUES ('" + coilIDs[i] + "' , '" + model.CoilDetails[i + 1].TYPE + "' , '" + model.CoilDetails[i + 1].COLOR + "' , " + model.CoilDetails[i + 1].WEIGHT + " , " + model.CoilDetails[i + 1].GAUGE + " , " + model.CoilDetails[i + 1].WIDTH + " , GETDATE(), 'N', '" + UserName + "') END"), newCon);
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
                    }
                }
            }
            return View(model);
        }

        [SessionExpire]
        [Authorize]
        public ActionResult Sliting()
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
        public ActionResult Sliting(CoilSlits slits)
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
                int uniqueLocation = 0;
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
                        slits.slits[i - 1].UNIQUELOCATION = uniqueLocation;
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
                            var uniqueLocation_sql = new SqlParameter("@uniqueLocation", slits.slits[i].UNIQUELOCATION);
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
                                "@length, " +
                                "@uniqueLocation ";

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
                                        length_sql,
                                        uniqueLocation_sql);
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
    }
}
