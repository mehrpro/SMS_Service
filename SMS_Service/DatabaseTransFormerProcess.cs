using System;
using System.Linq;

namespace SMS_Service
{
    public static class DatabaseTransFormerProcess
    {
        /// <summary>
        /// پردازش و انتقال بین دو سرور و ثبت و امحای تردد های ثبت شده
        /// </summary>
        public static void TransformDataBase()
        {
            using (var db = new schooldbEntities())
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var listForDisableinMySql = MySqlClass.ReaderSQL();// لیست تگ های ثبت نشده
                        foreach (var tagList in listForDisableinMySql)// ثبت در بانک اطلاعاتی اس کیوال
                        {
                            db.TagRecorders.Add(new TagRecorder()
                            {
                                TagID = tagList.Tag,
                                DateTimeRegister = tagList.dateRegister,
                                MysqlID = tagList.ID,
                                SMS = false,
                            });
                        }
                        db.SaveChanges();
                        // تغییر وضعیت تگ ها در مای اس کیوال
                        var resultMysql = MySqlClass.UpdateTagRecordList(listForDisableinMySql.Select(x => x.ID).ToList());
                        if (resultMysql)
                            trans.Commit();
                        else
                            trans.Rollback();
                    }
                    catch (Exception e)
                    {
                       Logger.WriteErrorLog(e);
                       trans.Rollback();
                    }
                }
            }
        }
    }
}
