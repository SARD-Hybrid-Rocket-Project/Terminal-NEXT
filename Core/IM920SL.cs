using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        internal void Send(ushort targetNode, string message)
        {
            string userData = string.Empty;

            if(targetNode == 0xFFFF) userData = $"{message}\r\n";
            else if (targetNode == 0) userData = $"TXDA {message}\r\n";
            else if (targetNode >= 1) userData = $"TXDU {targetNode:X4},{message}\r\n";

            Debug.WriteLine(userData);
            Send(userData);
        }
    }
}
