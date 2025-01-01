using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MissionController.Core
{
    public class IM920SL : WirelessModule
    {
        public string UnicastTargetID { get; set; } = "0001";
        //コンストラクタ・デストラクタ
        internal IM920SL() : base()
        {
        }
        ~IM920SL()
        {
            Close();
        }
        //メソッド
        internal async void Send(WirelessModuleSendMode mode, string message)
        {
            string userData = string.Empty;
            switch (mode)
            {
                case WirelessModuleSendMode.Command:
                    break;
                case WirelessModuleSendMode.BroadCast:
                    userData = $"TXDA{message}";
                    break;
                case WirelessModuleSendMode.UniCast:
                    userData = $"TXDU{UnicastTargetID},{message}";
                    break;
                default:
                    break;
            }
            await Send(userData);
        }
    }
}
