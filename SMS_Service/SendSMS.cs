using System;
using System.Collections.Generic;
using SmsIrRestful;
namespace SMS_Service
{

    public class SendSMS
    {
        public static void SendInput(long mobile, string fullName, string inDate, int id)
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
                UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);
                if (Convert.ToBoolean(ultraFastSendRespone.IsSuccessful))
                {
                    using (var dbx = new schooldbEntities())
                    {
                        var find = dbx.TagRecorders.Find(id);
                        find.SMS = true;
                        var resultSave = dbx.SaveChanges();
                        Logger.WriteMessageSenderLog(Convert.ToBoolean(resultSave) ? $"Send Input {mobile}" : $"Not Send Input {mobile}");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteErrorLog(e);
            }
        }
        public static void SendOutput(long mobile, string fullName, string inDate, int id)
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
                UltraFastSendRespone ultraFastSendRespone = new UltraFast().Send(token, ultraFastSend);
                if (Convert.ToBoolean(ultraFastSendRespone.IsSuccessful))
                {
                    using (var dbx = new schooldbEntities())
                    {
                        var find = dbx.TagRecorders.Find(id);
                        find.SMS = true;
                        var resultSave = dbx.SaveChanges();
                        Logger.WriteMessageSenderLog(Convert.ToBoolean(resultSave) ? $"Send output {mobile}" : $"Not Send output {mobile}");
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


