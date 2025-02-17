using PIDSimulatorGip.viewmodel;
using System.Windows;


namespace PIDSimulatorGip.view
{
    /// <summary>
    /// Interaction logic for MainSimulationWindow.xaml
    /// </summary>
    public partial class MainSimulationWindow : Window
    {
        public MainSimulationWindow()
        {
            InitializeComponent();
            MainSimulationViewModel vm = new MainSimulationViewModel();
            DataContext = vm;    
        }
    }

}

