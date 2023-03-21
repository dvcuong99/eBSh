using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBSH.Models
{
    public class PHH_CBS_GCN
    {
        public PHH_CBS_GCN()
        {
            SO_ID = 0;
            BANCAS_ID = 0;
            REF_ID = 0;
            MASTER_ID = 0;
            KH = "";
            THU_HUONG = "";
            SO_TK = "";
            TT = "T";
            MA_DVI = "000";           
            MA_BH = "BHCBS"; 
            NQL = "PNT03";
            TL_KM = 0;
            SO_HD = "";
            TEN_VI = "";
            CODE_KM = "";
            TL_KM = 0;
        }

        public string MA_DVI { get; set; }
        public decimal REF_ID { get; set; }
        public decimal SO_ID { get; set; }
        public decimal MASTER_ID { get; set; }
        public decimal BANCAS_ID { get; set; }
        public string SO_HD { get; set; }
        public string KH { get; set; }
        [Required(ErrorMessage = "Thông tin địa chỉ chưa nhập")]
        public string DCHIKH { get; set; }
        public string THU_HUONG { get; set; }
        [Required(ErrorMessage = "Thông tin họ người được bảo hiểm chưa nhập")]
        public string HO_TH { get; set; }
        [Required(ErrorMessage = "Thông tin tên người được bảo hiểm chưa nhập")]
        public string TEN_TH { get; set; }
        [Required(ErrorMessage ="Thông tin địa chỉ email chưa nhập")]
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
        public string TEN_VI { get; set; }
        [Required(ErrorMessage = "Thông tin Số tài khoản là bắt buộc.")]
        public string SO_TK { get; set; }
        [Required(ErrorMessage = "Thông tin ngân hàng là bắt buộc.")]
        public string NGAN_HANG { get; set; }
        [Required(ErrorMessage = "Số CMND/Hộ chiếu/Căn cước là bắt buộc")]
        [StringLength(20, ErrorMessage = "Số CMND/Hộ chiếu/Căn cước tối thiểu {2} ký tự.", MinimumLength = 6)]
        public string SO_CC { get; set; }
        [Required(ErrorMessage = "Ngày cấp CMND/Hộ chiếu/Căn cước chưa nhập")]
        public string NG_CAPCC { get; set; }
        [Required(ErrorMessage = "Nơi cấp chưa nhập")]
        public string NOI_CAPCC { get; set; }
        [Required(ErrorMessage = "Ngày hiệu lực chưa nhập")]
        public string NGAY_HL { get; set; }
        [Required(ErrorMessage = "Ngày hết hiệu lực chưa nhập")]
        public string NGAY_KT { get; set; }
        public string NGAY_CAP { get; set; }
        public string MA_BH { get; set; }
        public string MA_DL { get; set; }
        public string KENH { get; set; }
        public string MA_DT { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string CTBH { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string MTNBH { get; set; }
        public decimal TIENBH_U { get; set; }
        public decimal TIENBH_C { get; set; }
        public decimal TIENBH_I { get; set; }
        public decimal TIENBH_O { get; set; }
        public decimal TIENBH_R { get; set; }
        public decimal PHIBH { get; set; }
        public DateTime NGAY_NH { get; set; }
        public DateTime NGAY_HT { get; set; }
        public string NQL { get; set; }
        public string TT { get; set; }
        public string CODE_KM { get; set; }
        public int TL_KM { get; set; }
        public string TYPE { get; set; }
    }
    public class CBSExp
    {
        public decimal SO_ID { get; set; }
        public string SO_HD { get; set; }
        public string KH { get; set; }
        public string DCHIKH { get; set; }
        public string THU_HUONG { get; set; }
        public string HO_TH { get; set; }
        public string TEN_TH { get; set; }
        public string EMAIL { get; set; }
        public string PHONE { get; set; }
        public string TEN_VI { get; set; }
        public string SO_TK { get; set; }
        public string NGAN_HANG { get; set; }
        public string SO_CC { get; set; }
        public string NG_CAPCC { get; set; }
        public string NOI_CAPCC { get; set; }
        public string NGAY_HL { get; set; }
        public string NGAY_KT { get; set; }
        public string MA_BH { get; set; }
        public string MA_DL { get; set; }
        public string MA_DT { get; set; }
        public string CTBH { get; set; }
        public string MTNBH { get; set; }
        public decimal PHIBH { get; set; }
        public string NQL { get; set; }
        public string TT { get; set; }
    }

    public class CBSPaginatedVM
    {
        public IEnumerable<PHH_CBS_GCN> CBSList { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(CBSList.Count() / (double)ItemPerPage));
        }
        public IEnumerable<PHH_CBS_GCN> PaginatedList()
        {
            int start = (CurrentPage - 1) * ItemPerPage;
            return CBSList.OrderByDescending(b => b.NGAY_HT).Skip(start).Take(ItemPerPage);
        }

    }

}