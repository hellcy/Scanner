using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class CoilModel
    {
        public CoilModel()
        {
            Weight = null;
            Width = null;
            Gauge = null;
            Order = null;
            Date_inwh = null;
            Date_transfer = null;
            Last_stocktake_date = null;
            Clength = null;
            Count = 0;
        }

        public string Save2 { get; set; }
        public string Save { get; set; }
        public string Input { get; set; }
        public string ID { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public double? Weight { get; set; }
        public double? Gauge { get; set; }
        public double? Width { get; set; }
        public int? Order { get; set; }
        public string P_order { get; set; }
        public string Month_recd { get; set; }
        public DateTime? Date_inwh { get; set; }
        public DateTime? Date_transfer { get; set; }
        public DateTime? Last_stocktake_date { get; set; }
        public string Status { get; set; }
        public int? Clength { get; set; }
        public string Flag { get; set; }
        public int Count { get; set; }
    }

    public class CoilDetail
    {
        public IList<CoilModel> CoilDetails { get; set; } = null;
    }
}