using eBSH.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBSH.Models
{
    public class MotorVehicle
    {
        public MotorVehicle()
        {
        }
        public MotorVehicle(MotorVehicle_VM motovm)
        {
            so_hd = "";
            refcode= motovm.refcode;
            ma_dvi = motovm.ma_dvi;
            ma_dl = motovm.ma_dl;
            kenh_kt= motovm.kenh_kt;
            cb_du= motovm.cb_du;
            cb_ql= motovm.cb_ql;
             
            loai_xe = "XKKD";
            nhom_xe = "KH";
            tienbh_ntx = motovm.tienbh_ntx;
            phibh_ntx = motovm.phibh_ntx;
            ten_kh = motovm.ten_kh;
            ten = motovm.ten_kh;
            dchi_kh = motovm.dchi_kh;
            dien_thoai_kh = motovm.dien_thoai_kh;
            dchi = motovm.dchi;
            dien_thoai = motovm.dien_thoai_kh;
            gtinh = motovm.gtinh;
            email = motovm.email;
            nguoi_th = motovm.nguoi_th;
            dchi_th = motovm.dchi_th;
            bien_xe = motovm.bien_xe;
            hang_xe = motovm.hang_xe;
            hieu_xe = motovm.hieu_xe;
            so_khung = motovm.so_khung;
            so_may = motovm.so_may;
            ma_bh = motovm.ma_bh;
            //switch (ma_bh)
            //{
            //    case "":
            //        loai_xe = "XKKD";
            //        break;
            //    default:
            //        loai_xe = "XKKD";
            //        break;
            //}
            ttai = motovm.ttai;
            ttrang = motovm.ttrang;
            so_cn = motovm.so_cn;
            gcn_dt = "C";

            phibb_ds = motovm.phibb_ds;

            gio_hl = motovm.gio_hl;
            gio_kt = motovm.gio_kt;
            ngay_hl = motovm.ngay_hl;
            ngay_kt = motovm.ngay_kt;
            ngay_ht = motovm.ngay_ht;
            ngay_cap = motovm.ngay_hl;
            so_lpx = motovm.so_lpx;
            tienbb_ng = motovm.tienbb_ng;
            tienbb_ts = motovm.tienbb_ts;

            DS_DKBS = new List<GCN_DKbs_XE>();
        }
        public string ma_dvi { get; set; }
        public string cb_ql { get; set; }
        public string email_cb_ql { get; set; }
        public string refcode { get; set; }
        public string cb_du { get; set; }
        public string ma_dl { get; set; }
        public decimal so_id { get; set; }
        public decimal so_id_dt { get; set; }
        public decimal so_id_d { get; set; }
        public decimal so_id_g { get; set; }
        public string so_hd { get; set; }
        public string gcn { get; set; }
        public string gcn_m { get; set; }
        public string gcn_c { get; set; }
        public string gcn_s { get; set; }
        public string gcn_m_t { get; set; }
        public string gcn_c_t { get; set; }
        public string gcn_s_t { get; set; }
        public string makh { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên người mua")]
        public string ten_kh { get; set; }
        public string dchi_kh { get; set; }
        public string dien_thoai_kh { get; set; }
        public string ten { get; set; }
        public string dchi { get; set; }
        public string dien_thoai { get; set; }
        public string email { get; set; }
        public string gtinh { get; set; }
        public string nguoi_th { get; set; }
        public string dchi_th { get; set; }
        public string bien_xe { get; set; }
        public string so_khung { get; set; }
        public string so_may { get; set; }
        [RequiredIf(ErrorMessage = "Chưa chọn hãng xe")]
        public string hang_xe { get; set; }
        [RequiredIf(ErrorMessage = "Chưa chọn hiệu xe")]
        public string hieu_xe { get; set; }
        public string ma_bh { get; set; }
        public decimal ttai { get; set; }
        public decimal so_lpx { get; set; }
        public decimal so_cn { get; set; }
        public decimal nam_sx { get; set; }
        public decimal gia_xe { get; set; }
        public string nhom_xe { get; set; }
        public string loai_xe { get; set; }
        public string xuat_xu { get; set; }
        public string gio_hl { get; set; }
        public string ngay_hl { get; set; }
        public string gio_kt { get; set; }
        public string ngay_kt { get; set; }
        public string ngay_cap { get; set; }
        public string nd { get; set; }
        public string kenh_kt { get; set; }
        public string ma_pgd { get; set; }
        public string ma_cbgd { get; set; }
        public decimal tienbb_ng { get; set; }
        public decimal tienbb_ts { get; set; }
        public decimal phibb_ds { get; set; }
        public decimal tientn_ng { get; set; }
        public decimal tientn_ts { get; set; }
        public decimal tientn_hk { get; set; }
        public decimal tientn_hh { get; set; }
        public decimal phitn_ng { get; set; }
        public decimal phitn_ts { get; set; }
        public decimal phitn_hk { get; set; }
        public decimal phitn_hh { get; set; }
        public decimal tienbh_ntx { get; set; }
        public decimal phibh_ntx { get; set; }
        public decimal tienbh_vc { get; set; }
        public decimal phibh_vc { get; set; }
        public decimal tl_vc { get; set; }
        public decimal muc_mt { get; set; }
        public string ngay_ht { get; set; }
        public string ngay_qd { get; set; }
        public string ttrang { get; set; }
        public string gcn_dt { get; set; }
        public string in_gcn { get; set; }
        public List<HangXe> ListHangXe { get; set; }
        public List<HieuXe> ListHieuXe { get; set; }
        public List<GCN_DKbs_XE> DS_DKBS { get; set; }
    }

    public class MotorVehicle_VM
    {
        CoreNVService nvcoreSvc = CoreNVService.getInstance();
        public MotorVehicle_VM()
        {
            ListHangXe = new List<HangXe>();
            ListHangXe.Add(new HangXe { Ma = "-- Chọn hãng xe --" });
            ListHangXe.AddRange(AsyncUtil.RunSync<List<HangXe>>(() => nvcoreSvc.GetHangXe()));

            ListHieuXe = new List<HieuXe>();
            ListHieuXe.Add(new HieuXe { Ma = "-- Chọn hiệu xe --" });

            ListLoaiXe = new List<LoaiXe_XCG>();
            ListLoaiXe.Add(new LoaiXe_XCG { Ma = "0", Ten = "-- Chọn loại xe --" });
            ListLoaiXe.AddRange(AsyncUtil.RunSync<List<LoaiXe_XCG>>(() => nvcoreSvc.GetLoaiXe()));

            ListNhomXe = new List<NhomXe>();
            ListNhomXe.Add(new NhomXe { Ma = "0", Ten = "-- Chọn loại xe --" });
            ListNhomXe.AddRange(AsyncUtil.RunSync<List<NhomXe>>(() => nvcoreSvc.GetNhomXe()));

            listMaBHXe = new List<MaBHXe>();
            listMaBHXe.Add(new MaBHXe { Ma_BH = "0", Ten = "-- Chọn mục đích sử dụng --" });
            listMaBHXe.AddRange(AsyncUtil.RunSync<List<MaBHXe>>(() => nvcoreSvc.GetMaBHXe()));

            ListSTBH = new List<STBH>();
            ListSTBH.Add(new STBH { Phi = 0, SoTienBH = "-- Chọn số tiền bảo hiểm --" });
            ListSTBH.Add(new STBH { Phi = 10000, SoTienBH = "10 triệu đồng/vụ/người" });
            ListSTBH.Add(new STBH { Phi = 20000, SoTienBH = "20 triệu đồng/vụ/người" });
            ListSTBH.Add(new STBH { Phi = 30000, SoTienBH = "30 triệu đồng/vụ/người" });
            ListSTBH.Add(new STBH { Phi = 40000, SoTienBH = "40 triệu đồng/vụ/người" });
            ListSTBH.Add(new STBH { Phi = 50000, SoTienBH = "50 triệu đồng/vụ/người" });
            ListSTBH.Add(new STBH { Phi = 60000, SoTienBH = "60 triệu đồng/vụ/người" });
            ListSTBH.Add(new STBH { Phi = 70000, SoTienBH = "70 triệu đồng/vụ/người" });
            ListSTBH.Add(new STBH { Phi = 80000, SoTienBH = "80 triệu đồng/vụ/người" });
            ListSTBH.Add(new STBH { Phi = 90000, SoTienBH = "90 triệu đồng/vụ/người" });
            ListSTBH.Add(new STBH { Phi = 100000, SoTienBH = "100 triệu đồng/vụ/người" });

            ListBieuPhi = new List<BieuPhiXCGBB>();
            ListBieuPhi.Add(new BieuPhiXCGBB { Loai = "BT", Tien = 100000000, Phi = 0 });
            ListBieuPhi.Add(new BieuPhiXCGBB { Loai = "BN", Tien = 150000000, Phi = 873400 });
            ListBieuPhi.Add(new BieuPhiXCGBB { Loai = "BK", Tien = 0, Phi = 0 });

        }
        public string refcode { get; set; }
        public string ma_dvi { get; set; }
        public string cb_ql { get; set; }
        public string email_cb_ql { get; set; }
        public string cb_du { get; set; }
        public string ma_dl { get; set; }
        public decimal so_id { get; set; }
        public decimal so_id_dt { get; set; }
        public decimal so_id_d { get; set; }
        public decimal so_id_g { get; set; }
        public string so_hd { get; set; }
        public string gcn { get; set; }
        public string gcn_m { get; set; }
        public string gcn_c { get; set; }
        public string gcn_s { get; set; }
        public string gcn_m_t { get; set; }
        public string gcn_c_t { get; set; }
        public string gcn_s_t { get; set; }
        public string makh { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên")]
        public string ten_kh { get; set; }
        public string dchi_kh { get; set; }
        [Required(ErrorMessage = "Chưa nhập điện thoại")]
        public string dien_thoai_kh { get; set; }
        public string ten { get; set; }
        public string dchi { get; set; }
        public string dien_thoai { get; set; }
        [Required(ErrorMessage = "Chưa nhập địa chỉ email")]
        public string email { get; set; }
        public string gtinh { get; set; }
        public string nguoi_th { get; set; }
        public string dchi_th { get; set; }
        [BienSo("so_khung", "so_may", ErrorMessage = "Nhập biển số hoặc số khung và số máy")]
        public string bien_xe { get; set; }
        public string so_khung { get; set; }
        public string so_may { get; set; }
        [RequiredIf(ErrorMessage = "Chưa chọn hãng xe")]
        public string hang_xe { get; set; }
        [RequiredIf(ErrorMessage = "Chưa chọn hiệu xe")]
        public string hieu_xe { get; set; }
        [RequiredIf(ErrorMessage = "chưa chọn loại xe")]
        public string ma_bh { get; set; }
        public decimal ttai { get; set; }
        public decimal so_lpx { get; set; }
        public decimal so_cn { get; set; }
        public decimal nam_sx { get; set; }
        public decimal gia_xe { get; set; }
        public string nhom_xe { get; set; }
        public string loai_xe { get; set; }
        public string xuat_xu { get; set; }
        public string gio_hl { get; set; }
        public string ngay_hl { get; set; }
        public string gio_kt { get; set; }
        public string ngay_kt { get; set; }
        public string ngay_cap { get; set; }
        public string nd { get; set; }
        public string kenh_kt { get; set; }
        public string ma_pgd { get; set; }
        public string ma_cbgd { get; set; }
        [DisplayFormat(DataFormatString = "{0,00}", ApplyFormatInEditMode = true)]
        public decimal tienbb_ng { get; set; }
        public decimal tienbb_ts { get; set; }
        public decimal phibb_ds { get; set; }
        public decimal tientn_ng { get; set; }
        public decimal tientn_ts { get; set; }
        public decimal tientn_hk { get; set; }
        public decimal tientn_hh { get; set; }
        public decimal phitn_ng { get; set; }
        public decimal phitn_ts { get; set; }
        public decimal phitn_hk { get; set; }
        public decimal phitn_hh { get; set; }
        public decimal tienbh_ntx { get; set; }
        public decimal phibh_ntx { get; set; }
        public decimal tienbh_vc { get; set; }
        public decimal phibh_vc { get; set; }
        public decimal tl_vc { get; set; }
        public decimal muc_mt { get; set; }
        public string ngay_ht { get; set; }
        public string ngay_qd { get; set; }
        public string ttrang { get; set; }
        public string gcn_dt { get; set; }
        public string in_gcn { get; set; }
        public List<HangXe> ListHangXe { get; set; }
        public List<HieuXe> ListHieuXe { get; set; }
        public List<NhomXe> ListNhomXe { get; set; }
        public List<LoaiXe_XCG> ListLoaiXe { get; set; }
        public List<STBH> ListSTBH { get; set; }
        public List<MaBHXe> listMaBHXe { get; set; }
        public List<BieuPhiXCGBB> ListBieuPhi { get; set; }
        public List<GCN_DKbs_XE> DS_DKBS { get; set; }
    }

    public class GCN_DKbs_XE
    {
        public decimal dkbs_so_id_dt { get; set; }
        public decimal dkbs_id_dt { get; set; }
        public string dkbs_ma_dk { get; set; }
        public string dkbs_ma { get; set; }
        public string dkbs_nd { get; set; }
        public string dkbs_lh_nv { get; set; }
        public string dkbs_nt_tien { get; set; }
        public decimal dkbs_tien { get; set; }
        public decimal dkbs_pt { get; set; }
        public string dkbs_k_phi { get; set; }
        public decimal dkbs_phi { get; set; }
        public string dkbs_han_muc { get; set; }
    }

    public class LoaiXe_XCG
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
    }
    public class NhomXe
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
    }
    public class STBH
    {
        public decimal Phi { get; set; }
        public string SoTienBH { get; set; }
    }
    public class MaBHXe
    {
        public string Ma_BH { get; set; }
        public string Ten { get; set; }
        public int so_cn { get; set; }
        public string XE_KD { get; set; }
    }
    public class BieuPhiXCGBB
    {
        public string Loai { get; set; }
        public decimal Tien { get; set; }
        public decimal Phi { get; set; }
    }

    public class MVCPaginatedVM
    {
        public IEnumerable<MotorVehicle> MVCList { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(MVCList.Count() / (double)ItemPerPage));
        }
        public IEnumerable<MotorVehicle> PaginatedList()
        {
            int start = (CurrentPage - 1) * ItemPerPage;
            return MVCList.OrderByDescending(b => b.ngay_hl).Skip(start).Take(ItemPerPage);
        }

    }

    public class RequiredIf : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;
            string str = (string)value;
            if (str.Substring(0, 2) == "--" || str == null | str.Trim() == "")
                return false;
            else
                return true;
        }
    }

}