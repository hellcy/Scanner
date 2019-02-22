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
        }
        public Nullable<DateTime> ORDERDATE { get; set; }
        public Nullable<int> ACCNO { get; set; }
        public Nullable<int> HDR_SEQNO { get; set; }
        public string STOCKCODE { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<double> ORD_QUANT { get; set; }
        public Nullable<int> SEQNO { get; set; }

        public Nullable<double> QTYCollected { get; set; }
        public string Bundle { get; set; }
    }

    public class CollectedOrders
    {
        public IList<CollectedOrder> results { get; set; }
    }
}