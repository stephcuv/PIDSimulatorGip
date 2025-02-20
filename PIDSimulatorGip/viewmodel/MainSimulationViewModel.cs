using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using PIDSimulatorGip.model;
using PIDSimulatorGip.MVVM;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace PIDSimulatorGip.viewmodel
{

    internal class MainSimulationViewModel : ViewModelBase
    {

        private RegelaarInstellingen _RGLR;
        private ProcesBerekening _proces;
        private DispatcherTimer _timer;
        private PlotModel? _myPlot;


        private double _rglrWaarde = 0; //waarde van de regelaar op dit moment
        private double _procesWaarde = 0; //waarde van proces op dit moment

        private double _currentXaxis = 0;

        private double _simulatieSnelheid = 0.5;


        public MainSimulationViewModel()
        {
            _RGLR = new RegelaarInstellingen();
            _proces = new ProcesBerekening();
            _timer = new DispatcherTimer();

            MyPlot = new PlotModel { Title = "datapoints" };
            MyPlot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = 100,
                Minimum = 0,
            });

            _timer.Tick += Timer_Tick;
        }
        public RelayCommand StartCommand => new RelayCommand(execute => { StartSimulation(); }, canExecute => { return !_isRunning; });
        public RelayCommand ResetCommand => new RelayCommand(execute => { ResetSimulation(); }, canExecute => { return _isRunning; });
        public RelayCommand PauzeCommand => new RelayCommand(exectue => { PauseSimulation(); }, canExecute => { return _isRunning; });

        public RelayCommand StapSprongCommand => new RelayCommand(execute => { StapsprongGridVisibility(); }, canExecute => { return !_isRunning && !_serialComSimStatus; });
        public RelayCommand SerialCommCommand => new RelayCommand(execute => { SerialCommGridVisibility(); }, canExecute => { return !_isRunning && !_stapsprongSimStatus; });


        public PlotModel MyPlot { get { return _myPlot; } set { _myPlot = value; OnPropertyChanged(); } }

        #region stapsprong
        private double _stapsprongWaarde = 0;
        private double _stapsprongChangeWaarde = 0;
        public double StapsprongWaarde { set { _stapsprongWaarde = Math.Round(value, 2); OnPropertyChanged(); } get { return _stapsprongWaarde; } }
        public double StapsprongChangeWaarde { set { _stapsprongChangeWaarde = Math.Round(value, 2); OnPropertyChanged(); } get { return _stapsprongChangeWaarde; } }

        #endregion

        
        #region simulation status

        private bool _isRunning;
        private bool _standardSimStatus;
        private bool _serialComSimStatus;
        private bool _stapsprongSimStatus;        
        public double SimulatieSnelheid { set { _simulatieSnelheid = Math.Round(value, 2); OnPropertyChanged(); } get { return _simulatieSnelheid; } }
        public bool IsRunning { get { return !_isRunning; } set { _isRunning = value; OnPropertyChanged(); } }
        public bool SerialComActive { set { _serialComSimStatus = value; OnPropertyChanged(); } get { return _serialComSimStatus; } }
        public bool Stapsprong { set { _stapsprongSimStatus = value; OnPropertyChanged(); } get { return _stapsprongSimStatus; } }
        #endregion
        #region pid regelaar 
        public double VSFP { get { return _RGLR.VSFP; } set { _RGLR.VSFP = Math.Round(value, 3); OnPropertyChanged(); } }
        public double VSFI { get { return _RGLR.VSFI; } set { _RGLR.VSFI = Math.Round(value, 3); OnPropertyChanged(); } }
        public double VSFD { get { return _RGLR.VSFD; } set { _RGLR.VSFD = Math.Round(value, 3); OnPropertyChanged(); } }
        public double W { get { return _RGLR.W; } set { _RGLR.W = Math.Round(value, 2); OnPropertyChanged(); } }
        public double TijdsConstante { set { _RGLR.Tijdsconstante = Math.Round(value, 2); _proces.Tijdsconstante = Math.Round(value, 2); OnPropertyChanged(); } get { return _RGLR.Tijdsconstante; } }
        public string Type { set { _RGLR.Type = value; OnPropertyChanged(); } get { return _RGLR.Type; } }

        #endregion
        #region pid proces
        public double Kracht { set { _proces.Kracht = Math.Round(value, 2); OnPropertyChanged(); } get { return _proces.Kracht; } }
        public string DodeTijd { set { _proces.DodeTijd = value; OnPropertyChanged(); } get { return _proces.DodeTijd; } }
        public string Orde { set { _proces.Orde = value; OnPropertyChanged(); } get { return _proces.Orde; } }

        public double ProcesWaarde { private set { _procesWaarde = value; OnPropertyChanged(); } get { return _procesWaarde * 3; } }
        #endregion
        #region ui grid visibility 
        private Visibility _regelaarVisibility = Visibility.Visible;
        private Visibility _procesVisiblity = Visibility.Visible;
        private Visibility _serialVisibility = Visibility.Collapsed;
        private Visibility _stapsprongVisibility = Visibility.Collapsed;
        private Visibility _simulatieSnelheidVisibility = Visibility.Visible;
        public Visibility RegelaarVisibility { set { _regelaarVisibility = value; OnPropertyChanged(); } get { return _regelaarVisibility; } }
        public Visibility ProcesVisibility { set { _procesVisiblity = value; OnPropertyChanged(); } get { return _procesVisiblity; } }
        public Visibility SerialVisibility { set { _serialVisibility = value; OnPropertyChanged(); } get { return _serialVisibility; } }
        public Visibility StapsprongVisibility { set { _stapsprongVisibility = value; OnPropertyChanged(); } get { return _stapsprongVisibility; } }
        public Visibility SimulatieSnelheidVisibility { set { _simulatieSnelheidVisibility = value; OnPropertyChanged(); } get { return _simulatieSnelheidVisibility; } }

        private void StapsprongGridVisibility()
        {
            _stapsprongSimStatus = !_stapsprongSimStatus;
            RegelaarVisibility = (RegelaarVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            StapsprongVisibility = (StapsprongVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SerialCommGridVisibility()
        {
            _serialComSimStatus = !_serialComSimStatus;
            SerialVisibility = (SerialVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            ProcesVisibility = (ProcesVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            SimulatieSnelheidVisibility = (SimulatieSnelheidVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }
        #endregion

        #region multipliers
        private enum multiplier
        {
            [Description("1x")]
            OneX = 1,

            [Description("10x")]
            TenX = 10,

            [Description("100x")]
            HundredX = 100
        }
        private multiplier _VSFPMultiplier = multiplier.OneX;
        private multiplier _VSFIMultiplier = multiplier.OneX;
        private multiplier _VSFDMultiplier = multiplier.OneX;
        private multiplier _procesKrachtMultiplier = multiplier.OneX;
        private multiplier _tijdsconsteMultiPlier = multiplier.OneX;

        private multiplier VSFPMultiplier {set { _VSFPMultiplier = value; OnPropertyChanged(); } get { return _VSFPMultiplier; } }
        private multiplier VSFIMultiplier { set { _VSFIMultiplier = value; OnPropertyChanged(); } get { return _VSFIMultiplier; } }
        private multiplier VSFDMultiplier { set { _VSFDMultiplier = value; OnPropertyChanged(); } get { return _VSFDMultiplier; } }
        private multiplier ProcesKrachtMultiplier { set { _procesKrachtMultiplier = value; OnPropertyChanged(); } get { return _procesKrachtMultiplier; } }
        private multiplier TijdsconstanteMultiplier { set { _tijdsconsteMultiPlier = value; OnPropertyChanged(); } get { return _tijdsconsteMultiPlier; } }

        #endregion

        #region simulation control functions 
        private void StartSimulation()
        {
            if (!_stapsprongSimStatus && !_serialComSimStatus)
            {
                if ((TijdsConstante > 0) && (Kracht > 0) && !string.IsNullOrEmpty(DodeTijd) && !string.IsNullOrEmpty(Orde) && !string.IsNullOrEmpty(Type))
                {
                    IsRunning = true;
                    _standardSimStatus = true;
                    _timer.Interval = TimeSpan.FromMilliseconds(_simulatieSnelheid * 20);
                    _timer.Start();
                }
                else
                {
                    IsRunning = false;
                    _standardSimStatus = false;
                }
            }
            else if (_stapsprongSimStatus)
            {
                if ((Kracht > 0) && !string.IsNullOrEmpty(DodeTijd) && !string.IsNullOrEmpty(Orde))
                {
                    IsRunning = true;
                    _timer.Interval = TimeSpan.FromMilliseconds(_simulatieSnelheid * 20);
                    _timer.Start();
                }
                else
                {
                    IsRunning = false;
                }
            }

        }
        private void PauseSimulation()
        {
            IsRunning = false;
            _timer.Stop();
        }

        private void ResetSimulation()
        {
            IsRunning = false;
            _timer.Stop();

            var rglrWaardes = MyPlot.Series[0] as LineSeries;
            var procesWaardes = MyPlot.Series[1] as LineSeries;
            var wensWaardes = MyPlot.Series[2] as LineSeries;

            rglrWaardes.Points.Clear();
            procesWaardes.Points.Clear();
            wensWaardes.Points.Clear();
            _currentXaxis = 0;

            MyPlot.InvalidatePlot(true);

            for (int i = 0; i < _proces.DodeTijdNumber; i++)
            {
                _rglrWaarde = _RGLR.Berekening();
                ProcesWaarde = _proces.Proces(_rglrWaarde);
                _RGLR.X = _procesWaarde;
            }

            VSFP = 0;
            VSFI = 0;
            VSFD = 0;
            W = 0;
            TijdsConstante = 0;
            Type = string.Empty;

            Kracht = 0;
            DodeTijd = string.Empty;
            Orde = string.Empty;

            ProcesWaarde = 0;
            SimulatieSnelheid = 0.5;
        }
     
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_stapsprongSimStatus)
            {
                ProcesWaarde = _proces.Proces(_stapsprongWaarde);
                GraphAdd();
            }
            else if (_serialComSimStatus)
            {

            }
            else
            {
                _rglrWaarde = _RGLR.Berekening();
                ProcesWaarde = _proces.Proces(_rglrWaarde);
                _RGLR.X = _procesWaarde;
                GraphAdd();
            }
        }
        #endregion
        #region graph
        private void GraphAdd()
        {
            if (MyPlot.Series.Count == 0)
            {
                var legend = new Legend
                {
                    LegendPosition = LegendPosition.TopCenter,
                    LegendPlacement = LegendPlacement.Outside,
                    LegendOrientation = LegendOrientation.Horizontal,
                    LegendBorderThickness = 1
                };
                MyPlot.Legends.Add(legend);

                if (_standardSimStatus || _serialComSimStatus)
                {
                    MyPlot.Series.Add(new LineSeries { Title = "Regelaar Waarde", TrackerFormatString = "tijdstip: {2:0} sec\n" + "regelaar Waarde: {4:0.00}" });
                    MyPlot.Series.Add(new LineSeries { Title = "Proces Waarde", TrackerFormatString = "tijdstip: {2:0} sec\n" + "proces Waarde: {4:0.00}" });
                    MyPlot.Series.Add(new LineSeries { Title = "Wenswaarde", TrackerFormatString = "tijdstip: {2:0} sec\n" + "wens Waarde: {4:0.00}" });
                }
                else if (_stapsprongSimStatus)
                {
                    MyPlot.Series.Add(new LineSeries { Title = "Proces Waarde", TrackerFormatString = "tijdstip: {2:0} sec\n" + "Proces Waarde: {4:0.00}" });
                    MyPlot.Series.Add(new LineSeries { Title = "stapsprong", TrackerFormatString = "tijdstip: {2:0} sec\n" + "stapsprong Waarde: {4:0.00}" });
                }
                else
                {
                    Debug.WriteLine("fout in maken van de lineseries");
                }
            }
            if (_standardSimStatus || _serialComSimStatus)
            {
                var rglrWaardes = MyPlot.Series[0] as LineSeries;
                var procesWaardes = MyPlot.Series[1] as LineSeries;
                var wensWaardes = MyPlot.Series[2] as LineSeries;

                _currentXaxis += TijdsConstante;

                rglrWaardes.Points.Add(new DataPoint(_currentXaxis, _rglrWaarde));
                procesWaardes.Points.Add(new DataPoint(_currentXaxis, _procesWaarde));
                wensWaardes.Points.Add(new DataPoint(_currentXaxis, W));


                if (rglrWaardes.Points.Count > 100) rglrWaardes.Points.RemoveAt(0);
                if (procesWaardes.Points.Count > 100) procesWaardes.Points.RemoveAt(0);
                if (wensWaardes.Points.Count > 100) wensWaardes.Points.RemoveAt(0);
            }

            else if (_stapsprongSimStatus)
            {
                var procesWaardes = MyPlot.Series[0] as LineSeries;
                var wensWaardes = MyPlot.Series[1] as LineSeries;

                _currentXaxis += TijdsConstante;

                procesWaardes.Points.Add(new DataPoint(_currentXaxis, _procesWaarde));
                wensWaardes.Points.Add(new DataPoint(_currentXaxis, _stapsprongWaarde));


                if (procesWaardes.Points.Count > 100) procesWaardes.Points.RemoveAt(0);
                if (wensWaardes.Points.Count > 100) wensWaardes.Points.RemoveAt(0);
            }

            else
            {
                Debug.WriteLine("error in tekeken van graph");
            }
            var xAxis = MyPlot.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom);
            if (xAxis != null)
            {
                xAxis.Minimum = _currentXaxis - (100 * TijdsConstante); // Keeps last 100 points visible
                xAxis.Maximum = _currentXaxis;
            }


            MyPlot.InvalidatePlot(false);
        }
        #endregion
    }
}
