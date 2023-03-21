using ClosedXML.Excel;
using eBSH.Models;
using eBSH.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using DocumentFormat.OpenXml.Bibliography;
using System.Runtime.InteropServices.ComTypes;

namespace eBSH.Areas.manage.Controllers
{
    public class MCManageController : Controller
    {
        IMotorcycleRepository motorcycleRepository;
        public MCManageController(IMotorcycleRepository motorcycleRepository)
        {
            this.motorcycleRepository = motorcycleRepository;
        }
        public ActionResult Index(string SDate, string EDate, string MaDvi = "all", string Kenh = "all", string DaiLy = "all", int Page = 1)
        {
            if (SDate == null)
                SDate = DateTime.Today.AddMonths(-1).AddYears(-5).ToString("dd/MM/yyyy");
            if (EDate == null)
                EDate = DateTime.Today.ToString("dd/MM/yyyy");
            DateTime _SDate = Helper.Common.ToDateTime(SDate);
            DateTime _EDate = Helper.Common.ToDateTime(EDate);

            ViewBag.SDate = SDate;
            ViewBag.EDate = EDate;

            string _maDvi = (MaDvi == null || MaDvi.Trim() == "" ? "all" : MaDvi);
            string _maKenh = (Kenh == null || Kenh.Trim() == "" ? "all" : Kenh);
            string _maDaiLy = (DaiLy == null || DaiLy.Trim() == "" ? "all" : DaiLy);

            ViewBag.MaDvi = _maDvi;
            ViewBag.Kenh = _maKenh;
            ViewBag.DaiLy = _maDaiLy;

            ViewBag.MaDvi_text = (MaDvi == "all" ? "" : MaDvi).Trim();
            ViewBag.Kenh_text = (Kenh == "all" ? "" : Kenh).Trim();
            ViewBag.DaiLy_text = (DaiLy == "all" ? "" : DaiLy).Trim();

            var data = motorcycleRepository.GetList(_SDate, _EDate, _maDvi.ToUpper(), _maKenh.ToUpper(), _maDaiLy.ToUpper());
            var mcCardPage = new MCPaginatedVM
            {

                ItemPerPage = 10,
                MCList = data,
                CurrentPage = Page
            };
            return View(mcCardPage);
        }

        public ActionResult Detail(decimal id)
        {
            Motorcycle mcInf = new Motorcycle();
            if (id != 0)
            {
                try
                {
                    mcInf = motorcycleRepository.GetByID(id);
                    return View(mcInf);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống cấp đơn, hãy thử lại");
                    return View(mcInf);
                }
            }
            else
            {
                ModelState.AddModelError("", "Lỗi hệ thống cấp đơn, hãy thử lại");
                return View(mcInf);
            }
        }

        public ActionResult ExportData(string MA_CTBH, string SDate, string EDate, string MaDvi, string Kenh, string DaiLy)
        {
            DateTime _SDate = Helper.Common.ToDateTime(SDate);
            DateTime _EDate = Helper.Common.ToDateTime(EDate);
            DataTable dt = new DataTable();
            string strTitle;
            strTitle = "Danh sách hợp đồng từ ngày " + SDate + " đến " + EDate;

            var data = motorcycleRepository.GetList(_SDate, _EDate, MaDvi, Kenh, DaiLy);
            dt = Helper.Common.ToDataTable<Motorcycle>(data.ToList());
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