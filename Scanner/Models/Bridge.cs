using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class Bridge
    {
        public int HandId { get; set; }
        public int Vulnerability { get; set; }
        public string NHand { get; set; }

        public int HCP_N { get; set; }
        public int HCP_N_Extra { get; set; }
        public double NTOP { get; set; }
        public string NShape { get; set; }
        public string OpenerShape { get; set; }
        public string Grouping2 { get; set; }
        public string BidMade { get; set; }
        public int NSHL { get; set; }
        public int NR { get; set; }
        public int S_SHL { get; set; }
        public int H_SHL { get; set; }
        public int D_SHL { get; set; }
        public int C_SHL { get; set; }

        public double OpenerNLC { get; set; }
    }
}