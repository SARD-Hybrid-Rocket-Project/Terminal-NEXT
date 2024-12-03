using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionController.Core
{
    public class ApplicationProfile
    {
        private string[] _CommandResponce;

        public ApplicationProfile()
        {
            _CommandResponce = new string[0];
        }
    }
}
