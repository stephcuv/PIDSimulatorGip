using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIDSimulatorGip.model
{
    class PIDBerekeningen
    {
        private double _x;
        private double _w;

        private double _vsfP = 1;

        private double _vsfI = 1;
        private double _prevIWaarde;

        private double _vsfD = 1;
        private double _prevDFout;
        private double _prevDFiltered;

        private double _dFilter = 1;
        private double _t;
        private double _tijdsconstante;


        public double DFilter
        {
            get { return _dFilter; }
            set { _dFilter = value; }
        }
        public double Tijdsconstante
        {
            get { return _tijdsconstante; }
            set { _tijdsconstante = value; }
        }
        internal double X //gemeten waarde
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }
        }

        internal double W //wenswaarde 
        {
            get
            {
                return _w;
            }

            set
            {
                _w = value;
            }
        }

        internal double VSFP //versteringsfactor voor de P - regelaar
        {
            get
            {
                return _vsfP;
            }

            set
            {
                if (value >= 0 && _vsfP != value)
                {
                    _vsfP = value;
                }
            }
        }

        internal double VSFI //versterkingsfactor voor de I - regelaar
        {
            get
            {
                return _vsfI;
            }

            set
            {
                if (value >= 0 && _vsfI != value)
                {
                    _vsfI = value;
                }
            }
        }

        internal double VSFD //versterkingsfactor voor de D - regelaar
        {
            get
            {
                return _vsfD;
            }
            set
            {
                if (value > 0 && _vsfD != value)
                {
                    _vsfD = value;
                }
            }
        }

        //berekeningen voor de regelaars 3 meegegeven waardes besluiten welke bewerking moet gedaan worden. 
        internal double PRegelaar(bool Geschakeld, bool First, double Waarde)
        {
            double berekening;
            //1ste berekening zal gedaan worden als dit de eerste berekening is van de serieel geschakelde regelaar
            //of 1ste berekening zal gedaan worden als deze parallel is geschakeled.

            if ((Geschakeld && First) || !Geschakeld)
            {
                berekening = _vsfP * Fout();
            }
            else
            {
                berekening = _vsfP * Waarde;
            }
            berekening = Math.Round(berekening, 4);
            return berekening;
        }

        //1ste berekening zal gedaan worden als dit de eerste berekening is van de serieel geschakelde regelaar
        //of 1ste berekening zal gedaan worden als deze parallel is geschakeled.
        internal double IRegelaar(bool Geschakeld, bool First, double Waarde)
        {
            double berekening;


            if ((Geschakeld && First) || !Geschakeld)
            {
                berekening = _prevIWaarde + VSFI * Fout() * _tijdsconstante;
            }
            else
            {
                berekening = _prevIWaarde + VSFI * Waarde * _tijdsconstante;
            }
            berekening = Math.Round(berekening, 4);
            _prevIWaarde = berekening;
            return berekening;
        }

        //1ste berekening zal gedaan worden als dit de eerste berekening is van de serieel geschakelde regelaar
        //of 1ste berekening zal gedaan worden als deze parallel is geschakeled.
        internal double DRegelaar(bool Geschakeld, bool First, double waarde)
        {
            double _DnonFilter = 0;
            double outcome;
            if (_t > 0)
            {

                if ((Geschakeld && First) || !Geschakeld)
                {
                    _DnonFilter = _vsfD * (Fout() - _prevDFout) / _t;
                    _prevDFout = Fout();
                }
                else
                {
                    _DnonFilter = _vsfD * (waarde - _prevDFout) / _t;
                    _prevDFout = waarde;
                }
            }
            else
            {
                _DnonFilter = 0;
            }

            outcome = _dFilter * _DnonFilter + (1 - _dFilter) * _prevDFiltered;
            outcome = Math.Round(outcome, 4);
            _prevDFiltered = outcome;
            return outcome;
        }

        //berekend de fout 
        private double Fout()
        {
            return _w - _x;
        }
        //zal kijken hoeveel tijd er is verstreken. 
        public double EllapsedTime()
        {
            _t += _tijdsconstante;
            return _t;
        }
    }
}

