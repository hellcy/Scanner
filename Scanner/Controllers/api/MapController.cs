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
    public class MapController : ApiController
    {
        // GET api/<controller>
        //[HttpGet]

        public IList<BranchLoc> Get()
        {
            IList<BranchLoc> brancheLocs = new List<BranchLoc>();
            try
            {
                using (var context = new DbContext(Global.ConnStr))
                {
                    brancheLocs = context.Database.SqlQuery<BranchLoc>("proc_GetBranchLocs").ToList<BranchLoc>();
                }

                if (brancheLocs == null)
                {
                    brancheLocs = new List<BranchLoc>();
                }
            }
            catch (Exception e)
            {
                brancheLocs = new List<BranchLoc>();
            }

            return brancheLocs;
        }


        public string Get(string value)
        {
            string[] values = value.Split(new string[] { "~!" }, StringSplitOptions.None);
            int Id = -1;
            using (var context = new DbContext(Global.ConnStr))
            {
                object[] parameters = { values[0], values[1], values[2], values[3] };
                IList<int> data = context.Database.SqlQuery<int>("proc_AddBranchLoc {0},{1},{2},{3}", parameters).ToList<int>();


                if (data != null)
                {
                    if (data.Count > 0)
                    {
                        Id = data[0];
                    }
                }
                else
                {
                    Id = -1;
                }
            }


            return Id.ToString();
        }


        public void Delete(int id)
        {
            using (var context = new DbContext(Global.ConnStr))
            {
                object[] parameters = { id };
                context.Database.ExecuteSqlCommand("proc_DelBranchLoc {0}", parameters);
            }
        }
    }

}