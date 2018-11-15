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
    public class ReturnController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public IList<string> Get(string ItemTypeCode)
        {
            string flag = ItemTypeCode.Split('.')[0];
            ItemTypeCode = ItemTypeCode.Split('.')[1];
            IList<string> values = new List<string>();
            var ItemType = ItemTypeCode.Split(',')[1];
            var ItemTypeName = ItemTypeCode.Split(',')[0];
            switch (flag)
            {
                case "1":
                    try
                    {
                        using (var context = new DbContext(Global.ConnStr))
                        {
                            object[] parameters = { ItemType };
                            values = context.Database.SqlQuery<string>("proc_GetRtnColours {0}", parameters).ToList<string>();
                        }
                    }
                    catch (Exception e)
                    {
                        values = new List<string>();
                    }
                    break;
                case "2":
                    try
                    {
                        using (var context = new DbContext(Global.ConnStr))
                        {
                            object[] parameters = { ItemType, ItemTypeName };
                            values = context.Database.SqlQuery<string>("proc_GetRtnLengths {0},{1}", parameters).ToList<string>();
                        }
                    }
                    catch (Exception e)
                    {
                        values = new List<string>();
                    }
                    break;
                case "3":
                    var ItemColour = ItemTypeCode.Split(',')[3];
                    var ItemLength = ItemTypeCode.Split(',')[4];
                    IList<StockDescription> descriptions = new List<StockDescription>();
                    try
                    {
                        using (var context = new DbContext(Global.ConnStr))
                        {
                            object[] parameters = { ItemType, ItemLength, ItemLength, ItemColour };
                            descriptions = context.Database.SqlQuery<StockDescription>("proc_GetItemDescription {0},{1},{2},{3}", parameters).ToList<StockDescription>();
                            values.Add(descriptions[0].ItemCode + "||" + descriptions[0].Description + "||" + descriptions[0].PQTY + "||" + descriptions[0].Weight);
                        }
                    }
                    catch (Exception e)
                    {
                        values = new List<string>();
                    }

                    break;
                default:
                    break;

            }

            return values;
        }
    }
}