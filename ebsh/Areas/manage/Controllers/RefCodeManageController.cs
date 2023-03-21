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
using NLog.Fluent;
using NLog;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace eBSH.Areas.manage.Controllers
{
    public class RefCodeManageController : Controller
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        IRefCodeRepository refCodeRepository;
        public RefCodeManageController(IRefCodeRepository refCodeRepository)
        {
            this.refCodeRepository = refCodeRepository;
        }
        public ActionResult Index(string MA_CTBH, string MA_BH, string Ma_Dvi="", int Page = 1)
        {
            ViewBag.MaDvi = Ma_Dvi;
            LoadList();

            var data = refCodeRepository.GetList(MA_CTBH, MA_BH, Ma_Dvi);
            var refCodePageVM = new RefCodePageVM
            {
                ItemPerPage = 10,
                RefCodeList = data,
                CurrentPage = Page
            };
            return View(refCodePageVM);
        }

        public ActionResult Create()
        {
            LoadList();
            ViewBag.Message = "";
            return View();
        }
        [HttpPost]
        public ActionResult Create(RefCode refCodeInf)
        {
            LoadList();
            if (ModelState.IsValid)
            {
                try
                {
                    var value = refCodeRepository.Insert(refCodeInf);
                    ViewBag.Message = String.Format("Thêm thành công mà code:{0}, Hạn sử dụng {1} đến {2}"
                        , refCodeInf.Code,refCodeInf.StartDate.ToString("dd/mm/yyyy"),refCodeInf.EndDate.ToString("dd/mm/yyyy"));
                    return View();
                }
                catch (Exception)
                {
                    ViewBag.Message = String.Format("Không thể thêm mã:{0}", refCodeInf.Ma_BH);
                    Log.Info(JsonConvert.SerializeObject(refCodeInf));
                    return View();
                }
            }
            else
            {
                ViewBag.Message = "";
                return View();
            }    
        }

        public ActionResult Detail(string Code)
        {
            RefCode refCodeInf = new RefCode();
            if (Code != null)
            {
                try
                {
                    refCodeInf = refCodeRepository.GetByCode(Code);
                    return View(refCodeInf);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống, hãy thử lại");
                    return View(refCodeInf);
                }
            }
            else
            {
                ModelState.AddModelError("", "Lỗi hệ thống, hãy thử lại");
                return View(refCodeInf);
            }
        }
        public void LoadList()
        {
            List<List_CTBH> lisCTBH = new List<List_CTBH>() { new List_CTBH { CTBH = "",CTBHName="Tất cả chương trình" }};
            List<List_MaBH> lisMaBH = new List<List_MaBH>() {new List_MaBH { MaBH = "2B",BHName="Xe máy" },new List_MaBH { MaBH = "Xe",BHName="Xe cơ giới" }};
            ViewBag.ListCTBH = lisCTBH;
            ViewBag.ListMaBH = lisMaBH;
        }

    }
}