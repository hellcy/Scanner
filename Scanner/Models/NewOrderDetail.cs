using System;
using System.Collections.Generic;

namespace Scanner.Models
{
    public class NewOrderDetail : ICloneable
    {
        public int Id { get; set; }
        public string STOCKCODE { get; set; }
        public string ITEMTYPE { get; set; }
        public string HEADING { get; set; }
        public string STANDARD { get; set; }
        public string LENGTH { get; set; }
        public string StandardLength { get; set; }
        public string COLOUR { get; set; }
        public string DESCRIPTION { get; set; }
        public string WEIGHT { get; set; }
        public string QTY { get; set; }
        public string PQTY { get; set; }
        public string PRICE { get; set; }
        public string DISCOUNTRATE { get; set; }
        public string BASEPRICE { get; set; }
        public string ACCNO { get; set; }
        public string VALID { get; set; }
        public string SUNDRYCOLOUR { get; set; }
        public string ALERT { get; set; }
        public bool isNormal { get; set; }
        public bool needAdvice { get; set; }
        public Nullable<int> C_X { get; set; }
        public Nullable<int> C_Y { get; set; }
        public string CutDesc { get; set; }
        public int Cuts { get; set; }
        public decimal Idx { get; set; }
        public string nextRelColorId { get; set; }
        public string menuId { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}