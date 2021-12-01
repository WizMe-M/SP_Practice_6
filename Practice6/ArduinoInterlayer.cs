using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using RJCP.IO.Ports;

namespace Practice6
{
    public class ArduinoInterlayer
    {
        public Action InvasionHappened;
        public readonly int SubstationCount;
        private readonly SerialPortStream _port;
        
        public ArduinoInterlayer()
        {
            _port = new SerialPortStream();
            _port.BaudRate = 9600;
            _port.DataBits = 8;
            _port.Parity = Parity.None;
            _port.StopBits = StopBits.One;
            _port.Encoding = Encoding.ASCII;
            _port.ReadTimeout = 1000;
            _port.WriteTimeout = 1000;
            _port.DtrEnable = true;
            _port.RtsEnable = true;
            
            SubstationCount = 2;
        }

        public bool IsArduinoConnected => _port.IsOpen;

        public static IEnumerable<string> GetPortNames() => SerialPortStream.GetPortNames();

        public void ConnectToPort(string portName)
        {
            _port.PortName = portName;
            _port.DataReceived += OnDataReceived;
            _port.ErrorReceived += OnErrorReceived;
            _port.Open();
        }

        public void DisconnectFromPort()
        {
            _port.DataReceived -= OnDataReceived;
            _port.ErrorReceived -= OnErrorReceived;
            _port.Close();
        }

        public void LightLed(int substationIndex)
        {
            _port.Write("light" + substationIndex);
        }

        public void TurnOffLed(int substationIndex)
        {
            _port.Write("turnoff" + substationIndex);
        }

        public void EnableSignalling()
        {
            _port.Write("enableSignal");
        }

        public void DisableSignalling()
        {
            _port.Write("disableSignal");
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs _)
        {
            var port = (SerialPortStream)sender;
            var data = port.ReadLine();
            data = data?.Trim().ToLower();
            Debug.WriteLine(data, "port output");

            if (data == "invasion")
            {
                InvasionHappened?.Invoke();
            }
        }

        private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs _)
        {
            var port = (SerialPortStream)sender;
            var error = port.ReadLine().Trim().ToLower();
            Debug.WriteLine(error, "port errors");
        }
    }
}