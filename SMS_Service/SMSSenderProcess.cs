using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SMS_Service
{
    public class SMSSenderProcess
    {
        private static void DubleTagRemover()
        {
            using (var dbr = new schooldbEntities())
            {
                var qry = dbr.TagRecorders.Where(x => x.SMS == false).Select(x => x.TagID).ToList().RemoveDuplicates();
                while (dbr.TagRecorders.Count(x => x.SMS == false) > 1)// تگ های جدید
                {
                    var select = dbr.TagRecorders.First(x => x.SMS == false && x.enables && x.type == null);// اولین تگ 
                    var dtime = select.DateTimeRegister.AddSeconds(121);// بازه نهایی تگ
                    foreach (var item in dbr.TagRecorders.Where(x=>x.TagID == select.TagID).ToList())// یافتن تگ های تکراری تا دو دقیقه بعد
                    {
                        if(item.DateTimeRegister > select.DateTimeRegister && item.DateTimeRegister < dtime)
                        {
                            item.enables = false;
                        }
                    }
                    var selectLast = dbr.TagRecorders.LastOrDefault(x => x.TagID == select.TagID && x.enables && x.type != null).type;
                    if (select == null)
                    {
                        select.type = true;
                    }
                    else
                    {
                        select.type = !selectLast;
                    }
                }
            }
        }
        /// <summary>
        /// جستجوی شناسه تگ
        /// </summary>
        /// <param name="tag">تگ</param>
        /// <returns></returns>
        private static int FindTagID(string tag, schooldbEntities db)
        {
            var qry = db.Tags.FirstOrDefault(x => x.TagID_HEX == tag);
            if (qry == null)
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
            return qry.ID;
        }
        /// <summary>
        /// تگ فعال است؟
        /// </summary>
        /// <param name="id">شناسه تگ</param>
        /// <returns></returns>
        private static bool FindTagIN_StudentTAG(int id, schooldbEntities db)
        {
            return db.StudentTAGs.Any(x => x.TagID_FK == id && x.Enabled);
        }
        /// <summary>
        /// شناسه دانش آموز  
        /// </summary>
        /// <param name="id">شناسه تگ</param>
        /// <returns></returns>
        private static int FindStudentIDByTagID(int id, schooldbEntities db)
        {
            return db.StudentTAGs.Single(x => x.TagID_FK == id && x.Enabled).Student_FK;
        }
        /// <summary>
        /// پردازش و ارسال پیامک تردد
        /// </summary>
        public static void SMSSender()
        {
            using (var db = new schooldbEntities())
            {
                try
                {
                    //تمام ثبت های امروز
                    var qryDayRecorder =
                        db.TagRecorders.Where(x => x.DateTimeRegister.Year == DateTime.Now.Year && x.DateTimeRegister.Month == DateTime.Now.Month).ToList(); // تمام ثبت های امروز
                    var tagList = qryDayRecorder.Select(x => x.TagID).ToList().RemoveDuplicates();  // لیست تگ های امروز بدون تکرار
                    foreach (var item in tagList.ToList()) // جدول بندی تردد ها بر اساس هر تگ که در عین حال فقط و فقط متعلق به یک دانش آموز است
                    {
                        var resultTAG_ID = FindTagID(item, db); // retrun ID
                        if (FindTagIN_StudentTAG(resultTAG_ID, db))          // آیا این تگ  قابل استفاده است
                        {
                            var studentID = FindStudentIDByTagID(resultTAG_ID, db); // StudentID
                            var table = new List<TableTime>();
                            var counter = 0;
                            foreach (var tagItem in qryDayRecorder.Where(x => x.TagID == item).OrderBy(x => x.MysqlID)
                                .ToList()) //ساخت جدول تردد
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
                                    mobile = sms,
                                });
                            }
                            try
                            {
                                foreach (var tableTime in table) //ارسال پیام تردد
                                {
                                    if (tableTime.EvenODD % 2 == 0) //ورودی ها 
                                    {
                                        if (tableTime.IsSendSMS == false)
                                            SendSMS.SendInput(tableTime.mobile, tableTime.FullName,
                                                    tableTime.DateRecord.Convert_PersianCalender(), tableTime.ID); //ارسال
                                    }
                                    else
                                    {
                                        if (tableTime.IsSendSMS == false)
                                            SendSMS.SendOutput(tableTime.mobile, tableTime.FullName,
                                                 tableTime.DateRecord.Convert_PersianCalender(), tableTime.ID); //ارسال
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.WriteErrorLog(e);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteErrorLog(e);
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