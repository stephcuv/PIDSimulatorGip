using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIDSimulatorGip.model
{
    class ProcesBerekening
    {
        private double _kracht;
        private string? _orde;
        private bool _krachtBerState;
        private double _w;

        private byte _dodeTijd; //dodentijd enkel voor simulator
        private double T = 0;
        private double _tijdsconstante;
        private double _procesVerschil = 0;

        private List<double> _procesUitkomsten = new List<double>();
        private double _prevProcesUitkomst = 0;

        #region public variables for instance variables
        public double W
        {
            set
            {
                if ((value > 0) && value <= 100)
                {
                    _w = value;
                }
            }
        }
        public double Kracht
        {
            private get
            { return _kracht; }
            set
            {
                _kracht = value;
            }
        }
        public string Orde
        {
            private get
            {
                return _orde;
            }
            set
            {
                _orde = value;
            }
        }
        public string DodeTijd
        {
            set
            {
                switch (value)
                {
                    case "geen dodetijd":
                        _dodeTijd = 1;
                        break;
                    case "klein beetje dodetijd":
                        _dodeTijd = 2;
                        break;
                    case "wat dodetijd":
                        _dodeTijd = 3;
                        break;
                    case "gemiddeld dodetijd":
                        _dodeTijd = 4;
                        break;
                    case "meer dan gemiddeld dodetijd":
                        _dodeTijd = 5;
                        break;
                    case "veel dodetijd":
                        _dodeTijd = 6;
                        break;
                }
            }
        }
        public double Tijdsconstante

        {
            set { if (value > 0) { _tijdsconstante = value; } }
        }

        public double ProcesWaarde
        {
            get { return _prevProcesUitkomst; }
        }
        #endregion

        #region functions for adding or removing in _procesUitkomsten list
        private void add(double temp)
        {
            _procesUitkomsten.Add(temp);
        }

        private void RemoveCheck()
        {
            if (_procesUitkomsten.Count > _dodeTijd)
            {
                _procesUitkomsten.RemoveAt(0);
            }
        }
        #endregion

        public double Proces(double Y)
        {
            double X = 0;
            if (Y == 0)
            {
                _procesVerschil = 0;
                X = ProcesKrachtGelijk();
            }
            else if (Y < _w)
            {
                _procesVerschil = Y - _prevProcesUitkomst;
                X = ProcesKrachtStijgend();
            }
            else if (Y > _w)
            {
                _procesVerschil = _prevProcesUitkomst - Y;
                X = ProcesKrachtDalend();
            }
            double temp = Math.Round(X, 2);
            add(temp);
            RemoveCheck();
            return _procesUitkomsten.ElementAt(0);
        }

        #region functions proces kracht berekeningen
        private double ProcesKrachtGelijk()
        {
            double X;
            if (_krachtBerState == true)
            {
                X = ProcesKrachtStijgend();
            }
            else
            {
                X = ProcesKrachtDalend();
            }
            return X;
        }
        private double ProcesKrachtStijgend()
        {
            _krachtBerState = true;
            double _procesUitkomst = 0;
            double e = Math.Pow(Math.E, -T / (1 * Kracht));
            switch (Orde)
            {
                case "0orde":
                    _procesUitkomst = _prevProcesUitkomst + (_procesVerschil / Kracht);
                    T += _tijdsconstante;
                    _prevProcesUitkomst = _procesUitkomst;
                    break;
                case "1orde":
                    _procesUitkomst = _prevProcesUitkomst + (_procesVerschil * (1 - e));
                    T += _tijdsconstante;
                    _prevProcesUitkomst = _procesUitkomst;
                    break;
                case "2orde":
                    _procesUitkomst = _prevProcesUitkomst + (_procesVerschil * ((1 - e) * (1 - e)));
                    T += _tijdsconstante;
                    _prevProcesUitkomst = _procesUitkomst;
                    break;
            }
            return _procesUitkomst;
        }

        private double ProcesKrachtDalend()
        {
            _krachtBerState = false;
            double _procesUitkomst = 0;
            double e = Math.Pow(Math.E, -T / (1 * Kracht));
            switch (Orde)
            {
                case "0orde":
                    _procesUitkomst = _prevProcesUitkomst - (_procesVerschil / Kracht);
                    T += _tijdsconstante;
                    _prevProcesUitkomst = _procesUitkomst;
                    break;
                case "1orde":
                    _procesUitkomst = _prevProcesUitkomst - (_procesVerschil * (1 - e));
                    T += _tijdsconstante;
                    _prevProcesUitkomst = _procesUitkomst;
                    break;
                case "2orde":
                    _procesUitkomst = _prevProcesUitkomst - (_procesVerschil * ((1 - e) * (1 - e)));
                    T += _tijdsconstante;
                    _prevProcesUitkomst = _procesUitkomst;
                    break;
            }
            return _procesUitkomst;
        }
        #endregion 
    }
}
