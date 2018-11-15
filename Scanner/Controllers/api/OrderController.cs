using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Scanner.Models;
using Newtonsoft.Json;
using System.Web;
using System.Web.Configuration;

namespace Scanner.Controllers.api
{
    public class OrderController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public string Post(Order order)
        {
            string orderNo = "-1";
            try
            {
                fillPrice(order);
                var CustCompName = new SqlParameter("@CustCompName", order.Company);
                var ContactName = new SqlParameter("@ContactName", order.OrderBy);
                var CustMobile = new SqlParameter("@CustMobile", order.Mobile);
                var BranchId = new SqlParameter("@BranchId", order.BranchIDDealWith);
                var CustomerAccNo = new SqlParameter("@CustomerAccNo", order.ACCNO);
                var PickUp = (order.RequestForDelivery == "Y") ? new SqlParameter("@PickUp", "0") : new SqlParameter("@PickUp", "1");
                var PoNum = new SqlParameter("@PoNum", order.OrderNo);
                var TotalWeight = new SqlParameter("@TotalWeight", order.TotalWeight);
                var TotalPrice = new SqlParameter("@TotalPrice", order.TotalPrice);
                //var ORDERNO = new SqlParameter("@ORDERNO", SqlDbType.Int);
                //ORDERNO.Direction = ParameterDirection.Output;

                var sql = "exec dbo.X_PROC_INSERT_ORDER " +
                 "@CustCompName," +
                 "@ContactName," +
                 "@CustMobile," +
                 "@BranchId," +
                 "@CustomerAccNo," +
                 "@PickUp," +
                 "@PoNum," +
                 "@TotalWeight," +
                 "@TotalPrice";

                using (var context = new DbContext(Global.ConnStr))
                {
                    var data = context.Database.SqlQuery<decimal>(sql,
                        CustCompName,
                        ContactName,
                        CustMobile,
                        BranchId,
                        CustomerAccNo,
                        PickUp,
                        PoNum,
                        TotalWeight,
                        TotalPrice).ToList<decimal>()[0];
                    orderNo = data.ToString();
                }

                foreach (OrderDetail orderDetail in order.OrderDetails)
                {
                    using (var context = new DbContext(Global.ConnStr))
                    {
                        object[] parameters = {
                                orderNo,
                                "GramLine",
                                orderDetail.COLOUR,
                                orderDetail.STANDARD,
                                orderDetail.STOCKCODE,
                                orderDetail.DESCRIPTION,
                                orderDetail.WEIGHT,
                                orderDetail.PRICE,
                                orderDetail.QTY
                            };
                        context.Database.ExecuteSqlCommand("X_PROC_INSERT_ORDERDETAILS {0},{1},{2},{3},{4},{5},{6},{7},{8}", parameters);
                    }
                }

                using (var context = new DbContext(Global.ConnStr))
                {

                    object[] parameters = {
                               orderNo,
                               order.ACCNO
                            };
                    context.Database.ExecuteSqlCommand("proc_SendOrderNotification_v1 {0},{1}", parameters);
                }
            }
            catch (Exception e)
            {
                orderNo = "-1";
            }

            if (orderNo == "-1")
            {
                using (var context = new DbContext(Global.ConnStr))
                {

                    object[] parameters = {
                                "scottz@gram.com.au",
                                "Error: Order "+order.OrderNo+ "(Imcompleted)",
                                "Company: " +order.Company+ "<br>Order By: "+order.OrderBy+"<br>Mobile "+ order.Mobile,
                            };
                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2}", parameters);
                }
            }

