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
    public class RefCodeRepository : IRefCodeRepository
    {
        IDbSession DbSession = null;
        public RefCodeRepository(IDbSession DbSession)
        {
            this.DbSession = DbSession;
        }

        public RefCode GetByCode(string Code)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@Code", Code);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
            return DbSession.Connection.QuerySingle<RefCode>("pr_tblRefCode_SelectOne", param: dp, commandType: CommandType.StoredProcedure);
        }

        public decimal Update(RefCode orderInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@CodeKM", orderInf.Code);
            dp.Add("@OrderId", orderInf.OrderId);


            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblRefCode_Update", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);

            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                // Throw error.
                throw new Exception("Stored Procedure 'pr_tblRefCode_Update' reported the ErrorCode: " + _ErrorCode);
            }
            return orderInf.OrderId;
        }

        public IEnumerable<RefCode> GetList(string CTBH, string Ma_BH, string Ma_Dvi)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@Ma_Dvi", Ma_Dvi);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            using (var multi = DbSession.Connection.QueryMultiple("pr_tblRefCode_List", param: dp, commandType: CommandType.StoredProcedure))
            {
                return multi.Read<RefCode>();
            }
        }
        public string Insert(RefCode refCodeInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@Code", refCodeInf.Code);
            dp.Add("@Ma_Dvi", refCodeInf.Ma_Dvi);
            dp.Add("@Ma_DL", refCodeInf.Ma_DL);
            dp.Add("@Ma_Kenh", refCodeInf.Kenh_KT);
            dp.Add("@Ma_NSD", refCodeInf.Ma_NSD);
            dp.Add("@Ma_NQL", refCodeInf.Ma_NQL);
            dp.Add("@MA_BH", refCodeInf.Ma_BH);
            dp.Add("@CTBH", refCodeInf.CTBH);
            dp.Add("@TL", refCodeInf.TL);
            dp.Add("@MTN", refCodeInf.MTN);
            dp.Add("@StartDate", refCodeInf.StartDate);
            dp.Add("@EndDate", refCodeInf.EndDate);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblRefCode_Insert", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);

            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                // Throw error. 
                throw new Exception("Stored Procedure 'pr_tblRefCode_Insert' reported the ErrorCode: " + _ErrorCode);
            }
            return refCodeInf.Code;
        }
    }
}