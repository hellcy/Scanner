namespace Scanner.Models
{
    public class OutStanding
    {
        public int Id { get; set; }
        public int ACCNO { get; set; }
        public int ORDER_NO { get; set; }
        public string DESCRIPTION { get; set; }
        public string LOAD_NO { get; set; }
        public string PROD_ORDER { get; set; }
        public double QTY { get; set; }
        public double UNIT_WEIGHT { get; set; }
        public double WEIGHT { get; set; }
        public double PICK_NOW { get; set; }
        // public int DESPATCH_QTY { get; set; }
        public string Message { get; set; }
        public int maxPages { get; set; }
        public int TotalRows { get; set; }

    }
}