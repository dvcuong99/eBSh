using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBSH.Models
{
    public class RefCode
    {
        public RefCode()
        {
            Ma_Dvi="000";
            Ma_DL = "";
            Kenh_KT = "";
            Ma_NSD = "";
            Ma_NQL = "";
            Ma_BH = "";
            CTBH = "";
            TL = 0;
        }
        [Required(ErrorMessage = "Nhập mã tham chiếu")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Mã đơn vị chưa nhập")]
        public string Ma_Dvi { get; set; }
        public string Ma_NQL { get; set; }
        public string Ma_NSD { get; set; }
        public string Ma_DL { get; set; }
        public string Kenh_KT { get; set; }
        [Required(ErrorMessage = "Chọn mã BH")]
        public string Ma_BH { get; set; }
        public string CTBH { get; set; }
        [Required(ErrorMessage = "Thông tin tỷ lệ khuyến mại của mã chưa nhập")]
        [Range(1, 100, ErrorMessage = "Tỷ lệ từ 1 đến 100")]
        public decimal TL { get; set; }
        public string MTN { get; set; }
        public decimal OrderId { get; set; }
        [DateLessThan("EndDate", ErrorMessage = "Ngày bắt đầu hiệu lực trước ngày kết thúc")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Nhập ngày hiệu lực của má tham chiếu")]
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class RefCodePageVM
    {
        public IEnumerable<RefCode> RefCodeList { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(RefCodeList.Count() / (double)ItemPerPage));
        }
        public IEnumerable<RefCode> PaginatedList()
        {
            int start = (CurrentPage - 1) * ItemPerPage;
            return RefCodeList.OrderBy(b => b.Code).Skip(start).Take(ItemPerPage);
        }

    }
}