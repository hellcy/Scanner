using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Scanner.Models;
using System.IO;
using System.Diagnostics;
using System.Web.Configuration;
using System.Data.Entity;
//using System.Web;

namespace Scanner.Controllers.api
{
    public class InternalController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            // var session = HttpContext.Current.Session;
            return new string[] { "value1", "value2" };
        }


        public HttpResponseMessage Get(string req)
        {
            return null;
        }


        public IList<ItemDescription> Post(IList<ItemDescription> itemDescs)
        {
            return null;
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
        public string Delete(string filename)
        {
            string msg = "";
            string tmpDir = filename.Split(new string[] { "__________" }, StringSplitOptions.None)[0];
            string nfilename = filename.Split(new string[] { "__________" }, StringSplitOptions.None)[1];
            try
            {
                File.Delete(string.Format("{0}UploadedTmp\\" + tmpDir + @"\" + nfilename, System.Web.Hosting.HostingEnvironment.MapPath(@"\")));
            }
            catch (Exception e)
            {
                msg = "Error: " + e.Message;

                string errBody = "Error: " + e.Message.Replace(Environment.NewLine, "<br>");
                if (((e.InnerException != null) && (!string.IsNullOrEmpty(e.InnerException.Message))))
                {
                    errBody += @"<br><br><br>" + e.InnerException.Message.Replace("\n", "<br>");
                }

                object[] parameters_ = {
                                        WebConfigurationManager.AppSettings["GramAdminEmails"],
                                        "Error: Delete File Issue " + string.Format("{0}UploadedTmp\\" + tmpDir+@"\"+nfilename, System.Web.Hosting.HostingEnvironment.MapPath(@"\")),
                                        errBody,
                                        "",
                                        "",
                                        ""
                                    };
                using (var context = new DbContext(Global.ConnStr))
                {
                    context.Database.ExecuteSqlCommand("proc_SendIssueNotification {0},{1},{2},{3},{4},{5}", parameters_);
                }

            }

            return msg;
        }

    }
}