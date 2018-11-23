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
        public WorkOrder_HDR()
        {
            BILLCODE = "";
            PRODCODE = "";
            BATCHCODE = "";
            NOTES = "";
            REFERENCE = "";
            X_BR_ACCNO = "";
            X_BR_INVNO = "";
            X_BR = "";
        }

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
        public int maxPages { get; set; }
        public int TotalRows { get; set; }

    }

    public class WorkOrder_HDRs
    {
        public WorkOrder_HDRs()
        {
            errMsg = "";
        }

        public IList<WorkOrder_HDR> workOrder_HDRs { get; set; }
        public String errMsg { get; set; }
    }
}
