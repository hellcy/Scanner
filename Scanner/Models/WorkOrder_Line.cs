using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class WorkOrder_Line
    {
        public WorkOrder_Line()
        {
            STOCKCODE = "";
            DESCRIPTION = "";
            BATCHCODE = "";
            X_COLOR = "";
        }

        public Nullable<int> SEQNO { get; set; }
        public Nullable<int> HDR_SEQNO { get; set; }
        public string STOCKCODE { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<double> QTYREQD { get; set; }
        public Nullable<double> QTYUSED { get; set; }
        public string BATCHCODE { get; set; }
        public Nullable<int> X_LENGTH { get; set; }
        public string X_COLOR { get; set; }
        public Nullable<int> X_SOLINE { get; set; }
        public Nullable<int> X_NARRATIVE { get; set; }
        public Nullable<int> X_LINESTATUS { get; set; }

    }

    public class WorkOrder_Lines
    {
        public WorkOrder_Lines()
        {
            errMsg = "";
            workOrder_HDR = new WorkOrder_HDR();
            updateFlag = "";
        }

        public IList<WorkOrder_Line> workOrder_Lines { get; set; }
        public WorkOrder_HDR workOrder_HDR { get; set; }
        public String errMsg { get; set; }
        public string updateFlag { get; set; }
    }
}