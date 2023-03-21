using Dapper;
using eBSH.Core;
using eBSH.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace eBSH.Repositories
{
    public class PA_GCNRepository : IPA_GCNRepository
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        IDbSession DbSession = null;
        public PA_GCNRepository(IDbSession DbSession)
        {
            this.DbSession = DbSession;
        }
        public CN_PA_GCN GetByID(decimal PolicyID)
        {
            CN_PA_GCN gcn = null;
            try
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("@So_ID", PolicyID);
                dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
                gcn = DbSession.Connection.QuerySingle<CN_PA_GCN>("pr_tblPAGCN_SelectOne", param: dp, commandType: CommandType.StoredProcedure);
                using (var multi = DbSession.Connection.QueryMultiple("pr_tblPA_CT_SelectList", param: dp, commandType: CommandType.StoredProcedure))
                {
                    var ds = multi.Read<PA_CT>();
                    gcn.DS_NBH = new List<PA_CT>(ds);
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
            }
            return gcn;
        }
        public decimal Insert(CN_PA_GCN orderInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@So_ID", orderInf.So_ID);
            dp.Add("@Ma_Dvi", orderInf.Ma_Dvi);
            dp.Add("@So_HD", orderInf.So_HD);
            dp.Add("@KhachHang", orderInf.KhachHang);
            dp.Add("@CMND", orderInf.CMND);
            dp.Add("@Phone", orderInf.Phone);
            dp.Add("@Email", orderInf.Email);
            dp.Add("@DChi", orderInf.DChi);

            dp.Add("@Gio_HL", orderInf.Gio_HL);
            dp.Add("@Gio_KT", orderInf.Gio_KT);
            dp.Add("@Ngay_HL", orderInf.Ngay_HL);
            dp.Add("@Ngay_KT", orderInf.Ngay_KT);
            dp.Add("@Ngay_TT", orderInf.Ngay_TT);
            dp.Add("@Ma_BH", orderInf.Ma_BH);
            dp.Add("@TT", orderInf.TT);
            dp.Add("@NSD", orderInf.NSD);
            dp.Add("@NQL", orderInf.NQL);
            dp.Add("@MA_DL", orderInf.MA_DL);
            dp.Add("@KENH_KT", orderInf.KENH_KT);
            dp.Add("@CTBH", orderInf.CTBH);

            dp.Add("@CODE_KM", orderInf.CODE_KM);
            dp.Add("@TL_KM", orderInf.TL_KM);
            dp.Add("@Phi_BH", orderInf.Phi_BH);
            dp.Add("@ThoiHan", orderInf.ThoiHan);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblPAGCN_Insert", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);

            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                // Throw error.
                throw new Exception("Stored Procedure 'pr_tblPAGCN_Insert' reported the ErrorCode: " + _ErrorCode);
            }
            foreach (var item in orderInf.DS_NBH)
            {
                InsertCT(item);
            }
            return orderInf.So_ID;
        }

        private void InsertCT(PA_CT item)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@ID", item.ID);
            dp.Add("@So_ID", item.So_ID);
            dp.Add("@HoTen", item.HoTen);
            dp.Add("@QH", item.QH);
            dp.Add("@GioiTinh", item.GioiTinh);
            dp.Add("@NgSinh", item.NgSinh);
            dp.Add("@CMND", item.CMND);
            dp.Add("@CTBH", item.CTBH);
            dp.Add("@Phi_BH", item.Phi_BH);
            dp.Add("@Tien_BH", item.Tien_BH);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblPA_CT_Insert", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);
        }

        public decimal Update(CN_PA_GCN orderInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@So_ID", orderInf.So_ID);
            dp.Add("@Ma_Dvi", orderInf.Ma_Dvi);
            dp.Add("@So_HD", orderInf.So_HD);
            dp.Add("@KhachHang", orderInf.KhachHang);
            dp.Add("@CMND", orderInf.CMND);
            dp.Add("@Phone", orderInf.Phone);
            dp.Add("@Email", orderInf.Email);
            dp.Add("@DChi", orderInf.DChi);

            dp.Add("@Gio_HL", orderInf.Gio_HL);
            dp.Add("@Gio_KT", orderInf.Gio_KT);
            dp.Add("@Ngay_HL", orderInf.Ngay_HL);
            dp.Add("@Ngay_KT", orderInf.Ngay_KT);
            dp.Add("@Ngay_TT", orderInf.Ngay_TT);
            dp.Add("@Ma_BH", orderInf.Ma_BH);
            dp.Add("@TT", orderInf.TT);
            dp.Add("@NSD", orderInf.NSD);
            dp.Add("@NQL", orderInf.NQL);
            dp.Add("@MA_DL", orderInf.MA_DL);
            dp.Add("@KENH_KT", orderInf.KENH_KT);
            dp.Add("@CTBH", orderInf.CTBH);

            dp.Add("@CODE_KM", orderInf.CODE_KM);
            dp.Add("@TL_KM", orderInf.TL_KM);
            dp.Add("@Phi_BH", orderInf.Phi_BH);
            dp.Add("@ThoiHan", orderInf.ThoiHan);

            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblPAGCN_Update", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);
            foreach (var item in orderInf.DS_NBH)
            {
                InsertCT(item);
            }
            //var _ErrorCode = dp.Get<int>("@iErrorCode");

            //if (_ErrorCode != 0)
            //{
            //    // Throw error.
            //    throw new Exception("Stored Procedure 'pr_tblOrder_Update' reported the ErrorCode: " + _ErrorCode);
            //}
            return orderInf.So_ID;
        }
        public IEnumerable<CN_PA_GCN> GetList(string CTBH, DateTime SDate, DateTime EDate)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@CTBH", CTBH);
            dp.Add("@SDate", SDate);
            dp.Add("@EDate", EDate);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            using (var multi = DbSession.Connection.QueryMultiple("pr_PAGCN_List", param: dp, commandType: CommandType.StoredProcedure))
            {
                return multi.Read<CN_PA_GCN>();

            }
        }

    }
}