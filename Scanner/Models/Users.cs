using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scanner.Models
{
    public class Users
    {
        public IList<User> users { get; set; }
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
        public String errMsg { get; set; }
    }
}