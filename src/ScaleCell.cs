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
        public float weight = -1;

        private static MTT mtt = (MTT)Application.OpenForms["MTT"];
        private static SerialPort _serialPort = new SerialPort("COM2", 9600, Parity.Even, 7, StopBits.Two);

        private static Timer _timer = new Timer();
        private static Timer _jidaTimer = new Timer();

        private static UcLoadcell _ucLoadcell;
        private static UsbDevice _evoLinePrinter;

        public static bool enabled = false; 
        public static float netWeight = -1; 
        public static float tareWeight = -1; 

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
                    _timer.Interval = 500;
                    _timer.Tick += new EventHandler(timer_Tick);
                    _timer.Start();

                    // Setup jida board reopen timer
                    _jidaTimer.Interval = 30000;
                    _jidaTimer.Tick += new EventHandler(timer_Tick);
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
            try
            {
                // example string:
                // SX  G      0.000 kg  N      0.000 kg  T      0.000 kg

                //more info: https://www.waagen-forum.de/forum/forum/thread/7837-mettler-toledo-uc3-htt-p-mit-eigener-ansteuerung/ 


                //int address = (byte) data.Substring(0, 1);
                string command = data.Substring(0, 3);
                //mtt.logToBox($"Command: {command}");
                if (command == "SXI" || command == "SXD")
                {
                    mtt.logToBox("instable");
                    return;
                }
                string netWeightString = data.Substring(21, 16).Trim();
                //mtt.logToBox($"netWeightString: {netWeightString}");
                string tareWeightString = data.Substring(38, 16).Trim();
                //mtt.logToBox($"tareWeightString: {tareWeightString}");


                if (netWeightString[0]!='N' || tareWeightString[0]!='T')
                {
                    throw new Exception("Parse error");
                }

                netWeightString = System.Text.RegularExpressions.Regex.Split(netWeightString, @"\s+")[1];
                tareWeightString = System.Text.RegularExpressions.Regex.Split(tareWeightString, @"\s+")[1];

                //object weights = new object[]
                //{
                //};

                netWeight = float.Parse(NormalizeDecimal(netWeightString));
                tareWeight = float.Parse(NormalizeDecimal(tareWeightString));
                Weights w = new Weights(netWeight, tareWeight);

                mtt.logToBox(data.Trim());
                mtt.SetWeights(w);
                //mtt.BeginInvoke(new SetTextDeleg(mtt.si_DataReceived), new object[] { data });
            }
            catch (Exception ex)
            {
                mtt.logToBox($"Error: parsing weight: {ex.Message}");
            }

        }

        private static void timer_Tick(object sender, EventArgs e)
        {
            byte[] bytestosend = { 0x06, 0x53, 0x58, 0x49, 0x0d, 0x0a };
            _serialPort.Write(bytestosend, 0, bytestosend.Length);
        }

        private void jidaTimer_Tick(object sender, EventArgs e)
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
