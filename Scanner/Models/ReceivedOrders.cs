using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class ReceivedOrder
    {
        public ReceivedOrder()
        {
            DESCRIPTION = "";
            STOCKCODE = "";
        }
        public Nullable<DateTime> ORDERDATE { get; set; }
        public Nullable<int> ACCNO { get; set; }
        public Nullable<int> HDR_SEQNO { get; set; }
        public string STOCKCODE { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<double> ORD_QUANT { get; set; }
        public Nullable<double> SUP_QUANT { get; set; }
        public Nullable<int> SEQNO { get; set; }
        public string NAME { get; set; }

        public Nullable<double> QTYReceived { get; set; }
    }

    public class ReceivedOrders
    {
        public Nullable<DateTime> ReceivedTime { get; set; }
        public string USERNAME { get; set; }
        public IList<ReceivedOrder> results { get; set; }
    }
}