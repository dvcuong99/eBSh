using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using eBSH.Core;
using eBSH.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace eBSH.Repositories
{
    public class GiftCodeRepository : IGiftCodeRepository
    {
        IDbSession DbSession = null;
        public GiftCodeRepository(IDbSession DbSession)
        {
            this.DbSession = DbSession;
        }
        public GiftCode GetByCode(string CodeKM,string Ma_BH, string CTBH)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@CodeKM", CodeKM);
            dp.Add("@Ma_BH", Ma_BH);
            dp.Add("@CTBH", CTBH);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
            return DbSession.Connection.QuerySingle<GiftCode>("pr_tblGiftCode_SelectOne", param: dp, commandType: CommandType.StoredProcedure);
        }
        public decimal Update(GiftCode orderInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@CodeKM", orderInf.CodeKM);
            dp.Add("@OrderId", orderInf.OrderId);
           

            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblGiftCode_Update", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);

            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                // Throw error.
                throw new Exception("Stored Procedure 'pr_tblGiftCode_Update' reported the ErrorCode: " + _ErrorCode);
            }
            return orderInf.OrderId;
        }
        public void UpdateRemainderKM(string CodeKM)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@CodeKM", CodeKM);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblGiftCode_Update_RemainderKM", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);

            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                // Throw error.
                throw new Exception("Stored Procedure 'pr_tblGiftCode_Update_QuantityKM' reported the ErrorCode: " + _ErrorCode);
            }
        }
        public IEnumerable<GiftCode> GetList(string MA_BH,string CTBH, DateTime SDate, DateTime EDate)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@MA_BH", MA_BH);
            dp.Add("@CTBH", CTBH);
            dp.Add("@SDate", SDate);
            dp.Add("@EDate", EDate);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            using (var multi = DbSession.Connection.QueryMultiple("pr_tblGiftCode_List", param: dp, commandType: CommandType.StoredProcedure))
            {
                return multi.Read<GiftCode>();

            }
        }
        public string Insert(GiftCode giftInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@CodeKM", giftInf.CodeKM);
            dp.Add("@MA_BH", giftInf.Ma_BH);
            dp.Add("@CTBH", giftInf.CTBH);
            dp.Add("@TLKM", giftInf.TLKM);
            dp.Add("@MTN", giftInf.MTN);
            dp.Add("@StartDate", giftInf.StartDate);
            dp.Add("@EndDate", giftInf.EndDate);
            dp.Add("@Kenh_KT", giftInf.KENH_KT);
            dp.Add("@Ma_NQL", giftInf.Ma_NQL);
            dp.Add("@Ma_NSD", giftInf.Ma_NSD);
            dp.Add("@AmountKM", giftInf.AmountKM);

            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblGiftCode_Insert", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);

            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                // Throw error.
                throw new Exception("Stored Procedure 'pr_tblOrder_Insert' reported the ErrorCode: " + _ErrorCode);
            }
            return giftInf.CodeKM;
        }
        public GiftCode GetOne(string CodeKM, string Ma_BH, string CTBH)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@CodeKM", CodeKM);
            dp.Add("@Ma_BH", Ma_BH);
            dp.Add("@CTBH", CTBH);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
            return DbSession.Connection.QuerySingle<GiftCode>("pr_tblGiftCode_GetOne", param: dp, commandType: CommandType.StoredProcedure);
        }
    }
}