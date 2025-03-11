using PIDSimulatorGip.MVVM;
using PIDSimulatorGip.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PIDSimulatorGip.view
{
    /// <summary>
    /// Interaction logic for NewMainWindow.xaml
    /// </summary>
    public partial class NewMainWindow : Window
    {
        public NewMainWindow()
        {
            MainSimulationViewModel vm = new MainSimulationViewModel();
            DataContext = vm;
            (this.DataContext as MainSimulationViewModel).MessageBoxRequest += new EventHandler<MvvmMessageBoxEventArgs>(MyView_MessageBoxRequest);
        }

        void MyView_MessageBoxRequest(object sender, MvvmMessageBoxEventArgs e)
        {
            e.Show();
        }
    }
}
