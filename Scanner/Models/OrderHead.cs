using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.ModelBinding;

namespace Scanner.Models
{
    public class OrderHead
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "* Company is null.")]
        [StringLength(60, MinimumLength = 1, ErrorMessage = "* Company Input (Max 60 letters).")]
        [Display(Name = "Company:")]
        public string Company { get; set; }

        [Required(ErrorMessage = "* Name is null.")]
        [StringLength(60, MinimumLength = 1, ErrorMessage = "* Name Input (Max 60 letters).")]
        [Display(Name = "Name:")]
        public string OrderBy { get; set; }

        public string ContactName { get; set; }

        [Required(ErrorMessage = "* Mobile is null.")]
        [StringLength(60, MinimumLength = 1, ErrorMessage = " *Mobile Input(Max 30 letters).")]
        [Display(Name = "Mobile:")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "* Email is null.")]
        [Display(Name = "Email:")]
        [StringLength(200, MinimumLength = 0, ErrorMessage = "* Email Input (Max 200 letters).")]
        public string Email { get; set; }

        [Display(Name = "Driver License:")]
        [StringLength(10, MinimumLength = 0, ErrorMessage = "* Driver License Input (Max 10 letters).")]
        public string DriverLic { get; set; }

        [Display(Name = "ABN:")]
        [StringLength(30, MinimumLength = 0, ErrorMessage = "* ABN Input (Max 30 letters).")]
        public string ABN { get; set; }

        [Display(Name = "Request For Delivery:")]
        public bool RequestForDelivery { get; set; }

        [Required(ErrorMessage = "* PO No is null.")]
        [Display(Name = "Customer PO No:")]
        [StringLength(20, MinimumLength = 0, ErrorMessage = "* PO Num Input(Max 20 letters).")]
        public string CustomerOrderNo { get; set; }

        [Display(Name = "Reference:")]
        [StringLength(20, MinimumLength = 0, ErrorMessage = "* Reference Input(Max 20 letters).")]
        public string Reference { get; set; }

        [Display(Name = "Pickup by:")]
        public string PickupBy { get; set; }

        [Display(Name = "Date:")]
        public string OrderDate { get; set; }

        [Display(Name = "Date:")]
        public string ReadyToSubmitDate { get; set; }

        [Display(Name = "Date:")]
        public string SubmitDate { get; set; }

        [Display(Name = "Request Date:")]
        public string DispatchDate { get; set; }

        [Display(Name = "Address:")]
        public string Address { get; set; }

        [Display(Name = "Suburb/City:")]
        public string City { get; set; }

        [Display(Name = "State:")]
        public string State { get; set; }

        [Display(Name = "Postcode:")]
        public string Postcode { get; set; }

        [Display(Name = "Country:")]
        public string Country { get; set; }


        [Display(Name = "Driver License/Company/Email/User Name:")]
        [StringLength(100, MinimumLength = 0, ErrorMessage = "* Driver License/Company/Email/User Name Input (Max 100 letters).")]
        public string DriverLicSrch { get; set; }

        [Display(Name = "Comment:")]
        [StringLength(1000, MinimumLength = 0, ErrorMessage = "* Comment Input (Max 1000 letters).")]
        public string Comment { get; set; }

        public bool SplitOrder { get; set; }

        public string UserName { get; set; }

        public string SubmittedBy { get; set; }

        public string HandleBy { get; set; }

        public IList<string> CompanyList { get; set; }

        public string LastUpdate { get; set; }

        public string ACCNO { get; set; }

        public double TotalWeight { get; set; }

        public double TotalPrice { get; set; }

        public int BranchID { get; set; }

        public string Status { get; set; }

        public string ServerSEQNO { get; set; }

        public string Message { get; set; }

        public string PrintId1 { get; set; }

        public string PrintId2 { get; set; }

        public int maxPages { get; set; }

        public int TotalRows { get; set; }


    }
}