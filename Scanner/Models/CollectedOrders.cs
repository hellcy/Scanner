using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class CollectedOrder
    {
        public CollectedOrder()
        {
            DESCRIPTION = "";
            Bundle = "";
            STOCKCODE = "";
        }
        public Nullable<DateTime> ORDERDATE { get; set; }
        public Nullable<int> ACCNO { get; set; }
        public Nullable<int> HDR_SEQNO { get; set; }
        public string STOCKCODE { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<double> ORD_QUANT { get; set; }
        public Nullable<int> SEQNO { get; set; }

        public Nullable<double> QTYCollected { get; set; }
        public Nullable<double> QTYPacked { get; set; }
        public Nullable<double> QTYLoaded { get; set; }
        public string Bundle { get; set; }
    }

    public class Bundle
    {
        public string bundle_name;
        public Nullable<int> weight;
    }

    public class CollectedOrders
    {
        public Nullable<DateTime> ReceivedTime { get; set; }
        public string USERNAME { get; set; }
        public IList<CollectedOrder> results { get; set; }
        public IList<Bundle> bundles { get; set; }
    }
}