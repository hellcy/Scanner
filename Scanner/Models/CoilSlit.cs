using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class CoilSlit
    {
        public CoilSlit()
        {
            COIL_SLIT_ID = "";
            TYPE = "";
            COLOR = "";
            WEIGHT = 0;
            GAUGE = 0;
            WIDTH = 0;
            USERID = "";
        }

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

    public class CoilSlits
    {
        public IList<CoilSlit> slits { get; set; } = new List<CoilSlit>();
        public string input { get; set; }
        public IList<CoilMaster> CoilDetails { get; set; } = null;
        public string errMsg { get; set; }
        public IList<String> CoilSlitIDs { get; set; } = null;
        public IList<String> CoilSlitLabels { get; set; } = null;
        public IList<String> QRcodes { get; set; } = null;
        public IList<String> Barcodes { get; set; } = null;
        public string printFlag { get; set; }
        public int slitWidth { get; set; } = 0;
        public int slitNumber { get; set; } = 0;
    }
}