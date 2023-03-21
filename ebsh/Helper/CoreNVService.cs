using eBSH.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Threading;
using NLog;
using System.Web.Helpers;
using System.Drawing;
using static eBSH.Helper.ApiLoaiXeResponse;

namespace eBSH.Helper
{
    /// <summary>
    /// Helper class to run async methods within a sync process.
    /// </summary>
    public static class AsyncUtil
    {
        private static readonly TaskFactory _taskFactory = new
            TaskFactory(CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default);

        /// <summary>
        /// Executes an async Task method which has a void return value synchronously
        /// USAGE: AsyncUtil.RunSync(() => AsyncMethod());
        /// </summary>
        /// <param name="task">Task method to execute</param>
        public static void RunSync(Func<Task> task)
            => _taskFactory
                .StartNew(task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        /// <summary>
        /// Executes an async Task<T> method which has a T return type synchronously
        /// USAGE: T result = AsyncUtil.RunSync(() => AsyncMethod<T>());
        /// </summary>
        /// <typeparam name="TResult">Return Type</typeparam>
        /// <param name="task">Task<T> method to execute</param>
        /// <returns></returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> task)
            => _taskFactory
                .StartNew(task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
    }

    public class CoreNVService
    {
        private static CoreNVService instance;
        private static string token;
        private static DateTime tokenExpired;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private string ServiceUrl = "https://aut.bshc.com.vn";

        private CoreNVService()
        {
            ServiceUrl = ConfigurationManager.AppSettings["NVAPIUrl"];
            token = "";
            tokenExpired = DateTime.Now;
        }

        public static HttpClient GetClient(string baseAddress, string token)
        {
            var authValue = new AuthenticationHeaderValue("Bearer", token);

            var client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authValue },
                BaseAddress = new Uri(baseAddress)
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        public static HttpClient GetClient(string baseAddress)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress),
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };
            return client;
        }
        public static CoreNVService getInstance()
        {
            if (instance == null)
            {
                instance = new CoreNVService();
            }
            return instance;
        }

