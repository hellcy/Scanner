using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scanner.Models
{
    public class WorkOrders
    {
        public WorkOrders()
        {
            errMsg = "";
        }

        public IList<WorkOrder_HDR> workOrders { get; set; }
        public String errMsg { get; set; }
    }
}