using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PIDSimulatorGip.viewmodel
{
    class GridVisibilityViewModel
    {
        #region variables
        private Visibility _serialGridVisible = Visibility.Collapsed;
        private Visibility _animationGridsVisible = Visibility.Collapsed;
        private Visibility _PIDControlGridVisible = Visibility.Visible;
        private Visibility _PIDProcesGridVisible = Visibility.Visible;

        public Visibility SerialGridVisible
        {
            get { return _serialGridVisible; }
            set
            {
                if (_serialGridVisible != value)
                {
                    _serialGridVisible = value;
                }
            }
        }
        public Visibility AnimationGridsVisible
        {
            get { return _animationGridsVisible; }
            set
            {
                if (_animationGridsVisible != value)
                {
                    _animationGridsVisible = value;
                }
            }
        }
        public Visibility PIDControlGridVisible
        {
            get { return _PIDControlGridVisible; }
            set
            {
                if(_PIDControlGridVisible != value)
                {
                    _PIDControlGridVisible = value;
                }
            }
        }
        public Visibility PIDProcesGridVisible
        {
            get { return _PIDProcesGridVisible; }
            set
            {
                if (_PIDProcesGridVisible != value)
                {
                    _PIDProcesGridVisible = value;
                }
            }
        }
        #endregion
    }
}
    

