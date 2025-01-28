using PIDSimulatorGip.model;
using PIDSimulatorGip.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PIDSimulatorGip.viewmodel
{
    internal class MainViewModel : ViewModelBase
    {
        private RegelaarInstellingen _RGLR;
        private ProcesBerekening _proces;

        public double VSFP { get { return _RGLR.VSFP; } set { _RGLR.VSFP = value; OnPropertyChanged(); } }
        public double VSFI { get { return _RGLR.VSFI; } set { _RGLR.VSFI = value; OnPropertyChanged(); } }
        public double VSFD { get { return _RGLR.VSFD; } set { _RGLR.VSFD = value; OnPropertyChanged(); } }
        public double W { get { return _RGLR.W; } set { _RGLR.W = value; _proces.W = value; OnPropertyChanged(); } }
        public double TijdsConstante { set { _RGLR.Tijdsconstante = value; _proces.Tijdsconstante = value; OnPropertyChanged(); } }
        public string Type { set { _RGLR.Type = value; OnPropertyChanged(); } }


        public double Kracht { set { _proces.Kracht = value; OnPropertyChanged(); } }
        public string Dodetijd { set { _proces.DodeTijd = value; OnPropertyChanged(); } }
        public string Orde { set { _proces.Orde = value; OnPropertyChanged(); } }

    }
}
