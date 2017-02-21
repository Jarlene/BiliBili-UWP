using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BiliBili3
{
    class WebClientClass
    {
        public static async Task<string> GetResults(Uri url)
        {
            HttpBaseProtocolFilter fiter = new HttpBaseProtocolFilter();
            fiter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
         
            using (HttpClient hc = new HttpClient(fiter))
            { 
                HttpResponseMessage hr = await hc.GetAsync(url);
                    hr.EnsureSuccessStatusCode();
                    string results = await hr.Content.ReadAsStringAsync();
                    return results;
                }
        }

        public static async Task<string> GetResults_Proxy(string url)
        {
            string area = "cn";
            if (SettingHelper.Get_UseCN())
            {
                area = "cn";
            }
            if (SettingHelper.Get_UseHK())
            {
                area = "hk";
            }
            if (SettingHelper.Get_UseTW())
            {
                area = "tw";
            }

            HttpBaseProtocolFilter fiter = new HttpBaseProtocolFilter();
            fiter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);

            using (HttpClient hc = new HttpClient(fiter))
            {
                //url
                Uri uri = new Uri(string.Format("http://52uwp.com/api/BiliBili?area={0}&url={1}", area, Uri.EscapeDataString(url)));
                HttpResponseMessage hr = await hc.GetAsync(uri);
                hr.EnsureSuccessStatusCode();
                string results = await hr.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(results);
                if ((int)obj["code"]==0)
                {
                    return obj["message"].ToString();
                }
                else
                {
                    throw new NotSupportedException(obj["message"].ToString());
                }
            }
        }


        public static async Task<IBuffer> GetBuffer(Uri url)
        {
            using (HttpClient hc = new HttpClient())
            {
               
                HttpResponseMessage hr = await hc.GetAsync(url);
               
                hr.EnsureSuccessStatusCode();
                IBuffer results = await hr.Content.ReadAsBufferAsync();
                return results;
            }
        }

        

        public static async Task<string> PostResults(Uri url, string PostContent)
        {
            try
            {
                HttpBaseProtocolFilter fiter = new HttpBaseProtocolFilter();
                fiter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
                using (HttpClient hc = new HttpClient(fiter))
                {
                    hc.DefaultRequestHeaders.Referer = new Uri("http://www.bilibili.com/");
                    var response = await hc.PostAsync(url, new HttpStringContent(PostContent, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));
                    response.EnsureSuccessStatusCode();
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static async Task<string> PostResults(Uri url, string PostContent, string Referer)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders.Referer = new Uri(Referer);
                    var response = await hc.PostAsync(url, new HttpStringContent(PostContent, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));
                    response.EnsureSuccessStatusCode();
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static async Task<string> GetResults_Live(Uri url)
        {

            HttpBaseProtocolFilter fiter = new HttpBaseProtocolFilter();
            fiter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
            //  fiter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.InvalidName);
            // fiter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.UnknownCriticalExtension);
            // myClientHandler.ClientCertificateOptions = System.Net.Http.ClientCertificateOption.Automatic;
            //   myClientHandler.AllowAutoRedirect = true;
            //fiter.ServerCredential.
            using (HttpClient hc = new HttpClient(fiter))
            {

                hc.DefaultRequestHeaders.Add("Buvid", ApiHelper._buvid);
                HttpResponseMessage hr = await hc.GetAsync(url);
                hr.EnsureSuccessStatusCode();
                string results = await hr.Content.ReadAsStringAsync();

                //HttpResponseMessage hr = await hc.GetAsync(url);
                //hr.EnsureSuccessStatusCode();
                //var encodeResults = await hr.Content.ReadAsBufferAsync();
                //string results = Encoding.UTF8.GetString(encodeResults.ToArray(), 0, encodeResults.ToArray().Length);

                return results;
            }


        }

        public static async Task<string> PostResults(Uri url, string PostContent,string Referer,string Home)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders.Referer = new Uri(Referer);
                    hc.DefaultRequestHeaders.Host = new Windows.Networking.HostName(Home);
                    var response = await hc.PostAsync(url, new HttpStringContent(PostContent, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));
                    response.EnsureSuccessStatusCode();
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static async Task<string> PostResults(Uri url, StorageFile PostContent, string Referer, string Home)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    hc.DefaultRequestHeaders.Referer = new Uri(Referer);
                    hc.DefaultRequestHeaders.Host = new Windows.Networking.HostName(Home);
                    IBuffer buffer = await FileIO.ReadBufferAsync(PostContent);
                  
                    var response = await hc.PostAsync(url, new HttpBufferContent(buffer));
                    response.EnsureSuccessStatusCode();
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
      
        public static async Task<string> PostResults(Uri url, IInputStream PostContent, string Referer)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    //hc.DefaultRequestHeaders.Add("Content-Disposition", @"form-data; name=""img_file""");
                    //hc.DefaultRequestHeaders.Add("Content-Type", " application/octet-stream");
                    //hc.DefaultRequestHeaders.Add("Content-Transfer-Encoding", " binary");
                    //hc.DefaultRequestHeaders.Host = new Windows.Networking.HostName(Home);
                    hc.DefaultRequestHeaders.Referer = new Uri(Referer);
                    var response = await hc.PostAsync(url, new HttpStreamContent(PostContent));
                    response.EnsureSuccessStatusCode();
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static async Task<string> GetResultsUTF8Encode(Uri url)
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    HttpResponseMessage hr = await hc.GetAsync(url);
                    hr.EnsureSuccessStatusCode();
                    var encodeResults = await hr.Content.ReadAsBufferAsync();
                    string results = Encoding.UTF8.GetString(encodeResults.ToArray(), 0, encodeResults.ToArray().Length);
                    return results;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static async Task<string> GetResults_Phone(Uri url)
        {
            //HttpBaseProtocolFilter hb = new HttpBaseProtocolFilter();
            //HttpCookieCollection cookieCollection = hb.CookieManager.GetCookies(new Uri("http://bilibili.com/"));
            ////hb.CookieManager.GetCookies(new Uri("http://bilibili.com/"));
            //foreach (HttpCookie item in cookieCollection)
            //{
            //    if (item.Name == "buvid3" || item.Name == "fts")
            //    {
            //        hb.CookieManager.DeleteCookie(item);
            //        // _uid = item.Value;
            //    }
            //}

            HttpBaseProtocolFilter fiter = new HttpBaseProtocolFilter();
            fiter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
            using (HttpClient hc = new HttpClient(fiter))
            {

                hc.DefaultRequestHeaders.Add("user-agent", "Bilibili Windows.Desktop Client/1.2.0.0 (atelier39@outlook.com)");
                hc.DefaultRequestHeaders.Add("Referer", "http://interface.bilibili.com/");
                HttpResponseMessage hr = await hc.GetAsync(url);
                hr.EnsureSuccessStatusCode();
                

                string results = await hr.Content.ReadAsStringAsync();

                //HttpResponseMessage hr = await hc.GetAsync(url);
                //hr.EnsureSuccessStatusCode();
                //var encodeResults = await hr.Content.ReadAsBufferAsync();
                //string results = Encoding.UTF8.GetString(encodeResults.ToArray(), 0, encodeResults.ToArray().Length);

                return results;
            }


        }

    }

   
}
