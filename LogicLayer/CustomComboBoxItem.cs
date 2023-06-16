using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicLayer
{
    public class CustomComboBoxItem
    {
        public DateTime DisplayText { get; set; }
        public string DownloadValue { get; set; }

        public CustomComboBoxItem(DateTime displayText, string downloadValue)
        {
            DisplayText = displayText;
            DownloadValue = downloadValue;
        }
    }
}
