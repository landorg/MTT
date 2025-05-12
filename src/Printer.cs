using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using LibUsbDotNet;
using LibUsbDotNet.Main;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace MTT
{
    public class Printer
    {

        private static bool printer_disabled = false;

        private static Printer instance = null;
        private static readonly object padlock = new object();

        private static UsbDevice _evoLinePrinter;

        public bool enabled = false;


        private Printer()
        {
            MTT mtt = (MTT)Application.OpenForms["MTT"];

            try
            {
                if (!printer_disabled)
                {

                    _evoLinePrinter = UsbDevice.OpenUsbDevice(new UsbDeviceFinder(0x0EB8, 0x3000));
                    _evoLinePrinter.Open();

                    IUsbDevice wholeUsbDevice = _evoLinePrinter as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        wholeUsbDevice.SetConfiguration(1);

                        wholeUsbDevice.ClaimInterface(0);
                    }

                    mtt.logToBox("Open usb device printer", "INFO");

                    this.enabled = true;

                }
            }
            catch (Exception ex)
            {
                mtt.logToBox($"Failed to open usb device printer: {ex.Message}", "ERROR");
            }
        }

        public static Printer Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Printer();
                    }
                    return instance;
                }
            }
        }

        internal void feedLabel()
        {
            MTT mtt = (MTT)Application.OpenForms["MTT"];
            try
            {
                var evoEndpointWriter = _evoLinePrinter.OpenEndpointWriter(WriteEndpointID.Ep02, EndpointType.Bulk);
                evoEndpointWriter.Write(new byte[] { 0x01, 0x1b, 0x53 }, 1000, out int outer);
                evoEndpointWriter.Dispose();
                mtt.logToBox("Feed printer", "INFO");
            }
            catch (Exception ex)
            {
                mtt.logToBox($"Failed to feed printer {ex.Message}", "ERROR");
            }
        }

        internal void close()
        {
            IUsbDevice wholeUsbDevice = _evoLinePrinter as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                wholeUsbDevice.ReleaseInterface(0);
            }
            _evoLinePrinter.Close();
            UsbDevice.Exit();

            enabled = false;
        }

        internal void PrintReciept(ListView table, decimal sum)
        {
            int lenght;
            byte[] data = new byte[1024];
            ErrorCode errorCode;
            int timeout = 500;

            MTT mtt = (MTT)Application.OpenForms["MTT"];

            try
            {
                // Create label
                var bitmap = BitmapConverter.DrawReciept(table, sum, 680);
                //var bitmap = BitmapConverter.DrawReciept();

                bitmap.Save("C:/MTT/debug.bmp");


                if (!printer_disabled)
                {
                    // Open read and writer endpoint
                    var evoEndpointWriter = _evoLinePrinter.OpenEndpointWriter(WriteEndpointID.Ep02, EndpointType.Bulk);
                    var evoEndpointReader = _evoLinePrinter.OpenEndpointReader(ReadEndpointID.Ep02);

                    bitmap = BitmapConverter.BitmapTo1Bpp2(bitmap);

                    var command = BitmapConverter.Convert(bitmap);

                    var arrays = Utils.Split(command, 4000);

                    errorCode = evoEndpointWriter.Write(new byte[] { 0x01, 0x1b, 0x64, 0x31, 0x31, 0x31, 0x32 }, timeout, out lenght);

                    evoEndpointReader.Read(data, timeout, out lenght);
                    mtt.logToBox($"Write status: {errorCode} + {Utils.ByteArrayToString(data)}");

                    errorCode = evoEndpointWriter.Write(new byte[] { 0x02, 0x1b, 0x5d, 0x30, 0x31, 0x36, 0x34, 0x38 }, timeout, out lenght);
                    evoEndpointReader.Read(data, timeout, out lenght);
                    mtt.logToBox($"Write status: {errorCode} + {Utils.ByteArrayToString(data)}");

                    errorCode = evoEndpointWriter.Write(new byte[] { 0x03, 0x1b, 0x7e }, timeout, out lenght);
                    evoEndpointReader.Read(data, timeout, out lenght);
                    mtt.logToBox($"Write status: {errorCode} + {Utils.ByteArrayToString(data)}");

                    errorCode = evoEndpointWriter.Write(new byte[] { 0x04, 0x1b, 0x5a, 0x0d, 0x0a }, timeout, out lenght);
                    evoEndpointReader.Read(data, timeout, out lenght);
                    mtt.logToBox($"Write status: {errorCode} + {Utils.ByteArrayToString(data)}");

                    errorCode = evoEndpointWriter.Write(new byte[] { 0x05, 0x1b, 0x5a }, timeout, out lenght);
                    evoEndpointReader.Read(data, timeout, out lenght);
                    mtt.logToBox($"Write status: {errorCode} + {Utils.ByteArrayToString(data)}");

                    errorCode = evoEndpointWriter.Write(new byte[] { 0x06, 0x1b, 0x5a }, timeout, out lenght);
                    evoEndpointReader.Read(data, timeout, out lenght);
                    mtt.logToBox($"Write status: {errorCode} + {Utils.ByteArrayToString(data)}");

                    errorCode = evoEndpointWriter.Write(new byte[] { 0x07, 0x1b, 0xbe }, timeout, out lenght);
                    evoEndpointReader.Read(data, timeout, out lenght);
                    mtt.logToBox($"Write status: {errorCode} + {Utils.ByteArrayToString(data)}");
                    errorCode = evoEndpointWriter.Write(new byte[] {
                            0x08, 0x1b, 0x57, // command code
                            0x34, 0x33, 0x32, // width 432
                            0x30, 0x36, 0x38, 0x30 // length 680
                    }, timeout, out lenght);


                    evoEndpointReader.Read(data, timeout, out lenght);
                    mtt.logToBox($"Write status: {errorCode} + {Utils.ByteArrayToString(data)}");

                    foreach (var array in arrays)
                    {
                        errorCode = evoEndpointWriter.Write(array, timeout, out lenght);
                        evoEndpointReader.Read(data, timeout, out lenght);
                        mtt.logToBox($"Write label status: {errorCode} + {Utils.ByteArrayToString(data)}");
                        if (errorCode == ErrorCode.IoTimedOut)
                        {
                            evoEndpointWriter.Reset();
                        }
                    }

                    // Close writer
                    evoEndpointWriter.Reset();
                    evoEndpointWriter.Abort();
                    evoEndpointWriter.Dispose();

                    // Close reader
                    evoEndpointReader.Reset();
                    evoEndpointReader.Abort();
                    evoEndpointReader.Dispose();
                }

                mtt.logToBox("Print test label evo line printer");
            }
            catch (Exception ex)
            {
                mtt.logToBox($"Error: {ex.Message}");
            }
        }
    }
}
