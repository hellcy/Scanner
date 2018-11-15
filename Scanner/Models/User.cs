using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace Scanner.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "* User Name is null")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "* User Name Input (Max 50 letters)")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "* Password is null")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "* Password Input (Max 20 letters)")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [Required(ErrorMessage = "* Password Re-input is null")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "* Password Re-input (Max 20 letters)")]
        [DataType(DataType.Password)]
        [Display(Name = "Password Re-input")]
        public string RePassword { get; set; }

        [Required(ErrorMessage = "* Email is null")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "* Email Input (Max 50 letters)")]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "* First Name is null")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "* First tName Input (Max 50 letters)")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "* Last Name is null")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "* Last Name Input (Max 50 letters)")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "* ACCNO is null")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "ACCNO must be numeric")]
        [Display(Name = "ACCNO")]
        public int ACCNO { get; set; }


        [Display(Name = "Primary Type")]
        public int ACCGroup { get; set; }


        [Required(ErrorMessage = "* Mobile is null")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "* Mobile Input (Max 20 letters)")]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }


        [StringLength(20, MinimumLength = 1, ErrorMessage = "* Phone Input (Max 20 letters)")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }


        [StringLength(60, MinimumLength = 1, ErrorMessage = "* Company Input (Max 60 letters)")]
        [Required(ErrorMessage = "* Company is null")]
        [Display(Name = "Company")]
        public string CompanyName { get; set; }


        [StringLength(10, MinimumLength = 1, ErrorMessage = "*  DriverLic Input (Max 10 letters)")]
        [Display(Name = " DriverLic")]
        public string DriverLic { get; set; }

        [StringLength(30, MinimumLength = 1, ErrorMessage = "* ABN Input (Max 30 letters)")]
        [Display(Name = "ABN")]
        public string ABN { get; set; }

        [Required(ErrorMessage = "* Address is null")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "* Address Input (Max 30 letters)")]
        [Display(Name = "Address")]
        public string Address1 { get; set; }

        [Required(ErrorMessage = "* Suburb/City is null")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "* Suburb/City Input (Max 30 letters)")]
        [Display(Name = "Suburb/City")]
        public string Address2 { get; set; }

        [Required(ErrorMessage = "* State is null")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "* Postcode is null")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "* Postcode Input (Max 10 letters)")]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }


        public string allowReturn { get; set; }

        [Required(ErrorMessage = "* Country is null")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "* Country Input (Max 30 letters)")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        public bool Approved { get; set; }

        public Nullable<int> ApprovedBy { get; set; }

        public int BranchId { get; set; }

        public string BranchDBName { get; set; }

        public Nullable<DateTime> CreatedDate { get; set; }

        public string DisactiveDate { get; set; }

        public bool isTrial { get; set; }

        public bool isActive { get; set; }

        public int isUpdate { get; set; }

        public string Role { get; set; }

        public int maxPages { get; set; }

        public int TotalRows { get; set; }
    }
}