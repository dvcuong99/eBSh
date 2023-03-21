using eBSH.Models;
using eBSH.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eBSH.Areas.manage.Controllers
{
    [Authorize]
    public class TopCareManageController : Controller
    {
        IPA_GCNRepository paGCNRepository;
        public TopCareManageController(IPA_GCNRepository paGCNRepository)
        {
            this.paGCNRepository = paGCNRepository;
        }
        // GET: manage/QlyCyber
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

            var data = paGCNRepository.GetList(_maCTBH, _SDate, _EDate);
            var cbsCardPage = new PAPaginatedVM
            {
                ItemPerPage = 10,
                PAList = data,
                CurrentPage = Page
            };
            return View(cbsCardPage);
        }
        public ActionResult Detail(decimal id)
        {
            CN_PA_GCN cbsInf = new CN_PA_GCN();
            if (id != 0)
            {
                try
                {
                    cbsInf = paGCNRepository.GetByID(id);
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
    }
}
