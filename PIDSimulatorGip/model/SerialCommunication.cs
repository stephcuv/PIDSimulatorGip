using PIDSimulatorGip.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;


namespace PIDSimulatorGip.model
{
    class SerialCommunication : ViewModelBase
    {
        #region variables
        private SerialPort? _port;
        private string[]? _serPort;
        private string _chosenPort = string.Empty;
        private string _sendData = string.Empty;
        private string _incomeData = string.Empty;
        private bool _connected = false;

        public event EventHandler<string> DataReceived;

        public string[] SerPort{ get { return _serPort; }set { _serPort = value; }}
        public string ChosenPort{get { return _chosenPort; }set { _chosenPort = value; }}
        public bool Connected
        {get { return _connected;  } private set { _connected = value; }}
        public string IncomeData { get { return _incomeData; } set{ _incomeData = value; OnPropertyChanged(); }}
        public string SendData {get { return _sendData; }set { if (Connected && value != string.Empty) { _sendData = value; } }}
        #endregion

        #region serial connection functions

        public void FindAllPorts()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM WIN32_PnPEntity");
                var ports = new List<string>();

                foreach (ManagementObject obj in searcher.Get())
                {
                    try
                    {
                        string caption = obj["Caption"]?.ToString();
                        string name = obj["Name"]?.ToString();

                        if (!string.IsNullOrEmpty(caption) && caption.Contains("(COM") && caption.Contains("Arduino"))
                        {
                            string comPort = ExtractCOMPortName(name);
                            if (!string.IsNullOrEmpty(comPort))
                            {
                                ports.Add(comPort);
                                Console.WriteLine(comPort);
                            }
                        }
                    }
                    catch
                    {
                        Debug.WriteLine("fout in foreach findallports");
                    }
                }
                if(ports.Count > 0) _chosenPort = ports[0];
            }
            catch
            {
                Debug.WriteLine("fout in findallports try");
            }
        }

        static string ExtractCOMPortName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            int startIndex = name.LastIndexOf('(');
            int endIndex = name.LastIndexOf(')');

            if (startIndex >= 0 && endIndex > startIndex)
            {
                return name[(startIndex + 1)..endIndex];
            }

            return null;
        }

        public void ConnectSerPort()
        {
            try
            {
                FindAllPorts();

                _port = new SerialPort(_chosenPort);
                _port.BaudRate = 9600;
                _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                _port.Open();
                Console.WriteLine("trying to connect");
                if (_port.IsOpen)
                {
                    Connected = true;
                    Debug.WriteLine("connected");
                }
            }
            catch (ArgumentException)
            {
                Debug.WriteLine("opgegeven poort bestaat niet");
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("poort in gebuik");
            }
            catch (Exception)
            {
                Debug.WriteLine("er liep iets fout tijdens verbinden");
            }
        }
        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (Connected)
            {
                try { IncomeData = _port.ReadLine(); DataReceived.Invoke(this, IncomeData); }
                catch (TimeoutException) { Debug.WriteLine("timeout"); }
                catch (Exception) { Debug.WriteLine("fout tijdens data receiven"); }
            }
        }
        public void SendSerialData(string data)
        {
            if (Connected)
            {
                try { _port.WriteLine(data); }
                catch { }
            }
        }
        public void DisconnectSerPort()
        {
            try
            {
                _port.Close();
                if (!_port.IsOpen)
                {
                    Connected = false;
                    Console.WriteLine("disconected");
                    _port.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);

                }
            }
            catch { }
        }

        #endregion
    }
}

