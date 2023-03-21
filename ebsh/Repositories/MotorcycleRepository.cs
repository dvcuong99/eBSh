using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using eBSH.Core;
using eBSH.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;

namespace eBSH.Repositories
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        IDbSession DbSession = null;
        public MotorcycleRepository(IDbSession DbSession)
        {
            this.DbSession = DbSession;
        }
        public Motorcycle GetByID(decimal ID)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@so_id", ID);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
            return DbSession.Connection.QuerySingle<Motorcycle>("pr_tblMotorcycle_SelectOne", param: dp, commandType: CommandType.StoredProcedure);
        }
        public decimal Insert(Motorcycle mcInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@so_id", mcInf.so_id);
            dp.Add("@so_hd", mcInf.so_hd);
            dp.Add("@ten", mcInf.ten);
            dp.Add("@dien_thoai", mcInf.dien_thoai);
            dp.Add("@email", mcInf.email);
            dp.Add("@dchi", mcInf.dchi);
            dp.Add("@bien_xe", mcInf.bien_xe);
            dp.Add("@hieu_xe", mcInf.hieu_xe);
            dp.Add("@hang_xe", mcInf.hang_xe);
            dp.Add("@so_may", mcInf.so_may);
            dp.Add("@so_khung", mcInf.so_khung);
            dp.Add("@ma_bh", mcInf.ma_bh);
            dp.Add("@so_cn", mcInf.so_cn);
            dp.Add("@gio_hl", mcInf.gio_hl);
            dp.Add("@ngay_hl", mcInf.ngay_kt);
            dp.Add("@gio_kt", mcInf.gio_kt);
            dp.Add("@ngay_kt", mcInf.ngay_kt);
            dp.Add("@RefCode", mcInf.RefCode);
            dp.Add("@ma_dvi", mcInf.ma_dvi);
            dp.Add("@ma_dl", mcInf.ma_dl);
            dp.Add("@kenh_kt", mcInf.kenh_kt);
            dp.Add("@cb_ql", mcInf.cb_ql);
            dp.Add("@cb_du", mcInf.cb_du);

            dp.Add("@tienbh_ng", mcInf.tienbh_ng);
            dp.Add("@tienbh_ts", mcInf.tienbh_ts);
            dp.Add("@phibh_ds", mcInf.phibh_ds);
            dp.Add("@tienbh_nn", mcInf.tienbh_nn);
            dp.Add("@phibh_nn", mcInf.phibh_nn);

            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblMotorcycle_Insert", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);

            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                throw new Exception("Stored Procedure 'pr_tblMotorcycle_Inserte_Insert' reported the ErrorCode: " + _ErrorCode);
            }
            return mcInf.so_id;
        }
        public decimal Update(Motorcycle mcInf)
        {
            DynamicParameters dp = new DynamicParameters();

            dp.Add("@so_id", mcInf.so_id);
            dp.Add("@so_hd", mcInf.so_hd);
            dp.Add("@ten", mcInf.ten);
            dp.Add("@dien_thoai", mcInf.dien_thoai);
            dp.Add("@email", mcInf.email);
            dp.Add("@dchi", mcInf.dchi);
            dp.Add("@bien_xe", mcInf.bien_xe);
            dp.Add("@hieu_xe", mcInf.hieu_xe);
            dp.Add("@hang_xe", mcInf.hang_xe);
            dp.Add("@so_may", mcInf.so_may);
            dp.Add("@so_khung", mcInf.so_khung);
            dp.Add("@ma_bh", mcInf.ma_bh);
            dp.Add("@so_cn", mcInf.so_cn);
            dp.Add("@gio_hl", mcInf.gio_hl);
            dp.Add("@ngay_hl", mcInf.ngay_kt);
            dp.Add("@gio_kt", mcInf.gio_kt);
            dp.Add("@ngay_kt", mcInf.ngay_kt);
            dp.Add("@ttrang", mcInf.ttrang);

            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("pr_tblMotorcycle_Update", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);
            return mcInf.so_id;
        }
        public IEnumerable<Motorcycle> GetList(DateTime SDate, DateTime EDate, string MaDvi, string Kenh, string DaiLy)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@SDate", SDate);
            dp.Add("@EDate", EDate);
            dp.Add("@Ma_Dvi", MaDvi);
            dp.Add("@Ma_DL", DaiLy);
            dp.Add("@Kenh_KT", Kenh);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            using (var multi = DbSession.Connection.QueryMultiple("pr_tblMotorCycle_List", param: dp, commandType: CommandType.StoredProcedure))
            {
                return multi.Read<Motorcycle>();
            }
        }
    }
}