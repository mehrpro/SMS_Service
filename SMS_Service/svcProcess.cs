using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace SMS_Service
{
    public static class SvcProcess
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
        /// لیست تگ
        /// </summary>
        public class RegisteredTagList
        {
            public int ID { get; set; }
            public string Tagid { get; set; }
            public DateTime dateRegister { get; set; }
        }
        /// <summary>
        /// ثبت تگ جدید در SQL
        /// </summary>
        /// <param name="registeredTagLists"></param>
        /// <returns></returns>
        private static bool RegisterINSqlServer(List<RegisteredTagList> registeredTagLists)
        {
            using (var connSQL = new SqlConnection("data source=10.1.1.4;initial catalog=infraBase ;user id=sa;password=Ss987654;MultipleActiveResultSets=True;"))
            {
                try
                {
                    connSQL.Open();
                    foreach (var item in registeredTagLists)
                    {
                        var str =
                            $"insert into tagRec(TagID,DateTimeRegister,MysqlID) Values('{item.Tagid}','{item.dateRegister.Date}','{item.ID}')";
                        var cmd = new SqlCommand(str, connSQL);
                        var resly = cmd.ExecuteNonQuery();
                    }
                    connSQL.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    var str = ex.Message;
                    connSQL.Close();
                    return false;
                }
            }


        }
        private static List<RegisteredTagList> ReaderSQL()
        {
            string cs = @"server=10.1.1.3;port=3306;userid=fm;password=Ss987654;database=schooldb;SSL Mode = None";
            var list = new List<RegisteredTagList>();
            using (var conn = new MySqlConnection(cs))
            {
                try
                {
                    conn.Open();
                    var cmdString = "SELECT * FROM schooldb.tagrecive where registered = 1";
                    var cmd = new MySqlCommand(cmdString, conn);
                    var result = cmd.ExecuteReader();
                    while (result.Read())
                    {
                        list.Add(new RegisteredTagList()
                        {
                            ID = result.GetInt32(0),
                            Tagid = result.GetString(1),
                            dateRegister = result.GetDateTime(2),
                        });
                    }
                    conn.Clone();
                    return list;
                }
                catch
                {
                    conn.Clone();
                    return null;
                }
            }
        }

    }
}
