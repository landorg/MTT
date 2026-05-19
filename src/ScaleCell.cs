using System;
using System.IO.Ports;
using System.IO;
using LibUsbDotNet;
using System.Windows.Forms;
using LibUsbDotNet.Main;

namespace MTTApp
{

    public class ScaleCell
    {
        public decimal weight = -1;

        private static MTT mtt = (MTT)Application.OpenForms["MTT"];
        private static SerialPort _serialPort = new SerialPort("COM2", 9600, Parity.Even, 7, StopBits.Two);

        private static Timer _timer;
        private static Timer _jidaTimer;

        private static UcLoadcell _ucLoadcell;

        public static bool enabled = false;
        private static bool _initialized = false;

        public static decimal grossWeight = -1;
        public static decimal netWeight = -1;
        public static decimal tareWeight = -1;

        public static void init()
        {
            if (_initialized) return;
            _initialized = true;
            try
            {
                // Open serial port
                _serialPort.RtsEnable = false;
                _serialPort.DtrEnable = false;
                _serialPort.Handshake = Handshake.None;
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                _serialPort.Open();

                // Open jida board
                var initializeStatus = JiddaWrapper.JidaDllInitialize();
                if (initializeStatus && JiddaWrapper.JidaDllIsAvailable())
                {
                    IntPtr handle = IntPtr.Zero;
                    JiddaWrapper.JidaBoardOpenByNameA("UUP6", ref handle);
                    _ucLoadcell = new UcLoadcell(handle);
                    _ucLoadcell.ReOpenBoardCommunication();

                    // Send inital command for loadcell
                    byte[] bytestosend = { 0x16, 0x1B, 0x3F };
                    _serialPort.Write(bytestosend, 0, bytestosend.Length);

                    // Setup weight reading timer
                    _timer = new Timer { Interval = 200 };
                    _timer.Tick += new EventHandler(timer_Tick);
                    _timer.Start();

                    // Setup jida board reopen timer
                    _jidaTimer = new Timer { Interval = 30000 };
                    _jidaTimer.Tick += new EventHandler(jidaTimer_Tick);
                    _jidaTimer.Start();

                    enabled = true;
                }
                else
                {
                    mtt.logToBox("No access to jida board. Open application as administrator");
                }
            }
            catch (Exception ex)
            {
                mtt.logToBox($"Open scale failed {ex.Message}");
            }
        }


        public static void close()
        {
            if (!_initialized) return;
            _initialized = false;
            try
            {
                // Stop weight reading
                _timer.Stop();
                _timer.Dispose();

                // Close Jida
                JiddaWrapper.JidaDllUninitialize();
                _jidaTimer.Stop();
                _jidaTimer.Dispose();

                // Close serial port
                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(sp_DataReceived);
                _serialPort.Dispose();
                _serialPort.Close();
                mtt.logToBox("Close serial scale port");
            }
            catch (Exception ex)
            {
                mtt.logToBox($"Error when closing scale: {ex.Message}");
            }
        }

        public static void nullIt()
        {
            byte[] bytestosend = { 0x06, 0x5A, 0x0d, 0x0a };
            _serialPort.Write(bytestosend, 0, bytestosend.Length);
            mtt.logToBox("Set scale to null");
        }

        private static string NormalizeDecimal(string value)
        {
            return System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator == "." ? value.Replace(',', '.') : value.Replace('.', ',');
        }

        // Response format: SX  G      1.600 kg  N      1.600 kg  T      0.000 kg  (stable)
        //                  SXD G      1.600 kg  N      1.600 kg  T      0.000 kg  (dynamic/instable)
        //                  SXI   — invalid value
        //                  SXI-  — underload (scale below zero range)
        //                  SXI+  — overload
        private static void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int address = _serialPort.ReadByte();
            string data = _serialPort.ReadLine();

            if (data[0] == 'S')
            {
                data = data.Substring(0, data.Length - 1);
            }
            else
            {
                if (data.Length < 3) return;
                data = data.Substring(1, data.Length - 2);
            }

            mtt.logToBox(data);

            try
            {
                if (data.Length < 3) return;
                string command = data.Substring(0, 3).Trim();

                if (command == "SX" || command == "SXD")
                {
                    bool instable = command == "SXD";
                    var parts = System.Text.RegularExpressions.Regex.Split(data, @"\s+");
                    if (parts.Length < 9) return;

                    grossWeight = decimal.Parse(NormalizeDecimal(parts[2]));
                    netWeight   = decimal.Parse(NormalizeDecimal(parts[5]));
                    tareWeight  = decimal.Parse(NormalizeDecimal(parts[8]));

                    mtt.SetWeights(netWeight, tareWeight, grossWeight, instable);
                }
                else if (command == "SXI")
                {
                    if (data.Length > 3 && data[3] == '-')
                    {
                        mtt.logToBox("underload");
                        mtt.SetWeightError("-");
                    }
                    else if (data.Length > 3 && data[3] == '+')
                    {
                        mtt.logToBox("overload");
                        mtt.SetWeightError("+");
                    }
                    else mtt.logToBox("invalid");
                }
            }
            catch (Exception ex)
            {
                mtt.logToBox($"Error parsing weight: {ex.Message}");
            }
        }

        private static void timer_Tick(object sender, EventArgs e)
        {
            byte[] bytestosend = { 0x06, 0x53, 0x58, 0x49, 0x0d, 0x0a };
            _serialPort.Write(bytestosend, 0, bytestosend.Length);
        }

        private static void jidaTimer_Tick(object sender, EventArgs e)
        {
            _ucLoadcell.ReOpenBoardCommunication();
        }

        internal static void setTare()
        {
            byte[] bytestosend = { 0x06, 0x54, 0x0d, 0x0a };
            _serialPort.Write(bytestosend, 0, bytestosend.Length);
            mtt.logToBox("Tare scale");
        }
    }
}
