using PIDSimulatorGip.model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PIDSimulatorGip.viewmodel
{
    class PIDClassControlViewModel : INotifyPropertyChanged
    {
        private RegelaarInstellingen RGLR = new RegelaarInstellingen();
        private ProcesBerekening proces = new ProcesBerekening();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Regelaar
        {
            get => RGLR.Regelaar;
            set
            {
                if (RGLR.Regelaar != value)
                {
                    RGLR.Regelaar = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Geschakeld
        {
            get => RGLR.Geschakeld;
            set
            {
                if (RGLR.Geschakeld != value)
                {
                    RGLR.Geschakeld = value;
                    OnPropertyChanged();
                }
            }
        }

        public double W
        {
            get => RGLR.W;
            set
            {
                if (RGLR.W != value)
                {
                    RGLR.W = value;
                    proces.W = value;
                    OnPropertyChanged();
                }
            }
        }

        public double X
        {
            set
            {
                RGLR.X = value;
                OnPropertyChanged();

            }
        }

        public double VSFP
        {
            set
            {
                RGLR.VSFP = value;
                OnPropertyChanged();
            }
        }

        public double VSFI
        {
            set
            {
                RGLR.VSFI = value;
                OnPropertyChanged();
            }
        }

        public double VSFD
        {
            set
            {
                RGLR.VSFD = value;
                OnPropertyChanged();
            }
        }

        public double Dfilter
        {
            set
            {

                RGLR.Dfilter = value;
                OnPropertyChanged();

            }
        }

        public string DodeTijd
        {
            set
            {

                proces.DodeTijd = value;
                OnPropertyChanged();

            }
        }

        public string Orde
        {

            set
            {

                proces.Orde = value;
                OnPropertyChanged();

            }
        }

        public double TijdsConstante
        {
            set
            {

                RGLR.Tijdsconstante = value;
                proces.Tijdsconstante = value;
                OnPropertyChanged();

            }
        }
    }
}
