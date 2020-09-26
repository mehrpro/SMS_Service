using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SMS_Service
{
    public class SMSSenderProcess
    {

        ///// <summary>
        ///// حذف موارد تکراری و تعیین وضعیت ثبت های جدید در بانک
        ///// </summary>
        //public static void DubleTagRemover()
        //{
        //    using (var dbr = new schooldbEntities())
        //    {
        //        while (dbr.TagRecorders.Count(x => x.SMS == false && x.enables && x.type) > 1)// تگ های جدید
        //        {
        //            var qry = dbr.TagRecorders.Where(x => x.SMS == false && x.enables).ToList();

        //            var select = dbr.TagRecorders.First(x => x.SMS == false && x.enables && x.type == null);// اولین تگ 
        //            var dtime = select.DateTimeRegister.AddSeconds(121);// بازه نهایی تگ
        //            foreach (var item in dbr.TagRecorders.Where(x => x.TagID == select.TagID).ToList())// یافتن تگ های تکراری تا دو دقیقه بعد
        //            {
        //                if (item.DateTimeRegister > select.DateTimeRegister && item.DateTimeRegister < dtime)
        //                {
        //                    item.enables = false;
        //                }
        //            }
        //            var emroz = dbr.TagRecorders.Where(x =>
        //            x.DateTimeRegister.Year == DateTime.Now.Year &&
        //            x.DateTimeRegister.Month == DateTime.Now.Month &&
        //            x.DateTimeRegister.Day == DateTime.Now.Day).ToList();
        //            var selectLast = emroz.LastOrDefault(x => x.TagID == select.TagID && x.enables && x.type != null).type;//یافتن آخرین وضعیت ارسالی
        //            if (selectLast == null)
        //            {
        //                select.type = true;// اولین ورود
        //            }
        //            else
        //            {
        //                select.type = !selectLast;
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// جستجوی  تگ های جدید و حذف ثبت های بی مورد
        /// </summary>
        /// <returns></returns>
        public static bool TagFinder()
        {
            using (var dbTag = new schooldbEntities())
            {
                using (var trans = dbTag.Database.BeginTransaction())
                {
                    try
                    {
                        var masterQuery = dbTag.TagRecorders.Where(x => x.enables).ToList();
                        var qry = masterQuery.Where(x => x.SMS == false).Select(x => x.TagID).ToList().RemoveDuplicates();
                        foreach (var item in qry)
                        {
                            var tag = dbTag.Tags.Any(x => x.TagID_HEX == item);// تگ قبلا ثبت شده است
                            if (tag)
                            {// قبلا ثبت شده
                                var tagID_FK = dbTag.Tags.First(x => x.TagID_HEX == item).ID;

                                var tagUsed = dbTag.StudentTAGs.Any(x => x.Tag.TagID_HEX == item);// تگ مورد استفاده دانش آموز است؟
                                if (!tagUsed)// تگ خام است
                                {
                                    var remove = dbTag.TagRecorders.Where(x => x.TagID == item).ToList();
                                    dbTag.TagRecorders.RemoveRange(remove);
                                    dbTag.SaveChanges();
                                }
                                else
                                {
                                    foreach (var noTagID_FK in masterQuery.Where(x => x.TagID == item))
                                    {
                                        noTagID_FK.TagID_FK = tagID_FK;
                                        dbTag.SaveChanges();
                                    }
                                }
                            }
                            else
                            {
                                dbTag.Tags.Add(new Tag() { TagID_HEX = item });// تگ را اضافه میکند
                                var remove = dbTag.TagRecorders.Where(x => x.TagID == item).ToList();
                                dbTag.TagRecorders.RemoveRange(remove);
                                dbTag.SaveChanges();
                                Logger.WriteMessageLog($"Save New TAG : {item}");
                            }
                        }
                        trans.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Logger.WriteErrorLog(e);
                        return false;
                    }

                }
            }

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
                        db.TagRecorders.Where(x =>
                        x.DateTimeRegister.Year == DateTime.Now.Year &&
                        x.DateTimeRegister.Month == DateTime.Now.Month &&
                        x.DateTimeRegister.Day == DateTime.Now.Day && x.SMS == false && x.enables).ToList(); // تمام ثبت های امروز
                    foreach (var item in qryDayRecorder) // جدول بندی تردد ها بر اساس هر تگ که در عین حال فقط و فقط متعلق به یک دانش آموز است
                    {
                        var student = db.StudentTAGs.Single(x => x.Tag.TagID_HEX == item.TagID);// شناسه تگ
                        try
                        {
                            if (item.type) //ورودی ها 
                            {
                                var finder = db.TagRecorders.Any(x => 
                                x.TagID_FK == item.TagID_FK &&
                                x.DateTimeRegister > item.DateTimeRegister.AddSeconds(-300) &&
                                x.SMS &&
                                x.type && 
                                x.enables);
                                if (finder)
                                {
                                    item.SMS = true;
                                    item.enables = false;
                                    db.SaveChanges();
                                }
                                else
                                    SendSMS.SendInput(Convert.ToInt64(student.Student.SMS), student.Student.FullName,
                                        item.DateTimeRegister.Convert_PersianCalender(), item.ID); //ارسال
                            }
                            else
                            {
                                var finder = db.TagRecorders.Any(x =>
                                x.TagID_FK == item.TagID_FK &&
                                x.DateTimeRegister > item.DateTimeRegister.AddSeconds(-300) &&
                                x.SMS &&
                                x.type == false &&
                                x.enables);
                                if (finder)
                                {
                                    item.SMS = true;
                                    item.enables = false;
                                    db.SaveChanges();
                                }
                                else
                                    SendSMS.SendOutput(Convert.ToInt64(student.Student.SMS), student.Student.FullName,
                                        item.DateTimeRegister.Convert_PersianCalender(), item.ID); ; //ارسال
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.WriteErrorLog(e);
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
}