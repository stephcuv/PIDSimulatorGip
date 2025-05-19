using PIDSimulatorGip.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace PIDSimulatorGip.viewmodel
{
    internal class CustomMessageBoxViewModel
    {
        public string Message { get; set; }
        public string UserInput { get; set; }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        private readonly Window _dialogWindow;

        public CustomMessageBoxViewModel(Window dialogWindow, string message)
        {
            _dialogWindow = dialogWindow;
            Message = message;

            OkCommand = new RelayCommand(ExecuteOk);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private void ExecuteOk(object parameter)
        {
            _dialogWindow.DialogResult = true;
            _dialogWindow.Close();
        }

        private void ExecuteCancel(object parameter)
        {
            _dialogWindow.DialogResult = false;
            _dialogWindow.Close();
        }
    }
}
