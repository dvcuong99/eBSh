using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Presentation;
using eBSH.Helper;
using eBSH.Models;
using eBSH.Repositories;
using Newtonsoft.Json;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace eBSH.Controllers
{
    public class MotorVehicleController : Controller
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private IOrderRepository OrderRepo;
        private IMotorVehicleRepository MVCRepo;
        private IRefCodeRepository RefCodeRepo;
        CoreNVService nvcoreSvc = CoreNVService.getInstance();
        public MotorVehicleController(IOrderRepository orderRepo, IMotorVehicleRepository mvcRepo, IRefCodeRepository refCodeRepo)
        {
            OrderRepo = orderRepo;
            MVCRepo = mvcRepo;
            RefCodeRepo = refCodeRepo;
        }
        public ActionResult regOrder(string refCode)
        {
            //Fetch the Cookie using its Key
            HttpCookie cookie = Request.Cookies["RefCode"];

            MotorVehicle_VM motorvehicle = new MotorVehicle_VM();
            if (refCode != null)
            {
                motorvehicle.refcode = refCode;
                if (cookie == null)                
                {
                    //Create a Cookie with a suitable Key
                    cookie = new HttpCookie("RefCode");                    
                }
                //Set the Cookie value
                cookie.Value = motorvehicle.refcode;
                //Set the Expiry date
                cookie.Expires = DateTime.Now.AddDays(15);
                //Add the Cookie to Browser
                Response.Cookies.Add(cookie);
            }
            else
            {
                //If Cookie exists fetch its value
                if (cookie != null)
                {
                    motorvehicle.refcode = cookie.Value;
                }
                else
                {
                    //Create a Cookie with a suitable Key
                    cookie = new HttpCookie("RefCode");

                    //Set the Cookie value
                    cookie.Value = "000";

                    //Set the Expiry date
                    cookie.Expires = DateTime.Now.AddDays(15);

                    //Add the Cookie to Browser
                    Response.Cookies.Add(cookie);
                }    
            }    
            motorvehicle.ngay_hl= DateTime.Today.ToString("dd/MM/yyyy");
            motorvehicle.ngay_kt = DateTime.Today.AddYears(1).ToString("dd/MM/yyyy");
            var hieuxe = AsyncUtil.RunSync<List<MaBHXe>>(() => nvcoreSvc.GetMaBHXe());
            ViewBag.load_cn = motorvehicle.listMaBHXe;
            return View(motorvehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> regOrder(MotorVehicle_VM vmregInfo)
        {

            MotorVehicle regInfo=new MotorVehicle();
            ViewBag.load_cn = vmregInfo.listMaBHXe;
            try
            {
                if (ModelState.IsValid)
                {
                    regInfo = new MotorVehicle(vmregInfo);

                    //Mặc định BSH TSC                  
                    regInfo.ma_dvi = "053";
                    regInfo.ma_dl = "";
                    regInfo.kenh_kt = "";
                    regInfo.cb_du = "DND01";
                    regInfo.cb_ql = "VDT01";
                    if (regInfo.refcode != null && regInfo.refcode.Trim().Length > 0)
                    { 
                        var gcInf = RefCodeRepo.GetByCode(regInfo.refcode);
                        if (gcInf.Code!= "")                        
                        {
                            regInfo.ma_dvi = gcInf.Ma_Dvi;
                            regInfo.ma_dl = gcInf.Ma_DL;
                            regInfo.kenh_kt = gcInf.Kenh_KT;
                            regInfo.cb_du = gcInf.Ma_NQL;// người duyệt đơn
                            regInfo.cb_ql = gcInf.Ma_NSD;                             
                        }
                    }                  
                 

                    if (vmregInfo.so_id >0)
                    {
                        var cbInf = MVCRepo.GetByID(vmregInfo.so_id);
                        if (cbInf.ttrang == "D")
                        {
                            var duyetData = new GCNVM { Ma_Dvi = cbInf.ma_dvi, So_ID = cbInf.so_id };
                            var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.MVC_DuyetAPI(duyetData));
                            ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn đã được cấp";
                            ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.MVC_LinkGCNAPI(duyetData));
                            ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng- Số giấy chứng nhận: " + so_hd;
                            ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";
                            return View("viewpolicy");
                        }
                    }    
                    if (vmregInfo.bien_xe == null)
                        vmregInfo.bien_xe = "";

                    var data = await nvcoreSvc.MVC_NhapAPI(regInfo);
                    OrderInfo orderInf;
                    if (data.so_id> 0)
                    {
                        regInfo.so_id = data.so_id;
                        MotorVehicle mvcInf = MVCRepo.GetByID(data.so_id);
                        if(mvcInf.so_id == 0)
                        {
                            MVCRepo.Insert(regInfo);
                        }
                        else if (mvcInf.ttrang == "D")
                        {
                            MVCRepo.Update(regInfo);
                        }
                        if (mvcInf.ttrang=="T")
                        {
                            var duyetData = new GCNXE { ma_dvi = data.ma_dvi, so_id = data.so_id, so_id_dt= data.so_id_dt };
                            var strUrl = await nvcoreSvc.MVC_LinkPreGCNAPI(duyetData);
                            orderInf = new OrderInfo { PolicyID = data.so_id, Amount = 0, CreatedDate = DateTime.Now, PolicyPreviewUrl = strUrl };
                            var vm = new MvcOrderVM { mvcInf = regInfo, orderInf = orderInf };
                            vm.orderInf.Amount = vm.mvcInf.phibb_ds + vm.mvcInf.phitn_ng+ vm.mvcInf.phitn_ts+ vm.mvcInf.phitn_hk+ vm.mvcInf.phitn_hh + vm.mvcInf.phibh_ntx+ vm.mvcInf.phibh_vc;
                            
                            ViewBag.Message = "Quý khách vui lòng kiểm tra thông tin trên đơn bảo hiểm dưới đây, hệ thống sẽ gửi e-policy theo email sau khi thanh toán";
                            ViewBag.Note = "Quay lại để sửa thông tin, Xác nhận để tiến hành thanh toán.";
                            return View("acceptOrder",vm);
                        } 
                        else
                        {
                            var duyetData = new GCNVM { Ma_Dvi = regInfo.ma_dvi, So_ID = regInfo.so_id, CB_Du = regInfo.cb_ql };
                            ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn đã được cấp";
                            var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.MVC_DuyetAPI(duyetData));
                            ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.MVC_LinkGCNAPI(duyetData));
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
                        return View(vmregInfo);
                    }
                }
                else
                {
                    return View(vmregInfo);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Lỗi hệ thống - vui lòng thử lại";
                Log.Error(ex.Message);
                return View(vmregInfo);
            }
        }

        [HttpPost]
        public ActionResult GetHieuXe(string ma)
        {
            var hieuxe = AsyncUtil.RunSync<List<HieuXe>>(() => nvcoreSvc.GetHieuXe(ma));
            return Json(new { data = hieuxe }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBieuPhi(string ma)
        {
            var bieuphi = AsyncUtil.RunSync<List<BieuPhiXCGBB>>(() => nvcoreSvc.GetBieuPhiXCG(ma));
            return Json(new { data = bieuphi }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult acceptOrder()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult acceptOrder(MvcOrderVM vm)
        {
            try
            {
                var lastOrder = OrderRepo.GetLastOrderByPolicyID(vm.mvcInf.so_id);

                if (lastOrder.OrderId > 0)
                {
                    if (lastOrder.Status == 1 || lastOrder.PolicyStatus == "D")
                    {
                        var duyetData = new GCNVM { Ma_Dvi = vm.mvcInf.ma_dvi, So_ID = lastOrder.PolicyID };
                        var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.MVC_DuyetAPI(duyetData));
                        AsyncUtil.RunSync<string>(() => nvcoreSvc.MVC_LinkGCNAPI(duyetData));
                        ViewBag.Message = "Đơn của quý khách đã được thanh toán - Số giấy chứng nhận: " + lastOrder.PolicyNo;
                        return View(vm);
                    }
                }

                var gcn = MVCRepo.GetByID(vm.mvcInf.so_id);
                VNPayOrderVM vnpInf = new VNPayOrderVM();
                vnpInf.Ma_dvi = gcn.ma_dvi;               
                vnpInf.So_ID = vm.mvcInf.so_id;
                vnpInf.Ma_BH = "XE";
                vnpInf.vnpUrlReturn = ConfigurationManager.AppSettings["vnp_Returnurl"] + "/motorvehicle/vnpayresult"; //URL nhan ket qua tra ve 

                var paymentRes = AsyncUtil.RunSync<ApiVNPayResult>(() => nvcoreSvc.VNPay_CreatePayment(vnpInf));
                Log.Info(JsonConvert.SerializeObject(vnpInf));
                Log.Info(JsonConvert.SerializeObject(paymentRes));
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
                            Log.Info("amount: " + lastOrder.Amount + "|" + amount);
                            if (lastOrder.OrderId > 0)
                            {
                                if (lastOrder.Amount == amount)
                                {
                                    var cbInf = MVCRepo.GetByID(lastOrder.PolicyID);

                                    CoreNVService nvcoreSvc = CoreNVService.getInstance();
                                    var duyetData = new GCNVM { Ma_Dvi = cbInf.ma_dvi, So_ID = lastOrder.PolicyID, CB_Du = cbInf.cb_du };
                                    var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.MVC_DuyetAPI(duyetData));

                                    if (so_hd.Trim().Length > 0)
                                    {
                                        lastOrder.PolicyNo = so_hd;
                                        lastOrder.PolicyStatus = "D";
                                    }
                                    cbInf.so_hd = so_hd;
                                    cbInf.ttrang = "D";
                                    if (cbInf.so_id > 0)
                                        MVCRepo.Update(cbInf);

                                    lastOrder.vnp_Message = "Giao dịch thanh toán thành công";
                                    lastOrder.CardType = vnp_CardType;
                                    lastOrder.BankCode = vnp_BankCode;
                                    lastOrder.BankTranNo = vnp_BankTranNo;
                                    lastOrder.vnp_PayDate = vnp_PayDate;
                                    lastOrder.vnp_TransactionNo = vnpayTranId;
                                    lastOrder.vnp_TxnResponseCode = vnp_ResponseCode;
                                    lastOrder.vnp_ResponseDate = DateTime.Now;
                                    var url = AsyncUtil.RunSync<string>(() => nvcoreSvc.MVC_LinkGCNAPI(duyetData));

                                    lastOrder.PolicyUrl = url;
                                    OrderRepo.Update(lastOrder);

                                    //Thanh toan thanh cong
                                    lastOrder = OrderRepo.GetByOrderID(orderId);

                                    ViewBag.Title = "Hoàn tất & nhận e-policy - Thanh toán thành công";
                                    ViewBag.Success = "Chúc mừng bạn đã được bảo vệ bởi Bảo hiểm TNDS Xe cơ giới ";
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
                                    Log.Error("Thanh toan loi, số tiền giao dịch VNP trả về sai OrderID, OrderId={0}, XE TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
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