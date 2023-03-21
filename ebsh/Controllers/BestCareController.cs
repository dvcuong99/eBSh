using eBSH.Helper;
using eBSH.Models;
using eBSH.Repositories;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eBSH.Controllers
{
    public class BestCareController : Controller
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private ITopCarePremiumRepo PremiumRepo;
        private IPA_GCNRepository PA_GCNRepo;
        private IGiftCodeRepository GiftCodeRepo;
        private IOrderRepository OrderRepo;
        public BestCareController(ITopCarePremiumRepo premiumRepo, IPA_GCNRepository pa_GCNRepo, IGiftCodeRepository giftCodeRepo, IOrderRepository orderRepo)
        {
            PremiumRepo = premiumRepo;
            PA_GCNRepo = pa_GCNRepo;
            GiftCodeRepo = giftCodeRepo;
            OrderRepo = orderRepo;
        }
        // GET: BestCare
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult Premium(string NgaySinh, string CTBH, int ThoiHan, string NgayHL)
        {
            int year = (Common.ToDateTime(NgayHL) - Common.ToDateTime(NgaySinh)).Days / 365;

            var data = PremiumRepo.GetByYear(CTBH, ThoiHan, year);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult regorder(string gcn, string type)
        {
            CN_PA_GCN_VM regInfo = new CN_PA_GCN_VM();
            if (gcn != null)
            {
                decimal ID = 0;
                Decimal.TryParse(gcn, out ID);
                if (ID > 0)
                {
                    var pa_regInfo = PA_GCNRepo.GetByID(ID);
                    regInfo = new CN_PA_GCN_VM(pa_regInfo);
                    if (regInfo.So_ID == 0)
                        gcn = "";
                }
                else
                {
                    gcn = "";
                }
            }

            if (gcn == null || gcn == "")
            {
                if (type == null || type == "")
                    type = "Diamond";
                regInfo.CTBH = type;
                regInfo.ThoiHan = 3;
                regInfo.Ngay_HL = DateTime.Today.ToString("dd/MM/yyyy");
                regInfo.Ngay_KT = DateTime.Today.AddMonths(3).ToString("dd/MM/yyyy");
            }
            return View(regInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> regOrder(CN_PA_GCN_VM vmregInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CN_PA_GCN regInfo = new CN_PA_GCN(vmregInfo);
                    CoreNVService nvcoreSvc = CoreNVService.getInstance();

                    if (regInfo.So_ID > 0)
                    {
                        var cnInf = PA_GCNRepo.GetByID(regInfo.So_ID);
                        if (cnInf.TT == "D")
                        {
                            var duyetData = new GCNVM { Ma_Dvi = cnInf.Ma_Dvi, So_ID = cnInf.So_ID, CB_Du = cnInf.NSD };

                            var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_DuyetAPI(duyetData));
                            ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn đã được cấp";
                            ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_LinkGCNAPI(duyetData));
                            ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng- Số giấy chứng nhận: " + so_hd;
                            ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";
                            return View("viewpolicy");
                        }
                    }

                    if (regInfo.CODE_KM != null && regInfo.CODE_KM.Trim().Length > 0)
                    {
                        regInfo.CODE_KM = regInfo.CODE_KM.ToUpper();
                        var gcInf = GiftCodeRepo.GetByCode(regInfo.CODE_KM, "BESTCARE", regInfo.CTBH);

                        if (gcInf.CodeKM == "")
                        {
                            ViewBag.Message = "Mã khuyến mại không đúng, vui lòng kiểm tra lại";
                            return View(vmregInfo);
                        }
                        else
                        {
                            regInfo.Ma_Dvi = gcInf.Ma_Dvi;
                            regInfo.MA_DL = gcInf.Ma_DL;
                            regInfo.NQL = gcInf.Ma_NQL;
                            regInfo.NSD = "NDQ01";// người duyệt đơn
                            if (gcInf.CTBH == null)
                                gcInf.CTBH = "";
                            if (gcInf.MTN == null)
                                gcInf.MTN = "";
                            if (gcInf.OrderId > 0)//OrderID>0 là Mã đã dùng
                            {
                                ViewBag.Message = "Mã khuyến mại đã được sử dụng, vui lòng kiểm tra lại";
                                return View(vmregInfo);
                            }
                            else if (gcInf.CTBH != "" && gcInf.CTBH.ToUpper() != regInfo.CTBH.ToUpper())
                            {
                                ViewBag.Message = "Mã khuyến mại này dùng cho gói sản phẩm " + gcInf.CTBH + " vui lòng chọn lại, xin cảm ơn";
                                return View(vmregInfo);
                            }
                            else if (gcInf.RemainderKM == 0 && gcInf.AmountKM > 1)
                            {
                                ViewBag.Message = "Mã khuyến mại đã hết lượt sử dụng";
                                return View(vmregInfo);
                            }
                            else
                            {
                                regInfo.TL_KM = gcInf.TLKM;
                                regInfo.Ma_Dvi = gcInf.Ma_Dvi;
                                regInfo.NQL = gcInf.Ma_NQL;
                                regInfo.NSD = gcInf.Ma_NSD;
                            }
                        }
                    }
                    else//Mặc định BSH Tràng An
                    {
                        regInfo.Ma_Dvi = "033";
                        regInfo.MA_DL = "";
                        regInfo.NQL = "PQN02";
                        regInfo.NSD = "NDQ01";
                    }

                    if (regInfo.CTBH == null || regInfo.CTBH == "")
                        regInfo.CTBH = "Gold";
                    if (regInfo.ThoiHan == 0)
                        regInfo.ThoiHan = 12;

                    decimal PhiTT = 0;
                    foreach (var item in regInfo.DS_NBH)
                    {
                        int year = DateTime.Today.Year - Common.ToDateTime(item.NgSinh).Year;
                        item.Phi_BH = PremiumRepo.GetByYear(regInfo.CTBH, regInfo.ThoiHan, year).Phi;
                        PhiTT += item.Phi_BH;
                    }

                    //todo: cần lấy TT theo mã đơn vị 
                    regInfo.Ma_BH = "BESTCARE";


                    regInfo.Ngay_TT = DateTime.Today.ToString("dd/MM/yyyy");
                    vmregInfo = new CN_PA_GCN_VM(regInfo);
                    if (regInfo.TL_KM >= 0 && regInfo.TL_KM < 100)
                    {
                        // Phí sau khi sử dụng GiftCode
                        PhiTT = 0;
                        foreach (var item in regInfo.DS_NBH)
                        {
                            item.Phi_BH = (item.Phi_BH * (100 - regInfo.TL_KM) / 100);
                            PhiTT += item.Phi_BH;
                        }
                        regInfo.Phi_BH = PhiTT;
                        OrderInfo orderInf;

                        Log.Error(JsonConvert.SerializeObject(regInfo));

                        var so_id = await nvcoreSvc.PA_NhapAPI(regInfo);
                        Log.Error(JsonConvert.SerializeObject(regInfo));
                        if (so_id > 0)
                        {
                            regInfo.So_ID = so_id;
                            vmregInfo = new CN_PA_GCN_VM(regInfo);
                            foreach (var item in regInfo.DS_NBH)
                            {
                                item.So_ID = so_id;
                            }

                            var cbInf = PA_GCNRepo.GetByID(so_id);
                            if (cbInf == null || cbInf.So_ID == 0)
                            {
                                PA_GCNRepo.Insert(regInfo);
                            }
                            else if (regInfo.TT == "T")
                            {
                                PA_GCNRepo.Update(regInfo);
                            }
                            if (regInfo.TT == "T")
                            {
                                var duyetData = new GCNVM { Ma_Dvi = regInfo.Ma_Dvi, So_ID = so_id };
                                var strUrl = await nvcoreSvc.PA_LinkPreGCNAPI(duyetData);
                                orderInf = new OrderInfo { PolicyID = so_id, Amount = PhiTT, CreatedDate = DateTime.Now, OrderDescription = "Thanh toan don BH BESTCARE - Ma KH:" + regInfo.CMND + " - Ten:" + regInfo.KhachHang + " - HLBH:" + regInfo.Ngay_HL, PolicyPreviewUrl = strUrl };
                                var vm = new PAOrderVM { paInf = regInfo, orderInf = orderInf };
                                //Chuyển màn hình xác nhận thanh toán
                                if (regInfo.TL_KM > 0)
                                {
                                    ViewBag.KM = "Cảm ơn Quý khách, đơn bảo hiểm được giảm " + regInfo.TL_KM + "% phí";
                                }
                                else
                                    ViewBag.KM = "";

                                ViewBag.Message = "Quý khách vui lòng kiểm tra thông tin trên đơn bảo hiểm dưới đây, hệ thống sẽ gửi e-policy theo email sau khi thanh toán";
                                ViewBag.Note = "Quay lại để sửa thông tin, Xác nhận để tiến hành thanh toán.";

                                return View("acceptOrder", vm);

                                //TempData["paOrder"] = vm;
                                //return RedirectToAction("acceptOrder");
                            }
                            else
                            {
                                ViewBag.Message = "Đơn bảo hiểm này đã được thanh toán";
                                return View(vmregInfo);
                            }
                        }
                        else
                        {
                            string dataJson = JsonConvert.SerializeObject(regInfo);
                            ViewBag.Message = "Lỗi hệ thống cấp đơn - vui lòng thử lại";
                            Log.Error("Lỗi nhập API data: {0}", dataJson);
                            return View(vmregInfo);
                        }
                    }
                    else// đơn tặng 100% phí
                    {
                        ViewBag.KM = "Cảm ơn Quý khách, đơn bảo hiểm được tặng 100% phí.";
                        ViewBag.Message = "Quý khách vui lòng kiểm tra thông tin trên đơn bảo hiểm dưới đây, hệ thống sẽ gửi e-policy theo email sau khi xác nhận";
                        ViewBag.Note = "Quay lại để sửa thông tin, Xác nhận để tiến hành thanh toán.";

                        //regInfo.Phi_BH = PhiChuan(regInfo.CTBH, regInfo.MTNBH);
                        OrderInfo orderInf = new OrderInfo();

                        var so_id = await nvcoreSvc.PA_NhapAPI(regInfo);
                        if (so_id > 0)
                        {
                            regInfo.So_ID = so_id;
                            vmregInfo = new CN_PA_GCN_VM(regInfo);
                            foreach (var item in regInfo.DS_NBH)
                            {
                                item.So_ID = so_id;
                            }

                            var cbInf = PA_GCNRepo.GetByID(so_id);
                            if (cbInf == null || cbInf.So_ID == 0)
                            {
                                PA_GCNRepo.Insert(regInfo);
                            }
                            else if (regInfo.TT == "T")
                            {
                                PA_GCNRepo.Update(regInfo);
                            }

                            if (regInfo.TT == "T")
                            {
                                var duyetData = new GCNVM { Ma_Dvi = regInfo.Ma_Dvi, So_ID = so_id, CB_Du = regInfo.NSD };
                                var strUrl = await nvcoreSvc.PA_LinkPreGCNAPI(duyetData);
                                orderInf = new OrderInfo { PolicyID = so_id, Amount = regInfo.Phi_BH, CreatedDate = DateTime.Now, OrderDescription = "Don BH BESTCARE tang 100% phi - KH:" + regInfo.CMND + regInfo.KhachHang, PolicyPreviewUrl = strUrl };
                                var vm = new PAOrderVM { paInf = regInfo, orderInf = orderInf };
                                //Chuyển màn hình xác nhận thanh toán
                                return View("acceptOrderKM", vm);
                            }
                            else
                            {
                                var duyetData = new GCNVM { Ma_Dvi = regInfo.Ma_Dvi, So_ID = so_id, CB_Du = regInfo.NSD };
                                ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn đã được cấp";
                                var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_DuyetAPI(duyetData));
                                ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_LinkGCNAPI(duyetData));
                                ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng- Số giấy chứng nhận: " + so_hd;
                                ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";

                                return View("viewpolicy");
                            }

                        }
                        else
                        {
                            string dataJson = JsonConvert.SerializeObject(regInfo);
                            ViewBag.Message = "Lỗi hệ thống cấp đơn - vui lòng thử lại";
                            Log.Error("Lỗi nhập API data: {0}", dataJson);
                            return View(regInfo);
                        }
                    }
                }
                else
                    return View(vmregInfo);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi hệ thống cấp đơn - vui lòng thử lại";
                Log.Error(ex.Message);
                return View(vmregInfo);
            }
        }

        public ActionResult acceptOrder()
        {
            var paOrderVM = TempData["paOrder"] as PAOrderVM;
            return View(paOrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult acceptOrder(PAOrderVM vm)
        {
            CoreNVService nvcoreSvc = CoreNVService.getInstance();
            //Get payment input
            try
            {
                var lastOrder = OrderRepo.GetLastOrderByPolicyID(vm.paInf.So_ID);

                if (lastOrder.OrderId > 0)
                {
                    if (lastOrder.Status == 1 || lastOrder.PolicyStatus == "D")
                    {
                        var duyetData = new GCNVM { Ma_Dvi = vm.paInf.Ma_Dvi, So_ID = lastOrder.PolicyID };
                        var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_DuyetAPI(duyetData));
                        AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_LinkGCNAPI(duyetData));

                        ViewBag.Message = "Đơn của quý khách đã được thanh toán - Số giấy chứng nhận: " + lastOrder.PolicyNo;
                        return View(vm);
                    }
                }

                var gcn = PA_GCNRepo.GetByID(vm.paInf.So_ID);
                VNPayOrderVM vnpInf = new VNPayOrderVM();
                vnpInf.Ma_dvi = gcn.Ma_Dvi;
                vnpInf.So_ID = gcn.So_ID;
                vnpInf.Ma_BH = gcn.Ma_BH;
                vnpInf.vnpUrlReturn = ConfigurationManager.AppSettings["vnp_Returnurl"] + "/bestcare/vnpayresult"; //URL nhan ket qua tra ve 

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
        public ActionResult acceptOrderKM(PAOrderVM vm)
        {
            //Get payment input
            try
            {
                var lastOrder = OrderRepo.GetLastOrderByPolicyID(vm.paInf.So_ID);
                var duyetData = new GCNVM { Ma_Dvi = vm.paInf.Ma_Dvi, So_ID = vm.paInf.So_ID, CB_Du = vm.paInf.NSD };
                string so_hd = "";

                CoreNVService nvcoreSvc = CoreNVService.getInstance();

                if (lastOrder.OrderId > 0)
                {
                    if (lastOrder.Status == 1 || lastOrder.PolicyStatus == "D")
                    {
                        so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_DuyetAPI(duyetData));

                        var pa_regInfo = PA_GCNRepo.GetByID(vm.orderInf.PolicyID);

                        lastOrder.PolicyNo = so_hd;
                        OrderRepo.Update(lastOrder);
                        pa_regInfo.So_HD = so_hd;
                        pa_regInfo.TT = "D";
                        if (pa_regInfo.So_ID > 0)
                            PA_GCNRepo.Update(pa_regInfo);
                        ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn đã được cấp";
                        ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_LinkGCNAPI(duyetData));
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
                var cbInf = PA_GCNRepo.GetByID(vm.orderInf.PolicyID);

                if (cbInf.CODE_KM != null && cbInf.CODE_KM.Trim().Length > 0)
                {
                    var gcInf = GiftCodeRepo.GetByCode(cbInf.CODE_KM, "BESTCARE", cbInf.CTBH);
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

                so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_DuyetAPI(duyetData));
                ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_LinkGCNAPI(duyetData));

                vm.orderInf.PolicyNo = so_hd;
                OrderRepo.Update(vm.orderInf);
                cbInf.So_HD = so_hd;
                cbInf.TT = "D";
                if (cbInf.So_ID > 0)
                    PA_GCNRepo.Update(cbInf);

                ViewBag.Title = "Hoàn tất & nhận e-policy - Thanh toán thành công";
                ViewBag.Success = "Chúc mừng bạn đã được bảo vệ bởi Bảo hiểm BSH BEST CARE ";
                ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng.";
                ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";
                ViewBag.SoHD = "Giấy chứng nhận BH điện tử số: " + lastOrder.PolicyNo;

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

                                    var cbInf = PA_GCNRepo.GetByID(lastOrder.PolicyID);

                                    if (cbInf.CODE_KM != null && cbInf.CODE_KM.Trim().Length > 0)
                                    {
                                        var gcInf = GiftCodeRepo.GetByCode(cbInf.CODE_KM, "BESTCARE", cbInf.CTBH);
                                        gcInf.OrderId = orderId;
                                        if (gcInf.AmountKM == 1)
                                            GiftCodeRepo.Update(gcInf);// Cập nhật GiftCode 1 lần
                                        else
                                            GiftCodeRepo.UpdateRemainderKM(gcInf.CodeKM); // Cập nhật GiftCode nhiều lần
                                    }

                                    CoreNVService nvcoreSvc = CoreNVService.getInstance();
                                    var duyetData = new GCNVM { Ma_Dvi = "000", So_ID = lastOrder.PolicyID, CB_Du = "" };
                                    var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_DuyetAPI(duyetData));

                                    if (so_hd.Trim().Length > 0)
                                    {
                                        lastOrder.PolicyNo = so_hd;
                                        lastOrder.PolicyStatus = "D";
                                    }
                                    cbInf.So_HD = so_hd;
                                    cbInf.TT = "D";
                                    if (cbInf.So_ID > 0)
                                        PA_GCNRepo.Update(cbInf);

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
                                    ViewBag.Success = "Chúc mừng bạn đã được bảo vệ bởi Bảo hiểm BSH BESTCARE ";
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
                            Log.Error("Thanh toan loi, OrderId={0}, BSH BestCare TranId={1},ResponseCode={2}, Description ={3}", orderId, vnpayTranId, vnp_ResponseCode, errDes);
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