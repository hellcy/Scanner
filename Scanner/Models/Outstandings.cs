using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Scanner.Models
{
    public class OutStandings
    {
        public IList<OutStanding> outStandings { get; set; }
        public IList<string> CompanyList { get; set; }
        public double totalWeight { get; set; }
        public IList<Despatch> despatches { get; set; }
        public string despatchItems { get; set; }
        public int totalRows { get; set; }
        public int totalPages { get; set; }
        public int pageNum { get; set; }
        public int rowsPerPage { get; set; }
        public string sortCol { get; set; }
        public string sortColType { get; set; }
        [Display(Name = "Company")]
        public string Company { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "* Search Text Input (Max 50 letters)")]
        [Display(Name = "Search Text")]
        public string whereStr { get; set; }
        public string orderBy { get; set; }
        public string table { get; set; }
        public string selStr { get; set; }

        public string errMsg { get; set; }
    }
}