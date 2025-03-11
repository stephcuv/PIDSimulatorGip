using System;
using System.Windows;

namespace PIDSimulatorGip.MVVM
{
    public class MvvmMessageBoxEventArgs : EventArgs
    {
        public MvvmMessageBoxEventArgs(string messageBoxText, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None, MessageBoxOptions options = MessageBoxOptions.None)
        {
            this.messageBoxText = messageBoxText;
            this.caption = caption;
            this.button = button;
            this.icon = icon;
            this.defaultResult = defaultResult;
            this.options = options;
        }

        private string messageBoxText;
        private string caption;
        private MessageBoxButton button;
        private MessageBoxImage icon;
        private MessageBoxResult defaultResult;
        private MessageBoxOptions options;

        public void Show(Window owner)
        {
            MessageBox.Show(owner, messageBoxText, caption, button, icon, defaultResult, options);
        }

        public void Show()
        {
            MessageBox.Show(messageBoxText, caption, button, icon, defaultResult, options);
        }
    }
}
