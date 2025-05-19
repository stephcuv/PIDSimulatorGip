using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using PIDSimulatorGip.model;
using PIDSimulatorGip.MVVM;
using PIDSimulatorGip.view;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PIDSimulatorGip.viewmodel
{

    internal class MainSimulationViewModel : ViewModelBase
    {

        private RegelaarInstellingen _RGLR;
        private ProcesBerekening _proces;
        private DispatcherTimer _timer;
        private PlotModel? _myPlot;
        private SerialCommunication _serial;


        private double _rglrWaarde = 0; //waarde van de regelaar op dit moment
        private double _procesWaarde = 0; //waarde van proces op dit moment

        private double _currentXaxis = 0;
        private double _maxXAxisPoints = 100;
        private double _simulatieSnelheid = 1;

        public ICommand ShowMessageBoxCommand { get; private set; }



        public MainSimulationViewModel()
        {
            _RGLR = new RegelaarInstellingen();
            _proces = new ProcesBerekening();
            _timer = new DispatcherTimer();

            MyPlot = new PlotModel { Title = "datapoints" };
            MyPlot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "regelaar/proces",
                Maximum = 105,
                Minimum = -5,
            });

            KpMax = 100;
            KiMax = 100;
            KdMax = 100;
            _timer.Tick += Timer_Tick;
        }

        public RelayCommand StartCommand => new RelayCommand(execute => { StartSimulation(); }, canExecute => { return !_isRunning; });
        public RelayCommand ResetCommand => new RelayCommand(execute => { ResetSimulation(); }, canExecute => { return _isRunning || (!_isRunning && _standardSimStatus || _serialComSimStatus || _stapsprongSimStatus); });
        public RelayCommand PauzeCommand => new RelayCommand(exectue => { PauseSimulation(); }, canExecute => { return _isRunning; });


        public RelayCommand AdjustValueCommand => new RelayCommand(execute => { AdjustValue(execute); });
        public RelayCommand PIDBerekeningenZichtbaarCommand => new RelayCommand(execute => { PidBerkeningenZichtbaar(); }, canExecute => { return !_isRunning; });



        #region relaycommands voor de applicatie stand te wijzigen.
        public RelayCommand StapsprongCommand => new RelayCommand(execute => { StapsprongGridVisibility(); }, canExecute => { return !_isRunning; });
        public RelayCommand SerialCommCommand => new RelayCommand(execute => { SerialCommGridVisibility(); Serial(); }, canExecute => { return !_isRunning; });
        public RelayCommand StartStapsprongCommand => new RelayCommand(execute => { StartStapsprongFunction(); }, canExecute => { return _isRunning; });
        public PlotModel MyPlot { get { return _myPlot; } set { _myPlot = value; OnPropertyChanged(); } }
        #endregion

        #region simulation status

        private bool _isRunning;
        private bool _standardSimStatus;
        private bool _serialComSimStatus;
        private bool _stapsprongSimStatus;
        public double SimulatieSnelheid { set { _simulatieSnelheid = Math.Round(value, 2); _timer.Interval = TimeSpan.FromMilliseconds(_simulatieSnelheid * 20); OnPropertyChanged(); } get { return _simulatieSnelheid; } }
        public bool IsRunning { get { return !_isRunning; } set { _isRunning = value; OnPropertyChanged(); OnPropertyChanged(nameof(PIDBerZichtbaarIsEnabled)); } }
        public bool SerialComActive { set { _serialComSimStatus = value; OnPropertyChanged(); } get { return _serialComSimStatus; } }
        public bool Stapsprong { set { _stapsprongSimStatus = value; OnPropertyChanged(); } get { return _stapsprongSimStatus; } }
        public string MaxXAxisPoints { set { _maxXAxisPoints = Convert.ToDouble(value); OnPropertyChanged(); } get { return Convert.ToString(_maxXAxisPoints); } }

        #endregion
        #region ui grid visibility 
        private Visibility _regelaarVisibility = Visibility.Visible;
        private Visibility _procesVisiblity = Visibility.Visible;
        private Visibility _serialVisibility = Visibility.Collapsed;
        private Visibility _stapsprongVisibility = Visibility.Collapsed;
        private Visibility _simulatieSnelheidVisibility = Visibility.Visible;
        private Visibility _animatieVisibility = Visibility.Visible;
        public Visibility RegelaarVisibility { set { _regelaarVisibility = value; OnPropertyChanged(); } get { return _regelaarVisibility; } }
        public Visibility ProcesVisibility { set { _procesVisiblity = value; OnPropertyChanged(); } get { return _procesVisiblity; } }
        public Visibility SerialVisibility { set { _serialVisibility = value; OnPropertyChanged(); } get { return _serialVisibility; } }
        public Visibility StapsprongVisibility { set { _stapsprongVisibility = value; OnPropertyChanged(); } get { return _stapsprongVisibility; } }
        public Visibility SimulatieSnelheidVisibility { set { _simulatieSnelheidVisibility = value; OnPropertyChanged(); } get { return _simulatieSnelheidVisibility; } }
        public Visibility AnimatieVisibility { set { _animatieVisibility = value; OnPropertyChanged(); } get { return _animatieVisibility; } }

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
            AnimatieVisibility = (AnimatieVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;

        }
        #endregion

        #region pid regelaar 
        public double Kp { get { return _RGLR.Kp; } set { if ((_RGLR.Kp + value) >= 0) { _RGLR.Kp = Math.Round(value, 3); } else { _RGLR.Kp = 0; } OnPropertyChanged(); } }
        public double Ki { get { return _RGLR.Ki; } set { if ((_RGLR.Ki + value) >= 0) { _RGLR.Ki = Math.Round(value, 3); } else { _RGLR.Ki = 0; } OnPropertyChanged(); } }
        public double Kd { get { return _RGLR.Kd; } set { if ((_RGLR.Kd + value) >= 0) { _RGLR.Kd = Math.Round(value, 3); } else { _RGLR.Kd = 0; } OnPropertyChanged(); } }
        public double W { get { return _RGLR.W; } set { if (value >= 0) { _RGLR.W = Math.Round(value, 2); } else { _RGLR.W = 0; } OnPropertyChanged(); } }
        public string Type { set { _RGLR.Type = value; OnPropertyChanged(); pidStandaardCheck(); } get { return _RGLR.Type; } }
        public double SamplingRate { set { _RGLR.SamplingRate = Math.Round(value, 2); OnPropertyChanged(); } get { return _RGLR.SamplingRate; } }


        private bool _PIDBerekeningenZichtbaar = false;
        public bool PIDBerZichtbaarIsEnabled { get { return !_isRunning; } set { } }
        public bool PIDBerekeningenZichtbaar { get { return _PIDBerekeningenZichtbaar; } set { _PIDBerekeningenZichtbaar = value; OnPropertyChanged(); } }

        private double _kpMax = 100;
        private double _kiMax = 100;
        private double _kdMax = 100;
        public double KpMax { get { return _kpMax; } set { _kpMax = value; OnPropertyChanged(); } }
        public double KiMax { get { return _kiMax; } set { _kiMax = value; OnPropertyChanged(); } }
        public double KdMax { get { return _kdMax; } set { _kdMax = value; OnPropertyChanged(); } }

        private void pidStandaardCheck()
        {
            if (Type == "Standaard")
            {
                KpMax = 100;
                KiMax = 100;
                KdMax = 100;
                TijdconstanteMax = 50;
                TijdconstanteMin = 1;
                if (Kp > KpMax) { Kp = KpMax; }
                if (Tijdconstante > TijdconstanteMax) { Tijdconstante = TijdconstanteMax; }
                if (Tijdconstante < TijdconstanteMin) { Tijdconstante = TijdconstanteMin; }
            }
            else
            {
                KpMax = 5;
                KiMax = 5;
                KdMax = 5;
                TijdconstanteMax = 5;
                TijdconstanteMin = 0.01;
                if (Tijdconstante > TijdconstanteMax) { Tijdconstante = TijdconstanteMax; }
                if (Tijdconstante < TijdconstanteMin) { Tijdconstante = TijdconstanteMin; }
                if (Kp > KpMax) { Kp = KpMax; }
                if (Ki > KiMax) { Ki = KiMax; }
                if (Kd > KdMax) { Kd = KdMax; }
            }
        }
        #endregion
        #region proces
        public double Tijdconstante { set { if ((_proces.Tijdconstante + value) >= 0) { _proces.Tijdconstante = Math.Round(value, 2); } else { _proces.Tijdconstante = 0; } OnPropertyChanged(); } get { return _proces.Tijdconstante; } }
        public string DodeTijd { set { _proces.DodeTijd = value; OnPropertyChanged(); } get { return _proces.DodeTijd; } }
        public string Orde { set { _proces.Orde = value; OnPropertyChanged(); } get { return _proces.Orde; } }

        private double _tijdconstanteMax = 50;
        private double _tijdconstanteMin = 1;
        public double ProcesWaarde { private set { _procesWaarde = value; OnPropertyChanged(); } get { return _procesWaarde * 2; } }
        public double TijdconstanteMax { get { return _tijdconstanteMax; } set { _tijdconstanteMax = value; OnPropertyChanged(); } }
        public double TijdconstanteMin { get { return _tijdconstanteMin; } set { _tijdconstanteMin = value; OnPropertyChanged(); } }

        #endregion

        #region stapsprong
        private double _stapsprongWaarde = 0;
        private double _stapsprongChangeWaarde = 0;
        public double StapsprongWaarde { set { _stapsprongWaarde = Math.Round(value, 2); OnPropertyChanged(); } get { return _stapsprongWaarde; } }
        public double StapsprongChangeWaarde { set { _stapsprongChangeWaarde = Math.Round(value, 2); OnPropertyChanged(); } get { return _stapsprongChangeWaarde; } }


        private void StartStapsprongFunction()
        {
            if (_stapsprongChangeWaarde != 0) { _proces.T = 0; }

            if ((_stapsprongWaarde + _stapsprongChangeWaarde) > 100)
            {
                StapsprongWaarde = 100;
            }
            else if ((_stapsprongWaarde + _stapsprongChangeWaarde) < 0)
            {
                StapsprongWaarde = 0;
            }
            else
            {
                StapsprongWaarde = _stapsprongWaarde + _stapsprongChangeWaarde;
            }
        }
        #endregion

        #region serial communication     
        private bool _serialSubscribed = false;
        private EventHandler<string> _dataReceivedHandler;
        public List<string> _mogelijkSerPort = new List<string>();
        private void Serial()
        {
            Stopwatch stopwatch = new Stopwatch();

            if (_serial == null)
            {
                _serial = new SerialCommunication();
            }

            Task.Run(() =>
            {
                while (!_serial.Connected && _serialComSimStatus)
                {
                    _messageBoxText = "Starten Seriële Communicatie mislukt \n Herproberen over 10 seconden.";
                    AskTheQuestion();
                    _mogelijkSerPort = _serial.ConnectSerPort();
                    if (_mogelijkSerPort != null)
                    {
                        // ShowCustomMessageBox();
                    }
                    else
                    {
                        AskTheQuestion();
                    }
                    Thread.Sleep(10000);
                }
            });

            if (_serial.Connected)
            {
                if (!_serialSubscribed)
                {
                    _dataReceivedHandler += (sender, data) =>
                    {
                        if (_isRunning)
                        {
                            stopwatch.Stop();
                            Tijdconstante = stopwatch.ElapsedMilliseconds;
                            _procesWaarde = Convert.ToDouble(_serial.IncomeData);
                            _RGLR.X = _procesWaarde;
                            _rglrWaarde = _RGLR.Berekening();
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                GraphAdd();
                            });
                            _serial.SendSerialData($"{_rglrWaarde}");
                            stopwatch.Restart();
                        }
                    };
                    _serial.DataReceived += _dataReceivedHandler;
                    _serialSubscribed = true;
                }

            }
        }
        #endregion

        #region simulation control functions 
        private void AdjustValue(object parameter)
        {
            string? value = Convert.ToString(parameter);

            switch (value)
            {
                case "KpUp":
                    Kp += 0.005;
                    break;

                case "KpDown":
                    Kp -= 0.005;
                    break;

                case "KiUp":
                    Ki += 0.005;
                    break;

                case "KiDown":
                    Ki -= 0.005;
                    break;

                case "KdUp":
                    Kd += 0.005;
                    break;

                case "KdDown":
                    Kd -= 0.005;
                    break;

                case "DtUp":
                    Tijdconstante += 0.005;
                    break;

                case "DtDown":
                    Tijdconstante -= 0.005;
                    break;
                case "WUp":
                    W += 0.05;
                    break;

                case "WDown":
                    W -= 0.05;
                    break;

                case "SRUp":
                    SamplingRate += 0.5;
                    break;

                case "SRDown":
                    SamplingRate -= 0.5;
                    break;
            }
        }


        private void StartSimulation()
        {
            if (!_stapsprongSimStatus && !_serialComSimStatus)
            {
                if ((Tijdconstante <= 0) && (Tijdconstante <= 0) || string.IsNullOrEmpty(DodeTijd) || string.IsNullOrEmpty(Orde) || string.IsNullOrEmpty(Type) || Kp == 0)
                {
                    List<string> missingValues = new List<string>();
                    if (Tijdconstante <= 0) missingValues.Add("Tijdsconstante");
                    if (Tijdconstante <= 0) missingValues.Add("proces kracht");
                    if (string.IsNullOrEmpty(DodeTijd)) missingValues.Add("proces dodetijd");
                    if (string.IsNullOrEmpty(Orde)) missingValues.Add("proces orde");
                    if (string.IsNullOrEmpty(Type)) missingValues.Add("Regelaar type");
                    if (Kp <= 0) missingValues.Add("Versterkingsfactor P regelaar");

                    _messageBoxText = "volgende control(s) moeten een waarde krijgen voor het starten van de applicatie:\n" + string.Join("\n", missingValues);

                    AskTheQuestion();
                    IsRunning = false;
                }
                else
                {
                    IsRunning = true;
                    _myPlot.ResetAllAxes();
                    _standardSimStatus = true;
                    _timer.Start();
                }
            }

            else if (_stapsprongSimStatus)
            {
                if ((Tijdconstante <= 0) || string.IsNullOrEmpty(DodeTijd) || string.IsNullOrEmpty(Orde))
                {
                    List<string> missingValues = new List<string>();

                    if (Tijdconstante <= 0) missingValues.Add("proces kracht");
                    if (string.IsNullOrEmpty(DodeTijd)) missingValues.Add("proces dodetijd");
                    if (string.IsNullOrEmpty(Orde)) missingValues.Add("proces orde");

                    _messageBoxText = "volgende control(s) moeten een waarde krijgen voor het starten van de applicatie:\n" + string.Join("\n", missingValues);

                    AskTheQuestion();
                    IsRunning = false;
                }
                else
                {
                    IsRunning = true;
                    _timer.Interval = TimeSpan.FromMilliseconds(_simulatieSnelheid * 20);
                    _myPlot.ResetAllAxes();
                    _timer.Start();
                }
            }
            else if (_serialComSimStatus)
            {
                if (string.IsNullOrEmpty(Type) || !_serial.Connected || Kp == 0)
                {

                    List<string> missingValues = new List<string>();
                    if (!_serial.Connected) missingValues.Add("geen seriële communicatie actief");
                    if (string.IsNullOrEmpty(Type)) missingValues.Add("Regelaar type");
                    if (Kp <= 0) missingValues.Add("Versterkingsfactor P regelaar");

                    _messageBoxText = "volgende control(s) moeten een waarde krijgen voor het starten van de applicatie:\n" + string.Join("\n", missingValues);

                    AskTheQuestion();
                    IsRunning = false;

                }
                else
                {
                    IsRunning = true;
                    _myPlot.ResetAllAxes();
                    _serial.SendSerialData($"{_RGLR.Berekening()}");
                }
            }

        }
        private void PauseSimulation()
        {
            IsRunning = false;
            if (!_serialComSimStatus) _timer.Stop();
        }

        private void ResetSimulation()
        {
            IsRunning = false;

            if (!_serialComSimStatus) _timer.Stop();
            if (_serialComSimStatus)
            {
                _serial.DisconnectSerPort();
                _serialSubscribed = false;
                _serial.DataReceived -= _dataReceivedHandler;
            }

            Kp = 0;
            Ki = 0;
            Kd = 0;
            W = 0;

            _rglrWaarde = 0;
            SimulatieSnelheid = 1;
            Tijdconstante = 0;

            DodeTijd = string.Empty;
            Orde = string.Empty;
            Type = string.Empty;

            if (_stapsprongSimStatus)
            {
                StapsprongChangeWaarde = 0;
                StapsprongWaarde = 0;
            }

            Tijdconstante = 0;
            ProcesWaarde = 0;
            MaxXAxisPoints = "100";

            _RGLR.Reset();
            _proces.Reset();

            if (_stapsprongSimStatus) StapsprongGridVisibility();
            if (_serialComSimStatus) SerialCommGridVisibility();
            _stapsprongSimStatus = false;
            _serialComSimStatus = false;
            _standardSimStatus = false;

            _currentXaxis = 0;
            MyPlot.Series.Clear();
            MyPlot.Legends.Clear();
            MyPlot.ResetAllAxes();
            MyPlot.InvalidatePlot(true);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_stapsprongSimStatus)
            {
                ProcesWaarde = _proces.Proces(_stapsprongWaarde);
                GraphAdd();
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
        #region messagebox
        private string _messageBoxText;

        protected void AskTheQuestion()
        {
            MessageBox_Show(_messageBoxText, "error", System.Windows.MessageBoxButton.OK);
        }

        private void ShowCustomMessageBox()
        {
        }
        #endregion

        #region graph
        private void PidBerkeningenZichtbaar()
        {
            _PIDBerekeningenZichtbaar = !_PIDBerekeningenZichtbaar;

            if (_PIDBerekeningenZichtbaar)
            {
                MyPlot.Axes.Clear();
                MyPlot.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "regelaar/proces + PID berekeningen",
                    Maximum = 105,
                    Minimum = -5,
                });

                MyPlot.Series.Add(new LineSeries { Title = "P Waarde", TrackerFormatString = "tijdstip: {2:0.000} sec\n" + "P Waarde: {4:0.00}" });
                MyPlot.Series.Add(new LineSeries { Title = "I Waarde", TrackerFormatString = "tijdstip: {2:0.000} sec\n" + "I Waarde: {4:0.00}" });
                MyPlot.Series.Add(new LineSeries { Title = "D Waarde", TrackerFormatString = "tijdstip: {2:0.000} sec\n" + "D Waarde: {4:0.00}" });
            }
            else
            {
                MyPlot.Axes.Clear();
                MyPlot.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "regelaar/proces",
                    Maximum = 105,
                    Minimum = -5,
                });

                for (int i = 0; i < MyPlot.Series.Count; i++)
                {
                    var series = MyPlot.Series[i] as LineSeries;
                    switch (series.Title)
                    {
                        case "P Waarde":
                            MyPlot.Series.Remove(series);
                            break;
                        case "I Waarde":
                            MyPlot.Series.Remove(series);
                            break;
                        case "D Waarde":
                            MyPlot.Series.Remove(series);
                            break;
                    }
                }
            }
            MyPlot.InvalidatePlot(true);
        }


        private void GraphAdd()
        {
            if ((MyPlot.Series.Count == 0) || (_PIDBerekeningenZichtbaar && MyPlot.Series.Count == 3))
            {
                var legend = new Legend
                {
                    LegendPosition = LegendPosition.TopCenter,
                    LegendPlacement = LegendPlacement.Outside,
                    LegendOrientation = LegendOrientation.Horizontal,
                    LegendBorderThickness = 1
                };
                MyPlot.Legends.Add(legend);


                if (_standardSimStatus)
                {
                    MyPlot.Series.Add(new LineSeries { Title = "Regelaar Waarde", TrackerFormatString = "tijdstip: {2:0.000} sec\n" + "regelaar Waarde: {4:0.00}" });
                    MyPlot.Series.Add(new LineSeries { Title = "Proces Waarde", TrackerFormatString = "tijdstip: {2:0.000} sec\n" + "proces Waarde: {4:0.00}" });
                    MyPlot.Series.Add(new LineSeries { Title = "Wenswaarde", TrackerFormatString = "tijdstip: {2:0.000} sec\n" + "wens Waarde: {4:0.00}" });
                }
                else if (_serialComSimStatus)
                {
                    MyPlot.Series.Add(new LineSeries { Title = "Regelaar Waarde", TrackerFormatString = "tijdstip: {2:0.000} millisec\n" + "regelaar Waarde: {4:0.00}" });
                    MyPlot.Series.Add(new LineSeries { Title = "Proces Waarde", TrackerFormatString = "tijdstip: {2:0.000} millisec\n" + "proces Waarde: {4:0.00}" });
                    MyPlot.Series.Add(new LineSeries { Title = "Wenswaarde", TrackerFormatString = "tijdstip: {2:0.000} millisec\n" + "wens Waarde: {4:0.00}" });
                }

                else if (_stapsprongSimStatus)
                {
                    MyPlot.Series.Add(new LineSeries { Title = "Proces Waarde", TrackerFormatString = "tijdstip: {2:0.000} sec\n" + "Proces Waarde: {4:0.00}" });
                    MyPlot.Series.Add(new LineSeries { Title = "stapsprong", TrackerFormatString = "tijdstip: {2:0.000} sec\n" + "stapsprong Waarde: {4:0.00}" });
                }
                else
                {
                    Debug.WriteLine("fout in maken van de lineseries");
                }
            }
            if (_standardSimStatus || _serialComSimStatus)
            {
                for (int i = 0; i < MyPlot.Series.Count; i++)
                {
                    var series = MyPlot.Series[i] as LineSeries;

                    switch (series.Title)
                    {
                        case "Regelaar Waarde":
                            series.Points.Add(new DataPoint(_currentXaxis, _rglrWaarde));
                            break;
                        case "Proces Waarde":
                            series.Points.Add(new DataPoint(_currentXaxis, _procesWaarde));
                            break;
                        case "Wenswaarde":
                            series.Points.Add(new DataPoint(_currentXaxis, W));
                            break;
                        case "P Waarde":
                            series.Points.Add(new DataPoint(_currentXaxis, _RGLR.PWaarde));
                            break;
                        case "I Waarde":
                            series.Points.Add(new DataPoint(_currentXaxis, _RGLR.IWaarde));
                            break;
                        case "D Waarde":
                            series.Points.Add(new DataPoint(_currentXaxis, _RGLR.DWaarde));
                            break;
                    }
                }
                _currentXaxis += Tijdconstante;
            }


            else if (_stapsprongSimStatus)
            {
                var procesWaardes = MyPlot.Series[0] as LineSeries;
                var wensWaardes = MyPlot.Series[1] as LineSeries;

                _currentXaxis += Tijdconstante;

                procesWaardes.Points.Add(new DataPoint(_currentXaxis, _procesWaarde));
                wensWaardes.Points.Add(new DataPoint(_currentXaxis, _stapsprongWaarde));
            }

            else
            {
                Debug.WriteLine("error in tekeken van graph");
            }
            var xAxis = MyPlot.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom);
            if (xAxis != null)
            {
                if (!_serialComSimStatus) xAxis.Minimum = _currentXaxis - (_maxXAxisPoints * Tijdconstante); // Keeps last 100 points visible
                else xAxis.Minimum = _currentXaxis - _maxXAxisPoints;
                xAxis.Maximum = _currentXaxis;
            }


            MyPlot.InvalidatePlot(false);
        }
        #endregion
    }
}
