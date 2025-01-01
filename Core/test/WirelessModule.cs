using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionController.Core
{
    public enum WirelessModuleSendMode
    {
        Command = 0, BroadCast = 1, UniCast = 2
    }
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

        protected SerialPort Port { get; set; } = new SerialPort();

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
            //ポートが開いている場合は閉じる
            if (Port.IsOpen) Port.Close();
            try
            {
                Port = new SerialPort(portName, baudRate);
                Port.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal void Close()
        {
            //ポートが開いている場合は閉じる
            if (Port.IsOpen)
            {
                Port.Close();
                Port.Dispose();
            }
        }
        /// <summary>
        /// データを送信します
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected async Task Send(string data)
        {
            if (!Port.IsOpen || IsSending) return;
            await Task.Run(() =>
            {
                try
                {
                    IsSending = true;
                    Port.WriteLine(data);
                    IsSending = false;
                }
                catch (Exception) { throw; }
            });
        }
    }
}
