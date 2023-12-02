using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLH4S
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CollectAndFillIPv4Addresses();
        }

        /// <summary>
        /// Test button click actions
        /// </summary>
        private async void button2_Click(object sender, EventArgs e)
        {
            var sitHost = textBox1.Text.Trim();
            Console.WriteLine("PortKnocking " + sitHost);
            richTextBox1.Clear();
            richTextBox1.AppendText("[ Testing connection... please wait.. ]");
            disableFloodAsync();
            var port6969Open = await isPortOpenAsync(sitHost, 6969);
            var port6970Open = await isPortOpenAsync(sitHost, 6970);

            // Clear previous text
            richTextBox1.Clear();
            // Append colored text
            AppendColoredText("Port 6969 (AKI): ", GetColorForStatus(port6969Open));
            AppendColoredText(port6969Open, GetColorForStatus(port6969Open));
            richTextBox1.AppendText("\r\n"); // Newline

            AppendColoredText("Port 6970 (SIT): ", GetColorForStatus(port6970Open));
            AppendColoredText(port6970Open, GetColorForStatus(port6970Open));
        }

        private Color GetColorForStatus(string status)
        {
            return status == "AVAILABLE" ? Color.Green : Color.Red;
        }


        private void AppendColoredText(string text, Color color)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;

            richTextBox1.SelectionColor = color;
            richTextBox1.AppendText(text);
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button3.Hide();
            button1.Visible = true;
        }

        /// <summary>
        /// disables the Test button for 9s
        /// </summary>
        /// <returns></returns>
        private async Task disableFloodAsync()
        {
            button2.Enabled = false;
            await Task.Delay(9000);
            button2.Enabled = true;
        }
        /// <summary>
        /// the port knocker itself
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private async Task<string> isPortOpenAsync(string host, int port)
        {
            try
            {
                using (TcpClient tester = new TcpClient())
                {
                    await tester.ConnectAsync(host, port);
                    tester.NoDelay = true;
                    tester.SendTimeout = 2500;
                    tester.ReceiveTimeout = 2500;
                    return "AVAILABLE";
                }
            }
            catch (SocketException)
            {
                return "NOT AVAILABLE";
            }
            catch (Exception)
            {
                return "NOT AVAILABLE";
            }
        }

        /// <summary>
        /// Collect the IPs from System, and puts it in the localips textbox
        /// </summary>
        private void CollectAndFillIPv4Addresses()
        {
            foreach (IPAddress ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // Check if it's IPv4
                {
                    localips.Items.Add(ip.ToString());
                }
            }
        }
        /// <summary>
        /// sends the selected ip to clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (localips.SelectedItem != null)
            {
                Clipboard.SetText(localips.SelectedItem.ToString());
            }
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            textBox3.ReadOnly = false;
            textBox3.Enabled = true;
        }
    }
}
