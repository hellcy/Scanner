using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string M_COLOR { get; set; }
        public string S_COLOR { get; set; }
        public Nullable<double> WEIGHT { get; set; }
        public Nullable<double> GAUGE { get; set; }
        public Nullable<double> WIDTH { get; set; }
        public Nullable<DateTime> DATE_PRODUCED { get; set; }
        public Nullable<DateTime> DATE_USED { get; set; }
        public Nullable<int> STATUS { get; set; }
        public string USERID { get; set; }
        public Nullable<int> LENGTH { get; set; }
        public string SECTION { get; set; }
        public string RACK { get; set; }
        public Nullable<int> COLUMNS { get; set; }
        public Nullable<int> ROW { get; set; }

        public int maxPages { get; set; }
        public int TotalRows { get; set; }
    }

    public class CoilSlits
    {
        public IList<CoilSlit> slits { get; set; } = new List<CoilSlit>();
        public string input { get; set; }
        public string inputHidden { get; set; }
        public IList<CoilMaster> CoilDetails { get; set; } = null;
        public string errMsg { get; set; }
        public IList<String> CoilSlitIDs { get; set; } = null;
        public IList<String> CoilSlitLabels { get; set; } = null;
        public IList<String> QRcodes { get; set; } = null;
        public IList<String> Barcodes { get; set; } = null;
        public string printFlag { get; set; }
        public int slitWidth { get; set; } = 0;
        public int slitNumber { get; set; } = 0;

        public int totalRows { get; set; }
        public int totalPages { get; set; }
        public int pageNum { get; set; }
        public int rowsPerPage { get; set; }
        public string sortCol { get; set; }
        public string sortColType { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "* Search Text Input (Max 50 letters)")]
        [Display(Name = "Search Text")]
        public string whereStr { get; set; }
        public string orderBy { get; set; }
        public string table { get; set; }
        public string selStr { get; set; }
        public IList<string> ColorList { get; set; }
        public string colorSort { get; set; }
        public int widthSort { get; set; } = -1;
    }
}