using System;
using System.IO;

namespace SMS_Service
{
    public static class Logger
    {
        /// <summary>
        /// ثبت خطاهای سرویس
        /// </summary>
        /// <param name="ex">خطا</param>
        public static void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(DateTime.Now + ": " + ex.Source.Trim() + "; " +
                                       ex.Message.Trim());
                sw.Flush();
                sw.Close();
            }
            catch
            {
                // ignored
            }
        }
        /// <summary>
        /// ثبت پیام های سرویس
        /// </summary>
        /// <param name="message">پیام</param>
        public static void WriteMessageLog(string message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\MessageLog.txt", true);
                sw.WriteLine(DateTime.Now + ": " + message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
                // igroned
            }
        }
        /// <summary>
        /// خطاهای عدم ذخیره در بانک اطلاعاتی
        /// </summary>
        /// <param name="message"></param>
        public static void WriteErrorSaveToDatabase(string message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\MessageLog.txt", true);
                sw.WriteLine(DateTime.Now.Convert_PersianCalender() + ": " + message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
                // igroned
            }
        }
        public static void WriteMessageSenderLog(string message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\MessageSenderLog.txt", true);
                sw.WriteLine(DateTime.Now.Convert_PersianCalender() + ": " + message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
                // igroned
            }
        }
    }
}