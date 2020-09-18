using System;
using System.Collections.Generic;
using System.Linq;

namespace SMS_Service
{
    /// <summary>
    /// عملیات ثبت تگ
    /// </summary>
    public class TAG_Regsters
    {
        private schooldbEntities db;
        //private List<TagRecorder> _qryNew;
        public TAG_Regsters()
        {
            db = new schooldbEntities();
        }
        /// <summary>
        /// ثبت تگ های جدید در بانک اطلاعاتی
        /// </summary>
        /// <returns>تعداد تگ های ثبت شده</returns>
        public int RegisterTAG()
        {
            int counter = 0;
            var newListTAGs = AllTag(); // new TAG List
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var newListTaG in newListTAGs)
                    {
                        var tag = new Tag()
                        {
                            DeleteTAG = false,
                            Enabled = false,
                            TagID_HEX = newListTaG.Trim(),
                        };
                        db.Tags.Add(tag);
                        ++counter;
                    }
                    foreach (var taG in newListTAGs)
                    {
                        // حذف تگ های جدید
                        var listRemove = db.TagRecorders.Where(x => x.TagID == taG).ToList();
                        db.TagRecorders.RemoveRange(listRemove);
                    }
                    db.SaveChanges();
                    trans.Commit();
                    return counter;
                }
                catch
                {
                    return 0;
                }
            }

        }
        /// <summary>
        /// لیست تمام تگ های ثبت نشده
        /// </summary>
        /// <returns></returns>
        private List<string> AllTag()
        {
            var resultRemove = db.Tags.Select(x => x.TagID_HEX).ToList();
            var qry = db.TagRecorders.Where(x => x.SMS == false || x.SMS == null).Select(x => x.TagID).ToList();
            var result = RemoveDuplicates(qry);
            resultRemove.RemoveAll(item => result.Contains(item));
            return result;
        }
        private static List<T> RemoveDuplicates<T>(List<T> items)
        {
            return (from s in items select s).Distinct().ToList();
        }
    }
}