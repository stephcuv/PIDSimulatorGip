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

        private byte _dodeTijd; //dodentijd enkel voor simulator
        private double T = 0;
        private double _tijdsconstante;
        private double _procesVerschil = 0;

        public string? DodetijdChoice;

        private double[] _procesUitkomsten;

        #region variabelen

        public double Kracht
        {
             get
            { return _kracht; }
            set
            {
                _kracht = value;
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
            get {return DodetijdChoice;}
            set
            {
                if(string.IsNullOrEmpty(value)){return;}
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
        public double Tijdsconstante

        {
            set { if (value >= 0) { _tijdsconstante = value; } }
        }

        public double ProcesWaarde
        {
            get { return _procesUitkomsten[0]; }
        }
        #endregion

        #region 
        private void ArrayCheck(double waarde)
        {
            for (int i = _dodeTijd - 1; i > 0; i--)
            {
                _procesUitkomsten[i] = _procesUitkomsten[i - 1];
            }
            _procesUitkomsten[0] = waarde;
        }

        #endregion

        public double Proces(double Y)
        {
            double X = 0;
            try
            {


                if (Y == _procesUitkomsten[1])
                {
                    _procesVerschil = 0;
                    X = ProcesKrachtGelijk();
                }
                else if (Y < _procesUitkomsten[1])
                {
                    _procesVerschil = Y - _procesUitkomsten[1];
                    X = ProcesKrachtStijgend();
                }
                else if (Y > _procesUitkomsten[1])
                {
                    _procesVerschil = _procesUitkomsten[1] - Y;
                    X = ProcesKrachtDalend();
                }

                double value = Math.Round(X, 4);
                ArrayCheck(value);
                return _procesUitkomsten[_dodeTijd - 1];
            }
            catch (Exception) { ArrayCheck(0); return 0; }
        }

        #region functies proces kracht berekeningen
        private double ProcesKrachtGelijk()
        {
            double X;
            if (_krachtBerState)
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
            double macht = -T / Kracht;
            double e = Math.Pow(Math.E, macht);
            switch (Orde)
            {
                case "0orde":
                    _procesUitkomst = _procesUitkomsten[1] + (_procesVerschil / Kracht);
                    T += _tijdsconstante;
                    break;
                case "1orde":
                    _procesUitkomst = _procesUitkomsten[1] + (_procesVerschil * (1 - e));
                    T += _tijdsconstante;
                    break;
                case "2orde":
                    _procesUitkomst = _procesUitkomsten[1] + ((_procesVerschil * (1 - e)) * (1 - e));
                    T += _tijdsconstante;
                    break;
            }
            return _procesUitkomst;
        }

        private double ProcesKrachtDalend()
        {
            _krachtBerState = false;
            double _procesUitkomst = 0;
            double macht = -T / Kracht;
            double e = Math.Pow(Math.E, macht);
            switch (Orde)
            {
                case "0orde":
                    _procesUitkomst = _procesUitkomsten[1] - (_procesVerschil / Kracht);
                    T += _tijdsconstante;
                    break;
                case "1orde":
                    _procesUitkomst = _procesUitkomsten[1] - (_procesVerschil * (1 - e));
                    T += _tijdsconstante;
                    break;
                case "2orde":
                    _procesUitkomst = _procesUitkomsten[1] - ((_procesVerschil * (1 - e)) * (1 - e));
                    T += _tijdsconstante;
                    break;
            }
            return _procesUitkomst;
        }
        #endregion 
    }
}