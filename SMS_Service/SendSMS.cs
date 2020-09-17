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
        
        public bool Send(long mobile,string orderNumber,string OrderNumberValue,string orderDate,string orderDateValue)
        {
            var token = new Token().GetToken("17413e1864890dd130c73e17", "Fm&)**)!@(*");

            var ultraFastSend = new UltraFastSend()
            {
                Mobile = mobile,
                TemplateId = 33362,
                ParameterArray = new List<UltraFastParameters>()
                {
                    new UltraFastParameters() {Parameter = orderNumber , ParameterValue = OrderNumberValue,},
                    new UltraFastParameters() {Parameter = orderDate,ParameterValue = orderDateValue}

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


