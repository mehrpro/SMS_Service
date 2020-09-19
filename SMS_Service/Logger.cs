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
        public static async void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                await sw.WriteLineAsync(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " +
                                        ex.Message.ToString().Trim());
                await sw.FlushAsync();
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
        public static async void WriteMessageLog(string message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\MessageLog.txt", true);
                await sw.WriteLineAsync(DateTime.Now.ToString() + ": " + message);
                await sw.FlushAsync();
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
        public static async void WriteErrorSaveToDatabase(string message)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\MessageLog.txt", true);
                await sw.WriteLineAsync(DateTime.Now.Convert_PersianCalender() + ": " + message);
                await sw.FlushAsync();
                sw.Close();
            }
            catch
            {
                // igroned
            }
        }
    }
}