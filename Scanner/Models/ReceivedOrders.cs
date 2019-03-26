using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string USERNAME { get; set; }

        public Nullable<DateTime> ORDERDATE { get; set; }
        public Nullable<int> ACCNO { get; set; }
        public string ACCNAME { get; set; }
        public Nullable<int> HDR_SEQNO { get; set; }
        public string STOCKCODE { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<double> ORD_QUANT { get; set; }
        public Nullable<double> SUP_QUANT { get; set; }
        public Nullable<int> SEQNO { get; set; }

        public Nullable<double> QTYReceived { get; set; }
        public string Status { get; set; }
        public Nullable<DateTime> ReceivedTime { get; set; }
        public int maxPages { get; set; }
        public int TotalRows { get; set; }
    }

    public class ReceivedOrders
    {
        public Nullable<DateTime> ReceivedTime { get; set; }
        public string USERNAME { get; set; }
        public IList<ReceivedOrder> results { get; set; }

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

        public string printFlag { get; set; }
        public string updateFlag { get; set; }
    }
}