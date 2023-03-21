using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace eBSH.Models
{
    public class Motorcycle
    {
        public Motorcycle()
        {
            so_id = 0;
            ten = "";
            ttrang = "T";
            ma_dvi = "000";
            ma_bh = "";
            bien_xe = "";
            so_hd = "";
            tienbh_ng = 150000000;
            tienbh_ts = 50000000;
        }
        public string ma_dvi { get; set; }
        public decimal so_id { get; set; }
        public decimal so_id_dt { get; set; }
        public decimal so_id_d { get; set; }
        public decimal so_id_g { get; set; }
        public string RefCode { get; set; }
        public string so_hd { get; set; }
        public string cb_ql { get; set; }
        public string cb_du { get; set; }
        public string ma_dl { get; set; }
        public string gcn { get; set; }
        public string gcn_m { get; set; }
        public string gcn_c { get; set; }
        public string gcn_s { get; set; }
        public string makh { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên khách hàng")]
        public string ten { get; set; }
        [Required(ErrorMessage = "Chưa nhập địa chỉ chưa nhập")]
        public string dchi { get; set; }
        [Required(ErrorMessage = "Chưa nhập số điện thoại")]
        public string dien_thoai { get; set; }
        [Required(ErrorMessage = "Chưa nhập số Email")]
        public string email { get; set; }
        public string email_nsd { get; set; }
        public string nguoi_th { get; set; }
        public string dchi_th { get; set; }
        [BienSo("so_khung","so_may", ErrorMessage = "Nhập biển số hoặc số khung và số máy")]
        public string bien_xe { get; set; }
        public string hang_xe { get; set; }
        public string hieu_xe { get; set; }
        public string so_khung { get; set; }
        public string so_may { get; set; }
        public string ma_bh { get; set; }
        public int so_cn { get; set; }
        public string gio_hl { get; set; }
        public string ngay_hl { get; set; }
        public string gio_kt { get; set; }
        public string ngay_kt { get; set; }
        public string ngay_cap { get; set; }
        public string nd { get; set; }
        public decimal tienbh_ng { get; set; }
        public decimal tienbh_ts { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Chưa chọn loại xe")]
        public decimal phibh_ds { get; set; }
        public decimal tienbh_nn { get; set; }
        public decimal phibh_nn { get; set; }
        public decimal nam_sx { get; set; }
        public decimal gia_xe { get; set; }
        public decimal tienbh_vcx { get; set; }
        public decimal phibh_vcx { get; set; }
        public string mt { get; set; }
        public string kenh_kt { get; set; }
        public string ma_pgd { get; set; }
        public string ma_cbgd { get; set; }
        public string ttrang { get; set; }
        public string gcn_dt { get; set; }
        public string in_gcn { get; set; }
        public List<DKBS_2B> DKBS { get; set; }
    }
    public class BienSoAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty1;
        private readonly string _comparisonProperty2;

        public BienSoAttribute(string comparisonProperty1, string comparisonProperty2)
        {
            _comparisonProperty1 = comparisonProperty1;
            _comparisonProperty2 = comparisonProperty2;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (string)value;

            var property1 = validationContext.ObjectType.GetProperty(_comparisonProperty1);
            var property2 = validationContext.ObjectType.GetProperty(_comparisonProperty2);

            if (property1 == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue1 = (string)property1.GetValue(validationContext.ObjectInstance);
            var comparisonValue2 = (string)property2.GetValue(validationContext.ObjectInstance);

            if(currentValue == null || currentValue.Trim() == "")
            {
                if((comparisonValue1 == null || comparisonValue1.Trim() == "") || (comparisonValue2 == null || comparisonValue2.Trim() == ""))
                {
                    return new ValidationResult(ErrorMessage);
                }
                return ValidationResult.Success;
            }
            return ValidationResult.Success;

        }
    }
    public class DKBS_2B
    {
        public decimal dkbs_so_id_dt { get; set; }
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
    public class HangXe
    {
        public string Ma { get; set; }
    }
    public class HieuXe
    {
        public string Ma { get; set; }
    }
    public class ThoiHan
    {
        public int ID { get; set; }
        public string TH { get; set; }
    }
    public class LoaiXe
    {
        public string ID { get; set; }
        public string LX { get; set; }
    }
    public class TNN
    {
        public decimal Phi { get; set; }
        public string STBH { get; set; }
    }
    public class MCPaginatedVM
    {
        public IEnumerable<Motorcycle> MCList { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(MCList.Count() / (double)ItemPerPage));
        }
        public IEnumerable<Motorcycle> PaginatedList()
        {
            int start = (CurrentPage - 1) * ItemPerPage;
            return MCList.OrderByDescending(b => b.ngay_hl).Skip(start).Take(ItemPerPage);
        }

    }
}