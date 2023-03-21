using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eBSH.Helper;
using eBSH.Models;
using NLog;
using eBSH.Repositories;
using System.Threading.Tasks;

namespace eBSH.Controllers
{
    public class VNPayController : Controller
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private IOrderRepository OrderRepo;
        private IGCNRepository GCNRepo;
        private IGiftCodeRepository GiftCodeRepo;
        public VNPayController(IOrderRepository orderRepo, IGCNRepository gCNRepo, IGiftCodeRepository giftCodeRepo)
        {
            OrderRepo = orderRepo;
            GCNRepo = gCNRepo;
            GiftCodeRepo = giftCodeRepo;
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
                                        var gcInf = GiftCodeRepo.GetByCode(cbInf.CODE_KM, cbInf.MA_BH, cbInf.CTBH);
                                        gcInf.OrderId = orderId;
                                        if (gcInf.AmountKM == 1)
                                            GiftCodeRepo.Update(gcInf);
                                    }

                                    CoreNVService nvcoreSvc = CoreNVService.getInstance();
                                    var duyetData = new GCNVM { Ma_Dvi = "000", So_ID = lastOrder.PolicyID, CB_Du = "" };
                                    var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.PA_DuyetAPI(duyetData));

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
                                    ViewBag.Success = "Chúc mừng bạn đã được bảo vệ bởi Bảo hiểm BSH TopCare ";
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

        public ActionResult vnpayipn(string vnp_TxnRef, string vnp_TransactionNo, string vnp_ResponseCode, string vnp_SecureHash, string vnp_BankCode, string vnp_BankTranNo, string vnp_CardType, string vnp_PayDate, string vnp_Amount)
        {
            //vnp_Amount=40000000&vnp_BankCode=VNPAY&vnp_CardType=QRCODE&vnp_OrderInfo=Thanh+toan+don+BH+CyberGuard+-+TK%3A0213123123&vnp_PayDate=20201125171612&vnp_ResponseCode=24&vnp_TmnCode=BSHDD002&vnp_TransactionNo=0&vnp_TxnRef=2020112561843602&vnp_SecureHashType=SHA256&vnp_SecureHash=0feb2b4e7e2cded9dbb08a53971c3533d5cb16a7576396fde68bbd5022d1737b
            string returnContent = string.Empty;
            try
            {
                if (Request.QueryString.Count > 0)
                {
                    var vnpayData = Request.QueryString;
                    string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
                    VnPayLibrary vnpay = new VnPayLibrary();
                    if (vnpayData.Count > 0)
                    {
                        foreach (string s in vnpayData)
                        {
                            //get all querystring data
                            vnpay.AddResponseData(s, vnpayData[s]);
                        }
                    }

                    long orderId = Convert.ToInt64(vnp_TxnRef);
                    long vnpayTranId = Convert.ToInt64(vnp_TransactionNo);
                    long amount = Convert.ToInt64(vnp_Amount) / 100; // note amount/100
                    bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                    if (checkSignature)
                    {

                        var lastOrder = OrderRepo.GetByOrderID(orderId);
                        if (lastOrder.OrderId > 0)
                        {
                            if (lastOrder.Amount == amount)
                            {
                                if (lastOrder.Status == 0) // order.Status == 0 (Payment Status pending of Merchant system)
                                {
                                    if (vnp_ResponseCode == "00")
                                    {
                                        //Payment success
                                        Log.Info("Payment Success, OrderId={0}, VNPAY TranId={1}", orderId, vnp_TransactionNo);

                                        lastOrder.Status = 1;   // Payment Status Success of Merchant system
                                        returnContent = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";

                                        CoreNVService nvcoreSvc = CoreNVService.getInstance();
                                        var duyetData = new GCNVM { Ma_Dvi = "000", So_ID = lastOrder.PolicyID };
                                        var so_hd = AsyncUtil.RunSync<string>(() => nvcoreSvc.CBS_DuyetAPI(duyetData));

                                        if (so_hd.Trim().Length > 0)
                                        {
                                            lastOrder.PolicyNo = so_hd;
                                            lastOrder.PolicyStatus = "D";
                                        }

                                        lastOrder.vnp_Message = "Giao dịch thanh toán thành công";
                                        lastOrder.CardType = vnp_CardType;
                                        lastOrder.BankCode = vnp_BankCode;
                                        lastOrder.BankTranNo = vnp_BankTranNo;
                                        lastOrder.vnp_PayDate = vnp_PayDate;
                                        lastOrder.vnp_TransactionNo = vnpayTranId;
                                        lastOrder.vnp_TxnResponseCode = vnp_ResponseCode;
                                        lastOrder.vnp_ResponseDate = DateTime.Now;
                                        lastOrder.Status = 1;
                                        OrderRepo.Update(lastOrder);

                                        var cbInf = GCNRepo.GetByID(lastOrder.PolicyID);
                                        if (so_hd.Trim().Length > 0)
                                        {
                                            cbInf.SO_HD = so_hd;
                                            cbInf.TT = "D";
                                        }

                                        if (cbInf.SO_ID > 0)
                                            GCNRepo.Update(cbInf);

                                        if (cbInf.CODE_KM != null && cbInf.CODE_KM.Trim().Length > 0)//Cập nhật OrderID cho mã GiftCode - để chỉ sử dụng 1 lần
                                        {
                                            var gcInf = GiftCodeRepo.GetByCode(cbInf.CODE_KM, cbInf.MA_BH, cbInf.CTBH);
                                            gcInf.OrderId = orderId;
                                            GiftCodeRepo.Update(gcInf);
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
                                                errDes = "Quý khách nhập sai mật khẩu quá số lần quy định.Xin quý khách vui lòng thực hiện lại giao dịch"; break;
                                            case "13":
                                                errDes = "Quý khách nhập sai mật khẩu xác thực giao dịch(OTP).Xin quý khách vui lòng thực hiện lại giao dịch."; break;
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
                                        //Payment Failed
                                        lastOrder.vnp_Message = "Giao dịch thanh toán không thành công: " + vnp_ResponseCode + " - " + errDes;
                                        lastOrder.CardType = vnp_CardType;
                                        lastOrder.BankCode = vnp_BankCode;
                                        lastOrder.vnp_TxnResponseCode = vnp_ResponseCode;
                                        lastOrder.vnp_PayDate = vnp_PayDate;
                                        lastOrder.vnp_TransactionNo = vnpayTranId;
                                        lastOrder.vnp_ResponseDate = DateTime.Now;
                                        lastOrder.Status = 2; // Payment Status Failed of Merchant system
                                        OrderRepo.Update(lastOrder);

                                        Log.Info("payment Status Failed, OrderId={0}, VNPAY TranId={1},ResponseCode={2}", orderId, vnp_ResponseCode);
                                        returnContent = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
                                    }
                                }
                                else
                                {
                                    returnContent = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                                }
                            }
                            else
                            {
                                returnContent = "{\"RspCode\":\"04\",\"Message\":\"Invalid Amount\"}";
                            }
                        }
                        else
                        {
                            returnContent = "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
                        }
                    }
                    else
                    {
                        Log.Info("Invalid signature, InputData={0}", Request.RawUrl);
                        returnContent = "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
                    }
                }
                else
                {
                    returnContent = "{\"RspCode\":\"99\",\"Message\":\"Input data required\"}";
                }
            }
            catch (Exception ex)
            {
                Log.Error("Lỗi xử lý: InputData={0}, Exception:{1}", Request.RawUrl, ex.Message);
                returnContent = "{\"RspCode\":\"99\",\"Message\":\"Input data required\"}";
            }
            return Content(returnContent);
        }

    }
}