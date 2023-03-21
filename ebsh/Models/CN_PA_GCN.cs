using eBSH.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace eBSH.Models
{
    public class CN_PA_GCN
    {
        public CN_PA_GCN()
        {
            So_ID = 0;
            KhachHang = "";
            TT = "T";
            Ma_Dvi = "000";
            Ma_BH = "TOPCARE";
            NQL = "";

            So_HD = "";
            CODE_KM = "";
            TL_KM = 0;
            DS_NBH = new List<PA_CT>();
        }

        public CN_PA_GCN(CN_PA_GCN_VM vmRegInfo)
        {
            So_ID = vmRegInfo.So_ID;
            ThoiHan = vmRegInfo.ThoiHan;
            So_HD = vmRegInfo.So_HD;
            KhachHang = vmRegInfo.KhachHang;
            DChi = vmRegInfo.DChi;
            CMND = vmRegInfo.CMND;
            Phone = vmRegInfo.Phone;
            Email = vmRegInfo.Email;
            if (vmRegInfo.Ngay_HL == DateTime.Today.ToString("dd/MM/yyyy"))
            {
                Gio_HL = vmRegInfo.Gio_HL = DateTime.Now.ToString("HH:mm");
                Gio_KT = vmRegInfo.Gio_KT = DateTime.Now.ToString("HH:mm");
            }
            else if (Common.ToDateTime(vmRegInfo.Ngay_HL) > DateTime.Today)
            {
                Gio_HL = vmRegInfo.Gio_HL = ("00:00");
                Gio_KT = vmRegInfo.Gio_KT = ("00:00");
            }

            Ngay_HL = vmRegInfo.Ngay_HL;
            Ngay_KT = vmRegInfo.Ngay_KT;
            Ngay_TT = vmRegInfo.Ngay_TT;
            Ma_BH = vmRegInfo.Ma_BH;
            Phi_BH = vmRegInfo.Phi_BH;
            TT = vmRegInfo.TT;
            Ma_Dvi = vmRegInfo.Ma_Dvi;
            CTBH = vmRegInfo.CTBH;
            MA_DL = vmRegInfo.MA_DL;
            KENH_KT = vmRegInfo.KENH_KT;
            NQL = vmRegInfo.NQL;
            NSD = vmRegInfo.NSD;
            CODE_KM = vmRegInfo.CODE_KM;
            TL_KM = vmRegInfo.TL_KM;

            DS_NBH = new List<PA_CT>();
            int i = 1;
            DS_NBH.Add(new PA_CT
            {
                ID = i,
                HoTen = vmRegInfo.HoTen1,
                QH = vmRegInfo.QH1,
                NgSinh = vmRegInfo.NgSinh1,
                GioiTinh = vmRegInfo.GioiTinh1,
                CMND = vmRegInfo.CMND1,
                Phi_BH = vmRegInfo.Phi_BHCT1,
                Tien_BH = vmRegInfo.Tien_BH,
                Phone = vmRegInfo.Phone,
                Email = vmRegInfo.Email,
                Gio_HL = vmRegInfo.Gio_HL,
                Gio_KT = vmRegInfo.Gio_KT,
                Ngay_HL = vmRegInfo.Ngay_HL,
                Ngay_KT = vmRegInfo.Ngay_KT,
                ND = vmRegInfo.CODE_KM + " " + vmRegInfo.TL_KM.ToString(),
            });

            if (vmRegInfo.HoTen2 != null && vmRegInfo.HoTen2.Length > 0)
            {
                i++;
                DS_NBH.Add(new PA_CT
                {
                    ID = i,
                    HoTen = vmRegInfo.HoTen2,
                    QH = vmRegInfo.QH2,
                    NgSinh = vmRegInfo.NgSinh2,
                    GioiTinh = vmRegInfo.GioiTinh2,
                    CMND = vmRegInfo.CMND2,
                    Phi_BH = vmRegInfo.Phi_BHCT2,
                    Tien_BH = vmRegInfo.Tien_BH,
                    Phone = vmRegInfo.Phone,
                    Email = vmRegInfo.Email,
                    Gio_HL = Gio_HL,
                    Gio_KT = Gio_KT,
                    Ngay_HL = vmRegInfo.Ngay_HL,
                    Ngay_KT = vmRegInfo.Ngay_KT,
                    ND = vmRegInfo.CODE_KM + " " + vmRegInfo.TL_KM.ToString(),
                });
            }

            if (vmRegInfo.HoTen3 != null && vmRegInfo.HoTen3.Length > 0)
            {
                i++;
                DS_NBH.Add(new PA_CT
                {
                    ID = i,
                    HoTen = vmRegInfo.HoTen3,
                    QH = vmRegInfo.QH3,
                    NgSinh = vmRegInfo.NgSinh3,
                    GioiTinh = vmRegInfo.GioiTinh3,
                    CMND = vmRegInfo.CMND3,
                    Phi_BH = vmRegInfo.Phi_BHCT3,
                    Tien_BH = vmRegInfo.Tien_BH,
                    Phone = vmRegInfo.Phone,
                    Email = vmRegInfo.Email,
                    Gio_HL = Gio_HL,
                    Gio_KT = Gio_KT,
                    Ngay_HL = vmRegInfo.Ngay_HL,
                    Ngay_KT = vmRegInfo.Ngay_KT,
                    ND = vmRegInfo.CODE_KM + " " + vmRegInfo.TL_KM.ToString(),
                });
            }

            if (vmRegInfo.HoTen4 != null && vmRegInfo.HoTen4.Length > 0)
            {
                i++;
                DS_NBH.Add(new PA_CT
                {
                    ID = i,
                    HoTen = vmRegInfo.HoTen4,
                    QH = vmRegInfo.QH4,
                    NgSinh = vmRegInfo.NgSinh4,
                    GioiTinh = vmRegInfo.GioiTinh4,
                    CMND = vmRegInfo.CMND4,
                    Phi_BH = vmRegInfo.Phi_BHCT4,
                    Tien_BH = vmRegInfo.Tien_BH,
                    Phone = vmRegInfo.Phone,
                    Email = vmRegInfo.Email,
                    Gio_HL = Gio_HL,
                    Gio_KT = Gio_KT,
                    Ngay_HL = vmRegInfo.Ngay_HL,
                    Ngay_KT = vmRegInfo.Ngay_KT,
                    ND = vmRegInfo.CODE_KM + " " + vmRegInfo.TL_KM.ToString(),
                });
            }

            if (vmRegInfo.HoTen5 != null && vmRegInfo.HoTen5.Length > 0)
            {
                i++;
                DS_NBH.Add(new PA_CT
                {
                    ID = i,
                    HoTen = vmRegInfo.HoTen5,
                    QH = vmRegInfo.QH5,
                    NgSinh = vmRegInfo.NgSinh5,
                    GioiTinh = vmRegInfo.GioiTinh5,
                    CMND = vmRegInfo.CMND5,
                    Phi_BH = vmRegInfo.Phi_BHCT5,
                    Tien_BH = vmRegInfo.Tien_BH,
                    Phone = vmRegInfo.Phone,
                    Email = vmRegInfo.Email,
                    Gio_HL = Gio_HL,
                    Gio_KT = Gio_KT,
                    Ngay_HL = vmRegInfo.Ngay_HL,
                    Ngay_KT = vmRegInfo.Ngay_KT,
                    ND = vmRegInfo.CODE_KM + " " + vmRegInfo.TL_KM.ToString(),
                });
            }
        }
        public string Ma_Dvi { get; set; }
        public decimal So_ID { get; set; } = 0;
        public decimal So_ID_D { get; set; } = 0;
        public int ThoiHan { get; set; }
        public string So_HD { get; set; }
        [Required(ErrorMessage = "Thông tin khách hàng chưa nhập")]
        public string KhachHang { get; set; }
        [Required(ErrorMessage = "Thông tin địa chỉ chưa nhập")]
        public string DChi { get; set; }
        public string MST { get; set; } = "";
        [Required(ErrorMessage = "Thông tin số căn cước chưa nhập")]
        public string CMND { get; set; } = "";
        public string So_SHK { get; set; } = "";
        public string Phone { get; set; }
        [Required(ErrorMessage = "Thông tin email chưa nhập")]
        public string Email { get; set; }
        public string Gio_HL { get; set; }
        public string Gio_KT { get; set; }
        public string Ngay_HL { get; set; }
        public string Ngay_KT { get; set; }
        public string Ngay_TT { get; set; }
        public string Ma_BH { get; set; }
        public decimal Phi_BH { get; set; }
        public string TT { get; set; }
        public DateTime Ngay_HT { get; set; }
        public string NSD { get; set; }
        public string NQL { get; set; }
        public string MA_DL { get; set; }
        public string KENH_KT { get; set; }
        public string CTBH { get; set; }
        public string CODE_KM { get; set; }
        public int TL_KM { get; set; }
        public string EmailNSD { get; set; }
        public List<PA_CT> DS_NBH { get; set; }
    }

    public class PA_CT
    {
        public decimal ID { get; set; } = 0;
        public decimal So_ID { get; set; } = 0;
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string CMND { get; set; } = "";
        public string QH { get; set; }
        public string NgSinh { get; set; }
        public string ND { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CTBH { get; set; } = "";
        public decimal Tien_BH { get; set; }
        public decimal TL_Phi { get; set; } = 0;
        public decimal Phi_BH { get; set; } = 0;
        public string Gio_HL { get; set; }
        public string Gio_KT { get; set; }
        public string Ngay_HL { get; set; }
        public string Ngay_KT { get; set; }
    }

    public class CN_PA_GCN_VM : IValidatableObject
    {
        public CN_PA_GCN_VM()
        {
            So_ID = 0;
            KhachHang = "";
            TT = "T";
            Ma_Dvi = "000";
            Ma_BH = "TOPCARE";
            NQL = "";

            So_HD = "";
            CODE_KM = "";
            TL_KM = 0;
            lsThoiHan = new List<ThoiHanBH>();
            lsThoiHan.Add(new ThoiHanBH { ID = 3, TH = "3 Tháng" });
            lsThoiHan.Add(new ThoiHanBH { ID = 6, TH = "6 Tháng" });
            lsThoiHan.Add(new ThoiHanBH { ID = 12, TH = "1 Năm" });
            lsGioiTinh = new List<GioiTinh>();
            lsGioiTinh.Add(new GioiTinh { ID = 0, GT = "Nam" });
            lsGioiTinh.Add(new GioiTinh { ID = 1, GT = "Nữ" });
            lsQH = new List<QuanHe>();
            lsQH.Add(new QuanHe { ID = "Bản thân", QH = "Bản thân" });
            lsQH.Add(new QuanHe { ID = "Bố/Mẹ đẻ", QH = "Bố/Mẹ đẻ" });
            lsQH.Add(new QuanHe { ID = "Bố/Mẹ của vợ/chồng", QH = "Bố/Mẹ của vợ/chồng" });
            lsQH.Add(new QuanHe { ID = "Vợ/Chồng", QH = "Vợ/Chồng" });
            lsQH.Add(new QuanHe { ID = "Con đẻ/nuôi hợp pháp", QH = "Con đẻ/nuôi hợp pháp" });
            lsQH.Add(new QuanHe { ID = "Anh/Chị/Em ruột", QH = "Anh/Chị/Em ruột" });
            lsQH.Add(new QuanHe { ID = "Người khác nếu có QLBH", QH = "Người khác nếu có QLBH" });
        }

        public CN_PA_GCN_VM(CN_PA_GCN pa_regInf)
        {
            Ma_Dvi = pa_regInf.Ma_Dvi;
            So_ID = pa_regInf.So_ID;
            ThoiHan = pa_regInf.ThoiHan;
            So_HD = pa_regInf.So_HD;
            KhachHang = pa_regInf.KhachHang;
            DChi = pa_regInf.DChi;
            CMND = pa_regInf.CMND;
            Phone = pa_regInf.Phone;
            Email = pa_regInf.Email;
            Gio_HL = pa_regInf.Gio_HL;
            Gio_KT = pa_regInf.Gio_KT;
            Ngay_HL = pa_regInf.Ngay_HL;
            Ngay_KT = pa_regInf.Ngay_KT;
            Ngay_TT = pa_regInf.Ngay_TT;
            CTBH = pa_regInf.CTBH;
            Ma_BH = pa_regInf.Ma_BH;
            Phi_BH = pa_regInf.Phi_BH;
            TT = pa_regInf.TT;
            KENH_KT = pa_regInf.KENH_KT;
            MA_DL = pa_regInf.MA_DL;
            NQL = pa_regInf.NQL;
            NSD = pa_regInf.NSD;

            CODE_KM = pa_regInf.CODE_KM;
            TL_KM = pa_regInf.TL_KM;

            lsThoiHan = new List<ThoiHanBH>();
            lsThoiHan.Add(new ThoiHanBH { ID = 3, TH = "3 Tháng" });
            lsThoiHan.Add(new ThoiHanBH { ID = 6, TH = "6 Tháng" });
            lsThoiHan.Add(new ThoiHanBH { ID = 12, TH = "1 Năm" });

            lsGioiTinh = new List<GioiTinh>();
            lsGioiTinh.Add(new GioiTinh { ID = 0, GT = "Nam" });
            lsGioiTinh.Add(new GioiTinh { ID = 1, GT = "Nữ" });

            lsQH = new List<QuanHe>();
            lsQH.Add(new QuanHe { ID = "Bản thân", QH = "Bản thân" });
            lsQH.Add(new QuanHe { ID = "Bố/Mẹ đẻ", QH = "Bố/Mẹ đẻ" });
            lsQH.Add(new QuanHe { ID = "Bố/Mẹ của vợ/chồng", QH = "Bố/Mẹ của vợ/chồng" });
            lsQH.Add(new QuanHe { ID = "Vợ/Chồng", QH = "Vợ/Chồng" });
            lsQH.Add(new QuanHe { ID = "Con đẻ/nuôi hợp pháp", QH = "Con đẻ/nuôi hợp pháp" });
            lsQH.Add(new QuanHe { ID = "Anh/Chị/Em ruột", QH = "Anh/Chị/Em ruột" });
            lsQH.Add(new QuanHe { ID = "Người khác nếu có QLBH", QH = "Người khác nếu có QLBH" });

            if (pa_regInf.DS_NBH != null && pa_regInf.DS_NBH.Count > 0)
            {
                Ref_ID1 = pa_regInf.DS_NBH[0].ID;
                HoTen1 = pa_regInf.DS_NBH[0].HoTen;
                QH1 = pa_regInf.DS_NBH[0].QH;
                NgSinh1 = pa_regInf.DS_NBH[0].NgSinh;
                GioiTinh1 = pa_regInf.DS_NBH[0].GioiTinh;
                CMND1 = pa_regInf.DS_NBH[0].CMND;
                Phi_BHCT1 = pa_regInf.DS_NBH[0].Phi_BH;

                if (pa_regInf.DS_NBH.Count > 1)
                {
                    Ref_ID2 = pa_regInf.DS_NBH[1].ID;
                    HoTen2 = pa_regInf.DS_NBH[1].HoTen;
                    QH2 = pa_regInf.DS_NBH[1].QH;
                    NgSinh2 = pa_regInf.DS_NBH[1].NgSinh;
                    GioiTinh2 = pa_regInf.DS_NBH[1].GioiTinh;
                    CMND2 = pa_regInf.DS_NBH[1].CMND;
                    Phi_BHCT2 = pa_regInf.DS_NBH[1].Phi_BH;
                }

                if (pa_regInf.DS_NBH.Count > 2)
                {
                    Ref_ID3 = pa_regInf.DS_NBH[2].ID;
                    HoTen3 = pa_regInf.DS_NBH[2].HoTen;
                    QH3 = pa_regInf.DS_NBH[2].QH;
                    NgSinh3 = pa_regInf.DS_NBH[2].NgSinh;
                    GioiTinh3 = pa_regInf.DS_NBH[2].GioiTinh;
                    CMND3 = pa_regInf.DS_NBH[2].CMND;
                    Phi_BHCT3 = pa_regInf.DS_NBH[2].Phi_BH;
                }
                if (pa_regInf.DS_NBH.Count > 3)
                {
                    Ref_ID4 = pa_regInf.DS_NBH[3].ID;
                    HoTen4 = pa_regInf.DS_NBH[3].HoTen;
                    QH4 = pa_regInf.DS_NBH[3].QH;
                    NgSinh4 = pa_regInf.DS_NBH[3].NgSinh;
                    GioiTinh4 = pa_regInf.DS_NBH[3].GioiTinh;
                    CMND4 = pa_regInf.DS_NBH[3].CMND;
                    Phi_BHCT4 = pa_regInf.DS_NBH[3].Phi_BH;
                }
                if (pa_regInf.DS_NBH.Count > 4)
                {
                    Ref_ID5 = pa_regInf.DS_NBH[4].ID;
                    HoTen5 = pa_regInf.DS_NBH[4].HoTen;
                    QH5 = pa_regInf.DS_NBH[4].QH;
                    NgSinh5 = pa_regInf.DS_NBH[4].NgSinh;
                    GioiTinh5 = pa_regInf.DS_NBH[4].GioiTinh;
                    CMND5 = pa_regInf.DS_NBH[4].CMND;
                    Phi_BHCT5 = pa_regInf.DS_NBH[4].Phi_BH;
                }
            }
        }

        public string Ma_Dvi { get; set; }
        public decimal So_ID { get; set; } = 0;
        public int ThoiHan { get; set; }
        public string So_HD { get; set; }
        [Required(ErrorMessage = "Thông tin khách hàng chưa nhập")]
        public string KhachHang { get; set; }
        [Required(ErrorMessage = "Thông tin địa chỉ chưa nhập")]
        public string DChi { get; set; }
        public string MST { get; set; } = "";
        [Required(ErrorMessage = "Thông tin số căn cước chưa nhập")]
        public string CMND { get; set; } = "";
        public string Phone { get; set; }
        [Required(ErrorMessage = "Thông tin email chưa nhập")]
        public string Email { get; set; }
        public string Gio_HL { get; set; }
        public string Gio_KT { get; set; }
        public string Ngay_HL { get; set; }
        public string Ngay_KT { get; set; }
        public string Ngay_TT { get; set; }
        public string Ma_BH { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0}", ApplyFormatInEditMode = true)]
        public decimal Phi_BH { get; set; }
        public string TT { get; set; }
        public DateTime Ngay_HT { get; set; }
        public string NSD { get; set; }
        public string NQL { get; set; }
        public string MA_DL { get; set; }
        public string KENH_KT { get; set; }
        public string CTBH { get; set; }
        public string CODE_KM { get; set; }
        public int TL_KM { get; set; }
        public decimal Tien_BH { get; set; }

        public List<ThoiHanBH> lsThoiHan { get; set; }
        public List<QuanHe> lsQH { get; set; }
        public List<GioiTinh> lsGioiTinh { get; set; }

        public decimal Ref_ID1 { get; set; } = 0;
        public string HoTen1 { get; set; }
        public string QH1 { get; set; }
        public string NgSinh1 { get; set; }
        public string GioiTinh1 { get; set; }
        public string CMND1 { get; set; } = "";
        [DisplayFormat(DataFormatString = "{0:#,##0}", ApplyFormatInEditMode = true)]
        public decimal Phi_BHCT1 { get; set; } = 0;
        public decimal Ref_ID2 { get; set; } = 0;
        public string HoTen2 { get; set; }
        public string QH2 { get; set; }
        public string NgSinh2 { get; set; }
        public string GioiTinh2 { get; set; }
        public string CMND2 { get; set; } = "";
        [DisplayFormat(DataFormatString = "{0:#,##0}", ApplyFormatInEditMode = true)]
        public decimal Phi_BHCT2 { get; set; } = 0;

        public decimal Ref_ID3 { get; set; } = 0;
        public string HoTen3 { get; set; }
        public string QH3 { get; set; }
        public string NgSinh3 { get; set; }
        public string GioiTinh3 { get; set; }
        public string CMND3 { get; set; } = "";
        [DisplayFormat(DataFormatString = "{0:#,##0}", ApplyFormatInEditMode = true)]
        public decimal Phi_BHCT3 { get; set; } = 0;

        public decimal Ref_ID4 { get; set; } = 0;
        public string HoTen4 { get; set; }
        public string QH4 { get; set; }
        public string NgSinh4 { get; set; }
        public string GioiTinh4 { get; set; }
        public string CMND4 { get; set; } = "";
        [DisplayFormat(DataFormatString = "{0:#,##0}", ApplyFormatInEditMode = true)]
        public decimal Phi_BHCT4 { get; set; } = 0;

        public decimal Ref_ID5 { get; set; } = 0;
        public string HoTen5 { get; set; }
        public string QH5 { get; set; }
        public string NgSinh5 { get; set; }
        public string GioiTinh5 { get; set; }
        public string CMND5 { get; set; } = "";

        [DisplayFormat(DataFormatString = "{0:#,##0}", ApplyFormatInEditMode = true)]
        public decimal Phi_BHCT5 { get; set; } = 0;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var model = (CN_PA_GCN_VM)validationContext.ObjectInstance;
            if (model.HoTen1 != null && model.HoTen1.Trim().Length > 0)
            {
                if (model.NgSinh1 == null || model.NgSinh1.Trim().Length == 0)
                    yield return new ValidationResult("Yêu cầu nhập ngày sinh ", new string[] { "HoTen1" });
                else
                {
                    var date = Common.ToDateTime(model.NgSinh1);
                    if (date == DateTime.MinValue)
                        yield return new ValidationResult("Ngày sinh không hợp lệ", new string[] { "HoTen1" });
                    else if (date > DateTime.Today.AddDays(60) || date < DateTime.Today.AddMonths(-780))
                        yield return new ValidationResult("Độ tuổi tham gia bảo hiểm từ 60 ngày tuổi đến 65 tuổi ", new string[] { "HoTen1" });
                }

                if (model.CMND1 == null || model.CMND1.Trim().Length == 0)
                    yield return new ValidationResult("Yêu cầu nhập số căn cước/CMND hoặc số giấy khai sinh ", new string[] { "HoTen1" });
                if (model.Phi_BHCT1 <= 0)
                    yield return new ValidationResult("Phí bảo hiểm không hợp lệ ", new string[] { "HoTen1" });

            }

            if (model.HoTen2 != null && model.HoTen2.Trim().Length > 0)
            {
                if (model.NgSinh2 == null || model.NgSinh2.Trim().Length == 0)
                    yield return new ValidationResult("Yêu cầu nhập ngày sinh ", new string[] { "HoTen2" });
                else
                {
                    var date = Common.ToDateTime(model.NgSinh2);
                    if (date == DateTime.MinValue)
                        yield return new ValidationResult("Ngày sinh không hợp lệ", new string[] { "HoTen2" });
                    else if (date > DateTime.Today.AddDays(60) || date < DateTime.Today.AddMonths(-780))
                        yield return new ValidationResult("Độ tuổi tham gia bảo hiểm từ 60 ngày tuổi đến 65 tuổi ", new string[] { "HoTen2" });
                }

                if (model.CMND2 == null || model.CMND2.Trim().Length == 0)
                    yield return new ValidationResult("Yêu cầu nhập số căn cước/CMND hoặc số giấy khai sinh ", new string[] { "HoTen2" });
                if (model.Phi_BHCT2 <= 0)
                    yield return new ValidationResult("Phí bảo hiểm không hợp lệ ", new string[] { "HoTen2" });
            }

            if (model.HoTen3 != null && model.HoTen3.Trim().Length > 0)
            {
                if (model.NgSinh3 == null || model.NgSinh3.Trim().Length == 0)
                    yield return new ValidationResult("Yêu cầu nhập ngày sinh ", new string[] { "HoTen3" });
                else
                {
                    var date = Common.ToDateTime(model.NgSinh3);
                    if (date == DateTime.MinValue)
                        yield return new ValidationResult("Ngày sinh không hợp lệ", new string[] { "HoTen3" });
                    else if (date > DateTime.Today.AddDays(60) || date < DateTime.Today.AddMonths(-780))
                        yield return new ValidationResult("Độ tuổi tham gia bảo hiểm từ 60 ngày tuổi đến 65 tuổi ", new string[] { "HoTen3" });
                }

                if (model.CMND3 == null || model.CMND3.Trim().Length == 0)
                    yield return new ValidationResult("Yêu cầu nhập số căn cước/CMND hoặc số giấy khai sinh ", new string[] { "HoTen3" });
                if (model.Phi_BHCT3 <= 0)
                    yield return new ValidationResult("Phí bảo hiểm không hợp lệ ", new string[] { "HoTen3" });
            }

            if (model.HoTen4 != null && model.HoTen4.Trim().Length > 0)
            {
                if (model.NgSinh4 == null || model.NgSinh4.Trim().Length == 0)
                    yield return new ValidationResult("Yêu cầu nhập ngày sinh ", new string[] { "HoTen4" });
                else
                {
                    var date = Common.ToDateTime(model.NgSinh4);
                    if (date == DateTime.MinValue)
                        yield return new ValidationResult("Ngày sinh không hợp lệ", new string[] { "HoTen4" });
                    else if (date > DateTime.Today.AddDays(60) || date < DateTime.Today.AddMonths(-780))
                        yield return new ValidationResult("Độ tuổi tham gia bảo hiểm từ 60 ngày tuổi đến 65 tuổi ", new string[] { "HoTen4" });
                }

                if (model.CMND4 == null || model.CMND4.Trim().Length == 0)
                    yield return new ValidationResult("Yêu cầu nhập số căn cước/CMND hoặc số giấy khai sinh ", new string[] { "HoTen4" });
                if (model.Phi_BHCT4 <= 0)
                    yield return new ValidationResult("Phí bảo hiểm không hợp lệ ", new string[] { "HoTen4" });
            }

            if (model.HoTen5 != null && model.HoTen5.Trim().Length > 0)
            {
                if (model.NgSinh5 == null || model.NgSinh5.Trim().Length == 0)
                    yield return new ValidationResult("Yêu cầu nhập ngày sinh ", new string[] { "HoTen5" });
                else
                {
                    var date = Common.ToDateTime(model.NgSinh5);
                    if (date == DateTime.MinValue)
                        yield return new ValidationResult("Ngày sinh không hợp lệ", new string[] { "HoTen5" });
                    else if (date > DateTime.Today.AddDays(60) || date < DateTime.Today.AddMonths(-780))
                        yield return new ValidationResult("Độ tuổi tham gia bảo hiểm từ 60 ngày tuổi đến 65 tuổi ", new string[] { "HoTen5" });
                }

                if (model.CMND5 == null || model.CMND5.Trim().Length == 0)
                    yield return new ValidationResult("Yêu cầu nhập số căn cước/CMND hoặc số giấy khai sinh ", new string[] { "HoTen5" });
                if (model.Phi_BHCT5 <= 0)
                    yield return new ValidationResult("Phí bảo hiểm không hợp lệ ", new string[] { "HoTen5" });
            }
        }
    }

    public class ThoiHanBH
    {
        public int ID { get; set; }
        public string TH { get; set; }
    }
    public class QuanHe
    {
        public string ID { get; set; }
        public string QH { get; set; }
    }
    public class GioiTinh
    {
        public int ID { get; set; }
        public string GT { get; set; }
    }
    public class List_CTBH
    {
        public string CTBH { get; set; }
        public string CTBHName { get; set; }
    }
    public class List_MaBH
    {
        public string MaBH { get; set; }
        public string BHName { get; set; }
    }

    public class PAPaginatedVM
    {
        public IEnumerable<CN_PA_GCN> PAList { get; set; }
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int PageCount()
        {
            return Convert.ToInt32(Math.Ceiling(PAList.Count() / (double)ItemPerPage));
        }
        public IEnumerable<CN_PA_GCN> PaginatedList()
        {
            int start = (CurrentPage - 1) * ItemPerPage;
            return PAList.OrderByDescending(b => b.Ngay_HL).Skip(start).Take(ItemPerPage);
        }

    }

}