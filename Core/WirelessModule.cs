using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARD.Core
{
    public class WirelessModule
    {
        //プロパティ
        private SerialDataReceivedEventHandler? _dataReceivedEvent;
        public SerialDataReceivedEventHandler? DataReceivedEvent
        {
            get { return _dataReceivedEvent; }
            set
            {
                Port.DataReceived += value;
                _dataReceivedEvent = value;
            }
        }

        protected bool IsSending { get; set; } = false;

        public SerialPort Port { get; private set; } = new SerialPort();

        //コンストラクタ・デストラクタ
        internal WirelessModule()
        {
        }
        ~WirelessModule()
        {
            Close();
        }

        //メソッド

        /// <summary>
        /// portNameとbaudRateで指定されたポートを開くメソッドです。
        /// </summary>
        /// <param name="portName">シリアルポートを指定します</param>
        /// <param name="baudRate">シリアルポートのボーレートを指定します</param>
        internal void Open(string portName, int baudRate)
        {
            Port.PortName = portName;
            Port.BaudRate = baudRate;
            Port.Open();
        }
        internal void Close()
        {
            Port.Close();
            //Port.Dispose();
        }
        /// <summary>
        /// データを送信します
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected void Send(string data)
        {
            IsSending = true;
            Port.Write(data);
            IsSending = false;
        }
    }
}
