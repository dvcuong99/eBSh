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
    public class GCNRepository : IGCNRepository
    {
        protected IDbSession DbSession;
        public GCNRepository(IDbSession DbSession)
        {
            this.DbSession = DbSession;
        }
 
        public PHH_CBS_GCN GetByID(decimal PolicyID)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@SO_ID", PolicyID);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
            return DbSession.Connection.QuerySingle<PHH_CBS_GCN>("pr_tblCBSGCN_SelectOne", param: dp, commandType: CommandType.StoredProcedure);
        }
        public decimal Insert(PHH_CBS_GCN orderInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@SO_ID", orderInf.SO_ID);
            dp.Add("@HO_TH", orderInf.HO_TH);
            dp.Add("@TEN_TH", orderInf.TEN_TH);
            dp.Add("@DCHIKH", orderInf.DCHIKH);
            dp.Add("@SO_CC", orderInf.SO_CC);
            dp.Add("@NG_CAPCC", orderInf.NG_CAPCC);
            dp.Add("@NOI_CAPCC", orderInf.NOI_CAPCC);
            dp.Add("@EMAIL", orderInf.EMAIL);
            dp.Add("@PHONE", orderInf.PHONE);
            dp.Add("@SO_TK", orderInf.SO_TK);
            dp.Add("@NGAN_HANG", orderInf.NGAN_HANG);
            dp.Add("@NGAY_HL", orderInf.NGAY_HL);
            dp.Add("@NGAY_KT", orderInf.NGAY_KT);
            dp.Add("@CTBH", orderInf.CTBH);
            dp.Add("@MTNBH", orderInf.MTNBH);
            dp.Add("@PHIBH", orderInf.PHIBH);
            dp.Add("@CODE_KM", orderInf.CODE_KM);
            dp.Add("@TL_KM", orderInf.TL_KM);
            dp.Add("@TEN_VI", orderInf.TEN_VI);
            dp.Add("@SO_HD", orderInf.SO_HD);
            dp.Add("@TT", orderInf.TT);
            dp.Add("@KENH", orderInf.KENH);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblCBSGCN_Insert", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);

            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                // Throw error.
                throw new Exception("Stored Procedure 'pr_tblCBSGCN_Insert' reported the ErrorCode: " + _ErrorCode);
            }
            return orderInf.SO_ID;
        }

        public decimal Update(PHH_CBS_GCN orderInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@SO_ID", orderInf.SO_ID);
            dp.Add("@HO_TH", orderInf.HO_TH);
            dp.Add("@TEN_TH", orderInf.TEN_TH);
            dp.Add("@DCHIKH", orderInf.DCHIKH);
            dp.Add("@SO_CC", orderInf.SO_CC);
            dp.Add("@NG_CAPCC", orderInf.NG_CAPCC);
            dp.Add("@NOI_CAPCC", orderInf.NOI_CAPCC);
            dp.Add("@EMAIL", orderInf.EMAIL);
            dp.Add("@PHONE", orderInf.PHONE);
            dp.Add("@SO_TK", orderInf.SO_TK);
            dp.Add("@NGAN_HANG", orderInf.NGAN_HANG);
            dp.Add("@NGAY_HL", orderInf.NGAY_HL);
            dp.Add("@NGAY_KT", orderInf.NGAY_KT);
            dp.Add("@CTBH", orderInf.CTBH);
            dp.Add("@MTNBH", orderInf.MTNBH);
            dp.Add("@PHIBH", orderInf.PHIBH);
            dp.Add("@CODE_KM", orderInf.CODE_KM);
            dp.Add("@TL_KM", orderInf.TL_KM);
            dp.Add("@TEN_VI", orderInf.TEN_VI);
            dp.Add("@SO_HD", orderInf.SO_HD);
            dp.Add("@TT", orderInf.TT);
            dp.Add("@KENH", orderInf.KENH);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblCBSGCN_Update", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);

            //var _ErrorCode = dp.Get<int>("@iErrorCode");

            //if (_ErrorCode != 0)
            //{
            //    // Throw error.
            //    throw new Exception("Stored Procedure 'pr_tblOrder_Update' reported the ErrorCode: " + _ErrorCode);
            //}
            return orderInf.SO_ID;
        }
        public IEnumerable<PHH_CBS_GCN> GetList(string CTBH, DateTime SDate, DateTime EDate)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@CTBH", CTBH);
            dp.Add("@SDate", SDate);
            dp.Add("@EDate", EDate);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            using (var multi = DbSession.Connection.QueryMultiple("pr_CBSGCN_List", param: dp, commandType: CommandType.StoredProcedure))
            {
                return multi.Read<PHH_CBS_GCN>();

            }
        }
        public PHH_CBS_GCN GetByID(string SO_ID)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@SO_ID", SO_ID);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
            var result = DbSession.Connection.QuerySingle<PHH_CBS_GCN>("pr_CBSGCN_GetOne", param: dp, commandType: CommandType.StoredProcedure);
            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                throw new Exception("Stored Procedure 'pr_CBSGCN_GetOne' reported the ErrorCode: " + _ErrorCode);
            }
            return result;
        }
        public IEnumerable<CBSExp> GetData(string CTBH, DateTime SDate, DateTime EDate)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@CTBH", CTBH);
            dp.Add("@SDate", SDate);
            dp.Add("@EDate", EDate);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            using (var multi = DbSession.Connection.QueryMultiple("pr_CBSGCN_List", param: dp, commandType: CommandType.StoredProcedure))
            {
                return multi.Read<CBSExp>();

            }
        }

    }
}