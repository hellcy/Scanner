using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class Arithmetic
    {

        //public Boolean isOdd { get; set; }
        //public Boolean isPrime { get; set; }
        public string Odd { get; set; }
        public string Even { get; set; }
        public string Prime { get; set; }
        public string NotPrime { get; set; }
    }

    public class Arithmetics
    {
        public IList<Arithmetic> ArithmeticsList { get; set; } = null;
        public string Test { get; set; }
    }


}