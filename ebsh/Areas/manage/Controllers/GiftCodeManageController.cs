using eBSH.Models;
using eBSH.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBSH.Areas.manage.Controllers
{
    [Authorize]
    public class GiftCodeManageController : Controller
    {
        // GET: manage/QlyGiftCode
        IGCNRepository gcnRepository;
        IGiftCodeRepository giftCodeRepository;
        public GiftCodeManageController(IGCNRepository gcnRepository, IGiftCodeRepository giftCodeRepository)
        {
            this.gcnRepository = gcnRepository;
            this.giftCodeRepository = giftCodeRepository;
        }
        public ActionResult Index(string MA_BH,string MA_CTBH, string SDate, string EDate, int Page = 1)
        {
            if (SDate == null)
                SDate = DateTime.Today.AddMonths(-1).AddYears(-5).ToString("dd/MM/yyyy");
            if (EDate == null)
                EDate = DateTime.Today.ToString("dd/MM/yyyy");
            DateTime _SDate = Helper.Common.ToDateTime(SDate);
            DateTime _EDate = Helper.Common.ToDateTime(EDate);
            string _maCTBH = (MA_CTBH == null ? "all" : MA_CTBH);
            string _maBH = (MA_CTBH == null ? "all" : MA_BH);

            ViewBag.SDate = SDate;
            ViewBag.EDate = EDate;
            ViewBag.MA_CTBH = _maCTBH;
            LoadList();

            var data = giftCodeRepository.GetList(_maBH,_maCTBH, _SDate, _EDate);
            var giftCodePaginated = new GiftCodePaginatedVM
            {
                ItemPerPage = 10,
                GiftCodeList = data,
                CurrentPage = Page
            };
            return View(giftCodePaginated);
        }
        public ActionResult Create()
        {
            ViewBag.Message = "";
            LoadList();
            return View();
        }
        [HttpPost]
        public ActionResult Create(GiftCode giftCode)
        {
            LoadList();
            if (ModelState.IsValid)
            {
                try
                {
                    var value = giftCodeRepository.Insert(giftCode);
                    return RedirectToAction("Index", "QlyGiftCode");
                }
                catch (Exception)
                {
                    ViewBag.Message = String.Format("Không thể thêm mã:{0} ,Mã BH:{1}, Chương trình:{2}"
                        , giftCode.CodeKM,giftCode.Ma_BH,giftCode.CTBH);
                    return View();
                }
            }
            else
                return View();
        }
        public ActionResult Detail(string CodeKM,string Ma_BH,string CTBH )
        {
            GiftCode giftCodeInf = new GiftCode();
            if (CodeKM != null && Ma_BH != null && CTBH != null)
            {
                try
                {
                    giftCodeInf = giftCodeRepository.GetOne(CodeKM, Ma_BH, CTBH);
                    return View(giftCodeInf);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Lỗi hệ thống, hãy thử lại");
                    return View(giftCodeInf);
                }
            }
            else
            {
                ModelState.AddModelError("", "Lỗi hệ thống, hãy thử lại");
                return View(giftCodeInf);
            }
        }
        public void LoadList()
        {
            List<List_CTBH> lisCTBH = new List<List_CTBH>() { new List_CTBH { CTBH = "Gold",CTBHName="Gold" }
            ,new List_CTBH { CTBH = "Platinum",CTBHName="Platinum" },new List_CTBH { CTBH = "Titan",CTBHName="Titan" }};
            List<List_MaBH> lisMaBH = new List<List_MaBH>() { new List_MaBH { MaBH = "CBS",BHName="Cyber" }
            ,new List_MaBH { MaBH = "TopCare",BHName="TopCare" },new List_MaBH { MaBH = "BestCare",BHName="BestCare" }};
            ViewBag.ListCTBH = lisCTBH;
            ViewBag.ListMaBH = lisMaBH;
        }
    }
}