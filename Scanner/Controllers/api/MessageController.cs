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
    public class MessageController : ApiController
    {
        // GET api/<controller>


        public IEnumerable<string> Get()
        {
            string data = "";
            string sql = "select Message from dbo.TB_URGENT_MESSAGE where Id = 1";
            using (var context = new DbContext(Global.ConnStr))
            {
                data = context.Database.SqlQuery<string>(sql).ToList<string>()[0];
            }
            return new string[] { data };
        }
    }
}