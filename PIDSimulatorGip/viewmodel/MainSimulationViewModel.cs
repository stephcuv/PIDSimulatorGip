using PIDSimulatorGip.model;
using PIDSimulatorGip.MVVM;
using System.Collections.ObjectModel;
using OxyPlot;
using OxyPlot.Series;
using System.Drawing;
using System.Windows.Threading;
using OxyPlot.Axes;

namespace PIDSimulatorGip.viewmodel
{
    internal class MainSimulationViewModel : ViewModelBase
    {

        private RegelaarInstellingen _RGLR;
        private ProcesBerekening _proces;
        private DispatcherTimer _timer;
        private PlotModel? _myPlot;


        private double _rglrWaarde = 0;

        private double _procesWaarde = 0;


        private int _currentXaxis = 0;


        private bool _isRunning;
        private double _simulatieSnelheid;

        public MainSimulationViewModel()
        {
            _RGLR = new RegelaarInstellingen();
            _proces = new ProcesBerekening();
            _timer = new DispatcherTimer();

            MyPlot = new PlotModel { Title = "datapoints" };
            MyPlot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Maximum = 102,
                Minimum = -2,
            });

            _timer.Tick += Timer_Tick;
        }
        public RelayCommand StartCommand => new RelayCommand(execute => { StartSimulation(); });
        public RelayCommand StopCommand => new RelayCommand(execute => { StopSimuation(); }, canExecute => { return _isRunning; });
        public RelayCommand PauzeCommand => new RelayCommand(exectue => { PauseSimulation(); }, canExecute => { return _isRunning; });
        public PlotModel MyPlot { get { return _myPlot; } set { _myPlot = value; OnPropertyChanged(); } }
        public double VSFP { get { return _RGLR.VSFP; } set { _RGLR.VSFP = Math.Round(value, 3); OnPropertyChanged(); } }
        public double VSFI { get { return _RGLR.VSFI; } set { _RGLR.VSFI = Math.Round(value, 3); OnPropertyChanged(); } }
        public double VSFD { get { return _RGLR.VSFD; } set { _RGLR.VSFD = Math.Round(value, 3); OnPropertyChanged(); } }
        public double W { get { return _RGLR.W; } set { _RGLR.W = Math.Round(value, 2); _proces.W = Math.Round(value, 2); OnPropertyChanged(); } }
        public double TijdsConstante { set { _RGLR.Tijdsconstante = Math.Round(value, 2); _proces.Tijdsconstante = Math.Round(value, 2); OnPropertyChanged(); } get { return _RGLR.Tijdsconstante; } }
        public string Type { set { _RGLR.Type = value; OnPropertyChanged(); } }


        public double Kracht { set { _proces.Kracht = Math.Round(value, 2); OnPropertyChanged(); } get { return _proces.Kracht; } }
        public string DodeTijd { set { _proces.DodeTijd = value; OnPropertyChanged(); } get { return _proces.DodeTijd; } }
        public string Orde { set { _proces.Orde = value; OnPropertyChanged(); } get { return _proces.Orde; } }
        public double SimulatieSnelheid { set { _simulatieSnelheid = Math.Round(value, 1); OnPropertyChanged(); } get { return _simulatieSnelheid; } }

        private void StartSimulation()
        {
            _isRunning = true;
            _timer.Interval = TimeSpan.FromMilliseconds(_simulatieSnelheid * 100);
            _timer.Start();
        }
        private void StopSimuation()
        {
            _isRunning = false;
            _timer.Stop();
        }
        private void PauseSimulation()
        {

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _rglrWaarde = _RGLR.Berekening();
            _procesWaarde = _proces.Proces(_rglrWaarde);
            _RGLR.X = _procesWaarde;
            GraphAdd();
        }

        private void GraphAdd()
        {
            if (MyPlot.Series.Count == 0)
            {
                MyPlot.Series.Add(new LineSeries { Title = "Regelaar Waardes" });
                MyPlot.Series.Add(new LineSeries { Title = "Proces Waardes" });
                MyPlot.Series.Add(new LineSeries { Title = "Wenswaarde" });
            }
            var rglrWaardes = MyPlot.Series[0] as LineSeries;
            var procesWaardes = MyPlot.Series[1] as LineSeries;
            var wensWaardes = MyPlot.Series[2] as LineSeries;

            _currentXaxis += 5;

            rglrWaardes.Points.Add(new DataPoint(_currentXaxis, _rglrWaarde));
            procesWaardes.Points.Add(new DataPoint(_currentXaxis, _procesWaarde));
            wensWaardes.Points.Add(new DataPoint(_currentXaxis, W));


            if (rglrWaardes.Points.Count > 100) rglrWaardes.Points.RemoveAt(0);
            if (procesWaardes.Points.Count > 100) procesWaardes.Points.RemoveAt(0);
            if (wensWaardes.Points.Count > 100) wensWaardes.Points.RemoveAt(0);

            var xAxis = MyPlot.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom);
            if (xAxis != null)
            {
                xAxis.Minimum = _currentXaxis - (100 * 5); // Keeps last 100 points visible
                xAxis.Maximum = _currentXaxis;
            }


            MyPlot.InvalidatePlot(false);
        }
    }
}
