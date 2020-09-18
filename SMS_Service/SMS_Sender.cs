using System;
using System.Collections.Generic;
using System.Linq;

namespace SMS_Service
{
    public class SMS_Sender
    {
        private static List<T> RemoveDuplicates<T>(List<T> items)
        {
            return (from s in items select s).Distinct().ToList();
        }
        private schooldbEntities db;
        public SMS_Sender(schooldbEntities db)
        {
            this.db = db;
        }
        /// <summary>
        /// جستجوی شناسه تگ
        /// </summary>
        /// <param name="tag">تگ</param>
        /// <returns></returns>
        private int FindTagID(string tag)
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
                            Enabled = false,
                            DeleteTAG = false,
                        };
                        db.Tags.Add(newTag);
                        var remove = db.TagRecorders.FirstOrDefault(x => x.TagID == tag);
                        db.TagRecorders.Remove(remove);
                        db.SaveChanges();
                        trans.Commit();
                        return newTag.ID;
                    }
                    catch
                    {
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
        private bool FindTagIN_StudentTAG(int id)
        {
            return db.StudentTAGs.Any(x => x.TagID_FK == id && x.Enabled);
        }
        /// <summary>
        /// شناسه دانش آموز  
        /// </summary>
        /// <param name="id">شناسه تگ</param>
        /// <returns></returns>
        private int FindStudentIDByTagID(int id)
        {
            return db.StudentTAGs.Single(x => x.TagID_FK == id && x.Enabled).Student_FK;
        }

        public void SMSSender()
        {
            //تمام ثبت های امروز
            var qryDayRecorder = db.TagRecorders.Where(x => x.DateTimeRegister.Date == DateTime.Today).ToList();// تمام ثبت های امروز
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
                    table.Add(new TableTime
                    {
                        ID = tagItem.ID,
                        EvenODD = counter++,
                        TagID = resultTAG_ID,
                        DateRecord = tagItem.DateTimeRegister.Date,
                        TimeRecord = tagItem.DateTimeRegister.TimeOfDay,
                        StudentID = studentID,
                        IsSendSMS = tagItem.SMS
                    });
                }

                foreach (var tableTime in table)//ارسال پیام تردد
                {
                    if (tableTime.EvenODD % 2 == 0)//ورودی ها 
                    {
                        if (tableTime.IsSendSMS == false)
                        {
                            //send smsIN
                        }
                    }
                    else if (tableTime.EvenODD % 2 != 0)//  خروجی ها
                    {
                        if (tableTime.IsSendSMS == false)
                        {
                            //send smsout
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
    public class TableTime
    {
        public int ID { get; set; }
        public int EvenODD { get; set; }
        public int TagID { get; set; }
        public DateTime DateRecord { get; set; }
        public TimeSpan TimeRecord { get; set; }
        public int StudentID { get; set; }
        public bool IsSendSMS { get; set; }


    }
}