using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIDSimulatorGip.model
{
    class RegelaarInstellingen
    {
        #region variabelen
        private string _type = "Type C";

        private double _y;
        private double _w;

        private double _vsfP = 1;

        private double _vsfI = 1;

        private double _vsfD = 1;

        private double _tijdsconstante;

        private double[] _foutWaardes = new double[3];
        private double[] _meetWaardes = new double[3];

        //variabelen voor het afprinten in exel van de uitkomst  appart
        private double _pWaarde;
        private double _iWaarde;
        private double _dWaarde;

        private double _prevStuurWaarde;


        #region instantievariabelen
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
            get {return _tijdsconstante;}
            set { if (value > 0) { _tijdsconstante = value; } }
        }

        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _type = value;
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
                return _meetWaardes[0];
            }
            set
            {
                ArrayAddMeetW(value);
            }
        }
        public double W
        {
            get
            {
                return _w;
            }
            set
            {
                if ((value >= 0) && value <= 100)
                {
                    _w = value;
                }
            }
        }

        public double VSFP
        {
            get
            {
                return _vsfP;
            }
            set
            {
                if (value > 0)
                {
                    _vsfP = value;
                }
            }
        }
        public double VSFI
        {
            get
            {
                return _vsfI;
            }
            set
            {
                if (value > 0)
                {
                    _vsfI = value;
                }
            }
        }
        public double VSFD
        {
            get
            {
                return _vsfD;
            }
            set
            {
                if (value > 0)
                {
                    _vsfD = value;
                }
            }
        }
        #endregion
        #endregion

        private void RegelaarBerekening()
        {
            switch (_type)
            {
                case "Type A":
                    ArrayAddFout(Fout());
                    TypeA();
                    break;
                case "Type B":
                    ArrayAddFout(Fout());
                    TypeB();
                    break;
                case "Type C":
                    TypeC();
                    break;
            }
        }

        public double Berekening()
        {
            RegelaarBerekening();
            if (Y == 0)
            {
                return _prevStuurWaarde;
            }
            else
            {
                _prevStuurWaarde = Y;
                return Y;
            }

        }

        #region types
        private void TypeA()
        {

            _pWaarde = _vsfP * (_foutWaardes[0] - _foutWaardes[1]);
            _iWaarde = _vsfI * _foutWaardes[0] * _tijdsconstante;
            _dWaarde = (_vsfD / _tijdsconstante) * (_foutWaardes[0] - (2 * _foutWaardes[1]) + _foutWaardes[2]);

            Y = _prevStuurWaarde + _pWaarde + _iWaarde + _dWaarde;
        }

        private void TypeB()
        {
            _pWaarde = _vsfP * (_foutWaardes[0] - _foutWaardes[1]);
            _iWaarde = _vsfI * _foutWaardes[0] * _tijdsconstante;
            _dWaarde = (_vsfD / _tijdsconstante) * (_meetWaardes[0] - (2 * _meetWaardes[1]) + _meetWaardes[2]);


            Y = _prevStuurWaarde + _pWaarde + _iWaarde - _dWaarde;
        }

        private void TypeC()
        {
            _pWaarde = _vsfP * (_meetWaardes[0] - _meetWaardes[1]);
            _iWaarde = _vsfI * Fout() * _tijdsconstante;
            _dWaarde = (_vsfD / _tijdsconstante) * (_meetWaardes[0] - (2 * _meetWaardes[1]) + _meetWaardes[2]);


            Y = _prevStuurWaarde + _pWaarde + _iWaarde - _dWaarde;
        }

        private double Fout()
        {
            return _w - _meetWaardes[0];
        }
        #endregion


        #region arrayAddFunctions
        private void ArrayAddFout(double waarde)
        {
            for (int i = 2; i > 0; i--)
            {
                _foutWaardes[i] = _foutWaardes[i - 1];
            }
            _foutWaardes[0] = waarde;
        }
        private void ArrayAddMeetW(double waarde)
        {
            for (int i = 2; i > 0; i--)
            {
                _meetWaardes[i] = _meetWaardes[i - 1];
            }
            _meetWaardes[0] = waarde;
        }
        #endregion

    }
}