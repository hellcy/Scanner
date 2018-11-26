using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.ModelBinding;

namespace Scanner.Models
{
    public class WorkOrder_HDR
    {
        public Nullable<int> SEQNO { get; set; }
        public string BILLCODE { get; set; }
        public string PRODCODE { get; set; }
        public string BATCHCODE { get; set; }
        public Nullable<DateTime> TRANSDATE { get; set; }
        public Nullable<DateTime> PRODDATE { get; set; }
        public Nullable<DateTime> DUEDATE { get; set; }
        public Nullable<int> ORDSTATUS { get; set; }
        public Nullable<int> SALESORDNO { get; set; }
        public string NOTES { get; set; }
        public Nullable<double> PRODQTY { get; set; }
        public Nullable<double> ACTUALQTY { get; set; }
        public Nullable<int> PRODLOCNO { get; set; }
        public string REFERENCE { get; set; }
        public Nullable<int> STAFFNO { get; set; }
        public Nullable<int> COMPONENTLOCNO { get; set; }
        public Nullable<DateTime> EXPIRY_DATE { get; set; }
        public Nullable<int> X_BR_ORDER { get; set; }
        public string X_BR_ACCNO { get; set; }
        public string X_BR_INVNO { get; set; }
        public string X_BR { get; set; }
        public string X_CATEGORY { get; set; }
        public Nullable<DateTime> X_COMPLETION_DATE { get; set; }
        public int maxPages { get; set; }
        public int TotalRows { get; set; }

    }

    public class WorkOrder_HDRs
    {
        public IList<WorkOrder_HDR> workOrder_HDRs { get; set; }
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
    }
}
