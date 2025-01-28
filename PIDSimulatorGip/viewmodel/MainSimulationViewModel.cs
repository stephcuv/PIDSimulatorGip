using PIDSimulatorGip.model;
using PIDSimulatorGip.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace PIDSimulatorGip.viewmodel
{
    internal class MainSimulationViewModel : ViewModelBase
    {

        private RegelaarInstellingen _RGLR;
        private ProcesBerekening _proces;
        private DispatcherTimer _timer;

        private bool _isRunning;

        private string _geselecteerdDodetijd;
        private string _geslecteerdProces;

        public MainSimulationViewModel()
        {
            _RGLR = new RegelaarInstellingen();
            _proces = new ProcesBerekening();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
        }

        public RelayCommand StartCommand => new RelayCommand(execute =>{ StartSimulation();},canExecute =>{return !_isRunning;});
        public RelayCommand StopCommand => new RelayCommand(execute =>{StopSimuation();},canExecute => {return _isRunning;});


        public string GesecteerdDodetijd { set { _proces.DodeTijd; }  }

        public double VSFP { get { return _RGLR.VSFP; } set { _RGLR.VSFP = value; OnPropertyChanged(); } }
        public double VSFI { get { return _RGLR.VSFI; } set { _RGLR.VSFI = value; OnPropertyChanged(); } }
        public double VSFD { get { return _RGLR.VSFD; } set { _RGLR.VSFD = value; OnPropertyChanged(); } }
        public double W { get { return _RGLR.W; } set { _RGLR.W = value; _proces.W = value; OnPropertyChanged(); } }
        public double TijdsConstante { set { _RGLR.Tijdsconstante = value; _proces.Tijdsconstante = value; OnPropertyChanged(); } get { return _RGLR.Tijdsconstante; } }
        public string Type { set { _RGLR.Type = value; OnPropertyChanged(); } }


        public double Kracht { set { _proces.Kracht = value; OnPropertyChanged(); } get { return _proces.Kracht; } }
        public string Dodetijd { set { _proces.DodeTijd = value; OnPropertyChanged(); } }
        public string Orde { set { _proces.Orde = value; OnPropertyChanged(); } }

       private void StartSimulation()
        {
            _isRunning = true;
            _timer.Start();
        }
        private void StopSimuation()
        {
            _isRunning = false;
            _timer.Stop();
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            double value;
            value = _RGLR.Berekening();
            value = _proces.Proces(value);
           _RGLR.X = value;
        }

    }
}
