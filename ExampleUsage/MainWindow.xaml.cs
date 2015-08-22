using System;
using System.IO.Ports;
using System.Windows;

namespace ExampleUsage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly SendSms.SendSms _smsEngine = new SendSms.SendSms();
        private SerialPort _port = null;
        public MainWindow()
        {
            InitializeComponent();
            var ports = SerialPort.GetPortNames();

            // Add all port names to the combo box:
            foreach (var port in ports)
            {
                AvaillablePorts.Items.Add(port);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_port != null)
            {
                try
                {
                    MessageBox.Show(_smsEngine.SendMessage(_port, MobileNumber.Text, Message.Text)? "Message was sent successfuly": "Message not Sent");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("You must open a port first inorder to send a message");
            }
        }

        private void OpenPort_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _port = _smsEngine.OpenPort(AvaillablePorts.Text);
                if (_port != null)
                {
                    PortStatus.Text = "Port is open at: " + AvaillablePorts.Text;
                }
                else
                {
                    PortStatus.Text = "Invalid Port Settings: " + AvaillablePorts.Text;
                }

            }
            catch (Exception ex)
            {
                PortStatus.Text = ex.Message;
            }
        }

        private void ClosePort_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _smsEngine.ClosePort(_port);
                PortStatus.Text = "Not Open!";
            }
            catch (Exception ex)
            {
                PortStatus.Text = ex.Message;
            }
        }
    }
}
