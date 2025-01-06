using PIDSimulatorGip.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PIDSimulatorGip.view
{
    /// <summary>
    /// Interaction logic for MainSimulationWindow.xaml
    /// </summary>
    public partial class MainSimulationWindow : Window
    {
        bool _run;
        Polyline? _polylineX;
        Polyline? _polylineY;
        Line? _line;
        List<double> _dataPointsX = new List<double>();
        List<double> _dataPointsY = new List<double>();
        double _wensWaarde;

        private RegelaarInstellingen RGLR = new RegelaarInstellingen();
        private ProcesBerekening proces = new ProcesBerekening();

        public MainSimulationWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Proces;
            timer.Start();
            this.Loaded += MainWindow_Loaded;

            this.DataContext = this;
        }

        #region EventHandlers
        #region PIDControlEventHandlers
        private void SoortRegelaarCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string ?temp = SoortRegelaarCMB.SelectedItem.ToString();
            RGLR.Regelaar = temp;
        }

        private void GeschakeldCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? temp = GeschakeldCMB.SelectedItem.ToString();
            RGLR.Geschakeld = temp;
        }

        private void WensWaardeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double temp = Convert.ToDouble(WensWaardeSlider.Value);
            RGLR.W = temp;
            proces.W = temp;
            _wensWaarde = temp;
        }

        private void PactieSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RGLR.VSFP = Convert.ToDouble(PactieSlider.Value);
        }

        private void IactieSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RGLR.VSFI = Convert.ToDouble(IactieSlider.Value);
        }

        private void DactieSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RGLR.VSFD = Convert.ToDouble(DactieSlider.Value);
        }

        private void DfilterSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RGLR.Dfilter = Convert.ToDouble(DfilterSlider.Value);   
        }
        #endregion
        #region PIDProcesEventHandlers
        private void ProcesKrachtSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            proces.Kracht = Convert.ToDouble(ProcesKrachtSlider.Value); 
        }

        private void ProcesOrdeCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            proces.Orde = ProcesOrdeCMB.SelectedItem.ToString();
        }

        private void TijdsConstanteSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double temp = Convert.ToInt16(TijdsConstanteSlider.Value);
            RGLR.Tijdsconstante = temp;
            proces.Tijdsconstante = temp;   
        }
        private void DodetijdCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            proces.DodeTijd = DodetijdCMB.SelectedItem.ToString();
        }
        #endregion
        #region SimulatieEventHandlers
        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Check())
            {
                StopBtn.IsEnabled = true;
                PauseBtn.IsEnabled = true;

                StartBtn.IsEnabled = false;
                SoortRegelaarCMB.IsEnabled = false;
                GeschakeldCMB.IsEnabled = false;
                ProcesKrachtSlider.IsEnabled = false;
                ProcesOrdeCMB.IsEnabled = false;
                _run = true;
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            _run = false;
            StopBtn.IsEnabled = false;
            PauseBtn.IsEnabled = false;

            StartBtn.IsEnabled = true;
            SoortRegelaarCMB.IsEnabled = true;
            GeschakeldCMB.IsEnabled = true;
            ProcesKrachtSlider.IsEnabled = true;
            ProcesOrdeCMB.IsEnabled = true;
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            _run = false;
            StartBtn.IsEnabled = true;
        }
        #endregion
        #region SerialControlEventHandlers
        private void SerialConnectBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SerialDisconectBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SerialSelectionCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AutoSerialCKB_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void AutoSerialCKB_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        #endregion
        #endregion
        private void DataAddX(double value)
        {
            _dataPointsX.Add(value);
            if(_dataPointsX.Count > 100 )
            {
                _dataPointsX.RemoveAt(0);
            }
        }
        private void DataAddY(double value)
        {
            _dataPointsY.Add(value);
            if (_dataPointsY.Count > 100)
            {
                _dataPointsY.RemoveAt(0);
            }
        }
        #region UI visuals
        #region graph
        private void StartGraph()
        {
            _polylineX = new Polyline
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
            };
            _polylineY = new Polyline
            {
                Stroke = Brushes.Green,
                StrokeThickness = 2,
            };

            _line = new Line
            {
                Stroke = Brushes.Yellow,
                StrokeThickness = 2,
            }; 
        }

        private void UpdateGraph()
        {
            GraphCanvas.Children.Clear();
            _polylineX.Points.Clear();
            _polylineY.Points.Clear();
            
            

            double scaleY = GraphCanvas.ActualHeight / 10;
            double xSpacing = GraphCanvas.ActualWidth / 100;

            for(int i = 0; i < _dataPointsX.Count; i++)
            {
                double x = i * xSpacing;
                double y = GraphCanvas.ActualHeight - (_dataPointsX[i] * scaleY);
                _polylineX.Points.Add(new Point(x, y));
            }
            for(int i = 0; i < _dataPointsY.Count; i++)
            {
                double x = i * xSpacing;
                double y = GraphCanvas.ActualHeight - (_dataPointsY[i] * scaleY);
                _polylineY.Points.Add(new Point(x, y));
            }
            _line.X1 = 0;
            _line.X2 = GraphCanvas.ActualWidth;
            _line.Y1 = GraphCanvas.ActualHeight - (_wensWaarde * scaleY);
            _line.Y2 = GraphCanvas.ActualHeight - (_wensWaarde * scaleY);
            GraphCanvas.Children.Add(_polylineX);
            GraphCanvas.Children.Add(_polylineY);   
            GraphCanvas.Children.Add(_line); 
        }
        #endregion
        #endregion
        #region code for main simulation
        private bool Check()
        {
            return true;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SoortRegelaarCMB.Items.Add("P");
            SoortRegelaarCMB.Items.Add("I");
            SoortRegelaarCMB.Items.Add("PI");
            SoortRegelaarCMB.Items.Add("PD");
            SoortRegelaarCMB.Items.Add("PID");
            GeschakeldCMB.Items.Add("Parallel");
            GeschakeldCMB.Items.Add("Serieel");
            ProcesOrdeCMB.Items.Add("1orde");
            ProcesOrdeCMB.Items.Add("2orde");
            DodetijdCMB.Items.Add("geen dodetijd");
            DodetijdCMB.Items.Add("klein beetje dodetijd");
            DodetijdCMB.Items.Add("wat dodetijd");
            DodetijdCMB.Items.Add("gemiddeld dodetijd");
            DodetijdCMB.Items.Add("veel dodetijd");
        }
       private void Proces(object sender, EventArgs e)
       {
            double value;
            if(_run)
            {
               
                StartGraph();
                
                value = RGLR.Berekening();
                DataAddY(value);
                value = proces.Proces(value);
                RGLR.X = value;
                DataAddX(value);
                Dispatcher.BeginInvoke(new Action(() => UpdateGraph()));
            }
        }
        #endregion
    }
}
