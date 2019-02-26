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
using System.Web.Script.Serialization;

namespace Scanner.Controllers.api
{
    public class PurchaseOrderController : ApiController
    {
        // GET: api/JobOrder
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2", "purchase" };
        }

        // GET: api/JobOrder/5
        public JObject Get(int HDR_SEQNO)
        {

            IList<ReceivedOrder> results = new List<ReceivedOrder>();
            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    object[] parameters = { HDR_SEQNO };
                    results = context.Database.SqlQuery<ReceivedOrder>("GramOnline.dbo.proc_Y_App_GetPurchaseOrder {0}", parameters).ToList<ReceivedOrder>();
                }
            }
            catch (Exception e)
            {
                results = null;
            }
            var json = JsonConvert.SerializeObject(new { results = results });
            var jObject = JObject.Parse(json);

            return jObject;
        }

        // POST: api/JobOrder

        public JObject Post(JObject results)
        {
            JsonSerializer serializer = new JsonSerializer();
            ReceivedOrders receivedOrders = (ReceivedOrders)serializer.Deserialize(new JTokenReader(results), typeof(ReceivedOrders));
            int size = 0;
            // Works to be done: Now receivedOrders contains all the data collected -------------------------------------------------------------------
            // Get all the object members and pass them to a procedure as parameters
            // using a for loop to call that procedure and update a newly created table in SQL database
            foreach (ReceivedOrder order in receivedOrders.results)
            {
                size++;
            }


            return results;
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
