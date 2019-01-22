using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class CoilMaster
    {
        public CoilMaster()
        {
            WEIGHT = null;
            WIDTH = null;
            GAUGE = null;
            ORDER = null;
            DATE_INWH = null;
            DATE_TRANSFER = null;
            LAST_STOCKTAKE_DATE = null;
            CLENGTH = null;
            Count = 0;
        }

        public string SaveTable { get; set; }
        public string Save { get; set; }
        public string Input { get; set; }
        public string COILID { get; set; }
        public string TYPE { get; set; }
        public string COLOR { get; set; }
        public Nullable<double> WEIGHT { get; set; }
        public Nullable<double> GAUGE { get; set; }
        public Nullable<double> WIDTH { get; set; }
        public Nullable<int> ORDER { get; set; }
        public string P_ORDER { get; set; }
        public string MONTH_RECD { get; set; }
        public Nullable<DateTime> DATE_INWH { get; set; }
        public Nullable<DateTime> DATE_TRANSFER { get; set; }
        public Nullable<DateTime> LAST_STOCKTAKE_DATE { get; set; }
        public string STATUS { get; set; }
        public Nullable<int> CLENGTH { get; set; }
        public string ZINCCOAT { get; set; }
        public string Flag { get; set; }
        public int Count { get; set; }
        public int maxPages { get; set; }
        public int TotalRows { get; set; }
        public string updateFlag { get; set; }
    }

    public class CoilMasters
    {
        public CoilMasters()
        {
            excelCoilIDs = new List<string>();
        }
        public IList<CoilMaster> CoilDetails { get; set; } = null;
        public CoilMaster CoilDetail { get; set; } = null;
        public String errMsg { get; set; }
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
        public string FilePath { get; set; }
        public IList<string> excelCoilIDs { get; set; }
    }
}