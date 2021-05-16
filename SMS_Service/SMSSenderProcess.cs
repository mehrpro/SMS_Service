using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SMS_Service
{

    public class SMSSenderProcess
    {

        #region NewTags


        /// <summary>
        /// یافتن تگ های جدید و افزودن به بانک اطلاعاتی
        /// </summary>
        public static void NewTags()
        {
            using (var db = new schooldbEntities())
            {
                try
                {
                    var qry = db.TagRecorders.Where(x => x.enables && x.SMS == false).Select(x => x.TagID).ToList().RemoveDuplicates();
                    foreach (var item in qry)
                    {
                        var tag = db.Tags.FirstOrDefault(x => x.TagID_HEX == item);

                        if (tag == null)
                        {
                            db.Tags.Add(new Tag()
                            {
                                TagID_HEX = item,
                                CartView = HexToDecimal(item),

                            });
                            db.SaveChanges();
                            Logger.WriteMessageLog($"Save New TAG : {item} ViewLabel {HexToDecimal(item)}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteErrorLog(e, "NewTags");
                }
            }

        }
        #endregion



        #region HexToDecimal

        private static string HexToDecimal(string tagHex)
        {
            var hexValue = tagHex.Remove(0, 2).Remove(8);
            int decValue = int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            var str = decValue.ToString("0000000000");
            return str;
        }

        #endregion


        #region UpdateTagID

        /// <summary>
        /// حذف تگ های خام و بروزرسانی شناسه تگ ها
        /// </summary>
        public static void UpdateTagID()
        {
            using (var db = new schooldbEntities())
            {
                var listRemove = new List<TagRecorder>();
                try
                {
                    var qry = db.TagRecorders.Where(x => x.enables && x.SMS == false && x.TagID_FK == null).ToList();
                    foreach (var item in qry)
                    {
                        var result = db.StudentTAGs.FirstOrDefault(x => x.Tag.TagID_HEX == item.TagID && x.Enabled);
                        if (result == null)
                        {
                            listRemove.Add(item);
                        }
                        else
                        {
                            item.TagID_FK = result.TagID_FK;
                            item.StudentID_FK = result.Student_FK;
                        }
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Logger.WriteErrorLog(e, "UpdateTagID");

                }
                finally
                {
                    try
                    {
                        db.TagRecorders.RemoveRange(listRemove);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Logger.WriteErrorLog(e, "UpdateTagID Finally");
                    }
                }
            }
        }

        #endregion


        #region SMSSender

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
                    var qryDayRecorder = db.TagRecorders.Where(x => x.SMS == false && x.enables && x.StudentID_FK > 0 && x.TagID_FK > 0).ToList(); // تمام ثبت های امروز
                    foreach (var item in qryDayRecorder) // جدول بندی تردد ها بر اساس هر تگ که در عین حال فقط و فقط متعلق به یک دانش آموز است
                    {
                        var student = db.Students.Find(item.StudentID_FK.Value);
                        try
                        {
                            if (item.type) //ورودی ها 
                            {
                                var dat = item.DateTimeRegister.AddSeconds(-120);
                                var finder = db.TagRecorders.Any(x =>
                                x.TagID_FK == item.TagID_FK &&
                                x.DateTimeRegister > dat &&
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
                                    SendSMS.SendInput(Convert.ToInt64(student.SMS), student.FullName,
                                        item.DateTimeRegister.Convert_PersianCalender(), item.ID); //ارسال
                            }
                            else
                            {
                                var dat = item.DateTimeRegister.AddSeconds(-120);
                                var finder = db.TagRecorders.Any(x =>
                                x.TagID_FK == item.TagID_FK &&
                                x.DateTimeRegister > dat &&
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
                                    SendSMS.SendOutput(Convert.ToInt64(student.SMS), student.FullName,
                                        item.DateTimeRegister.Convert_PersianCalender(), item.ID); ; //ارسال
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.WriteErrorLog(e, "ForEach SMSSender");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteErrorLog(e, "SMSSender");
                }
            }
        }

        #endregion


        #region BirthDay
        public static void SMSSenderBirthDay()
        {

            using (var db = new schooldbEntities())
            {
                try
                {
                    //تمام ثبت های امروز
                    var qryDayRecorder = db.TagRecorders.
                        Where(x => x.SMS == false && x.enables && x.StudentID_FK > 0 && x.TagID_FK > 0).ToList(); // تمام ثبت های امروز
                    foreach (var item in qryDayRecorder) // جدول بندی تردد ها بر اساس هر تگ که در عین حال فقط و فقط متعلق به یک دانش آموز است
                    {
                        var student = db.Students.Find(item.StudentID_FK.Value);
                        try
                        {
                            if (item.type) //ورودی ها 
                            {
                                var dat = item.DateTimeRegister.AddSeconds(-120);
                                var finder = db.TagRecorders.Any(x =>
                                x.TagID_FK == item.TagID_FK &&
                                x.DateTimeRegister > dat &&
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
                                    SendSMS.SendInput(Convert.ToInt64(student.SMS), student.FullName,
                                        item.DateTimeRegister.Convert_PersianCalender(), item.ID); //ارسال
                            }
                            else
                            {
                                var dat = item.DateTimeRegister.AddSeconds(-120);
                                var finder = db.TagRecorders.Any(x =>
                                x.TagID_FK == item.TagID_FK &&
                                x.DateTimeRegister > dat &&
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
                                    SendSMS.SendOutput(Convert.ToInt64(student.SMS), student.FullName,
                                        item.DateTimeRegister.Convert_PersianCalender(), item.ID); ; //ارسال
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.WriteErrorLog(e, "ForEach SMSSender");
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteErrorLog(e, "SMSSender");
                }
            }
        }

        #endregion



    }
}