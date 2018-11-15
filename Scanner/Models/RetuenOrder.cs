using System.Collections.Generic;
namespace Scanner.Models
{
    public class ReturnOrder
    {
        public IList<string> TypeCodes { get; set; }
        public string colour { get; set; }
        public string size { get; set; }
        public int Qty { get; set; }
    }
}