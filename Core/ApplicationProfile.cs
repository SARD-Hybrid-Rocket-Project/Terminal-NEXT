using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightController.Core
{
    public class ApplicationProfile
    {
        private SerialPort serialPort;

        public ApplicationProfile()
        {
            serialPort = new SerialPort();
        }
    }
}
