using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIDSimulatorGip.model
{
    class RegelaarInstellingen
    {
        private PIDBerekeningen PIDBerekeningen = new PIDBerekeningen();

        private string? _regelaar;
        private string _geschakeld = "Parallel";

        private double _y;
        private double _prevY;

        private double _dFilter;
        private double _tijdsconstante;

        private double _pWaarde;
        private double _iWaarde;
        private double _dWaarde;

        #region public variabelen

        public double PWaarde
        {
            get { return _pWaarde; }
        }
        public double IWaarde
        {
            get { return _iWaarde; }
        }
        public double DWaarde
        {
            get { return _dWaarde; }
        }
        public double Tijdsconstante
        {
            get
            {
                return _tijdsconstante;
            }
            set
            {
                if (value > 0)
                {
                    _tijdsconstante = value;
                }
            }
        }

        public string Regelaar
        {
            get
            {
                return _regelaar;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _regelaar = value;
                }
            }
        }
        //public variabele om de instantievariabele te kunnen aanpassen.
        //kan serieel of parallel zijn. 
        public string Geschakeld
        {
            get
            {
                return _geschakeld;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _geschakeld = value;
                }
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }
            private set
            {
                _y = value;
            }
        }
        public double X
        {
            get
            {
                return PIDBerekeningen.X;
            }
            set
            {
                PIDBerekeningen.X = value;
            }
        }
        public double W
        {
            get
            {
                return PIDBerekeningen.W;
            }
            set
            {
                if ((value > 0) && value <= 100)
                {
                    PIDBerekeningen.W = value;
                }
            }
        }

        public double Dfilter
        {
            get { return _dFilter; }
            set { if (value >= 0 && value < 1) { _dFilter = value; } }
        }

        public double VSFP
        {
            get
            {
                return PIDBerekeningen.VSFP;
            }
            set
            {
                if (value > 0)
                {
                    PIDBerekeningen.VSFP = value;
                }
            }
        }
        public double VSFI
        {
            get
            {
                return PIDBerekeningen.VSFI;
            }
            set
            {
                if (value > 0)
                {
                    PIDBerekeningen.VSFI = value;
                }
            }
        }
        public double VSFD
        {
            get
            {
                return PIDBerekeningen.VSFD;
            }
            set
            {
                if (value > 0)
                {
                    PIDBerekeningen.VSFD = value;
                }
            }
        }
        #endregion
        private double Parallel()
        {
            double outcome = 0;
            switch (_regelaar)
            {
                case "P":
                    outcome = PIDBerekeningen.PRegelaar(false, false, 0);
                    break;
                case "I":
                    PIDBerekeningen.Tijdsconstante = Tijdsconstante;
                    outcome = PIDBerekeningen.IRegelaar(false, false, 0);
                    PIDBerekeningen.EllapsedTime();
                    break;

                case "PI":
                    PIDBerekeningen.Tijdsconstante = Tijdsconstante;
                    _pWaarde = PIDBerekeningen.PRegelaar(false, false, 0);
                    _iWaarde = PIDBerekeningen.IRegelaar(false, false, 0);
                    outcome = _pWaarde + _iWaarde;
                    PIDBerekeningen.EllapsedTime();
                    break;
                case "PD":
                    PIDBerekeningen.Tijdsconstante = Tijdsconstante;
                    PIDBerekeningen.DFilter = Dfilter;
                    _pWaarde = PIDBerekeningen.PRegelaar(false, false, 0);
                    _iWaarde = PIDBerekeningen.DRegelaar(false, false, 0);
                    outcome = _pWaarde + _iWaarde;
                    PIDBerekeningen.EllapsedTime();
                    break;
                case "PID":
                    PIDBerekeningen.Tijdsconstante = Tijdsconstante;
                    PIDBerekeningen.DFilter = Dfilter;

                    _pWaarde = PIDBerekeningen.PRegelaar(false, false, 0);
                    _iWaarde = PIDBerekeningen.IRegelaar(false, false, 0);
                    _dWaarde = PIDBerekeningen.DRegelaar(false, false, 0);
                    outcome = _pWaarde + _iWaarde + _dWaarde;
                    PIDBerekeningen.EllapsedTime();
                    break;
            }
            return outcome;
        }

        private double Serial()
        {
            double outcome = 0;
            switch (_regelaar)
            {
                case "PI":
                    PIDBerekeningen.Tijdsconstante = Tijdsconstante;
                    _pWaarde = PIDBerekeningen.PRegelaar(true, true, 0);
                    _iWaarde = PIDBerekeningen.IRegelaar(true, false, _pWaarde);
                    PIDBerekeningen.EllapsedTime();
                    outcome = _pWaarde + _iWaarde;
                    break;
                case "PD":
                    PIDBerekeningen.Tijdsconstante = Tijdsconstante;
                    PIDBerekeningen.DFilter = Dfilter;
                    _pWaarde = PIDBerekeningen.PRegelaar(true, true, 0);
                    _dWaarde = PIDBerekeningen.DRegelaar(true, false, _pWaarde);
                    outcome = _pWaarde + _dWaarde;
                    PIDBerekeningen.EllapsedTime();
                    break;
                case "PID":
                    PIDBerekeningen.Tijdsconstante = Tijdsconstante;
                    PIDBerekeningen.DFilter = Dfilter;

                    _pWaarde = PIDBerekeningen.PRegelaar(true, true, 0);
                    _iWaarde = PIDBerekeningen.IRegelaar(true, false, _pWaarde);
                    _dWaarde = PIDBerekeningen.DRegelaar(true, false, _pWaarde);
                    outcome = _pWaarde + _iWaarde + _dWaarde;
                    PIDBerekeningen.EllapsedTime();
                    break;
            }
            return outcome;
        }

        private void RegelaarBerekening()
        {
            double _endOutcome = 0;
            switch (_geschakeld)
            {
                case "Parallel":

                    _endOutcome = Parallel();
                    break;
                case "Serieel":

                    _endOutcome = Serial();

                    break;
            }
            Y = _endOutcome;
        }

        public double Berekening()
        {
            RegelaarBerekening();
            if (Y == 0)
            {
                return _prevY;
            }
            else
            {
                _prevY = Y;
                return Y;
            }

        }
    }
}
