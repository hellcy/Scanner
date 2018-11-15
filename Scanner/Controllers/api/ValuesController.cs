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
using System.Net.Http.Headers;
using System.Web.Configuration;
//using System.Web;

namespace Scanner.Controllers.api
{
    public class ValuesController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            // var session = HttpContext.Current.Session;
            return new string[] { "value1", "value2" };
        }


        //public HttpResponseMessage Get(string req)
        //{
        //    string[] splits = req.Split('~');            

        //    string Msg = "close";     
        //    using (var context = new DbContext(Global.ConnStr))
        //    {
        //        object[] parameters = { splits[0], splits[1] };
        //        Msg = context.Database.SqlQuery<string>("proc_SetOrderHandleBy {0},{1}", parameters).ToList<string>()[0];
        //    }

        //    var response = new HttpResponseMessage();

        //    if (Msg == "redirect")
        //    {
        //        if (WebConfigurationManager.AppSettings["pubDir"] == "")
        //        {
        //            response.Content = new StringContent("<html><body><form action = '/Order/ReloadOrder/" + splits[1] + " id = 'frmMain' method = 'post'></form></body><script language='javascript'>window.close();</script></html>");
        //        }
        //        else
        //        {
        //            response.Content = new StringContent("<html><body><form action = '/"+ WebConfigurationManager.AppSettings["pubDir"] + "/Order/ReloadOrder/" + splits[1] + " id = 'frmMain' method = 'post'></form></body><script language='javascript'>window.close();</script></html>");
        //        }
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
        //    }
        //    else
        //    {
        //        response.Content = new StringContent("<html><script language='javascript'>window.close();</script><body></body></html>");
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
        //    }
        //    return response;
        //}


        public HttpResponseMessage Get(string req)
        {
            string[] splits = req.Split('~');

            string Msg = "close";
            using (var context = new DbContext(Global.ConnStr))
            {
                object[] parameters = { splits[0], splits[1] };
                Msg = context.Database.SqlQuery<string>("proc_SetOrderHandleBy {0},{1}", parameters).ToList<string>()[0];
            }

            var response = new HttpResponseMessage();

            //if (Msg == "redirect")
            //{
            //    if (WebConfigurationManager.AppSettings["pubDir"] == "")
            //    {
            //        response.Content = new StringContent("<html><body><form action = '/Order/ReloadOrder/" + splits[1] + " id = 'frmMain' method = 'post'></form></body><script type='text/javascript'>window.open('','_self');window.close();</script></html>");
            //    }
            //    else
            //    {
            //        response.Content = new StringContent("<html><body><form action = '/" + WebConfigurationManager.AppSettings["pubDir"] + "/Order/ReloadOrder/" + splits[1] + " id = 'frmMain' method = 'post'></form></body><script language='javascript'>window.open('','_self');window.close();</script></html>");
            //    }              
            //}
            //else
            //{
            //    response.Content = new StringContent("<html><script type='text/javascript'>window.open('','_self');window.close();</script><body>If this page did not close automatically, Please Close this page</body></html>");     
            //}


            response.Content = new StringContent("<html><script type='text/javascript'>window.open('','_self');window.close();</script><body>If this page did not close automatically, Please close this page by manually, thanks.</body></html>");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }


        public IList<ItemDescription> Post(IList<ItemDescription> itemDescs)
        {
            string description = "";
            string tmpStr = "";
            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    foreach (var itemDesc in itemDescs)
                    {
                        description = getItemDescirption(itemDesc.TYPE, itemDesc.LENGTH, itemDesc.COLOUR);

                        if (description.IndexOf("needs adv") > -1)
                        {
                            itemDesc.STOCKCODE = "";
                            itemDesc.DESCRIPTION = description;
                        }
                        else
                        {
                            tmpStr = description.Substring(0, description.IndexOf(";"));
                            itemDesc.DESCRIPTION = description.Substring(description.IndexOf(";") + 1, description.Length - description.IndexOf(";") - 1);


                            string[] tmpArr = tmpStr.Split(',');


                            itemDesc.STOCKCODE = tmpArr[0];
                            itemDesc.WEIGHT = tmpArr[1];
                            itemDesc.PQTY = tmpArr[2];

                        }
                    }
                }
            }
            catch (Exception e)
            {
                itemDescs = null;
            }
            return itemDescs;
        }

        private string getItemDescirption(string type, string length, string colour)
        {
            string description = "";
            IList<string> descriptions;
            using (var context = new DbContext(Global.ConnStr))
            {
                object[] parameters = { type, length, colour };
                descriptions = context.Database.SqlQuery<string>("proc_GetItemDescription {0},{1},{2}", parameters).ToList<string>();
            }

            if (descriptions.Count > 0)
                description = descriptions[0].ToString();
            else
                description = "Item " + type + " " + length + " " + colour + " needs advice.";

            return description;
        }

        // POST api/<controller>
        // public string Post([FromBody]string value)
        public void Post(string value)
        {
            string[] splits = value.Split('~');
            //if (splits[0] == "aGdlfncosE") {
            //}           
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}