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
        SerialPort _port= new SerialPort();
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
            _port = _smsEngine.OpenPort(AvaillablePorts.Text);
            if (_port != null)
            {
                try
                {
                    MessageBox.Show(_smsEngine.SendMessage(_port, MobileNumber.Text, Message.Text)
                        ? "Message was sent successfuly"
                        : "Message not Sent");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Could not connect with Modem.");
            }
        }
    }
}
