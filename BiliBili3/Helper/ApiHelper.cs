using BiliBili3.Class;
using BiliBili3.Controls;
using BiliBili3.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace BiliBili3
{
    enum LoginStatus
    {
        Succeed,
        Failure,
        Error
    }
    class ApiHelper
    {
        //九幽反馈
        public const string JyAppkey = @"afaaf76fbe62a275d4dc309d6151d3c3";



        public const string _appSecret_Wp = "ba3a4e554e9a6e15dc4d1d70c2b154e3";//Wp
        public const string _appSecret_IOS = "8cb98205e9b2ad3669aad0fce12a4c13";//Ios
        public const string _appSecret_Android = "ea85624dfcf12d7cc7b2b3a94fac1f2c";//Android
        public const string _appSecret_DONTNOT = "2ad42749773c441109bdc0191257a664";
        public const string _appSecret_Android2= "jr3fcr8w7qey8wb0ty5bofurg2cmad8x";
        public const string _appSecret_VIP = "jr3fcr8w7qey8wb0ty5bofurg2cmad8x";
                                              
        public const string _appKey_Android2 = "1d8b6e7d45233436";
        public const string _appKey_VIP = "iVGUTjsxvpLeuDCf";

        public const string _appKey = "422fd9d7289a1dd9";//Wp
        public const string _appKey_IOS = "4ebafd7c4951b366";
        public const string _appKey_Android = "c1b107428d337928";
        public const string _appkey_DONTNOT = "85eb6835b0a1034e";//e5b8ba95cab6104100be35739304c23a
                                                                 //85eb6835b0a1034e,2ad42749773c441109bdc0191257a664
        public static string _buvid = "B3CC4714-C1D3-4010-918B-8E5253E123C16133infoc";
        public static string _hwid = "03008c8c0300d6d1";

        public static string access_key = string.Empty;
        public static List<string> followList;

        public static UserInfoModel userInfo;

        public static string GetSign(string url)
        {
            string result;
            string str = url.Substring(url.IndexOf("?", 4) + 1);
            List<string> list = str.Split('&').ToList();
            list.Sort();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str1 in list)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? "&" : string.Empty));
                stringBuilder.Append(str1);
            }
            stringBuilder.Append(_appSecret_Wp);
            result = MD5.GetMd5String(stringBuilder.ToString()).ToLower();
            return result;
        }


        public static string GetMd5String(string result)
        {
            //可以选择MD5 Sha1 Sha256 Sha384 Sha512
            string strAlgName = HashAlgorithmNames.Md5;

            // 创建一个 HashAlgorithmProvider 对象
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(strAlgName);

            // 创建一个可重用的CryptographicHash对象           
            CryptographicHash objHash = objAlgProv.CreateHash();

            IBuffer buffMsg1 = CryptographicBuffer.ConvertStringToBinary(result, BinaryStringEncoding.Utf16BE);
            objHash.Append(buffMsg1);
            IBuffer buffHash1 = objHash.GetValueAndReset();
            string strHash1 = CryptographicBuffer.EncodeToHexString(buffHash1);
            return strHash1;
        }
        public static string GetSign_Android(string url)
        {
            string result;
            string str = url.Substring(url.IndexOf("?", 4) + 1);
            List<string> list = str.Split('&').ToList();
            list.Sort();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str1 in list)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? "&" : string.Empty));
                stringBuilder.Append(str1);
            }
            stringBuilder.Append(_appSecret_Android);
            result = MD5.GetMd5String(stringBuilder.ToString()).ToLower();
            return result;
        }


        public static string GetSign_Ios(string url)
        {
            string result;
            string str = url.Substring(url.IndexOf("?", 4) + 1);
            List<string> list = str.Split('&').ToList();
            list.Sort();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str1 in list)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? "&" : string.Empty));
                stringBuilder.Append(str1);
            }
            stringBuilder.Append(_appSecret_IOS);
            result = MD5.GetMd5String(stringBuilder.ToString()).ToLower();
            return result;
        }


        public static string GetSign_Android2(string url)
        {
            string result;
            string str = url.Substring(url.IndexOf("?", 4) + 1);
            List<string> list = str.Split('&').ToList();
            list.Sort();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str1 in list)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? "&" : string.Empty));
                stringBuilder.Append(str1);
            }
            stringBuilder.Append(_appSecret_Android2);
            result = MD5.GetMd5String(stringBuilder.ToString()).ToLower();
            return result;
        }
        public static string GetSign_VIP(string url)
        {
            string result;
            string str = url.Substring(url.IndexOf("?", 4) + 1);
            List<string> list = str.Split('&').ToList();
            list.Sort();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str1 in list)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? "&" : string.Empty));
                stringBuilder.Append(str1);
            }
            stringBuilder.Append(_appSecret_VIP);
            result = MD5.GetMd5String(stringBuilder.ToString()).ToLower();
            return result;
        }


        public static string GetSign_DN(string url)
        {
            string result;
            string str = url.Substring(url.IndexOf("?", 4) + 1);
            List<string> list = str.Split('&').ToList();
            list.Sort();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str1 in list)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? "&" : string.Empty));
                stringBuilder.Append(str1);
            }
            stringBuilder.Append(_appSecret_DONTNOT);
            result = GetMd5String(stringBuilder.ToString()).ToLower();
            return result;
        }
        public static long GetTimeSpan
        {
            get { return Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0, 0)).TotalSeconds); }
        }
        public static long GetTimeSpan_2
        {
            get { return Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0, 0)).TotalMilliseconds); }
        }
        public static List<EmojiModel> emojis;
        public static List<FaceModel> emoji;
        public static async void SetEmojis()
        {
            try
            {
                string url = "http://api.bilibili.com/x/v2/reply/emojis";
                string results = await WebClientClass.GetResults(new Uri(url));
                FaceModel model = JsonConvert.DeserializeObject<FaceModel>(results);
                emoji = model.data;
                emojis = new List<EmojiModel>();
                model.data.ForEach(x => x.emojis.ForEach(y => emojis.Add(y)));
            }
            catch (Exception)
            {
            }

        }


        public static async Task<string> GetEncryptedPassword(string passWord)
        {
            string base64String;
            try
            {
                //https://secure.bilibili.com/login?act=getkey&rnd=4928
                //https://passport.bilibili.com/login?act=getkey&rnd=4928
                HttpBaseProtocolFilter httpBaseProtocolFilter = new HttpBaseProtocolFilter();
                httpBaseProtocolFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Expired);
                httpBaseProtocolFilter.IgnorableServerCertificateErrors.Add(Windows.Security.Cryptography.Certificates.ChainValidationResult.Untrusted);
                Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient(httpBaseProtocolFilter);
                //WebClientClass wc = new WebClientClass();
                string url = string.Format(" https://passport.bilibili.com/api/oauth2/getKey?appkey={0}&build=411005&mobi_app=android&platform=wp&ts={1}000",_appKey,GetTimeSpan);
                url += "&sign=" + GetSign(url);

                string stringAsync =await WebClientClass.PostResults(new Uri(url),"");
                JObject jObjects = JObject.Parse(stringAsync);
                string str = jObjects["data"]["hash"].ToString();
                string str1 = jObjects["data"]["key"].ToString();
                string str2 = string.Concat(str, passWord);
                string str3 = Regex.Match(str1, "BEGIN PUBLIC KEY-----(?<key>[\\s\\S]+)-----END PUBLIC KEY").Groups["key"].Value.Trim();
                byte[] numArray = Convert.FromBase64String(str3);
                AsymmetricKeyAlgorithmProvider asymmetricKeyAlgorithmProvider = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
                CryptographicKey cryptographicKey = asymmetricKeyAlgorithmProvider.ImportPublicKey(WindowsRuntimeBufferExtensions.AsBuffer(numArray), 0);
                IBuffer buffer = CryptographicEngine.Encrypt(cryptographicKey, WindowsRuntimeBufferExtensions.AsBuffer(Encoding.UTF8.GetBytes(str2)), null);
                base64String = Convert.ToBase64String(WindowsRuntimeBufferExtensions.ToArray(buffer));
            }
            catch (Exception)
            {
                //throw;
                base64String = passWord;
            }
            return base64String;
        }

        //public static async Task<string> LoginBilibili(string UserName, string Password)
        //{
        //    try
        //    {
        //        //https://api.bilibili.com/login?appkey=422fd9d7289a1dd9&platform=wp&pwd=JPJclVQpH4jwouRcSnngNnuPEq1S1rizxVJjLTg%2FtdqkKOizeIjS4CeRZsQg4%2F500Oye7IP4gWXhCRfHT6pDrboBNNkYywcrAhbOPtdx35ETcPfbjXNGSxteVDXw9Xq1ng0pcP1burNnAYtNRSayEKC1jiugi1LKyWbXpYE6VaM%3D&type=json&userid=xiaoyaocz&sign=74e4c872ec7b9d83d3a8a714e7e3b4b3
        //        //发送第一次请求，得到access_key
        //        string url = "https://api.bilibili.com/login?appkey=422fd9d7289a1dd9&platform=wp&pwd=" + WebUtility.UrlEncode(await GetEncryptedPassword(Password)) + "&type=json&userid=" + WebUtility.UrlEncode(UserName);
        //        url += "&sign="+GetSign(url);

        //        string results = await WebClientClass.GetResults(new Uri(url));
        //        //Json解析及数据判断
        //        LoginModel model = new LoginModel();
        //        model = JsonConvert.DeserializeObject<LoginModel>(results);
        //        if (model.code == -627)
        //        {
        //            return "登录失败，密码错误！";
        //        }
        //        if (model.code == -626)
        //        {
        //            return "登录失败，账号不存在！";
        //        }
        //        if (model.code == -625)
        //        {
        //            return "密码错误多次";
        //        }
        //        if (model.code == -628)
        //        {
        //            return "未知错误";
        //        }
        //        if (model.code == -1)
        //        {
        //            return "登录失败，程序注册失败！请联系作者！";
        //        }
               
        //        if (model.code == 0)
        //        {
        //            access_key = model.access_key;
        //            string urlgo = "http://api.bilibili.com/login/sso?gourl=http%3A%2F%2Fwww.bilibili.com&access_key=" + model.access_key + "&appkey=422fd9d7289a1dd9&platform=android&scale=xhdpi";
        //            urlgo += "&sign=" + ApiHelper.GetSign(urlgo);
        //            WebView WB = new WebView();
        //            WB.Navigate(new Uri(urlgo));

        //           // await WebClientClass.GetResults(new Uri(urlgo));
                   
        //            StorageFolder folder = ApplicationData.Current.LocalFolder;
        //            StorageFile file = await folder.CreateFileAsync("us.bili", CreationCollisionOption.OpenIfExists);
        //            await FileIO.WriteTextAsync(file, model.access_key);
        //        }
        //        //看看存不存在Cookie
        //        HttpBaseProtocolFilter hb = new HttpBaseProtocolFilter();
        //        HttpCookieCollection cookieCollection = hb.CookieManager.GetCookies(new Uri("http://bilibili.com/"));

        //        List<string> ls = new List<string>();
        //        foreach (HttpCookie item in cookieCollection)
        //        {
        //            ls.Add(item.Name);
        //        }
        //        if (ls.Contains("DedeUserID"))
        //        {
        //            return "登录成功";
        //        }
        //        else
        //        {
        //            return "登录失败";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.HResult == -2147012867)
        //        {
        //            return "登录失败，检查你的网络连接！";
        //        }
        //        else
        //        {
        //            return "登录发生错误";
        //        }
                
        //    }

        //}
        public static async Task<string> LoginBilibili(string UserName, string Password)
        {
            try
            {
                //https://api.bilibili.com/login?appkey=422fd9d7289a1dd9&platform=wp&pwd=JPJclVQpH4jwouRcSnngNnuPEq1S1rizxVJjLTg%2FtdqkKOizeIjS4CeRZsQg4%2F500Oye7IP4gWXhCRfHT6pDrboBNNkYywcrAhbOPtdx35ETcPfbjXNGSxteVDXw9Xq1ng0pcP1burNnAYtNRSayEKC1jiugi1LKyWbXpYE6VaM%3D&type=json&userid=xiaoyaocz&sign=74e4c872ec7b9d83d3a8a714e7e3b4b3
                //发送第一次请求，得到access_key
                string url = "https://passport.bilibili.com/api/oauth2/login"; //+ WebUtility.UrlEncode(await GetEncryptedPassword(Password)) + " &type=json&userid=" + WebUtility.UrlEncode(UserName);
                //url += "&sign=" + GetSign(url);

                string content = string.Format("appkey={0}&build=411005&mobi_app=android&password={1}&platform=wp&ts={2}000&username={3}",_appKey, WebUtility.UrlEncode(await GetEncryptedPassword(Password)), GetTimeSpan,WebUtility.UrlEncode(UserName));
                content += "&sign=" + GetSign(content);
                string results = await WebClientClass.PostResults(new Uri(url), content);
                //Json解析及数据判断
                LoginModel model = new LoginModel();
                model = JsonConvert.DeserializeObject<LoginModel>(results);
                if (model.code == -627)
                {
                    return "登录失败，密码错误！";
                }
                if (model.code == -626)
                {
                    return "登录失败，账号不存在！";
                }
                if (model.code == -625)
                {
                    return "密码错误多次";
                }
                if (model.code == -628)
                {
                    return "未知错误";
                }
                if (model.code == -1)
                {
                    return "登录失败，程序注册失败！请联系作者！";
                }

                if (model.code == 0)
                {
                    access_key = model.data.access_token;
                    string urlgo = "http://api.bilibili.com/login/sso?gourl=http%3A%2F%2Fwww.bilibili.com&access_key=" + model.data.access_token + "&appkey=422fd9d7289a1dd9&platform=android&scale=xhdpi";
                    urlgo += "&sign=" + ApiHelper.GetSign(urlgo);
                    try
                    {
                        await WebClientClass.GetResults(new Uri(urlgo));
                    }
                    catch (Exception )
                    {
                     }



                    // await WebClientClass.GetResults(new Uri(urlgo));
                    SettingHelper.Set_Access_key(model.data.access_token);
                    //StorageFolder folder = ApplicationData.Current.LocalFolder;
                    //StorageFile file = await folder.CreateFileAsync("us.bili", CreationCollisionOption.OpenIfExists);
                    //await FileIO.WriteTextAsync(file, model.data.access_token);
                }
                //看看存不存在Cookie
                HttpBaseProtocolFilter hb = new HttpBaseProtocolFilter();
                HttpCookieCollection cookieCollection = hb.CookieManager.GetCookies(new Uri("http://bilibili.com/"));

                List<string> ls = new List<string>();
                foreach (HttpCookie item in cookieCollection)
                {
                    ls.Add(item.Name);
                }
                if (ls.Contains("DedeUserID"))
                {
                    return "登录成功";
                }
                else
                {
                    return "登录失败";
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147012867)
                {
                    return "登录失败，检查你的网络连接！";
                }
                else
                {
                    return "登录发生错误";
                }

            }

        }


        public async static Task<LoginStatus> QRLogin(string oauthKey)
        {
            try
            {
                string url =string.Format( "https://passport.bilibili.com/qrcode/login?access_key={0}&appkey={1}&build=411005&mobi_app=android&oauthKey={2}&platform=wp",ApiHelper.access_key,ApiHelper._appKey,oauthKey);
                url += "&sign=" + ApiHelper.GetSign(url);
                string results= await WebClientClass.GetResults(new Uri(url));
                JObject obj = JObject.Parse(results);
                if ((int)obj["code"]==0)
                {
                    return LoginStatus.Succeed;
                }
                else
                {
                    return LoginStatus.Failure;
                }
            }
            catch (Exception)
            {
                return LoginStatus.Error;
            }
        }


        public static string GetUserId()
        {
            HttpBaseProtocolFilter hb = new HttpBaseProtocolFilter();
            HttpCookieCollection cookieCollection = hb.CookieManager.GetCookies(new Uri("http://bilibili.com/"));
            Dictionary<string, string> ls = new Dictionary<string, string>();
            string a = string.Empty;
            foreach (HttpCookie item in cookieCollection)
            {
                ls.Add(item.Name, item.Value);
                if (item.Name == "DedeUserID")
                {
                    a = item.Value;
                }
            }
            return a;

        }
        public static string GetHwid()
        {
            HttpBaseProtocolFilter hb = new HttpBaseProtocolFilter();
            HttpCookieCollection cookieCollection = hb.CookieManager.GetCookies(new Uri("http://bilibili.com/"));
            Dictionary<string, string> ls = new Dictionary<string, string>();
            string a = string.Empty;
            foreach (HttpCookie item in cookieCollection)
            {
                ls.Add(item.Name, item.Value);
                if (item.Name == "buvid3")
                {
                    a = item.Value;
                }
            }
            return a;

        }
        public static bool IsLogin()
        {
            HttpBaseProtocolFilter hb = new HttpBaseProtocolFilter();
            HttpCookieCollection cookieCollection = hb.CookieManager.GetCookies(new Uri("http://bilibili.com/"));
            List<string> ls = new List<string>();
            foreach (HttpCookie item in cookieCollection)
            {
                ls.Add(item.Name);
            }
            if (!ls.Contains("DedeUserID") || !ls.Contains("DedeUserID__ckMd5"))
            {
                return false;
            }
            else
            {
                hb.CookieManager.GetCookies(new Uri("http://bilibili.com/"));
                return true;

            }
        }

        public static async Task<string> GetVideoUrl(PlayerModel model, int quality)
        {
            switch (model.Mode)
            {
                case PlayMode.Sohu:
                   return await GetSoHuPlayInfo(model.rich_vid, quality);

                default:
                    return await GetBiliUrl(model, quality);
                   
            }
         
        }


        private static async Task<string> GetSoHuPlayInfo(string mid, int quality)
        {
            try
            {
                string[] str = mid.Split('|');
                //http://bangumi.bilibili.com/player/web_api/playurl?cid=10506396&module=movie&player=1&quality=4&ts=1475587467&sign=12b256ad5510d558d07ddf5c4430cd56
                // string url = string.Format("http://bangumi.bilibili.com/player/web_api/playurl?cid={0}&module=movie&player=1&quality=4&ts={1}&appkey={2}", mid,ApiHelper.GetTimeSpen,ApiHelper._appkey_DONTNOT);

                string url = string.Format("http://api.tv.sohu.com/v4/video/info/{0}.json?api_key=1820c56b9d16bbe3381766192e134811&uid=ad99774cfadfe5ecf12457ec5085359a&poid=1&plat=12&sver=3.7.0&partner=419&sysver=10.0.10586.318&ts={1}&verify=43026f88247fcbe0c56411624bd1531e&passport=&aid={2}&program_id=", str[1], ApiHelper.GetTimeSpan_2, str[0]);

                string results = await WebClientClass.GetResults(new Uri(url));
                SohuModel model = JsonConvert.DeserializeObject<SohuModel>(results);

                if (model.status == 200)
                {
                    
                    switch (quality)
                    {
                        case 1:
                            return model.data.url_nor + "&uid=1608111818273358&SOHUSVP=aaxZQgiYTy4uioObZPfLJCVK3BxYwluKsrZ-cpoyfEk&pt=1&prod=h5&pg=1&eye=0&cv=1.0.0&qd=68000&src=11050001&ca=4&cateCode=101&_c=1&appid=tv&oth=&cd=";
                        case 2:
                            return model.data.url_super + "&uid=1608111818273358&SOHUSVP=aaxZQgiYTy4uioObZPfLJCVK3BxYwluKsrZ-cpoyfEk&pt=1&prod=h5&pg=1&eye=0&cv=1.0.0&qd=68000&src=11050001&ca=4&cateCode=101&_c=1&appid=tv&oth=&cd=";
                        case 3:
                            return model.data.url_original + "&uid=1608111818273358&SOHUSVP=aaxZQgiYTy4uioObZPfLJCVK3BxYwluKsrZ-cpoyfEk&pt=1&prod=h5&pg=1&eye=0&cv=1.0.0&qd=68000&src=11050001&ca=4&cateCode=101&_c=1&appid=tv&oth=&cd=";
                        default:
                            return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";

            }
        }

        public static async Task<string> GetBiliUrl(PlayerModel model,int quality)
        {
            try
            {
                



                string url = "http://interface.bilibili.com/playurl?_device=uwp&cid=" + model.Mid + "&otype=xml&quality=" + quality + "&appkey=" + ApiHelper._appKey + "&access_key=" + ApiHelper.access_key + "&type=mp4&mid=" + "" + "&_buvid=" + ApiHelper._buvid + "&_hwid=" + ApiHelper._hwid + "&platform=uwp_desktop" + "&ts=" + ApiHelper.GetTimeSpan;
                url += "&sign=" + ApiHelper.GetSign(url);

                string re = "";
                if (SettingHelper.Get_UseCN() || SettingHelper.Get_UseHK() || SettingHelper.Get_UseTW())
                {
                    re = await WebClientClass.GetResults_Proxy(url);
                }
                else
                {
                    re = await WebClientClass.GetResults_Phone(new Uri(url));
                }

           
               // re = await WebClientClass.GetResults_Phone(new Uri(url));
                string playUrl = Regex.Match(re, "<url>(.*?)</url>").Groups[1].Value;
                playUrl = playUrl.Replace("<![CDATA[", "");
                playUrl = playUrl.Replace("]]>", "");
                return playUrl;
            }
            catch (Exception)
            {
                return "";
            }
         
           // mediaElement.Source = new Uri(playUrl);
        }

    }
    public class PlayParModel
    {
        public List<PlayerModel> viedeolist { get; set; }
        public int play { get; set; }
        public bool laod { get; set; }

    }
    public class SohuModel
    {
        public int status { get; set; }
        public string statusText { get; set; }
        public SohuModel data { get; set; }
        public string url_blue { get; set; }

        public string download_url { get; set; }
        public string url_high { get; set; }
        public string url_nor { get; set; }
        public string url_original { get; set; }
        public string url_super { get; set; }

        public string url_high_mp4 { get; set; }
        public string url_nor_mp4 { get; set; }
        public string url_original_mp4 { get; set; }
        public string url_super_mp4 { get; set; }
    }
    public class PlayerModel
    {
        public PlayMode Mode { get; set; }
        public string No { get; set; }
        public string ImageSrc { get; set; }
        public string rich_vid { get; set; }
        public string Aid { get; set; }
        public string Mid { get; set; }
        public string Title { get; set; }
        public string VideoTitle { get; set; }
        public string episode_id { get; set; }
        public string Path { get; set; }
        public object Parameter { get; set; }
    }
}
