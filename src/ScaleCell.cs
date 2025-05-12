using System;
using System.IO.Ports;
using System.IO;
using LibUsbDotNet;
using System.Windows.Forms;
using LibUsbDotNet.Main;

namespace MTT
{

    public class ScaleCell
    {
        public decimal weight = -1;

        private static MTT mtt = (MTT)Application.OpenForms["MTT"];
        private static SerialPort _serialPort = new SerialPort("COM2", 9600, Parity.Even, 7, StopBits.Two);

        private static Timer _timer = new Timer();
        private static Timer _jidaTimer = new Timer();

        private static UcLoadcell _ucLoadcell;

        public static bool enabled = false; 

        public static decimal grossWeight = -1; 
        public static decimal netWeight = -1; 
        public static decimal tareWeight = -1; 

        public static void init()
        {
            try
            {
                // Open serial port
                _serialPort.RtsEnable = false;
                _serialPort.DtrEnable = false;
                _serialPort.Handshake = Handshake.None;
                _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                _serialPort.Open();
                //eventBox.Items.Insert(0, "Open serial scale port");

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
                    _timer.Interval = 200;
                    _timer.Tick += new EventHandler(timer_Tick);
                    _timer.Start();

                    // Setup jida board reopen timer
                    _jidaTimer.Interval = 30000;
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
                //eventBox.Items.Insert(0, "Close serial scale port");

                //mtt.nullScaleBtn.Enabled = false;
                //mtt.tareScaleBtn.Enabled = false;
                //mtt.closeScaleBtn.Enabled = false;

            }
            catch(Exception ex)
            {
                mtt.logToBox($"Error when closing scale: {ex.Message}");
            }
        }

        public static void nullIt() {
            byte[] bytestosend = { 0x06, 0x5A, 0x0d, 0x0a };
            _serialPort.Write(bytestosend, 0, bytestosend.Length);
            mtt.logToBox("Set scale to null");
        }

        private static string NormalizeDecimal(string value)
        {
            return System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator == "." ? value.Replace(',', '.') : value.Replace('.', ',');
        }


        private static void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int address = _serialPort.ReadByte();
            string data = _serialPort.ReadLine();
            if (data[0] == 'S')
            {
                data = data.Substring(0, data.Length - 1);
            } else { 
                data = data.Substring(1, data.Length - 2);
            }
            // example string:
            // SX  G      0.000 kg  N      0.000 kg  T      0.000 kg

            //more info: https://www.waagen-forum.de/forum/forum/thread/7837-mettler-toledo-uc3-htt-p-mit-eigener-ansteuerung/ 

            mtt.logToBox(data);

            //Weights w = null;

            try
            {
                //int address = (byte) data.Substring(0, 1);
                string command = data.Substring(0, 3).Trim();
                //mtt.logToBox($"Command: {command}");

                bool instable = false;

                if (command == "SXI")
                {
                    mtt.logToBox("instable");
                }
                else
                {
                    if (command == "SXD")
                    {
                        instable = true;

                    }
                    else if (command == "SX")
                    {

                        //string netWeightString = System.Text.RegularExpressions.Regex.Split(data, @"\s+")[1];
                        //mtt.logToBox($"netWeightString: {netWeightString}");
                        //string tareWeightString = data.Substring(38, 16).Trim();
                        //mtt.logToBox($"tareWeightString: {tareWeightString}");

                        //if (netWeightString[0]!='N' || tareWeightString[0]!='T')
                        //{
                        //    throw new Exception("Parse error");
                        //}

                        string grossWeightString = System.Text.RegularExpressions.Regex.Split(data, @"\s+")[5];
                        string netWeightString = System.Text.RegularExpressions.Regex.Split(data, @"\s+")[5];
                        string tareWeightString = System.Text.RegularExpressions.Regex.Split(data, @"\s+")[8];

                        netWeight = decimal.Parse(NormalizeDecimal(netWeightString));
                        tareWeight = decimal.Parse(NormalizeDecimal(tareWeightString));
                        grossWeight = decimal.Parse(NormalizeDecimal(grossWeightString));

                        //w = new Weights(netWeight, tareWeight, grossWeight, instable);

                        mtt.SetWeights(netWeight, tareWeight, grossWeight, instable);
                    }
                }

            } catch (Exception ex) {
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
