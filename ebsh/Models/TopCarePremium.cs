using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBSH.Models
{
    public class TopCarePremium
    {
        public int ID { get; set; }
        public string CTBH { get; set; }
        public string ThoiHan { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public decimal Phi { get; set; }
    }
}