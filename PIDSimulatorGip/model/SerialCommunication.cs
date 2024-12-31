using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PIDSimulatorGip.model
{
    class SerialCommunication
    {
        #region variables
        private SerialPort? _port;
        private string[]? _serPort;
        private string _chosenPort = string.Empty;
        private string _sendData = string.Empty;
        private bool _connected = false;

        public string[] SerPort
        {
            get { return _serPort; }
            set { _serPort = value; }
        }
        public string ChosenPort
        {
            get { return _chosenPort; }
            set { _chosenPort = value; }
        }
        public bool Connected
        {
            get { return _connected; }
            private set { _connected = value; }
        }
        public string IncomeData
        {
            get { return _sendData; }
            set { _sendData = value; }
        }
        public string SendData
        {
            get { return _sendData; }
            set { if (Connected && value != string.Empty) { _sendData = value; } }
        }
        #endregion

        #region serial connection functions

        static string[] FindAllPorts()
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
                        string description = obj["Description"]?.ToString();

                        if (!string.IsNullOrEmpty(caption) && caption.Contains("(COM") && !string.IsNullOrEmpty(name))
                        {
                            string comPort = ExtractCOMPortName(name);
                            if (!string.IsNullOrEmpty(comPort))
                            {
                                ports.Add($"{comPort}\n{description}");
                            }
                        }
                    }
                    catch
                    {
                        // Skip errors for individual objects
                    }
                }

                return ports.ToArray();
            }
            catch
            {
                return Array.Empty<string>();
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


        public void AllPorts()
        {
            SerPort = SerialPort.GetPortNames();
        }
        public void ConnectSerPort()
        {
            try
            {
                _port = new SerialPort(_chosenPort);
                _port.BaudRate = 9600;
                _port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                _port.Open();
                if (_port.IsOpen)
                {
                    Connected = true;
                }
            }
            catch (ArgumentException)
            {

            }
            catch (UnauthorizedAccessException)
            {

            }
            catch (Exception)
            {

            }
        }
        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (Connected)
            {
                try
                {
                    IncomeData = _port.ReadLine();
                }
                catch (TimeoutException)
                {
                }
                catch (Exception)
                { }
            }
        }
        public void SendSerialData()
        {
            if (Connected)
            {
                try
                {
                    _port.WriteLine(SendData);
                }
                catch
                {

                }
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
                }
            }
            catch { }
        }

        #endregion
    }
}
