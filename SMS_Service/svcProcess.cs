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
    public  class SvcProcess
    {
        private schooldbEntities db;
        public SvcProcess()
        {
            db =new schooldbEntities();
        }
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
        public static bool RegisterINSqlServer(List<RegisteredTagList> registeredTagLists)
        {
            var connString =
                $"data source=localhost;initial catalog=infraBase ;user id=sa;password=Ss987654;MultipleActiveResultSets=True;";
            try
            {
                using (var connection =new SqlConnection(connString))
                {
                    connection.Open();
                    using (var trans = connection.BeginTransaction())
                    {
                        var command = connection.CreateCommand();
                        command.Transaction = trans;
                        try
                        {
                            foreach (var item in registeredTagLists)
                            {
                                command.CommandText =
                                    $"insert into tagRec(TagID,DateTimeRegister,MysqlID) Values('{item.Tagid}','{item.dateRegister.Date}','{item.ID}')";
                                command.ExecuteNonQuery();
                            }

                            trans.Commit();
                            return true;
                        }
                        catch
                        {
                            trans.Rollback();
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// خواندن ثبت جدید از بانک مای اس کیوال
        /// </summary>
        /// <returns></returns>
        public static List<RegisteredTagList> ReaderSQL()
        {
            string cs = @"server=localhost;port=3306;userid=fm;password=Ss987654;database=schooldb;SSL Mode = None";
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

        public static void SmsSender()
        {
            
        }

    }
}
