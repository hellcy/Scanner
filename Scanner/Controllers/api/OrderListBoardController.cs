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

namespace Scanner.Controllers.api
{
    public class OrderListBoardController : ApiController
    {
        // GET api/<controller>


        public IList<OrderHeads> Get()
        {

            OrderHeads orderHs = new OrderHeads();
            IList<OrderHeads> rtn = new List<OrderHeads>();
            orderHs.sortCol = "DefaultSort";
            orderHs.sortColType = "String";
            orderHs.rowsPerPage = 15;
            orderHs.pageNum = 1;
            orderHs.orderBy = "glyphicon glyphicon-arrow-up";


            if (String.IsNullOrEmpty(orderHs.whereStr))
            {
                orderHs.whereStr = "";
            }

            if (orderHs.whereStr.Replace(" ", "") == "")
            {
                orderHs.whereStr = "";
            }


            var Role = new SqlParameter("@Role", "Gram Admin");
            var UserName = new SqlParameter("@UserName", "scottz@gram.com.au");
            var pageNum = new SqlParameter("@pageNum", (orderHs.pageNum == 0) ? 1 : orderHs.pageNum);
            var rowsPerPage = new SqlParameter("@rowsPerPage", orderHs.rowsPerPage);
            var sortCol = new SqlParameter("@sortCol", orderHs.sortCol);
            var sortColType = new SqlParameter("@sortColType", orderHs.sortColType);
            var whereStr = new SqlParameter("@whereStr", orderHs.whereStr.ToString());
            var flag = new SqlParameter("@flag", 3);

            var orderBy = (orderHs.orderBy == "glyphicon glyphicon-arrow-down") ?
                new SqlParameter("@orderBy", "desc") :
                new SqlParameter("@orderBy", "asc");


            var table = new SqlParameter("@table", "dbo.View_Orders");
            var selStr = new SqlParameter("@selStr", "");

            var sql = "exec dbo.proc_GetOrders_v1 " +
                "@Role," +
                "@UserName, " +
                "@pageNum, " +
                "@rowsPerPage, " +
                "@sortCol, " +
                "@sortColType, " +
                "@whereStr, " +
                "@orderBy, " +
                "@table, " +
                "@selStr," +
                "@flag";

            var oldMsg = "";

            if (orderHs.errMsg == null)
                orderHs.errMsg = "";
            else
                oldMsg = orderHs.errMsg;

            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    orderHs.orderHeads = context.Database.SqlQuery<OrderHead2>(sql,
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
                       flag).ToList<OrderHead2>();
                }
            }
            catch (Exception e)
            {
                orderHs.totalPages = 0;
                orderHs.totalRows = 0;
                orderHs.orderHeads = null;
                orderHs.errMsg = "No Record Found";
            }

            if (orderHs.orderHeads != null)
            {
                if (orderHs.orderHeads.Count > 0)
                {
                    orderHs.totalPages = orderHs.orderHeads[0].maxPages;
                    orderHs.totalRows = orderHs.orderHeads[0].TotalRows;
                }
            }

            if (oldMsg != "")
                orderHs.errMsg = oldMsg;

            rtn.Add(orderHs);
            return rtn;

        }
    }
}