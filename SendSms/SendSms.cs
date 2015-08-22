using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace SendSms
{
    public class SendSms
    {
        public SerialPort OpenPort(string portName)
        {
            _receiveNow = new AutoResetEvent(false);
            var port = new SerialPort();
            try
            {
                port.PortName = portName;
                port.StopBits = StopBits.One;
                port.Parity = Parity.None;
                port.Encoding = Encoding.GetEncoding("iso-8859-1");
                port.DataReceived += ReceiveData;
                port.Open();
                port.DtrEnable = true;
                port.RtsEnable = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return port;
        }

        public void ClosePort(SerialPort port)
        {
            try
            {
                port.Close();
                port.DataReceived -= ReceiveData;
                port = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private AutoResetEvent _receiveNow;

        private string ReadResponse(SerialPort port, int timeout)
        {
            var buffer = string.Empty;
            try
            {
                do
                {
                    if (_receiveNow.WaitOne(timeout, false))
                    {
                        var t = port.ReadExisting();
                        buffer += t;
                    }
                    else
                    {
                        if (buffer.Length > 0)
                            throw new ApplicationException("Response received is incomplete.");
                        else
                            throw new ApplicationException("No data received from phone.");
                    }
                }
                while (!buffer.EndsWith("\r\nOK\r\n") && !buffer.EndsWith("\r\n> ") && !buffer.EndsWith("\r\nERROR\r\n"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return buffer;
        }

        private void ReceiveData(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                {
                    _receiveNow.Set();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string ExecuteATCommand(SerialPort port, string command, int responseTimeout, string errorMessage)
        {
            try
            {
                port.DiscardOutBuffer();
                port.DiscardInBuffer();
                _receiveNow.Reset();
                port.Write(command + "\r");

                var input = ReadResponse(port, responseTimeout);
                if ((input.Length == 0) || ((!input.EndsWith("\r\n> ")) && (!input.EndsWith("\r\nOK\r\n"))))
                    throw new ApplicationException("No success message was received.");
                return input;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }   


        public bool SendMessage(SerialPort port, string phoneNumber, string messageText)
        {
            var isSend = false;

            try
            {
                var recievedData = ExecuteATCommand(port, "AT", 300, "No phone connected");
                recievedData = ExecuteATCommand(port, "AT+CMGF=1", 300, "Failed to set message format.");
                var command = "AT+CMGS=\"" + phoneNumber + "\"";
                recievedData = ExecuteATCommand(port, command, 300, "Failed to accept phoneNo");
                command = messageText + char.ConvertFromUtf32(26) + "\r";
                recievedData = ExecuteATCommand(port, command, 3000, "Failed to send message"); //3 seconds
                if (recievedData.EndsWith("\r\nOK\r\n"))
                {
                    isSend = true;
                }
                else if (recievedData.Contains("ERROR"))
                {
                    isSend = false;
                }
                return isSend;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
