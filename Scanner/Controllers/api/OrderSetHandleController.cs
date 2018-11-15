using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Scanner.Controllers.api
{
    public class OrderSetHandleController : ApiController
    {
        // GET api/<controller>
        public string Delete(string request)
        {
            string ok = "no";
            if (request.ToUpper().IndexOf("GE") > -1)
            {
                string GramPass = request;
                string GramUserId = GramPass.ToUpper().Replace("GE", "");
                int value;
                if (int.TryParse(GramUserId, out value))
                {
                    string data = "";
                    using (var context = new DbContext(Global.ConnStr))
                    {

                        object[] parameters = {
                               GramUserId
                            };

                        data = context.Database.SqlQuery<string>("proc_GetStaffID {0}", parameters).ToList<string>()[0];

                    }

                    if (data == "1")
                    {

                        //HttpContext.Current.Session["HandledBy"] = GramUserId.ToString();

                        int a = HttpContext.Current.Session.Keys.Count;



                        //var session = HttpContext.Current.Session;
                        //session.Add("HandledBy", GramUserId.ToString());
                        //session["HandledBy"] = GramUserId.ToString();

                        ok = "ok";
                    }
                }
            }

            return ok;
        }
    }
}