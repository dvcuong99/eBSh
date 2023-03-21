using Dapper;
using eBSH.Core;
using eBSH.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace eBSH.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        IDbSession DbSession = null;
        public OrderRepository(IDbSession DbSession)
        {
            this.DbSession = DbSession;
        }
        public OrderInfo GetByOrderID(decimal orderID)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@orderID", orderID);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
            return DbSession.Connection.QuerySingle<OrderInfo>("pr_tblOrder_SelectOne", param: dp, commandType: CommandType.StoredProcedure);
        }

        public OrderInfo GetLastOrderByPolicyID(decimal PolicyID)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@policyID", PolicyID);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
            return DbSession.Connection.QuerySingle<OrderInfo>("pr_tblOrder_SelectByPolicy", param: dp, commandType: CommandType.StoredProcedure);
        }
        public decimal Insert(OrderInfo orderInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@orderID", orderInf.OrderId);
            dp.Add("@Amount", orderInf.Amount);
            dp.Add("@OrderDescription", orderInf.OrderDescription, size: 200);
            dp.Add("@PolicyID", orderInf.PolicyID );
            dp.Add("@PolicyStatus", orderInf.PolicyStatus, size: 20);
            dp.Add("@Status", orderInf.Status);
            
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblOrder_Insert", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);
            
            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                // Throw error.
                throw new Exception("Stored Procedure 'pr_tblOrder_Insert' reported the ErrorCode: " + _ErrorCode);
            }
            return orderInf.OrderId;
        }

        public decimal Update(OrderInfo orderInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@orderID", orderInf.OrderId);  
            dp.Add("@PolicyNo", orderInf.PolicyNo, size: 50);
            dp.Add("@PolicyStatus", orderInf.PolicyStatus, size: 20);
            dp.Add("@Status", orderInf.Status);
            dp.Add("@vnp_TransactionNo", orderInf.vnp_TransactionNo);
            dp.Add("@CardType", orderInf.CardType);
            dp.Add("@BankCode", orderInf.BankCode);
            dp.Add("@BankTranNo", orderInf.BankTranNo);
            dp.Add("@vnp_PayDate", orderInf.vnp_PayDate);
            dp.Add("@vnp_Message", orderInf.vnp_Message);
            dp.Add("@vnp_TxnResponseCode", orderInf.vnp_TxnResponseCode);
            dp.Add("@vnp_ResponseDate", orderInf.vnp_ResponseDate);
 
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblOrder_Update", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);
             
            //var _ErrorCode = dp.Get<int>("@iErrorCode");

            //if (_ErrorCode != 0)
            //{
            //    // Throw error.
            //    throw new Exception("Stored Procedure 'pr_tblOrder_Update' reported the ErrorCode: " + _ErrorCode);
            //}
            return orderInf.OrderId;
        }
    }
}