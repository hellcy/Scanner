using System.Collections.Generic;
namespace Scanner.Models
{
    public class NewOrder
    {
        public OrderHead Head { get; set; }
        public IList<NewOrderDetail> OrderDetails { get; set; }
        public bool SplitOrder { get; set; }
    }
}