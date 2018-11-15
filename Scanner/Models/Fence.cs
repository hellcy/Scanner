using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanner.Models
{
    public class Fence
    {
        public IList<string> colours { get; set; }
        public IList<string> standards { get; set; }
        public IList<bool> isNorms { get; set; }
        public IList<string> Messages { get; set; }
    }
}