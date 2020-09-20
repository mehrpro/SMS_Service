using System;
using System.Collections.Generic;
using System.Threading;
using SmsIrRestful;

namespace SMS_Service
{

  public  class SendSMS
    {
        public int ID { get; set; }
        /// <summary>
        /// ارسال ورود دانش آموز 
        /// </summary>
        /// <param name="mobile">گیرنده پیامک</param>
        /// <param name="fullName">نام کامل</param>
        /// <param name="inDate">تاریخ وساعت</param>
        /// <returns></returns>
        public  bool SendInput(long mobile,string fullName,string inDate)
        {
            try
            {
                var token = new Token().GetToken("17413e1864890dd130c73e17", "Fm&)**)!@(*");

                var ultraFastSend = new UltraFastSend()
                {
                    Mobile = mobile,
                    TemplateId = 33365,
                    ParameterArray = new List<UltraFastParameters>()
                    {
                        new UltraFastParameters() {Parameter = "FullName" , ParameterValue = fullName,},
                        new UltraFastParameters() {Parameter = "InDate",ParameterValue = inDate}
                    }.ToArray()
                };
       
                var ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);
                if (ultraFastSendRespone.IsSuccessful)
                {
                    using (var dbx = new schooldbEntities())
                    {
                        var find = dbx.TagRecorders.Find(ID);
                        find.SMS = true;
                        var resultSave = dbx.SaveChanges();
                        Logger.WriteMessageSenderLog(Convert.ToBoolean(resultSave) ? $"Send Input {mobile}" : $"Not Send Input {mobile}");
                    }
                    return true;
                }
                return false;

            }
            catch (Exception e)
            {
                Logger.WriteErrorLog(e);
                return false;

            }

        }
        /// <summary>
        /// ارسال خروج دانش آموز
        /// </summary>
        /// <param name="mobile">گیرنده پیامک</param>
        /// <param name="fullName">نام کامل</param>
        /// <param name="inDate">تاریخ و ساعت</param>
        /// <returns></returns>
        public  bool SendOutput(long mobile, string fullName, string inDate)
        {
            try
            {
                var token = new Token().GetToken("17413e1864890dd130c73e17", "Fm&)**)!@(*");
                var ultraFastSend = new UltraFastSend()
                {
                    Mobile = mobile,
                    TemplateId = 33366,
                    ParameterArray = new List<UltraFastParameters>()
                    {
                        new UltraFastParameters() {Parameter = "FullName" , ParameterValue = fullName,},
                        new UltraFastParameters() {Parameter = "InDate",ParameterValue = inDate}
                    }.ToArray()

                };
                var ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);
                if (ultraFastSendRespone.IsSuccessful)
                {
                    using (var dbx = new schooldbEntities())
                    {
                        var find = dbx.TagRecorders.Find(ID);
                        find.SMS = true;
                        var resultSave = dbx.SaveChanges();
                        Logger.WriteMessageSenderLog(Convert.ToBoolean(resultSave) ? $"Send Input {mobile}" : $"Not Send Input {mobile}");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Logger.WriteErrorLog(e);
                return false;
            }

        }
    }
}