        private async Task<string> GetToken()
        {
            try
            {
                using (var client = GetClient(ServiceUrl))
                {
                    var values = new Dictionary<string, string> {
                        { "username", ConfigurationManager.AppSettings["NVAPIUser"] },
                        { "password", ConfigurationManager.AppSettings["NVAPIPwd"] },
                        { "grant_type", "password"}
                    };

                    var postData = new FormUrlEncodedContent(values);

                    HttpResponseMessage responseToken = await client.PostAsync("oauth/token", postData);
                    if (responseToken.IsSuccessStatusCode)
                    {
                        Task<string> dataToken = responseToken.Content.ReadAsStringAsync();

                        var dataResult = JsonConvert.DeserializeObject<GetTokenResponse>(dataToken.Result);
                        if (dataResult.access_token.Trim().Length > 0)
                        {
                            var token = dataResult.access_token;
                            tokenExpired = DateTime.Now.AddSeconds(dataResult.expires_in);
                            return token;
                        }
                        else
                            return "";

                    }
                    else
                    {
                        Log.Error(responseToken.ReasonPhrase);
                        return responseToken.ReasonPhrase;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }
        }

        public async Task<ApiVNPayResult> VNPay_CreatePayment(VNPayOrderVM vnpayInf)
        {
            if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                token = await GetToken();
            string dataJson = JsonConvert.SerializeObject(vnpayInf);
            var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");
            ApiVNPayResult res = new ApiVNPayResult { url = "", vnpayOrderID = "0", vnp_TmnCode = "" };
            try
            {
                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/vnpay/createpayment", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiVNPayResponse kq = JsonConvert.DeserializeObject<ApiVNPayResponse>(resData.Result);
                        if (kq.Status == "OK")
                            return (kq.Data);
                        else
                        {
                            Log.Error(resData.Result);
                            return res;
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return res;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return res;
            }

        }


        public async Task<string> CBS_DuyetAPI(GCNVM data)
        {
            if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                token = await GetToken();
            string dataJson = JsonConvert.SerializeObject(data);
            var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");
            try
            {
                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/cbsins/approvecert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiDuyetResponse kq = JsonConvert.DeserializeObject<ApiDuyetResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                            return (kq.Data.soHD);
                        else
                        {
                            Log.Error(resData.Result);
                            return "";
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }
        }

        public async Task<string> CBS_LinkGCNAPI(GCNVM data)
        {
            if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                token = await GetToken();
            string dataJson = JsonConvert.SerializeObject(data);
            var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");
            try
            {
                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/cbsins/signedcert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiLinkResponse kq = JsonConvert.DeserializeObject<ApiLinkResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                            return (kq.Data.url);
                        else
                        {
                            Log.Error(resData.Result);
                            return "";
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }
        }

        public async Task<string> CBS_LinkPreGCNAPI(GCNVM data)
        {
            if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                token = await GetToken();
            string dataJson = JsonConvert.SerializeObject(data);
            var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");

            using (var client = GetClient(ServiceUrl, token))
            {
                var response = await client.PostAsync("/api/cbsins/previewcert", postData);
                if (response.IsSuccessStatusCode)
                {
                    Task<string> resData = response.Content.ReadAsStringAsync();
                    ApiLinkResponse kq = JsonConvert.DeserializeObject<ApiLinkResponse>(resData.Result);
                    if (kq.ErrCode == "000")
                        return (kq.Data.url);
                    else
                    {
                        Log.Error(resData.Result);
                        return "";
                    }
                }
                else
                {
                    Log.Error(response.ReasonPhrase);
                    return "";
                }
            }
        }
        public async Task<decimal> CBS_NhapAPI(PHH_CBS_GCN data)
        {
            if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                token = await GetToken();
            string dataJson = JsonConvert.SerializeObject(data);
            var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");

            using (var client = GetClient(ServiceUrl, token))
            {
                var response = await client.PostAsync("/api/cbsins/addcert", postData);
                if (response.IsSuccessStatusCode)
                {
                    Task<string> resData = response.Content.ReadAsStringAsync();
                    ApiDuyetResponse kq = JsonConvert.DeserializeObject<ApiDuyetResponse>(resData.Result);
                    if (kq.ErrCode == "000" || kq.Data.soID != null)
                        return decimal.Parse(kq.Data.soID);
                    else
                    {
                        Log.Error(resData.Result);
                        return 0;
                    }
                }
                else
                {
                    Log.Error(response.ReasonPhrase);
                    return 0;
                }
            }
        }


        public async Task<string> PA_DuyetAPI(GCNVM data)
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                string dataJson = JsonConvert.SerializeObject(data);
                var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");

                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/pains/approvecert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiDuyetResponse kq = JsonConvert.DeserializeObject<ApiDuyetResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                            return (kq.Data.soHD);
                        else
                        {
                            Log.Info(resData.Result);
                            return "";
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }

        }

        public async Task<decimal> PA_NhapAPI(CN_PA_GCN data)
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                string dataJson = JsonConvert.SerializeObject(data);
                var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");
                //MessageBox.Show(postData.ToString());
                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/pains/addcert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiDuyetResponse kq = JsonConvert.DeserializeObject<ApiDuyetResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                            return decimal.Parse(kq.Data.soID);
                        else
                        {
                            Log.Info(resData.Result);
                            return 0;
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public async Task<string> PA_LinkPreGCNAPI(GCNVM data)
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                string dataJson = JsonConvert.SerializeObject(data);
                var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");

                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/pains/previewcert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiLinkResponse kq = JsonConvert.DeserializeObject<ApiLinkResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                            return (kq.Data.url);
                        else
                        {
                            Log.Info(resData.Result);
                            return "";
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }
        }

        public async Task<string> PA_LinkGCNAPI(GCNVM data)
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                string dataJson = JsonConvert.SerializeObject(data);
                var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");

                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/pains/signedcert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiLinkResponse kq = JsonConvert.DeserializeObject<ApiLinkResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                            return (kq.Data.url);
                        else
                        {
                            Log.Info(resData.Result);
                            return "";
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }
        }
        public async Task<List<HangXe>> GetHangXe()
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                using (var client = GetClient(ServiceUrl, token))
                {
                    List<HangXe> lstHangXe = new List<HangXe>();
                    var response = await client.GetAsync("api/dmht/hangxe");
                    if (response.IsSuccessStatusCode)
                    {

                        var stringg = await response.Content.ReadAsStringAsync();
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiHangXeResponse kq = JsonConvert.DeserializeObject<ApiHangXeResponse>(stringg);
                        lstHangXe = kq.Data;
                        if (kq.ErrCode == "000")
                        {
                            //Log.Info(JsonConvert.SerializeObject(lstHangXe));
                            return lstHangXe;
                        }
                        else
                        {
                            Log.Info(kq);
                            return lstHangXe;
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return lstHangXe;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<HieuXe>> GetHieuXe(string hangXe)
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                using (var client = GetClient(ServiceUrl, token))
                {
                    List<HieuXe> lstHieuXe = new List<HieuXe>();
                    var response = await client.GetAsync("api/dmht/hieuxe?hangxe=" + hangXe);
                    if (response.IsSuccessStatusCode)
                    {

                        var stringg = await response.Content.ReadAsStringAsync();
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiHieuXeResponse kq = JsonConvert.DeserializeObject<ApiHieuXeResponse>(stringg);
                        lstHieuXe = kq.Data;
                        if (kq.ErrCode == "000")
                        {
                            //Log.Info(JsonConvert.SerializeObject(lstHieuXe));
                            return lstHieuXe;
                        }
                        else
                        {
                            Log.Info(kq);
                            return lstHieuXe;
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return lstHieuXe;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<NhomXe>> GetNhomXe()
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                using (var client = GetClient(ServiceUrl, token))
                {
                    List<NhomXe> lstHangXe = new List<NhomXe>();
                    var response = await client.GetAsync("api/dmht/nhomxe");
                    if (response.IsSuccessStatusCode)
                    {

                        var stringg = await response.Content.ReadAsStringAsync();
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiNhomXeResponse kq = JsonConvert.DeserializeObject<ApiNhomXeResponse>(stringg);
                        lstHangXe = kq.Data;
                        if (kq.ErrCode == "000")
                        {
                            //Log.Info(JsonConvert.SerializeObject(lstHangXe));
                            return lstHangXe;
                        }
                        else
                        {
                            Log.Info(kq);
                            return lstHangXe;
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return lstHangXe;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<LoaiXe_XCG>> GetLoaiXe()
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                using (var client = GetClient(ServiceUrl, token))
                {
                    List<LoaiXe_XCG> lstHangXe = new List<LoaiXe_XCG>();
                    var response = await client.GetAsync("api/dmht/loaixe");
                    if (response.IsSuccessStatusCode)
                    {

                        var stringg = await response.Content.ReadAsStringAsync();
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiLoaiXeResponse kq = JsonConvert.DeserializeObject<ApiLoaiXeResponse>(stringg);
                        lstHangXe = kq.Data;
                        if (kq.ErrCode == "000")
                        {
                            //Log.Info(JsonConvert.SerializeObject(lstHangXe));
                            return lstHangXe;
                        }
                        else
                        {
                            Log.Info(kq);
                            return lstHangXe;
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return lstHangXe;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<MaBHXe>> GetMaBHXe()
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                using (var client = GetClient(ServiceUrl, token))
                {
                    List<MaBHXe> lstHangXe = new List<MaBHXe>();
                    var response = await client.GetAsync("api/dmht/mabhxe");
                    if (response.IsSuccessStatusCode)
                    {

                        var stringg = await response.Content.ReadAsStringAsync();
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiMaBHXeResponse kq = JsonConvert.DeserializeObject<ApiMaBHXeResponse>(stringg);
                        lstHangXe = kq.Data;
                        if (kq.ErrCode == "000")
                        {
                            //Log.Info(JsonConvert.SerializeObject(lstHangXe));
                            return lstHangXe;
                        }
                        else
                        {
                            Log.Info(kq);
                            return lstHangXe;
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return lstHangXe;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<BieuPhiXCGBB>> GetBieuPhiXCG(string maBH)
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                using (var client = GetClient(ServiceUrl, token))
                {
                    List<BieuPhiXCGBB> lstBieuPhi = new List<BieuPhiXCGBB>();
                    var response = await client.GetAsync("api/dmht/xcgbb?mabh="+ maBH);
                    if (response.IsSuccessStatusCode)
                    {
                        var stringg = await response.Content.ReadAsStringAsync();
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiBieuPhiXCGResponse kq = JsonConvert.DeserializeObject<ApiBieuPhiXCGResponse>(stringg);
                        lstBieuPhi = kq.Data;
                        if (kq.ErrCode == "000")
                        {
                            return lstBieuPhi;
                        }
                        else
                        {
                            Log.Info(kq);
                            return lstBieuPhi;
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return lstBieuPhi;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<decimal> Motorcyle_NhapAPI(Motorcycle data)
        {
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                string dataJson = JsonConvert.SerializeObject(data);
                var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");
                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/motor/addcert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiDuyetResponse kq = JsonConvert.DeserializeObject<ApiDuyetResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                        {
                            return decimal.Parse(kq.Data.soID);
                        }
                        else
                        {
                            Log.Info(resData.Result);
                            return 0;
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return 3;
            }
        }

        public async Task<string> Motorcyle_DuyetAPI(GCNVM data)
        {
            if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                token = await GetToken();
            string dataJson = JsonConvert.SerializeObject(data);
            var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");
            try
            {
                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/motor/approvecert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiDuyetResponse kq = JsonConvert.DeserializeObject<ApiDuyetResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                            return (kq.Data.soHD);
                        else
                        {
                            Log.Error(resData.Result);
                            return "";
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }

        }
        public async Task<string> Motorcyle_LinkGCNAPI(GCNVM data)
        {
            if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                token = await GetToken();
            string dataJson = JsonConvert.SerializeObject(data);
            var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");
            try
            {
                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/motor/signedcert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiLinkResponse kq = JsonConvert.DeserializeObject<ApiLinkResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                            return (kq.Data.url);
                        else
                        {
                            Log.Error(resData.Result);
                            return "";
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }
        }
        public async Task<GCNXE> MVC_NhapAPI(MotorVehicle data)
        {
            GCNXE gCNXE = new GCNXE();
            try
            {
                if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                    token = await GetToken();
                string dataJson = JsonConvert.SerializeObject(data);
                var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");
                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/mvc/add", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiDuyetResponse kq = JsonConvert.DeserializeObject<ApiDuyetResponse>(resData.Result);
                        gCNXE.so_id = decimal.Parse(kq.Data.soID);
                        gCNXE.so_id_dt = decimal.Parse(kq.Data.soID_dt);
                        if (kq.ErrCode == "000")
                        {
                            return gCNXE;
                        }
                        else
                        {
                            Log.Info(resData.Result);
                            return gCNXE;
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return gCNXE;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return gCNXE;
            }
        }
        public async Task<string> MVC_LinkGCNAPI(GCNVM data)
        {
            if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                token = await GetToken();
            string dataJson = JsonConvert.SerializeObject(data);
            var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");
            try
            {
                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/mvc/signedcert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiLinkResponse kq = JsonConvert.DeserializeObject<ApiLinkResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                            return (kq.Data.url);
                        else
                        {
                            Log.Error(resData.Result);
                            return "";
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }
        }
        public async Task<string> MVC_DuyetAPI(GCNVM data)
        {
            if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                token = await GetToken();
            string dataJson = JsonConvert.SerializeObject(data);
            var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");
            try
            {
                using (var client = GetClient(ServiceUrl, token))
                {
                    var response = await client.PostAsync("/api/mvc/approvecert", postData);
                    if (response.IsSuccessStatusCode)
                    {
                        Task<string> resData = response.Content.ReadAsStringAsync();
                        ApiDuyetResponse kq = JsonConvert.DeserializeObject<ApiDuyetResponse>(resData.Result);
                        if (kq.ErrCode == "000")
                            return (kq.Data.soHD);
                        else
                        {
                            Log.Error(resData.Result);
                            return "";
                        }
                    }
                    else
                    {
                        Log.Error(response.ReasonPhrase);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return "";
            }

        }
        public async Task<string> MVC_LinkPreGCNAPI(GCNXE data)
        {
            if (token == "" || tokenExpired <= DateTime.Now.AddSeconds(-10))
                token = await GetToken();
            string dataJson = JsonConvert.SerializeObject(data);
            var postData = new StringContent(dataJson, Encoding.UTF8, "application/json");

            using (var client = GetClient(ServiceUrl, token))
            {
                var response = await client.PostAsync("/api/mvc/precert", postData);
                if (response.IsSuccessStatusCode)
                {
                    Task<string> resData = response.Content.ReadAsStringAsync();
                    ApiLinkResponse kq = JsonConvert.DeserializeObject<ApiLinkResponse>(resData.Result);
                    if (kq.ErrCode == "000")
                        return (kq.Data.url);
                    else
                    {
                        Log.Error(resData.Result);
                        return "";
                    }
                }
                else
                {
                    Log.Error(response.ReasonPhrase);
                    return "";
                }
            }
        }

    }
    public class GetTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class ApiDuyetResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ErrCode { get; set; }
        public ApiData Data { get; set; }
    }

    public class ApiLinkResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ErrCode { get; set; }
        public ApiLinkData Data { get; set; }
    }
    public class ApiData
    {
        public string soHD { get; set; }
        public string soID { get; set; }
        public string soID_dt { get; set; }
    }

    public class ApiLinkData
    {
        public string url { get; set; }
        public string soID { get; set; }
    }
    public class GCNVM
    {
        public string Ma_Dvi { get; set; }
        public decimal So_ID { get; set; }
        public string CB_Du { get; set; }
    }
    public class GCNXE
    {
        public string ma_dvi { get; set; }
        public decimal so_id { get; set; }
        public decimal so_id_dt { get; set; }
    }

    public class ApiVNPayResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ErrCode { get; set; }
        public ApiVNPayResult Data { get; set; }
    }
    public class ApiVNPayResult
    {
        public string url { get; set; }
        public string vnpayOrderID { get; set; }
        public string vnp_TmnCode { get; set; }
    }
    public class ApiHangXeResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ErrCode { get; set; }
        public List<HangXe> Data { get; set; }
    }
    public class ApiHieuXeResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ErrCode { get; set; }
        public List<HieuXe> Data { get; set; }
    }
    public class ApiNhomXeResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ErrCode { get; set; }
        public List<NhomXe> Data { get; set; }
    }
    public class ApiLoaiXeResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ErrCode { get; set; }
        public List<LoaiXe_XCG> Data { get; set; }
    }
    public class ApiMaBHXeResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ErrCode { get; set; }
        public List<MaBHXe> Data { get; set; }
    }
    public class ApiBieuPhiXCGResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string ErrCode { get; set; }
        public List<BieuPhiXCGBB> Data { get; set; }
    }
}