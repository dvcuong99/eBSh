using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eBSH.Models
{
    public class OrderInfo
    {
        /// <summary>
        /// Merchant OrderId
        /// </summary>
        public decimal OrderId { get; set; }
        /// <summary>
        /// Payment amount
        /// </summary>
        public decimal Amount { get; set; }
        public string OrderDescription { get; set; }
        public decimal PolicyID { get; set; }
        public string PolicyNo { get; set; }
        public string PolicyUrl { get; set; }
        public string PolicyPreviewUrl { get; set; }
        public string PolicyStatus { get; set; }
        public string CardType { get; set; }
        public string BankCode { get; set; }
        public string BankTranNo { get; set; }
        /// <summary>
        /// Order Status
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Transaction Id
        /// </summary>
        public decimal vnp_TransactionNo { get; set; }
        public string vnp_TmnCode { get; set; }
        public string vnp_IpAddr { get; set; }
        public string vnp_Message { get; set; }
        public string vnp_TxnResponseCode { get; set; }
        public string vnp_PayDate { get; set; }
        public DateTime vnp_ResponseDate { get; set; }
    }

    public class OrderVM
    {
        public PHH_CBS_GCN cyberInf { get; set; }
        public OrderInfo orderInf { get; set; }
    }

    public class PAOrderVM
    {
        public CN_PA_GCN paInf { get; set; }
        public OrderInfo orderInf { get; set; }
    }

    public class McOrderVM
    {
        public Motorcycle mcInf { get; set; }
        public OrderInfo orderInf { get; set; }
    }
    public class MvcOrderVM
    {
        public MotorVehicle mvcInf { get; set; }
        public OrderInfo orderInf { get; set; }
    }
}