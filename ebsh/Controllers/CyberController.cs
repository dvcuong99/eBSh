using eBSH.Helper;
using eBSH.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using System.Configuration;
using eBSH.Repositories;
using NLog;
using Newtonsoft.Json;

namespace eBSH.Controllers
{
    public class CyberController : Controller
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private IGCNRepository GCNRepo;
        private IGiftCodeRepository GiftCodeRepo;
        private IOrderRepository OrderRepo;
        public CyberController(IGCNRepository gCNRepo, IGiftCodeRepository giftCodeRepo, IOrderRepository orderRepo)
        { 
            GCNRepo = gCNRepo;
            GiftCodeRepo = giftCodeRepo;
            OrderRepo = orderRepo;
        }

        public ActionResult shb()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult regOrder(string gcn, string type, string size, string ver)
        {
            PHH_CBS_GCN cyberInfo = new PHH_CBS_GCN();
            if (gcn != null)
            {
                decimal ID = 0;
                Decimal.TryParse(gcn, out ID);
                if (ID > 0)
                {
                    cyberInfo = GCNRepo.GetByID(ID);
                    if (cyberInfo.CODE_KM == "KM30")
                        cyberInfo.CODE_KM = "";
                    if (cyberInfo.SO_ID == 0)
                        gcn = "";
                }
                else
                {
                    gcn = "";
                }
            }

            if (gcn == null || gcn == "")
            {
                cyberInfo.NGAY_HL = DateTime.Today.ToString("dd/MM/yyyy");
                cyberInfo.NGAY_KT = DateTime.Today.AddDays(364).ToString("dd/MM/yyyy");
                if (size == null)
                    size = "XL";
                if (ver == null)
                    ver = "NH";
                if (type != null)
                {
                    switch (type.ToUpper())
                    {
                        case "PLATINUM":
                            cyberInfo.CTBH = "Platinum";
                            ViewBag.Description = "CyberGuard Platinum";
                            ViewBag.U = "U - BH Giao dịch giả mạo";
                            ViewBag.O = "O - BH Lừa đảo bán lẻ trực tuyến";
                            ViewBag.C = "C - BH Tống tiền qua mạng";
                            ViewBag.R = "R - Chi phí phục hồi";
                            ViewBag.I = "I - BH Trộm cắp danh tính";
                            switch (size.ToUpper())
                            {
                                case "XL":
                                    ViewBag.Size = "3000 USD";
                                    cyberInfo.MTNBH = size;
                                    cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                                    break;
                                case "M":
                                    ViewBag.Size = "1500 USD";
                                    cyberInfo.MTNBH = size;
                                    cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                                    break;
                                default:
                                    ViewBag.Size = "3000 USD";
                                    cyberInfo.MTNBH = "XL";
                                    cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                                    break;
                            }
                            break;
                        case "TITAN":
                            cyberInfo.CTBH = "Titan";
                            ViewBag.Description = "CyberGuard Titan";
                            ViewBag.U = "U - BH Giao dịch giả mạo";
                            ViewBag.O = "O - BH Lừa đảo bán lẻ trực tuyến";
                            ViewBag.C = " "; ;
                            ViewBag.R = " ";
                            ViewBag.I = " ";
                            switch (size.ToUpper())
                            {
                                case "XL":
                                    ViewBag.Size = "3000 USD";
                                    cyberInfo.MTNBH = size;
                                    cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                                    break;
                                case "M":
                                    ViewBag.Size = "1500 USD";
                                    cyberInfo.MTNBH = size;
                                    cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                                    break;
                                default:
                                    ViewBag.Size = "3000 USD";
                                    cyberInfo.MTNBH = "XL";
                                    cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                                    break;
                            }
                            break;
                        default:
                            cyberInfo.CTBH = "Gold";
                            ViewBag.Description = "CyberGuard Gold";
                            ViewBag.U = "U - BH Giao dịch giả mạo";
                            ViewBag.O = "O - BH Lừa đảo bán lẻ trực tuyến";
                            ViewBag.C = "C - BH Tống tiền qua mạng";
                            ViewBag.R = " ";
                            ViewBag.I = " ";
                            switch (size.ToUpper())
                            {
                                case "XL":
                                    ViewBag.Size = "3000 USD";
                                    cyberInfo.MTNBH = size;
                                    cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                                    break;
                                case "M":
                                    ViewBag.Size = "1500 USD";
                                    cyberInfo.MTNBH = size;
                                    cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                                    break;
                                default:
                                    ViewBag.Size = "3000 USD";
                                    cyberInfo.MTNBH = "XL";
                                    cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                                    break;
                            }
                            break;
                    }

                }
                else
                {
                    ViewBag.U = "U - BH Giao dịch giả mạo";
                    ViewBag.O = "O - BH Lừa đảo bán lẻ trực tuyến";
                    ViewBag.C = "C - BH Tống tiền qua mạng";
                    ViewBag.R = "R - Chi phí phục hồi";
                    ViewBag.I = "I - BH Trộm cắp danh tính";
                    ViewBag.Description = "CyberGuard Platinum";
                    cyberInfo.CTBH = "Platinum";
                    cyberInfo.MTNBH = "XL";
                    ViewBag.Size = "3000 USD";
                    cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                }
            }
            return View(cyberInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> regOrder(PHH_CBS_GCN cyberInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CoreNVService nvcoreSvc = CoreNVService.getInstance();

                    if (cyberInfo.SO_ID > 0)
                    {
                        var cbInf = GCNRepo.GetByID(cyberInfo.SO_ID);
                        if (cbInf.TT == "D")
                        {
                            var duyetData = new GCNVM { Ma_Dvi = "000", So_ID = cbInf.SO_ID };

                            var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_DuyetAPI(duyetData));
                            ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn đã được cấp";
                            ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_LinkGCNAPI(duyetData));
                            ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng- Số giấy chứng nhận: " + so_hd;
                            ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";
                            return View("viewpolicy");
                        }
                    }

                    if (cyberInfo.CODE_KM != null && cyberInfo.CODE_KM.Trim().Length > 0)
                    {
                        cyberInfo.CODE_KM = cyberInfo.CODE_KM.ToUpper();
                        var gcInf = GiftCodeRepo.GetByCode(cyberInfo.CODE_KM, "BHCBS", cyberInfo.CTBH);

                        if (gcInf.CodeKM == "")
                        {
                            ViewBag.Message = "Mã khuyến mại không đúng, vui lòng kiểm tra lại";
                            return View(cyberInfo);
                        }
                        else
                        {
                            if (gcInf.CTBH == null)
                                gcInf.CTBH = "";
                            if (gcInf.MTN == null)
                                gcInf.MTN = "";
                            if (gcInf.OrderId > 0)//OrderID>0 là Mã đã dùng
                            {
                                ViewBag.Message = "Mã khuyến mại đã được sử dụng, vui lòng kiểm tra lại";
                                return View(cyberInfo);
                            }
                            else if ((gcInf.CTBH != "" && gcInf.CTBH.ToUpper() != cyberInfo.CTBH.ToUpper()) || (gcInf.MTN != "" && gcInf.MTN.ToUpper() != cyberInfo.MTNBH.ToUpper()))
                            {
                                ViewBag.Message = "Mã khuyến mại này dùng cho gói sản phẩm " + gcInf.CTBH + "- size: " + gcInf.MTN.ToUpper() + ", vui lòng chọn lại, xin cảm ơn";
                                return View(cyberInfo);
                            }
                            else if (gcInf.RemainderKM == 0 && gcInf.AmountKM > 1)
                            {
                                ViewBag.Message = "Mã khuyến mại đã hết lượt sử dụng";
                                return View(cyberInfo);
                            }
                            else
                            {
                                cyberInfo.TL_KM = gcInf.TLKM;
                                cyberInfo.MA_DVI = gcInf.Ma_Dvi;
                                cyberInfo.NQL = gcInf.Ma_NQL;
                                cyberInfo.KENH = gcInf.KENH_KT;
                                cyberInfo.MA_BH = "BHCBS";
                                cyberInfo.MA_DT = "CS";
                            }
                        }
                    }
                    else
                    {
                        if (DateTime.Today >= DateTime.Parse("2 DEC 2020") && DateTime.Today <= DateTime.Parse("31 DEC 2020"))
                        {
                            cyberInfo.CODE_KM = "KM30";
                            cyberInfo.TL_KM = 30;
                        }

                        cyberInfo.MA_DVI = "000";
                        cyberInfo.MA_DT = "CS";
                        cyberInfo.MA_BH = "BHCBS";
                        cyberInfo.NQL = "PNT03";
                    }

                   
                    cyberInfo.KH = cyberInfo.HO_TH + " " + cyberInfo.TEN_TH;
                    cyberInfo.NGAY_CAP = DateTime.Today.ToString("dd/MM/yyyy");

                    if (cyberInfo.TL_KM >= 0 && cyberInfo.TL_KM < 100)
                    {
                        decimal PhiTT = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH) * (100 - cyberInfo.TL_KM) / 100;
                        cyberInfo.PHIBH = PhiTT;
                        OrderInfo orderInf;

                        var so_id = await nvcoreSvc.CBS_NhapAPI(cyberInfo);
                        if (so_id > 0)
                        {
                            cyberInfo.SO_ID = so_id;

                            var cbInf = GCNRepo.GetByID(so_id);
                            if (cbInf.SO_ID == 0)
                            {
                                Log.Info(so_id);
                                GCNRepo.Insert(cyberInfo);
                            }
                            else if (cyberInfo.TT == "T")
                            {
                                GCNRepo.Update(cyberInfo);
                            }

                            if (cyberInfo.TT == "T")
                            {
                                var duyetData = new GCNVM { Ma_Dvi = "000", So_ID = so_id };
                                var strUrl = await nvcoreSvc.CBS_LinkPreGCNAPI(duyetData);
                                orderInf = new OrderInfo { PolicyID = so_id, Amount = PhiTT, CreatedDate = DateTime.Now, OrderDescription = "Thanh toan don BH CyberGuard - Ma KH:" + cyberInfo.SO_CC + " - TK:" + cyberInfo.SO_TK + " - HLBH:" + cyberInfo.NGAY_HL, PolicyPreviewUrl = strUrl };
                                var vm = new OrderVM { cyberInf = cyberInfo, orderInf = orderInf };
                                //Chuyển màn hình xác nhận thanh toán
                                if (cyberInfo.TL_KM > 0)
                                {
                                    ViewBag.KM = "Cảm ơn Quý khách, đơn bảo hiểm được giảm " + cyberInfo.TL_KM + "% phí";
                                }
                                else
                                    ViewBag.KM = "";

                                ViewBag.Message = "Quý khách vui lòng kiểm tra thông tin trên đơn bảo hiểm dưới đây, hệ thống sẽ gửi e-policy theo email sau khi thanh toán";
                                ViewBag.Note = "Quay lại để sửa thông tin, Xác nhận để tiến hành thanh toán.";
                                return View("acceptOrder", vm);
                            }
                            else
                            {
                                ViewBag.Message = "Đơn bảo hiểm này đã được thanh toán";
                                return View(cyberInfo);
                            }
                        }
                        else
                        {
                            string dataJson = JsonConvert.SerializeObject(cyberInfo);
                            ViewBag.Message = "Lỗi hệ thống cấp đơn - vui lòng thử lại";
                            Log.Error("Lỗi nhập API data: {0}", dataJson);
                            return View(cyberInfo);
                        }
                    }
                    else// đơn tặng 100% phí
                    {
                        ViewBag.KM = "Cảm ơn Quý khách, đơn bảo hiểm được tặng 100% phí.";
                        ViewBag.Message = "Quý khách vui lòng kiểm tra thông tin trên đơn bảo hiểm dưới đây, hệ thống sẽ gửi e-policy theo email sau khi xác nhận";
                        ViewBag.Note = "Quay lại để sửa thông tin, Xác nhận để tiến hành thanh toán.";

                        cyberInfo.PHIBH = PhiChuan(cyberInfo.CTBH, cyberInfo.MTNBH);
                        OrderInfo orderInf;

                        var so_id = await nvcoreSvc.CBS_NhapAPI(cyberInfo);
                        if (so_id > 0)
                        {
                            cyberInfo.SO_ID = so_id;

                            var cbInf = GCNRepo.GetByID(so_id);
                            if (cbInf.SO_ID == 0)
                            {
                                GCNRepo.Insert(cyberInfo);
                            }
                            else if (cyberInfo.TT == "T")
                            {
                                GCNRepo.Update(cyberInfo);
                            }
                            if (cyberInfo.TT == "T")
                            {
                                var duyetData = new GCNVM { Ma_Dvi = "000", So_ID = so_id };
                                var strUrl = await nvcoreSvc.CBS_LinkPreGCNAPI(duyetData);
                                orderInf = new OrderInfo { PolicyID = so_id, Amount = cyberInfo.PHIBH, CreatedDate = DateTime.Now, OrderDescription = "Don BH CyberGuard tang 100% phi - TK:" + cyberInfo.SO_TK, PolicyPreviewUrl = strUrl };
                                var vm = new OrderVM { cyberInf = cyberInfo, orderInf = orderInf };
                                //Chuyển màn hình xác nhận thanh toán
                                return View("acceptOrderKM", vm);
                            }
                            else
                            {
                                var duyetData = new GCNVM { Ma_Dvi = "000", So_ID = so_id };
                                ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn đã được cấp";
                                var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_DuyetAPI(duyetData));
                                ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_LinkGCNAPI(duyetData));
                                ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng- Số giấy chứng nhận: " + so_hd;
                                ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";

                                return View("viewpolicy");
                            }
                        }
                        else
                        {
                            string dataJson = JsonConvert.SerializeObject(cyberInfo);
                            ViewBag.Message = "Lỗi hệ thống cấp đơn - vui lòng thử lại";
                            Log.Error("Lỗi nhập API data: {0}", dataJson);
                            return View(cyberInfo);
                        }
                    }
                }
                else
                    return View(cyberInfo);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi hệ thống cấp đơn - vui lòng thử lại";
                Log.Error(ex.Message);
                return View(cyberInfo);
            }
        }

        private decimal PhiChuan(string CTBH, string MTNBH)
        {
            decimal Phi = 0;
            if (MTNBH == null)
                MTNBH = "S";

            if (CTBH != null)
            {
                switch (CTBH.ToUpper())
                {
                    case "PLATINUM":
                        switch (MTNBH.ToUpper())
                        {
                            case "XL":
                                Phi = 990000;
                                break;
                            case "M":
                                Phi = 720000;
                                break;
                            default:
                                Phi = 700000;
                                break;
                        }
                        break;
                    case "GOLD":
                        switch (MTNBH.ToUpper())
                        {
                            case "XL":
                                Phi = 560000;
                                break;
                            case "M":
                                Phi = 410000;
                                break;
                            default:
                                Phi = 400000;
                                break;
                        }
                        break;
                    default:
                        switch (MTNBH.ToUpper())
                        {
                            case "XL":
                                Phi = 400000;
                                break;
                            case "M":
                                Phi = 290000;
                                break;
                            default:
                                Phi = 250000;
                                break;
                        }
                        break;
                }

            }
            else
            {
                Phi = 250000;
            }
            return Phi;
        }
        public ActionResult cybertype(string type)
        {
            ViewBag.Type = type;
            switch (type)
            {
                case "Platinum":
                    ViewBag.U = "BH Giao dịch giả mạo";
                    ViewBag.O = "BH Lừa đảo bán lẻ trực tuyến";
                    ViewBag.C = "BH Tống tiền qua mạng";
                    ViewBag.R = "Chi phí phục hồi";
                    ViewBag.I = "BH Trộm cắp danh tính";
                    ViewBag.PhiXL = "1.300.000 VNĐ";
                    ViewBag.PhiM = "830.000 VNĐ";
                    ViewBag.PhiS = "700.000 VNĐ";
                    break;
                case "Gold":
                    ViewBag.U = "BH Giao dịch giả mạo";
                    ViewBag.O = "BH Lừa đảo bán lẻ trực tuyến";
                    ViewBag.C = "Tống tiền qua mạng";
                    ViewBag.R = "                   ";
                    ViewBag.I = "                   ";
                    ViewBag.PhiXL = "700.000 VNĐ";
                    ViewBag.PhiM = "470.000 VNĐ";
                    ViewBag.PhiS = "400.000 VNĐ";
                    break;
                default:
                    ViewBag.Type = "Titan";
                    ViewBag.U = "BH Giao dịch giả mạo";
                    ViewBag.O = "BH Lừa đảo bán lẻ trực tuyến";
                    ViewBag.C = "                   ";
                    ViewBag.R = "                   ";
                    ViewBag.I = "                   ";
                    ViewBag.PhiXL = "400.000 VNĐ";
                    ViewBag.PhiM = "290.000 VNĐ";
                    ViewBag.PhiS = "200.000 VNĐ";
                    break;
            }
            return View();
        }
        public ActionResult acceptOrder()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult acceptOrder(OrderVM vm)
        {
            CoreNVService nvcoreSvc = CoreNVService.getInstance();
            //Get payment input
            try
            {
                var lastOrder = OrderRepo.GetLastOrderByPolicyID(vm.cyberInf.SO_ID);

                if (lastOrder.OrderId > 0)
                {
                    if (lastOrder.Status == 1 || lastOrder.PolicyStatus == "D")
                    {
                        var duyetData = new GCNVM { Ma_Dvi = vm.cyberInf.MA_DVI, So_ID = lastOrder.PolicyID };
                        var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_DuyetAPI(duyetData));
                        AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_LinkGCNAPI(duyetData));

                        ViewBag.Message = "Đơn của quý khách đã được thanh toán - Số giấy chứng nhận: " + lastOrder.PolicyNo;
                        return View(vm);
                    }
                }

                VNPayOrderVM vnpInf = new VNPayOrderVM();
                vnpInf.Ma_dvi = vm.cyberInf.MA_DVI;
                vnpInf.So_ID = vm.cyberInf.SO_ID;
                vnpInf.Ma_BH = vm.cyberInf.MA_BH;
                vnpInf.vnpUrlReturn = ConfigurationManager.AppSettings["vnp_Returnurl"] + "/cyber/vnpayresult"; //URL nhan ket qua tra ve 

                var paymentRes = AsyncUtil.RunSync<ApiVNPayResult>(() => nvcoreSvc.VNPay_CreatePayment(vnpInf));
                if (Common.ToDecimal(paymentRes.vnpayOrderID) != 0)
                {
                    string paymentUrl = paymentRes.url;
                    Log.Info("VNPAY URL: {0}", paymentUrl);
                    vm.orderInf.OrderId = Common.ToDecimal(paymentRes.vnpayOrderID);
                    vm.orderInf.PolicyStatus = "T";
                    vm.orderInf.vnp_IpAddr = Utils.GetIpAddress();
                    vm.orderInf.CreatedDate = DateTime.Now;
                    vm.orderInf.vnp_TmnCode = paymentRes.vnp_TmnCode;
                    vm.orderInf.Status = 0;
                    OrderRepo.Insert(vm.orderInf);

                    return Redirect(paymentUrl);
                }
                else
                {
                    Log.Error(paymentRes.url);
                    return View(vm);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return View(vm);
            }
        }

        public ActionResult acceptOrderKM()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult acceptOrderKM(OrderVM vm)
        {
            //Get payment input
            try
            {
                var lastOrder = OrderRepo.GetLastOrderByPolicyID(vm.cyberInf.SO_ID);
                var duyetData = new GCNVM { Ma_Dvi = "000", So_ID = vm.cyberInf.SO_ID };
                string so_hd = "";

                CoreNVService nvcoreSvc = CoreNVService.getInstance();

                if (lastOrder.OrderId > 0)
                {
                    if (lastOrder.Status == 1 || lastOrder.PolicyStatus == "D")
                    {
                        so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_DuyetAPI(duyetData));

                        var cbgInf = GCNRepo.GetByID(vm.orderInf.PolicyID);

                        lastOrder.PolicyNo = so_hd;
                        OrderRepo.Update(lastOrder);
                        cbgInf.SO_HD = so_hd;
                        cbgInf.TT = "D";
                        if (cbgInf.SO_ID > 0)
                            GCNRepo.Update(cbgInf);
                        ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn đã được cấp";
                        ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_LinkGCNAPI(duyetData));
                        ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng- Số giấy chứng nhận: " + lastOrder.PolicyNo;
                        ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";
                        return View("viewpolicy");
                    }
                    else
                    {
                        vm.orderInf.OrderId = lastOrder.OrderId + 1;
                    }
                }
                else
                {
                    vm.orderInf.OrderId = vm.orderInf.PolicyID * 100;
                }
                var cbInf = GCNRepo.GetByID(vm.orderInf.PolicyID);

                if (cbInf.CODE_KM != null && cbInf.CODE_KM.Trim().Length > 0)
                {
                    var gcInf = GiftCodeRepo.GetByCode(cbInf.CODE_KM, "BHCBS", cbInf.CTBH);
                    gcInf.OrderId = vm.orderInf.OrderId;
                    if (gcInf.AmountKM == 1)
                        GiftCodeRepo.Update(gcInf); // Cập nhật GiftCode 1 lần
                    else
                        GiftCodeRepo.UpdateRemainderKM(gcInf.CodeKM); // Cập nhật GiftCode nhiều lần lần
                }
                vm.orderInf.vnp_Message = "Đơn tặng 100% phí";
                vm.orderInf.PolicyStatus = "D";
                vm.orderInf.vnp_IpAddr = Utils.GetIpAddress();
                vm.orderInf.CreatedDate = DateTime.Now;
                vm.orderInf.Status = 1;
                vm.orderInf.vnp_ResponseDate = DateTime.Now;
                OrderRepo.Insert(vm.orderInf);

                so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_DuyetAPI(duyetData));
                ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_LinkGCNAPI(duyetData));

                vm.orderInf.PolicyNo = so_hd;
                OrderRepo.Update(vm.orderInf);
                cbInf.SO_HD = so_hd;
                cbInf.TT = "D";
                if (cbInf.SO_ID > 0)
                    GCNRepo.Update(cbInf);

                ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn tặng 100% phí";
                ViewBag.Success = "Chúc mừng bạn đã được bảo vệ bởi CyberGuard, bảo hiểm rủi ro cá nhân trên không gian mạng.";
                ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng.";
                ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";

                return View("viewpolicy");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return View(vm);
            }
        }

        public ActionResult vnpayresult(string vnp_TxnRef, string vnp_TransactionNo, string vnp_ResponseCode, string vnp_SecureHash, string vnp_BankCode, string vnp_BankTranNo, string vnp_CardType, string vnp_PayDate, string vnp_Amount)
        {
            //vnp_Amount=1.300.000.00&vnp_BankCode=NCB&vnp_BankTranNo=20201124140446&vnp_CardType=ATM&vnp_OrderInfo=Thanh+toan+don+BH+CyberGuard+-+TK%3A0213123123&vnp_PayDate=20201124140430&vnp_ResponseCode=00&vnp_TmnCode=BHSGHNOI&vnp_TransactionNo=13423248&vnp_TxnRef=20201124618239&vnp_SecureHashType=SHA256&vnp_SecureHash=412dbbf80c2930bcc1417815f734e8fda98a9c3758ee94caef672e6f363cad1a
            //vnp_Amount=10000000&vnp_BankCode=NCB&vnp_BankTranNo=20201102-153705&vnp_CardType=ATM&vnp_OrderInfo=Noi+dung+thanh+toan%3A20201031161132&vnp_PayDate=20201102153652&vnp_ResponseCode=00&vnp_TmnCode=BHSGHNOI&vnp_TransactionNo=13411635&vnp_TxnRef=637399281661121422&vnp_SecureHashType=SHA256&vnp_SecureHash=b5dc4b59286b2b3666d7a6c9fe62a035b2ad4800c1305cc958fb09e4886483eb
            //log.InfoFormat("Begin BSHCyber Return, URL={0}", Request.RawUrl);
            OrderInfo lastOrder = new OrderInfo();
            try
            {
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();
                if (vnpayData.Count > 0)
                {
                    foreach (string s in vnpayData)
                    {
                        //get all querystring data
                        if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                        {
                            vnpay.AddResponseData(s, vnpayData[s]);
                        }
                    }

                    string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat 
                    //vnp_TxnRef: Ma don hang merchant gui BSHCyber tai command=pay    
                    long orderId = Convert.ToInt64(vnp_TxnRef);
                    //vnp_TransactionNo: Ma GD tai he thong BSHCyber
                    long vnpayTranId = Convert.ToInt64(vnp_TransactionNo);
                    long amount = Convert.ToInt64(vnp_Amount) / 100;
                    //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu

                    bool checkSignature = true;// vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                    if (checkSignature)
                    {
                        if (vnp_ResponseCode == "00")
                        {
                            lastOrder = OrderRepo.GetByOrderID(orderId);
                            if (lastOrder.OrderId > 0)
                            {
                                if (lastOrder.Amount == amount)
                                {
                                    var cbInf = GCNRepo.GetByID(lastOrder.PolicyID);
                                    if (cbInf.CODE_KM != null && cbInf.CODE_KM.Trim().Length > 0)//Cập nhật OrderID cho mã GiftCode - để chỉ sử dụng 1 lần
                                    {
                                        var gcInf = GiftCodeRepo.GetByCode(cbInf.CODE_KM, "BHCBS", cbInf.CTBH);
                                        gcInf.OrderId = orderId;
                                        if (gcInf.AmountKM == 1)
                                            GiftCodeRepo.Update(gcInf); // Cập nhật GiftCode 1 lần
                                        else
                                            GiftCodeRepo.UpdateRemainderKM(gcInf.CodeKM); // Cập nhật GiftCode nhiều lần lần
                                    }

                                    CoreNVService nvcoreSvc = CoreNVService.getInstance();
                                    var duyetData = new GCNVM { Ma_Dvi = "000", So_ID = lastOrder.PolicyID, CB_Du= cbInf.NQL };
                                    var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_DuyetAPI(duyetData));

                                    if (so_hd.Trim().Length > 0)
                                    {
                                        lastOrder.PolicyNo = so_hd;
                                        lastOrder.PolicyStatus = "D";
                                    }
                                    cbInf.SO_HD = so_hd;
                                    cbInf.TT = "D";
                                    if (cbInf.SO_ID > 0)
                                        GCNRepo.Update(cbInf);

                                    lastOrder.vnp_Message = "Giao dịch thanh toán thành công";
                                    lastOrder.CardType = vnp_CardType;
                                    lastOrder.BankCode = vnp_BankCode;
                                    lastOrder.BankTranNo = vnp_BankTranNo;
                                    lastOrder.vnp_PayDate = vnp_PayDate;
                                    lastOrder.vnp_TransactionNo = vnpayTranId;
                                    lastOrder.vnp_TxnResponseCode = vnp_ResponseCode;
                                    lastOrder.vnp_ResponseDate = DateTime.Now;
                                    var url = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_LinkGCNAPI(duyetData));

                                    lastOrder.PolicyUrl = url;
                                    OrderRepo.Update(lastOrder);

                                    //Thanh toan thanh cong
                                    lastOrder = OrderRepo.GetByOrderID(orderId);

                                    ViewBag.Title = "Hoàn tất & nhận e-policy - Thanh toán thành công";
                                    ViewBag.Success = "Chúc mừng bạn đã được bảo vệ bởi Bảo hiểm BSH CyberGuard ";
                                    ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng.";
                                    ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";
                                    ViewBag.MaGD = "Mã giao dịch thanh toán: " + vnp_TxnRef;
                                    ViewBag.NDTT = lastOrder.OrderDescription;
                                    ViewBag.NGTT = "Ngày: " + lastOrder.vnp_ResponseDate.ToString("dd/MM/yyyy");
                                    ViewBag.SoHD = "Giấy chứng nhận BH điện tử số: " + lastOrder.PolicyNo;
                                    ViewBag.PolicyUrl = url;
                                    Log.Info("Thanh toan thanh cong, OrderId={0}, VNPayTranId={1}", orderId, vnpayTranId);
                                }
                                else
                                {
                                    ViewBag.Title = "Hoàn tất & nhận e-policy - Giao dịch lỗi";
                                    ViewBag.MaGD = "Mã giao dịch thanh toán: " + vnp_TxnRef;
                                    ViewBag.NDTT = lastOrder.OrderDescription;
                                    ViewBag.NGTT = "Ngày: " + lastOrder.vnp_ResponseDate.ToString("dd/MM/yyyy");
                                    ViewBag.Message = "Giao dịch thanh toán không thành công: " + vnp_ResponseCode + "-" + vnp_TxnRef;
                                    ViewBag.Note = "Quý khách vui lòng thực hiện lại";
                                    ViewBag.Success = "";
                                    Log.Error("Thanh toan loi, số tiền giao dịch VNP trả về sai OrderID, OrderId={0}, BSHCyber TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                                }
                            }
                            else
                            {
                                ViewBag.Title = "Hoàn tất & nhận e-policy - Giao dịch lỗi";
                                ViewBag.MaGD = "Mã giao dịch thanh toán: " + vnp_TxnRef;
                                ViewBag.Message = "Giao dịch thanh toán không thành công, không tìm thấy giao dịch: " + vnp_ResponseCode + "-" + vnp_TxnRef;
                                ViewBag.Note = "Quý khách vui lòng thực hiện lại";
                                ViewBag.Success = "";
                                Log.Error("Thanh toan loi, khong tim thay OrderID, OrderId={0}, BSHCyber TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                            }
                        }
                        else
                        {
                            var errDes = "";
                            switch (vnp_ResponseCode)
                            {
                                case "01":
                                    errDes = "Giao dịch đã tồn tại"; break;
                                case "02":
                                    errDes = "Merchant không hợp lệ "; break;
                                case "03":
                                    errDes = "Dữ liệu gửi sang không đúng định dạng"; break;
                                case "04":
                                    errDes = "Khởi tạo GD không thành công do Website đang bị tạm khóa"; break;
                                case "05":
                                    errDes = "Giao dịch không thành công do: Quý khách nhập sai mật khẩu quá số lần quy định.Xin quý khách vui lòng thực hiện lại giao dịch"; break;
                                case "13":
                                    errDes = "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch(OTP).Xin quý khách vui lòng thực hiện lại giao dịch."; break;
                                case "07":
                                    errDes = "Giao dịch bị nghi ngờ là giao dịch gian lận"; break;
                                case "09":
                                    errDes = "Thẻ / Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng."; break;
                                case "10":
                                    errDes = "Khách hàng xác thực thông tin thẻ / tài khoản không đúng quá 3 lần"; break;
                                case "11":
                                    errDes = "Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch."; break;
                                case "12":
                                    errDes = "Thẻ / Tài khoản của khách hàng bị khóa."; break;
                                case "24":
                                    errDes = "Giao dịch bị hủy."; break;
                                case "51":
                                    errDes = "Tài khoản của quý khách không đủ số dư để thực hiện giao dịch."; break;
                                case "65":
                                    errDes = "Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày."; break;
                                case "08":
                                    errDes = "Hệ thống Ngân hàng đang bảo trì.Xin quý khách tạm thời không thực hiện giao dịch bằng thẻ/ tài khoản của Ngân hàng này."; break;
                                default:
                                    errDes = "Có lỗi xảy ra trong quá trình xử lý ";
                                    break;
                            }
                            ViewBag.Title = "Hoàn tất & nhận e-policy - Giao dịch lỗi " + vnp_ResponseCode;
                            ViewBag.MaGD = "Mã giao dịch thanh toán: " + vnp_TxnRef;
                            ViewBag.Message = "Giao dịch thanh toán không thành công: " + vnp_ResponseCode + " - " + errDes;
                            ViewBag.Note = "Quý khách vui lòng thực hiện lại";
                            //ViewBag.NDTT = lastOrder.OrderDescription;
                            //ViewBag.NGTT = "Ngày: " + lastOrder.vnp_ResponseDate.ToString("dd/MM/yyyy");
                            ViewBag.Success = "";
                            Log.Error("Thanh toan loi, OrderId={0}, BSHCyber TranId={1},ResponseCode={2}, Description ={3}", orderId, vnpayTranId, vnp_ResponseCode, errDes);
                        }
                    }
                    else
                    {
                        ViewBag.Title = "Hoàn tất & nhận e-policy - Giao dịch lỗi ";
                        ViewBag.MaGD = "Mã giao dịch thanh toán: " + vnp_TxnRef;
                        Log.Error("Invalid signature, InputData={0}", Request.RawUrl);
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                        ViewBag.Note = "Quý khách vui lòng thực hiện lại";
                        ViewBag.Success = "";
                    }

                }
                else
                {
                    ViewBag.Title = "Hoàn tất & nhận e-policy - Giao dịch lỗi ";
                    ViewBag.MaGD = "Mã giao dịch thanh toán: " + vnp_TxnRef;
                    Log.Error("URL thiếu Para, InputData={0}", Request.RawUrl);
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                    ViewBag.Note = "Quý khách vui lòng thực hiện lại";
                    ViewBag.Success = "";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Title = "Hoàn tất & nhận e-policy - Giao dịch lỗi ";
                ViewBag.MaGD = "Mã giao dịch thanh toán: " + vnp_TxnRef;
                Log.Error("Lỗi xử lý: InputData={0}, Exception:{1}", Request.RawUrl, ex.Message);
                ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                ViewBag.Note = "Quý khách vui lòng thực hiện lại";
                ViewBag.Success = "";
            }
            return View();
        }

    }
}
