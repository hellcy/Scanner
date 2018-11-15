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
    public class UserController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public IList<User> Get(string DriverLic)
        {
            IList<User> users = new List<User>();
            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    object[] parameters = { DriverLic };
                    users = context.Database.SqlQuery<User>("proc_GetUserByDriverLic {0}", parameters).ToList<User>();
                }
            }
            catch (Exception e)
            {
                users = null;
            }
            return users;
        }

        public IList<User> Get(int Id)
        {
            IList<User> users = new List<User>();
            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    object[] parameters = { Id };
                    users = context.Database.SqlQuery<User>("proc_GetUser {0}", parameters).ToList<User>();
                }
            }
            catch (Exception e)
            {
                users = null;
            }
            return users;
        }


        public string Post(string company)
        {
            string msg = "";
            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    object[] parameters = { company };
                    msg = context.Database.SqlQuery<string>("proc_SendUserLogin {0}", parameters).ToList<string>()[0];
                }
            }
            catch (Exception e)
            {
                msg = "";
            }
            return msg;

        }

    }
}