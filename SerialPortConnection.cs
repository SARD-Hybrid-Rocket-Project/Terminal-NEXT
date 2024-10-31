using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal_XP
{
    static class SerialPortConnection
    {
        public static string[]? GetSerialPortInformation()
        {
            PortSelectionDialog dialog = new PortSelectionDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string[] ports = new string[2]
                {
                    dialog.ComboBox_PortList.SelectedValue?.ToString() ?? string.Empty,
                    dialog.ComboBox_PortList.SelectedItem?.ToString() ?? string.Empty,
                };
                if (ports[1] != string.Empty)
                {
                    return ports; //シリアルポートを返す（予定）
                }
            }
            return null;
        }
    }
}
