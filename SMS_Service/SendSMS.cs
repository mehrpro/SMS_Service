using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmsIrRestful;

namespace SMS_Service
{

  public class SendSMS
    {
        /// <summary>
        /// ارسال ورود دانش آموز 
        /// </summary>
        /// <param name="mobile">گیرنده پیامک</param>
        /// <param name="fullName">نام کامل</param>
        /// <param name="inDate">تاریخ وساعت</param>
        /// <returns></returns>
        public bool SendInput(long mobile,string fullName,string inDate)
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

            if (ultraFastSendRespone.IsSuccessful)
            {
                return true;
            }
            else
            {
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
        public bool SendOutput(long mobile, string fullName, string inDate)
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

            if (ultraFastSendRespone.IsSuccessful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}


