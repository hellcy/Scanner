using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.ModelBinding;

namespace Scanner.Models
{
    public class OrderHead2
    {
        public int Id { get; set; }
        public string Company { get; set; }

        public string OrderBy { get; set; }

        public string ContactName { get; set; }

        public string SubmittedBy { get; set; }

        public string HandleBy { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public bool RequestForDelivery { get; set; }

        public string CustomerOrderNo { get; set; }

        public string Reference { get; set; }

        public string PickupBy { get; set; }

        public Nullable<DateTime> OrderDate { get; set; }

        public Nullable<DateTime> LastUpdate { get; set; }

        public string ACCNO { get; set; }

        public Nullable<double> TotalWeight { get; set; }

        public Nullable<double> TotalPrice { get; set; }

        public Nullable<int> BranchID { get; set; }

        public string Status { get; set; }

        public string ServerSEQNO { get; set; }

        public string Comment { get; set; }


        public Nullable<bool> isFront { get; set; }

        public int CreatedById { get; set; }


        public string Message { get; set; }

        public int maxPages { get; set; }

        public int TotalRows { get; set; }

        public Nullable<bool> Approved { get; set; }
    }
}