using System;

namespace SMS_Service
{
    /// <summary>
    /// لیست تگ
    /// </summary>
    public class View_TagList
    {
        public int ID { get; set; }
        public string Tag { get; set; }
        public DateTime dateRegister { get; set; }
        public byte TypeImport { get; set; }
    }
}