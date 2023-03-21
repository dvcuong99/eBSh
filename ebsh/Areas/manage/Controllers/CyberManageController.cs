using ClosedXML.Excel;
using eBSH.Models;
using eBSH.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace eBSH.Areas.manage.Controllers
{
    [Authorize]
    public class CyberManageController : Controller
    {
        IGCNRepository gcnRepository;
        public CyberManageController(IGCNRepository gcnRepository)
        {
            this.gcnRepository = gcnRepository;
        }
        public ActionResult Index(string MA_CTBH, string SDate, string EDate, int Page = 1)
        {
            if (SDate == null)
                SDate = DateTime.Today.AddMonths(-1).AddYears(-5).ToString("dd/MM/yyyy");
            if (EDate == null)
                EDate = DateTime.Today.ToString("dd/MM/yyyy");
            DateTime _SDate = Helper.Common.ToDateTime(SDate);
            DateTime _EDate = Helper.Common.ToDateTime(EDate);
            string _maCTBH = (MA_CTBH == null ? "all" : MA_CTBH);
            List<List_CTBH> lisCTBH = new List<List_CTBH>() { new List_CTBH { CTBH = "all",CTBHName="Tất cả các gói" }, new List_CTBH { CTBH = "Gold",CTBHName="Gold" }
            ,new List_CTBH { CTBH = "Platinum",CTBHName="Platinum" },new List_CTBH { CTBH = "Titan",CTBHName="Titan" }};

            ViewBag.SDate = SDate;
            ViewBag.EDate = EDate;
            ViewBag.MA_CTBH = _maCTBH;
            ViewBag.ListCTBH = lisCTBH;

            var data = gcnRepository.GetList(_maCTBH, _SDate, _EDate);
            var cbsCardPage = new CBSPaginatedVM
            {

                ItemPerPage = 10,
                CBSList = data,
                CurrentPage = Page
            };
            return View(cbsCardPage);
        }
        public ActionResult Detail(string id)
        {
            PHH_CBS_GCN cbsInf = new PHH_CBS_GCN();
            if (id != null)
            {
                try
                {
                    cbsInf = gcnRepository.GetByID(id);
                    return View(cbsInf);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống cấp đơn, hãy thử lại");
                    return View(cbsInf);
                }
            }
            else
            {
                ModelState.AddModelError("", "Lỗi hệ thống cấp đơn, hãy thử lại");
                return View(cbsInf);
            }
        }
        public ActionResult ExportData(string MA_CTBH, string SDate, string EDate)
        {
            DateTime _SDate = Helper.Common.ToDateTime(SDate);
            DateTime _EDate = Helper.Common.ToDateTime(EDate);
            DataTable dt = new DataTable();
            string strTitle;
            strTitle = "Danh sách hợp đồng từ ngày " + SDate + " đến " + EDate;

            var data = gcnRepository.GetData(MA_CTBH, _SDate, _EDate);
            dt = Helper.Common.ToDataTable<CBSExp>(data.ToList());
            return ExportExcel(dt, strTitle);
        }
        public FileResult ExportExcel(DataTable dt, string loaiBC)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Sheet1");
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                IXLWorksheet worksheet = wb.Worksheet("Sheet1");
                worksheet.Row(1).InsertRowsAbove(4);
                worksheet.Cell(1, 1).Value = loaiBC;
                worksheet.Cell("A1").Style.Font.FontSize = 18;
                worksheet.Cell("A1").Style.Font.Bold = true;
                worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A1:G1").Merge();


                worksheet.Cell("A3").Value = "Tổng số dòng: " + dt.Rows.Count.ToString() + " Ngày xuất dữ liệu: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cell("A3").Style.Font.Bold = true;
                worksheet.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A3:G3").Merge();

                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    return File(MyMemoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", loaiBC + ".xlsx");
                }
            }
        }
    }
}