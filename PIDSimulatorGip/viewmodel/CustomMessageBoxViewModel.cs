using PIDSimulatorGip.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIDSimulatorGip.viewmodel
{
    internal class CustomMessageBoxViewModel 
    {
        public RelayCommand ButtonCommand => new RelayCommand(execute => { DataProcessing(execute); });

        public string MessageboxText { get; set; }
        public string Btn1Text { get; set; }
        public string Btn2Text { get; set; }
        public string Btn3Text { get; set; }

        public string ChosenPort { get; set; }
        private void DataProcessing(object? parameter)
        {
            ChosenPort = parameter?.ToString();   
        }

        public void Show(string messageboxText, List<string> mogelijkeSerPort)
        {

            MessageboxText = messageboxText;
            Btn1Text = mogelijkeSerPort[0];
            Btn2Text = mogelijkeSerPort[1];
            
            if(mogelijkeSerPort.Count() == 3)
            {
                Btn3Text = mogelijkeSerPort[2];
            }
        }
    }
}
