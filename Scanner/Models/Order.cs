using System.Collections.Generic;
namespace Scanner.Models
{
    public class Order
    {
        public string Company { get; set; }
        public string ACCNO { get; set; }
        public string BranchIDDealWith { get; set; }
        public string OrderBy { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string RequestForDelivery { get; set; }
        public string OrderNo { get; set; }
        public string Reference { get; set; }
        public string OrderDate { get; set; }
        public string TotalWeight { get; set; }
        public string TotalPrice { get; set; }
        public IList<OrderDetail> OrderDetails { get; set; }
    }
}