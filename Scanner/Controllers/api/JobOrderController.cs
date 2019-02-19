using Scanner.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;


namespace Scanner.Controllers.api
{
    public class JobOrderController : ApiController
    {
        // GET: api/JobOrder
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        // GET: api/JobOrder/5
        public JObject Get(int HDR_SEQNO)
        {
            
            IList<WorkOrder_Line> WorkOrder_Lines = new List<WorkOrder_Line>();
            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    object[] parameters = { HDR_SEQNO };
                    WorkOrder_Lines = context.Database.SqlQuery<WorkOrder_Line>("GramOnline.dbo.proc_Y_App_GetJobOrder {0}", parameters).ToList<WorkOrder_Line>();
                }
            }
            catch (Exception e)
            {
                WorkOrder_Lines = null;
            }
            var json = JsonConvert.SerializeObject(new { results = WorkOrder_Lines });
            var jObject = JObject.Parse(json);

            //return WorkOrder_Lines;
            return jObject;
        }

        // POST: api/JobOrder
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/JobOrder/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/JobOrder/5
        public void Delete(int id)
        {
        }
    }
}
