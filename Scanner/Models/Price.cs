
using System;

namespace Scanner.Models
{
    public class Price
    {
        public double BEST_PRICE { get; set; }
        public double DISCOUNTRATE { get; set; }
        public double BASEPRICE { get; set; }
        public Nullable<double> DISCOUNT_AMOUNT { get; set; }
        public string IS_SPECIAL_PRICE { get; set; }
        public Nullable<int> POLICY_HDR { get; set; }
        public string FREIGHT_FREE { get; set; }
        public string FIXEDPOLICY { get; set; }

    }
}