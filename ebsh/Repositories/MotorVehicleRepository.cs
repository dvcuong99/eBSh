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
    public class MotorVehicleRepository:IMotorVehicleRepository
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        IDbSession DbSession = null;
        public MotorVehicleRepository(IDbSession DbSession)
        {
            this.DbSession = DbSession;
        }
        public MotorVehicle GetByID(decimal ID)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@so_id", ID);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
            return DbSession.Connection.QuerySingle<MotorVehicle>("pr_tblMotorVehicle_SelectOne", param: dp, commandType: CommandType.StoredProcedure);
        }
        public decimal Insert(MotorVehicle mvcInf)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@so_id", mvcInf.so_id);
            dp.Add("@so_hd", mvcInf.so_hd);
            dp.Add("@ten", mvcInf.ten);
            dp.Add("@dien_thoai", mvcInf.dien_thoai);
            dp.Add("@email", mvcInf.email);
            dp.Add("@dchi", mvcInf.dchi_kh);
            dp.Add("@bien_xe", mvcInf.bien_xe);
            dp.Add("@hieu_xe", mvcInf.hieu_xe);
            dp.Add("@hang_xe", mvcInf.hang_xe);
            dp.Add("@so_may", mvcInf.so_may);
            dp.Add("@so_khung", mvcInf.so_khung);
            dp.Add("@ma_bh", mvcInf.ma_bh);
            dp.Add("@so_cn", mvcInf.so_cn);
            dp.Add("@gio_hl", mvcInf.gio_hl);
            dp.Add("@ngay_hl", mvcInf.ngay_hl);
            dp.Add("@gio_kt", mvcInf.gio_kt);
            dp.Add("@ngay_kt", mvcInf.ngay_kt);
            dp.Add("@RefCode", mvcInf.refcode);
            dp.Add("@ma_dvi", mvcInf.ma_dvi);
            dp.Add("@ma_dl", mvcInf.ma_dl);
            dp.Add("@kenh_kt", mvcInf.kenh_kt);
            dp.Add("@cb_ql", mvcInf.cb_ql);
            dp.Add("@cb_du", mvcInf.cb_du);

            dp.Add("@tienbb_ng", mvcInf.tienbb_ng);
            dp.Add("@tienbb_ts", mvcInf.tienbb_ts);
            dp.Add("@phibb_ds", mvcInf.phibb_ds);
            dp.Add("@tienbh_ntx", mvcInf.tienbh_ntx);
            dp.Add("@phibh_ntx", mvcInf.phibh_ntx);


            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            var res = DbSession.Connection.Execute("dbo.pr_tblMotorVehicle_Insert", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);

            var _ErrorCode = dp.Get<int>("@iErrorCode");

            if (_ErrorCode != 0)
            {
                throw new Exception("Stored Procedure 'pr_tblMotorVehicle_Insert' reported the ErrorCode: " + _ErrorCode);
            }
            return mvcInf.so_id;
        }
        public decimal Update(MotorVehicle mcInf)
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

            var res = DbSession.Connection.Execute("pr_tblMotorVehicle_Update", dp, transaction: DbSession.Transaction, commandType: CommandType.StoredProcedure);
            return mcInf.so_id;
        }

        public IEnumerable<MotorVehicle> GetList(DateTime SDate, DateTime EDate, string MaDvi, string Kenh, string DaiLy)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@SDate", SDate);
            dp.Add("@EDate", EDate);
            dp.Add("@Ma_Dvi", MaDvi);
            dp.Add("@Ma_DL", DaiLy);
            dp.Add("@Kenh_KT", Kenh);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);

            using (var multi = DbSession.Connection.QueryMultiple("pr_tblMotorVehicle_List", param: dp, commandType: CommandType.StoredProcedure))
            {
                return multi.Read<MotorVehicle>();
            }
        }
    }
}