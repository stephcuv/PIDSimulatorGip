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
        private string _type;

        private double _y;
        private double _w;

        private double _vsfP;

        private double _vsfI;

        private double _vsfD;

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
            set { if (value >= 0) { _tijdsconstante = value; } }
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
                    string result = value.Substring(value.IndexOf(":") + 2);
                    _type = result;
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
                ArrayAdd(_meetWaardes, value);
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
                if (value >= 0)
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
                if (value >= 0)
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
                if (value >= 0)
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
                    ArrayAdd(_foutWaardes, Fout()); 
                    TypeA();
                    break;
                case "Type B":
                    ArrayAdd(_foutWaardes, Fout());
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

            if (_vsfP != 0) _pWaarde = _vsfP * (_foutWaardes[0] - _foutWaardes[1]);
            if (_vsfI != 0) _iWaarde = _vsfI * _foutWaardes[0] * _tijdsconstante;
             if(_vsfD != 0)_dWaarde = (_vsfD / _tijdsconstante) * (_foutWaardes[0] - (2 * _foutWaardes[1]) + _foutWaardes[2]);

                double temp = _prevStuurWaarde + _pWaarde + _iWaarde + _dWaarde;
            if(temp < 0)
            {
                temp = 0;
            }
            else if(temp > 100)
            {
                temp = 100;
            }
            Y = temp;
        }

        private void TypeB()
        {
           if (_vsfP != 0) _pWaarde = _vsfP * (_foutWaardes[0] - _foutWaardes[1]);
           if (_vsfI != 0) _iWaarde = _vsfI * _foutWaardes[0] * _tijdsconstante;
           if(_vsfD != 0) _dWaarde = (_vsfD / _tijdsconstante) * (_meetWaardes[0] - (2 * _meetWaardes[1]) + _meetWaardes[2]);


            double temp = _prevStuurWaarde + _pWaarde + _iWaarde - _dWaarde;
            if (temp < 0)
            {
                temp = 0;
            }
            else if (temp > 100)
            {
                temp = 100;
            }
            Y = temp;
        }

        private void TypeC()
        {
            if (_vsfP != 0) _pWaarde = _vsfP * (_meetWaardes[0] - _meetWaardes[1]);
            if (_vsfI != 0) _iWaarde = _vsfI * Fout() * _tijdsconstante;
            if (_vsfD != 0) _dWaarde = (_vsfD / _tijdsconstante) * (_meetWaardes[0] - (2 * _meetWaardes[1]) + _meetWaardes[2]);


            double temp = _prevStuurWaarde + _pWaarde + _iWaarde - _dWaarde;
            if (temp < 0)
            {
                temp = 0;
            }
            else if (temp > 100)
            {
                temp = 100;
            }
            Y = temp;
        }

        private double Fout()
        {
            return _w - _meetWaardes[0];
        }
        #endregion

        #region arrayAddFunctions

        private void ArrayAdd(double[] array, double value)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                array[i] = array[i - 1];
            }
            array[0] = value;
        }
        #endregion

    }
}