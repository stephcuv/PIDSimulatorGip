using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIDSimulatorGip.model
{
    class ProcesBerekening
    {
        private double _tijdconstante;
        public string? DodetijdChoice;

        private string? _orde;
        private bool _krachtBerState;
        private double _prevProcesUitkomst;

        private byte _dodeTijd; //dodentijd enkel voor simulator
        private double _t = 0;
        private double _tijdStap = 1;
        private double _procesVerschil = 0;
        private double _prevStuurwaarde;
        private double[] _procesUitkomsten;

        #region variabelen
        public double Tijdconstante
        {
             get
            { return _tijdconstante; }
            set
            {
                _tijdconstante = value;
            }
        }
        public string Orde
        {
            get
            {
                return _orde;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) { return; }
                string result = value.Substring(value.IndexOf(":") + 2);
                _orde = result;
            }
        }

        public int DodeTijdNumber
        {
            get { return _dodeTijd; }
        }
        public string DodeTijd
        {
            get { return DodetijdChoice; }
            set
            {
                if (string.IsNullOrEmpty(value)) { return; }
                DodetijdChoice = value.Substring(value.IndexOf(":") + 2);
                switch (DodetijdChoice)
                {
                    case "geen dodetijd":
                        _dodeTijd = 2;
                        break;
                    case "klein beetje dodetijd":
                        _dodeTijd = 3;
                        break;
                    case "wat dodetijd":
                        _dodeTijd = 4;
                        break;
                    case "gemiddeld dodetijd":
                        _dodeTijd = 5;
                        break;
                    case "meer dan gemiddeld dodetijd":
                        _dodeTijd = 6;
                        break;
                    case "veel dodetijd":
                        _dodeTijd = 7;
                        break;
                }
                _procesUitkomsten = new double[_dodeTijd];
            }
        }
        public double TijdStap

        {
            set { if (value > 0) { _tijdStap = value; } } get { return _tijdStap; }   
        }

        public double T

        {
            set { _t = 0; }
        }
        public double ProcesWaarde
        {
            get { return _procesUitkomsten[0]; }
        }
        public double ProcesVerschil
        {
            get { return _procesVerschil; }
        }
        private double _e;
        public double E
        {
            set { _e = value; }
            get { return _e; }
        }
        public double PrevProcesWaarde
        {
            get { return _prevProcesUitkomst; }
        }

        public bool StapsprongOn { get; set; }
        #endregion

        public double Proces(double Y)
        {
            double X = 0;
            try
            {
                if (Y == _prevStuurwaarde)
                {
                    if (!StapsprongOn)
                    {
                        _prevProcesUitkomst = _procesUitkomsten[0];
                    }

                    if (_krachtBerState)
                    {
                        X = BeginStijgen();
                    }
                    else
                    {
                        X = BeginDalen();
                    }
                }
                else if (Y > _prevStuurwaarde)
                {
                    _prevProcesUitkomst = _procesUitkomsten[0];
                    _procesVerschil = _procesUitkomsten[0] - Y;

                    X = BeginDalen();
                }
                else if (Y < _prevStuurwaarde)
                {
                    _prevProcesUitkomst = _procesUitkomsten[0];
                    _procesVerschil = Y - _procesUitkomsten[0];

                    X = BeginStijgen();
                }

                double value = Math.Round(X, 2);
                value = Math.Clamp(value, 0, 100);
                ArrayCheck(value);

                
                if(_t != 0 && !StapsprongOn) if (Math.Abs(_procesUitkomsten[0] - _procesUitkomsten[1]) < 0.01)
                {
                    _t = 0;
                } 
                
                
                _t += _tijdStap;
                _prevStuurwaarde = Y;

                return _procesUitkomsten[_dodeTijd - 2];
            }
            catch (Exception) 
            { 
                ArrayCheck(0); return 0; 
            }
        }


        #region functies proces kracht berekeningen
        private double BeginStijgen()
        {
            _krachtBerState = true;
            double _procesUitkomst = 0;
            double macht = -_t / Tijdconstante;
            double e = Math.Exp(macht);
            E = e;
            switch (Orde)
            {
                case "0orde":
                    _procesUitkomst = _prevProcesUitkomst + (_procesVerschil / 1);
                    break;
                case "1orde":
                    _procesUitkomst = _prevProcesUitkomst + (_procesVerschil * (1 - e));
                    break;
                case "2orde":
                    _procesUitkomst = _prevProcesUitkomst + (_procesVerschil * (1 - e) * (1 - e));
                    break;
            }
            return _procesUitkomst;
        }

        private double BeginDalen()
        {
            _krachtBerState = false;
            double _procesUitkomst = 0;
            double macht = -_t / Tijdconstante;
            double e = Math.Exp(macht);
            E = e;
            switch (Orde)
            {
                case "0orde":
                    _procesUitkomst = _prevProcesUitkomst - (_procesVerschil / 1);
                    break;
                case "1orde":
                    _procesUitkomst = _prevProcesUitkomst - (_procesVerschil * (1 - e));
                    break;
                case "2orde":
                    _procesUitkomst = _prevProcesUitkomst - (_procesVerschil * (1 - e) * (1 - e));
                    break;
            }
            return _procesUitkomst;
        }

        public void Reset()
        {
            foreach (double i in _procesUitkomsten)
            {
                ArrayCheck(0);
            }
            _procesVerschil = 0;
            _t = 0;
            StapsprongOn = false;
        }
        private void ArrayCheck(double waarde)
        {
            for (int i = _dodeTijd - 1; i > 0; i--)
            {
                _procesUitkomsten[i] = _procesUitkomsten[i - 1];
            }
            _procesUitkomsten[0] = waarde;
        }
        #endregion 
    }
}
