using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBSH.Models
{
    public class VNPayOrderVM
    {
        public decimal So_ID { get; set; }
        public string Ma_dvi { get; set; }
        public string Ma_BH { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string vnpUrlReturn { get; set; }
    }
    public class VNPayOrderRes
    {
        public decimal OrderId { get; set; }
        public string Ma_dvi { get; set; }
        public string Ma_BH { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public decimal So_ID { get; set; }
        public string So_HD { get; set; }
        public string Url { get; set; }
        public string PreviewUrl { get; set; }
        public string TT { get; set; }
        public string CardType { get; set; }
        public string BankCode { get; set; }
        public string BankTranNo { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public decimal TransactionNo { get; set; }
        public string TMNCode { get; set; }
        public string IpAddr { get; set; }
        public string Message { get; set; }
        public string TXNResponseCode { get; set; }
        public string PayDate { get; set; }
        public DateTime ResponseDate { get; set; }

        public string vnpUrlReturn { get; set; }
    }
}