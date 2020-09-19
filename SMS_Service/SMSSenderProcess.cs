using System;
using System.Collections.Generic;
using System.Linq;

namespace SMS_Service
{
    public  class SMSSenderProcess
    {
        private static List<T> RemoveDuplicates<T>(List<T> items)
        {
            return (from s in items select s).Distinct().ToList();
        }
        public schooldbEntities db;
        public SMSSenderProcess()
        {
            db = new schooldbEntities();
        }
        /// <summary>
        /// جستجوی شناسه تگ
        /// </summary>
        /// <param name="tag">تگ</param>
        /// <returns></returns>
        private  int FindTagID(string tag)
        {
            var qry = db.Tags.FirstOrDefault(x => x.TagID_HEX == tag);
            if (qry == null)
            {
                using (var db = new schooldbEntities())
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var newTag = new Tag
                            {
                                TagID_HEX = tag,
                            };
                            db.Tags.Add(newTag);
                            db.SaveChanges();
                            var remove = db.TagRecorders.Where(x => x.TagID == tag).ToList();
                            db.TagRecorders.RemoveRange(remove);
                            db.SaveChanges();
                            trans.Commit();
                            Logger.WriteMessageLog($"Save New TAG : {newTag.TagID_HEX}");
                            return newTag.ID;
                        }
                        catch
                        {
                            Logger.WriteMessageLog($"Error In FindTagID ");
                            return 0;
                        }
                    }
                }
            }
            return qry.ID;
        }
        /// <summary>
        /// تگ فعال است؟
        /// </summary>
        /// <param name="id">شناسه تگ</param>
        /// <returns></returns>
        private  bool FindTagIN_StudentTAG(int id)
        {
            return db.StudentTAGs.Any(x => x.TagID_FK == id && x.Enabled);
        }
        /// <summary>
        /// شناسه دانش آموز  
        /// </summary>
        /// <param name="id">شناسه تگ</param>
        /// <returns></returns>
        private  int FindStudentIDByTagID(int id)
        {
            return db.StudentTAGs.Single(x => x.TagID_FK == id && x.Enabled).Student_FK;
        }
        /// <summary>
        /// پردازش و ارسال پیامک تردد
        /// </summary>
        public void SMSSender()
        {
            var todays = DateTime.Now.Date;
            //تمام ثبت های امروز
            var qryDayRecorder = db.TagRecorders.Where(x => x.DateTimeRegister.Date == todays).ToList();// تمام ثبت های امروز
            var TagList = RemoveDuplicates(qryDayRecorder.Select(x => x.TagID).ToList()); // لیست تگ های امروز بدون تکرار
            foreach (var item in TagList) // جدول بندی تردد ها بر اساس هر تگ که در عین حال فقط و فقط متعلق به یک دانش آموز است
            {
                var resultTAG_ID = FindTagID(item); // retrun ID
                if (FindTagIN_StudentTAG(resultTAG_ID) == false) return; // آیا این تگ  قابل استفاده است
                var studentID = FindStudentIDByTagID(resultTAG_ID); // StudentID
                var table = new List<TableTime>();
                var counter = 0;
                foreach (var tagItem in qryDayRecorder.Where(x => x.TagID == item).OrderBy(x => x.MysqlID).ToList())//ساخت جدول تردد
                {
                    var stu = db.Students.Find(studentID);
                    var sms = Convert.ToInt64(stu.SMS);
                    table.Add(new TableTime
                    {
                        ID = tagItem.ID,
                        EvenODD = counter++,
                        TagID = resultTAG_ID,
                        DateRecord = tagItem.DateTimeRegister,
                        MySQLID = tagItem.MysqlID,
                        IsSendSMS = tagItem.SMS,
                        FullName = stu.FullName,
                        mobile = Convert.ToInt64(sms.ToString("00000000000"))
                    });
                }

                foreach (var tableTime in table)//ارسال پیام تردد
                {
                    if (tableTime.EvenODD % 2 == 0)//ورودی ها 
                    {
                        if (tableTime.IsSendSMS == false)
                        {
                            var result = new SendSMS().SendInput(tableTime.mobile, tableTime.FullName,
                                tableTime.DateRecord.Convert_PersianCalender());//ارسال
                            if (result)
                            {
                                var find = db.TagRecorders.Find(tableTime.ID);
                                find.SMS = true;
                                var resultSave = db.SaveChanges();
                                if (Convert.ToBoolean(resultSave))
                                {
                                    var resultMySQL = MySqlClass.UpdateTagRecord(tableTime.MySQLID);
                                    if (!resultMySQL)
                                    {
                                        //Log Error Message 
                                        Logger.WriteErrorSaveToDatabase($"No Update ID : {tableTime.MySQLID} in Mysql.schooldb.tagRec");
                                    }
                                }
                            }
                        }
                    }
                    else if (tableTime.EvenODD % 2 != 0)//  خروجی ها
                    {
                        if (tableTime.IsSendSMS == false)
                        {
                            var result = new SendSMS().SendOutput(tableTime.mobile, tableTime.FullName,
                                tableTime.DateRecord.Convert_PersianCalender());//ارسال
                            if (result)
                            {
                                var find = db.TagRecorders.Find(tableTime.ID);
                                find.SMS = true;
                                var resultSave = db.SaveChanges();
                                if (Convert.ToBoolean(resultSave))
                                {
                                    var resultMySQL = MySqlClass.UpdateTagRecord(tableTime.MySQLID);
                                    if (!resultMySQL)
                                    {
                                        //Log Error Message 
                                        Logger.WriteErrorSaveToDatabase($"No Update ID : {tableTime.MySQLID} in Mysql.schooldb.tagRec");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

        }
    }
    /// <summary>
    /// جدول تردد
    /// </summary>
    internal class TableTime
    {
        public int ID { get; set; }
        public int EvenODD { get; set; }
        public int TagID { get; set; }
        public DateTime DateRecord { get; set; }
        public int MySQLID { get; set; }
        public string FullName { get; set; }
        public bool IsSendSMS { get; set; }
        public long mobile { get; set; }


    }
}