            return orderNo;
        }

        private void fillPrice(Order order)
        {
            double totalWeight = 0;
            double totalPrice = 0;

            if (order.ACCNO == "") order.ACCNO = "0";
            foreach (var orderDetail in order.OrderDetails)
            {
                totalWeight += Convert.ToDouble(orderDetail.WEIGHT);
                var STOCKCODE = new SqlParameter("@STOCKCODE", orderDetail.STOCKCODE);
                var ACCNO = new SqlParameter("@ACCNO", order.ACCNO);
                var QTY = new SqlParameter("@QTY", orderDetail.QTY);
                var TRANSDATE = new SqlParameter("@TRANSDATE", DateTime.Now);
                var BEST_FLAG = new SqlParameter("@BEST_FLAG", 'Y');
                var SPECIAL_LEN = new SqlParameter("@SPECIAL_LEN", -1);
                if (orderDetail.STOCKCODE[4].ToString() + orderDetail.STOCKCODE[5].ToString() == "SL")
                {
                    SPECIAL_LEN = new SqlParameter("@SPECIAL_LEN", orderDetail.STANDARD);
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
                          DISCOUNTRATE);

                    var item = data.Single<Price>();
                    totalPrice += item.BEST_PRICE;
                    orderDetail.PRICE = item.BEST_PRICE.ToString();
                    order.TotalWeight += orderDetail.WEIGHT;
                }
            }
            order.TotalWeight = totalWeight.ToString();
            order.TotalPrice = totalPrice.ToString();
        }


        // POST api/<controller>
        // public string Post([FromBody]string value)
        public string Post(string request)
        {
            string ok = "no";
            string staffName = request.Split(' ')[0];
            string staffPass = request.Split(' ')[1];

            if (staffPass == "fence123")
            {
                string data = "";
                using (var context = new DbContext(Global.ConnStr))
                {
                    object[] parameters = {
                            staffName
                        };

                    data = context.Database.SqlQuery<string>("proc_GetStaffID {0}", parameters).ToList<string>()[0];
                }

                if (data != "-1")
                {
                    HttpContext.Current.Session["HandledBy"] = data;
                    ok = "ok";
                }
            }

            return ok;
        }


        public string UpLoad(string value)
        {
            string aaa = value;
            return aaa;
        }


        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public string Delete(decimal idx)
        {
            //var gramline = HttpSessionStateBase["GramLine"];
            var session = HttpContext.Current.Session;
            string msg = "";
            string ItemType = "";
            int C_X = 0;
            int C_Y = 0;
            int remvId = -1;

            if (session != null)
            {
                NewOrder order = (NewOrder)session["newOrder"];

                for (var i = 0; i < order.OrderDetails.Count; i++)
                {
                    if (order.OrderDetails[i].Idx == idx)
                    {
                        remvId = i;
                        break;
                    }
                }

                if (remvId > -1)
                {
                    ItemType = order.OrderDetails[remvId].ITEMTYPE;
                    C_X = order.OrderDetails[remvId].C_X ?? -1;
                    C_Y = order.OrderDetails[remvId].C_Y ?? -1;
                    if (C_X > -1)
                    {
                        DataTable DT = (DataTable)session[ItemType];
                        if (ItemType == "SmartSlat")
                        {
                            if ((C_X == 2) ||
                                (C_X == 4) ||
                                (C_X == 7) ||
                                (C_X == 9) ||
                                (C_X == 12) ||
                                (C_X == 14))
                            {
                                DT.Rows[C_Y][C_X - 1] = "";
                            }
                        }

                        if (ItemType == "SmartSlatAng")
                        {
                            if ((C_X == 2) ||
                                (C_X == 4) ||
                                (C_X == 6) ||
                                (C_X == 8) ||
                                (C_X == 10))
                            {
                                DT.Rows[C_Y][C_X - 1] = "";
                            }
                        }


                        if (ItemType == "SmartSlatLBSO")
                        {
                            if ((C_X == 2) ||
                                (C_X == 4))
                            {
                                DT.Rows[C_Y][C_X - 1] = "";
                            }
                        }


                        if (ItemType == "Fasteners")
                        {
                            DT.Rows[C_Y][C_X - 1] = "";
                        }

                        if (ItemType == "GramSlat")
                        {
                            DT.Rows[C_Y][C_X + 1] = "";
                        }

                        if (idx > Convert.ToDecimal(Convert.ToInt32(idx)))
                        {
                            if (idx == Convert.ToDecimal(0.1))
                            {
                                order.OrderDetails[0].Cuts = order.OrderDetails[remvId].Cuts;
                                order.OrderDetails[0].CutDesc = order.OrderDetails[remvId].CutDesc;
                            }
                            else
                            {
                                order.OrderDetails[remvId - 1].Cuts = order.OrderDetails[remvId].Cuts;
                                order.OrderDetails[remvId - 1].CutDesc = order.OrderDetails[remvId].CutDesc;
                            }

                            DT.Rows[C_Y][C_X] = Convert.ToInt32(DT.Rows[C_Y][C_X]) - Convert.ToInt32(order.OrderDetails[remvId].QTY);

                        }
                        else
                        {
                            if (remvId < order.OrderDetails.Count - 1)
                            {
                                if (order.OrderDetails[remvId + 1].Idx > Convert.ToDecimal(Convert.ToInt32(order.OrderDetails[remvId + 1].Idx)))
                                {
                                    DT.Rows[C_Y][C_X] = order.OrderDetails[remvId + 1].QTY;
                                }
                                else
                                {
                                    DT.Rows[C_Y][C_X] = "";
                                }
                            }
                            else
                            {
                                DT.Rows[C_Y][C_X] = "";
                            }
                        }

                        order.OrderDetails.RemoveAt(remvId);
                    }

                    session[ItemType + "AllCuts"] = null;
                    string allCuts = "";
                    if (order.OrderDetails.Count > 0)
                    {
                        foreach (var item in order.OrderDetails)
                        {
                            if (item.Cuts > 0)
                            {
                                allCuts += item.COLOUR + "|~|" + item.STANDARD + "|~|" + item.Cuts.ToString() + "|~|" + item.CutDesc + "|_|";
                            }
                        }
                    }

                    if (allCuts != "")
                    {
                        session[ItemType + "AllCuts"] = allCuts;
                    }

                    return "Item has been deleted";
                }
            }

            return "Error: Delete Item is not Existing.";
        }
    }
}