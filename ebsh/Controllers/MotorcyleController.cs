using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Presentation;
using eBSH.Helper;
using eBSH.Models;
using eBSH.Repositories;
using Newtonsoft.Json;
using NLog;
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
    public class MotorcyleController : Controller
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private IOrderRepository OrderRepo;
        private IMotorcycleRepository MCRepo;
        private IRefCodeRepository RefCodeRepo;
        public MotorcyleController(IOrderRepository orderRepo, IMotorcycleRepository mcRepo, IRefCodeRepository refCodeRepo)
        {
            OrderRepo = orderRepo;
            MCRepo = mcRepo;
            RefCodeRepo = refCodeRepo;
        }
        public ActionResult regOrder(string refCode)
        {
            LoadList();
            Motorcycle motorcycle = new Motorcycle();

            //Fetch the Cookie using its Key
            HttpCookie cookie = Request.Cookies["RefCode"];

            if (refCode != null)
            {
                motorcycle.RefCode = refCode;
                if (cookie == null)
                {
                    //Create a Cookie with a suitable Key
                    cookie = new HttpCookie("RefCode");
                }
                //Set the Cookie value
                cookie.Value = refCode;
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
                    motorcycle.RefCode = cookie.Value;
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

            motorcycle.ngay_hl = DateTime.Today.ToString("dd/MM/yyyy");
            motorcycle.ngay_kt = DateTime.Today.AddYears(1).ToString("dd/MM/yyyy");
            return View(motorcycle);
        }

        [HttpPost]
        public async Task<ActionResult> regOrder(Motorcycle motorcycleInfo)
        {
            LoadList();
            try
            {
                if (ModelState.IsValid)
                {
                    CoreNVService nvcoreSvc = CoreNVService.getInstance();
                    //Mặc định BSH TSC                  
                    motorcycleInfo.ma_dvi = "053";
                    motorcycleInfo.ma_dl = "";
                    motorcycleInfo.kenh_kt = "";
                    motorcycleInfo.cb_du = "DND01";
                    motorcycleInfo.cb_ql = "VDT01";
                    if (motorcycleInfo.RefCode != null && motorcycleInfo.RefCode.Trim().Length > 0)
                    {
                        var gcInf = RefCodeRepo.GetByCode(motorcycleInfo.RefCode);
                        if (gcInf.Code != "")
                        {
                            motorcycleInfo.ma_dvi = gcInf.Ma_Dvi;
                            motorcycleInfo.ma_dl = gcInf.Ma_DL;
                            motorcycleInfo.kenh_kt = gcInf.Kenh_KT;
                            motorcycleInfo.cb_du = gcInf.Ma_NQL;// người duyệt đơn
                            motorcycleInfo.cb_ql = gcInf.Ma_NSD;
                        }
                    }

                    if (motorcycleInfo.so_id > 0)
                    {
                        var mcInf = MCRepo.GetByID(motorcycleInfo.so_id);
                        if (mcInf.ttrang == "D")
                        {
                            var duyetData = new GCNVM { Ma_Dvi = motorcycleInfo.ma_dvi, So_ID = motorcycleInfo.so_id };

                            var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.Motorcyle_DuyetAPI(duyetData));
                            ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn đã được cấp";
                            ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.Motorcyle_LinkGCNAPI(duyetData));
                            ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng- Số giấy chứng nhận: " + so_hd;
                            ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";
                            return View("viewpolicy");
                        }
                    }


                    motorcycleInfo.gio_hl = "08:00";
                    motorcycleInfo.gio_kt = "08:00";
                    motorcycleInfo.ngay_cap = DateTime.Today.ToString("dd/MM/yyyy");
                    motorcycleInfo.nd = "";

                    motorcycleInfo.phibh_ds = (motorcycleInfo.phibh_ds) / 100 * 110;

                    // cấp đơn
                    if (motorcycleInfo.bien_xe == null)
                        motorcycleInfo.bien_xe = "";
                    var so_id = await nvcoreSvc.Motorcyle_NhapAPI(motorcycleInfo);
                    OrderInfo orderInf;

                    if (so_id > 0) // cấp đơn thành công insert đơn vào sqlserver
                    {
                        motorcycleInfo.so_id = so_id;
                        var mcInf = MCRepo.GetByID(so_id);
                        if (mcInf.so_id == 0) // chưa có đơn cấp mới
                        {
                            MCRepo.Insert(motorcycleInfo);
                        }
                        else if (mcInf.ttrang == "T") // đã có đơn và chưa duyệt
                        {
                            MCRepo.Update(motorcycleInfo);
                        }

                        if (mcInf.ttrang == "T") // đơn chưa duyệt chuyển sang màn hình duyệt
                        {
                            var duyetData = new GCNVM { Ma_Dvi = motorcycleInfo.ma_dvi, So_ID = so_id };
                            orderInf = new OrderInfo { PolicyID = so_id, Amount = motorcycleInfo.phibh_nn + motorcycleInfo.phibh_ds, CreatedDate = DateTime.Now };
                            var vm = new McOrderVM { mcInf = motorcycleInfo, orderInf = orderInf };
                            vm.orderInf.Amount = vm.mcInf.phibh_nn + vm.mcInf.phibh_ds;
                            //Chuyển màn hình xác nhận thanh toán
                            return View("acceptOrder", vm);
                        }
                        else // đơn đã duyệt chuyển sang mang hình Preview
                        {
                            return View(motorcycleInfo);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Đơn bảo hiểm này đã được thanh toán";
                        return View(motorcycleInfo);
                    }
                }
                else
                {
                    string dataJson = JsonConvert.SerializeObject(motorcycleInfo);
                    ViewBag.Message = "Lỗi hệ thống cấp đơn - vui lòng thử lại";
                    Log.Error("Lỗi nhập API data: {0}", dataJson);
                    return View(motorcycleInfo);
                }

            }
            catch (Exception ex)
            {
                Log.Error("Lỗi nhập data: {0}", ex.Message);
                ViewBag.Message = "Lỗi hệ thống cấp đơn - vui lòng thử lại";
                return View(motorcycleInfo);
            }
        }

        public ActionResult acceptOrder()
        {
            //var mcOrderVM = TempData["mcOrderVM"] as McOrderVM;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult acceptOrder(McOrderVM vm)
        {
            CoreNVService nvcoreSvc = CoreNVService.getInstance();
            try
            {
                //var lastOrder = OrderRepo.GetLastOrderByPolicyID(vm.mcInf.so_id);
                //var duyetData = new GCNVM { Ma_Dvi = "000", So_ID = vm.mcInf.so_id, CB_Du = "ADMINIT" };
                //string so_hd = "";
                //if (lastOrder.OrderId > 0)
                //{
                //    if (lastOrder.Status == 1 || lastOrder.PolicyStatus == "D")
                //    {
                //        Log.Info(JsonConvert.SerializeObject(duyetData));
                //        so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.Motorcyle_DuyetAPI(duyetData));
                //        MCRepo.Update(vm.mcInf);

                //        var mcInf = MCRepo.GetByID(vm.orderInf.PolicyID);

                //        lastOrder.PolicyNo = so_hd;
                //        OrderRepo.Update(lastOrder);
                //        mcInf.so_hd = so_hd;
                //        mcInf.ttrang = "D";
                //        if (mcInf.so_id > 0)
                //            MCRepo.Update(mcInf);
                //        ViewBag.Title = "Hoàn tất & nhận e-policy - Đơn đã được cấp";
                //        ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.Motorcyle_LinkGCNAPI(duyetData));
                //        ViewBag.Message = "Đơn bảo hiểm điện tử (e-Policy) đã được gửi tới địa chỉ email của Quý khách hàng- Số giấy chứng nhận: " + lastOrder.PolicyNo;
                //        ViewBag.Note = "Xin vui lòng kiểm tra email, trong trường hợp cần hỗ trợ xin liên hệ: 1900969609";
                //        Log.Info("Luồng 1");
                //        return View("viewpolicy");
                //    }
                //    else
                //    {
                //        Log.Info("Luồng 2");
                //        vm.orderInf.OrderId = lastOrder.OrderId + 1;
                //    }
                //}
                //else
                //{
                //    Log.Info("Luồng 3");
                //    vm.orderInf.OrderId = vm.orderInf.PolicyID * 100;
                //}
                //var mcyInf = MCRepo.GetByID(vm.orderInf.PolicyID);
                //vm.orderInf.vnp_Message = "";
                //vm.orderInf.PolicyStatus = "D";
                //vm.orderInf.vnp_IpAddr = Utils.GetIpAddress();
                //vm.orderInf.CreatedDate = DateTime.Now;
                //vm.orderInf.Status = 1;
                //vm.orderInf.vnp_ResponseDate = DateTime.Now;
                //OrderRepo.Insert(vm.orderInf);
                //so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.Motorcyle_DuyetAPI(duyetData));

                //ViewBag.PolicyUrl = AsyncUtil.RunSync<string>(() => nvcoreSvc.Motorcyle_LinkGCNAPI(duyetData));
                //vm.orderInf.PolicyNo = so_hd;
                //OrderRepo.Update(vm.orderInf);
                //mcyInf.so_hd = so_hd;
                //mcyInf.ttrang = "D";
                //if (mcyInf.so_id > 0)
                //    MCRepo.Update(mcyInf);

                //Log.Info(ViewBag.PolicyUrl);

                //return View("viewpolicy");

                // Thanh toán VN Pay

                var lastOrder = OrderRepo.GetLastOrderByPolicyID(vm.mcInf.so_id);

                if (lastOrder.OrderId > 0)
                {
                    if (lastOrder.Status == 1 || lastOrder.PolicyStatus == "D")
                    {
                        var duyetData = new GCNVM { Ma_Dvi = vm.mcInf.ma_dvi, So_ID = lastOrder.PolicyID };
                        var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.Motorcyle_DuyetAPI(duyetData));
                        AsyncUtil.RunSync<string>(() => nvcoreSvc.Motorcyle_LinkGCNAPI(duyetData));

                        ViewBag.Message = "Đơn của quý khách đã được thanh toán - Số giấy chứng nhận: " + lastOrder.PolicyNo;
                        return View(vm);
                    }
                }
                var gcn = MCRepo.GetByID(vm.mcInf.so_id);
                VNPayOrderVM vnpInf = new VNPayOrderVM();
                vnpInf.Ma_dvi = vm.mcInf.ma_dvi;
                vnpInf.So_ID = vm.mcInf.so_id;
                vnpInf.Ma_BH = "2B";
                vnpInf.vnpUrlReturn = ConfigurationManager.AppSettings["vnp_Returnurl"] + "/motorcyle/vnpayresult"; //URL nhan ket qua tra ve 

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
                    //vm.orderInf.Amount = 66000;
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
                Log.Error(ex);
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
                                    var cbInf = MCRepo.GetByID(lastOrder.PolicyID);

                                    CoreNVService nvcoreSvc = CoreNVService.getInstance();
                                    var duyetData = new GCNVM { Ma_Dvi = cbInf.ma_dvi, So_ID = lastOrder.PolicyID, CB_Du = cbInf.cb_du };
                                    var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.Motorcyle_DuyetAPI(duyetData));

                                    if (so_hd.Trim().Length > 0)
                                    {
                                        lastOrder.PolicyNo = so_hd;
                                        lastOrder.PolicyStatus = "D";
                                    }
                                    cbInf.so_hd = so_hd;
                                    cbInf.ttrang = "D";
                                    if (cbInf.so_id > 0)
                                        MCRepo.Update(cbInf);

                                    lastOrder.vnp_Message = "Giao dịch thanh toán thành công";
                                    lastOrder.CardType = vnp_CardType;
                                    lastOrder.BankCode = vnp_BankCode;
                                    lastOrder.BankTranNo = vnp_BankTranNo;
                                    lastOrder.vnp_PayDate = vnp_PayDate;
                                    lastOrder.vnp_TransactionNo = vnpayTranId;
                                    lastOrder.vnp_TxnResponseCode = vnp_ResponseCode;
                                    lastOrder.vnp_ResponseDate = DateTime.Now;
                                    var url = AsyncUtil.RunSync<string>(() => nvcoreSvc.Motorcyle_LinkGCNAPI(duyetData));

                                    lastOrder.PolicyUrl = url;
                                    OrderRepo.Update(lastOrder);

                                    //Thanh toan thanh cong
                                    lastOrder = OrderRepo.GetByOrderID(orderId);

                                    ViewBag.Title = "Hoàn tất & nhận e-policy - Thanh toán thành công";
                                    ViewBag.Success = "Chúc mừng bạn đã được bảo vệ bởi Bảo hiểm TNDS Xe máy ";
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
                                    Log.Error("Thanh toan loi, số tiền giao dịch VNP trả về sai OrderID, OrderId={0}, 2B TranId={1},ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
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

        public void LoadList()
        {
            List<ThoiHan> lsThoiHan = new List<ThoiHan>
            {
                new ThoiHan { ID = 1, TH = "1 năm" },
                new ThoiHan { ID = 2, TH = "2 năm" },
                new ThoiHan { ID = 3, TH = "3 năm" }
            };

            List<LoaiXe> lsLoaiXe = new List<LoaiXe>
            {
                new LoaiXe { ID = "0", LX = "-- Chọn loại xe --" },
                new LoaiXe { ID = "N", LX = "Xe dưới 50cc" },
                new LoaiXe { ID = "L", LX = "Xe trên 50cc" },
                new LoaiXe { ID = "B", LX = "Xe 3 bánh" },
                new LoaiXe { ID = "D", LX = "Xe máy điện" }
            };

            List<TNN> lsTNN = new List<TNN>
            {
                new TNN { Phi = 0, STBH = "--Chọn số tiền BH--" },
                new TNN { Phi = 5000, STBH = "5 triệu đồng/người/vụ" },
                new TNN { Phi = 10000, STBH = "10 triệu đồng/người/vụ" }
            };

            ViewBag.thoihan = lsThoiHan;
            ViewBag.loaixe = lsLoaiXe;
            ViewBag.tnn = lsTNN;
        }
    }
}