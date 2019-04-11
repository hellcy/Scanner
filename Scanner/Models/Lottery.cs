using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class Lottery
    {
        public double totalPrize { get; set; }
        public double wallet { get; set; }
        public int count { get; set; } = 0;
    }
}