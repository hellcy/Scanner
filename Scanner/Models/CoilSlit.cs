using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class CoilSlit
    {
        public string COIL_SLIT_ID { get; set; }
        public string TYPE { get; set; }
        public string COLOR { get; set; }
        public Nullable<double> WEIGHT { get; set; }
        public Nullable<double> GAUGE { get; set; }
        public Nullable<double> WIDTH { get; set; }
        public Nullable<DateTime> DATE_PRODUCED { get; set; }
        public Nullable<DateTime> DATE_USED { get; set; }
        public Nullable<int> STATUS { get; set; }
        public string USERID { get; set; }
    }
}