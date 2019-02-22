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
    public class JobOrderController : ApiController
    {
        // GET: api/JobOrder
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2", "value3" };
        }
        
        // GET: api/JobOrder/5
        public JObject Get(int HDR_SEQNO)
        {
            
            IList<CollectedOrder> results = new List<CollectedOrder>();
            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    object[] parameters = { HDR_SEQNO };
                    results = context.Database.SqlQuery<CollectedOrder>("GramOnline.dbo.proc_Y_App_GetJobOrder {0}", parameters).ToList<CollectedOrder>();
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
        
        public string Post(JObject results)
        {
            JsonSerializer serializer = new JsonSerializer();
            CollectedOrders collectedOrders = (CollectedOrders)serializer.Deserialize(new JTokenReader(results), typeof(CollectedOrders));
            int size = 0;
            // Works to be done: Now collectedOrders contains all the data collected after dispatch-------------------------------------------------------------------
            // Get all the object members and pass them to a procedure as parameters
            // using a for loop to call that procedure and update a newly created table in SQL database
            foreach (CollectedOrder order in collectedOrders.results)
            {
                size++;
            }


            return "success size: " + size + " " + collectedOrders.results[0].DESCRIPTION + " " + collectedOrders.results[0].QTYCollected;
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
