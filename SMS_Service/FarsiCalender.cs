using System;
using System.Globalization;

namespace SMS_Service
{
    public static class FarsiCalender
    {
        public static string Convert_PersianCalender(this DateTime dt)
        {
            var pc = new PersianCalendar();
            var years = pc.GetYear(dt);
            var month = pc.GetMonth(dt);
            var day = pc.GetDayOfMonth(dt);
            var hou = pc.GetHour(dt);
            var minu = pc.GetMinute(dt);
            var sec = pc.GetSecond(dt);
            return new DateTime(years, month, day,hou,minu,sec).ToString("yyyy/MM/dd hh:mm");
        }
    }
}