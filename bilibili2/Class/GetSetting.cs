using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bilibili2.Class
{
    public static class GetSetting
    {
        static SettingHelper st;
        public static bool isDark()
        {
            if (st==null)
            {
                st = new SettingHelper();
            }
            if (st.SettingContains("Drak"))
            {
                return (bool)st.GetSettingValue("Drak");
            }
            else
            {
                return false;
            }
        }


    }
}
