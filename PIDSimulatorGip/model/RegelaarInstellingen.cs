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

        private double _kp;

        private double _ki;

        private double _kd;

        private double _kc;

        private double _samplingRate = 1;

        private double[] _foutWaardes = new double[3];
        private double[] _meetWaardes = new double[3];

        //variabelen voor het afprinten in exel van de uitkomst  appart
        private double _pWaarde;
        private double _iWaarde;
        private double _dWaarde;

        private double _prevStuurWaarde;
        private double _prevIWaarde;



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
        public double SamplingRate
        {
            get { return _samplingRate; }
            set { if (value >= 0) { _samplingRate = value; } }
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

        public double Kp
        {
            get
            {
                return _kp;
            }
            set
            {
                if (value >= 0)
                {
                    _kp = value;
                }
            }
        }
        public double Ki
        {
            get
            {
                return _ki;
            }
            set
            {
                if (value >= 0)
                {
                    _ki = value;
                }
            }
        }
        public double Kd
        {
            get
            {
                return _kd;
            }
            set
            {
                if (value >= 0)
                {
                    _kd = value;
                }
            }
        }

        public double Kc
        {
            get
            {
                return _kc;
            }
            set
            {
                if (value >= 0)
                {
                    _kc = value;
                }
            }
        }
        #endregion
        #endregion

        public double Berekening()
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
                case "Standaard":
                    ArrayAdd(_foutWaardes, Fout());
                    standaard();
                    break;
            }

            Y = Math.Round(Y, 2);
            _prevStuurWaarde = Y;
            return Y;

        }

        #region types
        private void TypeA()
        {

            _pWaarde = _kp * (_foutWaardes[0] - _foutWaardes[1]);
            _iWaarde = _ki * _foutWaardes[0] * _samplingRate;
            _dWaarde = (_kd / _samplingRate) * (_foutWaardes[0] - (2 * _foutWaardes[1]) + _foutWaardes[2]);

            double temp = _prevStuurWaarde + _pWaarde + _iWaarde + _dWaarde;
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

        private void TypeB()
        {
            _pWaarde = _kp * (_foutWaardes[0] - _foutWaardes[1]);
            _iWaarde = _ki * _foutWaardes[0] * _samplingRate;
            _dWaarde = (_kd / _samplingRate) * (_meetWaardes[0] - (2 * _meetWaardes[1]) + _meetWaardes[2]);


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
            _pWaarde = _kp * (_meetWaardes[0] - _meetWaardes[1]);
            _iWaarde = _ki * Fout() * _samplingRate;
            _dWaarde = (_kd / _samplingRate) * (_meetWaardes[0] - (2 * _meetWaardes[1]) + _meetWaardes[2]);


            double temp = _prevStuurWaarde - _pWaarde + _iWaarde - _dWaarde;
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

        private void standaard()
        {

            _kc = (1 / _kp) * 100;

            _pWaarde = _kc * _foutWaardes[0];
            if (_pWaarde < 0)
            {
                _pWaarde = 0;
            }
            if (_pWaarde > 100)
            {
                _pWaarde = 100;
            }



            if (_ki != 0)
            {
                _iWaarde = _prevIWaarde + (_kc / _ki) * _foutWaardes[0] * _samplingRate;
            }
            _dWaarde = -_kc * (_kd / 60) * ((_meetWaardes[0] - _meetWaardes[1]) / _samplingRate);

            _prevIWaarde = _iWaarde;

            double temp = _pWaarde + _iWaarde + _dWaarde;
            if (temp <= 0)
            {
                temp = 0;
            }
            else if (temp > 100)
            {
                temp = 100;
            }
            _prevStuurWaarde = temp;
            Y = temp;
        }

        private double Fout()
        {
            return _w - _meetWaardes[0];
        }
        #endregion

        #region arrayFunctions

        private void ArrayAdd(double[] array, double value)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                array[i] = array[i - 1];
            }
            array[0] = value;
        }
        #endregion

        public void Reset()
        {
            for (int i = 0; i < 3; i++)
            {
                ArrayAdd(_meetWaardes, 0);
                ArrayAdd(_foutWaardes, 0);
            }
            _prevStuurWaarde = 0;
        }

    }
}