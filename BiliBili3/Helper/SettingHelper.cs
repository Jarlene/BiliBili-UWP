using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using Windows.ApplicationModel;
using Microsoft.Toolkit.Uwp;
using Windows.UI.StartScreen;

namespace BiliBili3
{
    public static class SettingHelper
    {
        static ApplicationDataContainer container;
        public async static Task<string> Get_HomeInfo()
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            
            if (await localFolder.FileExistsAsync("HomeInfo.json"))
            {
                return await StorageFileHelper.ReadTextFromLocalFileAsync("HomeInfo.json");
            }
            else
            {
                return "";
            }
            // Load some text from a file named appFilename.txt in the local folder 
            
        }
        public async static void Set_HomeInfo(string value)
        {
            //StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
          
            await StorageFileHelper.WriteTextToLocalFileAsync(value, "HomeInfo.json");

           
        }

        #region  外观和常规
        public static string Get_Theme()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["Theme"] != null)
            {
                return (string)container.Values["Theme"];
            }
            else
            {
                return "Pink";
            }
        }

        public static void Set_Theme(string value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["Theme"] = value;
        }

        public static int Get_Rigth()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["Rigth"] != null)
            {
                return (int)container.Values["Rigth"];
            }
            else
            {
                if (SettingHelper.IsPc())
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

       

        public static void Set_Rigth(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["Rigth"] = value;
        }


        public static void Set_CustomBG(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["CustomBG"] = value;
        }

        public static bool Get_CustomBG()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["CustomBG"] != null)
            {
                return (bool)container.Values["CustomBG"];
            }
            else
            {
                Set_CustomBG(false);
                return false;
            }
        }

        public static void Set_BGPath(string value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["BGPath"] = value;
        }

        public static string Get_BGPath()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["BGPath"] != null)
            {
                return container.Values["BGPath"].ToString();
            }
            else
            {
                Set_BGPath("");
                return "";
            }
        }


        public static void Set_BGStretch(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["BGStretch"] = value;
        }

        public static int Get_BGStretch()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["BGStretch"] != null)
            {
                return (int)container.Values["BGStretch"];
            }
            else
            {
                Set_BGStretch(0);
                return 0;
            }
        }



        public static void Set_BGVer(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["BGVer"] = value;
        }

        public static int Get_BGVer()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["BGVer"] != null)
            {
                return (int)container.Values["BGVer"];
            }
            else
            {
                Set_BGVer(1);
                return 1;
            }
        }

        public static void Set_BGOpacity(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["BGOpacity"] = value;
        }

        public static int Get_BGOpacity()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["BGOpacity"] != null)
            {
                return (int)container.Values["BGOpacity"];
            }
            else
            {
                Set_BGOpacity(10);
                return 10;
            }
        }

        public static void Set_FrostedGlass(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["FrostedGlass"] = value;
        }

        public static int Get_FrostedGlass()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["FrostedGlass"] != null)
            {
                return (int)container.Values["FrostedGlass"];
            }
            else
            {
                Set_FrostedGlass(0);
                return 0;
            }
        }

        public static void Set_BGMaxWidth(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["BGMaxWidth"] = value;
        }

        public static int Get_BGMaxWidth()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["BGMaxWidth"] != null)
            {
                return (int)container.Values["BGMaxWidth"];
            }
            else
            {
                Set_BGMaxWidth(0);
                return 0;
            }
        }

        public static void Set_BGMaxHeight(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["BGMaxHeight"] = value;
        }

        public static int Get_BGMaxHeight()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["BGMaxHeight"] != null)
            {
                return (int)container.Values["BGMaxHeight"];
            }
            else
            {
                Set_BGMaxHeight(0);
                return 0;
            }
        }



        public static void Set_BGHor(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["_BGHor"] = value;
        }

        public static int Get__BGHor()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["_BGHor"] != null)
            {
                return (int)container.Values["_BGHor"];
            }
            else
            {
                Set_BGHor(1);
                return 1;
            }
        }


        public static void Set_HideStatus(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["HideStatus"] = value;
        }

        public static bool Get_HideStatus()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["HideStatus"] != null)
            {
                return (bool)container.Values["HideStatus"];
            }
            else
            {
                Set_HideStatus(true);
                return true;
            }
        }


        public static void Set_LoadSplash(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["LoadSplash"] = value;
        }

        public static bool Get_LoadSplash()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["LoadSplash"] != null)
            {
                return (bool)container.Values["LoadSplash"];
            }
            else
            {
                Set_LoadSplash(true);
                return true;
            }
        }


        public static void Set_HideAD(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["HideAD"] = value;
        }

        public static bool Get_HideAD()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["HideAD"] != null)
            {
                return (bool)container.Values["HideAD"];
            }
            else
            {
                Set_HideAD(false);
                return false;
            }
        }

        //sw_RefreshButton
        public static void Set_RefreshButton(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["RefreshButton"] = value;
        }

        public static bool Get_RefreshButton()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["RefreshButton"] != null)
            {
                return (bool)container.Values["RefreshButton"];
            }
            else
            {
                Set_RefreshButton(true);
                return true;
            }
        }

        public static bool Get_First()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["First"] != null)
            {
                return (bool)container.Values["First"];
            }
            else
            {
                Set_First(true);
                return true;
            }
        }

        public static void Set_First(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["First"] = value;
        }

      




        #endregion


        #region 播放器
        public static double Get_Volume()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["Volume"] != null)
            {
                return (double)container.Values["Volume"];
            }
            else
            {
                container.Values["Volume"] = 1;
                return 1;
            }
        }

        public static void Set_Volume(double value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["Volume"] = value;
        }

        public static int Get_PlayQualit()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["PlayQualit"] != null)
            {
                return (int)container.Values["PlayQualit"];
            }
            else
            {
                container.Values["PlayQualit"] = 3;
                return 3;
            }
        }

        public static void Set_PlayQualit(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["PlayQualit"] = value;
        }

        public static int Get_VideoType()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["VideoType"] != null)
            {
                return (int)container.Values["VideoType"];
            }
            else
            {
                container.Values["VideoType"] = 0;
                return 0;
            }
        }

        public static void Set_VideoType(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["VideoType"] = value;
        }

        public static void Set_ForceAudio(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["ForceAudio"] = value;
        }

        public static bool Get_ForceAudio()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["ForceAudio"] != null)
            {
                return (bool)container.Values["ForceAudio"];
            }
            else
            {
                Set_ForceAudio(true);
                return true;
            }
        }

        public static void Set_ForceVideo(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["ForceVideo"] = value;
        }

        public static bool Get_ForceVideo()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["ForceVideo"] != null)
            {
                return (bool)container.Values["ForceVideo"];
            }
            else
            {
                Set_ForceVideo(true);
                return true;
            }
        }


        public static int Get_Playback()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["Playback"] != null)
            {
                return (int)container.Values["Playback"];
            }
            else
            {
                container.Values["Playback"] = 0;
                return 0;
            }
        }

        public static void Set_Playback(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["Playback"] = value;
        }


        public static bool Get_FFmpeg()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["FFmpeg"] != null)
            {
                return (bool)container.Values["FFmpeg"];
            }
            else
            {
                Set_FFmpeg(false);
                return false;
            }
        }

        public static void Set_FFmpeg(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["FFmpeg"] = value;
        }


        public static bool Get_UseH5()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["UseH5"] != null)
            {
                return (bool)container.Values["UseH5"];
            }
            else
            {
                Set_UseH5(false);
                return false;
            }
        }

        public static void Set_UseH5(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["UseH5"] = value;
        }



        public static int Get_ClearLiveComment()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["ClearLiveComment"] != null)
            {
                return (int)container.Values["ClearLiveComment"];
            }
            else
            {
                container.Values["ClearLiveComment"] = 0;
                return 0;
            }
        }

        public static void Set_ClearLiveComment(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["ClearLiveComment"] = value;
        }


        #endregion


        #region 弹幕设置




        public static void Set_Guanjianzi(string value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["Guanjianzi"] = value;
        }

        public static string Get_Guanjianzi()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["Guanjianzi"] != null)
            {
                return (string)container.Values["Guanjianzi"];
            }
            else
            {
                return "Guanjianzi";
            }
        }

        public static void Set_Yonghu(string value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["Yonghu"] = value;
        }

        public static string Get_Yonghu()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["Yonghu"] != null)
            {
                return (string)container.Values["Yonghu"];
            }
            else
            {
                return "Yonghu";
            }
        }




        public static int Get_DMNumber()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DMNumber"] != null)
            {
                return Convert.ToInt32( container.Values["DMNumber"]);
            }
            else
            {
                container.Values["DMNumber"] = 0;
                return 0;
            }
        }

        public static void Set_DMNumber(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DMNumber"] = value;
        }


        //Get_DMBorder
        public static bool Get_DMBorder()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DMBorder"] != null)
            {
                return (bool)container.Values["DMBorder"];
            }
            else
            {
                return true;
            }
        }

        public static void Set_DMBorder(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DMBorder"] = value;
        }


        public static double Get_DMSize()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DMSize"] != null)
            {
                return (double)container.Values["DMSize"];
            }
            else
            {
                if (!SettingHelper.IsPc())
                {
                    container.Values["DMSize"] = 16;
                    return 16;
                }
                else
                {
                    container.Values["DMSize"] = 22;
                    return 22;
                }


            }
        }

        public static void Set_DMSize(double value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DMSize"] = value;
        }


        public static int Get_DMFont()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DMFont"] != null)
            {
                return (int)container.Values["DMFont"];
            }
            else
            {
                container.Values["DMFont"] = 0;
                return 0;
            }
        }

        public static void Set_DMFont(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DMFont"] = value;
        }

        public static double Get_DMSpeed()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DMSpeed"] != null)
            {
                return (double)container.Values["DMSpeed"];
            }
            else
            {

                container.Values["DMSpeed"] = 12;
                return 12;



            }
        }

        public static void Set_DMSpeed(double value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DMSpeed"] = value;
        }

        public static double Get_DMTran()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DMTran"] != null)
            {
                return (double)container.Values["DMTran"];
            }
            else
            {

                container.Values["DMTran"] = 100;
                return 100;



            }
        }

        public static void Set_DMTran(double value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DMTran"] = value;
        }



        public static bool Get_DMVisTop()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DMVisTop"] != null)
            {
                return (bool)container.Values["DMVisTop"];
            }
            else
            {
                Set_DMVisTop(true);
                return true;
            }
        }

        public static void Set_DMVisTop(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DMVisTop"] = value;
        }

        public static bool Get_DMVisBottom()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DMVisBottom"] != null)
            {
                return (bool)container.Values["DMVisBottom"];
            }
            else
            {
                Set_DMVisBottom(true);
                return true;
            }
        }

        public static void Set_DMVisBottom(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DMVisBottom"] = value;
        }

        public static bool Get_DMVisRoll()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DMVisRoll"] != null)
            {
                return (bool)container.Values["DMVisRoll"];
            }
            else
            {
                Set_DMVisRoll(true);
                return true;
            }
        }

        public static void Set_DMVisRoll(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DMVisRoll"] = value;
        }

        public static string Get_DMZZ()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DMZZ"] != null)
            {
                return (string)container.Values["DMZZ"];
            }
            else
            {
                Set_DMZZ("");
                return "";
            }
        }

        public static void Set_DMZZ(string value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DMZZ"] = value;
        }



        #endregion


        #region 下载

        public static int Get_DownQualit()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DownQualit"] != null)
            {
                return (int)container.Values["DownQualit"];
            }
            else
            {
                container.Values["DownQualit"] = 3;
                return 3;
            }
        }

        public static void Set_DownQualit(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DownQualit"] = value;
        }


        public static int Get_DownMode()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DownMode"] != null)
            {
                return (int)container.Values["DownMode"];
            }
            else
            {
                container.Values["DownMode"] = 0;
                return 0;
            }
        }

        public static void Set_DownMode(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DownMode"] = value;
        }



        public static void Set_CustomDownPath(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["CustomDownPath"] = value;
        }

        public static bool Get_CustomDownPath()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["CustomDownPath"] != null)
            {
                return (bool)container.Values["CustomDownPath"];
            }
            else
            {
                Set_CustomDownPath(false);
                return false;
            }
        }

        public static void Set_DownPath(string value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DownPath"] = value;
        }

        public static string Get_DownPath()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DownPath"] != null)
            {
                return (string)container.Values["DownPath"];
            }
            else
            {
                Set_DownPath("系统视频库");
                return "系统视频库";
            }
        }





        public static bool Get_Use4GDown()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["Use4GDown"] != null)
            {
                return (bool)container.Values["Use4GDown"];
            }
            else
            {
                Set_Use4GDown(false);
                return false;
            }
        }

        public static void Set_Use4GDown(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["Use4GDown"] = value;
        }

        #endregion


        #region 通知


        public static bool Get_DTCT()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DTCT"] != null)
            {
                return (bool)container.Values["DTCT"];
            }
            else
            {
                Set_DTCT(true);
                return true;
            }
        }
        public static void Set_DTCT(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DTCT"] = value;
        }

       

        public static bool Get_DT()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["DT"] != null)
            {
                return (bool)container.Values["DT"];
            }
            else
            {
                Set_DT(true);
                return true;
            }
        }

        public static void Set_DT(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["DT"] = value;
        }

        public static bool Get_FJ()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["FJ"] != null)
            {
                return (bool)container.Values["FJ"];
            }
            else
            {
                Set_FJ(true);
                return true;
            }
        }

        public static void Set_FJ(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["FJ"] = value;
        }

        public static string Get_TsDt()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["TsDt"] != null)
            {
                return (string)container.Values["TsDt"];
            }
            else
            {
                Set_TsDt("");
                return "";
            }
        }

        public static void Set_TsDt(string value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["TsDt"] = value;
        }


        #endregion

        #region 黑科技

        public static bool Get_PlayerMode()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["PlayerMode"] != null)
            {
                return (bool)container.Values["PlayerMode"];
            }
            else
            {
                Set_PlayerMode(false);
                return false;
            }
        }

        public static void Set_PlayerMode(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["PlayerMode"] = value;
        }



        public static bool Get_UseHK()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["UseHK"] != null)
            {
                return (bool)container.Values["UseHK"];
            }
            else
            {
                Set_UseHK(false);
                return false;
            }
        }

        public static void Set_UseHK(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["UseHK"] = value;
        }


        public static bool Get_UseTW()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["UseTW"] != null)
            {
                return (bool)container.Values["UseTW"];
            }
            else
            {
                Set_UseTW(false);
                return false;
            }
        }

        public static void Set_UseTW(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["UseTW"] = value;
        }



        public static bool Get_UseCN()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["UseCN"] != null)
            {
                return (bool)container.Values["UseCN"];
            }
            else
            {
                Set_UseCN(false);
                return false;
            }
        }

        public static void Set_UseCN(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["UseCN"] = value;
        }





        #endregion

        #region 用户信息

        public static string Get_UserName()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["UserName"] != null)
            {
                return (string)container.Values["UserName"];
            }
            else
            {
                return "";
            }
        }
        public static void Set_UserName(string value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["UserName"] = value;
        }
        public static string Get_Password()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["Password"] != null)
            {
                return (string)container.Values["Password"];
            }
            else
            {
                return "";
            }
        }
        public static void Set_Password(string value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["Password"] = value;
        }

        public static string Get_Access_key()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["Access_key"] != null)
            {
                return (string)container.Values["Access_key"];
            }
            else
            {
                return "";
            }
        }
        public static void Set_Access_key(string value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["Access_key"] = value;
        }
        #endregion

        #region 系统方法

        static PackageId pack = (Package.Current).Id;
        public static string GetVersion()
        {
            return string.Format("{0}.{1}.{2}.{3}", pack.Version.Major, pack.Version.Minor, pack.Version.Build, pack.Version.Revision);
        }

        public static bool IsPc()
        {
            string device = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (device != "Windows.Mobile")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region 直播弹幕
        public static double Get_LDMSize()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["LDMSize"] != null)
            {
                return (double)container.Values["LDMSize"];
            }
            else
            {
                if (!SettingHelper.IsPc())
                {
                    container.Values["LDMSize"] = 16;
                    return 16;
                }
                else
                {
                    container.Values["LDMSize"] = 22;
                    return 22;
                }


            }
        }

        public static void Set_LDMSize(double value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["LDMSize"] = value;
        }


        public static int Get_LDMFont()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["LDMFont"] != null)
            {
                return (int)container.Values["LDMFont"];
            }
            else
            {
                container.Values["LDMFont"] = 0;
                return 0;
            }
        }

        public static void Set_LDMFont(int value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["LDMFont"] = value;
        }

        public static double Get_LDMSpeed()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["LDMSpeed"] != null)
            {
                return (double)container.Values["LDMSpeed"];
            }
            else
            {

                container.Values["LDMSpeed"] = 100;
                return 100;



            }
        }

        public static void Set_LDMSpeed(double value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["LDMSpeed"] = value;
        }

        public static double Get_LDMTran()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["LDMTran"] != null)
            {
                return (double)container.Values["LDMTran"];
            }
            else
            {

                container.Values["LDMTran"] = 100;
                return 100;



            }
        }

        public static void Set_LDMTran(double value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["LDMTran"] = value;
        }


        public static bool Get_LDMGift()
        {
            container = ApplicationData.Current.LocalSettings;
            if (container.Values["LDMGift"] != null)
            {
                return (bool)container.Values["LDMGift"];
            }
            else
            {
                Set_LDMGift(true);
                return true;
            }
        }

        public static void Set_LDMGift(bool value)
        {
            container = ApplicationData.Current.LocalSettings;
            container.Values["LDMGift"] = value;
        }



        #endregion

        //public async static void PinTile(string id,string par,string name,string imgUrl)
        //{
           
         
        //    Uri square150x150Logo = new Uri(imgUrl, UriKind.RelativeOrAbsolute);
        //    TileSize newTileDesiredSize = TileSize.Square150x150;

        //    SecondaryTile secondaryTile = new SecondaryTile();


        //    secondaryTile.TileId = id;
        //    secondaryTile.DisplayName = name;
        //    secondaryTile.Arguments = par;
        //    //secondaryTile. = square150x150Logo;


        //    secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
        //    secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
        //    secondaryTile.VisualElements.ForegroundText = ForegroundText.Light;
        //    //secondaryTile.VisualElements.Square44x44Logo = new Uri(imgUrl, UriKind.RelativeOrAbsolute);
        //    secondaryTile.VisualElements.Wide310x150Logo = new Uri(imgUrl, UriKind.RelativeOrAbsolute);
        //    //secondaryTile.VisualElements.BackgroundColor = co;
        //    //Windows.Foundation.Rect rect =(FrameworkElement)sender;
        //  //  Windows.UI.Popups.Placement placement = Windows.UI.Popups.Placement.Above;
          
        //      await secondaryTile.RequestCreateAsync();
        //}



    }
}
